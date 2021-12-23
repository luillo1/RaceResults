using System;
using System.ComponentModel.DataAnnotations;

namespace RaceResults.Common.Models
{
    public class RaceResult : IModel
    {
        public Guid Id { get; set; }

        [Required]
        public Guid MemberId { get; set; }

        [Required]
        public Guid RaceId { get; set; }

        [Required]
        public TimeSpan Time { get; set; }

        public string Comments { get; set; }

        public string DataSource { get; set; }

        public DateTime Submitted { get; set; }

        public string GetPartitionKey()
        {
            return MemberId.ToString();
        }
    }
}
