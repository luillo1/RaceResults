using RaceResults.Common.Models;

namespace RaceResults.Data.Core
{
    public interface ICosmosDbContainerProvider
    {
        RaceResultContainerClient RaceResultContainer { get; }

        RaceContainerClient RaceContainer { get; }

        MemberContainerClient MemberContainer { get; }

        SubmissionCheckpointContainerClient SubmissionCheckpointContainer { get; }

        OrganizationContainerClient OrganizationContainer { get; }

        AuthContainerClient<RaceResultsAuth> RaceResultsAuthContainer { get; }

        AuthContainerClient<WildApricotAuth> WildApricotAuthContainer { get; }

        // ICosmosDbContainerClient<MemberMatchRecord> MemberMatchRecordContainer { get; }
    }
}
