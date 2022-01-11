using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RaceResults.Api.Authorization;
using RaceResults.Api.MemberProviders.WildApricot;
using RaceResults.Api.RequestObjects;
using RaceResults.Api.ResponseObjects;
using RaceResults.Common.Models;
using RaceResults.Data.Core;
using RaceResults.Data.KeyVault;

namespace RaceResults.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("organizations/{orgId}/auth/wildapricot")]
    public class WildApricotAuthController : ControllerBase
    {
        private readonly ICosmosDbContainerProvider containerProvider;

        private readonly ILogger<OrganizationsController> logger;

        private readonly IKeyVaultClient keyVaultClient;

        public WildApricotAuthController(
                ICosmosDbContainerProvider cosmosDbContainerProvider,
                IKeyVaultClient keyVaultClient,
                ILogger<OrganizationsController> logger)
        {
            this.containerProvider = cosmosDbContainerProvider;
            this.keyVaultClient = keyVaultClient;
            this.logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateWildApricotAuth(string orgId, [FromBody] CreateWildApricotAuthRequest body)
        {
            var auth = body.Auth;
            if (auth.OrganizationId.ToString() != orgId)
            {
                return BadRequest();
            }

            if (!await containerProvider.OrganizationContainer.ItemExistsAsync(orgId, orgId))
            {
                return BadRequest($"An organization with id {orgId} was not found.");
            }

            var secretName = orgId + "-client-secret";
            await this.keyVaultClient.PutSecretAsync(secretName, body.ClientSecret);

            var addedAuth = await containerProvider.WildApricotAuthContainer.AddOneAsync(auth);
            return CreatedAtAction(nameof(CreateWildApricotAuth), new { id = addedAuth.Id }, addedAuth);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWildApricot(string orgId, WildApricotLoginRequest loginRequest)
        {
            var org = await containerProvider.OrganizationContainer.GetOneAsync(orgId, orgId);

            if (org.AuthType != AuthType.WildApricot)
            {
                return BadRequest();
            }

            var auth = await containerProvider.WildApricotAuthContainer.GetAuthForOrganization(orgId);

            // TODO: keep secrets in-memory
            var secretName = org.Id + "-client-secret";
            var clientSecret = await this.keyVaultClient.GetSecretAsync(secretName);

            // Get accontId and access token
            var loginResponse = await WildApricotApi.LoginAsync(auth.ClientId, clientSecret, loginRequest);
            if (!loginResponse.success)
            {
                return Unauthorized();
            }

            var requiredHeaders = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>(RequireOrganizationAuthenticationAttribute.WildApricotAccountIdHeader, loginResponse.accountId),
                new KeyValuePair<string, string>(RequireOrganizationAuthenticationAttribute.WildApricotAuthorizationHeader, loginResponse.authorization),
            };

            foreach (var header in requiredHeaders)
            {
                Request.Headers.Add(header.Key, header.Value);
            }

            var memberResponse = await WildApricotApi.GetLoggedInMembersOrgIdAsync(Request);
            if (!memberResponse.success)
            {
                return Unauthorized();
            }

            var result = new OrganizationLoginResponse()
            {
                OrgAssignedMemberId = memberResponse.memberId,
                RequiredHeaders = requiredHeaders,
            };

            return Ok(result);
        }
    }
}
