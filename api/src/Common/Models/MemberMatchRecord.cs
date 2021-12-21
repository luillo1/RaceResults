using System;
using System.ComponentModel.DataAnnotations;

namespace RaceResults.Common.Models
{
    public class MemberMatchRecord : IModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public double Probability { get; set; }

        public string GetPartitionKey()
        {
            return Id.ToString();
        }
    }
}
