using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace DeadStorageHandler
{
    public class DeadStorageHandler
    {
        [FunctionName("deadStorageHandler")]
        public void Run([BlobTrigger("deadcontainer/{name}", Connection = "AzureWebJobsStorageDeadContainer")]Stream myBlob, string name, ILogger log)
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");
            var x = new StreamReader(myBlob).ReadToEnd();
            log.LogInformation(x);
        }
    }
}
