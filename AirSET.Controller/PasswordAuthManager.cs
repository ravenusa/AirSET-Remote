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

            string hash = ComputeHash(newPassword);
            File.WriteAllText(PasswordFile, hash, Encoding.UTF8);
        }

        public static bool VerifyPassword(string inputPassword)
        {
            if (!IsPasswordSet()) return false;

            try
            {
                string storedHash = File.ReadAllText(PasswordFile, Encoding.UTF8).Trim();
                string inputHash = ComputeHash(inputPassword);
                return string.Equals(storedHash, inputHash, StringComparison.Ordinal);
            }
            catch
            {
                return false;
            }
        }

        private static string ComputeHash(string input)
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
    }
}