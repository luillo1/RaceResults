using System.Collections.Generic;
using Internal.RaceResults.Data.Utils;
using Microsoft.Azure.Cosmos;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RaceResults.Common.Models;
using RaceResults.Data.Core;

namespace Internal.RaceResults.Data.Core
{
    [TestClass]
    public class CosmosDbContainerProviderTests
    {
        [TestMethod]
        public void ContructorTest()
        {
            Container memberContainer = MockContainerProvider<Member>.CreateMockContainer(new List<Member>());
            Container organizationContainer = MockContainerProvider<Organization>.CreateMockContainer(new List<Organization>());
            Container raceContainer = MockContainerProvider<Race>.CreateMockContainer(new List<Race>());
            Container raceResultContainer = MockContainerProvider<RaceResult>.CreateMockContainer(new List<RaceResult>());

            MockCosmosDbClient cosmosDbClient = new MockCosmosDbClient();

            cosmosDbClient.AddNewContainer(ContainerConstants.MemberContainerName, memberContainer);
            cosmosDbClient.AddNewContainer(ContainerConstants.OrganizationContainerName, organizationContainer);
            cosmosDbClient.AddNewContainer(ContainerConstants.RaceContainerName, raceContainer);
            cosmosDbClient.AddNewContainer(ContainerConstants.RaceResultContainerName, raceResultContainer);

            ICosmosDbContainerProvider containerProvider = new CosmosDbContainerProvider(cosmosDbClient);
        }
    }
}
