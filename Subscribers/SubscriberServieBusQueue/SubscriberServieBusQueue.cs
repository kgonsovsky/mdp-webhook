using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace SubscriberServieBusQueue
{
    public class SubscriberServiceBusQueue
    {
        [FunctionName("subscriberServiceBusQueue")]
        public void Run([ServiceBusTrigger("test-service-bus-mdp-queue", Connection = "AzureWebJobServiceBus")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");
        }
    }
}
