using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Net.Http;
using System.Globalization;
using System.Text;
using System.Net.Http.Headers;

namespace mdpwebhookAzureFunction
{
    /// <summary>
    /// https://mdpwebhookazurefunction20220610214734.azurewebsites.net/api/mdpwebhookFunction?code=SEgs8YVe3dDQKHvJXSz7daqwMUpJWgt1g8n6UQaSoifFAzFuh9y6dA==
    /// http://144.91.91.117:7071/api/mdpCloudInputFunction?code=lcFBirMB5c/WyO8m5GiCL3kzHS8tNO7YE4/kp5BHAs0=
    /// https://stackoverflow.com/questions/52348802/how-to-write-to-azure-event-grid-topic-in-azure-function
    /// </summary>
    public static class MdpCloudInputFunction
    {
        [FunctionName("mdpCloudInputFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post","put", Route = null)] HttpRequest req,
            ILogger log)
        {
          
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            var z = @"payload"":""(.*)(?=}})";
            var regex = new Regex(z, RegexOptions.Multiline | RegexOptions.CultureInvariant | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);

            Match m = regex.Match(requestBody);
            var x = m.Value.Substring(10) + "}}";
            if (m.Success)
            {
                Console.WriteLine(x);
            }

            var client = new HttpClient { BaseAddress = new Uri("https://mdpcloudtopic.westeurope-1.eventgrid.azure.net/api/events") };

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("aeg-sas-key", "lcFBirMB5c/WyO8m5GiCL3kzHS8tNO7YE4/kp5BHAs0=");

            var cloudEvent = new CloudEvent<dynamic>
            {
                EventId = Guid.NewGuid().ToString(),
                EventType = "BookingEvent",
                EventTypeVersion = "1.0",
                CloudEventVersion = "0.1",
                Data = JsonConvert.DeserializeObject(x),
                Source = $"BookingEvent",
                EventTime = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture),
            };

            var json = JsonConvert.SerializeObject(cloudEvent);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(string.Empty, content);

            return new OkObjectResult(response);

  
        }
    }
}
