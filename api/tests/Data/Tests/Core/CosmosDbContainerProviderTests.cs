using System.Collections.Generic;
using Internal.Data.Utils;
using Microsoft.Azure.Cosmos;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RaceResults.Common.Models;
using RaceResults.Data.Core;

namespace Internal.Data.Tests
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
            Container raceResultAuthContainer = MockContainerProvider<RaceResultsAuth>.CreateMockContainer(new List<RaceResultsAuth>());
            Container wildApricotAuthContainer = MockContainerProvider<WildApricotAuth>.CreateMockContainer(new List<WildApricotAuth>());

            MockCosmosDbClient cosmosDbClient = new MockCosmosDbClient();

            cosmosDbClient.AddNewContainer(ContainerConstants.MemberContainerName, memberContainer);
            cosmosDbClient.AddNewContainer(ContainerConstants.OrganizationContainerName, organizationContainer);
            cosmosDbClient.AddNewContainer(ContainerConstants.RaceContainerName, raceContainer);
            cosmosDbClient.AddNewContainer(ContainerConstants.RaceResultContainerName, raceResultContainer);
            cosmosDbClient.AddNewContainer(ContainerConstants.RaceResultsAuthContainerName, raceResultAuthContainer);
            cosmosDbClient.AddNewContainer(ContainerConstants.WildApricotAuthContainerName, wildApricotAuthContainer);

            ICosmosDbContainerProvider containerProvider = new CosmosDbContainerProvider(cosmosDbClient);
        }
    }
}
