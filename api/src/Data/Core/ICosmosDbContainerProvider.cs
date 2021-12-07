namespace RaceResults.Data.Core
{
    public interface ICosmosDbContainerProvider
    {
        RaceResultContainerClient RaceResultContainer { get; }

        RaceContainerClient RaceContainer { get; }

        MemberContainerClient MemberContainer { get; }

        OrganizationContainerClient OrganizationContainer { get; }

        // ICosmosDbContainerClient<MemberMatchRecord> MemberMatchRecordContainer { get; }
    }
}
