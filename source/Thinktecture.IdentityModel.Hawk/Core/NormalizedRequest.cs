using System;
using System.Text;
using System.Text.RegularExpressions;
using Thinktecture.IdentityModel.Hawk.Core.Helpers;
using Thinktecture.IdentityModel.Hawk.Core.MessageContracts;
using Thinktecture.IdentityModel.Hawk.Core.Extensions;

namespace Thinktecture.IdentityModel.Hawk.Core
{
    /// <summary>
    /// Represents the normalized request, in the following format.
    /// hawk.1.header\n
    /// timestamp\n
    /// nonce\n
    /// HTTP method\n
    /// uri path and query string\n
    /// host name\n
    /// port\n
    /// payload hash\n
    /// application specific data\n
    /// </summary>
    internal class NormalizedRequest
    {
        private const string REQUEST_PREAMBLE = HawkConstants.Scheme + "." + HawkConstants.Version + ".header"; // hawk.1.header
        private const string BEWIT_PREAMBLE = HawkConstants.Scheme + "." + HawkConstants.Version + ".bewit"; // hawk.1.bewit

        private const string HTTP_PORT = "80";
        private const string HTTPS_PORT = "443";
        private const string MATCH_PATTERN_HOSTNAME_OR_IPV4 = @"^(?:(?:\r\n)?\s)*([^:]+)(?::(\d+))?(?:(?:\r\n)?\s)*$";
        private const string MATCH_PATTERN_IPV6 = @"^(?:(?:\r\n)?\s)*(\[[^\]]+\])(?::(\d+))?(?:(?:\r\n)?\s)*$";
        private const string XFF_HEADER_NAME = "X-Forwarded-For";

        private readonly ArtifactsContainer artifacts = null;
        
        private readonly string method = null;
        private readonly string path = null;
        private readonly string hostName = null;
        private readonly string port = null;

        internal NormalizedRequest(IRequestMessage request, ArtifactsContainer artifacts)
        {
            this.artifacts = artifacts;

            // Determine host and port - take the host name from X-Forwarded-For header, if present, or from
            // the Host header, if present, or from the HttpRequestMessage object. For bewit, it is always from URI.
            string firstPreference = IsBewit? null : request.ForwardedFor;
            string secondPreference = IsBewit ? null : request.Host;

            this.hostName = this.GetHostName(firstPreference, out this.port) ??
                                this.GetHostName(secondPreference, out this.port) ??
                                    request.Uri.Host;

            if (String.IsNullOrWhiteSpace(this.port))
                this.port = request.Uri.Port.ToString();

            this.method = request.Method.Method.ToUpper();
            this.path = request.Uri.PathAndQuery;
        }

        /// <summary>
        /// Set to true, if this instance is for a bewit.
        /// </summary>
        internal bool IsBewit { get; set; }

        /// <summary>
        /// Returns the normalized request string.
        /// </summary>
        public override string ToString()
        {
            StringBuilder result = new StringBuilder();
            result
                .AppendNewLine(this.IsBewit ? BEWIT_PREAMBLE : REQUEST_PREAMBLE)
                .AppendNewLine(artifacts.Timestamp.ToString())
                .AppendNewLine(artifacts.Nonce)
                .AppendNewLine(this.method)
                .AppendNewLine(this.path)
                .AppendNewLine(this.hostName)
                .AppendNewLine(this.port)
                .AppendNewLine(artifacts.PayloadHash == null ? null : artifacts.PayloadHash.ToBase64String())
                .AppendNewLine(artifacts.ApplicationSpecificData);

            return result.ToString();
        }

        /// <summary>
        /// Returns the normalized request bytes.
        /// </summary>
        internal byte[] ToBytes()
        {
            return this.ToString().ToBytesFromUtf8();
        }

        private string GetHostName(string hostHeader, out string port)
        {
            if (!String.IsNullOrWhiteSpace(hostHeader))
            {
                string pattern = hostHeader[0] == '[' ? MATCH_PATTERN_IPV6 : MATCH_PATTERN_HOSTNAME_OR_IPV4;
                var match = Regex.Match(hostHeader, pattern);

                if (match.Success && match.Groups.Count == 3)
                {
                    string hostName = match.Groups[1].Value;

                    if (!String.IsNullOrWhiteSpace(hostName))
                    {
                        port = match.Groups[2].Value;
                        return hostName;
                    }
                }
            }

            port = null;
            return null;
        }
    }
}
