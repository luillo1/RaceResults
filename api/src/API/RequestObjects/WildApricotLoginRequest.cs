using System.ComponentModel.DataAnnotations;

namespace RaceResults.Api.RequestObjects
{
    public class WildApricotLoginRequest : LoginRequest
    {
        [Required]
        public string AuthorizationCode { get; set; }

        [Required]
        public string RedirectUri { get; set; }

        [Required]
        public string Scope { get; set; }
    }
}