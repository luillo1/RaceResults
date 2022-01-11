using System;
using System.ComponentModel.DataAnnotations;

namespace RaceResults.Common.Models
{
    public struct RaceResultsAuth : IAuthModel
    {
        public Guid Id { get; set; }

        [Required]
        public Guid OrganizationId { get; set; }

        [Required]
        public AuthType AuthType
        {
            get
            {
                return AuthType.RaceResults;
            }

            set
            {
            }
        }

        public string GetPartitionKey()
        {
            return OrganizationId.ToString();
        }
    }
}
