using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using RaceResults.Common.Models;
using RaceResults.Data.Core;

namespace RaceResultsTests.Data.Core
{
    [TestClass]
    public class ContainerClientTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            Assert.IsTrue(true);
        }

        private Container GetMockContainer(List<IModel> includedData)
        {
            List<IModel> database = new List<IModel>();
            Container container = Substitute.For<Container>();

            ItemResponse<IModel> response = Substitute.For<ItemResponse<IModel>>();
            container.ReadItemAsync<IModel>(Arg.Any<string>(), Arg.Any<PartitionKey>()).Returns(x =>
                    {
                        Guid id = (Guid)x[0];
                        PartitionKey partitionKey = (PartitionKey)x[1];

                        // TODO: Make sure theres only one item that matches
                        IModel result = database.Single(model =>
                                model.Id.Equals(id) &&
                                model.GetPartitionKey().Equals(partitionKey));

                        return CreateMockItemResponse(result);
                    });
            container.CreateItemAsync<IModel>(Arg.Any<IModel>()).Returns(x =>
                    {
                        IModel item = (IModel)x[0];

                        // TODO: Make sure this is an insert and not an update
                        database.Add(item);
                        return CreateMockItemResponse(item);
                    });
            container.DeleteItemAsync<IModel>(Arg.Any<string>(), Arg.Any<PartitionKey>()).Returns(x =>
                    {
                        Guid id = (Guid)x[0];
                        PartitionKey partitionKey = (PartitionKey)x[1];

                        // TODO: Make sure theres only one item that matches
                        IModel result = database.Single(model =>
                                model.Id.Equals(id) &&
                                model.GetPartitionKey().Equals(partitionKey));

                        database.Remove(result);
                        return CreateMockItemResponse(result);
                    });
            container.GetItemLinqQueryable<IModel>().Returns(database.AsQueryable());

            return container;
        }

        private ItemResponse<T> CreateMockItemResponse<T>(T item)
        {
            ItemResponse<T> response = Substitute.For<ItemResponse<T>>();
            response.Resource.Returns(item);
            return response;
        }
    }
}
