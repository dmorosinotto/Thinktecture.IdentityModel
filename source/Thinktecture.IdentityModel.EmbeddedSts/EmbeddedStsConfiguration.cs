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

namespace Thinktecture.IdentityModel.EmbeddedSts
{
    class EmbeddedStsConfiguration
    {
        public static void Start()
        {
            if (UseEmbeddedSTS())
            {
                ConfigureRoutes();
                ConfigureWIF();
                DisableUrlAuthorizationModule.Configure();
            }
        }

        private static bool UseEmbeddedSTS()
        {
            var appPath = HostingEnvironment.ApplicationVirtualPath;
            if (!appPath.EndsWith("/")) appPath += "/";
            appPath += EmbeddedStsConstants.WsFedPath;

            var config = FederatedAuthentication.FederationConfiguration;
            return config.WsFederationConfiguration.Issuer.EndsWith(appPath);
        }

        private static void ConfigureWIF()
        {
            var config = FederatedAuthentication.FederationConfiguration;
            
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
