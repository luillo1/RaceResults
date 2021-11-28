namespace RaceResults.Common.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

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
