using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using RaceResults.Common.Models;

namespace RaceResults.Data.Core
{
    /// <summary>
    ///     Client used to interact with the container for <see cref="RaceResult"/> models.
    /// </summary>
    public class RaceResultContainerClient
        : ContainerClient<RaceResult>
    {
        /// <inheritdoc/>
        protected override string ContainerName => ContainerConstants.RaceResultContainerName;

        /// <summary>
        ///     Initializes a new instance of the <see cref="RaceResultContainerClient"/> class.
        /// </summary>
        /// <param name="cosmosDbClient">
        ///     The <see cref="ICosmosDbClient"/> used to get an instance of a <see cref="Container"/>.
        /// </param>
        public RaceResultContainerClient(ICosmosDbClient cosmosDbClient)
            : base(cosmosDbClient)
        {
        }

        /// <summary>
        ///     Queries the container for all instances of the <see cref="RaceResult"/> model
        ///     that are for a specific <see cref="Member"/>.
        /// </summary>
        /// <param name="memberId">
        ///     The <see cref="IModel.Id"/> of the <see cref="Member"/> to return
        ///     <see cref="RaceResult"/>s for.
        /// </param>
        /// <returns>
        ///     An await-able task that queries the container and returns the result.
        /// </returns>
        public async Task<IEnumerable<RaceResult>> GetRaceResultsForMemberAsync(string memberId)
        {
            return await GetRaceResultsForMembersAsync(new string[] { memberId });
        }

        /// <summary>
        ///     Queries the container for all instances of the <see cref="RaceResult"/> model
        ///     that are for specific <see cref="Member"/>s.
        /// </summary>
        /// <param name="memberIds">
        ///     The <see cref="IModel.Id"/>s of the <see cref="Member"/>s to return
        ///     <see cref="RaceResult"/>s for.
        /// </param>
        /// <returns>
        ///     An await-able task that queries the container and returns the result.
        /// </returns>
        public async Task<IEnumerable<RaceResult>> GetRaceResultsForMembersAsync(IEnumerable<string> memberIds)
        {
            var memberGuids = memberIds.Select(id => Guid.Parse(id)).ToHashSet();
            return await this.GetModelsAsync(it => it.Where(raceResult => memberGuids.Contains(raceResult.MemberId)));
        }
    }
}
