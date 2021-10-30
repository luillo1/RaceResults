using Azure.Identity;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;

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
            CosmosClientBuilder builder = new CosmosClientBuilder(
                "https://raceresults-db.documents.azure.com/",
                new DefaultAzureCredential());

            this.cosmosClient = builder.Build();
            this.database = this.cosmosClient.CreateDatabaseIfNotExistsAsync(CosmosDbClient.DatabaseName).GetAwaiter().GetResult();
        }

        public Container GetContainer(string containerName)
        {
            return this.database.GetContainer(containerName);
        }
    }
}
