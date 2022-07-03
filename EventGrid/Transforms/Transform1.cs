using System.Text.Json.Nodes;

namespace EventGrid.Transforms
{
    public class Transform1 : IMdpTransform
    {
        public JsonObject Transform(string jsonString)
        {
            var root = JsonNode.Parse(jsonString).AsObject();
            root.Remove("_id");
            return root;
        }
    }
}
