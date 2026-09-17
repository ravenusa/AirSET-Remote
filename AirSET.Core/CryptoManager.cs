using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Serialization;

namespace AirSET.Core.Services
{
    public static class CryptoManager
    {
        private static readonly string SecretPasscode = Environment.GetEnvironmentVariable("AIRSET_SECRET_KEY") 
            ?? "AirSET_Default_Development_Key_Change_In_Production_3623";

        public static string Encrypt<T>(T obj)
        {
            var serializer = new JavaScriptSerializer { MaxJsonLength = int.MaxValue };
            string json = serializer.Serialize(obj);
            return EncryptString(json);
        }

        public static T Decrypt<T>(string cipherText)
        {
            string json = DecryptString(cipherText);
            if (string.IsNullOrEmpty(json)) return default(T);

            var serializer = new JavaScriptSerializer { MaxJsonLength = int.MaxValue };
            return serializer.Deserialize<T>(json);
        }

        public static string EncryptString(string plainText)
        {
            try
            {
                byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                byte[] key = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(SecretPasscode));

                byte[] iv = new byte[16];
                using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
                {
                    rng.GetBytes(iv);
                }

                using (Aes aes = Aes.Create())
                {
                    aes.Key = key;
                    aes.IV = iv;
                    using (ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                    {
                        byte[] cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
                        byte[] result = new byte[iv.Length + cipherBytes.Length];
                        Array.Copy(iv, 0, result, 0, iv.Length);
                        Array.Copy(cipherBytes, 0, result, iv.Length, cipherBytes.Length);

                        return Convert.ToBase64String(result);
                    }
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        public static string DecryptString(string base64Cipher)
        {
            try
            {
                byte[] fullCipher = Convert.FromBase64String(base64Cipher);
                byte[] key = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(SecretPasscode));

                byte[] iv = new byte[16];
                byte[] cipherBytes = new byte[fullCipher.Length - 16];

                Array.Copy(fullCipher, 0, iv, 0, 16);
                Array.Copy(fullCipher, 16, cipherBytes, 0, cipherBytes.Length);

                using (Aes aes = Aes.Create())
                {
                    aes.Key = key;
                    aes.IV = iv;
                    using (ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                    {
                        byte[] decryptedBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
                        return Encoding.UTF8.GetString(decryptedBytes);
                    }
                }
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
