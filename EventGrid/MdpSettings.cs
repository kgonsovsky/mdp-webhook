using System.Text.Json.Serialization;

namespace EventGrid
{
    public class MdpSettings
    {
        public class Topic
        {
            [JsonPropertyName("endPoint")]
            public string EndPoint { get; set;}

            [JsonPropertyName("secret")]
            public string Secret { get; set; }

            [JsonPropertyName("resourceName")]
            public string ResourceName { get; set; }

            [JsonPropertyName("transform")]
            public string Transform { get; set; }

        }

        [JsonPropertyName("topics")]
        public Topic[] Topics{ get; set; }
    }
}
