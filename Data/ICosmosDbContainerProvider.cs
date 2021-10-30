using RaceResults.Models;

namespace RaceResults.Data
{
    public interface ICosmosDbContainerProvider
    {
        ICosmosDbContainerClient<RaceResult> RaceResultContainer { get; }

        ICosmosDbContainerClient<Race> RaceContainer { get; }

        ICosmosDbContainerClient<Runner> RunnerContainer { get; }
    }
}
