# Play.Identity

ASP.NET Core Identity project to handle authorization and authenticaton of PlayEconomy microservices. Instead of the usual marriage of 
ASP.NET Core Identity project to use an sql based db, this one will use MongoDB.

## Create and Publish Play.Identity.Contracts nuget package to Github
```powershell
$version=1.0.1
$owner="PlayEcomony-Microservices"
$gh_pat="[PAT HERE]"
dotnet pack src\Play.Identity.Contracts --configuration Release -p:PackageVersion=$version -p:RepositoryUrl=https://github.com/$owner/Play.Identity -o ..\packages

dotnet nuget push ..\packages\Play.Identity.Contracts.$version.nupkg --api-key $gh_pat --source "github"
```

## Build Docker Image
```powershell
$env:GH_OWNER="PlayEcomony-Microservices"
$env:GH_PAT="[PAT HERE]"
docker build --secret id=GH_OWNER --secret id=GH_PAT -t play.identity:$version . 
```
