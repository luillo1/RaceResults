using Azure.Identity;
using Microsoft.Azure.Cosmos;

namespace RaceResults.Data.CosmosDb
{
    public class CosmosDbClient : ICosmosDbClient
    {
        private readonly CosmosClient cosmosClient;

        private readonly Database database;

        public CosmosDbClient(string endpoint, string databaseName)
        {
            CosmosClientOptions clientOptions = new CosmosClientOptions()
            {
                SerializerOptions = new CosmosSerializationOptions()
                {
                    IgnoreNullValues = true,
                    PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
                }
            };

            this.cosmosClient = new CosmosClient(
                    endpoint,
                    new DefaultAzureCredential(),
                    clientOptions);
            this.database = this.cosmosClient.GetDatabase(databaseName);
        }

        public Container GetContainer(string containerName)
        {
            return this.database.GetContainer(containerName);
        }
    }
}
