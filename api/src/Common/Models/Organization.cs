namespace RaceResults.Common.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class Organization
    {
        public Guid Id { get; } = Guid.NewGuid();

        [Required]
        public string Name { get; set; }
    }
}
