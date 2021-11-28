namespace RaceResults.Data.Core
{
    using RaceResults.Common.Models;

    public interface ICosmosDbContainerProvider
    {
        ICosmosDbContainerClient<RaceResult> RaceResultContainer { get; }

        ICosmosDbContainerClient<Race> RaceContainer { get; }

        ICosmosDbContainerClient<Runner> RunnerContainer { get; }

        ICosmosDbContainerClient<Organization> OrganizationContainer { get; }

        ICosmosDbContainerClient<MemberMatchRecord> MemberMatchRecordContainer { get; }
    }
}
