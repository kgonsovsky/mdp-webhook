using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace EventGrid
{
    public interface IMdpTransform
    {
        public JsonObject Transform(string jsonString);
    }
}
