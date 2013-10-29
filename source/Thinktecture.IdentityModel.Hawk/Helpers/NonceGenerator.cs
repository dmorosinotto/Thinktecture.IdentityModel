using System;
using System.Linq;

namespace Thinktecture.IdentityModel.Hawk.Core.Helpers
{
    /// <summary>
    /// Generates a nonce to be used by a .NET client.
    /// </summary>
    public class NonceGenerator
    {
        /// <summary>
        /// Generates a nonce matching the specified length and returns the same. Default length is 6.
        /// </summary>
        public static string Generate(int length = 6)
        {
            string alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            int min = 0;
            int max = alphabet.Length - 1;

            var random = new Random();

            char[] randomCharacters = Enumerable.Range(0, length)
                                            .Select(i => alphabet[random.Next(min, max)])
                                                .ToArray();
            return new String(randomCharacters);
        }
    }
}
