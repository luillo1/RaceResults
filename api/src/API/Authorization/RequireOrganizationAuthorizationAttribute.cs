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
    public class RequireOrganizationAuthorizationAttribute : ActionFilterAttribute
    {
        public const string WildApricotAccountIdHeader = "WA-AccountId";
        public const string WildApricotAuthorizationHeader = "WA-Authorization";

        private readonly ICosmosDbContainerProvider containerProvider;

        public RequireOrganizationAuthorizationAttribute(ICosmosDbContainerProvider containerProvider)
        {
            this.containerProvider = containerProvider;
        }

        public override async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            var orgId = GetOrgId(context);
            var org = await containerProvider.OrganizationContainer.GetOneAsync(orgId, orgId);
            var orgAssignedMemberId = TryGetOrgAssignedMemberId(context);

            bool authorized;
            switch (org.AuthType)
            {
                case AuthType.RaceResults:
                    authorized = AuthorizeRaceResults(context);
                    break;
                case AuthType.WildApricot:
                    authorized = await AuthorizeWildApricot(context, orgAssignedMemberId);
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

        private bool AuthorizeRaceResults(ActionExecutingContext context)
        {
            // TODO: find a better way to do this
            return context.HttpContext.User.Identity.IsAuthenticated;
        }

        private async Task<bool> AuthorizeWildApricot(
            ActionExecutingContext context,
            string orgAssignedMemberId)
        {
            var hasAccountId = context.HttpContext.Request.Headers.TryGetValue(WildApricotAccountIdHeader, out var accountId);
            var hasAuthorization = context.HttpContext.Request.Headers.TryGetValue(WildApricotAuthorizationHeader, out var authorization);
            if (!hasAccountId || !hasAuthorization)
            {
                return false;
            }

            var response = await WildApricotApi.GetLoggedInMembersOrgIdAsync(context.HttpContext.Request);
            if (!response.success)
            {
                return false;
            }

            if (orgAssignedMemberId == null)
            {
                // TODO: figure out a way to ensure logged in user has acces
                // to a specific org
                return true;
            }
            else
            {
                return string.Equals(response.memberId, orgAssignedMemberId, StringComparison.CurrentCultureIgnoreCase);
            }
        }

        private string TryGetOrgAssignedMemberId(ActionExecutingContext context)
        {
            // Look for "orgAssignedMemberId" in any model field, controller parameter, etc.
            if (context.ModelState.TryGetValue("orgAssignedMemberId", out var modelState)
                && modelState.RawValue is string orgAssignedMemberId
                && !string.IsNullOrEmpty(orgAssignedMemberId))
            {
                return orgAssignedMemberId;
            }

            return null;
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