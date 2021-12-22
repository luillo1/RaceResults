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
    public class RaceResultsControllerTests
    {
        [TestMethod]
        public async Task GetAllRaceResultsSingleMemberTest()
        {
            List<RaceResult> data = new List<RaceResult>();
            Guid orgId = Guid.NewGuid();
            Guid memberId = Guid.NewGuid();
            Guid raceId = Guid.NewGuid();
            RaceResult raceResult = new RaceResult()
            {
                Id = raceId,
                MemberId = memberId,
                RaceId = raceId,
                Time = TimeSpan.FromHours(2.5),
                DataSource = "Manual Entry",
            };
            data.Add(raceResult);
            ICosmosDbClient client = Utils<RaceResult>.GetMockCosmosClient(data);
            ICosmosDbContainerProvider provider = new CosmosDbContainerProvider(client);
            RaceResultsController controller = new RaceResultsController(provider, NullLogger<RaceResultsController>.Instance);

            IActionResult result = await controller.GetAllRaceResults(orgId.ToString(), memberId.ToString());
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.IsTrue(data.Contains(raceResult));
            Assert.AreEqual(1, data.Count);
        }

        [TestMethod]
        public void GetAllRaceResultsWholeOrganizationTest()
        {
            // TODO: Need to refactor mock utilities to enable access to multiple containers
        }

        [TestMethod]
        public async Task GetOneRaceResultTest()
        {
            List<RaceResult> data = new List<RaceResult>();
            Guid orgId = Guid.NewGuid();
            Guid memberId = Guid.NewGuid();
            Guid raceId = Guid.NewGuid();
            RaceResult raceResult = new RaceResult()
            {
                Id = raceId,
                MemberId = memberId,
                RaceId = raceId,
                Time = TimeSpan.FromHours(2.5),
                DataSource = "Manual Entry",
            };
            data.Add(raceResult);
            ICosmosDbClient client = Utils<RaceResult>.GetMockCosmosClient(data);
            ICosmosDbContainerProvider provider = new CosmosDbContainerProvider(client);
            RaceResultsController controller = new RaceResultsController(provider, NullLogger<RaceResultsController>.Instance);

            IActionResult result = await controller.GetOneRaceResult(orgId.ToString(), memberId.ToString(), raceId.ToString());
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.IsTrue(data.Contains(raceResult));
            Assert.AreEqual(1, data.Count);
        }

        [TestMethod]
        public async Task CreateTest()
        {
            List<RaceResult> data = new List<RaceResult>();
            Guid orgId = Guid.NewGuid();
            Guid memberId = Guid.NewGuid();
            Guid raceId = Guid.NewGuid();
            RaceResult raceResult = new RaceResult()
            {
                MemberId = memberId,
                RaceId = raceId,
                Time = TimeSpan.FromHours(2.5),
                DataSource = "Manual Entry",
            };
            ICosmosDbClient client = Utils<RaceResult>.GetMockCosmosClient(data);
            ICosmosDbContainerProvider provider = new CosmosDbContainerProvider(client);
            RaceResultsController controller = new RaceResultsController(provider, NullLogger<RaceResultsController>.Instance);

            IActionResult result = await controller.Create(orgId.ToString(), memberId.ToString(), raceResult);
            Assert.IsInstanceOfType(result, typeof(CreatedAtActionResult));
            Assert.IsNotNull(raceResult.Id);
            Assert.IsTrue(data.Contains(raceResult));
            Assert.AreEqual(1, data.Count);
        }
    }
}
