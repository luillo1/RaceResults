using System;
using System.ComponentModel.DataAnnotations;

namespace RaceResults.Common.Models
{
    public class Race : IModel
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

        public string GetPartitionKey()
        {
            return Id.ToString();
        }
    }
}
