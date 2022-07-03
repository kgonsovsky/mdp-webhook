using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Nodes;

namespace EventGrid
{
    public interface IMdpTransform
    {
        public JsonObject Transform(string jsonString);
    }
}
