using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RaceResults.Common.Exceptions;
using RaceResults.Common.Models;

namespace RaceResults.Data.Core
{
    public class SubmissionCheckpointContainerClient : ContainerClient<SubmissionCheckpoint>
    {
        public SubmissionCheckpointContainerClient(ICosmosDbClient cosmosDbClient)
            : base(cosmosDbClient.GetContainer(ContainerConstants.SubmissionCheckpointContainerName))
        {
        }

        public async Task<IEnumerable<SubmissionCheckpoint>> GetAllSubmissionCheckpointsAsync(string orgId)
        {
            var orgGuid = Guid.Parse(orgId);
            return await this.GetManyAsync(it => it.Where(checkpoint => checkpoint.OrganizationId == orgGuid));
        }
    }
}
