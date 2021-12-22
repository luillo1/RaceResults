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
            ICosmosDbClient client = Utils<Member>.GetMockCosmosClient(data);
            ICosmosDbContainerProvider provider = new CosmosDbContainerProvider(client);
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
            ICosmosDbClient client = Utils<Member>.GetMockCosmosClient(data);
            ICosmosDbContainerProvider provider = new CosmosDbContainerProvider(client);
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
            ICosmosDbClient client = Utils<Member>.GetMockCosmosClient(data);
            ICosmosDbContainerProvider provider = new CosmosDbContainerProvider(client);
            MembersController controller = new MembersController(provider, NullLogger<MembersController>.Instance);
            Guid orgId = Guid.NewGuid();
            Member member = new Member()
            {
                OrganizationId = orgId,
                OrgAssignedMemberId = "10",
                FirstName = "Ben",
                LastName = "Bitdiddle",
                Email = "ben.bit@raceresults.run",
            };

            IActionResult result = await controller.CreateNewMember(orgId.ToString(), member);
            Assert.IsInstanceOfType(result, typeof(CreatedAtActionResult));
            Assert.IsNotNull(member.Id);
            Assert.IsTrue(data.Contains(member));
            Assert.AreEqual(1, data.Count);
        }

        [TestMethod]
        public async Task CreateNewMemberTest_InvalidOrgId()
        {
            ICosmosDbClient client = Utils<IModel>.GetMockCosmosClient(new List<IModel>());
            ICosmosDbContainerProvider provider = new CosmosDbContainerProvider(client);
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
