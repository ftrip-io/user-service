using System;
using System.Security.Cryptography;
using System.Text;

namespace ftrip.io.user_service.Accounts.Utilities
{
    public class PasswordHasher
    {
        public static string Hash(string password, string salt)
        {
            using var sha265Algorithm = SHA256.Create();
            var saltedPassword = password + salt;
            var saltedPasswordBytes = Encoding.Unicode.GetBytes(saltedPassword);
            var hashedSaltedPasswordBytes = sha265Algorithm.ComputeHash(saltedPasswordBytes);

            return Convert.ToBase64String(hashedSaltedPasswordBytes);
        }
    }
}