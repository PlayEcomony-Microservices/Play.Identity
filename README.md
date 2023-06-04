# Play.Identity

ASP.NET Core Identity project to handle authorization and authentication of PlayEconomy microservices. Instead of the usual marriage of
ASP.NET Core Identity project to use an no-SQL based DB, this one will use MongoDB.

## Create and Publish Play.Identity.Contracts NuGet package to GitHub

```powershell
$version="1.0.3"
$owner="PlayEcomony-Microservices"
$gh_pat="[PAT HERE]"
dotnet pack src\Play.Identity.Contracts --configuration Release -p:PackageVersion=$version -p:RepositoryUrl=https://github.com/$owner/Play.Identity -o ..\packages

dotnet nuget push ..\packages\Play.Identity.Contracts.$version.nupkg --api-key $gh_pat --source "github"
```

## Build docker image

```powershell
$env:GH_OWNER="PlayEcomony-Microservices"
$env:GH_PAT="[PAT HERE]"
docker build --secret id=GH_OWNER --secret id=GH_PAT -t play.identity:$version . 
```

## Run the docker image

```powershell
$adminPass="[password here]"
$cosmosDbConnStr="[CONN STRING HERE]"
docker run -it --rm -p 5002:5002 --name identity -e MongoDbSettings__ConnectionString=$cosmosDbConnStr -e RabbitMQSettings__Host=rabbitmq -e IdentitySettings__AdminUserPassword=$adminPass --network playinfra_default play.identity:$version
```
