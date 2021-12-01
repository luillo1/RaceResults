using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using RaceResults.Common.Models;

namespace RaceResults.Data.Core
{
    public class OrganizationContainerClient : IOrganizationContainerClient
    {
        private readonly Container container;

        public OrganizationContainerClient(ICosmosDbClient cosmosDbClient)
        {
            this.container = cosmosDbClient.GetContainer(ContainerConstants.OrganizationContainerName);
        }

        public async Task<Organization> GetOrganizationAsync(string orgId)
        {
            PartitionKey partitionKey = new PartitionKey(orgId);
            ItemResponse<Organization> response = await this.container.ReadItemAsync<Organization>(orgId, partitionKey);
            return response.Resource;
        }

        public async Task<IEnumerable<Organization>> GetAllOrganizationsAsync()
        {
            FeedIterator<Organization> iterator = this.container.GetItemLinqQueryable<Organization>().ToFeedIterator();

            List<Organization> results = new List<Organization>();
            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task AddOrganizationAsync(Organization organization)
        {
            organization.Id = Guid.NewGuid();
            await this.container.CreateItemAsync<Organization>(organization);
        }
    }
}
