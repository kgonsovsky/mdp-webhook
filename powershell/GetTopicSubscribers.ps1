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
#Connect-AzAccount  -ResourceGroup mdpwebhookgroup
#https://github.com/Azure-Samples/event-grid-dotnet-handle-deadlettered-events/blob/master/event-subscription-with-dead-lettering/Program.cs

#https://github.com/Azure-Samples/event-grid-dotnet-publish-consume-events/blob/master/EGManageTopicsAndEventSubscriptions/EGManageTopicsAndEventSubscriptions/Program.cs
#Account          : gonsovskii.konstantin@coderhuddle.com
#SubscriptionName : Valamar Riviera
#SubscriptionId   : fb0de01f-ba59-4fba-bb03-be345a469de6
#TenantId         : c7223f2c-1ba2-43c8-be7f-57e6e1465036

Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser


$username = "gonsovskii.konstantin@coderhuddle.com"
$SecurePassword = Get-Content "C:\password.txt" | ConvertTo-SecureString
$cred = new-object -typename System.Management.Automation.PSCredential `
     -argumentlist $username, $SecurePassword

Login-AzureRmAccount -Credential $cred


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


