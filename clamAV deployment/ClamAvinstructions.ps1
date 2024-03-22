
#Apply all the YAML files to your AKS cluster:

kubectl apply -f clamav-configmap.yaml
kubectl apply -f clamav-deployment.yaml
kubectl apply -f function-configmap.yaml
kubectl apply -f function-secrets.yaml
kubectl apply -f function-deployment.yaml
kubectl apply -f clamav-pv.yaml
kubectl apply -f clamav-pvc.yaml
##### Replace yourdockerhubusername/azurefunctionimage:latest with the actual path to your Docker image.



#Check that all pods are running and services are functioning

kubectl get all




# Verify the Deployment
kubectl get pods
kubectl describe pod <azure-function-pod-name>
