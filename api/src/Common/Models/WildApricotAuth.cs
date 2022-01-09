using System;
using System.ComponentModel.DataAnnotations;

namespace RaceResults.Common.Models
{
    public struct WildApricotAuth : IAuthModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public Guid OrganizationId { get; set; }

        [Required]
        public string Domain { get; set; }

        [Required]
        public string ClientId { get; set; }

        [Required]
        public AuthType AuthType
        {
            get
            {
                return AuthType.WildApricot;
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
