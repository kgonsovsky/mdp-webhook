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
                log.LogInformation($"kafka topic: {eventData.Topic}, time_stamp: {eventData.Timestamp}, offset: {eventData.Offset}");
                var y = EG.PostToEventGrid(eventData, setting,log);
            }
        }
    }
}
