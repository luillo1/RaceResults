using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RaceResults.Common.Models;

namespace RaceResults.Data.Core
{
    public class MemberContainerClient : ContainerClient<Member>
    {
        public MemberContainerClient(ICosmosDbClient cosmosDbClient)
            : base(cosmosDbClient, ContainerConstants.MemberContainerName)
        {
        }

        public async Task<IEnumerable<Member>> GetAllMembersAsync(string orgId)
        {
            var orgGuid = Guid.Parse(orgId);
            return await this.GetManyAsync(it => it.Where(member => member.OrganizationId == orgGuid));
        }
    }
}
