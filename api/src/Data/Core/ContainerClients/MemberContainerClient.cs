using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using RaceResults.Common.Models;

namespace RaceResults.Data.Core
{
    public class MemberContainerClient : IMemberContainerClient
    {
        private readonly Container container;

        public MemberContainerClient(ICosmosDbClient cosmosDbClient)
        {
            this.container = cosmosDbClient.GetContainer(ContainerConstants.MemberContainerName);
        }

        public async Task<Member> GetMemberAsync(string orgId, string memberId)
        {
            PartitionKey partitionKey = new PartitionKey(orgId);
            ItemResponse<Member> response = await this.container.ReadItemAsync<Member>(memberId, partitionKey);
            return response.Resource;
        }

        public async Task<IEnumerable<Member>> GetAllMembersAsync(string orgId)
        {
            FeedIterator<Member> iterator = this.container.GetItemLinqQueryable<Member>()
                .Where(runner => runner.OrganizationId == Guid.Parse(orgId)).ToFeedIterator();

            List<Member> results = new List<Member>();
            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task AddMemberAsync(string orgId, Member member)
        {
            await this.container.CreateItemAsync<Member>(member);
        }

        public async Task DeleteMemberAsync(string orgId, string memberId)
        {
            PartitionKey partitionKey = new PartitionKey(orgId);
            await this.container.DeleteItemAsync<Member>(memberId, partitionKey);
        }
    }
}
