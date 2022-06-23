using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace subscriberServieBusQueueTrigger
{
    public class SubscriberServieBusQueueTrigger
    {
        [FunctionName("subscriberServieBusQueueTrigger")]
        public void Run([ServiceBusTrigger("test-service-bus-mdp-queue", Connection = "AzureWebJobserviceBus")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");
        }
    }
}
