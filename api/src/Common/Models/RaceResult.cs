using System;
using System.ComponentModel.DataAnnotations;

namespace RaceResults.Common.Models
{
    public class RaceResult
    {
        public Guid Id { get; } = Guid.NewGuid();

        [Required]
        public Runner Runner { get; set; }

        [Required]
        public Race Race { get; set; }

        [Required]
        public TimeSpan Time { get; set; }
    }
}
