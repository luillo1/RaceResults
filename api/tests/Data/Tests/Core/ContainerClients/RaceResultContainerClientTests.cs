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
    public class RaceResultContainerClientTests
    {
        private static Guid memberId1 = Guid.NewGuid();

        private static Guid memberId2 = Guid.NewGuid();

        private static Guid memberId3 = Guid.NewGuid();

        private static List<RaceResult> raceResults = new List<RaceResult>()
        {
            new RaceResult()
            {
                Id = Guid.NewGuid(),
                MemberId = memberId1,
            },
            new RaceResult()
            {
                Id = Guid.NewGuid(),
                MemberId = memberId1,
            },
            new RaceResult()
            {
                Id = Guid.NewGuid(),
                MemberId = memberId2,
            },
            new RaceResult()
            {
                Id = Guid.NewGuid(),
                MemberId = memberId3,
            },
        };

        private RaceResultContainerClient containerClient;

        [TestInitialize]
        public void TestInitialize()
        {
            Container container = MockContainerProvider<RaceResult>.CreateMockContainer(raceResults);
            MockCosmosDbClient cosmosDbClient = new MockCosmosDbClient();
            cosmosDbClient.AddNewContainer("RaceResultContainer", container);
            containerClient = new RaceResultContainerClient(cosmosDbClient);
        }

        [TestMethod]
        public async Task GetRaceResultsForMemberAsyncTest()
        {
            IEnumerable<RaceResult> output = await containerClient.GetRaceResultsForMemberAsync(memberId1.ToString());

            Assert.IsTrue(raceResults.Where(result => result.MemberId == memberId1).ToHashSet().SetEquals(output));
        }

        [TestMethod]
        public async Task GetRaceResultsForMembersAsyncTest()
        {
            List<string> memberIdQuery = new List<string>()
            {
                memberId1.ToString(),
                memberId2.ToString(),
            };

            IEnumerable<RaceResult> output = await containerClient.GetRaceResultsForMembersAsync(memberIdQuery);
            Assert.IsTrue(raceResults
                    .Where(result => result.MemberId == memberId1 || result.MemberId == memberId2)
                    .ToHashSet()
                    .SetEquals(output));
        }
    }
}
