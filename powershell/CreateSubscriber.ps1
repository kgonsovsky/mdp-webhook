set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser


$clientID = "d105bb3d-cfe5-4acf-a3bf-450e46a9209f"  
$key = "Aru8Q~Ddc3n2yGxF3CUwGq6qyouej3QLWlELIcEN"
$tenant="c7223f2c-1ba2-43c8-be7f-57e6e1465036"
$SecurePassword = $key | ConvertTo-SecureString -AsPlainText -Force 
$cred = new-object -typename System.Management.Automation.PSCredential -argumentlist $clientID, $SecurePassword

# login non-interactive
Connect-AzAccount -Credential $cred -ServicePrincipal -Tenant $tenant 


$containername = "deadcontainer"

$topicid = (Get-AzEventGridTopic -ResourceGroupName "test-rg-mdp-webhook" -Name "test-event-grid-mdp-topic").Id
$storageid = (Get-AzStorageAccount -ResourceGroupName "test-rg-mdp-webhook" -Name "testmdpwebhookstorage").Id
$subscriptionName="test-event-grid-mdp-subscription-local"
$endPoint = "https://vmi901413.contaboserver.net/api/EndPoint"
$deaddest="/subscriptions/5258beac-d2a1-4e36-8e1b-2d1fbe17450f/resourceGroups/test-rg-mdp-webhook/providers/Microsoft.Storage/storageAccounts/testmdpwebhookstorage/blobServices/default/containers/deadcontainer"
New-AzEventGridSubscription -ResourceId $topicid -EventSubscriptionName $subscriptionName -Endpoint $endPoint -DeadLetterEndpoint $deaddest