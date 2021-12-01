using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using RaceResults.Common.Models;

namespace RaceResults.Data.Core
{
    public class RaceContainerClient : IRaceContainerClient
    {
        private readonly Container container;

        public RaceContainerClient(ICosmosDbClient cosmosDbClient)
        {
            this.container = cosmosDbClient.GetContainer(ContainerConstants.RaceContainerName);
        }

        public async Task<Race> GetRaceAsync(string raceId)
        {
            PartitionKey partitionKey = new PartitionKey(raceId);
            ItemResponse<Race> response = await this.container.ReadItemAsync<Race>(raceId, partitionKey);
            return response.Resource;
        }

        public async Task<IEnumerable<Race>> GetAllRacesAsync()
        {
            FeedIterator<Race> iterator = this.container.GetItemLinqQueryable<Race>().ToFeedIterator();

            List<Race> results = new List<Race>();
            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task AddRaceAsync(Race race)
        {
            race.Id = Guid.NewGuid();
            await this.container.CreateItemAsync<Race>(race);
        }
    }
}
