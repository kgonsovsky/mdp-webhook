using namespace System.Net

# /webhokList command
param($Request, $TriggerMetadata)

# modules
Import-Module Az.Accounts
Import-module $PSScriptRoot\..\Modules\Az.EventGrid\1.3.0\Az.EventGrid.psm1

# auth
$securePassword = $env:appSecret | ConvertTo-SecureString -AsPlainText -Force 
$cred = new-object -typename System.Management.Automation.PSCredential -argumentlist  $env:appId, $securePassword
Connect-AzAccount -Credential $cred -ServicePrincipal -Tenant $env:tenantId

# subscriptions
$subs = Get-AzEventGridSubscription -ResourceGroupName $env:resourceGroup -TopicName $env:topic

# return
Push-OutputBinding -Name Response -Value ([HttpResponseContext]@{
    StatusCode = [HttpStatusCode]::OK
    Body = $subs.PsEventSubscriptionsList
})