using Microsoft.Azure.Cosmos;

namespace RaceResults.Data.Core
{
    public interface ICosmosDbClient
    {
        Container GetContainer(string containerName);
    }
}
