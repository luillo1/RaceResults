using System;
using System.Collections.Generic;
using System.Linq;
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
        private static Guid organizationA = Guid.NewGuid();

        private static Guid organizationB = Guid.NewGuid();

        private static List<Member> members = new List<Member>()
        {
            new Member()
            {
                Id = Guid.NewGuid(),
                OrganizationId = organizationA,
                OrgAssignedMemberId = "10",
                FirstName = "Ben",
                LastName = "Bitdiddle",
                Email = "ben.bit@raceresults.run",
            },
            new Member()
            {
                Id = Guid.NewGuid(),
                OrganizationId = organizationA,
                OrgAssignedMemberId = "20",
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@raceresults.run",
            },
            new Member()
            {
                Id = Guid.NewGuid(),
                OrganizationId = organizationB,
                OrgAssignedMemberId = "30",
                FirstName = "Jane",
                LastName = "Doe",
                Email = "Jane.doe@raceresults.run",
            },
        };

        private MembersController controller;

        [TestInitialize]
        public void TestInitialize()
        {
            Container memberContainer = MockContainerProvider<Member>.CreateMockContainer(members);

            MockCosmosDbClient cosmosDbClient = new MockCosmosDbClient();
            cosmosDbClient.AddNewContainer(ContainerConstants.MemberContainerName, memberContainer);
            cosmosDbClient.AddEmptyOrganizationContainer();
            cosmosDbClient.AddEmptyRaceContainer();
            cosmosDbClient.AddEmptyRaceResultContainer();

            ICosmosDbContainerProvider provider = new CosmosDbContainerProvider(cosmosDbClient);
            this.controller = new MembersController(provider, NullLogger<MembersController>.Instance);
        }

        [TestMethod]
        public async Task GetAllMembersTest()
        {
            Guid orgId = organizationA;
            IActionResult result = await this.controller.GetAllMembers(orgId.ToString());

            HashSet<Member> expectedResult = members.Where(member => member.OrganizationId == orgId).ToHashSet();
            Assert.IsTrue(expectedResult.Count > 1, "Expected to query for more than 1 member.");
            AssertFoundMembers(expectedResult, result);
        }

        [TestMethod]
        public async Task GetOneMemberTest()
        {
            var memberToFind = members.First();
            var orgId = memberToFind.OrganizationId;
            var memberId = memberToFind.Id;

            IActionResult result = await controller.GetOneMember(orgId.ToString(), memberId.ToString());
            AssertFoundMember(memberToFind, result);
        }

        [TestMethod]
        public async Task CreateNewMemberTest_ValidMember()
        {
            var orgId = organizationA;

            Member memberToAdd = new Member()
            {
                OrganizationId = orgId,
                OrgAssignedMemberId = "40",
                FirstName = "Ben",
                LastName = "Bitdiddle",
                Email = "ben.bit@raceresults.run",
            };

            IActionResult result = await controller.CreateNewMember(orgId.ToString(), memberToAdd);
            Assert.IsInstanceOfType(result, typeof(CreatedAtActionResult));
            Assert.IsNotNull(memberToAdd.Id);
        }

        [TestMethod]
        public async Task CreateNewMemberTest_InvalidOrgId()
        {
            var invalidOrgId = Guid.NewGuid();

            Member memberToAdd = new Member()
            {
                OrganizationId = invalidOrgId,
                OrgAssignedMemberId = "40",
                FirstName = "Ben",
                LastName = "Bitdiddle",
                Email = "ben.bit@raceresults.run",
            };
            IActionResult result = await controller.CreateNewMember(Guid.NewGuid().ToString(), memberToAdd);
            Assert.IsInstanceOfType(result, typeof(BadRequestResult));
        }

        private void AssertFoundMember(Member expected, IActionResult result)
        {
            AssertFoundMembers(new HashSet<Member>(new Member[] { expected }), result);
        }

        private void AssertFoundMembers(HashSet<Member> expected, IActionResult result)
        {
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var actual = result as OkObjectResult;

            HashSet<Member> actualMembersFound = null;
            switch (actual.Value)
            {
                case Member foundMember:
                    actualMembersFound = new HashSet<Member>(new Member[] { foundMember });
                    break;
                case IEnumerable<Member> foundMembers:
                    actualMembersFound = new HashSet<Member>(foundMembers);
                    break;
                default:
                    Assert.Fail();
                    break;
            }

            Assert.AreEqual(expected.Count, actualMembersFound.Count);
            foreach (var expectedMember in expected)
            {
                Assert.IsTrue(actualMembersFound.Contains(expectedMember));
            }
        }
    }
}
