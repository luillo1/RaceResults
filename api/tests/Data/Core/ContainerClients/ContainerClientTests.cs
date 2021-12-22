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
        public class ContainerClientConcrete : ContainerClient<SampleModel>
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

        [TestMethod]
        public async Task GetOneAsyncTest()
        {
            List<SampleModel> includedData = new List<SampleModel>();
            SampleModel item = new SampleModel();
            includedData.Add(item);

            ICosmosDbClient cosmosDbClient = Utils<SampleModel>.GetMockCosmosClient(includedData);
            ContainerClientConcrete containerClient = new ContainerClientConcrete(cosmosDbClient, "ContainerName");

            SampleModel result = await containerClient.GetOneAsync(item.Id.ToString(), item.GetPartitionKey());

            Assert.AreEqual(1, includedData.Count);
            Assert.AreEqual(item, result);
            Assert.IsTrue(includedData.Contains(item));
        }

        [TestMethod]
        public async Task GetManyAsyncTest()
        {
            List<SampleModel> includedData = new List<SampleModel>();
            SampleModel item1 = new SampleModel();
            includedData.Add(item1);

            SampleModel item2 = new SampleModel();
            includedData.Add(item2);

            SampleModel item3 = new SampleModel();
            includedData.Add(item3);

            ICosmosDbClient cosmosDbClient = Utils<SampleModel>.GetMockCosmosClient(includedData);
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
            List<SampleModel> includedData = new List<SampleModel>();
            SampleModel item1 = new SampleModel();
            includedData.Add(item1);

            SampleModel item2 = new SampleModel();
            includedData.Add(item2);

            SampleModel item3 = new SampleModel();
            includedData.Add(item3);

            ICosmosDbClient cosmosDbClient = Utils<SampleModel>.GetMockCosmosClient(includedData);
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
            List<SampleModel> includedData = new List<SampleModel>();
            ICosmosDbClient cosmosDbClient = Utils<SampleModel>.GetMockCosmosClient(includedData);
            ContainerClientConcrete containerClient = new ContainerClientConcrete(cosmosDbClient, "ContainerName");

            SampleModel item = new SampleModel();
            await containerClient.AddOneAsync(item);

            Assert.AreEqual(1, includedData.Count);
            Assert.IsTrue(includedData.Contains(item));
        }

        [TestMethod]
        public async Task DeleteOneAsyncTest()
        {
            List<SampleModel> includedData = new List<SampleModel>();
            SampleModel item = new SampleModel();
            includedData.Add(item);

            ICosmosDbClient cosmosDbClient = Utils<SampleModel>.GetMockCosmosClient(includedData);
            ContainerClientConcrete containerClient = new ContainerClientConcrete(cosmosDbClient, "ContainerName");

            await containerClient.DeleteOneAsync(item.Id.ToString(), item.GetPartitionKey());

            Assert.AreEqual(0, includedData.Count);
        }
    }
}
