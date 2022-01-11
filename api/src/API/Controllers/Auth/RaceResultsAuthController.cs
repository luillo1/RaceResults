using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RaceResults.Api.RequestObjects;
using RaceResults.Api.ResponseObjects;
using RaceResults.Common.Models;
using RaceResults.Data.Core;
using RaceResults.Data.KeyVault;

namespace RaceResults.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("organizations/{orgId}/auth/raceresults")]
    public class RaceResultsAuthController : ControllerBase
    {
        private readonly ICosmosDbContainerProvider containerProvider;

        private readonly ILogger<OrganizationsController> logger;

        private readonly IKeyVaultClient keyVaultClient;

        public RaceResultsAuthController(
                ICosmosDbContainerProvider cosmosDbContainerProvider,
                IKeyVaultClient keyVaultClient,
                ILogger<OrganizationsController> logger)
        {
            this.containerProvider = cosmosDbContainerProvider;
            this.keyVaultClient = keyVaultClient;
            this.logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateRaceResultsAuth(string orgId, RaceResultsAuth auth)
        {
            if (auth.OrganizationId.ToString() != orgId)
            {
                return BadRequest();
            }

            if (!await containerProvider.OrganizationContainer.ItemExistsAsync(orgId, orgId))
            {
                return BadRequest($"An organization with id {orgId} was not found.");
            }

            var addedAuth = await containerProvider.RaceResultsAuthContainer.AddOneAsync(auth);
            return CreatedAtAction(nameof(CreateRaceResultsAuth), new { id = addedAuth.Id }, addedAuth);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginRaceResults(string orgId, RaceResultsLoginRequest request)
        {
            var org = await containerProvider.OrganizationContainer.GetOneAsync(orgId, orgId);

            if (org.AuthType != AuthType.RaceResults)
            {
                return BadRequest();
            }

            // TODO (#85): Do something better
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }

            // TODO (#86): use some kind of AAD library to get an ID or something
            var name = User.Identity.Name;
            var response = new OrganizationLoginResponse()
            {
                OrgAssignedMemberId = name,
                RequiredHeaders = new List<KeyValuePair<string, string>>(),
            };

            return Ok(response);
        }
    }
}
