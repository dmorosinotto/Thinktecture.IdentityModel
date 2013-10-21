using System;
using System.Collections.Generic;
using System.IdentityModel;
using System.IdentityModel.Configuration;
using System.IdentityModel.Protocols.WSTrust;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Thinktecture.IdentityModel.EmbeddedSts.WsFed
{
    class EmbeddedTokenService : SecurityTokenService
    {
        public EmbeddedTokenService(SecurityTokenServiceConfiguration config)
            : base(config)
        {
        }

        protected override Scope GetScope(ClaimsPrincipal principal, RequestSecurityToken request)
        {
            return new Scope(
                request.AppliesTo.Uri.AbsoluteUri,
                this.SecurityTokenServiceConfiguration.SigningCredentials)
                {
                    ReplyToAddress = request.ReplyTo,
                    TokenEncryptionRequired = false
                };
        }
        
        protected override ClaimsIdentity GetOutputClaimsIdentity(ClaimsPrincipal principal, RequestSecurityToken request, Scope scope)
        {
            return CreateClaimsIdentity(principal.Claims);
        }

        public static ClaimsIdentity CreateClaimsIdentity(IEnumerable<Claim> claims)
        {
            var id = new ClaimsIdentity(claims, "EmbeddedSTS");
            var nameClaim = claims.FirstOrDefault(x=>x.Type==ClaimTypes.Name);
            if (nameClaim != null)
            {
                id.AddClaim(new Claim(ClaimTypes.NameIdentifier, nameClaim.Value));
            }
            id.AddClaim(new Claim(ClaimTypes.AuthenticationInstant, DateTime.UtcNow.ToString("o")));
            id.AddClaim(new Claim(ClaimTypes.AuthenticationMethod, AuthenticationTypes.Password));
            return id;
        }
    }
}
