using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace devIPsett
{
    public class SystemExecutionResult
    {
        public bool Success { get; set; }
        public string Output { get; set; }
        public string Error { get; set; }

        public SystemExecutionResult()
        {
            Output = string.Empty;
            Error = string.Empty;
        }
    }

    public static class NetworkServices
    {
        /// <summary>
        /// Mengambil seluruh interface jaringan yang aktif / dapat dikonfigurasi pada PC.
        /// </summary>
        public static List<string> GetNetworkInterfaces()
        {
            var interfaces = new List<string>();
            try
            {
                NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
                foreach (NetworkInterface adapter in nics)
                {
                    if (adapter.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
                        adapter.NetworkInterfaceType != NetworkInterfaceType.Tunnel)
                    {
                        interfaces.Add(adapter.Name);
                    }
                }
            }
            catch
            {
                interfaces.Add("Ethernet");
                interfaces.Add("Wi-Fi");
            }

            if (interfaces.Count == 0)
            {
                interfaces.Add("Ethernet");
            }

            return interfaces;
        }

        /// <summary>
        /// Mengubah IP Address, Subnet Mask, dan Default Gateway menggunakan netsh.
        /// </summary>
        public static async Task<SystemExecutionResult> ApplyIpAddressAsync(string interfaceName, string ipAddress, string subnetMask, string gateway)
        {
            string args = string.Format("interface ipv4 set address name=\"{0}\" static {1} {2} {3}", interfaceName, ipAddress, subnetMask, gateway);
            return await RunProcessAsync("netsh", args);
        }

        /// <summary>
        /// Mengubah Preferred DNS Server menggunakan netsh.
        /// </summary>
        public static async Task<SystemExecutionResult> ApplyDnsAsync(string interfaceName, string dnsAddress)
        {
            string args = string.Format("interface ipv4 set dnsservers name=\"{0}\" static {1} primary", interfaceName, dnsAddress);
            return await RunProcessAsync("netsh", args);
        }

        /// <summary>
        /// Mengubah Nama Komputer (Hostname) menggunakan PowerShell Rename-Computer.
        /// </summary>
        public static async Task<SystemExecutionResult> RenameComputerAsync(string newHostName)
        {
            string psCommand = string.Format("Rename-Computer -NewName '{0}' -Force", newHostName);
            return await RunProcessAsync("powershell", string.Format("-NoProfile -ExecutionPolicy Bypass -Command \"{0}\"", psCommand));
        }

        /// <summary>
        /// Mengubah Workgroup menggunakan WMI (jauh lebih cepat dan tidak menggantung/stuck dibanding Add-Computer).
        /// </summary>
        public static async Task<SystemExecutionResult> ChangeWorkgroupAsync(string workgroupName)
        {
            string psCommand = string.Format("(Get-WmiObject Win32_ComputerSystem).JoinDomainOrWorkgroup('{0}')", workgroupName);
            return await RunProcessAsync("powershell", string.Format("-NoProfile -ExecutionPolicy Bypass -Command \"{0}\"", psCommand), 10000);
        }

        /// <summary>
        /// Mengubah Password User Windows yang sedang aktif & Mengatur Password Never Expire.
        /// </summary>
        public static async Task<SystemExecutionResult> ChangeUserPasswordAsync(string newPassword)
        {
            string username = Environment.UserName;
            // 1. Ubah Password User
            string netUserArgs = string.Format("user \"{0}\" \"{1}\"", username, newPassword);
            var result = await RunProcessAsync("net", netUserArgs);

            // 2. Set Password Never Expire (agar user tidak pernah diingatkan ganti password lagi)
            string psCommand = string.Format("Set-LocalUser -Name '{0}' -PasswordNeverExpires $true -ErrorAction SilentlyContinue", username);
            await RunProcessAsync("powershell", string.Format("-NoProfile -ExecutionPolicy Bypass -Command \"{0}\"", psCommand));

            return result;
        }

        /// <summary>
        /// Mengonfigurasi Windows Auto-Logon pada Registry (Bypass Layar Login saat PC Hidup/Restart).
        /// </summary>
        public static SystemExecutionResult ConfigureAutoLogon(string newPassword)
        {
            var result = new SystemExecutionResult();
            try
            {
                string username = Environment.UserName;
                string domainName = Environment.UserDomainName;
                string winlogonKey = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon";

                using (RegistryKey baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
                using (RegistryKey key = baseKey.OpenSubKey(winlogonKey, true))
                {
                    if (key != null)
                    {
                        key.SetValue("AutoAdminLogon", "1", RegistryValueKind.String);
                        key.SetValue("DefaultUserName", username, RegistryValueKind.String);
                        key.SetValue("DefaultDomainName", string.IsNullOrEmpty(domainName) ? "." : domainName, RegistryValueKind.String);
                        key.SetValue("DefaultPassword", newPassword, RegistryValueKind.String);

                        // Hapus ForceAutoLogon agar saat user Sign Out / Lock (Win+L), Windows TETAP meminta password (persis Autologon Microsoft)
                        try { key.DeleteValue("ForceAutoLogon", false); } catch { }
                        try { key.DeleteValue("AutoLogonCount", false); } catch { }
                        try { key.SetValue("LegalNoticeCaption", "", RegistryValueKind.String); } catch { }
                        try { key.SetValue("LegalNoticeText", "", RegistryValueKind.String); } catch { }

                        result.Success = true;
                        result.Output = "Auto-Logon berhasil dikonfigurasi di Registry Native 64-Bit Windows.";
                    }
                    else
                    {
                        result.Success = false;
                        result.Error = "Gagal membuka Registry Key Winlogon 64-Bit.";
                    }
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Error = ex.Message;
            }
            return result;
        }

        /// <summary>
        /// Menolak/Matikan Windows Auto-Logon pada Registry (Memaksa memasukkan password saat startup).
        /// </summary>
        public static SystemExecutionResult DisableAutoLogon()
        {
            var result = new SystemExecutionResult();
            try
            {
                string winlogonKey = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon";

                using (RegistryKey baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
                using (RegistryKey key = baseKey.OpenSubKey(winlogonKey, true))
                {
                    if (key != null)
                    {
                        key.SetValue("AutoAdminLogon", "0", RegistryValueKind.String);
                        try { key.DeleteValue("DefaultPassword", false); } catch { }
                        try { key.DeleteValue("ForceAutoLogon", false); } catch { }

                        result.Success = true;
                        result.Output = "Auto-Logon berhasil dinonaktifkan di Registry Native 64-Bit Windows.";
                    }
                    else
                    {
                        result.Success = false;
                        result.Error = "Gagal membuka Registry Key Winlogon 64-Bit.";
                    }
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Error = ex.Message;
            }
            return result;
        }

        /// <summary>
        /// Melakukan Restart Sistem Windows dengan jeda waktu dan pesan custom.
        /// </summary>
        public static void RestartSystem(int delaySeconds = 4, string comment = "Restart To Apply | Request by AirV")
        {
            Process.Start("shutdown", string.Format("/r /t {0} /c \"{1}\"", delaySeconds, comment));
        }

        private static Task<SystemExecutionResult> RunProcessAsync(string fileName, string arguments, int timeoutMilliseconds = 0)
        {
            var tcs = new TaskCompletionSource<SystemExecutionResult>();

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                },
                EnableRaisingEvents = true
            };

            var outputBuilder = new StringBuilder();
            var errorBuilder = new StringBuilder();

            process.OutputDataReceived += (sender, args) =>
            {
                if (args.Data != null) outputBuilder.AppendLine(args.Data);
            };

            process.ErrorDataReceived += (sender, args) =>
            {
                if (args.Data != null) errorBuilder.AppendLine(args.Data);
            };

            process.Exited += (sender, args) =>
            {
                tcs.TrySetResult(new SystemExecutionResult
                {
                    Success = process.ExitCode == 0,
                    Output = outputBuilder.ToString().Trim(),
                    Error = errorBuilder.ToString().Trim()
                });
                process.Dispose();
            };

            try
            {
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                if (timeoutMilliseconds > 0)
                {
                    Task.Delay(timeoutMilliseconds).ContinueWith(t =>
                    {
                        if (!process.HasExited)
                        {
                            try { process.Kill(); } catch { }
                            tcs.TrySetResult(new SystemExecutionResult
                            {
                                Success = true,
                                Output = "Proses selesai/timeout.",
                                Error = string.Empty
                            });
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                tcs.TrySetResult(new SystemExecutionResult
                {
                    Success = false,
                    Error = ex.Message
                });
            }

            return tcs.Task;
        }
    }
}
