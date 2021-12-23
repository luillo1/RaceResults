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
    public class MembersControllerTests
    {
        [TestMethod]
        public async Task GetAllMembersTest()
        {
            List<Member> data = new List<Member>();
            Guid memberId = Guid.NewGuid();
            Guid orgId = Guid.NewGuid();
            Member member = new Member()
            {
                Id = memberId,
                OrganizationId = orgId,
                OrgAssignedMemberId = "10",
                FirstName = "Ben",
                LastName = "Bitdiddle",
                Email = "ben.bit@raceresults.run",
            };
            data.Add(member);
            Container memberContainer = MockContainerProvider<Member>.CreateMockContainer(data);

            MockCosmosDbClient cosmosDbClient = new MockCosmosDbClient();
            cosmosDbClient.AddNewContainer(ContainerConstants.MemberContainerName, memberContainer);
            cosmosDbClient.AddEmptyOrganizationContainer();
            cosmosDbClient.AddEmptyRaceContainer();
            cosmosDbClient.AddEmptyRaceResultContainer();

            ICosmosDbContainerProvider provider = new CosmosDbContainerProvider(cosmosDbClient);
            MembersController controller = new MembersController(provider, NullLogger<MembersController>.Instance);

            IActionResult result = await controller.GetAllMembers(orgId.ToString());
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.IsTrue(data.Contains(member));
            Assert.AreEqual(1, data.Count);
        }

        [TestMethod]
        public async Task GetOneMemberTest()
        {
            List<Member> data = new List<Member>();
            Guid memberId = Guid.NewGuid();
            Guid orgId = Guid.NewGuid();
            Member member = new Member()
            {
                Id = memberId,
                OrganizationId = orgId,
                OrgAssignedMemberId = "10",
                FirstName = "Ben",
                LastName = "Bitdiddle",
                Email = "ben.bit@raceresults.run",
            };
            data.Add(member);
            Container memberContainer = MockContainerProvider<Member>.CreateMockContainer(data);

            MockCosmosDbClient cosmosDbClient = new MockCosmosDbClient();
            cosmosDbClient.AddNewContainer(ContainerConstants.MemberContainerName, memberContainer);
            cosmosDbClient.AddEmptyOrganizationContainer();
            cosmosDbClient.AddEmptyRaceContainer();
            cosmosDbClient.AddEmptyRaceResultContainer();

            ICosmosDbContainerProvider provider = new CosmosDbContainerProvider(cosmosDbClient);
            MembersController controller = new MembersController(provider, NullLogger<MembersController>.Instance);

            IActionResult result = await controller.GetOneMember(orgId.ToString(), memberId.ToString());
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(member, ((OkObjectResult)result).Value);
            Assert.IsTrue(data.Contains(member));
            Assert.AreEqual(1, data.Count);
        }

        [TestMethod]
        public async Task CreateNewMemberTest_ValidMember()
        {
            List<Member> data = new List<Member>();
            Guid orgId = Guid.NewGuid();
            Member member = new Member()
            {
                OrganizationId = orgId,
                OrgAssignedMemberId = "10",
                FirstName = "Ben",
                LastName = "Bitdiddle",
                Email = "ben.bit@raceresults.run",
            };
            Container memberContainer = MockContainerProvider<Member>.CreateMockContainer(data);

            MockCosmosDbClient cosmosDbClient = new MockCosmosDbClient();
            cosmosDbClient.AddNewContainer(ContainerConstants.MemberContainerName, memberContainer);
            cosmosDbClient.AddEmptyOrganizationContainer();
            cosmosDbClient.AddEmptyRaceContainer();
            cosmosDbClient.AddEmptyRaceResultContainer();

            ICosmosDbContainerProvider provider = new CosmosDbContainerProvider(cosmosDbClient);
            MembersController controller = new MembersController(provider, NullLogger<MembersController>.Instance);

            IActionResult result = await controller.CreateNewMember(orgId.ToString(), member);
            Assert.IsInstanceOfType(result, typeof(CreatedAtActionResult));
            Assert.IsNotNull(member.Id);
            Assert.IsTrue(data.Contains(member));
            Assert.AreEqual(1, data.Count);
        }

        [TestMethod]
        public async Task CreateNewMemberTest_InvalidOrgId()
        {
            Container memberContainer = MockContainerProvider<Member>.CreateMockContainer(new List<Member>());

            MockCosmosDbClient cosmosDbClient = new MockCosmosDbClient();
            cosmosDbClient.AddNewContainer(ContainerConstants.MemberContainerName, memberContainer);
            cosmosDbClient.AddEmptyOrganizationContainer();
            cosmosDbClient.AddEmptyRaceContainer();
            cosmosDbClient.AddEmptyRaceResultContainer();

            ICosmosDbContainerProvider provider = new CosmosDbContainerProvider(cosmosDbClient);
            MembersController controller = new MembersController(provider, NullLogger<MembersController>.Instance);
            Member member = new Member()
            {
                OrganizationId = Guid.NewGuid(),
                OrgAssignedMemberId = "10",
                FirstName = "Ben",
                LastName = "Bitdiddle",
                Email = "ben.bit@raceresults.run",
            };

            IActionResult result = await controller.CreateNewMember(Guid.NewGuid().ToString(), member);
            Assert.IsInstanceOfType(result, typeof(BadRequestResult));
        }
    }
}
