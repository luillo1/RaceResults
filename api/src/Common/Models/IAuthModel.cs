using System;
using System.ComponentModel.DataAnnotations;

namespace RaceResults.Common.Models
{
    public interface IAuthModel : IModel
    {
        Guid Id { get; set; }

        Guid OrganizationId { get; set; }

        AuthType AuthType { get; set; }
    }
}
