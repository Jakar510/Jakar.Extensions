$packageId = "Jakar.Extensions"
$apiKey = ""

$json = Invoke-WebRequest -Uri "https://api.nuget.org/v3-flatcontainer/$PackageId/index.json" | ConvertFrom-Json

foreach($version in $json.versions)
{
  Write-Host "Unlisting $packageId, Ver $version"
  dotnet nuget delete $packageId $version --source https://api.nuget.org/v3/index.json --non-interactive --api-key $apiKey
}