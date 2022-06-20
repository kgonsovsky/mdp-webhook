using EventGrid;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Kafka;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace mdpKafkaTriggerFunction
{
    /// <summary>
    /// https://mdpkafkatriggerfunction20220619153559.azurewebsites.net/api/mdpKafkaTriggerFunction?code=BkueeSTH_hpx3fgAOQrjN9wQcEFA51BhX4cwj-Sc2V2EAzFumn2gOA==
    /// </summary>
    /// </summary>
    public class MdpKafkaTriggerFunction
    {
        // KafkaTrigger sample 
        // Consume the message from "__mdp" on the LocalBroker.
        // Add `pkc-epwny.eastus.azure.confluent.cloud:9092BrokerList` and `KafkaPassword` to the local.settings.json
        // For EventHubs
        // "pkc-epwny.eastus.azure.confluent.cloud:9092BrokerList": "{EVENT_HUBS_NAMESPACE}.servicebus.windows.net:9093"
        // "KafkaPassword":"{EVENT_HUBS_CONNECTION_STRING}
        /// <summary>
        /// Runs the.
        /// </summary>
        /// <param name="events">The events.</param>
        /// <param name="log">The log.</param>
        [FunctionName("mdpKafkaTriggerFunction")]
        public void Run(
            [KafkaTrigger("pkc-epwny.eastus.azure.confluent.cloud:9092BrokerList",
                          "__mdp",
                          Username = "FSWDPEVFDNYLJRRJ",
                          Password = "oPDfLFWA8IKr5cbEZ+0V/Cgrnv1QbeE5gRsM2GBrDUSYe8Bi9JMNZLznlbDYU/my",
                          Protocol = BrokerProtocol.SaslSsl,
                          AuthenticationMode = BrokerAuthenticationMode.Plain,
                          ConsumerGroup = "$Default")] KafkaEventData<string>[] events,
            ILogger log)
        {
            foreach (KafkaEventData<string> eventData in events)
            {
                log.LogInformation($"C# Kafka trigger function processed a message: {eventData.Value}");

                var x = EG.ExtractMdpObject(eventData.Value);

                var y = EG.PostToEventGridWithStandartSchema(x, "Kafka Trigger");
            }
        }
    }
}
