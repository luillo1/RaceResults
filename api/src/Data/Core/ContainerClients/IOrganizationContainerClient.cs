using System.Collections.Generic;
using System.Threading.Tasks;
using RaceResults.Common.Models;

namespace RaceResults.Data.Core
{
    public interface IOrganizationContainerClient
    {
        Task<Organization> GetOrganizationAsync(string orgId);

        Task<IEnumerable<Organization>> GetAllOrganizationsAsync();

        Task AddOrganizationAsync(Organization organization);
    }
}
