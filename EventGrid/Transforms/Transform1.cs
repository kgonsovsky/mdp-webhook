using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Microsoft.Azure.WebJobs.Extensions.Kafka;

namespace EventGrid.Transforms
{
    public class Transform1Object: MdpTransformObject
    {
        [JsonPropertyName("OPERA")] 
        public OperaClass Opera { get; set; } = new OperaClass();

        public class OperaClass
        {
            [JsonPropertyName("CONFIRMATION_NO")]
            public string ConfirmationNo { get; set; }

            [JsonPropertyName("RESV_NAME_ID")]
            public string ResvNameId { get; set; }
        }
    }

    public class Transform1 : MdpTransform
    {
        public override MdpTransformObject Transform(KafkaEventData<string> kevent, string json)
        {
            var root = JsonNode.Parse(json).AsObject();
            var result = new Transform1Object();
           
            BaseTransform(result, root, kevent);

            if (kevent.Topic.ToLower().Contains("reservations"))
            {
                try
                {
                    result.Opera = new Transform1Object.OperaClass()
                    {
                        ConfirmationNo = root["fullDocument"]?["mappingFields"]?["OPERA"]?["CONFIRMATION_NO"]?.GetValue<string>(),
                        ResvNameId = root["fullDocument"]?["mappingFields"]?["OPERA"]?["RESV_NAME_ID"]?.GetValue<string>(),
                    };
                }
                catch (Exception e)
                {
                    result.Debug.Exception = e.Message;
                }
            }
            return result;
        }
    }
}
