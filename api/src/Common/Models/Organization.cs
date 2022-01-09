using System;
using System.ComponentModel.DataAnnotations;

namespace RaceResults.Common.Models
{
    public struct Organization : IModel
    {
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string WildApricotDomain { get; set; }

        [Required]
        public string WildApricotClientId { get; set; }

        [Required]
        public AuthType AuthType { get; set; }

        public string GetPartitionKey()
        {
            return Id.ToString();
        }
    }
}
