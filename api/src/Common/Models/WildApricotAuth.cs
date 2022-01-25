using System;
using System.ComponentModel.DataAnnotations;

namespace RaceResults.Common.Models
{
    public struct WildApricotAuth : IAuthModel
    {
        public Guid Id { get; set; }

        [Required]
        public Guid OrganizationId { get; set; }

        [Required]
        public string[] Domains { get; set; }

        [Required]
        public string ClientId { get; set; }

        public string GetPartitionKey()
        {
            return OrganizationId.ToString();
        }
    }
}
