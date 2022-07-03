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

# topicName
$topicName = $Request.Query.topicName
if (-not $topicName) {
    $topicName = $Request.Body.topicName
}

# event grid subscriptions
class item
{
    [String]$TopicName
    [String]$EventSubscriptionName
}

$result = [System.Collections.Generic.List[item]]::new()

# topics
$topics = Get-AzEventGridTopic -ResourceGroup $env:resourceGroup
foreach ($topic in $topics.PsTopicsList) 
{
    if (-not $topicName -Or $topic.TopicName -eq $topicName)
    {
        # subscriptions
        $subs = Get-AzEventGridSubscription -ResourceGroupName $env:resourceGroup -TopicName $topic.TopicName
        foreach ($sub in $subs.PsEventSubscriptionsList) 
        {
            $item = [item]::new()
            $item.TopicName = $topic.TopicName
            $item.EventSubscriptionName = $sub.EventSubscriptionName
            $result.Add($item);
        }
    }
}

# return
Push-OutputBinding -Name Response -Value ([HttpResponseContext]@{
    StatusCode = [HttpStatusCode]::OK
    Body = $result
})