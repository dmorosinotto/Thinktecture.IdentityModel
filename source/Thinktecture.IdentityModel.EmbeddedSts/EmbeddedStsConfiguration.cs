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

        private static bool UseEmbeddedSTS()
        {
            var config = FederatedAuthentication.FederationConfiguration;
            Uri issuer;
            return
                Uri.TryCreate(config.WsFederationConfiguration.Issuer, UriKind.Absolute, out issuer) &&
                EmbeddedStsConstants.EmbeddedStsIssuerHost.Equals(issuer.Host, StringComparison.OrdinalIgnoreCase);
        }

        private static void ConfigureWIF()
        {
            var inr = new ConfigurationBasedIssuerNameRegistry();
            inr.AddTrustedIssuer(EmbeddedStsConstants.SigningCertificate.Thumbprint,
                                 EmbeddedStsConstants.TokenIssuerName);
            var config = FederatedAuthentication.FederationConfiguration;
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
