using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using EventGrid;
using Microsoft.Extensions.Options;

namespace KafkaSinkTrigger
{
    /// <summary>
    /// https://test-event-grid-mdp-kafka-sinik-trigger.azurewebsites.net/api/mdpSinkAcceptorFunction?code=ocniAZFiF2zFb4y9JPElY4dqSS5jRo5DkmjKW_xENN5aAzFuVPgVEA==
    /// </summary>
    public class KafkaSinkTrigger
    {
        private readonly MdpSettings _settings;

        public KafkaSinkTrigger(IOptions<MdpSettings> settings)
        {
            _settings = settings.Value;
        }

        [FunctionName("mdpSinkAcceptorFunction")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {

            string message = _settings.Topics.First().EndPoint;
            log.LogInformation(message);

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            var topic = req.Headers["topic"];


            var y = EG.PostToEventGrid(requestBody, topic, _settings,"Sink Connector" );

            return new OkObjectResult(y);
        }
    }
}
