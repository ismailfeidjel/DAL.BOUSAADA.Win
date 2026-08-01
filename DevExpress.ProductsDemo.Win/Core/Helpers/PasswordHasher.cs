using System;
using System.Security.Cryptography;

namespace DevExpress.ProductsDemo.Win.Core.Helpers
{
    public static class PasswordHasher
    {
        private const int SaltSize = 16;
        private const int HashSize = 32;
        private const int Iterations = 100_000;

        public static void CreateHash(string password, out string hash, out string salt)
        {
            byte[] saltBytes = new byte[SaltSize];
            using (var rng = RandomNumberGenerator.Create())
                rng.GetBytes(saltBytes);

            byte[] hashBytes = Pbkdf2(password, saltBytes);

            hash = Convert.ToBase64String(hashBytes);
            salt = Convert.ToBase64String(saltBytes);
        }

        public static bool Verify(string password, string storedHash, string storedSalt)
        {
            byte[] saltBytes = Convert.FromBase64String(storedSalt);
            byte[] hashBytes = Pbkdf2(password, saltBytes);
            string computedHash = Convert.ToBase64String(hashBytes);

            return SlowEquals(computedHash, storedHash);
        }

        private static byte[] Pbkdf2(string password, byte[] salt)
        {
            using (var deriveBytes = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256))
            {
                return deriveBytes.GetBytes(HashSize);
            }
        }

        // Constant-time comparison — avoids timing attacks on hash comparison
        private static bool SlowEquals(string a, string b)
        {
            if (a.Length != b.Length) return false;

            int diff = 0;
            for (int i = 0; i < a.Length; i++)
                diff |= a[i] ^ b[i];

            return diff == 0;
        }
    }
}