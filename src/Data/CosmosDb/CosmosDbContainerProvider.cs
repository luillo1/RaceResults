using RaceResults.Common.Models;

namespace RaceResults.Data.CosmosDb
{
    public class CosmosDbContainerProvider : ICosmosDbContainerProvider
    {
        private const string RaceResultContainerName = "RaceResultsContainer";

        private const string RaceContainerName = "RacesContainer";

        private const string RunnerContainerName = "RunnersContainer";

        public ICosmosDbContainerClient<RaceResult> RaceResultContainer { get; }

        public ICosmosDbContainerClient<Race> RaceContainer { get; }

        public ICosmosDbContainerClient<Runner> RunnerContainer { get; }

        public CosmosDbContainerProvider(ICosmosDbClient cosmosDbClient)
        {
            this.RaceResultContainer = new CosmosDbContainerClient<RaceResult>(
                    cosmosDbClient,
                    CosmosDbContainerProvider.RaceResultContainerName);

            this.RaceContainer = new CosmosDbContainerClient<Race>(
                    cosmosDbClient,
                    CosmosDbContainerProvider.RaceContainerName);

            this.RunnerContainer= new CosmosDbContainerClient<Runner>(
                    cosmosDbClient,
                    CosmosDbContainerProvider.RunnerContainerName);
        }
    }
}
