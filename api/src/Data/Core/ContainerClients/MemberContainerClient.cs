using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RaceResults.Common.Exceptions;
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

        public async Task<IEnumerable<Member>> GetMembersAsync(string orgId, IEnumerable<Guid> ids)
        {
            var orgGuid = Guid.Parse(orgId);
            var memberIds = ids.ToHashSet();
            return await this.GetManyAsync(it => it.Where(member => member.OrganizationId == orgGuid && memberIds.Contains(member.Id)));
        }

        public async Task<Member> GetOneMemberAsync(string orgAssignedMemberId, string orgId)
        {
           var orgGuid = Guid.Parse(orgId);
           IEnumerable<Member> result = await this.GetManyAsync(it => it.Where(member =>
                       member.OrganizationId == orgGuid &&
                       member.OrgAssignedMemberId == orgAssignedMemberId));
           try
           {
               Member member = result.Single();
               return member;
           }
           catch (InvalidOperationException)
           {
               throw new MemberIdNotFoundException();
           }
        }
    }
}
