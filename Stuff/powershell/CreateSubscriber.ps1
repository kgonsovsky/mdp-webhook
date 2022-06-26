& .\config.ps1


$subscriptionName= -join($prefix,"local" ,$suffix)
$endPoint = "https://vmi901413.contaboserver.net/api/EndPoint"

Remove-AzEventGridSubscription -ResourceGroupName $resourceGroup -TopicName $topic -EventSubscriptionName $subscriptionName
New-AzEventGridSubscription -ResourceGroupName $resourceGroup -TopicName $topic -EventSubscriptionName $subscriptionName -Endpoint $endPoint -DeadLetterEndpoint $deadlink