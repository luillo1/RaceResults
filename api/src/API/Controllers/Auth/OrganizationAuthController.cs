using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RaceResults.Common.Models;
using RaceResults.Data.Core;
using RaceResults.Data.KeyVault;

namespace RaceResults.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("organizations/{orgId}/auth")]
    public class OrganizationAuthController : ControllerBase
    {
        private readonly ICosmosDbContainerProvider containerProvider;

        private readonly ILogger<OrganizationsController> logger;

        private readonly IKeyVaultClient keyVaultClient;

        public OrganizationAuthController(
                ICosmosDbContainerProvider cosmosDbContainerProvider,
                IKeyVaultClient keyVaultClient,
                ILogger<OrganizationsController> logger)
        {
            this.containerProvider = cosmosDbContainerProvider;
            this.keyVaultClient = keyVaultClient;
            this.logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAuthForOrganization(string orgId)
        {
            var org = await containerProvider.OrganizationContainer.GetOneAsync(orgId, orgId);
            switch (org.AuthType)
            {
                case AuthType.RaceResults:
                    return Ok(await containerProvider.RaceResultsAuthContainer.GetAuthForOrganization(orgId));
                case AuthType.WildApricot:
                    return Ok(await containerProvider.WildApricotAuthContainer.GetAuthForOrganization(orgId));
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
