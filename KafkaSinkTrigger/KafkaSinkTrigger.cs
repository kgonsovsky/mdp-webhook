using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using EventGrid;
using Microsoft.Azure.WebJobs.Extensions.Kafka;
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
            log.LogInformation($"Sink: {req.QueryString}");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            var topic = "";
            if (string.IsNullOrEmpty(topic))
                topic = req.Query["topic"];
            if (string.IsNullOrEmpty(topic))
                topic = req.Headers["topic"];
            if (string.IsNullOrEmpty(topic))
                topic = "unknown_topic";

            var obj = new KafkaEventData<string>() { Value = requestBody, Topic = topic };
            obj.Value = requestBody;
            obj.Topic = topic + " (sink connector)";
            var y = EG.PostToEventGrid(obj, _settings, log);
            return y;
        }
    }
}
