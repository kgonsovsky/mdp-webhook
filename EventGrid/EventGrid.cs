using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Text.RegularExpressions;

namespace EventGrid
{
    public static class EG
    {
        public static object ExtractMdpObject(string requestBody)
        {
            var z = @"payload"":""(.*)(?=}})";
            var regex = new Regex(z, RegexOptions.Multiline | RegexOptions.CultureInvariant | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);

            Match m = regex.Match(requestBody);
            var x = m.Value.Substring(10) + "}}";
            return JsonConvert.DeserializeObject(x);
        }

        public static ObjectResult PostToEventGridWithStandartSchema(object mdpObject, string sender)
        {
            var client = new HttpClient()
            {
                BaseAddress = new Uri("https://mdptopic.westeurope-1.eventgrid.azure.net/api/events")
            };
            client.DefaultRequestHeaders.Add("aeg-sas-key", "2vn2IdoRgG/2a1QNKUdylnThQW1SZ8lwEKq0lawoxDk=");

            var egevent = new
            {
                Id = 1234,
                Subject = sender,
                EventType = "BookingEvent",
                EventTime = DateTime.UtcNow,
                Data = mdpObject
            };
            var x2 = client.PostAsJsonAsync("", new[] { egevent }).Result;

            return new OkObjectResult(x2);
        }


    }
}