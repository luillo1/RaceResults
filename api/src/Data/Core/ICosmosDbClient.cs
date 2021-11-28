namespace RaceResults.Data.Core
{
    using Microsoft.Azure.Cosmos;

    public interface ICosmosDbClient
    {
        Container GetContainer(string containerName);
    }
}
