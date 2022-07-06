using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Microsoft.Azure.WebJobs.Extensions.Kafka;

namespace EventGrid
{
    public abstract class MdpTransformObject
    {
        [JsonPropertyName("operationType")]
        public string OperationType { get; set; }

        [JsonPropertyName("topic")]
        public string Topic { get; set; }

        [JsonPropertyName("__debug")]
        public DebugClass Debug { get; set; } = new DebugClass();

        public class DebugClass
        {
            [JsonPropertyName("exception")]
            public string Exception { get; set; }

            [JsonPropertyName("offset")]
            public long Offset { get; set; }

            [JsonPropertyName("partition")]
            public int Partition { get; set; }

            [JsonPropertyName("timestamp")]
            public DateTime Timestamp { get; set; }
        }
    }

    public abstract class MdpTransform
    {
        public abstract MdpTransformObject Transform(KafkaEventData<string> kevent, string json);

        public void BaseTransform(MdpTransformObject obj, JsonObject root, KafkaEventData<string> kevent)
        {
            obj.Debug = new MdpTransformObject.DebugClass()
            {
                Offset = kevent.Offset,
                Partition = kevent.Partition,
                Timestamp = kevent.Timestamp
            };
            obj.OperationType = root["operationType"]?.GetValue<string>() ?? "unknown";
            obj.Topic = kevent.Topic;
        }
    }
}
