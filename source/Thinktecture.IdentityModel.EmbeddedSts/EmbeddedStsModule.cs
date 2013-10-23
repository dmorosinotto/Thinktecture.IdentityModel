/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see LICENSE
 */

using System;
using System.IdentityModel.Services;
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
            ConfigureFam();

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

        void ConfigureFam()
        {
            var fam = FederatedAuthentication.WSFederationAuthenticationModule;
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

            config.Issuer = fam.Issuer = stsurl;
        }
    }
}
