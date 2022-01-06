using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RaceResults.Common.Models
{
    public struct RaceResult : IModel
    {
        public Guid Id { get; set; }

        [Required]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
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
