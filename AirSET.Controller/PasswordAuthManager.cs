using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace AirSET.Controller
{
    public static class PasswordAuthManager
    {
        private static readonly string ConfigDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "AirSET_Controller"
        );
        private static readonly string PasswordFile = Path.Combine(ConfigDir, "auth.dat");

        // Catatan Arsitektur: Menghapus file auth.dat akan mereset Controller ke alur first-run.
        // Pengamanan sesungguhnya bergantung pada proteksi akses fisik dan hak Administrator pada komputer Controller.
        public static bool IsPasswordSet()
        {
            try
            {
                return File.Exists(PasswordFile) && new FileInfo(PasswordFile).Length > 0;
            }
            catch
            {
                return false;
            }
        }

        public static void SetPassword(string newPassword)
        {
            if (!Directory.Exists(ConfigDir))
            {
                Directory.CreateDirectory(ConfigDir);
            }

            byte[] salt = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }

            int iterations = 100000;
            byte[] hash;
            using (var pbkdf2 = new Rfc2898DeriveBytes(newPassword, salt, iterations))
            {
                hash = pbkdf2.GetBytes(32);
            }

            // Format v2: v2:<iterations>:<salt-base64>:<hash-base64>
            string record = string.Format("v2:{0}:{1}:{2}", iterations, Convert.ToBase64String(salt), Convert.ToBase64String(hash));
            File.WriteAllText(PasswordFile, record, Encoding.UTF8);
        }

        public static bool VerifyPassword(string inputPassword)
        {
            if (!IsPasswordSet()) return false;

            try
            {
                string stored = File.ReadAllText(PasswordFile, Encoding.UTF8).Trim();
                if (stored.StartsWith("v2:"))
                {
                    string[] parts = stored.Split(':');
                    if (parts.Length != 4) return false;

                    int iterations = int.Parse(parts[1]);
                    byte[] salt = Convert.FromBase64String(parts[2]);
                    byte[] expectedHash = Convert.FromBase64String(parts[3]);

                    byte[] actualHash;
                    using (var pbkdf2 = new Rfc2898DeriveBytes(inputPassword, salt, iterations))
                    {
                        actualHash = pbkdf2.GetBytes(expectedHash.Length);
                    }

                    return FixedTimeEquals(expectedHash, actualHash);
                }
                else
                {
                    // Format v1 lama (SHA256 tunggal): verifikasi & migrasi otomatis ke v2 jika valid
                    string expectedHashHex = stored;
                    string actualHashHex = ComputeV1Hash(inputPassword);

                    if (string.Equals(expectedHashHex, actualHashHex, StringComparison.OrdinalIgnoreCase))
                    {
                        // Migrasi instan ke v2 tanpa mengunci pengguna
                        try { SetPassword(inputPassword); } catch { }
                        return true;
                    }
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        private static string ComputeV1Hash(string input)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes("AirSET_Controller_Salt_Release_" + input));
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    sb.Append(bytes[i].ToString("x2"));
                }
                return sb.ToString();
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