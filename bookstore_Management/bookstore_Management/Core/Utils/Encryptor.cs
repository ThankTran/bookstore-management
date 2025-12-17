using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bookstore_Management.Utils
{
    internal class Encryptor
    {
        public static string Hash(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            using (var sha = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(input);
                var hash = sha.ComputeHash(bytes);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }

        public static bool Verify(string input, string hash)
        {
            if (string.IsNullOrEmpty(hash)) return false;
            return string.Equals(Hash(input), hash, StringComparison.OrdinalIgnoreCase);
        }
    }
}
