using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using RaceResults.Common.Models;

namespace RaceResults.Data.Core
{
    public class MemberMatchContainerClient
    {
        private readonly Container container;

        public MemberMatchContainerClient(Container container)
        {
            this.container = container;
        }

        public async Task<MemberMatchRecord> GetOneAsync(string id, string partitionKey)
        {
            PartitionKey partition = new PartitionKey(partitionKey);
            ItemResponse<MemberMatchRecord> response = await this.container.ReadItemAsync<MemberMatchRecord>(id, partition);
            return response.Resource;
        }
    }
}
