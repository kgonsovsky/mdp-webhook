using EventGrid;
using Microsoft.Azure.WebJobs.Extensions.Kafka;
using Microsoft.Extensions.Logging;

namespace KafkaTrigger
{
    internal static class Trigger
    {
        internal static void Run(KafkaEventData<string>[] events, MdpSettings setting, ILogger log)
        {
            foreach (KafkaEventData<string> eventData in events)
            {
                log.LogInformation($"C# Kafka trigger function processed a message: {eventData.Value}");

                var y = EG.PostToEventGrid(eventData.Value, eventData.Topic, setting,"Kafka Trigger");
            }
        }
    }
}
