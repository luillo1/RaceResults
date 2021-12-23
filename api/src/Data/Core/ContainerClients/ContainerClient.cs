using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using RaceResults.Common.Models;

namespace RaceResults.Data.Core
{
    public abstract class ContainerClient<T>
        where T : IModel
    {
        private readonly Container container;

        public ContainerClient(Container container)
        {
            this.container = container;
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
            IQueryable<T> iterator = iteratorCreator(queryable);

            return await ConstructDict(iterator);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return (await GetAllAsDictAsync()).Values;
        }

        public async Task<IDictionary<Guid, T>> GetAllAsDictAsync()
        {
            IQueryable<T> iterator = this.container.GetItemLinqQueryable<T>();

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

        private static Task<IDictionary<Guid, T>> ConstructDict(IQueryable<T> iterator)
        {
            IDictionary<Guid, T> results = new Dictionary<Guid, T>();
            foreach (T item in iterator)
            {
                if (results.ContainsKey(item.Id))
                {
                    throw new InvalidOperationException($"Duplicate GUID {item.Id} found for {typeof(T)}");
                }

                results[item.Id] = item;
            }

            return Task.FromResult(results);
        }
    }
}
