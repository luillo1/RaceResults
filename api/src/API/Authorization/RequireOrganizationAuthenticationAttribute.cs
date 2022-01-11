using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using RaceResults.Api.MemberProviders.WildApricot;
using RaceResults.Api.Parameters;
using RaceResults.Common.Models;
using RaceResults.Data.Core;

namespace RaceResults.Api.Authorization
{
    /// <summary>
    ///     Used to denote that a route requires the request to come from a user
    ///     authenticated to an organization. Routes that use this attribute MUST decorate
    ///     a parameter with a <see cref="RaceResults.Api.Parameters.OrganizationIdAttribute" />.
    ///     <br />
    ///     This does not ensure the member is a particular person within that organization.
    ///     For that, use <see cref="RequireOrganizationAuthorizationAttribute"/>.
    ///  </summary>
    public class RequireOrganizationAuthenticationAttribute : ActionFilterAttribute
    {
        public const string WildApricotAccountIdHeader = "WA-AccountId";
        public const string WildApricotAuthorizationHeader = "WA-Authorization";

        protected readonly ICosmosDbContainerProvider containerProvider;

        public RequireOrganizationAuthenticationAttribute(ICosmosDbContainerProvider containerProvider)
        {
            this.containerProvider = containerProvider;
        }

        public override async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            var orgId = GetOrgId(context);
            var org = await containerProvider.OrganizationContainer.GetOneAsync(orgId, orgId);

            bool authorized;
            switch (org.AuthType)
            {
                case AuthType.RaceResults:
                    authorized = await TestAgainstRaceResultsAuth(context, org);
                    break;
                case AuthType.WildApricot:
                    authorized = await TestAgainstWildApricotAuth(context, org);
                    break;
                default:
                    throw new InvalidOperationException();
            }

            if (!authorized)
            {
                context.Result = new ForbidResult();
            }
            else
            {
                await next();
            }
        }

        protected virtual Task<bool> TestAgainstRaceResultsAuth(ActionExecutingContext context, Organization org)
        {
            // TODO (#85): find a better way to do this
            return Task.FromResult(context.HttpContext.User.Identity.IsAuthenticated);
        }

        protected virtual async Task<bool> TestAgainstWildApricotAuth(
            ActionExecutingContext context,
            Organization org)
        {
            var response = await WildApricotApi.GetLoggedInMembersOrgIdAsync(context.HttpContext.Request);
            if (!response.success)
            {
                return false;
            }
            else
            {
                // TODO (#84): Ensure the current user is authenticated against this specific org
                return true;
            }
        }

        private string GetOrgId(ActionExecutingContext context)
        {
            // Look for a OrganizationIdAttribute on any controller parameter
            var parameters = context.ActionDescriptor.Parameters;
            foreach (var param in parameters)
            {
                if (!(param is ControllerParameterDescriptor controllerParameter))
                {
                    continue;
                }

                var attributes = controllerParameter
                    .ParameterInfo
                    .GetCustomAttributes(typeof(OrganizationIdAttribute), false);

                if (!attributes.Any())
                {
                    continue;
                }

                var argument = context.ActionArguments
                    .Where(p => p.Key == param.Name)
                    .Select(s => s.Value)
                    .Single();

                if (argument is string orgId)
                {
                    return orgId;
                }
            }

            throw new InvalidOperationException("Did not find orgID in controller parameters.");
        }
    }
}