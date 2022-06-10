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

using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;


namespace CCloud
{
    class Program
    {
        static async Task<ClientConfig> LoadConfig(string configPath, string certDir)
        {
            try
            {
                var q = await File.ReadAllLinesAsync(configPath);
                var cloudConfig = (await File.ReadAllLinesAsync(configPath))
                    .Where(line => !line.StartsWith("#"))
                    .ToDictionary(
                        line => line.Substring(0, line.IndexOf('=')),
                        line => line.Substring(line.IndexOf('=') + 1));

                var clientConfig = new ClientConfig(cloudConfig);

                if (certDir != null)
                {
                    clientConfig.SecurityProtocol = SecurityProtocol.Plaintext;
                    //clientConfig.SslCaLocation = "cacert.pem";
                    //clientConfig.SaslMechanism = SaslMechanism.Plain;

                }

                return clientConfig;
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error occured reading the config file from '{configPath}': {e.Message}");
                System.Environment.Exit(1);
                return null; // avoid not-all-paths-return-value compiler error.
            }
        }

        static async Task CreateTopicMaybe(string name, int numPartitions, short replicationFactor, ClientConfig cloudConfig)
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
        
        static void Produce(string topic, ClientConfig config)
        {
            using (var producer = new ProducerBuilder<string, string>(config).Build())
            {
                int numProduced = 0;
                int numMessages = 1;
                for (int i=0; i<numMessages; ++i)
                {
                    var key = "reservation";
                    var val = @"

{
""schema"":{
""type"":""string""
""optional"":false
}
""payload"":""{""_id"": {""_data"": ""82627A06CB000000032B022C0100296E5A1004BA90090870E04E3BBA59F3A4325E2E2446645F69640064627A06C926FA9FB10659D6B10004""}, ""operationType"": ""replace"", ""clusterTime"": {""$timestamp"": {""t"": 1652164299, ""i"": 3}}, ""fullDocument"": {""_id"": {""$oid"": ""627a06c926fa9fb10659d6b1""}, ""masterId"": ""dafb48e9-2197-4799-ba47-3bb5e0eec209"", ""isActive"": true, ""isDeleted"": false, ""createdBySystem"": ""PHOBS"", ""createdDate"": {""$date"": 1652164297903}, ""modifiedBySystem"": null, ""modifiedDate"": null, ""version"": 2, ""mappingFields"": {""VRC"": {""_id"": ""627a06cb7511d533fcebd7b4""}}, ""personalData"": {""firstName"": ""Damir"", ""lastName"": ""Imamović"", ""salutation"": null, ""gender"": null, ""language"": null, ""birthDate"": null, ""age"": null, ""familyStatusIds"": []}, ""address"": {""country"": null, ""city"": null, ""street"": null, ""zipCode"": null}, ""contact"": {""phone"": null, ""email"": null, ""viber"": null, ""messenger"": null, ""additional"": {""phones"": [], ""emails"": [""mosnik@icloud.com""], ""tempEmails"": []}}, ""additionalInformation"": {""revenueSegment"": null, ""isBlacklisted"": false, ""gdprDelete"": false, ""isVip"": false, ""customerMemberType"": null, ""isOtaRestricted"": false, ""extraMeal"": false, ""babyCot"": false, ""travelWithPets"": false, ""isTopGuest"": false}, ""advertisingPermissions"": {""email"": null, ""sms"": null, ""viber"": null, ""whatsApp"": null, ""push"": null, ""segmentation"": null}, ""products"": {""valamar"": null, ""camping"": null, ""bike"": null, ""valfresco"": null}, ""interestIds"": [], ""status"": null, ""duplicated"": false, ""confirmed"": false}, ""ns"": {""db"": ""DataHub"", ""coll"": ""accounts""}, ""documentKey"": {""_id"": {""$oid"": ""627a06c926fa9fb10659d6b1""}}}""
}

            ";

               //     val = @"{""a"": ""b""}";


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

        static void Consume(string topic, ClientConfig config)
        {
            var consumerConfig = new ConsumerConfig(config);
            consumerConfig.GroupId = "dotnet-example-group-1";
            consumerConfig.AutoOffsetReset = AutoOffsetReset.Earliest;
            consumerConfig.EnableAutoCommit = false;

            CancellationTokenSource cts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) => {
                e.Cancel = true; // prevent the process from terminating.
                cts.Cancel();
            };

            using (var consumer = new ConsumerBuilder<string, string>(consumerConfig).Build())
            {
                consumer.Subscribe(topic);
                var totalCount = 0;
                try
                {
                    while (true)
                    {
                        var cr = consumer.Consume(cts.Token);
                        totalCount += JObject.Parse(cr.Message.Value).Value<int>("count");
                        Console.WriteLine($"Consumed record with key {cr.Message.Key} and value {cr.Message.Value}, and updated total count to {totalCount}");
                    }
                }
                catch (OperationCanceledException)
                {
                    // Ctrl-C was pressed.
                }
                finally
                {
                    consumer.Close();
                }
            }
        }

        static void PrintUsage()
        {
            Console.WriteLine("usage: .. produce|consume <topic> <configPath> [<certDir>]");
            System.Environment.Exit(1);
        }

        static async Task Main(string[] args)
        {
          //  if (args.Length != 3 && args.Length != 4) { PrintUsage(); }

            var mode = "produce";
            var topic = "__mdp";
            var configPath = "./csharp.config";
            var certDir = "./";

            //var mode = args[0];
            //var topic = args[1];
            //var configPath = args[2];
            //var certDir = args.Length == 4 ? args[3] : null;

            var config = await LoadConfig(configPath, certDir);

            switch (mode)
            {
                case "produce":
                    await CreateTopicMaybe(topic, 1, 3, config);
                    Produce(topic, config);
                    break;
                case "consume":
                    Consume(topic, config);
                    break;
                default:
                    PrintUsage();
                    break;
            }
        }
    }
}
