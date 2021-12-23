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
    public class OrganizationsControllerTests
    {
        [TestMethod]
        public async Task GetAllOrganizationsTest()
        {
            List<Organization> data = new List<Organization>();
            Guid orgId = Guid.NewGuid();
            Organization org = new Organization()
            {
                Id = orgId,
                Name = "Ben's Running Club",
            };
            data.Add(org);
            Container organizationContainer = MockContainerProvider<Organization>.CreateMockContainer(data);

            MockCosmosDbClient cosmosDbClient = new MockCosmosDbClient();
            cosmosDbClient.AddEmptyMemberContainer();
            cosmosDbClient.AddNewContainer(ContainerConstants.OrganizationContainerName, organizationContainer);
            cosmosDbClient.AddEmptyRaceContainer();
            cosmosDbClient.AddEmptyRaceResultContainer();

            ICosmosDbContainerProvider provider = new CosmosDbContainerProvider(cosmosDbClient);
            OrganizationsController controller = new OrganizationsController(provider, NullLogger<OrganizationsController>.Instance);

            IActionResult result = await controller.GetAllOrganizations();
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.IsTrue(data.Contains(org));
            Assert.AreEqual(1, data.Count);
        }

        [TestMethod]
        public async Task GetOneOrganizationTest()
        {
            List<Organization> data = new List<Organization>();
            Guid orgId = Guid.NewGuid();
            Organization org = new Organization()
            {
                Id = orgId,
                Name = "Ben's Running Club",
            };
            data.Add(org);
            Container organizationContainer = MockContainerProvider<Organization>.CreateMockContainer(data);

            MockCosmosDbClient cosmosDbClient = new MockCosmosDbClient();
            cosmosDbClient.AddEmptyMemberContainer();
            cosmosDbClient.AddNewContainer(ContainerConstants.OrganizationContainerName, organizationContainer);
            cosmosDbClient.AddEmptyRaceContainer();
            cosmosDbClient.AddEmptyRaceResultContainer();

            ICosmosDbContainerProvider provider = new CosmosDbContainerProvider(cosmosDbClient);
            OrganizationsController controller = new OrganizationsController(provider, NullLogger<OrganizationsController>.Instance);

            IActionResult result = await controller.GetOneOrganization(orgId.ToString());
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.IsTrue(data.Contains(org));
            Assert.AreEqual(1, data.Count);
        }

        [TestMethod]
        public async Task CreateNewOrganizationTest()
        {
            List<Organization> data = new List<Organization>();
            Organization org = new Organization()
            {
                Name = "Ben's Running Club",
            };
            Container organizationContainer = MockContainerProvider<Organization>.CreateMockContainer(data);

            MockCosmosDbClient cosmosDbClient = new MockCosmosDbClient();
            cosmosDbClient.AddEmptyMemberContainer();
            cosmosDbClient.AddNewContainer(ContainerConstants.OrganizationContainerName, organizationContainer);
            cosmosDbClient.AddEmptyRaceContainer();
            cosmosDbClient.AddEmptyRaceResultContainer();

            ICosmosDbContainerProvider provider = new CosmosDbContainerProvider(cosmosDbClient);
            OrganizationsController controller = new OrganizationsController(provider, NullLogger<OrganizationsController>.Instance);

            IActionResult result = await controller.CreateNewOrganization(org);
            Assert.IsInstanceOfType(result, typeof(CreatedAtActionResult));
            Assert.IsNotNull(org.Id);
            Assert.IsTrue(data.Contains(org));
            Assert.AreEqual(1, data.Count);
        }
    }
}
