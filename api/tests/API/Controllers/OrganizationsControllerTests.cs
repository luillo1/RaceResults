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
            ICosmosDbClient client = Utils<Organization>.GetMockCosmosClient(data);
            ICosmosDbContainerProvider provider = new CosmosDbContainerProvider(client);
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
            ICosmosDbClient client = Utils<Organization>.GetMockCosmosClient(data);
            ICosmosDbContainerProvider provider = new CosmosDbContainerProvider(client);
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
            ICosmosDbClient client = Utils<Organization>.GetMockCosmosClient(data);
            ICosmosDbContainerProvider provider = new CosmosDbContainerProvider(client);
            OrganizationsController controller = new OrganizationsController(provider, NullLogger<OrganizationsController>.Instance);

            IActionResult result = await controller.CreateNewOrganization(org);
            Assert.IsInstanceOfType(result, typeof(CreatedAtActionResult));
            Assert.IsNotNull(org.Id);
            Assert.IsTrue(data.Contains(org));
            Assert.AreEqual(1, data.Count);
        }
    }
}
