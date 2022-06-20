using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace mdpEventInputFunction
{


    public static class MdpEventInputFunction
    {
        [FunctionName("mdpEventInputFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            var z = @"payload"":""(.*)(?=}})";
            var regex = new Regex(z, RegexOptions.Multiline | RegexOptions.CultureInvariant | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);

            Match m = regex.Match(requestBody);
            var x = m.Value.Substring(10) + "}}";
            if (m.Success)
            {
                Console.WriteLine(x);
            }


            var client = new HttpClient()
            {
                BaseAddress = new Uri("https://mdptopic.westeurope-1.eventgrid.azure.net/api/events")
            };
            client.DefaultRequestHeaders.Add("aeg-sas-key", "2vn2IdoRgG/2a1QNKUdylnThQW1SZ8lwEKq0lawoxDk=");

            var egevent = new
            {
                Id = 1234,
                Subject = "Event dilivered from standalone Kafka",
                EventType = "Creation",
                EventTime = DateTime.UtcNow,
                Data = JsonConvert.DeserializeObject(x)
            };
            var x2 = await client.PostAsJsonAsync("", new[] { egevent });

            return new OkObjectResult(x2);

        }
    }
}
