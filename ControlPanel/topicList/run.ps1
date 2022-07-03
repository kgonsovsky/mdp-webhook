using namespace System.Net

# /topicList command
param($Request, $TriggerMetadata)

# modules
Import-Module Az.Accounts
Import-module $PSScriptRoot\..\Modules\Az.EventGrid\1.3.0\Az.EventGrid.psm1

# auth
$securePassword = $env:appSecret | ConvertTo-SecureString -AsPlainText -Force 
$cred = new-object -typename System.Management.Automation.PSCredential -argumentlist  $env:appId, $securePassword
Connect-AzAccount -Credential $cred -ServicePrincipal -Tenant $env:tenantId

# event grid topics
class item
{
    [String]$TopicName
}

$result = [System.Collections.Generic.List[item]]::new()

$topics = Get-AzEventGridTopic -ResourceGroup $env:resourceGroup
foreach ($topic in $topics.PsTopicsList) 
{
    $item = [item]::new()
    $item.TopicName = $topic.TopicName
    $result.Add($item);
}

# return
Push-OutputBinding -Name Response -Value ([HttpResponseContext]@{
    StatusCode = [HttpStatusCode]::OK
    Body = $result
})