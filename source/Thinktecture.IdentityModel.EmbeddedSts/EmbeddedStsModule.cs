using System;
using System.Collections.Generic;
using System.IdentityModel.Services;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Thinktecture.IdentityModel.EmbeddedSts
{
    public class EmbeddedStsModule : IHttpModule
    {
        public void Init(HttpApplication app)
        {
            app.BeginRequest += app_BeginRequest;
        }

        void app_BeginRequest(object sender, EventArgs e)
        {
            var req = HttpContext.Current.Request;
            var stsurl =  req.Url.Scheme + "://" + req.Url.Host;
            if ((req.IsSecureConnection && req.Url.Port != 443) || 
                (!req.IsSecureConnection && req.Url.Port != 80))
            {
                stsurl += ":" + req.Url.Port;
            }
            stsurl += req.ApplicationPath;
            if (!stsurl.EndsWith("/")) stsurl += "/";
            stsurl += EmbeddedStsConstants.WsFedPath;

            var fam = FederatedAuthentication.WSFederationAuthenticationModule;
            if (EmbeddedStsConstants.EmbeddedStsIssuer.Equals(fam.Issuer, StringComparison.OrdinalIgnoreCase))
            {
                fam.Issuer = stsurl;
            }
            
            var config = FederatedAuthentication.FederationConfiguration.WsFederationConfiguration;
            if (EmbeddedStsConstants.EmbeddedStsIssuer.Equals(config.Issuer, StringComparison.OrdinalIgnoreCase))
            {
                config.Issuer = stsurl;
            }

            var ctx = HttpContext.Current;
            var stsPath = ctx.Request.ApplicationPath;
            if (!stsPath.EndsWith("/")) stsPath += "/";
            stsPath += EmbeddedStsConstants.WsFedPath;
            if (ctx.Request.Url.AbsolutePath.StartsWith(stsPath))
            {
                ctx.SkipAuthorization = true;
            }
        }

        public void Dispose()
        {
        }
    }
}
