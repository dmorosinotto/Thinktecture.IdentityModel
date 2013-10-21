using System;
using System.Collections.Generic;
using System.IdentityModel.Services;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Script.Serialization;
using Thinktecture.IdentityModel.EmbeddedSts.Assets;

namespace Thinktecture.IdentityModel.EmbeddedSts.WsFed
{
    public class EmbeddedStsController : Controller
    {
        ContentResult Html(string html)
        {
            return Content(html, "text/html");
        }

        public ActionResult Index()
        {
            var message = WSFederationMessage.CreateFromUri(Request.Url);
            var signInMsg = message as SignInRequestMessage;
            if (signInMsg != null)
            {
                var user = GetUser();
                if (user != null)
                {
                    return ProcessSignIn(signInMsg, user);
                }
                else
                {
                    return ShowUserList();
                }
            }

            var signOutMsg = message as SignOutRequestMessage;
            if (signOutMsg != null)
            {
                return ProcessSignOut(signOutMsg);
            }

            return new EmptyResult();
        }

        private ActionResult ShowUserList()
        {
            var html = AssetManager.LoadString(EmbeddedStsConstants.SignInFile);

            var users = UserManager.GetAllUserNames();
            var ser = new JavaScriptSerializer();
            var json = ser.Serialize(users.ToArray());
            html = html.Replace("{userArray}", json);

            var url = Request.Url.PathAndQuery;
            html = html.Replace("{signInUrl}", url);
            
            return Html(html);
        }

        private ClaimsPrincipal GetUser()
        {
            var username = Request.Form["username"];
            if (String.IsNullOrWhiteSpace(username)) return null;

            var claims = UserManager.GetClaimsForUser(username);
            if (claims == null || !claims.Any()) return null;
            
            var id = new ClaimsIdentity(claims);
            return new ClaimsPrincipal(id);
        }

        private ActionResult ProcessSignIn(SignInRequestMessage signInMsg, ClaimsPrincipal user)
        {
            var config = new EmbeddedTokenServiceConfiguration();
            var sts = config.CreateSecurityTokenService();

            var appPath = Request.ApplicationPath;
            if (!appPath.EndsWith("/")) appPath += "/";

            signInMsg.Reply = new Uri(Request.Url, appPath).AbsoluteUri;
            var response = FederatedPassiveSecurityTokenServiceOperations.ProcessSignInRequest(signInMsg, user, sts);

            var body = response.WriteFormPost();
            return Html(body);
        }

        private ActionResult ProcessSignOut(SignOutRequestMessage signOutMsg)
        {
            var appPath = Request.ApplicationPath;
            if (!appPath.EndsWith("/")) appPath += "/";

            var rpUrl = new Uri(Request.Url, appPath);
            var signOutCleanupMsg = new SignOutCleanupRequestMessage(rpUrl);
            var signOutUrl = signOutCleanupMsg.WriteQueryString();

            var html = AssetManager.LoadString(EmbeddedStsConstants.SignOutFile);
            html = html.Replace("{redirectUrl}", signOutMsg.Reply);
            html = html.Replace("{signOutUrl}", signOutUrl);

            return Html(html);
        }
    }
}
