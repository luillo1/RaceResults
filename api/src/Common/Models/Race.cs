using System;
using System.ComponentModel.DataAnnotations;

namespace RaceResults.Common.Models
{
    public enum Distance
    {
        FiveK,
        TenK,
        HalfMarathon,
        Marathon
    }

    public class Race
    {
        public Guid Id { get; } = Guid.NewGuid();

        [Required]
        public string Name { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public string Location { get; set; }

        [Required]
        public Distance Distance { get; set; }
    }
}
