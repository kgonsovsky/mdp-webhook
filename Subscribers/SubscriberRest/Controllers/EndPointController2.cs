using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SubscriberRest.Models;

namespace SubscriberRest.Controllers
{
    [Route("api/[controller]")]
    public class EndPoint2Controller : Controller
    {

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                var jsonContent = await reader.ReadToEndAsync();

                Log(Request.Method);
                Log(string.Join(',', Request.Headers.Select(x => x.Key + ":" + x.Value)));
                Log(jsonContent);
                Log("");

                if (EventTypeSubcriptionValidation)
                {
                    return await HandleValidation(jsonContent);
                }
                else if (EventTypeNotification)
                {
                    return BadRequest("Bad Request");

                    if (IsCloudEvent(jsonContent))
                    {
                        return await HandleCloudEvent(jsonContent);
                    }

                    return await HandleGridEvents(jsonContent);
                }

                return BadRequest("Bad Request");
            }

            return Ok("POST");
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return BadRequest("Get - Bad Request");
        }

        [HttpOptions]
        public async Task<IActionResult> Options()
        {
            using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                var webhookRequestOrigin = HttpContext.Request.Headers["WebHook-Request-Origin"].FirstOrDefault();
                var webhookRequestCallback = HttpContext.Request.Headers["WebHook-Request-Callback"];
                var webhookRequestRate = HttpContext.Request.Headers["WebHook-Request-Rate"];
                HttpContext.Response.Headers.Add("WebHook-Allowed-Rate", "*");
                HttpContext.Response.Headers.Add("WebHook-Allowed-Origin", webhookRequestOrigin);
            }

            return Ok("OPTIONS");
        }

        private bool EventTypeSubcriptionValidation
            => HttpContext.Request.Headers["aeg-event-type"].FirstOrDefault() ==
               "SubscriptionValidation";

        private bool EventTypeNotification
            => HttpContext.Request.Headers["aeg-event-type"].FirstOrDefault() ==
               "Notification";

        public EndPoint2Controller()
        {

        }

        private async Task<JsonResult> HandleValidation(string jsonContent)
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

        private async Task<IActionResult> HandleGridEvents(string jsonContent)
        {
            var events = JArray.Parse(jsonContent);
            foreach (var e in events)
            {
                Log(e.ToString());
            }
            return Ok("HandleGridEvents");
        }

        private async Task<IActionResult> HandleCloudEvent(string jsonContent)
        {
            var details = JsonConvert.DeserializeObject<CloudEvent<dynamic>>(jsonContent);
            var eventData = JObject.Parse(jsonContent);
            Log(details.Subject);
            return Ok("HandleCloudEvent");
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

        void Log(string s)
        {
            System.IO.File.AppendAllText(@"C:\inetpub\wwwroot\static\logs\mdppartner2.txt", DateTime.Now.ToString() + ": " + s + "\r\n");
        }
    }
}