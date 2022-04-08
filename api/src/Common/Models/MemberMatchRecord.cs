using System;
using System.ComponentModel.DataAnnotations;

namespace RaceResults.Common.Models
{
    public class MemberMatchRecord : IModel
    {
        // TODO: question, can this even implement IModel?? Like how is this set up on cosmosdb...
        // 1. Need IModel to implement to use the ContainerClient interface
        // 2. Container Client interface needed so the e2e use of MemberMatchRecord looks like other models
        // 3. What is the partitionkey?? I need that in Scorer.cs to access the db and get the probability.
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string nameId { get; set; }

        [Required]
        public double Probability { get; set; }

        public string GetPartitionKey()
        {
            return nameId.ToString();
        }
    }
}
