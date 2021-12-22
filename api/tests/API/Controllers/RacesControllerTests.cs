using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RaceResults.Api.Controllers;
using RaceResults.Common.Models;
using RaceResults.Data.Core;
using RaceResultsTests.Data.Core;

namespace RaceResultsTests.Api.Controllers
{
    [TestClass]
    public class RacesControllerTests
    {
        [TestMethod]
        public async Task GetOneTest()
        {
            List<Race> data = new List<Race>();
            Guid raceId = Guid.NewGuid();
            Race race = new Race()
            {
                Id = raceId,
                Name = "Ben's Annual Race",
                Date = DateTime.Now,
                Location = "Ben's House",
                Distance = "5k",
            };
            data.Add(race);
            ICosmosDbClient client = Utils<Race>.GetMockCosmosClient(data);
            ICosmosDbContainerProvider provider = new CosmosDbContainerProvider(client);
            RacesController controller = new RacesController(provider, NullLogger<RacesController>.Instance);

            IActionResult result = await controller.GetOne(raceId.ToString());
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.IsTrue(data.Contains(race));
            Assert.AreEqual(1, data.Count);
        }

        [TestMethod]
        public async Task GetAllTest()
        {
            List<Race> data = new List<Race>();
            Guid raceId = Guid.NewGuid();
            Race race = new Race()
            {
                Id = raceId,
                Name = "Ben's Annual Race",
                Date = DateTime.Now,
                Location = "Ben's House",
                Distance = "5k",
            };
            data.Add(race);
            ICosmosDbClient client = Utils<Race>.GetMockCosmosClient(data);
            ICosmosDbContainerProvider provider = new CosmosDbContainerProvider(client);
            RacesController controller = new RacesController(provider, NullLogger<RacesController>.Instance);

            IActionResult result = await controller.GetAll();
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.IsTrue(data.Contains(race));
            Assert.AreEqual(1, data.Count);
        }

        [TestMethod]
        public async Task CreateTest()
        {
            List<Race> data = new List<Race>();
            Race race = new Race()
            {
                Name = "Ben's Annual Race",
                Date = DateTime.Now,
                Location = "Ben's House",
                Distance = "5k",
            };
            ICosmosDbClient client = Utils<Race>.GetMockCosmosClient(data);
            ICosmosDbContainerProvider provider = new CosmosDbContainerProvider(client);
            RacesController controller = new RacesController(provider, NullLogger<RacesController>.Instance);

            IActionResult result = await controller.Create(race);
            Assert.IsInstanceOfType(result, typeof(CreatedAtActionResult));
            Assert.IsNotNull(race.Id);
            Assert.IsTrue(data.Contains(race));
            Assert.AreEqual(1, data.Count);
        }
    }
}
