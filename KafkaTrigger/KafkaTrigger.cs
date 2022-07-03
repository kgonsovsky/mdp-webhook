using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Kafka;
using Microsoft.Extensions.Logging;

namespace KafkaTrigger
{
    public class KafkaTrigger
    {
        [FunctionName("test-event-grid-mdp-kafka-trigger-accounts")]
        public void RunAccounts(
            [KafkaTrigger("pkc-epwny.eastus.azure.confluent.cloud:9092BrokerList",
                          "topic.datahub.accounts",
                          Username = "FSWDPEVFDNYLJRRJ",
                          Password = "oPDfLFWA8IKr5cbEZ+0V/Cgrnv1QbeE5gRsM2GBrDUSYe8Bi9JMNZLznlbDYU/my",
                          Protocol = BrokerProtocol.SaslSsl,
                          AuthenticationMode = BrokerAuthenticationMode.Plain,
                          ConsumerGroup = "$Default")] KafkaEventData<string>[] events,
            ILogger log)
        {
            Trigger.Run(events, log);
        }

        [FunctionName("test-event-grid-mdp-kafka-trigger-loyalties")]
        public void RunLoyalties(
            [KafkaTrigger("pkc-epwny.eastus.azure.confluent.cloud:9092BrokerList",
                          "topic.datahub.loyalties",
                          Username = "FSWDPEVFDNYLJRRJ",
                          Password = "oPDfLFWA8IKr5cbEZ+0V/Cgrnv1QbeE5gRsM2GBrDUSYe8Bi9JMNZLznlbDYU/my",
                          Protocol = BrokerProtocol.SaslSsl,
                          AuthenticationMode = BrokerAuthenticationMode.Plain,
                          ConsumerGroup = "$Default")] KafkaEventData<string>[] events,
            ILogger log)
        {
            Trigger.Run(events, log);
        }

        [FunctionName("test-event-grid-mdp-kafka-trigger-reservations")]
        public void RunReservations(
            [KafkaTrigger("pkc-epwny.eastus.azure.confluent.cloud:9092BrokerList",
                          "topic.datahub.reservations",
                          Username = "FSWDPEVFDNYLJRRJ",
                          Password = "oPDfLFWA8IKr5cbEZ+0V/Cgrnv1QbeE5gRsM2GBrDUSYe8Bi9JMNZLznlbDYU/my",
                          Protocol = BrokerProtocol.SaslSsl,
                          AuthenticationMode = BrokerAuthenticationMode.Plain,
                          ConsumerGroup = "$Default")] KafkaEventData<string>[] events,
            ILogger log)
        {
            Trigger.Run(events, log);
        }
    }
}