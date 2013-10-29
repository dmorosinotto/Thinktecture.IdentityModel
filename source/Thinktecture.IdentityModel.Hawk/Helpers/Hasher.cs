using System;
using System.Security.Cryptography;
using Thinktecture.IdentityModel.Hawk.Core.Extensions;

namespace Thinktecture.IdentityModel.Hawk.Core.Helpers
{
    /// <summary>
    /// The class responsible for creating and validating hash and HMAC.
    /// </summary>
    internal class Hasher
    {
        private readonly string algorithmName;

        /// <summary>
        /// The class responsible for creating and validating hash and HMAC.
        /// </summary>
        /// <param name="algorithmToUse">The hashing algorithm to use</param>
        internal Hasher(SupportedAlgorithms algorithmToUse)
        {
            if (!Enum.IsDefined(typeof(SupportedAlgorithms), algorithmToUse))
                throw new ArgumentException("Invalid algorithm");

            this.algorithmName = algorithmToUse.ToString();
        }

        /// <summary>
        /// Computes the hash value for the specified data.
        /// </summary>
        internal byte[] ComputeHash(byte[] data)
        {
            if(data == null || data.Length == 0)
                throw new ArgumentException("Invalid data to hash");

            using (var hashAlgorithm = HashAlgorithm.Create(this.algorithmName))
            {
                return hashAlgorithm.ComputeHash(data);
            }
        }

        /// <summary>
        /// Returns true, if the computed hash value for the specified data matches the specified hash.
        /// Matching is done constant-time to prevent time-based analysis.
        /// </summary>
        internal bool IsValidHash(byte[] data, byte[] incomingHash)
        {
            byte[] ComputedHash = this.ComputeHash(data);

            return ComputedHash.IsConstantTimeEqualTo(incomingHash);
        }

        /// <summary>
        /// Computes the keyed hash value (HMAC) for the specified data.
        /// </summary>
        internal byte[] ComputeHmac(byte[] data, string key)
        {
            if (data == null || data.Length == 0)
                throw new ArgumentException("Invalid data to hash");

            if(String.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Invalid key");

            using (var algorithm = KeyedHashAlgorithm.Create("hmac" + this.algorithmName))
            {
                algorithm.Key = key.ToBytesFromUtf8();
                return algorithm.ComputeHash(data);
            }
        }

        /// <summary>
        /// Returns true, if the computed HMAC for the specified data matches the specified HMAC.
        /// Matching is done constant-time to prevent time-based analysis.
        /// </summary>
        internal bool IsValidMac(byte[] data, string key, byte[] incomingMac)
        {
            byte[] computedMac = this.ComputeHmac(data, key);

            return computedMac.IsConstantTimeEqualTo(incomingMac);
        }
    }
}
