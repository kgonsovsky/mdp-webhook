using namespace System.Net

# /webhokRemove command
param($Request, $TriggerMetadata)

# modules
Import-Module Az.Accounts
Import-module $PSScriptRoot\..\Modules\Az.EventGrid\1.3.0\Az.EventGrid.psm1

# auth
$securePassword = $env:appSecret | ConvertTo-SecureString -AsPlainText -Force 
$cred = new-object -typename System.Management.Automation.PSCredential -argumentlist  $env:appId, $securePassword
Connect-AzAccount -Credential $cred -ServicePrincipal -Tenant $env:tenantId

# subscriptionName
$subscriptionName = $Request.Query.subscriptionName
if (-not $subscriptionName) {
    $subscriptionName = $Request.Body.subscriptionName
}

# topicName
$topicName = $Request.Query.topicName
if (-not $topicName) {
    $topicName = $Request.Body.topicName
}
if (-not $topicName) {
    $topicName = $env:defaultTopic
}

# endPoint
$endPoint = $Request.Query.endPoint
if (-not $endPoint) {
    $endPoint = $Request.Body.endPoint
}

# process
$statuscode =[HttpStatusCode]::BadRequest
$body="error"
if (-not $endPoint -or -not $subscriptionName) {
    $body = "subscriptName & endPoint fields are mandatory"
    $statuscode=[HttpStatusCode]::BadRequest
} else {
    $subscriptionName = $env:prefix + $subscriptionName + $env:suffix
    Write-Host "Createing subscription ${subscriptionName} with endpint ${endPoint}..."

    $dead_destination="/subscriptions/" + $env:subscriptionId + "/resourceGroups/" + $env:resourceGroup + "/providers/Microsoft.Storage/storageAccounts/" + $env:deadstorage + "/blobServices/default/containers/" + $env:deadContainer
    
    Remove-AzEventGridSubscription -ResourceGroupName $env:resourceGroup -TopicName $topicName -EventSubscriptionName $subscriptionName
    $statuscode =[HttpStatusCode]::OK
    $body = New-AzEventGridSubscription -ResourceGroupName $env:resourceGroup -TopicName $topicName -EventSubscriptionName $subscriptionName -Endpoint $endPoint -DeadLetterEndpoint $dead_destination
}

# return
Push-OutputBinding -Name Response -Value ([HttpResponseContext]@{
    StatusCode = $statuscode
    Body = $body
})
