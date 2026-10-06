using System;
using System.Security.Cryptography;
using System.Text;

namespace FlowState.Server
{
    internal static class ServerCryptography
    {
        internal static string RandomHex(int count)
        {
            if (count < 1 || count > 1024) throw new ArgumentOutOfRangeException(nameof(count));
            return Convert.ToHexString(RandomNumberGenerator.GetBytes(count)).ToLowerInvariant();
        }

        internal static string HmacHex(string keyHex, string message)
        {
            byte[] key = Convert.FromHexString(keyHex);
            try
            {
                return Convert.ToHexString(HMACSHA256.HashData(key, Encoding.UTF8.GetBytes(message))).ToLowerInvariant();
            }
            finally { CryptographicOperations.ZeroMemory(key); }
        }

        internal static bool FixedEquals(string leftHex, string rightHex)
        {
            return CryptographicOperations.FixedTimeEquals(Convert.FromHexString(leftHex), Convert.FromHexString(rightHex));
        }

        internal static string ConvertEncoding(string value, string from, string to)
        {
            byte[] bytes;
            if (from == "hex") bytes = Convert.FromHexString(value);
            else if (from == "base64") bytes = Convert.FromBase64String(value);
            else throw new ArgumentException("Unsupported encoding.");
            if (to == "hex") return Convert.ToHexString(bytes).ToLowerInvariant();
            if (to == "base64") return Convert.ToBase64String(bytes);
            throw new ArgumentException("Unsupported encoding.");
        }
    }
}
