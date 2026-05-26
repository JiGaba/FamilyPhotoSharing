using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EncryptionLayer.Password
{
    /// <summary>
    /// Password Hash by PBKDF2
    /// </summary>
    public static class PasswordEncryption
    {
        private const int _ITERATION_NUMBER = 100_003;
        public static string HashPassword(string password)
        {
            var salt = GetSalt();
            byte[] hash = GetPBKDF2(password, salt);

            return Convert.ToBase64String(salt) + ":" + Convert.ToBase64String(hash);
        }
        public static bool VerifyPassword(string userPasswordPlain, string storedHash)
        {
            var parts = storedHash.Split(':');
            if (parts.Length != 2) return false;

            byte[] salt = Convert.FromBase64String(parts[0]);
            byte[] storedHashBytes = Convert.FromBase64String(parts[1]);
            byte[] enteredHashBytes = GetPBKDF2(userPasswordPlain, salt);

            return CryptographicOperations.FixedTimeEquals(enteredHashBytes, storedHashBytes);
        }

        private static byte[] GetSalt()
        {
            using var rng = RandomNumberGenerator.Create();
            byte[] salt = new byte[16];
            rng.GetBytes(salt);

            return salt;
        }

        private static byte[] GetPBKDF2(string password, byte[] salt)
        {
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, _ITERATION_NUMBER, HashAlgorithmName.SHA256);
            byte[] hash = pbkdf2.GetBytes(32); // 256bit hash

            return hash;
        }
    }
}
