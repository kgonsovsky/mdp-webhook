## Demo for mdp webhook proposal.

Launch datahuPublish project. The application sends timed events to Kafka topic, which reach subscribers.
 
## Kafka settings in the /configs folder

## Settings of topics in the /topics folder

The application is pre-configured with 2 independently running kafka servers. One of them is located in Confluent Cloud and running under Azure Kafka Trigger, the other is on a VPS and running under Sink Connector and Azure Function. The source codes and settings for the Event Grid are in the respective projects. Application transform message before send it to client.

Active subscriber: https://mdpwebhooksite.azurewebsites.net/
