using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using RaceResults.Common.Models;

namespace RaceResults.Data.Core
{
    /// <summary>
    ///     Client used to interact with the container for <see cref="Member"/> models.
    /// </summary>
    public class MemberContainerClient
        : ContainerClient<Member>
    {
        /// <inheritdoc/>
        protected override string ContainerName => ContainerConstants.MemberContainerName;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MemberContainerClient"/> class.
        /// </summary>
        /// <param name="cosmosDbClient">
        ///     The <see cref="ICosmosDbClient"/> used to get an instance of a <see cref="Container"/>.
        /// </param>
        public MemberContainerClient(ICosmosDbClient cosmosDbClient)
            : base(cosmosDbClient)
        {
        }

        /// <summary>
        ///     Queries the container for all instances of the <see cref="Member"/> model
        ///     that belong to a specific <see cref="Organization"/>.
        /// </summary>
        /// <param name="orgId">
        ///     The <see cref="IModel.Id"/> of the <see cref="Organization"/> to return
        ///     members of.
        /// </param>
        /// <returns>
        ///     An await-able task that queries the container and returns the result.
        /// </returns>
        public async Task<IEnumerable<Member>> GetAllMembersAsync(string orgId)
        {
            var orgGuid = Guid.Parse(orgId);
            return await this.GetModelsAsync(it => it.Where(member => member.OrganizationId == orgGuid));
        }
    }
}
