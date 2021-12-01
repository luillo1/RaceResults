using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RaceResults.Common.Models;
using RaceResults.Data.Core;

namespace RaceResults.Api.Controllers
{
    // [Authorize]
    [ApiController]
    [Route("organizations")]
    public class OrganizationsController : ControllerBase
    {
        private readonly ICosmosDbContainerProvider containerProvider;

        private readonly ILogger<OrganizationsController> logger;

        public OrganizationsController(
                ICosmosDbContainerProvider cosmosDbContainerProvider,
                ILogger<OrganizationsController> logger)
        {
            this.containerProvider = cosmosDbContainerProvider;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrganizations()
        {
            IOrganizationContainerClient container = containerProvider.OrganizationContainer;
            IEnumerable<Organization> result = await container.GetAllOrganizationsAsync();
            return Ok(result);
        }

        [HttpGet("{orgId}")]
        public async Task<IActionResult> GetOneOrganization(string orgId)
        {
            IOrganizationContainerClient container = containerProvider.OrganizationContainer;
            Organization result = await container.GetOrganizationAsync(orgId);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewOrganization(Organization organization)
        {
            organization.Id = Guid.NewGuid();
            IOrganizationContainerClient container = containerProvider.OrganizationContainer;
            await container.AddOrganizationAsync(organization);
            return CreatedAtAction(nameof(CreateNewOrganization), new { id = organization.Id }, organization);
        }
    }
}
