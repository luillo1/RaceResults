using System.Collections.Generic;

namespace RaceResults.Api.ResponseObjects
{
    public struct OrganizationLoginResponse
    {
        public string OrgAssignedMemberId { get; set; }

        public IEnumerable<KeyValuePair<string, string>> RequiredHeaders { get; set; }
    }
}
