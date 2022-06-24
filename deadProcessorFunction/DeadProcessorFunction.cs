// This is the default URL for triggering event grid function in the local environment.
// http://localhost:7071/admin/extensions/EventGridExtensionConfig?functionName={functionname} 

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Azure.Messaging.EventGrid;
using System;
using Azure.Messaging.EventGrid.SystemEvents;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace DeadLetterSample
{
    /// <summary>
    /// 
    /// </summary>
    public static class DeadProcessorFunction
    {
        [FunctionName("deadProcessorFunction")]
        public static void Run([EventGridTrigger] EventGridEvent eventGridEvent, ILogger logger)
        {
            logger.LogInformation($"C# Event grid trigger function has begun...");
            const string StorageBlobCreatedEvent = "Microsoft.Storage.BlobCreated";


         logger.LogInformation($"Print EventGridEvent Event attributes for Id: {eventGridEvent.Id}, Topic: {eventGridEvent.Topic}, Subject: {eventGridEvent.Subject}, EventType: {eventGridEvent.EventType}, DataVersion: {eventGridEvent.DataVersion}, EventTime: {eventGridEvent.EventTime}");


            if (string.Equals(eventGridEvent.EventType, StorageBlobCreatedEvent, StringComparison.OrdinalIgnoreCase))
            {
                logger.LogInformation("Received blob created event..");
                StorageBlobCreatedEventData data = eventGridEvent.Data.ToObjectFromJson<StorageBlobCreatedEventData>();
                logger.LogInformation($"Dead Letter Blob Url:{data.Url}, {data.Sequencer}, {data.Identity}, {data.RequestId}, {data.ClientRequestId}, {data.Api}, {data.ETag}");
                logger.LogInformation("Reading blob data from storage account..");
                ProcessBlob(data.Url, logger);
            }
        }

        /*
        This function uses the blob url/location received in the BlobCreated event to fetch data from the storage container.
        Here, we perform a simple GET request on the blob url and deserialize the dead lettered events in a json array.
        */
        public static void ProcessBlob(string url, ILogger logger)
        {

            // sas key generated through the portal for your storage account used for authentication
            const string sasKey = "9PRqQZcO2T9o6Z68oyiDGQ79LDY9FU0uW%2FOtkYvPIxo%3D";
            string uri = url + sasKey;

            logger.LogInformation($"url: {url}");

            WebClient client = new WebClient();

            Stream data = client.OpenRead(uri);
            StreamReader reader = new StreamReader(data);
            string blob = reader.ReadToEnd();
            data.Close();
            reader.Close();

            logger.LogInformation($"Dead Letter Events:{blob}");

            // deserialize the blob into dead letter events 
            DeadLetterEvent[] dlEvents = JsonConvert.DeserializeObject<DeadLetterEvent[]>(blob);

            foreach (DeadLetterEvent dlEvent in dlEvents) {
       
                logger.LogInformation($"Printing Dead Letter Event attributes for EventId: {dlEvent.Id}, Dead Letter Reason:{dlEvent.DeadLetterReason}, DeliveryAttempts:{dlEvent.DeliveryAttempts}, LastDeliveryOutcome:{dlEvent.LastDeliveryOutcome}, LastHttpStatusCode:{dlEvent.LastHttpStatusCode}");
                // TODO: steps for processing the dead letter event further
            }
        }
    }
}
