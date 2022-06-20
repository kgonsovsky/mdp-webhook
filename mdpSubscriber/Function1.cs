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

        private static bool EventTypeSubcriptionValidation(HttpRequest req) => req.Headers["aeg-event-type"].FirstOrDefault() =="SubscriptionValidation";

        private static bool EventTypeNotification(HttpRequest req)=> req.Headers["aeg-event-type"].FirstOrDefault() == "Notification";

    
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
