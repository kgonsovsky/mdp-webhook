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
using EventGrid;

namespace mdpEventInputFunction
{
    /// <summary>
    /// https://mdpsinkacceptorfunction20220620204529.azurewebsites.net/api/mdpSinkAcceptorFunction?code=c5aEDv2dbBwg-JmsroCbG4FdoeRbnadZftcW6NBC7Q3-AzFutUWPdA==
    /// </summary>
    public static class MdpSinkAcceptorFunction
    {`

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