using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Internal.RaceResults.Data.Utils;
using Microsoft.Azure.Cosmos;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RaceResults.Common.Models;
using RaceResults.Data.Core;

namespace Internal.RaceResults.Data.Core
{
    [TestClass]
    public class ContainerClientTests
    {
        public class ContainerClientConcrete : ContainerClient<SampleModel>
        {
            public ContainerClientConcrete(Container container)
                : base(container)
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

            Container container = MockContainerProvider<SampleModel>.CreateMockContainer(includedData);
            ContainerClientConcrete containerClient = new ContainerClientConcrete(container);

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

            Container container = MockContainerProvider<SampleModel>.CreateMockContainer(includedData);
            ContainerClientConcrete containerClient = new ContainerClientConcrete(container);

            IEnumerable<SampleModel> output = await containerClient.GetManyAsync(x => x.Where(_ => true));
            List<SampleModel> result = output.ToList();

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

            Container container = MockContainerProvider<SampleModel>.CreateMockContainer(includedData);
            ContainerClientConcrete containerClient = new ContainerClientConcrete(container);

            IEnumerable<SampleModel> output = await containerClient.GetAllAsync();
            List<SampleModel> result = output.ToList();

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

            Container container = MockContainerProvider<SampleModel>.CreateMockContainer(includedData);
            ContainerClientConcrete containerClient = new ContainerClientConcrete(container);

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

            Container container = MockContainerProvider<SampleModel>.CreateMockContainer(includedData);
            ContainerClientConcrete containerClient = new ContainerClientConcrete(container);

            await containerClient.DeleteOneAsync(item.Id.ToString(), item.GetPartitionKey());

            Assert.AreEqual(0, includedData.Count);
        }
    }
}
