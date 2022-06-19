using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mdpwebhookAzureFunction
{
    public class CloudEvent<T> where T : class
    {
        [JsonProperty("id")]
        public string EventId { get; set; }

        [JsonProperty("cloudEventsVersion")]
        public string CloudEventVersion { get; set; }

        [JsonProperty("type")]
        public string EventType { get; set; }

        [JsonProperty("eventTypeVersion")]
        public string EventTypeVersion { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("time")]
        public string EventTime { get; set; }

        [JsonProperty("data")]
        public T Data { get; set; }
    }
}
