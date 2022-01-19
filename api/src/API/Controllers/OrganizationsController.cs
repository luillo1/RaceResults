using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RaceResults.Api.Authorization;
using RaceResults.Api.Parameters;
using RaceResults.Common.Models;
using RaceResults.Common.Requests;
using RaceResults.Data.Core;
using RaceResults.Data.KeyVault;

namespace RaceResults.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("organizations")]
    public class OrganizationsController : ControllerBase
    {
        private readonly ICosmosDbContainerProvider containerProvider;

        private readonly ILogger<OrganizationsController> logger;

        private readonly IKeyVaultClient keyVaultClient;

        public OrganizationsController(
                ICosmosDbContainerProvider cosmosDbContainerProvider,
                IKeyVaultClient keyVaultClient,
                ILogger<OrganizationsController> logger)
        {
            this.containerProvider = cosmosDbContainerProvider;
            this.keyVaultClient = keyVaultClient;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrganizations()
        {
            OrganizationContainerClient container = containerProvider.OrganizationContainer;
            IEnumerable<Organization> result = await container.GetAllAsync();
            return Ok(result);
        }

        // TODO: this endpoint should really require organization authentication. Frontend needs to be
        // refactored a bit to allow for this.
        [HttpGet("{orgId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetOneOrganization([OrganizationId] string orgId)
        {
            OrganizationContainerClient container = containerProvider.OrganizationContainer;
            Organization result = await container.GetOneAsync(orgId, orgId);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewOrganization(Organization organization)
        {
            OrganizationContainerClient container = containerProvider.OrganizationContainer;
            var addedOrg = await container.AddOneAsync(organization);

            return CreatedAtAction(nameof(CreateNewOrganization), new { id = addedOrg.Id }, addedOrg);
        }
    }
}
