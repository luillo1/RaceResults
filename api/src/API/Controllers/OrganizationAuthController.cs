using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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

        [HttpPost]
        [AllowAnonymous]
        [Route("login/raceresults")]
        public async Task<IActionResult> LoginRaceResults(string orgId)
        {
            var org = await containerProvider.OrganizationContainer.GetOneAsync(orgId, orgId);

            if (org.AuthType != AuthType.RaceResults)
            {
                return BadRequest();
            }

            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }

            // TODO: do this a better way
            var name = User.Identity.Name;
            var response = new OrganizationLoginResponse()
            {
                OrgAssignedMemberId = name,
                RequiredHeaders = new List<KeyValuePair<string, string>>(),
            };

            return Ok(response);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("login/wildapricot")]
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
                new KeyValuePair<string, string>(RequireOrganizationAuthorizationAttribute.WildApricotAccountIdHeader, loginResponse.accountId),
                new KeyValuePair<string, string>(RequireOrganizationAuthorizationAttribute.WildApricotAuthorizationHeader, loginResponse.authorization),
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
