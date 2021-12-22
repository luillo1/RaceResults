using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.Cosmos;
using NSubstitute;
using RaceResults.Common.Models;
using RaceResults.Data.Core;

namespace Internal.RaceResults.Data.Utils
{
    public static class Utils<T>
       where T : IModel
    {
        public static ICosmosDbClient GetMockCosmosClient(List<T> includedData)
        {
            ICosmosDbClient result = Substitute.For<ICosmosDbClient>();
            Container container = Utils<T>.GetMockContainer(includedData);
            result.GetContainer(Arg.Any<string>()).Returns(container);

            return result;
        }

        private static Container GetMockContainer(List<T> includedData)
        {
            Container container = Substitute.For<Container>();

            ItemResponse<T> response = Substitute.For<ItemResponse<T>>();
            container.ReadItemAsync<T>(Arg.Any<string>(), Arg.Any<PartitionKey>()).Returns(x =>
                    {
                        Guid id = Guid.Parse((string)x[0]);
                        PartitionKey partitionKey = (PartitionKey)x[1];

                        T result = includedData.Single(model =>
                                model.Id.Equals(id) &&
                                new PartitionKey(model.GetPartitionKey()) == partitionKey);

                        return Utils<T>.CreateMockItemResponse(result);
                    });
            container.CreateItemAsync<T>(Arg.Any<T>()).Returns(x =>
                    {
                        T item = (T)x[0];

                        // TODO: Make sure this is an insert and not an update
                        includedData.Add(item);
                        return Utils<T>.CreateMockItemResponse(item);
                    });
            container.DeleteItemAsync<T>(Arg.Any<string>(), Arg.Any<PartitionKey>()).Returns(x =>
                    {
                        Guid id = Guid.Parse((string)x[0]);
                        PartitionKey partitionKey = (PartitionKey)x[1];

                        // TODO: Make sure theres only one item that matches
                        T result = includedData.Single(model =>
                                model.Id.Equals(id) &&
                                new PartitionKey(model.GetPartitionKey()) == partitionKey);

                        includedData.Remove(result);
                        return Utils<T>.CreateMockItemResponse(result);
                    });
            container.GetItemLinqQueryable<T>().Returns(includedData.AsQueryable());

            return container;
        }

        private static ItemResponse<T> CreateMockItemResponse(T item)
        {
            ItemResponse<T> response = Substitute.For<ItemResponse<T>>();
            response.Resource.Returns(item);
            return response;
        }
    }
}
