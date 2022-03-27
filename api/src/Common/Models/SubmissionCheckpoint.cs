using System;
using System.ComponentModel.DataAnnotations;

namespace RaceResults.Common.Models
{
    public struct SubmissionCheckpoint : IModel
    {
        public Guid Id { get; set; }

        [Required]
        public Guid OrganizationId { get; set; }

        [Required]
        public DateTime Checkpointed { get; set; }

        public string GetPartitionKey()
        {
            return OrganizationId.ToString();
        }
    }
}
