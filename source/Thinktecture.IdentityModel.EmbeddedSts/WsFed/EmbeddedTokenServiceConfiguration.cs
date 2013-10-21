using System;
using System.Collections.Generic;
using System.IdentityModel;
using System.IdentityModel.Configuration;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Thinktecture.IdentityModel;

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
