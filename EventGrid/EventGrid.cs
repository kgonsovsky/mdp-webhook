using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using Azure.Messaging.EventGrid;
using Azure;
using System.Text.Json.Nodes;
using EventGrid.Transforms;
using Microsoft.Azure.WebJobs.Extensions.Kafka;
using Microsoft.Extensions.Logging;

namespace EventGrid
{
    public static class EG
    {
        private static MdpTransform GetTransform(string transform)
        {
            if (string.IsNullOrEmpty(transform))
                return new Transform0();
            Type t = Type.GetType($"EventGrid.Transforms.{transform}");
            if (t == null)
                return new Transform0();
            var obj = Activator.CreateInstance(t) as MdpTransform;
            return obj;
        }

        public static MdpTransformObject ExtractMdpObject(KafkaEventData<string> kevent, MdpSettings.Topic topic)
        {
            var mdpJson = kevent.Value.Replace(@"\", "");
            var transform = GetTransform(topic.Transform);
            var z = @"payload"":""(.*)(?=}})";
            var regex = new Regex(z, RegexOptions.Multiline | RegexOptions.CultureInvariant | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);
            Match m = regex.Match(mdpJson);
            mdpJson = m.Value.Substring(10) + "}}";
            var mdpObj = transform.Transform(kevent, mdpJson);
            return mdpObj;
        }

        public static ObjectResult PostToEventGrid(KafkaEventData<string> kevent, MdpSettings setting, ILogger log)
        {
            try
            {
                foreach (var topic in setting.Topics)
                {
                    try
                    {
                        AzureKeyCredential credential = new AzureKeyCredential(topic.Secret);
                        Uri endpoint = new Uri(topic.EndPoint);
                        var mdpPayload = ExtractMdpObject(kevent, topic);
                        EventGridPublisherClient client = new EventGridPublisherClient(endpoint, credential);
                        EventGridEvent firstEvent = new EventGridEvent(
                            subject: mdpPayload.Topic,
                            eventType: mdpPayload.OperationType,
                            dataVersion: "1.0",
                            data: mdpPayload
                        );
                        if (setting.Enabled)
                        {
                            var x = client.SendEventAsync(firstEvent).Result;
                            var y = new StreamReader(x.ContentStream).ReadToEnd();
                        }
                    }
                    catch (Exception e)
                    {
                        log.LogError($"{e.Message}");
                        return new BadRequestObjectResult(e.Message);
                    }
                }
            }
            catch (Exception e)
            {
                log.LogCritical($"{e.Message}");
                return new BadRequestObjectResult(e.Message);
            }
         
            return new OkObjectResult("");
        }
    }
}