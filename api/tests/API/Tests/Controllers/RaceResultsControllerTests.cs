using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
            Container raceResultContainer = MockContainerProvider<RaceResult>.CreateMockContainer(data);

            MockCosmosDbClient cosmosDbClient = new MockCosmosDbClient();
            cosmosDbClient.AddEmptyMemberContainer();
            cosmosDbClient.AddEmptyOrganizationContainer();
            cosmosDbClient.AddEmptyRaceContainer();
            cosmosDbClient.AddNewContainer(ContainerConstants.RaceResultContainerName, raceResultContainer);

            ICosmosDbContainerProvider provider = new CosmosDbContainerProvider(cosmosDbClient);
            RaceResultsController controller = new RaceResultsController(provider, NullLogger<RaceResultsController>.Instance);

            IActionResult result = await controller.GetAllRaceResults(orgId.ToString(), memberId.ToString());
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.IsTrue(data.Contains(raceResult));
            Assert.AreEqual(1, data.Count);
        }

        [TestMethod]
        public async Task GetAllRaceResultsWholeOrganizationTest()
        {
            List<Member> memberData = new List<Member>();
            Guid orgId = Guid.NewGuid();
            Guid memberId = Guid.NewGuid();
            Member member = new Member()
            {
                Id = memberId,
                OrganizationId = orgId,
                OrgAssignedMemberId = "10",
                FirstName = "Ben",
                LastName = "Bitdiddle",
                Email = "ben.bit@raceresults.run",
            };
            memberData.Add(member);

            List<RaceResult> raceResultData = new List<RaceResult>();
            Guid raceId = Guid.NewGuid();
            RaceResult raceResult = new RaceResult()
            {
                Id = raceId,
                MemberId = memberId,
                RaceId = raceId,
                Time = TimeSpan.FromHours(2.5),
                DataSource = "Manual Entry",
            };
            raceResultData.Add(raceResult);
            Container memberContainer = MockContainerProvider<Member>.CreateMockContainer(memberData);
            Container raceResultContainer = MockContainerProvider<RaceResult>.CreateMockContainer(raceResultData);

            MockCosmosDbClient cosmosDbClient = new MockCosmosDbClient();
            cosmosDbClient.AddNewContainer(ContainerConstants.MemberContainerName, memberContainer);
            cosmosDbClient.AddEmptyOrganizationContainer();
            cosmosDbClient.AddEmptyRaceContainer();
            cosmosDbClient.AddNewContainer(ContainerConstants.RaceResultContainerName, raceResultContainer);

            ICosmosDbContainerProvider provider = new CosmosDbContainerProvider(cosmosDbClient);
            RaceResultsController controller = new RaceResultsController(provider, NullLogger<RaceResultsController>.Instance);

            IActionResult result = await controller.GetAllRaceResults(orgId.ToString());
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.IsTrue(raceResultData.Contains(raceResult));
            Assert.AreEqual(1, raceResultData.Count);
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
            Container raceResultContainer = MockContainerProvider<RaceResult>.CreateMockContainer(data);

            MockCosmosDbClient cosmosDbClient = new MockCosmosDbClient();
            cosmosDbClient.AddEmptyMemberContainer();
            cosmosDbClient.AddEmptyOrganizationContainer();
            cosmosDbClient.AddEmptyRaceContainer();
            cosmosDbClient.AddNewContainer(ContainerConstants.RaceResultContainerName, raceResultContainer);

            ICosmosDbContainerProvider provider = new CosmosDbContainerProvider(cosmosDbClient);
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
            Container raceResultContainer = MockContainerProvider<RaceResult>.CreateMockContainer(data);

            MockCosmosDbClient cosmosDbClient = new MockCosmosDbClient();
            cosmosDbClient.AddEmptyMemberContainer();
            cosmosDbClient.AddEmptyOrganizationContainer();
            cosmosDbClient.AddEmptyRaceContainer();
            cosmosDbClient.AddNewContainer(ContainerConstants.RaceResultContainerName, raceResultContainer);

            ICosmosDbContainerProvider provider = new CosmosDbContainerProvider(cosmosDbClient);
            RaceResultsController controller = new RaceResultsController(provider, NullLogger<RaceResultsController>.Instance);

            IActionResult result = await controller.Create(orgId.ToString(), memberId.ToString(), raceResult);
            Assert.IsInstanceOfType(result, typeof(CreatedAtActionResult));
            Assert.IsNotNull(raceResult.Id);
            Assert.IsTrue(data.Contains(raceResult));
            Assert.AreEqual(1, data.Count);
        }
    }
}
