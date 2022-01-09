using System;

namespace RaceResults.Api.Parameters
{
    /// <summary>
    ///     Used to decorate a route parameter that represents an
    ///     <see cref="RaceResults.Common.Models.Organization.Id"/>.
    //      <br />
    //      This parameter allows <see cref="RaceResults.API.Authorization.RequireOrganizationAuthorizationAttribute" />
    //      to identity the organization to authorize against.
    //  </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class OrganizationIdAttribute : Attribute
    {
    }
}