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
using RaceResults.Api.ResponseObjects;
using RaceResults.Common.Models;
using RaceResults.Data.Core;

namespace Internal.RaceResults.Api.Controllers
{
    [TestClass]
    public class RaceResultsControllerTests
    {
        private static Guid organizationA = Guid.NewGuid();

        private static Guid organizationB = Guid.NewGuid();

        private static Guid memberA = Guid.NewGuid();

        private static Guid memberB = Guid.NewGuid();

        private static Guid memberC = Guid.NewGuid();

        private static Guid raceA = Guid.NewGuid();

        private static Guid raceB = Guid.NewGuid();

        private static Guid raceEventId = Guid.NewGuid();

        private static List<Member> members = new List<Member>()
        {
            new Member()
            {
                Id = memberA,
                OrganizationId = organizationA,
                OrgAssignedMemberId = "10",
                FirstName = "Ben",
                LastName = "Bitdiddle",
                Email = "ben.bit@raceresults.run",
            },
            new Member()
            {
                Id = memberB,
                OrganizationId = organizationA,
                OrgAssignedMemberId = "20",
                FirstName = "Alyssa",
                LastName = "Hacker",
                Email = "alyssa.hacker@raceresults.run",
            },
            new Member()
            {
                Id = memberC,
                OrganizationId = organizationB,
                OrgAssignedMemberId = "10",
                FirstName = "Jane",
                LastName = "Doe",
                Email = "jane.doe@swimresults.swim",
            },
        };

        private static List<RaceResult> raceResults = new List<RaceResult>()
        {
            new RaceResult()
            {
                Id = Guid.NewGuid(),
                MemberId = memberA,
                RaceId = raceA,
                Time = TimeSpan.FromHours(2.5),
                DataSource = "Manual Entry",
            },
            new RaceResult()
            {
                Id = Guid.NewGuid(),
                MemberId = memberA,
                RaceId = raceB,
                Time = TimeSpan.FromHours(3),
                DataSource = "Manual Entry",
            },
            new RaceResult()
            {
                Id = Guid.NewGuid(),
                MemberId = memberB,
                RaceId = raceA,
                Time = TimeSpan.FromHours(4),
                DataSource = "Manual Entry",
            },
            new RaceResult()
            {
                Id = Guid.NewGuid(),
                MemberId = memberC,
                RaceId = raceB,
                Time = TimeSpan.FromHours(1),
                DataSource = "Manual Entry",
            },
        };

        private static List<Race> races = new List<Race>()
        {
            new Race()
            {
                Id = raceA,
                EventId = raceEventId,
                Name = "Ben's Annual Race",
                Date = DateTime.Now,
                Location = "Ben's House",
                Distance = "5k",
            },
            new Race()
            {
                Id = raceB,
                EventId = raceEventId,
                Name = "Alyssa's Annual Race",
                Date = DateTime.Now,
                Location = "Alyssa's House",
                Distance = "Marathon",
            },
        };

        private RaceResultsController controller;

        [TestInitialize]
        public void TestInitialize()
        {
            Container memberContainer = MockContainerProvider<Member>.CreateMockContainer(members);
            Container raceContainer = MockContainerProvider<Race>.CreateMockContainer(races);
            Container raceResultContainer = MockContainerProvider<RaceResult>.CreateMockContainer(raceResults);

            MockCosmosDbClient cosmosDbClient = new MockCosmosDbClient();
            cosmosDbClient.AddNewContainer(ContainerConstants.MemberContainerName, memberContainer);
            cosmosDbClient.AddEmptyOrganizationContainer();
            cosmosDbClient.AddNewContainer(ContainerConstants.RaceContainerName, raceContainer);
            cosmosDbClient.AddNewContainer(ContainerConstants.RaceResultContainerName, raceResultContainer);

            ICosmosDbContainerProvider provider = new CosmosDbContainerProvider(cosmosDbClient);
            controller = new RaceResultsController(provider, NullLogger<RaceResultsController>.Instance);
        }

        [TestMethod]
        public async Task GetAllRaceResultsSingleMemberTest()
        {
            IActionResult result = await controller.GetAllRaceResults(organizationA.ToString(), memberA.ToString());
            HashSet<RaceResult> expectedResult = raceResults.Where(result => result.MemberId == memberA).ToHashSet();
            Assert.IsTrue(expectedResult.Count > 1, "Expected to query for more than 1 raceResult.");
            ValidationTools.AssertFoundItems(expectedResult, result);
        }

        [TestMethod]
        public async Task GetAllRaceResultsWholeOrganizationTest()
        {
            IActionResult result = await controller.GetAllRaceResults(organizationA.ToString());
            HashSet<RaceResultResponse> expectedResult = raceResults.Join(
                    members,
                    raceResult => raceResult.MemberId,
                    member => member.Id,
                    (raceResult, member) => new RaceResultResponse()
                    {
                        RaceResult = raceResult,
                        Member = member,
                        Race = races.Single(race => race.Id == raceResult.RaceId),
                    }).Where(response => response.Member.OrganizationId == organizationA).ToHashSet();
            Assert.IsTrue(expectedResult.Count > 1, "Expected to query for more than 1 raceResult.");
            ValidationTools.AssertFoundItems(expectedResult, result);
        }

        [TestMethod]
        public async Task GetOneRaceResultTest()
        {
            RaceResult raceResultToFind = raceResults.First();
            Guid memberId = raceResultToFind.MemberId;
            Guid raceResultId = raceResultToFind.Id;
            Guid orgId = members.Single(member => member.Id == memberId).OrganizationId;

            IActionResult result = await controller.GetOneRaceResult(orgId.ToString(), memberId.ToString(), raceResultId.ToString());
            ValidationTools.AssertFoundItem(raceResultToFind, result);
        }

        [TestMethod]
        public async Task CreateTest()
        {
            Guid orgId = organizationA;
            Guid memberId = memberA;
            Guid raceId = raceA;
            RaceResult raceResult = new RaceResult()
            {
                MemberId = memberId,
                RaceId = raceId,
                Time = TimeSpan.FromHours(2.5),
                DataSource = "Manual Entry",
            };

            IActionResult result = await controller.Create(orgId.ToString(), memberId.ToString(), raceResult);
            Assert.IsInstanceOfType(result, typeof(CreatedAtActionResult));
            Assert.IsNotNull(raceResult.Id);
        }
    }
}
