namespace RaceResults.Data.Core
{
    using RaceResults.Common.Models;

    public class CosmosDbContainerProvider : ICosmosDbContainerProvider
    {
        private const string RaceResultContainerName = "RaceResultContainer";

        private const string RaceContainerName = "RaceContainer";

        private const string RunnerContainerName = "RunnerContainer";

        private const string OrganizationContainerName = "OrganizationContainer";

        private const string MemberMatchRecordContainerName = "MemberMatchRecordContainer";

        public ICosmosDbContainerClient<RaceResult> RaceResultContainer { get; }

        public ICosmosDbContainerClient<Race> RaceContainer { get; }

        public ICosmosDbContainerClient<Runner> RunnerContainer { get; }

        public ICosmosDbContainerClient<Organization> OrganizationContainer { get; }

        public ICosmosDbContainerClient<MemberMatchRecord> MemberMatchRecordContainer { get; }

        public CosmosDbContainerProvider(ICosmosDbClient cosmosDbClient)
        {
            this.RaceResultContainer = new CosmosDbContainerClient<RaceResult>(
                    cosmosDbClient,
                    CosmosDbContainerProvider.RaceResultContainerName);

            this.RaceContainer = new CosmosDbContainerClient<Race>(
                    cosmosDbClient,
                    CosmosDbContainerProvider.RaceContainerName);

            this.RunnerContainer = new CosmosDbContainerClient<Runner>(
                    cosmosDbClient,
                    CosmosDbContainerProvider.RunnerContainerName);

            this.OrganizationContainer = new CosmosDbContainerClient<Organization>(
                    cosmosDbClient,
                    CosmosDbContainerProvider.OrganizationContainerName);

            this.MemberMatchRecordContainer = new CosmosDbContainerClient<MemberMatchRecord>(
                    cosmosDbClient,
                    CosmosDbContainerProvider.MemberMatchRecordContainerName);
        }
    }
}
