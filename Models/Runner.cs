using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RaceResults.Models
{
    public class Runner
    {
        public Guid Id { get; } = Guid.NewGuid();

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public List<string> Nicknames { get; set; }
    }
}
