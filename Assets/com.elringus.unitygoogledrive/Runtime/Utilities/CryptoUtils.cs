using System;
using System.Security.Cryptography;
using System.Text;

namespace UnityGoogleDrive
{
    public static class CryptoUtils
    {
        /// <summary>
        /// Returns the SHA256 hash of the input string.
        /// </summary>
        public static byte[] Sha256 (string inputString)
        {
            var bytes = Encoding.ASCII.GetBytes(inputString);
            var sha256 = new SHA256Managed();
            return sha256.ComputeHash(bytes);
        }

        /// <summary>
        /// Returns URI-safe data with a given input length.
        /// </summary>
        /// <param name="length">Input length (nb. output will be longer).</param>
        public static string RandomDataBase64Uri (uint length)
        {
            var cryptoProvider = new RNGCryptoServiceProvider();
            var bytes = new byte[length];
            cryptoProvider.GetBytes(bytes);
            return Base64UriEncodeNoPadding(bytes);
        }

        /// <summary>
        /// Base64Uri no-padding encodes the given input buffer.
        /// </summary>
        public static string Base64UriEncodeNoPadding (byte[] buffer)
        {
            var base64 = Convert.ToBase64String(buffer);

            // Converts base64 to Base64Uri.
            base64 = base64.Replace("+", "-");
            base64 = base64.Replace("/", "_");
            // Strips padding.
            base64 = base64.Replace("=", "");

            return base64;
        }
    }
}
