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
            IQueryable<T> queryable = this.container.GetItemLinqQueryable<T>();
            List<T> result = iteratorCreator(queryable).ToList();

            return result;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            List<T> result = this.container.GetItemLinqQueryable<T>().ToList();

            return result;
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
    }
}
