using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RaceResults.Common.Models
{
    public class Member : IModel
    {
        public Guid Id { get; set; }

        [Required]
        public Guid OrganizationId { get; set; }

        [Required]
        public string OrgAssignedMemberId { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public List<string> Nicknames { get; set; }

        public string Hometown { get; set; }

        public string GetPartitionKey()
        {
            return OrganizationId.ToString();
        }
    }
}
