using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using RaceResults.Common.Models;

namespace RaceResults.Data.Core
{
    /// <summary>
    ///     Provides common methods for interacting with a <see cref="Microsoft.Azure.Cosmos.Container"/>.
    /// </summary>
    /// <typeparam name="T">
    ///     The <see cref="IModel"/> this container client is for.
    /// </typeparam>
    public abstract class ContainerClient<T>
        where T : IModel
    {
        /// <summary>
        ///     Gets the <see cref="Microsoft.Azure.Cosmos.Container"/> deriving classes can use to interact with
        ///     the database.
        /// </summary>
        protected Container Container { get; }

        /// <summary>
        ///     Gets the name of the container.
        /// </summary>
        protected abstract string ContainerName { get; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ContainerClient{T}"/> class.
        /// </summary>
        /// <param name="cosmosDbClient">
        ///     The <see cref="ICosmosDbClient"/> used to get an instance of a <see cref="Container"/>.
        /// </param>
        public ContainerClient(ICosmosDbClient cosmosDbClient)
        {
            this.Container = cosmosDbClient.GetContainer(this.ContainerName);
        }

        /// <summary>
        ///     Queries the container for an instance of <typeparamref name="T"/>
        ///     that has a <see cref="IModel.Id"/> corresponding to <paramref name="id"/>.
        /// </summary>
        /// <param name="id">
        ///     The <see cref="Guid"/> of the <see cref="IModel"/> to query.
        /// </param>
        /// <param name="partitionKey">
        ///     The <see cref="PartitionKey"/> for the partition that should be queried.
        /// </param>
        /// <returns>
        ///     An await-able task that queries the container and returns the result.
        /// </returns>
        public async Task<T> GetModelAsync(string id, PartitionKey partitionKey)
        {
            ItemResponse<T> response = await this.Container.ReadItemAsync<T>(id, partitionKey);
            return response.Resource;
        }

        /// <summary>
        ///     Queries the container for all instances of <typeparamref name="T"/>
        ///     that satisfy the given <paramref name="filter"/>
        /// </summary>
        /// <param name="iteratorCreator">
        ///     A method maps the base <see cref="IQueryable{T}"/> to the final <see cref="IQueryable{T}"/>
        ///     that should be used to construct the iterator of objects that should be returned.
        /// </param>
        /// <returns>
        ///     An await-able task that queries the container and returns the result.
        /// </returns>
        /// <example>
        ///     A caller can use this method to query for all models that satisfies some condition. For example:
        ///     <code>
        ///         GetModelsAsync(it => it.Where(myModel => myModel.HasProperty))
        ///     </code>
        /// </example>
        public async Task<IEnumerable<T>> GetModelsAsync(Func<IQueryable<T>, IQueryable<T>> iteratorCreator)
        {
            var queryable = this.Container.GetItemLinqQueryable<T>();

            var feedIterator = iteratorCreator(queryable).ToFeedIterator();

            return await ConstructList(feedIterator);
        }

        /// <summary>
        ///     Queries the container for all instances of <typeparamref name="T"/>.
        /// </summary>
        /// <returns>
        ///     An await-able task that queries the container and returns the result.
        /// </returns>
        public async Task<IEnumerable<T>> GetAllModelsAsync()
        {
            FeedIterator<T> iterator = this.Container.GetItemLinqQueryable<T>().ToFeedIterator();
            return await ConstructList(iterator);
        }

        /// <summary>
        ///     Gives the given instance of <typeparamref name="T"/> a new
        ///     <see cref="IModel.Id"/> and adds it to the container.
        /// </summary>
        /// <param name="item">
        ///     The instance of <typeparamref name="T"/> to add to the container.
        /// </param>
        /// <returns>
        ///     An await-able task that adds the given <paramref name="item"/> to
        ///     the container and returns nothing.
        /// </returns>
        public async Task AddModelAsync(T item)
        {
            item.Id = Guid.NewGuid();
            await this.Container.CreateItemAsync(item, new PartitionKey(item.GetPartitionKey()));
        }

        private static async Task<IEnumerable<T>> ConstructList(FeedIterator<T> iterator)
        {
            List<T> results = new List<T>();
            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results;
        }
    }
}
