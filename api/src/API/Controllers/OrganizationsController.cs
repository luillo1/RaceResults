using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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

        [HttpGet("{orgId}")]
        public async Task<IActionResult> GetOneOrganization(string orgId)
        {
            OrganizationContainerClient container = containerProvider.OrganizationContainer;
            Organization result = await container.GetOneAsync(orgId, orgId);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewOrganization([FromBody] CreateOrganizationRequest body)
        {
            var organization = body.Organization;
            OrganizationContainerClient container = containerProvider.OrganizationContainer;
            var addedOrg = await container.AddOneAsync(organization);

            var secretName = addedOrg.Id + "-client-secret";
            await this.keyVaultClient.PutSecretAsync(secretName, body.ClientSecret);

            return CreatedAtAction(nameof(CreateNewOrganization), new { id = addedOrg.Id }, addedOrg);
        }
    }
}
