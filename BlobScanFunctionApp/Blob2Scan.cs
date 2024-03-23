using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using nClam;
using Azure.Storage.Blobs;

namespace Scan.Function
{
    public class Blob2Scan
    {
        [FunctionName("Blob2Scan")]
        public void Run([BlobTrigger("%BLOB_CONTAINER_NAME%/{name}", Connection = "AzureWebJobsStorage")] Stream myBlob, string name, ILogger log)
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name: {name} Size: {myBlob.Length} Bytes");

            // this section Retrieve the ClamAV service details from environment variables 
            string serverName = Environment.GetEnvironmentVariable("CLAMAV_SERVICE_NAME") ?? "clamav-service";
            int serverPort = int.Parse(Environment.GetEnvironmentVariable("CLAMAV_SERVICE_PORT") ?? "3310");

            log.LogInformation($"ClamAV Server Name: {serverName}:{serverPort}");

            try
            {
                // Create a ClamClient instance to interact with the ClamAV service
                var clamClient = new ClamClient(serverName, serverPort);
                
                // Scanning the blob for viruses
                var scanResult = clamClient.SendAndScanFileAsync(myBlob).Result;

                switch (scanResult.Result)
                {
                    case ClamScanResults.Clean:
                        log.LogInformation($"The file: {name} is clean!");
                        break;
                    case ClamScanResults.VirusDetected:
                        log.LogWarning($"Virus Found in file: {name}");
                        foreach (var virus in scanResult.InfectedFiles)
                        {
                            log.LogWarning($"Virus detected: {virus.VirusName}");
                        }

                        // Delete the infected file
                        log.LogWarning($"Deleting infected file: {name}");
                        var blobServiceClient = new BlobServiceClient(Environment.GetEnvironmentVariable("AzureWebJobsStorage"));
                        var containerClient = blobServiceClient.GetBlobContainerClient(Environment.GetEnvironmentVariable("BLOB_CONTAINER_NAME"));
                        var blobClient = containerClient.GetBlobClient(name);
                        blobClient.DeleteIfExists();
                        log.LogWarning($"Deleted file: {name}");

                        break;
                    case ClamScanResults.Error:
                        log.LogError($"Error scanning the file: {name}. Error: {scanResult.RawResult}");
                        break;
                }
            }
            catch (Exception ex)
            {
                log.LogError($"Exception occurred while scanning the file: {name}. Exception: {ex.Message}");
            }
        }
    }
}
