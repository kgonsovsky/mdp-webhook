// 
// Copyright (c) Microsoft.  All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Management.EventGrid.Models;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Azure.Management.ResourceManager;
using Microsoft.Azure.Management.ResourceManager.Models;
using Microsoft.Rest;
using Microsoft.Azure.Management.EventGrid;

namespace EGManageTopicsAndEventSubscriptions
{
    /// <summary>
    /// Azure EventGrid Management Sample: Demonstrate how to create and manage EventGrid eventsubscriptions with Dead Letter destinations and Retry Policy options using the EventGrid Management SDK.
    /// Documentation References:
    /// - EventGrid .NET SDK documentation - https://docs.microsoft.com/en-us/dotnet/api/overview/azure/eventgrid?view=azure-dotnet
    /// </summary>
    public class Program
    {
        // Enter the Azure subscription ID you want to use for this sample.
        const string SubscriptionId = "5258beac-d2a1-4e36-8e1b-2d1fbe17450f";

        // Specify a resource group name of your choice. Specifying a new value will create a new resource group.
        const string ResourceGroupName = "test-rg-mdp-webhook";

        // Using a random topic name. Optionally, replace this with a topic name of your choice.
        static readonly string TopicName = "test-event-grid-mdp-topic";

        // Replace the endpoint URL with the URL of your Azure function.
        // See the EventGridConsumer sample for a sample of an Azure function that can handle EventGridEvents
        // Publish the EventGridConsumer sample as an Azure function and use the URL of that function for the below.
        const string EndpointUrl = "https://vmi901413.contaboserver.net/api/endPoint";

        // To run the sample, you must first create an Azure service principal. To create the service principal, follow one of these guides:
        // Azure Portal: https://azure.microsoft.com/documentation/articles/resource-group-create-service-principal-portal/)
        // PowerShell: https://azure.microsoft.com/documentation/articles/resource-group-authenticate-service-principal/
        // Azure CLI: https://azure.microsoft.com/documentation/articles/resource-group-authenticate-service-principal-cli/
        // Creating the service principal will generate the values you need to specify for the constants below.

        // Use the values generated when you created the Azure service principal.
        //const string ApplicationId = "2b1f4208-4a2d-464d-a94c-eee7139bbf46";
        //const string Password = "_SO8Q~oNA6H5dXi8EZdGDQ9zIVP7TpQoptSiodin";
        //const string TenantId = "ed925669-c818-463b-bd8e-5bcb7131f38d";

        const string ApplicationId = "d105bb3d-cfe5-4acf-a3bf-450e46a9209f";
        const string Password = "Aru8Q~Ddc3n2yGxF3CUwGq6qyouej3QLWlELIcEN";
        const string TenantId = "c7223f2c-1ba2-43c8-be7f-57e6e1465036";

        const string EventSubscriptionName = "test-event-grid-mdp-subscription-dead";
        const string DefaultLocation = "westeurope";

        // An example would be /subscriptions/{subid}/resourceGroups/{rg}/providers/Microsoft.Storage/storageAccounts/{storageAccount}
        const string DeadLetterDestinationResourceId = "/subscriptions/5258beac-d2a1-4e36-8e1b-2d1fbe17450f/resourceGroups/test-rg-mdp-webhook/providers/Microsoft.Storage/storageAccounts/testmdpwebhookstorage/blobServices/default";
        const string StorageBlobContainerName = "deadcontainer";

        //The following method will enable you to use the token to create credentials
        static async Task<string> GetAuthorizationHeaderAsync()
        {
            ClientCredential cc = new ClientCredential(ApplicationId, Password);
            var context = new AuthenticationContext("https://login.windows.net/" + TenantId);
            var result = await context.AcquireTokenAsync("https://management.azure.com/", cc);

            if (result == null)
            {
                throw new InvalidOperationException("Failed to obtain the JWT token. Please verify the values for your applicationId, Password, and Tenant.");
            }

            string token = result.AccessToken;
            return token;
        }

        public static void Main(string[] args)
        {
            PerformTopicAndEventSubscriptionOperations().Wait();
        }

