using System.Collections.Generic;
using System.Threading.Tasks;
using RaceResults.Common.Models;

namespace RaceResults.Data.Core
{
    public interface IMemberContainerClient
    {
        Task<Member> GetMemberAsync(string orgId, string memberId);

        Task<IEnumerable<Member>> GetAllMembersAsync(string orgId);

        Task AddMemberAsync(string orgId, Member member);

        Task DeleteMemberAsync(string orgId, string memberId);
    }
}
