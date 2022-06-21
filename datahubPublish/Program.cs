// Copyright 2020 Confluent Inc.

// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at

// http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.Admin;


namespace DatahubPublish
{
    internal class Program
    {
        internal const int Interval = 30;

        internal static List<ClientConfig> Configs;

        private static ClientConfig LoadConfig(string configPath)
        {
            var cloudConfig = (File.ReadAllLines(configPath)).Where(line => !line.StartsWith("#"))
                .ToDictionary(line => line.Substring(0, line.IndexOf('=')),
                    line => line.Substring(line.IndexOf('=') + 1));
            var clientConfig = new ClientConfig(cloudConfig);
            var p = Path.GetDirectoryName(configPath) + "/" + Path.GetFileNameWithoutExtension(configPath) + ".pem";
            if (System.IO.File.Exists(p))
            clientConfig.SslCaLocation = p;
            return new ClientConfig(cloudConfig);
        }

        private static async Task CreateTopicMaybe(string name, int numPartitions, short replicationFactor, ClientConfig cloudConfig)
        {
            using (var adminClient = new AdminClientBuilder(cloudConfig).Build())
            {
                try
                {
                    await adminClient.CreateTopicsAsync(new List<TopicSpecification> {
                        new TopicSpecification { Name = name, NumPartitions = numPartitions, ReplicationFactor = replicationFactor } });
                }
                catch (CreateTopicsException e)
                {
                    if (e.Results[0].Error.Code != ErrorCode.TopicAlreadyExists)
                    {
                        Console.WriteLine($"An error occured creating topic {name}: {e.Results[0].Error.Reason}");
                    }
                    else
                    {
                        Console.WriteLine("Topic already exists");
                    }
                }
            }
        }

        /// <summary>
        /// Produces the.
        /// </summary>
        /// <param name="topic">The topic.</param>
        /// <param name="config">The config.</param>
        private static void Produce(string topic, ClientConfig config)
        {
            using (var producer = new ProducerBuilder<string, string>(config).Build())
            {
                int numProduced = 0;
                int numMessages = 1;
                for (int i=0; i<numMessages; ++i)
                {
                    var key = "bookingEvent";

                    var val = System.IO.File.ReadAllText("./topics/" + topic + ".txt");
     
                    Console.WriteLine($"Producing record: {key} {val}");

                    producer.Produce(topic, new Message<string, string> { Key = key, Value = val },
                        (deliveryReport) =>
                        {
                            if (deliveryReport.Error.Code != ErrorCode.NoError)
                            {
                                Console.WriteLine($"Failed to deliver message: {deliveryReport.Error.Reason}");
                            }
                            else
                            {
                                Console.WriteLine($"Produced message to: {deliveryReport.TopicPartitionOffset}");
                                numProduced += 1;
                            }
                        });
                }

                producer.Flush(TimeSpan.FromSeconds(10));

                Console.WriteLine($"{numProduced} messages were produced to topic {topic}");
            }
        }


        private static async Task Main(string[] args)
        {
            Configs = Directory.EnumerateFiles("./configs", "*.txt").Select(a => LoadConfig(a)).ToList();
            foreach (var c in Configs)
            {
                foreach (var x in Directory.EnumerateFiles("./topics", "*.txt"))
                {
                    await CreateTopicMaybe(Path.GetFileNameWithoutExtension(x), 1, 3, c);
                }
            }

            while (true)
            {
                foreach (var c in Configs)
                {
                    foreach (var x in Directory.EnumerateFiles("./topics", "*.txt"))
                    {
                        Produce(Path.GetFileNameWithoutExtension(x), c);
                    }
              
                }
                Thread.Sleep(Interval*1000);
            }
        }
    }
}
