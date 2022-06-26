using System.Text.Json.Nodes;

namespace EventGrid.Transforms
{
    internal class Transform1
    {
        internal JsonObject Transform(string jsonString)
        {
            var root = JsonNode.Parse(jsonString).AsObject();
            root.Remove("_id");
            return root;
        }
    }
}
