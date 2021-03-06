using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Internal.Api.Utils;
using Internal.Data.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RaceResults.Api.Controllers;
using RaceResults.Common.Models;
using RaceResults.Common.Requests;
using RaceResults.Data.Core;

namespace Internal.Api.Tests
{
    [TestClass]
    public class OrganizationsControllerTests
    {
        private static List<Organization> organizations = new List<Organization>()
        {
            new Organization()
            {
                Id = Guid.NewGuid(),
                Name = "Ben's Running Club",
            },
            new Organization()
            {
                Id = Guid.NewGuid(),
                Name = "Alyssa's Running Club",
            },
            new Organization()
            {
                Id = Guid.NewGuid(),
                Name = "Jane's Running Club",
            },
        };

        private OrganizationsController controller;

        private MockKeyVaultClient keyVaultClient;

        [TestInitialize]
        public void TestInitialize()
        {
            Container organizationContainer = MockContainerProvider<Organization>.CreateMockContainer(organizations);

            keyVaultClient = new MockKeyVaultClient();

            MockCosmosDbClient cosmosDbClient = new MockCosmosDbClient();
            cosmosDbClient.AddEmptyMemberContainer();
            cosmosDbClient.AddNewContainer(ContainerConstants.OrganizationContainerName, organizationContainer);
            cosmosDbClient.AddEmptyRaceContainer();
            cosmosDbClient.AddEmptyRaceResultAuthContainer();
            cosmosDbClient.AddEmptyWildApricotAuthContainer();
            cosmosDbClient.AddEmptyRaceResultContainer();
            cosmosDbClient.AddEmptySubmissionCheckpointContainer();

            ICosmosDbContainerProvider provider = new CosmosDbContainerProvider(cosmosDbClient);
            controller = new OrganizationsController(provider, keyVaultClient, NullLogger<OrganizationsController>.Instance);
        }

        [TestMethod]
        public async Task GetAllOrganizationsTest()
        {
            IActionResult result = await controller.GetAllOrganizations();
            HashSet<Organization> expectedResult = organizations.ToHashSet();
            Assert.IsTrue(expectedResult.Count > 1, "Expected to query for more than 1 organization.");
            ValidationTools.AssertFoundItems(expectedResult, result);
        }

        [TestMethod]
        public async Task GetOneOrganizationTest()
        {
            Organization orgToFind = organizations.First();
            Guid orgId = orgToFind.Id;

            IActionResult result = await controller.GetOneOrganization(orgId.ToString());
            ValidationTools.AssertFoundItem(orgToFind, result);
        }

        [TestMethod]
        public async Task CreateNewOrganizationTest()
        {
            Organization org = new Organization()
            {
                Name = "Bob's Running Club",
                AuthType = AuthType.RaceResults,
            };

            IActionResult result = await controller.CreateNewOrganization(org);
            Assert.IsInstanceOfType(result, typeof(CreatedAtActionResult));
            Assert.IsNotNull(org.Id);
        }
    }
}
