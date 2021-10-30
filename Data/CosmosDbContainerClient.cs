using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RaceResults.Data
{
    public class CosmosDbContainerClient<T> : ICosmosDbContainerClient<T>
    {
        private readonly Container container;

        public CosmosDbContainerClient(ICosmosDbClient cosmosDbClient, string containerName)
        {
            this.container = cosmosDbClient.GetContainer(containerName);
        }

        public async Task AddItemAsync(T item)
        {
            await this.container.CreateItemAsync<T>(item);
        }

        public async Task DeleteItemAsync(string id, PartitionKey partitionKey)
        {
            await this.container.DeleteItemAsync<T>(id, partitionKey);
        }

        public async Task<T> GetItemAsync(string id, PartitionKey partitionKey)
        {
            ItemResponse<T> response = await this.container.ReadItemAsync<T>(id, partitionKey);
            return response.Resource;
        }

        public async Task<IEnumerable<T>> GetItemsAsync()
        {
            IOrderedQueryable<T> queryable = this.container.GetItemLinqQueryable<T>();
            FeedIterator<T> iterator = queryable.ToFeedIterator();

            List<T> results = new List<T>();
            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task UpdateItemAsync(T item, PartitionKey partitionKey)
        {
            await this.container.UpsertItemAsync<T>(item, partitionKey);
        }
    }
}
