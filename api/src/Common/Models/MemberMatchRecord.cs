using System.ComponentModel.DataAnnotations;

namespace RaceResults.Common.Models
{
    public class MemberMatchRecord
    {
        [Required]
        public string Id { get; set; }

        [Required]
        public double Probability { get; set; }
    }
}
