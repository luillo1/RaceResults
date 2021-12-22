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
        public async Task GetOneAsyncTest()
        {
            List<IModel> includedData = new List<IModel>();
            IModel item = new SampleModel();
            includedData.Add(item);

            ICosmosDbClient cosmosDbClient = GetMockCosmosClient(includedData);
            ContainerClientConcrete containerClient = new ContainerClientConcrete(cosmosDbClient, "ContainerName");

            IModel result = await containerClient.GetOneAsync(item.Id.ToString(), item.GetPartitionKey());

            Assert.AreEqual(1, includedData.Count);
            Assert.AreEqual(item, result);
            Assert.IsTrue(includedData.Contains(item));
        }

        [TestMethod]
        public async Task GetManyAsyncTest()
        {
            List<IModel> includedData = new List<IModel>();
            SampleModel item1 = new SampleModel();
            includedData.Add(item1);

            SampleModel item2 = new SampleModel();
            includedData.Add(item2);

            SampleModel item3 = new SampleModel();
            includedData.Add(item3);

            ICosmosDbClient cosmosDbClient = GetMockCosmosClient(includedData);
            ContainerClientConcrete containerClient = new ContainerClientConcrete(cosmosDbClient, "ContainerName");

            IEnumerable<IModel> output = await containerClient.GetManyAsync(x => x.Where(_ => true));
            List<IModel> result = output.ToList();

            Assert.AreEqual(3, includedData.Count);
            Assert.AreEqual(3, result.Count);
            Assert.IsTrue(result.Contains(item1));
            Assert.IsTrue(result.Contains(item2));
            Assert.IsTrue(result.Contains(item3));
        }

        [TestMethod]
        public async Task GetAllAsyncTest()
        {
            List<IModel> includedData = new List<IModel>();
            SampleModel item1 = new SampleModel();
            includedData.Add(item1);

            SampleModel item2 = new SampleModel();
            includedData.Add(item2);

            SampleModel item3 = new SampleModel();
            includedData.Add(item3);

            ICosmosDbClient cosmosDbClient = GetMockCosmosClient(includedData);
            ContainerClientConcrete containerClient = new ContainerClientConcrete(cosmosDbClient, "ContainerName");

            IEnumerable<IModel> output = await containerClient.GetAllAsync();
            List<IModel> result = output.ToList();

            Assert.AreEqual(3, includedData.Count);
            Assert.AreEqual(3, result.Count);
            Assert.IsTrue(result.Contains(item1));
            Assert.IsTrue(result.Contains(item2));
            Assert.IsTrue(result.Contains(item3));
        }

        [TestMethod]
        public async Task AddOneAsyncTest()
        {
            List<IModel> includedData = new List<IModel>();
            ICosmosDbClient cosmosDbClient = GetMockCosmosClient(includedData);
            ContainerClientConcrete containerClient = new ContainerClientConcrete(cosmosDbClient, "ContainerName");

            IModel item = new SampleModel();
            await containerClient.AddOneAsync(item);

            Assert.AreEqual(1, includedData.Count);
            Assert.IsTrue(includedData.Contains(item));
        }

        [TestMethod]
        public async Task DeleteOneAsyncTest()
        {
            List<IModel> includedData = new List<IModel>();
            SampleModel item = new SampleModel();
            includedData.Add(item);

            ICosmosDbClient cosmosDbClient = GetMockCosmosClient(includedData);
            ContainerClientConcrete containerClient = new ContainerClientConcrete(cosmosDbClient, "ContainerName");

            await containerClient.DeleteOneAsync(item.Id.ToString(), item.GetPartitionKey());

            Assert.AreEqual(0, includedData.Count);
        }

        private ICosmosDbClient GetMockCosmosClient(List<IModel> includedData)
        {
            ICosmosDbClient result = Substitute.For<ICosmosDbClient>();
            Container container = GetMockContainer(includedData);
            result.GetContainer(Arg.Any<string>()).Returns(container);

            return result;
        }

        private Container GetMockContainer(List<IModel> includedData)
        {
            Container container = Substitute.For<Container>();

            ItemResponse<IModel> response = Substitute.For<ItemResponse<IModel>>();
            container.ReadItemAsync<IModel>(Arg.Any<string>(), Arg.Any<PartitionKey>()).Returns(x =>
                    {
                        Guid id = Guid.Parse((string)x[0]);
                        PartitionKey partitionKey = (PartitionKey)x[1];

                        IModel result = includedData.Single(model =>
                                model.Id.Equals(id) &&
                                new PartitionKey(model.GetPartitionKey()) == partitionKey);

                        return CreateMockItemResponse(result);
                    });
            container.CreateItemAsync<IModel>(Arg.Any<IModel>()).Returns(x =>
                    {
                        IModel item = (IModel)x[0];

                        // TODO: Make sure this is an insert and not an update
                        includedData.Add(item);
                        return CreateMockItemResponse(item);
                    });
            container.DeleteItemAsync<IModel>(Arg.Any<string>(), Arg.Any<PartitionKey>()).Returns(x =>
                    {
                        Guid id = Guid.Parse((string)x[0]);
                        PartitionKey partitionKey = (PartitionKey)x[1];

                        // TODO: Make sure theres only one item that matches
                        IModel result = includedData.Single(model =>
                                model.Id.Equals(id) &&
                                new PartitionKey(model.GetPartitionKey()) == partitionKey);

                        includedData.Remove(result);
                        return CreateMockItemResponse(result);
                    });
            container.GetItemLinqQueryable<IModel>().Returns(includedData.AsQueryable());

            return container;
        }

        private ItemResponse<T> CreateMockItemResponse<T>(T item)
        {
            ItemResponse<T> response = Substitute.For<ItemResponse<T>>();
            response.Resource.Returns(item);
            return response;
        }
    }

    public class ContainerClientConcrete : ContainerClient<IModel>
    {
        public ContainerClientConcrete(ICosmosDbClient client, string containerName)
            : base(client, containerName)
        {
        }
    }

    public class SampleModel : IModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string GetPartitionKey()
        {
            return Id.ToString();
        }
    }
}
