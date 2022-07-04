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

# event grid subscription stats
$titles = @("DeliverySuccessCount", "DeliveryAttemptFailCount")
 
class item
{
    [String]$TopicName
    [String]$EventSubscriptionName
    [DateTime]$Date
    [int]$DeliverySuccessCount
    [int]$DeliveryAttemptFailCount
}

$result = [System.Collections.Generic.List[item]]::new()

$topics = Get-AzEventGridTopic -ResourceGroup $env:resourceGroup
foreach ($topic in $topics.PsTopicsList) 
{
    $subs = Get-AzEventGridSubscription -ResourceGroupName $env:resourceGroup -TopicName $topic.TopicName
    foreach ($sub in $subs.PsEventSubscriptionsList) 
    {
        $item = [item]::new()
        $item.TopicName = $topic.TopicName
        $item.EventSubscriptionName = $sub.EventSubscriptionName

        foreach ($title in $titles)
        {
            $m = Get-AzMetric -ResourceId $sub.Id -MetricName $title -TimeGrain 01:00:00

            foreach ($dt in $m.Data)
            {
                if ($title -eq "DeliverySuccessCount"){
                   $item.DeliverySuccessCount = $dt.Total
                }
                if ($title -eq "DeliveryAttemptFailCount"){
                   $item.DeliveryAttemptFailCount = $dt.Total
                }
               $item.Date = $dt.TimeStamp
            }

            foreach ($tm in $m.Timeseries)
            {
           
            }
        }
        $result.Add($item);
    }
}

# return
Push-OutputBinding -Name Response -Value ([HttpResponseContext]@{
    StatusCode = [HttpStatusCode]::OK
    Body = $result
})