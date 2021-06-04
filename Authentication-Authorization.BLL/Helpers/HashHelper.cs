using Authentication_Authorization.BLL.Exceptions;
using System;
using System.Security.Cryptography;

namespace Authentication_Authorization.BLL.Helpers
{
    public static class HashHelper
    {
        private const int saltSize = 16;
        private const int hashSize = 20;

        public static string Hash(string value)
        {
            return Hash(value, 1000);
        }

        public static string Hash(string value, int iterations)
        {
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[saltSize]);

            var pbkdf2 = new Rfc2898DeriveBytes(value, salt, iterations);
            byte[] hash = pbkdf2.GetBytes(hashSize);

            byte[] hashBytes = new byte[saltSize + hashSize];
            Array.Copy(salt, 0, hashBytes, 0, saltSize);
            Array.Copy(hash, 0, hashBytes, saltSize, hashSize);

            string base64Hash = Convert.ToBase64String(hashBytes);

            return base64Hash;
        }

        public static void VerifyValue(string value, string hashedValue)
        {
            byte[] hashBytes = Convert.FromBase64String(hashedValue);
            byte[] salt = new byte[saltSize];
            Array.Copy(hashBytes, 0, salt, 0, 16);
            var pbkdf2 = new Rfc2898DeriveBytes(value, salt, 1000);
            byte[] hash = pbkdf2.GetBytes(hashSize);

            for (int i = 0; i < hashSize; i++)
            {
                if (hashBytes[i + saltSize] != hash[i])
                {
                    throw new BussinesException("Value incorrect", 400);
                }
            }
        }
    }
}
