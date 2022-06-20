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
    /// Target for Sink Connector
    /// </summary>
    public static class MdpEventInputFunction
    {

        [FunctionName("mdpEventInputFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            var x = EG.ExtractMdpObject(requestBody);

            var y = EG.PostToEventGridWithStandartSchema(x,"Sink Connector");

            return new OkObjectResult(y);
        }
    }
}
