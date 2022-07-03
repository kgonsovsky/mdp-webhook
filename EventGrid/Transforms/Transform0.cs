using System.Text.Json.Nodes;

namespace EventGrid.Transforms
{
    public class Transform0: IMdpTransform
    {
        public JsonObject Transform(string jsonString)
        {
            var root = JsonNode.Parse(jsonString).AsObject();
            return root;
        }
    }
}
