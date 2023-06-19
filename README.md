# Play.Identity

ASP.NET Core Identity project to handle authorization and authentication for PlayEconomy microservices. Instead of the usual marriage of
ASP.NET Core Identity project with no-SQL based DB, this one will use MongoDB.

## Create and Publish Play.Identity.Contracts NuGet package to GitHub

```powershell
$version="1.0.8"
$owner="PlayEcomony-Microservices"
$gh_pat="[PAT HERE]"
dotnet pack src\Play.Identity.Contracts --configuration Release -p:PackageVersion=$version -p:RepositoryUrl=https://github.com/$owner/Play.Identity -o ..\packages

dotnet nuget push ..\packages\Play.Identity.Contracts.$version.nupkg --api-key $gh_pat --source "github"
```

## Build docker image

```powershell
$env:GH_OWNER="PlayEcomony-Microservices"
$env:GH_PAT="[PAT HERE]"
$acrName="playeconomybkm"
docker build --secret id=GH_OWNER --secret id=GH_PAT -t "$acrName.azurecr.io/play.identity:$version" . 
```

## Run the docker image

```powershell
$adminPass="[password here]"
$cosmosDbConnStr="[CONN STRING HERE]"
$serviceBusConnString="[CONN STRING HERE]"
docker run -it --rm -p 5002:5002 --name identity -e MongoDbSettings__ConnectionString=$cosmosDbConnStr -e ServiceBusSettings__ConnectionString=$serviceBusConnString -e ServiceSettings__MessageBroker="SERVICEBUS" -e IdentitySettings__AdminUserPassword=$adminPass play.identity:$version
```

## Publish docker image to Azure Container Registry

```powershell
az acr login --name $acrName
docker push "$acrName.azurecr.io/play.identity:$version"
```

## Create the Kubernetes namespace

```powershell
$namespace="identity"
kubectl create namespace $namespace
```

## Create the Kubernetes secrets

```powershell
kubectl create secret generic identity-secrets --from-literal=cosmosdb-connectionstring=$cosmosDbConnStr --from-literal=servicebus-connectionstring=$serviceBusConnString --from-literal=admin-password=$adminPass -n $namespace
```

## Create the Kubernetes pod

```powershell
kubectl apply -f .\kubernetes\identity.yaml -n $namespace
```
