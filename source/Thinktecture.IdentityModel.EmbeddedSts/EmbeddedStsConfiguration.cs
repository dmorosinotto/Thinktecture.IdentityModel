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
using System.IO;

[assembly: PreApplicationStartMethod(typeof(Thinktecture.IdentityModel.EmbeddedSts.EmbeddedStsConfiguration), "Start")]

namespace Thinktecture.IdentityModel.EmbeddedSts
{
    public class EmbeddedStsConfiguration
    {
        public static void Start()
        {
            FederatedAuthentication.FederationConfigurationCreated += FederatedAuthentication_FederationConfigurationCreated;
            if (UseEmbeddedSTS())
            {
                ConfigureRoutes();
                ConfigureWIF();
                DynamicModuleUtility.RegisterModule(typeof(EmbeddedStsModule));
                UserManager.WriteClaimsFile();
            }
        }

        internal static bool IsEmbeddedSts(string issuer)
        {
            Uri uri;
            return
                Uri.TryCreate(issuer, UriKind.Absolute, out uri) &&
                EmbeddedStsConstants.EmbeddedStsIssuerHost.Equals(uri.Host, StringComparison.OrdinalIgnoreCase);
        }

        static void FederatedAuthentication_FederationConfigurationCreated(object sender, FederationConfigurationCreatedEventArgs e)
        {
            if (IsEmbeddedSts(e.FederationConfiguration.WsFederationConfiguration.Issuer))
            {
                var inr = new ConfigurationBasedIssuerNameRegistry();
                inr.AddTrustedIssuer(EmbeddedStsConstants.SigningCertificate.Thumbprint,
                                     EmbeddedStsConstants.TokenIssuerName);
                
                var config = e.FederationConfiguration;
                config.IdentityConfiguration.IssuerNameRegistry = inr;

                var rpRealm = new Uri(config.WsFederationConfiguration.Realm);
                if (!config.IdentityConfiguration.AudienceRestriction.AllowedAudienceUris.Contains(rpRealm))
                {
                    config.IdentityConfiguration.AudienceRestriction.AllowedAudienceUris.Add(rpRealm);
                }
                config.IdentityConfiguration.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.None;
                config.IdentityConfiguration.RevocationMode = System.Security.Cryptography.X509Certificates.X509RevocationMode.NoCheck;
            }
        }

        private static bool UseEmbeddedSTS()
        {
            var config = FederatedAuthentication.FederationConfiguration;
            return IsEmbeddedSts(config.WsFederationConfiguration.Issuer);
        }

        private static void ConfigureWIF()
        {
            var inr = new ConfigurationBasedIssuerNameRegistry();
            inr.AddTrustedIssuer(EmbeddedStsConstants.SigningCertificate.Thumbprint,
                                 EmbeddedStsConstants.TokenIssuerName);
            var config = FederatedAuthentication.FederationConfiguration;
            config.IdentityConfiguration.IssuerNameRegistry = inr;

            var rpRealm = new Uri(config.WsFederationConfiguration.Realm);
            if (!config.IdentityConfiguration.AudienceRestriction.AllowedAudienceUris.Contains(rpRealm))
            {
                config.IdentityConfiguration.AudienceRestriction.AllowedAudienceUris.Add(rpRealm);
            }
            config.IdentityConfiguration.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.None;
            config.IdentityConfiguration.RevocationMode = System.Security.Cryptography.X509Certificates.X509RevocationMode.NoCheck;

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
