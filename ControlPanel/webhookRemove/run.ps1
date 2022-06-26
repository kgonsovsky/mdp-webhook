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

# process
$statuscode =[HttpStatusCode]::OK
$body="OK"
if (-not $subscriptionName) {
    $body = "subscriptName field is mandatory"
    $statuscode=[HttpStatusCode]::BadRequest
} else {
    $subscriptionName = $env:prefix + $subscriptionName + $env:suffix
    Write-Host "Createing subscription ${subscriptionName} with endpint ${endPoint}..."

    $body = Remove-AzEventGridSubscription -ResourceGroupName $c:resourceGroup -TopicName $env:topic -EventSubscriptionName $subscriptionName
}

# return
Push-OutputBinding -Name Response -Value ([HttpResponseContext]@{
    StatusCode = $statuscode
    Body = $body
})
