using Microsoft.Azure.Cosmos;

namespace RaceResults.Data.CosmosDb
{
    public interface ICosmosDbClient
    {
        Container GetContainer(string containerName);
    }
}
