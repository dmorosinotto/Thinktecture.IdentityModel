using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Thinktecture.IdentityModel.EmbeddedSts.Assets;

namespace Thinktecture.IdentityModel.EmbeddedSts
{
    public class EmbeddedStsConstants
    {
        public const string EmbeddedStsIssuerHost = "EmbeddedSts";
        public const string TokenIssuerName = "urn:Thinktecture:EmbeddedSTS";

        internal const int SamlTokenLifetime = 60 * 10;

        internal const string SigningCertificateFile = "EmbeddedSigningCert.pfx";
        internal const string SigningCertificatePassword = "password";

        internal const string SignInFile = "SignIn.html";
        internal const string SignOutFile = "SignOut.html";

        internal const string WsFedPath = "_sts";

        public static X509Certificate2 SigningCertificate
        {
            get
            {
                return new X509Certificate2(AssetManager.LoadBytes(SigningCertificateFile), SigningCertificatePassword);
            }
        }
    }
}
