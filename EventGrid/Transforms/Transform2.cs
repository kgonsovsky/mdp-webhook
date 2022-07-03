using System.Text.Json.Nodes;

namespace EventGrid.Transforms
{
    public class Transform2 : IMdpTransform
    {
        public JsonObject Transform(string jsonString)
        {
            var root = JsonNode.Parse(jsonString).AsObject();
            root.Remove("_id");
            root.Remove("fullDocument");
            return root;
        }
    }
}
