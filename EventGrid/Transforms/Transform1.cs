using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

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
