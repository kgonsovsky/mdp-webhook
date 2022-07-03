using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

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
