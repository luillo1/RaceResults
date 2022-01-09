using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using RaceResults.Api.MemberProviders.WildApricot;
using RaceResults.Common.Models;
using RaceResults.Data.Core;

namespace RaceResults.Api.Authorization
{
    /// <summary>
    ///     Used to denote that a route requires the request to come from a user that is both
    ///     authenticated to an organization and has a specific orgAssignedMemberId.
    ///     Routes that use this attribute MUST decorate a parameter with a <see cref="RaceResults.Api.Parameters.OrganizationIdAttribute" />
    ///     and have a parameter or bound model property, named either "orgAssignedMemberId" or "memberId".
    ///  </summary>
    public class RequireOrganizationAuthorizationAttribute : RequireOrganizationAuthenticationAttribute
    {
        public RequireOrganizationAuthorizationAttribute(ICosmosDbContainerProvider containerProvider)
        : base(containerProvider)
        {
        }

        protected override Task<bool> TestAgainstRaceResultsAuth(ActionExecutingContext context, Organization org)
        {
            // TODO: do something
            return base.TestAgainstRaceResultsAuth(context, org);
        }

        protected virtual async Task<bool> AuthorizeWildApricot(
            ActionExecutingContext context,
            Organization org)
        {
            var orgAssignedMemberId = await GetOrgAssignedMemberId(context, org);

            var response = await WildApricotApi.GetLoggedInMembersOrgIdAsync(context.HttpContext.Request);
            if (!response.success)
            {
                return false;
            }
            else
            {
                return string.Equals(response.memberId, orgAssignedMemberId, StringComparison.CurrentCultureIgnoreCase);
            }
        }

        private async Task<string> GetOrgAssignedMemberId(ActionExecutingContext context, Organization org)
        {
            // Look for "orgAssignedMemberId" in any model field, controller parameter, etc.
            if (context.ModelState.TryGetValue("orgAssignedMemberId", out var orgAssignedMemberIdModelState)
                && orgAssignedMemberIdModelState.RawValue is string orgAssignedMemberId
                && !string.IsNullOrEmpty(orgAssignedMemberId))
            {
                return orgAssignedMemberId;
            }

            // Look for "memberId" in any model field, controller parameter, etc.
            if (context.ModelState.TryGetValue("memberId", out var memberIdmodelState)
                && memberIdmodelState.RawValue is string memberId
                && !string.IsNullOrEmpty(memberId))
            {
                var member = await containerProvider.MemberContainer.GetOneAsync(memberId, org.Id.ToString());
                return member.OrgAssignedMemberId;
            }

            throw new InvalidOperationException("No orgAssignedMemberId or memberId found in request");
        }
    }
}