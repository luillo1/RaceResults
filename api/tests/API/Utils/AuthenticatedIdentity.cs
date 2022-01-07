using System.Security.Claims;

namespace Internal.Api.Utils
{
    public class AuthenticatedIdentity : ClaimsIdentity
    {
        public override bool IsAuthenticated => true;
    }
}