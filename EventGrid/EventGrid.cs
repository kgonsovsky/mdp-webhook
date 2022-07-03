using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using Azure.Messaging.EventGrid;
using Azure;
using System.Text.Json.Nodes;
using EventGrid.Transforms;

namespace EventGrid
{
    public static class EG
    {
        private static IMdpTransform GetTransform(string transform)
        {
            if (string.IsNullOrEmpty(transform))
                return new Transform0();
            Type t = Type.GetType($"EventGrid.Transforms.{transform}");
            var obj = Activator.CreateInstance(t) as IMdpTransform;
            return obj;
        }

        private static JsonObject ExtractMdpObject(string requestBody, MdpSettings.Topic topic)
        {
            var transform = GetTransform(topic.Transform);
            var z = @"payload"":""(.*)(?=}})";
            var regex = new Regex(z, RegexOptions.Multiline | RegexOptions.CultureInvariant | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);
            Match m = regex.Match(requestBody);
            var mdpJson = m.Value.Substring(10) + "}}";

            var mdpObj = transform.Transform(mdpJson);

            return mdpObj;
        }

        public static ObjectResult PostToEventGrid(string payload, string topicName, MdpSettings setting, string sender)
        {
            foreach (var topic in setting.Topics)
            {
                AzureKeyCredential credential = new AzureKeyCredential(topic.Secret);
                Uri endpoint = new Uri(topic.EndPoint);

                var mdpPayload = ExtractMdpObject(payload, topic);

                EventGridPublisherClient client = new EventGridPublisherClient(endpoint, credential);
                EventGridEvent firstEvent = new EventGridEvent(
                    subject: $"{(topicName == null ? "null" : topicName)} ({sender}) ({topic.ResourceName} / {topic.Transform}",
                    eventType: mdpPayload["operationType"].GetValue<string>(),
                    dataVersion: "1.0",
                    data: mdpPayload
                );
                var x = client.SendEventAsync(firstEvent).Result;
                var y = new StreamReader(x.ContentStream).ReadToEnd();
           
            }
            return new OkObjectResult("");
        }
    }
}