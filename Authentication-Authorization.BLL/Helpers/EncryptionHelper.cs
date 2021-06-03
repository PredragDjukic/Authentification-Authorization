using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Authentication_Authorization.BLL.Helpers
{
    public static class EncryptionHelper
    {
        public static string Encrypt(string value, string userSecretId, string secretKey)
        {
            byte[] bytesBuff = Encoding.Unicode.GetBytes(value);
            byte[] salt = Encoding.Unicode.GetBytes(secretKey);

            using (Aes aes = Aes.Create())
            {
                Rfc2898DeriveBytes crypto = new Rfc2898DeriveBytes(
                    userSecretId,
                    salt
                );

                aes.Key = crypto.GetBytes(32);
                aes.IV = crypto.GetBytes(16);

                using (MemoryStream mStream = new MemoryStream())
                {
                    using (CryptoStream cStream = 
                        new CryptoStream(mStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cStream.Write(bytesBuff, 0, bytesBuff.Length);
                        cStream.Close();
                    }
                    value = Convert.ToBase64String(mStream.ToArray());
                }
            }

            return value;
        }

        public static string Decrypt(string value, string userSecretId, string secretKey)
        {
            value = value.Replace(" ", "+");
            byte[] bytesBuff = Convert.FromBase64String(value);
            byte[] salt = Encoding.Unicode.GetBytes(secretKey);

            using (Aes aes = Aes.Create())
            {
                Rfc2898DeriveBytes crypto = new Rfc2898DeriveBytes(
                        userSecretId, 
                        salt
                    );
                aes.Key = crypto.GetBytes(32);
                aes.IV = crypto.GetBytes(16);

                using (MemoryStream mStream = new MemoryStream())
                {
                    using (CryptoStream cStream = new CryptoStream(mStream, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cStream.Write(bytesBuff, 0, bytesBuff.Length);
                        cStream.Close();
                    }
                    value = Encoding.Unicode.GetString(mStream.ToArray());
                }
            }

            return value;
        }
    }
}
