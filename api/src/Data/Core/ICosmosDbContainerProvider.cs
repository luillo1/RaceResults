using RaceResults.Common.Models;

namespace RaceResults.Data.Core
{
    public interface ICosmosDbContainerProvider
    {
        IRaceResultContainerClient RaceResultContainer { get; }

        IRaceContainerClient RaceContainer { get; }

        IMemberContainerClient MemberContainer { get; }

        IOrganizationContainerClient OrganizationContainer { get; }

        ICosmosDbContainerClient<MemberMatchRecord> MemberMatchRecordContainer { get; }
    }
}
