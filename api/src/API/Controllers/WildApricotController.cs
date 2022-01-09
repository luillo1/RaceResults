using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RaceResults.Api.MemberProviders.WildApricot;
using RaceResults.Common.Models;
using RaceResults.Data.KeyVault;
using RestSharp;

namespace RaceResults.Api.Controllers
{
    [ApiController]
    [Route("wa")]
    public class WildApricotController : ControllerBase
    {
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        // FOR TESTING. Need to figure out something better...
        public static bool OverrideAuth { get; set; }

        public static async Task<bool> Authorized(HttpRequest request, string orgAssignedMemberId)
        {
            if (OverrideAuth)
            {
                return true;
            }

            var hasAccount = int.TryParse(request.Headers["WA-AccountId"], out var accountId);
            var authorization = request.Headers["WA-Authorization"];

            if (!hasAccount || string.IsNullOrEmpty(authorization))
            {
                return false;
            }

            var waResponse = await WildApricotController.GetCurrentUserFromWildApricot(accountId, authorization);
            if (waResponse.Id.ToString() != orgAssignedMemberId)
            {
                return false;
            }

            return true;
        }

        public static async Task<WildApricotMember> GetCurrentUserFromWildApricot(int accountId, string authorization)
        {
            var client = new RestClient("https://api.wildapricot.org/publicview/v1");

            var request = new RestRequest($"accounts/{accountId}/contacts/me?includeDetails=true", DataFormat.Json);
            request.AddHeader("Authorization", authorization);

            return await client.GetAsync<WildApricotMember>(request);
        }

        private readonly ILogger<OrganizationsController> logger;

        private readonly IKeyVaultClient keyVaultClient;

        public WildApricotController(
                IKeyVaultClient keyVaultClient,
                ILogger<OrganizationsController> logger)
        {
            this.keyVaultClient = keyVaultClient;
            this.logger = logger;
        }

        public class GetAccessTokenBody
        {
            public string OrganizationId { get; set; }

            public string ClientId { get; set; }

            public string RedirectURI { get; set; }

            public string Scope { get; set; }
        }

        [HttpPost("oauth/{authorization_code}")]
        public async Task<IActionResult> GetAccessToken(string authorization_code, [FromBody] GetAccessTokenBody body)
        {
            var secretName = body.OrganizationId + "-client-secret";
            var clientSecret = await this.keyVaultClient.GetSecretAsync(secretName);

            using (var client = new HttpClient())
            {
                var postData = new List<KeyValuePair<string, string>>();
                postData.Add(new KeyValuePair<string, string>("grant_type", "authorization_code"));
                postData.Add(new KeyValuePair<string, string>("code", authorization_code));
                postData.Add(new KeyValuePair<string, string>("scope", body.Scope));
                postData.Add(new KeyValuePair<string, string>("client_id", body.ClientId));
                postData.Add(new KeyValuePair<string, string>("redirect_uri", body.RedirectURI));

                HttpContent content = new FormUrlEncodedContent(postData);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

                var authValue = new AuthenticationHeaderValue("Basic", Base64Encode(body.ClientId + ":" + clientSecret));
                client.DefaultRequestHeaders.Authorization = authValue;

                var responseResult = await client.PostAsync("https://oauth.wildapricot.org/auth/token", content);

                var resp = await responseResult.Content.ReadAsStringAsync();
                return Ok(resp);
            }
        }

        [HttpGet("currentUser/{accountId}")]
        public async Task<IActionResult> GetCurrentUser(int accountId)
        {
            var resp = await GetCurrentUserFromWildApricot(accountId, Request.Headers["WA-Authorization"]);
            return Ok(resp);
        }
    }
}