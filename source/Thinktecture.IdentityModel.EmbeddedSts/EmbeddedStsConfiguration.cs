using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using System;
using System.Collections.Generic;
using System.IdentityModel.Services;
using System.IdentityModel.Services.Configuration;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;
using System.Web.Mvc;
using System.Web.Hosting;

[assembly: PreApplicationStartMethod(typeof(Thinktecture.IdentityModel.EmbeddedSts.EmbeddedStsConfiguration), "Start")]

namespace Thinktecture.IdentityModel.EmbeddedSts
{
    public class EmbeddedStsConfiguration
    {
        public static void Start()
        {
            if (UseEmbeddedSTS())
            {
                ConfigureRoutes();
                ConfigureWIF();
                DynamicModuleUtility.RegisterModule(typeof(EmbeddedStsModule));
            }
        }

        private static string GetStsUrl()
        {
            var config = FederatedAuthentication.FederationConfiguration.WsFederationConfiguration;
            var req = HttpContext.Current.Request;

            var stsurl = config.RequireHttps ? "https://" : "http://";
            stsurl += req.Url.Host;
            if ((req.IsSecureConnection && req.Url.Port != 443) ||
                (!req.IsSecureConnection && req.Url.Port != 80))
            {
                stsurl += ":" + req.Url.Port;
            }
            stsurl += req.ApplicationPath;
            if (!stsurl.EndsWith("/")) stsurl += "/";
            stsurl += EmbeddedStsConstants.WsFedPath;

            return stsurl;
        }

        private static bool UseEmbeddedSTS()
        {
            var config = FederatedAuthentication.FederationConfiguration;
            return EmbeddedStsConstants.EmbeddedStsIssuer.Equals(config.WsFederationConfiguration.Issuer, StringComparison.OrdinalIgnoreCase);
        }

        private static void ConfigureWIF()
        {
            var config = FederatedAuthentication.FederationConfiguration;
            //config.WsFederationConfiguration.Issuer = GetStsUrl();
            
            var inr = new ConfigurationBasedIssuerNameRegistry();
            inr.AddTrustedIssuer(EmbeddedStsConstants.SigningCertificate.Thumbprint,
                                 EmbeddedStsConstants.TokenIssuerName);
            config.IdentityConfiguration.IssuerNameRegistry = inr;
        }

        private static void ConfigureRoutes()
        {
            var routes = RouteTable.Routes;

            routes.MapRoute(
                "EmbeddedSts-WsFed",
                EmbeddedStsConstants.WsFedPath,
                new { controller = "EmbeddedSts", action = "Index" },
                null,
                new string[] { "Thinktecture.IdentityModel.EmbeddedSts" });
        }
    }
}
