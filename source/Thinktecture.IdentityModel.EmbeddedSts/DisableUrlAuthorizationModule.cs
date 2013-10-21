using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Thinktecture.IdentityModel.EmbeddedSts
{
    public class DisableUrlAuthorizationModule : IHttpModule
    {
        public static void Configure()
        {
            DynamicModuleUtility.RegisterModule(typeof(DisableUrlAuthorizationModule));
        }

        public void Init(HttpApplication app)
        {
            app.BeginRequest += app_BeginRequest;
        }

        void app_BeginRequest(object sender, EventArgs e)
        {
            var ctx = HttpContext.Current;
            var stsPath = ctx.Request.ApplicationPath;
            if (!stsPath.EndsWith("/")) stsPath += "/";
            stsPath += EmbeddedStsConstants.PathPrefix;
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
