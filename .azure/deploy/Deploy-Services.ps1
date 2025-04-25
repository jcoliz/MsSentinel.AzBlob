param(
    [Parameter(Mandatory=$true)]
    [string]
    $ResourceGroup,
    [Parameter(Mandatory=$true)]
    [string]
    $Location
)

Write-Output "Creating Resource Group $ResourceGroup in $Location"
az group create --name $ResourceGroup --location $Location

Write-Output "Deploying to Resource Group $ResourceGroup"
$result = az deployment group create --name "Deploy-$(Get-Random)" --resource-group $ResourceGroup --template-file $PSScriptRoot\AzDeploy.Bicep\App\containerAppCompleteWeb.bicep --parameters $PSScriptRoot\containerAppCompleteWeb.parameters.json | ConvertFrom-Json

Write-Output "OK"
Write-Output ""

$fqdn = $result.properties.outputs.fqdn.value
$endpointUri = $result.properties.outputs.endpointUri.value
$stream = $result.properties.outputs.stream.value

Write-Output "Container app running."
Write-Output ""

Write-Output "When finished, run:"
Write-Output "az group delete --name $ResourceGroup"
