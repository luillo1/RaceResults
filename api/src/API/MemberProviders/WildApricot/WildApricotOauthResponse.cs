using System.Collections.Generic;

namespace RaceResults.Api.MemberProviders.WildApricot
{
    public struct WildApricotOauthResponse
    {
        public struct WildApricotOauthPermissionsObject
        {
            public int accountId { get; set; }

            public IEnumerable<string> availableScopes { get; set; }
        }

        public string access_token { get; set; }

        public string token_type { get; set; }

        public uint expires_in { get; set; }

        public string refresh_token { get; set; }

        public IEnumerable<WildApricotOauthPermissionsObject> permissions { get; set; }
    }
}
