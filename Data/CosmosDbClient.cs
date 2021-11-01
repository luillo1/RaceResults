using Azure.Identity;
using Microsoft.Azure.Cosmos;

namespace RaceResults.Data
{
    public class CosmosDbClient : ICosmosDbClient
    {
        private const string AccountEndpoint = "https://raceresults-db.documents.azure.com/";

        private const string DatabaseName = "RaceResultsDb";

        private readonly CosmosClient cosmosClient;

        private readonly Database database;

        public CosmosDbClient()
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
                    CosmosDbClient.AccountEndpoint,
                    new DefaultAzureCredential(),
                    clientOptions);
            this.database = this.cosmosClient.GetDatabase(CosmosDbClient.DatabaseName);
        }

        public Container GetContainer(string containerName)
        {
            return this.database.GetContainer(containerName);
        }
    }
}
