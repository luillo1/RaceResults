using System;
using System.Linq;
using System.Threading.Tasks;

using RaceResults.Common.Models;

namespace RaceResults.Data.Core
{
    public class AuthContainerClient<T> : ContainerClient<T>
    where T : IAuthModel
    {
        public AuthContainerClient(ICosmosDbClient cosmosDbClient, string containerName)
            : base(cosmosDbClient.GetContainer(containerName))
        {
        }

        public async Task<T> GetAuthForOrganization(string orgId)
        {
            var orgGuid = Guid.Parse(orgId);
            var all = await this.GetAllAsync();
            return (await this.GetManyAsync(it => it.Where(auth => auth.OrganizationId == orgGuid))).Single();
        }
    }
}
