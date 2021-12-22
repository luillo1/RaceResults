using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Internal.RaceResults.Data.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RaceResults.Common.Models;
using RaceResults.Data.Core;

namespace Internal.RaceResults.Data.Core
{
    [TestClass]
    public class RaceResultContainerClientTests
    {
        [TestMethod]
        public async Task GetRaceResultsForMemberAsyncTest()
        {
            List<RaceResult> includedData = new List<RaceResult>();

            Guid memberId1 = Guid.NewGuid();
            Guid memberId2 = Guid.NewGuid();

            RaceResult raceResult1 = new RaceResult()
            {
                MemberId = memberId1,
            };

            RaceResult raceResult2 = new RaceResult()
            {
                MemberId = memberId1,
            };

            RaceResult raceResult3 = new RaceResult()
            {
                MemberId = memberId2,
            };

            includedData.Add(raceResult1);
            includedData.Add(raceResult2);
            includedData.Add(raceResult3);

            ICosmosDbClient cosmosDbClient = Utils<RaceResult>.GetMockCosmosClient(includedData);
            RaceResultContainerClient containerClient = new RaceResultContainerClient(cosmosDbClient);

            IEnumerable<RaceResult> output = await containerClient.GetRaceResultsForMemberAsync(memberId1.ToString());
            List<RaceResult> result = output.ToList();

            Assert.AreEqual(3, includedData.Count);
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.Contains(raceResult1));
            Assert.IsTrue(result.Contains(raceResult2));
        }

        [TestMethod]
        public async Task GetRaceResultsForMembersAsyncTest()
        {
            List<RaceResult> includedData = new List<RaceResult>();

            Guid memberId1 = Guid.NewGuid();
            Guid memberId2 = Guid.NewGuid();
            Guid memberId3 = Guid.NewGuid();

            RaceResult raceResult1 = new RaceResult()
            {
                MemberId = memberId1,
            };

            RaceResult raceResult2 = new RaceResult()
            {
                MemberId = memberId1,
            };

            RaceResult raceResult3 = new RaceResult()
            {
                MemberId = memberId2,
            };

            RaceResult raceResult4 = new RaceResult()
            {
                MemberId = memberId3,
            };

            includedData.Add(raceResult1);
            includedData.Add(raceResult2);
            includedData.Add(raceResult3);
            includedData.Add(raceResult4);

            ICosmosDbClient cosmosDbClient = Utils<RaceResult>.GetMockCosmosClient(includedData);
            RaceResultContainerClient containerClient = new RaceResultContainerClient(cosmosDbClient);

            List<string> memberIdQuery = new List<string>()
            {
                memberId1.ToString(),
                memberId2.ToString(),
            };
            IEnumerable<RaceResult> output = await containerClient.GetRaceResultsForMembersAsync(memberIdQuery);
            List<RaceResult> result = output.ToList();

            Assert.AreEqual(4, includedData.Count);
            Assert.AreEqual(3, result.Count);
            Assert.IsTrue(result.Contains(raceResult1));
            Assert.IsTrue(result.Contains(raceResult2));
            Assert.IsTrue(result.Contains(raceResult3));
        }
    }
}
