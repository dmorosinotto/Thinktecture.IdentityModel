/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see LICENSE
 */

using System;
using System.IdentityModel.Configuration;
using System.IdentityModel.Tokens;

namespace Thinktecture.IdentityModel.EmbeddedSts.WsFed
{
    class EmbeddedTokenServiceConfiguration : SecurityTokenServiceConfiguration
    {
        public EmbeddedTokenServiceConfiguration()
            : base(false)
        {
            this.SecurityTokenService = typeof(EmbeddedTokenService);
            this.TokenIssuerName = EmbeddedStsConstants.TokenIssuerName;
            this.SigningCredentials = new X509SigningCredentials(EmbeddedStsConstants.SigningCertificate);
            this.DefaultTokenLifetime = TimeSpan.FromMinutes(EmbeddedStsConstants.SamlTokenLifetime);
        }
    }
}
