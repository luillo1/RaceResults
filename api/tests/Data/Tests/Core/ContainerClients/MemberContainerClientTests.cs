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
    public class MemberContainerClientTests
    {
        [TestMethod]
        public async Task GetAllMembersAsyncTest()
        {
            List<Member> includedData = new List<Member>();

            Guid orgId1 = Guid.NewGuid();
            Guid orgId2 = Guid.NewGuid();

            Member member1 = new Member()
            {
                OrganizationId = orgId1,
            };

            Member member2 = new Member()
            {
                OrganizationId = orgId1,
            };

            Member member3 = new Member()
            {
                OrganizationId = orgId2,
            };

            includedData.Add(member1);
            includedData.Add(member2);
            includedData.Add(member3);

            Container container = MockContainerProvider<Member>.CreateMockContainer(includedData);

            MockCosmosDbClient cosmosDbClient = new MockCosmosDbClient();
            cosmosDbClient.AddNewContainer("MemberContainer", container);

            MemberContainerClient containerClient = new MemberContainerClient(cosmosDbClient);

            IEnumerable<Member> output = await containerClient.GetAllMembersAsync(orgId1.ToString());
            List<Member> result = output.ToList();

            Assert.AreEqual(3, includedData.Count);
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.Contains(member1));
            Assert.IsTrue(result.Contains(member2));
        }
    }
}
