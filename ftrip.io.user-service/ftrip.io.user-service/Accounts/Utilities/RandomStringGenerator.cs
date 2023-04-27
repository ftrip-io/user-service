using System;
using System.Security.Cryptography;

namespace ftrip.io.user_service.Accounts.Utilities
{
    public class RandomStringGenerator
    {
        public static string Generate(int length)
        {
            using var randomGenerator = RandomNumberGenerator.Create();
            var saltBytes = new byte[length];
            randomGenerator.GetBytes(saltBytes);

            return Convert.ToBase64String(saltBytes);
        }
    }
}