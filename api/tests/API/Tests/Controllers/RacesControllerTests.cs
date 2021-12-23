using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Internal.RaceResults.Api.Utils;
using Internal.RaceResults.Data.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RaceResults.Api.Controllers;
using RaceResults.Common.Models;
using RaceResults.Data.Core;

namespace Internal.RaceResults.Api.Controllers
{
    [TestClass]
    public class RacesControllerTests
    {
        private static List<Race> races = new List<Race>()
        {
            new Race()
            {
                Id = Guid.NewGuid(),
                Name = "Ben's Annual Race",
                Date = DateTime.Now,
                Location = "Ben's House",
                Distance = "5k",
            },
            new Race()
            {
                Id = Guid.NewGuid(),
                Name = "Alyssa's Annual Race",
                Date = DateTime.Now,
                Location = "Alyssa's House",
                Distance = "Marathon",
            },
            new Race()
            {
                Id = Guid.NewGuid(),
                Name = "Jane's Annual Race",
                Date = DateTime.Now,
                Location = "Jane's House",
                Distance = "10k",
            },
        };

        private RacesController controller;

        [TestInitialize]
        public void TestInitialize()
        {
            Container raceContainer = MockContainerProvider<Race>.CreateMockContainer(races);

            MockCosmosDbClient cosmosDbClient = new MockCosmosDbClient();
            cosmosDbClient.AddEmptyMemberContainer();
            cosmosDbClient.AddEmptyOrganizationContainer();
            cosmosDbClient.AddNewContainer(ContainerConstants.RaceContainerName, raceContainer);
            cosmosDbClient.AddEmptyRaceResultContainer();

            ICosmosDbContainerProvider provider = new CosmosDbContainerProvider(cosmosDbClient);
            controller = new RacesController(provider, NullLogger<RacesController>.Instance);
        }

        [TestMethod]
        public async Task GetOneTest()
        {
            Race raceToFind = races.First();
            Guid raceId = raceToFind.Id;

            IActionResult result = await controller.GetOne(raceId.ToString());
            ValidationTools.AssertFoundItem(raceToFind, result);
        }

        [TestMethod]
        public async Task GetAllTest()
        {
            IActionResult result = await controller.GetAll();
            HashSet<Race> expectedResult = races.ToHashSet();
            Assert.IsTrue(expectedResult.Count > 1, "Expected to query for more than 1 race.");
            ValidationTools.AssertFoundItems(expectedResult, result);
        }

        [TestMethod]
        public async Task CreateTest()
        {
            Race race = new Race()
            {
                Name = "Ben's Annual Race",
                Date = DateTime.Now,
                Location = "Ben's House",
                Distance = "5k",
            };

            IActionResult result = await controller.Create(race);
            Assert.IsInstanceOfType(result, typeof(CreatedAtActionResult));
            Assert.IsNotNull(race.Id);
        }
    }
}
