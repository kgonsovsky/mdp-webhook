using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Microsoft.Azure.WebJobs.Extensions.Kafka;

namespace EventGrid.Transforms
{
    public class Transform0Object : MdpTransformObject
    {
        [JsonPropertyName("fullDocument")]
        public JsonObject FullDocument { get; set; }
    }

    public class Transform0: MdpTransform
    {
        public override MdpTransformObject Transform(KafkaEventData<string> kevent, string json)
        {
            var root = JsonNode.Parse(json).AsObject();
            var result = new Transform0Object();
            BaseTransform(result, root, kevent);
            try
            {
                result.FullDocument = root["fullDocument"].AsObject();
            }
            catch (Exception e)
            {
                result.Debug.Exception = e.Message;
            }
            return result;
        }
    }
}
