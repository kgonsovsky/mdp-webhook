#https://docs.microsoft.com/en-us/azure/event-grid/query-event-subscriptions
#https://docs.microsoft.com/en-us/azure/templates/microsoft.eventgrid/partnertopics/eventsubscriptions?tabs=bicep
#https://docs.microsoft.com/en-us/azure/event-grid/metrics
#https://www.netreo.com/solutions/cloud-monitoring/microsoft-azure-event-grid/
#https://docs.microsoft.com/en-us/azure/event-grid/edge/monitor-topics-subscriptions
#https://medium.com/microsoftazure/azure-event-grid-the-whole-story-4b7b4ec4ad23
#https://medium.com/microsoftazure/azure-event-grid-the-whole-story-4b7b4ec4ad23
#https://github.com/MicrosoftDocs/azure-docs/issues/60595
#https://docs.microsoft.com/en-us/azure/azure-monitor/essentials/rest-api-walkthrough

#https://docs.nodinite.com/Documentation/LoggingAndMonitoring%2FAzure?doc=/Features/Event%20Grid/Monitoring%20Event%20Grid#fa-border-right-event-grid-topic-subscription

#https://docs.microsoft.com/en-us/samples/azure-samples/event-grid-dotnet-handle-deadlettered-events/event-grid-dotnet-handle-deadlettered-events/

Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
Import-Module Az
Import-Module Az.EventGrid
Import-AzureRmContext -Path "C:\azure.txt"
#Connect-AzAccount  -ResourceGroup mdpwebhookgroup
#https://github.com/Azure-Samples/event-grid-dotnet-handle-deadlettered-events/blob/master/event-subscription-with-dead-lettering/Program.cs

$containername = "testcontainer"

$topicid = (Get-AzEventGridTopic -ResourceGroupName gridResourceGroup -Name demoTopic).Id
$storageid = (Get-AzStorageAccount -ResourceGroupName gridResourceGroup -Name demostorage).Id
echo $topicid
echo $storageid

foreach ($res in Get-AzResourceGroup)
{
    $topics = Get-AzEventGridTopic -ResourceGroup $res.ResourceGroupName
    foreach ($topic in $topics.PsTopicsList) 
    {
        $subs = Get-AzEventGridSubscription -ResourceGroupName $topic.ResourceGroupName -TopicName $topic.TopicName
        foreach ($sub in $subs.PsEventSubscriptionsList) 
        {
            echo $sub
        }
    }
}


