using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.Cosmos;
using NSubstitute;
using RaceResults.Common.Models;

namespace Internal.RaceResults.Data.Utils
{
    public static class MockContainerProvider<T>
        where T : IModel
    {
        public static Container CreateMockContainer(List<T> data)
        {
            Container container = Substitute.For<Container>();

            container.ReadItemAsync<T>(Arg.Any<string>(), Arg.Any<PartitionKey>()).Returns(x =>
                    {
                        Guid id = Guid.Parse((string)x[0]);
                        PartitionKey partitionKey = (PartitionKey)x[1];

                        T document = data.Single(model =>
                                model.Id.Equals(id) &&
                                new PartitionKey(model.GetPartitionKey()) == partitionKey);

                        ItemResponse<T> response = Substitute.For<ItemResponse<T>>();
                        response.Resource.Returns(document);

                        return response;
                    });

            container.CreateItemAsync<T>(Arg.Any<T>()).Returns(x =>
                    {
                        T document = (T)x[0];

                        // TODO: Make sure this is an insert and not an update
                        data.Add(document);

                        ItemResponse<T> response = Substitute.For<ItemResponse<T>>();
                        response.Resource.Returns(document);

                        return response;
                    });

            container.DeleteItemAsync<T>(Arg.Any<string>(), Arg.Any<PartitionKey>()).Returns(x =>
                    {
                        Guid id = Guid.Parse((string)x[0]);
                        PartitionKey partitionKey = (PartitionKey)x[1];

                        T document = data.Single(model =>
                                model.Id.Equals(id) &&
                                new PartitionKey(model.GetPartitionKey()) == partitionKey);

                        data.Remove(document);

                        ItemResponse<T> response = Substitute.For<ItemResponse<T>>();
                        response.Resource.Returns(document);

                        return response;
                    });

            container.GetItemLinqQueryable<T>().Returns(data.AsQueryable());

            return container;
        }
    }
}
