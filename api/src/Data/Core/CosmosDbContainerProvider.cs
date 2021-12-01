using RaceResults.Common.Models;

namespace RaceResults.Data.Core
{
    public class CosmosDbContainerProvider : ICosmosDbContainerProvider
    {
        public IRaceResultContainerClient RaceResultContainer { get; }

        public IRaceContainerClient RaceContainer { get; }

        public IMemberContainerClient MemberContainer { get; }

        public IOrganizationContainerClient OrganizationContainer { get; }

        public ICosmosDbContainerClient<MemberMatchRecord> MemberMatchRecordContainer { get; }

        public CosmosDbContainerProvider(ICosmosDbClient cosmosDbClient)
        {
            this.RaceResultContainer = new RaceResultContainerClient(cosmosDbClient);

            this.RaceContainer = new RaceContainerClient(cosmosDbClient);

            this.MemberContainer = new MemberContainerClient(cosmosDbClient);

            this.OrganizationContainer = new OrganizationContainerClient(cosmosDbClient);

            this.MemberMatchRecordContainer = new CosmosDbContainerClient<MemberMatchRecord>(
                    cosmosDbClient,
                    ContainerConstants.MemberMatchRecordContainerName);
        }
    }
}
