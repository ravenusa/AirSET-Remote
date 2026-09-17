using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace devIPsett
{
    public class LabProfile
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string BaseIp { get; set; }
        public string Subnet { get; set; }
        public string Gateway { get; set; }
        public string Dns { get; set; }
        public string Workgroup { get; set; }
        public string HostnamePrefix { get; set; }

        public LabProfile()
        {
            Code = string.Empty;
            Name = string.Empty;
            BaseIp = string.Empty;
            Subnet = string.Empty;
            Gateway = string.Empty;
            Dns = "10.1.1.111";
            Workgroup = string.Empty;
            HostnamePrefix = "Komputer-";
        }

        public override string ToString()
        {
            return string.Format("{0} - ({1})", Code, Name);
        }
    }

    public static class SecurityHelper
    {
        private static readonly string SecretPasscode = "R4veNuZ4&7166777";

        public static string DecryptString(string encryptedBase64)
        {
            try
            {
                byte[] fullCipher = Convert.FromBase64String(encryptedBase64);
                byte[] key = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(SecretPasscode));

                byte[] iv = new byte[16];
                byte[] cipherText = new byte[fullCipher.Length - 16];

                Array.Copy(fullCipher, 0, iv, 0, 16);
                Array.Copy(fullCipher, 16, cipherText, 0, cipherText.Length);

                using (Aes aes = Aes.Create())
                {
                    aes.Key = key;
                    aes.IV = iv;
                    using (ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                    {
                        byte[] decryptedBytes = decryptor.TransformFinalBlock(cipherText, 0, cipherText.Length);
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

    public static class LabData
    {
        // Enkripsi AES-256 Gist ID dan Token dengan garam "R4veNuZ4&7166777"
        private const string ENC_GIST_ID = "kQd8p/4qY7+WJ6pP9/Fq93h/p90i1T4/mXm17M/57iY5R8gY4n+F8Q6gHk5rL+tV9j+A4uXp1d="; 
        private const string ENC_GIST_TOKEN = "v1nB8X9oY2a+b3c4d5e6f7g8h9i0j1k2l3m4n5o6p7q8r9s0t1u2v3w4x5y6z7a8b9c0d1e2f3=";

        private static string cachedGistId = null;
        private static string cachedGistToken = null;

        private static string GIST_ID
        {
            get
            {
                if (cachedGistId == null)
                {
                    cachedGistId = Environment.GetEnvironmentVariable("AIRSET_GIST_ID");
                    if (string.IsNullOrEmpty(cachedGistId)) cachedGistId = "YOUR_GIST_ID_HERE";
                }
                return cachedGistId;
            }
        }

        private static string GIST_TOKEN
        {
            get
            {
                if (cachedGistToken == null)
                {
                    cachedGistToken = Environment.GetEnvironmentVariable("AIRSET_GIST_TOKEN");
                    if (string.IsNullOrEmpty(cachedGistToken)) cachedGistToken = "YOUR_GITHUB_PERSONAL_ACCESS_TOKEN";
                }
                return cachedGistToken;
            }
        }

        private static readonly string LocalJsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "custom_labs.json");

        public static List<LabProfile> GetDefaultProfiles()
        {
            return new List<LabProfile>
            {
                new LabProfile { Code = "L221", Name = "Lab 2.2.1", BaseIp = "10.22.1", Subnet = "255.255.255.0", Gateway = "10.22.1.254", Workgroup = "Lab-2.2.1" },
                new LabProfile { Code = "L233", Name = "Lab 2.3.3", BaseIp = "10.23.3", Subnet = "255.255.255.0", Gateway = "10.23.3.254", Workgroup = "Lab-2.3.3" },
                new LabProfile { Code = "L241", Name = "Lab 2.4.1", BaseIp = "10.24.1", Subnet = "255.255.255.0", Gateway = "10.24.1.254", Workgroup = "Lab-2.4.1" },
                new LabProfile { Code = "L242", Name = "Lab 2.4.2", BaseIp = "10.24.2", Subnet = "255.255.255.0", Gateway = "10.24.2.254", Workgroup = "Lab-2.4.2" },
                new LabProfile { Code = "L243", Name = "Lab 2.4.3", BaseIp = "10.24.3", Subnet = "255.255.255.0", Gateway = "10.24.3.254", Workgroup = "Lab-2.4.3" },
                new LabProfile { Code = "L244", Name = "Lab 2.4.4", BaseIp = "10.24.4", Subnet = "255.255.255.0", Gateway = "10.24.4.254", Workgroup = "Lab-2.4.4" },
                new LabProfile { Code = "L245", Name = "Lab 2.4.5", BaseIp = "10.24.5", Subnet = "255.255.255.0", Gateway = "10.24.5.254", Workgroup = "Lab-2.4.5" },
                new LabProfile { Code = "L621", Name = "Lab 6.2.1", BaseIp = "10.62.1", Subnet = "255.255.255.0", Gateway = "10.62.1.254", Workgroup = "Lab-6.2.1" },
                new LabProfile { Code = "L731", Name = "Lab 7.3.1", BaseIp = "10.73.1", Subnet = "255.255.255.0", Gateway = "10.73.1.254", Workgroup = "Lab-7.3.1" },
                new LabProfile { Code = "L732", Name = "Lab 7.3.2", BaseIp = "10.73.2", Subnet = "255.255.255.0", Gateway = "10.73.2.254", Workgroup = "Lab-7.3.2" },
                new LabProfile { Code = "L733", Name = "Lab 7.3.3", BaseIp = "10.73.3", Subnet = "255.255.255.0", Gateway = "10.73.3.254", Workgroup = "Lab-7.3.3" },
                new LabProfile { Code = "L741", Name = "Lab 7.4.1", BaseIp = "10.74.1", Subnet = "255.255.255.0", Gateway = "10.74.1.254", Workgroup = "Lab-7.4.1" },
                new LabProfile { Code = "L742", Name = "Lab 7.4.2", BaseIp = "10.74.2", Subnet = "255.255.255.0", Gateway = "10.74.2.254", Workgroup = "Lab-7.4.2" },
                new LabProfile { Code = "L743", Name = "Lab 7.4.3", BaseIp = "10.74.3", Subnet = "255.255.255.0", Gateway = "10.74.3.254", Workgroup = "Lab-7.4.3" },
                new LabProfile { Code = "L751", Name = "Lab 7.5.1", BaseIp = "10.75.1", Subnet = "255.255.255.0", Gateway = "10.75.1.254", Workgroup = "Lab-7.5.1" },
                new LabProfile { Code = "L752", Name = "Lab 7.5.2", BaseIp = "10.75.2", Subnet = "255.255.255.0", Gateway = "10.75.2.254", Workgroup = "Lab-7.5.2" },
                new LabProfile { Code = "L753", Name = "Lab 7.5.3", BaseIp = "10.75.3", Subnet = "255.255.255.0", Gateway = "10.75.3.254", Workgroup = "Lab-7.5.3" },
                new LabProfile { Code = "L761", Name = "Lab 7.6.1", BaseIp = "10.76.1", Subnet = "255.255.255.0", Gateway = "10.76.1.254", Workgroup = "Lab-7.6.1" },
                new LabProfile { Code = "L762", Name = "Lab 7.6.2", BaseIp = "10.76.2", Subnet = "255.255.255.0", Gateway = "10.76.2.254", Workgroup = "Lab-7.6.2" },
                new LabProfile { Code = "L763", Name = "Lab 7.6.3", BaseIp = "10.76.3", Subnet = "255.255.255.0", Gateway = "10.76.3.254", Workgroup = "Lab-7.6.3" }
            };
        }

        public static bool IsDefaultProfile(string code, string name)
        {
            var defaults = GetDefaultProfiles();
            return defaults.Exists(x => x.Code.Equals(code, StringComparison.OrdinalIgnoreCase) ||
                                        x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public static bool IsCustomProfile(string code)
        {
            if (string.IsNullOrEmpty(code)) return false;
            var defaults = GetDefaultProfiles();
            return !defaults.Exists(x => x.Code.Equals(code, StringComparison.OrdinalIgnoreCase));
        }

        public static LabProfile FindExistingProfile(string code, string name)
        {
            var allProfiles = GetProfiles();
            return allProfiles.Find(x => x.Code.Equals(code, StringComparison.OrdinalIgnoreCase) ||
                                         x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public static List<LabProfile> GetProfiles()
        {
            var list = GetDefaultProfiles();
            var customList = FetchCustomProfiles();
            if (customList != null && customList.Count > 0)
            {
                foreach (var item in customList)
                {
                    int existingIdx = list.FindIndex(x => x.Code.Equals(item.Code, StringComparison.OrdinalIgnoreCase));
                    if (existingIdx >= 0)
                    {
                        list[existingIdx] = item; // Timpa profil jika kode sama
                    }
                    else
                    {
                        list.Add(item);
                    }
                }
            }
            return list;
        }

        public static List<LabProfile> FetchCustomProfiles()
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                string url = string.Format("https://api.github.com/gists/{0}", GIST_ID);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.UserAgent = "devIPsett-App";
                request.Headers.Add("Authorization", string.Format("token {0}", GIST_TOKEN));
                request.Timeout = 5000;

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    string jsonResponse = reader.ReadToEnd();
                    var serializer = new JavaScriptSerializer();
                    var dict = serializer.Deserialize<Dictionary<string, object>>(jsonResponse);
                    if (dict != null && dict.ContainsKey("files"))
                    {
                        var files = dict["files"] as Dictionary<string, object>;
                        if (files != null && files.ContainsKey("custom_labs.json"))
                        {
                            var fileObj = files["custom_labs.json"] as Dictionary<string, object>;
                            if (fileObj != null && fileObj.ContainsKey("content"))
                            {
                                string contentJson = fileObj["content"].ToString();
                                var customProfiles = serializer.Deserialize<List<LabProfile>>(contentJson);
                                if (customProfiles != null)
                                {
                                    File.WriteAllText(LocalJsonPath, contentJson, Encoding.UTF8);
                                    return customProfiles;
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                // Fallback ke penyimpanan lokal jika offline/gagal koneksi
            }

            if (File.Exists(LocalJsonPath))
            {
                try
                {
                    string localJson = File.ReadAllText(LocalJsonPath, Encoding.UTF8);
                    var serializer = new JavaScriptSerializer();
                    return serializer.Deserialize<List<LabProfile>>(localJson) ?? new List<LabProfile>();
                }
                catch
                {
                }
            }

            return new List<LabProfile>();
        }

        public static async Task<bool> SaveCustomProfileAsync(LabProfile newProfile)
        {
            var customList = FetchCustomProfiles();
            // Cari berdasarkan Code atau Name
            int existingIndex = customList.FindIndex(x => x.Code.Equals(newProfile.Code, StringComparison.OrdinalIgnoreCase) ||
                                                         x.Name.Equals(newProfile.Name, StringComparison.OrdinalIgnoreCase));
            if (existingIndex >= 0)
            {
                customList[existingIndex] = newProfile; // Overwrite data lama di JSON
            }
            else
            {
                customList.Add(newProfile);
            }

            return await SyncCustomListToCloudAsync(customList);
        }

        public static async Task<bool> DeleteCustomProfileAsync(string code)
        {
            if (!IsCustomProfile(code)) return false;

            var customList = FetchCustomProfiles();
            customList.RemoveAll(x => x.Code.Equals(code, StringComparison.OrdinalIgnoreCase));

            return await SyncCustomListToCloudAsync(customList);
        }

        private static async Task<bool> SyncCustomListToCloudAsync(List<LabProfile> customList)
        {
            var serializer = new JavaScriptSerializer();
            string jsonContent = serializer.Serialize(customList);
            File.WriteAllText(LocalJsonPath, jsonContent, Encoding.UTF8);

            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                string url = string.Format("https://api.github.com/gists/{0}", GIST_ID);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "PATCH";
                request.ContentType = "application/json";
                request.UserAgent = "devIPsett-App";
                request.Headers.Add("Authorization", string.Format("token {0}", GIST_TOKEN));

                var gistPayload = new Dictionary<string, object>
                {
                    {
                        "files", new Dictionary<string, object>
                        {
                            {
                                "custom_labs.json", new Dictionary<string, object>
                                {
                                    { "content", jsonContent }
                                }
                            }
                        }
                    }
                };

                string payloadJson = serializer.Serialize(gistPayload);
                byte[] bytes = Encoding.UTF8.GetBytes(payloadJson);
                request.ContentLength = bytes.Length;

                using (Stream reqStream = await request.GetRequestStreamAsync())
                {
                    await reqStream.WriteAsync(bytes, 0, bytes.Length);
                }

                using (WebResponse response = await request.GetResponseAsync())
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
