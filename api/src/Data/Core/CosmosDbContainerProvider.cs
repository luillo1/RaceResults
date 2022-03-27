using RaceResults.Common.Models;

namespace RaceResults.Data.Core
{
    public class CosmosDbContainerProvider : ICosmosDbContainerProvider
    {
        public RaceResultContainerClient RaceResultContainer { get; }

        public RaceContainerClient RaceContainer { get; }

        public MemberContainerClient MemberContainer { get; }

        public SubmissionCheckpointContainerClient SubmissionCheckpointContainer { get; }

        public OrganizationContainerClient OrganizationContainer { get; }

        public AuthContainerClient<RaceResultsAuth> RaceResultsAuthContainer { get; }

        public AuthContainerClient<WildApricotAuth> WildApricotAuthContainer { get; }

        public CosmosDbContainerProvider(ICosmosDbClient cosmosDbClient)
        {
            this.RaceResultContainer = new RaceResultContainerClient(cosmosDbClient);

            this.RaceContainer = new RaceContainerClient(cosmosDbClient);

            this.MemberContainer = new MemberContainerClient(cosmosDbClient);

            this.SubmissionCheckpointContainer = new SubmissionCheckpointContainerClient(cosmosDbClient);

            this.OrganizationContainer = new OrganizationContainerClient(cosmosDbClient);

            this.RaceResultsAuthContainer = new AuthContainerClient<RaceResultsAuth>(cosmosDbClient, ContainerConstants.RaceResultsAuthContainerName);

            this.WildApricotAuthContainer = new AuthContainerClient<WildApricotAuth>(cosmosDbClient, ContainerConstants.WildApricotAuthContainerName);
        }
    }
}
