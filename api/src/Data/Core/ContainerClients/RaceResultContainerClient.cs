using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RaceResults.Common.Models;

namespace RaceResults.Data.Core
{
    public class RaceResultContainerClient : ContainerClient<RaceResult>
    {
        public RaceResultContainerClient(ICosmosDbClient cosmosDbClient)
            : base(cosmosDbClient, ContainerConstants.RaceResultContainerName)
        {
        }

        public async Task<IEnumerable<RaceResult>> GetRaceResultsForMemberAsync(string memberId)
        {
            return await GetRaceResultsForMembersAsync(new string[] { memberId });
        }

        public async Task<IEnumerable<RaceResult>> GetRaceResultsForMembersAsync(IEnumerable<string> memberIds)
        {
            var memberGuids = memberIds.Select(id => Guid.Parse(id)).ToHashSet();
            return await this.GetManyAsync(it => it.Where(raceResult => memberGuids.Contains(raceResult.MemberId)));
        }
    }
}
