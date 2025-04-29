using System.Security.Cryptography;
using System.Text;

namespace Models.Handlers
{
    public static class EncryptionHandler
    {
        private static readonly byte[] Key = Encoding.UTF8.GetBytes(Keys.Key32);
        private static readonly byte[] IV = Encoding.UTF8.GetBytes(Keys.IV16);

        public static string Encrypt(string plainText)
        {
            string encrypted = "";

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using MemoryStream msEncrypt = new();
                using (CryptoStream csEncrypt = new(msEncrypt, encryptor, CryptoStreamMode.Write))
                using (StreamWriter swEncrypt = new(csEncrypt))
                {
                    swEncrypt.Write(plainText);
                }
                encrypted = Convert.ToBase64String(msEncrypt.ToArray());
            }

            return encrypted;
        }

        public static string Decrypt(string cipherText)
        {
            try
            {
                byte[] cipherBytes = Convert.FromBase64String(cipherText);

                using Aes aesAlg = Aes.Create();
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using MemoryStream msDecrypt = new(cipherBytes);
                using CryptoStream csDecrypt = new(msDecrypt, decryptor, CryptoStreamMode.Read);
                using StreamReader srDecrypt = new(csDecrypt);
                string plainText = srDecrypt.ReadToEnd();
                return plainText;
            }
            catch (Exception /*ex*/) { throw; }
        }
    }
}
