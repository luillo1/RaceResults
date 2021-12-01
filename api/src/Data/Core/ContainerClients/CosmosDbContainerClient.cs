using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

namespace RaceResults.Data.Core
{
    public class CosmosDbContainerClient<T> : ICosmosDbContainerClient<T>
    {
        private readonly Container container;

        public CosmosDbContainerClient(ICosmosDbClient client, string containerName)
        {
            this.container = client.GetContainer(containerName);
        }

        public async Task<T> GetItemAsync(string id, string partitionKey)
        {
            PartitionKey partition = new PartitionKey(partitionKey);
            ItemResponse<T> response = await this.container.ReadItemAsync<T>(id, partition);
            return response.Resource;
        }

        public async Task<IEnumerable<T>> GetAllItemsAsync()
        {
            FeedIterator<T> iterator = this.container.GetItemLinqQueryable<T>()
                                                     .ToFeedIterator();

            List<T> results = new List<T>();
            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task AddItemAsync(T item)
        {
            await this.container.CreateItemAsync<T>(item);
        }

        public async Task DeleteItemAsync(string id, string partitionKey)
        {
            PartitionKey partition = new PartitionKey(partitionKey);
            await this.container.DeleteItemAsync<T>(id, partition);
        }
    }
}
