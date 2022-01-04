using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Internal.Data.Utils;
using Microsoft.Azure.Cosmos;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RaceResults.Common.Models;
using RaceResults.Data.Core;

namespace Internal.Data.Tests
{
    [TestClass]
    public class MemberContainerClientTests
    {
        private static Guid orgainzationId1 = Guid.NewGuid();

        private static Guid orgainzationId2 = Guid.NewGuid();

        private static List<Member> members = new List<Member>()
        {
            new Member()
            {
                Id = Guid.NewGuid(),
                OrganizationId = orgainzationId1,
            },
            new Member()
            {
                Id = Guid.NewGuid(),
                OrganizationId = orgainzationId1,
            },
            new Member()
            {
                Id = Guid.NewGuid(),
                OrganizationId = orgainzationId2,
            },
        };

        private MemberContainerClient containerClient;

        [TestInitialize]
        public void TestInitialize()
        {
            Container container = MockContainerProvider<Member>.CreateMockContainer(members);
            MockCosmosDbClient cosmosDbClient = new MockCosmosDbClient();
            cosmosDbClient.AddNewContainer("MemberContainer", container);
            containerClient = new MemberContainerClient(cosmosDbClient);
        }

        [TestMethod]
        public async Task GetAllMembersAsyncTest()
        {
            IEnumerable<Member> output = await containerClient.GetAllMembersAsync(orgainzationId1.ToString());

            Assert.IsTrue(members.Where(member => member.OrganizationId == orgainzationId1).ToHashSet().SetEquals(output));
        }
    }
}
