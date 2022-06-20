using EventGrid;
using Microsoft.Azure.WebJobs.Extensions.Kafka;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mdpKafkaTriggerFunction
{
    internal static class Trigger
    {
        internal static void Run(KafkaEventData<string>[] events, ILogger log)
        {
            foreach (KafkaEventData<string> eventData in events)
            {
                log.LogInformation($"C# Kafka trigger function processed a message: {eventData.Value}");

                var x = EG.ExtractMdpObject(eventData.Value);

                var y = EG.PostToEventGridWithStandartSchema(x, eventData.Topic, "Kafka Trigger");
            }
        }
    }
}
