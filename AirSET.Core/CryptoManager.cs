using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Serialization;

namespace AirSET.Core.Services
{
    public static class CryptoManager
    {
        public static event Action<string> OnCryptoError;

        private static string _secretPasscode;
        public static string SecretPasscode
        {
            get
            {
                if (_secretPasscode == null)
                {
                    string envKey = Environment.GetEnvironmentVariable("AIRSET_SECRET_KEY");
                    if (string.IsNullOrWhiteSpace(envKey))
                    {
                        throw new InvalidOperationException("Environment variable 'AIRSET_SECRET_KEY' wajib diset sebelum aplikasi dijalankan.");
                    }
                    _secretPasscode = envKey.Trim();
                }
                return _secretPasscode;
            }
        }

        private static readonly byte[] StaticSalt = Encoding.UTF8.GetBytes("AirSET_V2_PBKDF2_Static_App_Salt_Amikom_Lab");
        private static byte[] _cachedAesKey;
        private static byte[] _cachedHmacKey;
        private static readonly object _keyLock = new object();

        private static void EnsureKeysDerived()
        {
            if (_cachedAesKey == null || _cachedHmacKey == null)
            {
                lock (_keyLock)
                {
                    if (_cachedAesKey == null || _cachedHmacKey == null)
                    {
                        string pass = SecretPasscode;
                        using (var kdf = new Rfc2898DeriveBytes(pass, StaticSalt, 100000))
                        {
                            byte[] derived = kdf.GetBytes(64);
                            byte[] aesKey = new byte[32];
                            byte[] hmacKey = new byte[32];
                            Array.Copy(derived, 0, aesKey, 0, 32);
                            Array.Copy(derived, 32, hmacKey, 0, 32);
                            _cachedAesKey = aesKey;
                            _cachedHmacKey = hmacKey;
                        }
                    }
                }
            }
        }

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
                EnsureKeysDerived();

                byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                byte[] iv = new byte[16];
                using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
                {
                    rng.GetBytes(iv);
                }

                byte[] cipherBytes;
                using (Aes aes = Aes.Create())
                {
                    aes.Key = _cachedAesKey;
                    aes.IV = iv;
                    using (ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                    {
                        cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
                    }
                }

                // Format: [IV 16 byte] + [Ciphertext N byte] + [HMAC 32 byte]
                byte[] ivAndCipher = new byte[iv.Length + cipherBytes.Length];
                Array.Copy(iv, 0, ivAndCipher, 0, iv.Length);
                Array.Copy(cipherBytes, 0, ivAndCipher, iv.Length, cipherBytes.Length);

                byte[] mac;
                using (var hmac = new HMACSHA256(_cachedHmacKey))
                {
                    mac = hmac.ComputeHash(ivAndCipher);
                }

                byte[] fullPacket = new byte[ivAndCipher.Length + mac.Length];
                Array.Copy(ivAndCipher, 0, fullPacket, 0, ivAndCipher.Length);
                Array.Copy(mac, 0, fullPacket, ivAndCipher.Length, mac.Length);

                return Convert.ToBase64String(fullPacket);
            }
            catch (Exception ex)
            {
                OnCryptoError?.Invoke("Encrypt error: " + ex.Message);
                return string.Empty;
            }
        }

        public static string DecryptString(string base64Cipher)
        {
            try
            {
                if (string.IsNullOrEmpty(base64Cipher)) return string.Empty;

                EnsureKeysDerived();

                byte[] fullPacket = Convert.FromBase64String(base64Cipher);
                // Minimal 16 (IV) + 1 (data) + 32 (HMAC) = 49 bytes
                if (fullPacket.Length < 48)
                {
                    OnCryptoError?.Invoke("Decrypt error: Paket terlalu pendek.");
                    return string.Empty;
                }

                int ivLen = 16;
                int macLen = 32;
                int ivAndCipherLen = fullPacket.Length - macLen;
                int cipherLen = ivAndCipherLen - ivLen;

                byte[] iv = new byte[ivLen];
                Array.Copy(fullPacket, 0, iv, 0, ivLen);

                byte[] cipherBytes = new byte[cipherLen];
                Array.Copy(fullPacket, ivLen, cipherBytes, 0, cipherLen);

                byte[] receivedMac = new byte[macLen];
                Array.Copy(fullPacket, ivAndCipherLen, receivedMac, 0, macLen);

                byte[] computedMac;
                using (var hmac = new HMACSHA256(_cachedHmacKey))
                {
                    computedMac = hmac.ComputeHash(fullPacket, 0, ivAndCipherLen);
                }

                // Verifikasi HMAC dengan perbandingan waktu-konstan (constant-time XOR)
                if (!FixedTimeEquals(receivedMac, computedMac))
                {
                    OnCryptoError?.Invoke("Decrypt error: Verifikasi HMAC gagal (integritas data rusak atau kunci salah).");
                    return string.Empty;
                }

                using (Aes aes = Aes.Create())
                {
                    aes.Key = _cachedAesKey;
                    aes.IV = iv;
                    using (ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                    {
                        byte[] decryptedBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
                        return Encoding.UTF8.GetString(decryptedBytes);
                    }
                }
            }
            catch (Exception ex)
            {
                OnCryptoError?.Invoke("Decrypt exception: " + ex.Message);
                return string.Empty;
            }
        }

        private static bool FixedTimeEquals(byte[] a, byte[] b)
        {
            if (a == null || b == null || a.Length != b.Length) return false;
            int diff = 0;
            for (int i = 0; i < a.Length; i++)
            {
                diff |= a[i] ^ b[i];
            }
            return diff == 0;
        }
    }
}
