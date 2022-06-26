# Azure Functions profile.ps1
#
# This profile.ps1 will get executed every "cold start" of your Function App.
# "cold start" occurs when:
#
# * A Function App starts up for the very first time
# * A Function App starts up after being de-allocated due to inactivity
#
# You can define helper functions, run commands, or specify environment variables
# NOTE: any variables defined that are not environment variables will get reset after the first execution

# Authenticate with Azure PowerShell using MSI.
# Remove this if you are not planning on using MSI or Azure PowerShell.
if ($env:MSI_SECRET) {
    Disable-AzContextAutosave -Scope Process | Out-Null
    Connect-AzAccount -Identity
}

# Uncomment the next line to enable legacy AzureRm alias in Azure PowerShell.
# Enable-AzureRmAlias

# You can also define functions or aliases that can be referenced in any of your PowerShell functions.

$Global:appId = "d105bb3d-cfe5-4acf-a3bf-450e46a9209f"  
$Global:appSecret = "Aru8Q~Ddc3n2yGxF3CUwGq6qyouej3QLWlELIcEN"
$Global:tenantId="c7223f2c-1ba2-43c8-be7f-57e6e1465036"
$Global:resourceGroup = "test-rg-mdp-webhook"
$Global:subscriptionId="5258beac-d2a1-4e36-8e1b-2d1fbe17450f"
$Global:topic="test-event-grid-mdp-topic"
$Global:storage="testmdpwebhookstorage"

$Global:deadstorage="testmdpwebhookstorage2"
$Global:deadContainer = "deadcontainer"

$Global:prefix = "test-subscription-"
$Global:suffix = "-mdp" 

$securePassword = $appSecret | ConvertTo-SecureString -AsPlainText -Force 
$cred = new-object -typename System.Management.Automation.PSCredential -argumentlist $appId, $securePassword
#Connect-AzAccount -Credential $cred -ServicePrincipal -Tenant $tenantId

$Global:topicId = (Get-AzEventGridTopic -ResourceGroupName $resourceGroup -Name $topic).Id
$Global:storageId = (Get-AzStorageAccount -ResourceGroupName $resourceGroup -Name $storage).Id

$Global:deadlink="/subscriptions/" + $subscriptionId + "/resourceGroups/" + $resourceGroup + "/providers/Microsoft.Storage/storageAccounts/" + $deadstorage + "/blobServices/default/containers/" + $deadContainer