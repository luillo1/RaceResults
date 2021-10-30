using Microsoft.Azure.Cosmos;

namespace RaceResults.Data
{
    public interface ICosmosDbClient
    {
        Container GetContainer(string containerName);
    }
}
