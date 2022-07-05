$appId = "d105bb3d-cfe5-4acf-a3bf-450e46a9209f"  
$appSecret = "Aru8Q~Ddc3n2yGxF3CUwGq6qyouej3QLWlELIcEN"
$tenantId="c7223f2c-1ba2-43c8-be7f-57e6e1465036"
$resourceGroup = "test-rg-mdp-webhook"
$subscriptionId="5258beac-d2a1-4e36-8e1b-2d1fbe17450f"

$storage="testmdpwebhookstorage"

$securePassword = $appSecret | ConvertTo-SecureString -AsPlainText -Force 
$cred = new-object -typename System.Management.Automation.PSCredential -argumentlist $appId, $securePassword
Connect-AzAccount -Credential $cred -ServicePrincipal -Tenant $tenantId

##$config = (Get-AzAppConfigurationStore -ResourceGroupName $resourceGroup)[0]

#$MyConfig = Get-AzAppConfigurationStoreKeyValue -Store $config.Name -Label Production

#echo $MyConfig

$titles = @("DeliverySuccessCounts", "DeliveryFailureCounts")
 
class item
{
    [String]$TopicName
    [String]$EventSubscriptionName
    [DateTime]$Date
    [int]$DeliverySuccessCounts
    [int]$DeliveryFailureCounts
   
}

$result = [System.Collections.Generic.List[item]]::new()

$topics = Get-AzEventGridTopic -ResourceGroup $resourceGroup
foreach ($topic in $topics.PsTopicsList) 
{

    $subs = Get-AzEventGridSubscription -ResourceGroupName $resourceGroup -TopicName $topic.TopicName
    foreach ($sub in $subs.PsEventSubscriptionsList) 
    {
        $item = [item]::new()
        $item.TopicName = $topic.TopicName
        $item.EventSubscriptionName = $sub.EventSubscriptionName

        foreach ($title in $titles)
        {
            $m = Get-AzMetric -ResourceId $sub.Id -MetricName $title -TimeGrain 01:00:00
            echo $m

            foreach ($dt in $m.Data)
            {
                if ($title -eq "DeliverySuccessCounts"){
                   $item.DeliveryFailureCounts = $dt.Total
                }
                if ($title -eq "DeliveryFailureCounts"){
                   $item.DeliveryFailureCounts = $dt.Total
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



#echo $result







<# 

$resId = "/subscriptions/5258beac-d2a1-4e36-8e1b-2d1fbe17450f/resourceGroups/test-rg-mdp-webhook/providers/Microsoft.EventGrid/topics/test-event-grid-mdp-topic"

Get-AzMetricDefinition -ResourceId $resId


$metric = Get-AzMetric -ResourceId $resId -MetricName "DeliverySuccessCount" -TimeGrain 01:00:00 -DetailedOutput
echo $metric

foreach ($m in $metric.Data)
{
    echo $m
}


foreach ($tm in $metric.Timeseries)
{
    echo $tm
}


$data = @()
#>


<# 


foreach ($res in Get-AzResourceGroup)
{
    $topics = Get-AzEventGridTopic -ResourceGroup $res.ResourceGroupName
    foreach ($topic in $topics.PsTopicsList) 
    {
        $subs = Get-AzEventGridSubscription -ResourceGroupName $topic.ResourceGroupName -TopicName $topic.TopicName
        foreach ($sub in $subs.PsEventSubscriptionsList) 
        {
          echo $sub.Id
         # $def = Get-AzMetricDefinition -ResourceId $sub.Id
         # echo $def
            $metric = Get-AzMetric -ResourceId $sub.Id -MetricName "DeliverySuccessCount" -TimeGrain 00:01:00 
          #echo $metric
            $record=[pscustomobject]@{
                
                    EventSubscriptionName= $sub.EventSubscriptionName;
                    TimeStamp= $metric.TimeStamp;
                    Total= $metric.Total;
                    Count= $metric.Count;
                }
            $data += $record
                
        }
    }
}
#$metric = Get-AzMetric -ResourceId $resId -MetricName "DeadLetteredCount" -TimeGrain 01:00:00 -DetailedOutput

#echo $data

#>