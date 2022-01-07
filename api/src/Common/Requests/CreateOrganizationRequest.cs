using RaceResults.Common.Models;

namespace RaceResults.Common.Requests
{
    public class CreateOrganizationRequest
    {
        public Organization Organization { get; set; }

        public string ClientSecret { get; set; }
    }
}