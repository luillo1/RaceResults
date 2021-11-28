using System;
using System.ComponentModel.DataAnnotations;

namespace RaceResults.Common.Models
{
    public class Organization
    {
        public Guid Id { get; } = Guid.NewGuid();

        [Required]
        public string Name { get; set; }
    }
}
