using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Net.Http.Json;
using Newtonsoft.Json.Linq;
using Azure.Messaging.EventGrid;
using Azure;
using System.Text.Json.Nodes;

namespace EventGrid
{
    public static class EG
    {
        public static JsonObject ExtractMdpObject(string requestBody)
        {
            var z = @"payload"":""(.*)(?=}})";
            var regex = new Regex(z, RegexOptions.Multiline | RegexOptions.CultureInvariant | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);
            Match m = regex.Match(requestBody);
            var mdpJson = m.Value.Substring(10) + "}}";

            var transform = new Transforms.Transform1();
            var mdpObj = transform.Transform(mdpJson);

            return mdpObj;
        }

        public static ObjectResult PostToEventGridWithStandartSchema(JsonObject mdpObject, string topic, string sender)
        {
            AzureKeyCredential credential = new AzureKeyCredential("2vn2IdoRgG/2a1QNKUdylnThQW1SZ8lwEKq0lawoxDk=");
            Uri endpoint = new Uri("https://mdptopic.westeurope-1.eventgrid.azure.net/api/events");

            EventGridPublisherClient client = new EventGridPublisherClient(endpoint, credential);
            EventGridEvent firstEvent = new EventGridEvent(
             subject: $"{(topic == null ? "null" : topic)} ({sender})",
             eventType: mdpObject["operationType"].GetValue<string>(),
             dataVersion: "1.0",
             data: mdpObject
             );
            var x = client.SendEventAsync(firstEvent).Result;
            var y = new StreamReader(x.ContentStream).ReadToEnd();
            return new OkObjectResult(y);
        }
    }
}