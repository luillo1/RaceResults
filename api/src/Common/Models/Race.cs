using System;
using System.ComponentModel.DataAnnotations;

namespace RaceResults.Common.Models
{
    // TODO (#52): Make races specific to organizations
    public struct Race : IModel
    {
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public Guid EventId { get; set; }

        [Required]
        public string Location { get; set; }

        [Required]
        public string Distance { get; set; }

        public bool IsPublic { get; set; }

        public DateTime Submitted { get; set; }

        public string GetPartitionKey()
        {
            // TODO (#52): Make races specific to organizations
            return Id.ToString();
        }
    }
}
