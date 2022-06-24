using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace deadTriggerFunction
{
    /// <summary>
    /// https://test-event-grid-mdp-subscriber-dead.azurewebsites.net/api/deadTriggerFunction?code=https://test-event-grid-mdp-subscriber-dead.azurewebsites.net/api/deadTriggerFunction?code=4f5Q7cLMpijlByO7nzP7c3hv0wK4e6e2NQguIloiWA4cAzFurtiTWg==
    /// </summary>
    public static class DeadTriggerFunction
    {
        [FunctionName("deadTriggerFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,ILogger log)
        {
            log.LogInformation($"C# HTTP trigger function begun");
            var requestContent = await new StreamReader(req.Body).ReadToEndAsync();

            try
            {
                if (req.Method.ToLower()=="options")
                {
                    var webhookRequestOrigin = req.HttpContext.Request.Headers["WebHook-Request-Origin"].FirstOrDefault();
                    var webhookRequestCallback = req.HttpContext.Request.Headers["WebHook-Request-Callback"];
                    var webhookRequestRate = req.HttpContext.Request.Headers["WebHook-Request-Rate"];
                    req.HttpContext.Response.Headers.Add("WebHook-Allowed-Rate", "*");
                    req.HttpContext.Response.Headers.Add("WebHook-Allowed-Origin", webhookRequestOrigin);
                }
                else
                {
                    log.LogInformation(req.Method.ToString());
                    log.LogInformation(string.Join(',', req.Headers.Select(x => x.Key + ":" + x.Value)));
                    log.LogInformation(requestContent);

                    if (EventTypeSubcriptionValidation(req))
                    {
                        return await HandleValidation(requestContent, req, log);
                    }
                    else if (EventTypeNotification(req))
                    {
                        return new BadRequestResult(); //force death
                        if (IsCloudEvent(requestContent))
                        {
                            return await HandleCloudEvent(requestContent, req, log);
                        }
                        else
                        {
                            return await HandleGridEvents(requestContent, req, log);
                        }
                    }
                    else
                    {
                        return new JsonResult(new
                        {

                        });
                    }
                }

                return new JsonResult(new
                {

                });
            }
            catch(Exception e)
            {
                log.LogInformation(e.Message);
                return new JsonResult(new
                {

                });
            }
       
        }

        private static bool EventTypeSubcriptionValidation(HttpRequest req)
           => req.Headers["aeg-event-type"] ==
              "SubscriptionValidation";

        private static bool EventTypeNotification(HttpRequest req)
            => req.Headers["aeg-event-type"] ==
               "Notification";

        private static async Task<JsonResult> HandleValidation(string jsonContent, HttpRequest req, ILogger log)
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

        private static async Task<IActionResult> HandleGridEvents(string jsonContent, HttpRequest req, ILogger log)
        {
            var events = JArray.Parse(jsonContent);
            foreach (var e in events)
            {
                log.LogInformation(e.ToString());
            }
            return new JsonResult(new
            {
                
            });
        }

        private static async Task<IActionResult> HandleCloudEvent(string jsonContent, HttpRequest req, ILogger log)
        {
            var details = JsonConvert.DeserializeObject<CloudEvent<dynamic>>(jsonContent);
            var eventData = JObject.Parse(jsonContent);
            log.LogInformation(details.Subject);
            return new JsonResult(new
            {

            });
        }

        private static bool IsCloudEvent(string jsonContent)
        {
            try
            {
                var eventData = JObject.Parse(jsonContent);
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