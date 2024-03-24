# Use a mcr.microsoft.com/dotnet/sdk:6.0 as base image
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["BlobScanFunctionApp.csproj", "./"]
RUN dotnet restore "BlobScanFunctionApp.csproj"
COPY . .
RUN dotnet build "BlobScanFunctionApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BlobScanFunctionApp.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/azure-functions/dotnet:4.0 AS final
WORKDIR /home/site/wwwroot
COPY --from=publish /app/publish .
