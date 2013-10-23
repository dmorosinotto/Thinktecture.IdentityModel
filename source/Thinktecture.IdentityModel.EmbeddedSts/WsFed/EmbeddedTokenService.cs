/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see LICENSE
 */

using System.IdentityModel;
using System.IdentityModel.Configuration;
using System.IdentityModel.Protocols.WSTrust;
using System.Security.Claims;

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
            var id = new ClaimsIdentity(principal.Claims, "EmbeddedSTS");
            return id;
        }
    }
}
