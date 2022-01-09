using System.Collections.Generic;

namespace RaceResults.Api.MemberProviders.WildApricot
{
    public struct WildApricotOauthResponse
    {
        public struct WildApricotOauthPermissionsObject
        {
            public int AccountId { get; set; }

            public IEnumerable<string> AvailableScopes { get; set; }
        }

        public string AccessToken { get; set; }

        public string TokenType { get; set; }

        public uint ExpiresIn { get; set; }

        public string RefreshToken { get; set; }

        public IEnumerable<WildApricotOauthPermissionsObject> Permissions { get; set; }
    }
}
