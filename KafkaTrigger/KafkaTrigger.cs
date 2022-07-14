using EventGrid;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KafkaTrigger
{
    public class KafkaTrigger
    {
        private readonly MdpSettings _settings;

        public KafkaTrigger(IOptions<MdpSettings> settings)
        {
            _settings = settings.Value;
        }

        //[FunctionName("test-event-grid-mdp-kafka-trigger-accounts")]
        //public void RunAccounts(
        //    [KafkaTrigger("pkc-epwny.eastus.azure.confluent.cloud:9092BrokerList",
        //                  "topic.datahub.accounts",
        //                  Username = "FSWDPEVFDNYLJRRJ",
        //                  Password = "oPDfLFWA8IKr5cbEZ+0V/Cgrnv1QbeE5gRsM2GBrDUSYe8Bi9JMNZLznlbDYU/my",
        //                  Protocol = BrokerProtocol.SaslSsl,
        //                  AuthenticationMode = BrokerAuthenticationMode.Plain,
        //                  ConsumerGroup = "$Default")] KafkaEventData<string> events,
        //    ILogger log)
        //{
        //    Trigger.Run(new KafkaEventData<string>[]{events}, _settings, log);
        //}

        //[FunctionName("test-event-grid-mdp-kafka-trigger-loyalties")]
        //public void RunLoyalties(
        //    [KafkaTrigger("pkc-epwny.eastus.azure.confluent.cloud:9092BrokerList",
        //                  "topic.datahub.loyalties",
        //                  Username = "FSWDPEVFDNYLJRRJ",
        //                  Password = "oPDfLFWA8IKr5cbEZ+0V/Cgrnv1QbeE5gRsM2GBrDUSYe8Bi9JMNZLznlbDYU/my",
        //                  Protocol = BrokerProtocol.SaslSsl,
        //                  AuthenticationMode = BrokerAuthenticationMode.Plain,
        //                  ConsumerGroup = "$Default")] KafkaEventData<string> events,
        //    ILogger log)
        //{
        //    Trigger.Run(new KafkaEventData<string>[]{events}, _settings, log);
        //}

        [FunctionName("test-event-grid-mdp-kafka-trigger-reservations")]
        public void RunReservations(
            [KafkaTrigger("%mdpSettings:brokerList%",
                          "topic.DataHub.reservations",
                          Protocol = BrokerProtocol.Plaintext,
                          AuthenticationMode = BrokerAuthenticationMode.NotSet,
                          ConsumerGroup = "%mdpSettings:consumerGroup%")] KafkaEventData<string> events,
            ILogger log)
        {
            log.LogInformation($">>> {_settings.BrokerList},{_settings.ConsumerGroup},{_settings.Enabled}: {events.Topic}, time_stamp: {events.Timestamp}, offset: {events.Offset}");
            Trigger.Run(new KafkaEventData<string>[] { events }, _settings, log);
        }


        //[FunctionName("test-event-grid-mdp-kafka-trigger-reservationsQ")]
        //public void RunReservationsQ(
        //    [KafkaTrigger("167.86.107.116:9092",
        //        "topic.DataHub.reservations",
        //        Protocol = BrokerProtocol.Plaintext,
        //        AuthenticationMode = BrokerAuthenticationMode.NotSet,
        //        ConsumerGroup = "%mdpSettings:consumerGroup%")] KafkaEventData<string> events,
        //    ILogger log)
        //{
        //    log.LogInformation($">>> {"167.86.107.116:9092"},{_settings.ConsumerGroup},{_settings.Enabled}:, time_stamp: {events.Timestamp}, offset: {events.Offset}");
        //    Trigger.Run(new KafkaEventData<string>[] { events }, _settings, log);
        //}

    }
}
