using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using RaceResults.Common.Models;

namespace RaceResults.Data.Core
{
    public class RaceResultContainerClient : IRaceResultContainerClient
    {
        private readonly Container container;

        public RaceResultContainerClient(ICosmosDbClient cosmosDbClient)
        {
            this.container = cosmosDbClient.GetContainer(ContainerConstants.RaceResultContainerName);
        }

        public async Task<RaceResult> GetRaceResultAsync(string memberId, string raceResultId)
        {
            PartitionKey partitionKey = new PartitionKey(memberId);
            ItemResponse<RaceResult> response = await this.container.ReadItemAsync<RaceResult>(raceResultId, partitionKey);
            return response.Resource;
        }

        public async Task<IEnumerable<RaceResult>> GetAllRaceResultsAsync(string memberId)
        {
            FeedIterator<RaceResult> iterator = this.container.GetItemLinqQueryable<RaceResult>()
                .Where(result => result.MemberId == Guid.Parse(memberId)).ToFeedIterator();

            List<RaceResult> results = new List<RaceResult>();
            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task<IEnumerable<RaceResult>> GetAllRaceResultsAsync(IEnumerable<string> memberIds)
        {
            List<RaceResult> results = new List<RaceResult>();
            foreach (string memberId in memberIds)
            {
                results.AddRange(await GetAllRaceResultsAsync(memberId));
            }

            return results;
        }

        public async Task AddRaceResultAsync(RaceResult raceResult)
        {
            raceResult.Id = Guid.NewGuid();
            await this.container.CreateItemAsync<RaceResult>(raceResult);
        }
    }
}
