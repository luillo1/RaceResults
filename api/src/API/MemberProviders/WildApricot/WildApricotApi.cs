using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using RaceResults.Api.Authorization;
using RaceResults.Api.RequestObjects;
using RaceResults.Common.Models;
using RestSharp;

namespace RaceResults.Api.MemberProviders.WildApricot
{
    // TOOD: make an interface for this
    public static class WildApricotApi
    {
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public static async Task<(bool success, string accountId, string authorization)> LoginAsync(string clientId, string clientSecret, WildApricotLoginRequest loginRequest)
        {
            var client = new RestClient("https://oauth.wildapricot.org");

            var request = new RestRequest("/auth/token", DataFormat.Json);
            request.AddHeader("Authorization", "Basic " + Base64Encode(clientId + ":" + clientSecret));
            request.AddBody(new Dictionary<string, string>()
            {
                { "grant_type", "authorization_code" },
                { "code", loginRequest.AuthorizationCode },
                { "scope", loginRequest.Scope },
                { "client_id", clientId },
                { "redirect_uri", loginRequest.RedirectUri },
            });

            var loginResponse = await client.ExecutePostAsync<WildApricotOauthResponse>(request);

            if (!loginResponse.IsSuccessful)
            {
                return (false, string.Empty, string.Empty);
            }

            return (true, loginResponse.Data.Permissions.First().AccountId.ToString(), loginResponse.Data.TokenType + " " + loginResponse.Data.AccessToken);
        }

        public static async Task<(bool success, string memberId)> GetLoggedInMembersOrgIdAsync(HttpRequest request)
        {
            var hasAccountId = request.Headers.TryGetValue(RequireOrganizationAuthenticationAttribute.WildApricotAccountIdHeader, out var accountId);
            var hasAuthorization = request.HttpContext.Request.Headers.TryGetValue(RequireOrganizationAuthenticationAttribute.WildApricotAuthorizationHeader, out var authorization);
            if (!hasAccountId || !hasAuthorization)
            {
                return (false, string.Empty);
            }

            var client = new RestClient("https://api.wildapricot.org/v2.2");

            var accountRequest = new RestRequest($"accounts/{accountId}/contacts/me?includeDetails=true", DataFormat.Json);
            accountRequest.AddHeader("Authorization", authorization);

            var memberResponse = await client.ExecuteGetAsync<WildApricotMember>(accountRequest);

            if (!memberResponse.IsSuccessful)
            {
                return (false, string.Empty);
            }

            return (true, memberResponse.Data.Id.ToString());
        }

        public static async Task<(bool success, Member? member)> GetMemberModelForLoggedInUser(HttpRequest request)
        {
            var hasAccountId = request.Headers.TryGetValue(RequireOrganizationAuthenticationAttribute.WildApricotAccountIdHeader, out var accountId);
            var hasAuthorization = request.HttpContext.Request.Headers.TryGetValue(RequireOrganizationAuthenticationAttribute.WildApricotAuthorizationHeader, out var authorization);
            if (!hasAccountId || !hasAuthorization)
            {
                return (false, null);
            }

            var client = new RestClient("https://api.wildapricot.org/publicview/v1");

            var accountRequest = new RestRequest($"accounts/{accountId}/contacts/me?includeDetails=false", DataFormat.Json);
            accountRequest.AddHeader("Authorization", authorization);

            var memberResponse = await client.ExecuteGetAsync<WildApricotMember>(accountRequest);

            if (!memberResponse.IsSuccessful)
            {
                return (false, null);
            }

            var member = new Member()
            {
                FirstName = memberResponse.Data.FirstName,
                LastName = memberResponse.Data.LastName,
                Email = memberResponse.Data.Email,
                OrgAssignedMemberId = memberResponse.Data.Id.ToString(),
            };

            return (true, member);
        }
    }
}