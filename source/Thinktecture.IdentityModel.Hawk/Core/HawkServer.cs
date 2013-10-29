using System;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Thinktecture.IdentityModel.Hawk.Core.Helpers;
using Thinktecture.IdentityModel.Hawk.Core.MessageContracts;
using Thinktecture.IdentityModel.Hawk.Core.Extensions;

namespace Thinktecture.IdentityModel.Hawk.Core
{
    /// <summary>
    /// Authenticates the incoming request based on the Authorize request header or bewit query string parameter
    /// and sets the Server-Authorization or the WWW-Authenticate response header. HawkServer is for per-request use.
    /// </summary>
    public class HawkServer
    {
        private readonly IRequestMessage request = null;
        private readonly Options options = null;

        private ulong now = 0;
        private AuthenticationResult result = null;
        private bool isBewitRequest = false;

        /// <summary>
        /// Authenticates the incoming request based on the Authorize request header or bewit query string parameter
        /// </summary>
        /// <param name="request">The request object to be authenticated</param>
        /// <param name="options">Hawk authentication options</param>
        public HawkServer(IRequestMessage request, Options options)
        {
            now = DateTime.UtcNow.ToUnixTimeMillis(); // Record time before doing anything else

            if (options == null || options.CredentialsCallback == null)
                throw new ArgumentNullException("Invalid Hawk authentication options. Credentials callback cannot be null.");

            this.request = request;
            this.options = options;
        }

        /// <summary>
        /// Returns a ClaimsPrincipal object with the NameIdentifier and Name claims, if the request can be
        /// successfully authenticated based on query string parameter bewit or HTTP Authorization header (hawk scheme).
        /// </summary>
        public async Task<ClaimsPrincipal> AuthenticateAsync()
        {
            string bewit;
            bool isBewit = Bewit.TryGetBewit(this.request, out bewit);

            if (isBewit)
                Tracing.Information("Bewit Found");

            this.result = isBewit ?
                                Bewit.Authenticate(bewit, now, request, options) :
                                    await HawkSchemeHeader.AuthenticateAsync(now, request, options);

            if (result.IsAuthentic)
            {
                // At this point, authentication is successful but make sure the request parts match what is in the
                // application specific data 'ext' parameter by invoking the callback passing in the request object and 'ext'.
                // The application specific data is considered verified, if the callback is not set or it returns true.
                bool isAppSpecificDataVerified = options.VerificationCallback == null ||
                                                    options.VerificationCallback(request, result.ApplicationSpecificData);

                if (isAppSpecificDataVerified)
                {
                    // Set the flag so that Server-Authorization header is not sent for bewit requests.
                    this.isBewitRequest = isBewit;

                    var idClaim = new Claim(ClaimTypes.NameIdentifier, result.Credential.Id);
                    var nameClaim = new Claim(ClaimTypes.Name, result.Credential.User);

                    var identity = new ClaimsIdentity(new[] { idClaim, nameClaim }, HawkConstants.Scheme);

                    return new ClaimsPrincipal(identity);
                }
                else
                    Tracing.Information("Invalid Application Specific Data, though authentication is successful.");
            }

            return Principal.Anonymous;
        }

        /// <summary>
        /// Returns the name of the response header (WWW-Authenticate or Server-Authorization) and the corresponding
        /// value, respectively for an unauthorized and a successful request.
        /// </summary>
        public async Task<Tuple<string, string>> CreateServerAuthorizationAsync(IResponseMessage response)
        {
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                string challenge = String.Format(" {0}", request.ChallengeParameter ?? String.Empty);
                string headerValue = HawkConstants.Scheme + challenge.TrimEnd(' ');

                return new Tuple<string, string>(HawkConstants.WwwAuthenticateHeaderName, headerValue);
            }
            else
            {
                // No Server-Authorization header for the following:
                // (1) There is no result or failed authentication.
                // (2) The credential is a bewit.
                // (3) The server is configured to not send the header.
                bool createHeader = this.result != null
                                        && this.result.IsAuthentic
                                            && (!this.isBewitRequest)
                                                && options.EnableServerAuthorization;

                if (createHeader)
                {
                    if (options.NormalizationCallback != null)
                        this.result.Artifacts.ApplicationSpecificData = options.NormalizationCallback(response);

                    // Sign the response
                    var normalizedRequest = new NormalizedRequest(request, this.result.Artifacts);
                    var crypto = new Cryptographer(normalizedRequest, this.result.Artifacts, this.result.Credential);

                    // Response body is needed only if payload hash must be included in the response MAC.
                    string body = null;
                    if (options.ResponsePayloadHashabilityCallback != null && 
                            options.ResponsePayloadHashabilityCallback(this.request))
                    {
                        body = await response.ReadBodyAsStringAsync();
                    }

                    crypto.Sign(body, response.ContentType);

                    string authorization = this.result.Artifacts.ToServerAuthorizationHeaderParameter();

                    if (!String.IsNullOrWhiteSpace(authorization))
                    {
                        return new Tuple<string, string>(HawkConstants.ServerAuthorizationHeaderName,
                            String.Format("{0} {1}", HawkConstants.Scheme, authorization));
                    }
                }
            }

            return null;
        }
    }
}