        static async Task PerformTopicAndEventSubscriptionOperations()
        {
            string token = await GetAuthorizationHeaderAsync();
            TokenCredentials credential = new TokenCredentials(token);
            ResourceManagementClient resourcesClient = new ResourceManagementClient(credential)
            {
                SubscriptionId = SubscriptionId
            };

            EventGridManagementClient eventGridManagementClient = new EventGridManagementClient(credential)
            {
                SubscriptionId = SubscriptionId,
                LongRunningOperationRetryTimeout = 2
            };

            try
            {
                // Register the EventGrid Resource Provider with the Subscription
                await RegisterEventGridResourceProviderAsync(resourcesClient);

                // Create a new resource group
                await CreateResourceGroupAsync(ResourceGroupName, resourcesClient);

                // Create a new Event Grid topic in a resource group
                await CreateEventGridTopicAsync(ResourceGroupName, TopicName, eventGridManagementClient);

                // Create an event subscription
                await CreateEventGridEventSubscriptionAsync(ResourceGroupName, TopicName, EventSubscriptionName, eventGridManagementClient, EndpointUrl);

                Console.WriteLine("Press any key to exit...");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
            }
        }

        public static async Task RegisterEventGridResourceProviderAsync(ResourceManagementClient resourcesClient)
        {
            Console.WriteLine("Registering EventGrid Resource Provider with subscription...");
            await resourcesClient.Providers.RegisterAsync("Microsoft.EventGrid");
            Console.WriteLine("EventGrid Resource Provider registered.");
        }

        static async Task CreateResourceGroupAsync(string rgname, ResourceManagementClient resourcesClient)
        {
            Console.WriteLine("Creating a resource group...");
            var resourceGroup = await resourcesClient.ResourceGroups.CreateOrUpdateAsync(
                    rgname,
                    new ResourceGroup
                    {
                        Location = DefaultLocation
                    });
            Console.WriteLine("Resource group created with name " + resourceGroup.Name);
        }

        static async Task CreateEventGridTopicAsync(string rgname, string topicName, EventGridManagementClient EventGridMgmtClient)
        {
            Console.WriteLine("Creating an EventGrid topic...");

            Dictionary<string, string> defaultTags = new Dictionary<string, string>
            {
                {"key1","value1"},
                {"key2","value2"}
            };

            Topic topic = new Topic()
            {
                Tags = defaultTags,
                Location = DefaultLocation,
                InputSchema = InputSchema.EventGridSchema,
                InputSchemaMapping = null
            };

            Topic createdTopic = await EventGridMgmtClient.Topics.CreateOrUpdateAsync(rgname, topicName, topic);
            Console.WriteLine("EventGrid topic created with name " + createdTopic.Name);
        }

        static async Task CreateEventGridEventSubscriptionAsync(string rgname, string topicName, string eventSubscriptionName, EventGridManagementClient eventGridMgmtClient, string endpointUrl)
        {
            Topic topic = await eventGridMgmtClient.Topics.GetAsync(rgname, topicName);
            string eventSubscriptionScope = topic.Id;

            Console.WriteLine($"Creating an event subscription to topic {topicName}...");

            EventSubscription eventSubscription = new EventSubscription()
            {
                Destination = new WebHookEventSubscriptionDestination()
                {
                    EndpointUrl = endpointUrl
                },
                // The below are all optional settings
                EventDeliverySchema = EventDeliverySchema.EventGridSchema,
                Filter = new EventSubscriptionFilter()
                {
                    // By default, "All" event types are included
                    IsSubjectCaseSensitive = false,
                    SubjectBeginsWith = "",
                    SubjectEndsWith = ""
                },
                /* Retry policy decides when an event can be marked as expired. 
                   The default retry policy keeps the event alive for 24 hrs (=1440 mins or 30 retries with exponential backoffs)
                   An event is marked as expired once any of the retry policy limits are exceeded. 
                   Note: The below configuration for the retry policy will cause events to expire after one delivery attempt. 
                   This is only to make it easier to help test/verify dead letter destinations quickly.
                */
                RetryPolicy = new RetryPolicy()
                {
                    MaxDeliveryAttempts = 1,
                    EventTimeToLiveInMinutes = 10,
                },
                // With dead-letter destination configured, all expired events will be delivered to this destination.
                // Note: only Storage Blobs are supported as dead letter destinations as of now.
                DeadLetterDestination = new StorageBlobDeadLetterDestination()
                {
                    ResourceId = DeadLetterDestinationResourceId,
                    BlobContainerName = StorageBlobContainerName,
                }
            };


         try
            {
                EventSubscription createdEventSubscription = await eventGridMgmtClient.EventSubscriptions.CreateOrUpdateAsync(eventSubscriptionScope, eventSubscriptionName, eventSubscription);
                Console.WriteLine("EventGrid event subscription created with name " + createdEventSubscription.Name);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);

                Console.WriteLine(e.StackTrace);
            }

           
        }
    }
}