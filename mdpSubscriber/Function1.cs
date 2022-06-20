using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace mdpSubscriber
{
    /// <summary>
    /// https://mdpsubscriber20220620081000.azurewebsites.net/api/mdpSubscriberFunction?code=0C386bXUHzNWgk_J_q0p8YDjLUiiVOMqVTuYHvsw7AR_AzFuciMTDA==
    /// </summary>
    public static class MdpSubscriberFunction
    {
        /// <summary>
        /// Runs the.
        /// </summary>
        /// <param name="req">The req.</param>
        /// <param name="log">The log.</param>
        /// <returns>A Task.</returns>
        [FunctionName("mdpSubscriberFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {

            string jsonContent = await new StreamReader(req.Body).ReadToEndAsync();

            log.LogInformation(req.Method + " " + req.QueryString.ToString());
            log.LogInformation(String.Join(';',req.Headers.Select(a => a.Key + ": " + a.Value)));
            log.LogInformation(jsonContent);
            log.LogInformation("");
            // dynamic data = JsonConvert.DeserializeObject(requestBody);

            if (EventTypeSubcriptionValidation(req))
            {
                var gridEvent =
             JsonConvert.DeserializeObject<List<GridEvent<Dictionary<string, string>>>>(jsonContent)
                 .First();

                var validationCode = gridEvent.Data["validationCode"];
                return new JsonResult(new
                {
                    validationResponse = validationCode
                });
            }
            else if (EventTypeNotification(req))
            {
                if (IsCloudEvent(jsonContent))
                {

                }
            }

            string responseMessage = "OK";

            return new OkObjectResult(responseMessage);
        }

        /// <summary>
        /// Events the type subcription validation.
        /// </summary>
        /// <param name="req">The req.</param>
        /// <returns>A bool.</returns>
        private static bool EventTypeSubcriptionValidation(HttpRequest req) => req.Headers["aeg-event-type"].FirstOrDefault() =="SubscriptionValidation";

        /// <summary>
        /// Events the type notification.
        /// </summary>
        /// <param name="req">The req.</param>
        /// <returns>A bool.</returns>
        private static bool EventTypeNotification(HttpRequest req)=> req.Headers["aeg-event-type"].FirstOrDefault() == "Notification";


        /// <summary>
        /// Are the cloud event.
        /// </summary>
        /// <param name="jsonContent">The json content.</param>
        /// <returns>A bool.</returns>
        private static bool IsCloudEvent(string jsonContent)
        {
            // Cloud events are sent one at a time, while Grid events
            // are sent in an array. As a result, the JObject.Parse will 
            // fail for Grid events. 
            try
            {
                // Attempt to read one JSON object. 
                var eventData = JObject.Parse(jsonContent);

                // Check for the spec version property.
                var version = eventData["specversion"].Value<string>();
                if (!string.IsNullOrEmpty(version)) return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return false;
        }

    }
}
