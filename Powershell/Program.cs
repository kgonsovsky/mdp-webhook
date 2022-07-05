// See https://aka.ms/new-console-template for more information

using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using EventGrid;
using EventGrid.Transforms;
using Microsoft.Azure.WebJobs.Extensions.Kafka;
using Newtonsoft.Json;

Console.WriteLine("Hello, World!");

var q = System.IO.File.ReadAllText("1.txt");


var kevent = new KafkaEventData<string>() {Value = q, Topic = "reservations" };

var result = EventGrid.EG.ExtractMdpObject(kevent, new MdpSettings.Topic() { Transform = "Transform1" });

Console.WriteLine(JsonConvert.SerializeObject(result));
