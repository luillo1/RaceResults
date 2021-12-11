using System;
using System.ComponentModel.DataAnnotations;

namespace RaceResults.Common.Models
{
    public class Organization : IModel
    {
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string GetPartitionKey()
        {
            return Id.ToString();
        }
    }
}
