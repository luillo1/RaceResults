using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using RaceResults.Common.Models;

namespace RaceResults.Data.Core
{
    public abstract class ContainerClient<T>
        where T : IModel
    {
        private readonly Container container;

        public ContainerClient(ICosmosDbClient client, string containerName)
        {
            this.container = client.GetContainer(containerName);
        }

        public async Task<T> GetOneAsync(string id, string partitionKey)
        {
            PartitionKey partition = new PartitionKey(partitionKey);
            ItemResponse<T> response = await this.container.ReadItemAsync<T>(id, partition);
            return response.Resource;
        }

        public async Task<IEnumerable<T>> GetManyAsync(Func<IQueryable<T>, IQueryable<T>> iteratorCreator)
        {
            return (await GetManyAsDictAsync(iteratorCreator)).Values;
        }

        public async Task<IDictionary<Guid, T>> GetManyAsDictAsync(Func<IQueryable<T>, IQueryable<T>> iteratorCreator)
        {
            IQueryable<T> queryable = this.container.GetItemLinqQueryable<T>();
            FeedIterator<T> iterator = iteratorCreator(queryable).ToFeedIterator();

            return await ConstructDict(iterator);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return (await GetAllAsDictAsync()).Values;
        }

        public async Task<IDictionary<Guid, T>> GetAllAsDictAsync()
        {
            FeedIterator<T> iterator = this.container.GetItemLinqQueryable<T>()
                                                     .ToFeedIterator();

            return await ConstructDict(iterator);
        }

        public async Task AddOneAsync(T item)
        {
            item.Id = Guid.NewGuid();
            await this.container.CreateItemAsync<T>(item);
        }

        public async Task DeleteOneAsync(string id, string partitionKey)
        {
            PartitionKey partition = new PartitionKey(partitionKey);
            await this.container.DeleteItemAsync<T>(id, partition);
        }

        private static async Task<IDictionary<Guid, T>> ConstructDict(FeedIterator<T> iterator)
        {
            Dictionary<Guid, T> results = new Dictionary<Guid, T>();
            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                foreach (var item in response)
                {
                    if (results.ContainsKey(item.Id))
                    {
                        throw new InvalidOperationException($"Duplicate GUID {item.Id} found for {typeof(T)}");
                    }

                    results[item.Id] = item;
                }
            }

            return results;
        }
    }
}
