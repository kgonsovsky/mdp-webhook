using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using EventGrid;

namespace KafkaSinkTrigger
{
    /// <summary>
    /// https://test-event-grid-mdp-kafka-sinik-trigger.azurewebsites.net/api/mdpSinkAcceptorFunction?code=ocniAZFiF2zFb4y9JPElY4dqSS5jRo5DkmjKW_xENN5aAzFuVPgVEA==
    /// </summary>
    public static class KafkaSinkTrigger
    {

        [FunctionName("mdpSinkAcceptorFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            var topic = req.Headers["topic"];

            var x = EG.ExtractMdpObject(requestBody);

            var y = EG.PostToEventGridWithStandartSchema(x, topic, "Sink Connector");

            return new OkObjectResult(y);
        }
    }
}
