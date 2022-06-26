$Global:appId = "d105bb3d-cfe5-4acf-a3bf-450e46a9209f"  
$Global:appSecret = "Aru8Q~Ddc3n2yGxF3CUwGq6qyouej3QLWlELIcEN"
$Global:tenantId="c7223f2c-1ba2-43c8-be7f-57e6e1465036"
$Global:resourceGroup = "test-rg-mdp-webhook"
$Global:subscriptionId="5258beac-d2a1-4e36-8e1b-2d1fbe17450f"
$Global:topic="test-event-grid-mdp-topic"
$Global:storage="testmdpwebhookstorage"

$Global:At1 = "test-event-"
$Global:suffix = "-mdp" 

$Global:deadstorage="testmdpwebhookstorage2"
$Global:deadContainer = "deadcontainer"

$Global:prefix = "test-subscription-"
$Global:suffix = "-mdp" 

$securePassword = $appSecret | ConvertTo-SecureString -AsPlainText -Force 
$cred = new-object -typename System.Management.Automation.PSCredential -argumentlist $appId, $securePassword
Connect-AzAccount -Credential $cred -ServicePrincipal -Tenant $tenantId

$Global:topicId = (Get-AzEventGridTopic -ResourceGroupName $resourceGroup -Name $topic).Id
$Global:storageId = (Get-AzStorageAccount -ResourceGroupName $resourceGroup -Name $storage).Id

$Global:deadlink="/subscriptions/" + $subscriptionId + "/resourceGroups/" + $resourceGroup + "/providers/Microsoft.Storage/storageAccounts/" + $deadstorage + "/blobServices/default/containers/" + $deadContainer


 $topics = Get-AzEventGridTopic -ResourceGroup $Global:resourceGroup
    foreach ($topic in $topics.PsTopicsList) 
    {
        $subs = Get-AzEventGridSubscription -ResourceGroupName $topic.ResourceGroupName -TopicName $topic.TopicName
        foreach ($sub in $subs.PsEventSubscriptionsList) 
        {
            echo $sub
        }
    }