M D P   W E B H O O K   P R O P O S A L.

* Kafka settings in the /configs folder
* Settings of topics in the /topics folder

Project structure

- dataHubublish. Put events to Kafka topic

- mdpKafkaTriggerFunction. Listen for Kafka inside of Azure and post message to Azure Event Grids

- mdpSinkAccpetorFunction. Pushed by Kafka when new message posted to topic and post message to Azure Event Grids

- subscriberRest. Example of stand alone RestApi client for EventGrid.

- subscriberServiceBusQueueTrigger. Example of client via Service Bus Queue (RabbitMQ)

- subscriberEventGrid. Example of client EventGrid to EventGrid.

Run publisher, goto your azure account, check for activity: https://mdpwebhooksite.azurewebsites.net/
