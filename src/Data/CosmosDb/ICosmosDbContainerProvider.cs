using RaceResults.Common.Models;

namespace RaceResults.Data.CosmosDb
{
    public interface ICosmosDbContainerProvider
    {
        ICosmosDbContainerClient<RaceResult> RaceResultContainer { get; }

        ICosmosDbContainerClient<Race> RaceContainer { get; }

        ICosmosDbContainerClient<Runner> RunnerContainer { get; }
    }
}
