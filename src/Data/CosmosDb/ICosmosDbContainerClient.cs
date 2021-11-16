using Microsoft.Azure.Cosmos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RaceResults.Data.CosmosDb
{
    public interface ICosmosDbContainerClient<T>
    {
        Task AddItemAsync(T item);

        Task DeleteItemAsync(string id, PartitionKey partitionKey);

        Task<T> GetItemAsync(string id, PartitionKey partitionKey);

        Task<IEnumerable<T>> GetItemsAsync();

        Task UpdateItemAsync(T item, PartitionKey partitionKey);
    }
}
