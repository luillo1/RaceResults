namespace RaceResults.Data.Core
{
    public class CosmosDbContainerProvider : ICosmosDbContainerProvider
    {
        public RaceResultContainerClient RaceResultContainer { get; }

        public RaceContainerClient RaceContainer { get; }

        public MemberContainerClient MemberContainer { get; }

        public OrganizationContainerClient OrganizationContainer { get; }

        public CosmosDbContainerProvider(ICosmosDbClient cosmosDbClient)
        {
            this.RaceResultContainer = new RaceResultContainerClient(cosmosDbClient);

            this.RaceContainer = new RaceContainerClient(cosmosDbClient);

            this.MemberContainer = new MemberContainerClient(cosmosDbClient);

            this.OrganizationContainer = new OrganizationContainerClient(cosmosDbClient);
        }
    }
}
