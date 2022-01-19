using System.ComponentModel.DataAnnotations;
using RaceResults.Common.Models;

namespace RaceResults.Api.RequestObjects
{
    public class CreateWildApricotAuthRequest : LoginRequest
    {
        [Required]
        public string ClientSecret { get; set; }

        [Required]
        public WildApricotAuth Auth { get; set; }
    }
}
