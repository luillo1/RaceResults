using RaceResults.Common.Models;

namespace RaceResults.Data.Core
{
    public interface ICosmosDbContainerProvider
    {
        ICosmosDbContainerClient<RaceResult> RaceResultContainer { get; }

        ICosmosDbContainerClient<Race> RaceContainer { get; }

        ICosmosDbContainerClient<Runner> RunnerContainer { get; }

        ICosmosDbContainerClient<Organization> OrganizationContainer { get; }

        ICosmosDbContainerClient<MemberMatchRecord> MemberMatchRecordContainer { get; }
    }
}
