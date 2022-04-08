using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using RaceResults.Common.Models;

namespace RaceResults.Data.Core
{
    public class MemberMatchContainerClient : ContainerClient<MemberMatchRecord>
    {
        // TODO: Determine whether or not this needs to implement ContainerClient<IModel> (all of the other ones do and it makes it easier E2E)
        public MemberMatchContainerClient(ICosmosDbClient cosmosDbClient)
            : base(cosmosDbClient.GetContainer(ContainerConstants.MemberMatchRecordContainerName))
        {
        }
    }
}
