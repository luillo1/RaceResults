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

        private static List<SampleModel> models = new List<SampleModel>()
        {
            new SampleModel()
            {
                Id = Guid.NewGuid(),
            },
            new SampleModel()
            {
                Id = Guid.NewGuid(),
            },
            new SampleModel()
            {
                Id = Guid.NewGuid(),
            },
        };

        private ContainerClientConcrete containerClient;

        [TestInitialize]
        public void TestInitialize()
        {
            Container container = MockContainerProvider<SampleModel>.CreateMockContainer(models);
            containerClient = new ContainerClientConcrete(container);
        }

        [TestMethod]
        public async Task GetOneAsyncTest()
        {
            SampleModel item = models.First();
            SampleModel result = await containerClient.GetOneAsync(item.Id.ToString(), item.GetPartitionKey());
            Assert.AreEqual(item, result);
        }

        [TestMethod]
        public async Task GetManyAsyncTest()
        {
            IEnumerable<SampleModel> output = await containerClient.GetManyAsync(x => x.Where(_ => true));

            Assert.IsTrue(models.ToHashSet().SetEquals(output));
        }

        [TestMethod]
        public async Task GetAllAsyncTest()
        {
            IEnumerable<SampleModel> output = await containerClient.GetAllAsync();

            Assert.IsTrue(models.ToHashSet().SetEquals(output));
        }

        [TestMethod]
        public async Task AddOneAsyncTest()
        {
            SampleModel item = new SampleModel();
            await containerClient.AddOneAsync(item);

            Assert.IsTrue(models.Contains(item));
        }

        [TestMethod]
        public async Task DeleteOneAsyncTest()
        {
            SampleModel item = models.First();
            await containerClient.DeleteOneAsync(item.Id.ToString(), item.GetPartitionKey());

            Assert.IsTrue(!models.Contains(item));
        }
    }
}
