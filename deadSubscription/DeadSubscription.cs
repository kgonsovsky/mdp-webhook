using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using subscriberRest.Models;

namespace DeadLetterSample
{
    /// <summary>
    /// https://deadsubscription20220623024206.azurewebsites.net/api/deadSubscriptionFunction?code=uTyukqgMunP-amP1w5QcWkWrW6Uz0aMfGX9bAjjEAtGlAzFujvQM4A==
    /// </summary>
    public static class DeadSubscription
    {
        [FunctionName("deadSubscriptionFunction")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequestMessage req, TraceWriter log)
        {
            log.Info($"C# HTTP trigger function begun");
            string response = string.Empty;
            const string SubscriptionValidationEvent = "Microsoft.EventGrid.SubscriptionValidationEvent";

            string requestContent = await req.Content.ReadAsStringAsync();
            log.Info($"Received events: {requestContent}");
            EventGridEvent[] eventGridEvents = JsonConvert.DeserializeObject<EventGridEvent[]>(requestContent);

            var gridEvent =
           JsonConvert.DeserializeObject<List<GridEvent<Dictionary<string, string>>>>(requestContent)
               .First();
            var validationCode = gridEvent.Data["validationCode"];
            var responseData = new SubscriptionValidationResponse();
            responseData.ValidationResponse = validationCode;
            return req.CreateResponse(HttpStatusCode.OK, responseData);

            //foreach (EventGridEvent eventGridEvent in eventGridEvents)
            //{
            //    // Deserialize the event data into the appropriate type based on event type
            //    if (string.Equals(eventGridEvent.EventType, SubscriptionValidationEvent, StringComparison.OrdinalIgnoreCase))
            //    {

            //        var eventData = eventGridEvent.Data.ToObjectFromJson<SubscriptionValidationEventData>();
            //        log.Info($"Got SubscriptionValidation event data, validationCode: {eventData.ValidationCode},  validationUrl: {eventData.ValidationUrl}, topic: {eventGridEvent.Topic}");
            //        // Do any additional validation (as required) such as validating that the Azure resource ID of the topic matches
            //        // the expected topic and then return back the below response
            //        var responseData = new SubscriptionValidationResponse();
            //        responseData.ValidationResponse = eventData.ValidationCode;
            //        return req.CreateResponse(HttpStatusCode.OK, responseData);
            //    }
            //}

            // Responding back with a 400 Bad Request is intentional and only for the purpose of demonstrating dead lettering.
            return req.CreateResponse(HttpStatusCode.BadRequest, response);
        }
    }
}