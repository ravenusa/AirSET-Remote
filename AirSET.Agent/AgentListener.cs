using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AirSET.Core.Models;
using AirSET.Core.Services;

namespace AirSET.Agent
{
    public class AgentListener
    {
        private UdpClient udpListener;
        private TcpListener tcpListener;
        private CancellationTokenSource cts;
        public bool IsRunning { get; private set; }

        public event Action<string> OnLogReceived;

        public void Start()
        {
            if (IsRunning) return;
            IsRunning = true;
            cts = new CancellationTokenSource();

            Task.Run(() => ListenUdpDiscovery(cts.Token));
            Task.Run(() => ListenTcpCommands(cts.Token));
            Log("AirSET Agent Service aktif di Port " + NetworkConstants.DefaultPort);
        }

        public void Stop()
        {
            IsRunning = false;
            try { cts?.Cancel(); } catch { }
            try { udpListener?.Close(); } catch { }
            try { tcpListener?.Stop(); } catch { }
            Log("AirSET Agent Service dinonaktifkan.");
        }

        private devIPsett.OsAndShieldInfo cachedShieldInfo = null;
        private DateTime lastShieldCacheTime = DateTime.MinValue;

        private devIPsett.OsAndShieldInfo GetCachedShieldInfo()
        {
            // Cache valid selama 30 detik agar tidak membebani eksekusi process/WMI berulang
            if (cachedShieldInfo == null || (DateTime.Now - lastShieldCacheTime).TotalSeconds > 30)
            {
                cachedShieldInfo = devIPsett.DiskShieldServices.DetectOsAndShieldStatus();
                lastShieldCacheTime = DateTime.Now;
            }
            return cachedShieldInfo;
        }

        private void RefreshShieldCacheInBackground()
        {
            Task.Run(() =>
            {
                try
                {
                    cachedShieldInfo = devIPsett.DiskShieldServices.DetectOsAndShieldStatus();
                    lastShieldCacheTime = DateTime.Now;
                }
                catch { }
            });
        }

        private async Task ListenUdpDiscovery(CancellationToken token)
        {
            try
            {
                udpListener = new UdpClient();
                udpListener.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                udpListener.Client.Bind(new IPEndPoint(IPAddress.Any, NetworkConstants.DefaultPort));

                // Pre-warm cache saat listener aktif
                RefreshShieldCacheInBackground();

                while (!token.IsCancellationRequested)
                {
                    var receivedResult = await udpListener.ReceiveAsync();
                    string rawData = Encoding.UTF8.GetString(receivedResult.Buffer);
                    if (rawData.Contains("DISCOVERY_REQUEST"))
                    {
                        var shieldInfo = GetCachedShieldInfo();

                        var response = new DiscoveryMessage
                        {
                            Type = "DISCOVERY_RESPONSE",
                            Hostname = System.Net.Dns.GetHostName(),
                            MacAddress = GetActiveAdapterMac(),
                            CurrentIp = GetLocalIpAddress(),
                            CurrentAdapter = GetActiveAdapterName(),
                            Status = FormLockScreen.IsLocked ? "Locked" : "Online",
                            OsCaption = shieldInfo != null ? shieldInfo.OsCaption : "Windows",
                            ShieldStatus = shieldInfo != null ? shieldInfo.ShieldStatus : "Unknown"
                        };

                        string jsonResponse = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(response);
                        byte[] sendBytes = Encoding.UTF8.GetBytes(jsonResponse);
                        await udpListener.SendAsync(sendBytes, sendBytes.Length, receivedResult.RemoteEndPoint);
                        Log("Merespon broadcast discovery dari: " + receivedResult.RemoteEndPoint.Address);
                    }
                }
            }
            catch (Exception ex)
            {
                if (IsRunning) Log("UDP Listener Error: " + ex.Message);
            }
        }

        private static readonly SemaphoreSlim _tcpConcurrencyLimiter = new SemaphoreSlim(32, 32);

        private async Task ListenTcpCommands(CancellationToken token)
        {
            try
            {
                tcpListener = new TcpListener(IPAddress.Any, NetworkConstants.DefaultPort);
                tcpListener.Start();

                while (!token.IsCancellationRequested)
                {
                    TcpClient client = await tcpListener.AcceptTcpClientAsync();
                    if (!await _tcpConcurrencyLimiter.WaitAsync(100, token))
                    {
                        // Batas konkurensi (32 koneksi aktif) terlampaui, tolak koneksi berlebih
                        try { client.Close(); } catch { }
                        continue;
                    }

                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            await HandleClientCommand(client);
                        }
                        finally
                        {
                            _tcpConcurrencyLimiter.Release();
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                if (IsRunning) Log("TCP Listener Error: " + ex.Message);
            }
        }

        private async Task HandleClientCommand(TcpClient client)
        {
            using (client)
            using (NetworkStream stream = client.GetStream())
            {
                try
                {
                    // Atur socket timeout 30 detik untuk mencegah koneksi menggantung menahan slot
                    client.ReceiveTimeout = 30000;
                    client.SendTimeout = 30000;

                    // Baca 4 byte length prefix terlebih dahulu
                    byte[] lengthBuffer = new byte[4];
                    int totalLengthBytes = 0;
                    while (totalLengthBytes < 4)
                    {
                        int read = await stream.ReadAsync(lengthBuffer, totalLengthBytes, 4 - totalLengthBytes);
                        if (read <= 0) break;
                        totalLengthBytes += read;
                    }

                    if (totalLengthBytes < 4)
                    {
                        return; // Koneksi ditutup sebelum length-prefix diterima
                    }

                    int payloadLength = BitConverter.ToInt32(lengthBuffer, 0);
                    // Batasi payload maksimum yang diizinkan (maksimal 500MB untuk transfer file besar)
                    if (payloadLength <= 0 || payloadLength > 500 * 1024 * 1024)
                    {
                        Log(string.Format("Koneksi ditolak: ukuran payload tidak valid ({0} bytes)", payloadLength));
                        return;
                    }

                    string rawCipher = string.Empty;
                    using (var ms = new MemoryStream())
                    {
                        byte[] buffer = new byte[65536]; // Buffer tetap 64 KB
                        int bytesRemaining = payloadLength;
                        while (bytesRemaining > 0)
                        {
                            int toRead = Math.Min(buffer.Length, bytesRemaining);
                            int read = await stream.ReadAsync(buffer, 0, toRead);
                            if (read <= 0) break;
                            ms.Write(buffer, 0, read);
                            bytesRemaining -= read;
                        }

                        if (ms.Length == payloadLength)
                        {
                            rawCipher = Encoding.UTF8.GetString(ms.ToArray());
                        }
                    }

                    RemoteConfigPayload payload = null;
                    try
                    {
                        if (!string.IsNullOrEmpty(rawCipher))
                        {
                            payload = CryptoManager.Decrypt<RemoteConfigPayload>(rawCipher);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log("Dekripsi payload error: " + ex.Message);
                    }

                    var response = new ExecutionResponse { Hostname = System.Net.Dns.GetHostName() };
                    bool alreadySentResponse = false;

                    if (payload != null && payload.Action == "APPLY_CONFIG")
                    {
                        Log(string.Format("Menerima konfigurasi remote: PC #{0}, BaseIP: {1}", payload.PcNumber, payload.BaseIp));
                        string calculatedIp = string.Format("{0}.{1}", payload.BaseIp, payload.PcNumber);
                        string calculatedHostname = string.Format("{0}{1:D2}", payload.Prefix, payload.PcNumber);

                        IPAddress clientLocalIp = null;
                        if (client.Client != null && client.Client.LocalEndPoint is IPEndPoint ep)
                        {
                            clientLocalIp = ep.Address;
                        }

                        string targetInterface = ResolveTargetInterface(payload.TargetInterface, clientLocalIp);
                        Log(string.Format("Mengonfigurasi Adapter: '{0}' (Mode: {1})", targetInterface, payload.TargetInterface ?? "Auto"));

                        var logSummary = new StringBuilder();
                        bool allSuccessful = true;

                        // 1. Rename Hostname
                        var renameRes = await devIPsett.NetworkServices.RenameComputerAsync(calculatedHostname);
                        if (renameRes.Success)
                        {
                            logSummary.Append("Hostname [OK] ");
                        }

                        // 2. Change Workgroup
                        if (!string.IsNullOrEmpty(payload.Workgroup))
                        {
                            await devIPsett.NetworkServices.ChangeWorkgroupAsync(payload.Workgroup);
                        }

                        // 3. Password
                        if (payload.ChangePassword && !string.IsNullOrEmpty(payload.NewPassword))
                        {
                            var passRes = await devIPsett.NetworkServices.ChangeUserPasswordAsync(payload.NewPassword);
                            if (passRes.Success)
                            {
                                logSummary.Append("Pass [OK] ");
                            }
                            else
                            {
                                logSummary.Append("Pass Failed ");
                            }
                        }

                        // 4. Auto-Logon (Independent from ChangePassword)
                        if (payload.AutoLogon)
                        {
                            var autoRes = devIPsett.NetworkServices.ConfigureAutoLogon(payload.NewPassword ?? "");
                            if (autoRes.Success)
                            {
                                logSummary.Append("AutoLogon [OK] ");
                            }
                            else
                            {
                                logSummary.Append("AutoLogon Failed ");
                            }
                        }
                        else if (payload.DisableAutoLogon)
                        {
                            var disRes = devIPsett.NetworkServices.DisableAutoLogon();
                            if (disRes.Success)
                            {
                                logSummary.Append("DisableLogon [OK] ");
                            }
                        }

                        if (payload.RestartAfterApply)
                        {
                            logSummary.Append("-> Rebooting Client");
                        }

                        response.Success = true;
                        response.Message = string.Format("Adapter: '{0}' -> {1}", targetInterface, logSummary.ToString().Trim());
                        Log(string.Format("Selesai eksekusi setting sistem: {0}", response.Message));

                        // Kirim respon balik ke Controller SEBELUM mengubah IP adapter atau me-restart PC
                        // Agar koneksi TCP Controller tidak terputus di tengah jalan!
                        string jsonRespEarly = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(response);
                        byte[] respBytesEarly = Encoding.UTF8.GetBytes(jsonRespEarly);
                        await stream.WriteAsync(respBytesEarly, 0, respBytesEarly.Length);
                        await stream.FlushAsync();
                        alreadySentResponse = true;

                        // Beri jeda sangat singkat agar paket TCP respon sampai ke Controller
                        await Task.Delay(250);

                        // 5. Apply IP & DNS sekarang (setelah TCP respon terkirim)
                        _ = Task.Run(async () =>
                        {
                            try
                            {
                                var ipRes = await devIPsett.NetworkServices.ApplyIpAddressAsync(targetInterface, calculatedIp, payload.Subnet, payload.Gateway);
                                Log(string.Format("Apply IP {0}: {1}", calculatedIp, ipRes.Success ? "OK" : ipRes.Error));

                                if (!string.IsNullOrEmpty(payload.Dns))
                                {
                                    var dnsRes = await devIPsett.NetworkServices.ApplyDnsAsync(targetInterface, payload.Dns);
                                    Log(string.Format("Apply DNS {0}: {1}", payload.Dns, dnsRes.Success ? "OK" : dnsRes.Error));
                                }

                                if (payload.RestartAfterApply)
                                {
                                    devIPsett.NetworkServices.RestartSystem(3, "Restart Request | Ravenusa");
                                }
                            }
                            catch (Exception ex)
                            {
                                Log("Error apply network async: " + ex.Message);
                            }
                        });
                    }
                    else if (payload != null && payload.Action == "PING_DISCOVERY")
                    {
                        var shieldInfo = GetCachedShieldInfo();
                        response.Success = true;
                        response.Hostname = System.Net.Dns.GetHostName();
                        response.Message = "Online";
                        // Serialisasikan info OS, Shield, & MAC ke OutputLog agar Controller Direct Scan juga mendapatkannya
                        response.OutputLog = string.Format("{0}|{1}|{2}", shieldInfo != null ? shieldInfo.OsCaption : "Windows", shieldInfo != null ? shieldInfo.ShieldStatus : "Unknown", GetActiveAdapterMac());
                    }
                    else if (payload != null && payload.Action == "SHIELD_ACTION")
                    {
                        Log(string.Format("Menerima perintah Disk Shield: {0}", payload.ShieldCommand));
                        string cmd = payload.ShieldCommand ?? "";

                        devIPsett.SystemExecutionResult execResult = null;
                        string rebootReason = "";

                        try
                        {
                            if (cmd == "UWF_INSTALL")
                            {
                                Log("Memulai instalasi fitur Windows UWF (DISM)...");
                                execResult = await devIPsett.DiskShieldServices.InstallUwfFeatureAsync();
                                rebootReason = "Restart Request | Ravenusa";
                            }
                            else if (cmd == "UWF_UNINSTALL")
                            {
                                Log("Memulai proses uninstall fitur Windows UWF (DISM)...");
                                execResult = await devIPsett.DiskShieldServices.UninstallUwfFeatureAsync();
                                rebootReason = "Restart Request | Ravenusa";
                            }
                            else if (cmd == "UWF_LOCK")
                            {
                                Log("Mengaktifkan proteksi UWF (Lock Disk C:)...");
                                execResult = await devIPsett.DiskShieldServices.EnableUwfAsync();
                                rebootReason = "Restart Request | Ravenusa";
                            }
                            else if (cmd == "UWF_UNLOCK")
                            {
                                Log("Menonaktifkan proteksi UWF (Unlock Disk)...");
                                execResult = await devIPsett.DiskShieldServices.DisableUwfAsync();
                                rebootReason = "Restart Request | Ravenusa";
                            }
                            else if (cmd == "DFC_FREEZE")
                            {
                                Log("Mengeksekusi Deep Freeze Freeze...");
                                execResult = await devIPsett.DiskShieldServices.FreezeDeepFreezeAsync(payload.DeepFreezePassword);
                                rebootReason = "Restart untuk Freeze Deep Freeze | AirV";
                            }
                            else if (cmd == "DFC_THAW")
                            {
                                Log("Mengeksekusi Deep Freeze Thaw...");
                                execResult = await devIPsett.DiskShieldServices.ThawDeepFreezeAsync(payload.DeepFreezePassword);
                                rebootReason = "Restart untuk Thaw Deep Freeze | AirV";
                            }
                            else
                            {
                                execResult = new devIPsett.SystemExecutionResult { Success = false, Error = "Perintah shield tidak dikenali: " + cmd };
                            }
                        }
                        catch (Exception ex)
                        {
                            execResult = new devIPsett.SystemExecutionResult { Success = false, Error = ex.Message };
                        }

                        if (execResult != null && execResult.Success)
                        {
                            Log(string.Format("Eksekusi {0} BERHASIL: {1}", cmd, execResult.Output));
                            response.Success = true;
                            bool shouldRestart = payload != null && payload.RestartAfterApply;
                            response.Message = shouldRestart
                                ? string.Format("Perintah '{0}' sukses. Memulai reboot...", cmd)
                                : string.Format("Perintah '{0}' sukses diterapkan (siap diverifikasi manual).", cmd);
                            response.OutputLog = execResult.Output;

                            // Kirim respon sukses ke Controller SEBELUM restart
                            string jsonRespShield = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(response);
                            byte[] respBytesShield = Encoding.UTF8.GetBytes(jsonRespShield);
                            await stream.WriteAsync(respBytesShield, 0, respBytesShield.Length);
                            await stream.FlushAsync();
                            alreadySentResponse = true;

                            if (shouldRestart)
                            {
                                await Task.Delay(250);
                                devIPsett.NetworkServices.RestartSystem(3, rebootReason);
                            }
                        }
                        else
                        {
                            string err = (execResult != null) ? execResult.Error : "Unknown error";
                            Log(string.Format("GAGAL eksekusi {0}: {1}", cmd, err));
                            response.Success = false;
                            response.Message = string.Format("Gagal ({0}): {1}", cmd, err);
                            response.OutputLog = err;

                            string jsonRespShield = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(response);
                            byte[] respBytesShield = Encoding.UTF8.GetBytes(jsonRespShield);
                            await stream.WriteAsync(respBytesShield, 0, respBytesShield.Length);
                            await stream.FlushAsync();
                            alreadySentResponse = true;
                        }
                    }
                    else if (payload != null && (payload.Action == "REBOOT" || payload.Action == "POWER_REBOOT"))
                    {
                        response.Success = true;
                        response.Message = "Memulai restart sistem...";

                        string jsonRespReboot = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(response);
                        byte[] respBytesReboot = Encoding.UTF8.GetBytes(jsonRespReboot);
                        await stream.WriteAsync(respBytesReboot, 0, respBytesReboot.Length);
                        await stream.FlushAsync();
                        alreadySentResponse = true;

                        await Task.Delay(250);
                        devIPsett.NetworkServices.RestartSystem(3, "Restart Request | Ravenusa");
                    }
                    else if (payload != null && payload.Action == "POWER_SHUTDOWN")
                    {
                        response.Success = true;
                        response.Message = "Memulai shutdown sistem...";

                        string jsonRespShutdown = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(response);
                        byte[] respBytesShutdown = Encoding.UTF8.GetBytes(jsonRespShutdown);
                        await stream.WriteAsync(respBytesShutdown, 0, respBytesShutdown.Length);
                        await stream.FlushAsync();
                        alreadySentResponse = true;

                        await Task.Delay(250);
                        devIPsett.NetworkServices.ShutdownSystem(3, "Remote Shutdown by AirSET Controller");
                    }
                    else if (payload != null && payload.Action == "CLEAR_DESKTOP")
                    {
                        int closedCount = CloseRunningDesktopApplications();
                        response.Success = true;
                        response.Message = string.Format("Desktop dibersihkan. {0} aplikasi berhasil ditutup.", closedCount);
                        Log(response.Message);
                    }
                    else if (payload != null && payload.Action == "QUICK_LAUNCH")
                    {
                        string target = (payload.LaunchTarget ?? "").Trim().Trim('\"', '\'').Trim();
                        string args = (payload.LaunchArguments ?? "").Trim();

                        // Auto-koreksi typo umum seperti 'Dekstop' -> 'Desktop'
                        if (target.IndexOf(@"\Dekstop\", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            target = Regex.Replace(target, @"\\Dekstop\\", @"\Desktop\", RegexOptions.IgnoreCase);
                        }
                        else if (target.IndexOf(@"/Dekstop/", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            target = Regex.Replace(target, @"/Dekstop/", @"/Desktop/", RegexOptions.IgnoreCase);
                        }

                        // Jika path file tidak ditemukan secara langsung, coba cari di Desktop pengguna lokal saat ini
                        if (!File.Exists(target) && !target.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                        {
                            string fileNameOnly = Path.GetFileName(target);
                            var desktopCandidates = ResolveTargetCandidateDirectories("Desktop");
                            foreach (var dDir in desktopCandidates)
                            {
                                string candidatePath = Path.Combine(dDir, fileNameOnly);
                                if (File.Exists(candidatePath))
                                {
                                    target = candidatePath;
                                    break;
                                }
                            }
                        }

                        try
                        {
                            if (!string.IsNullOrEmpty(target))
                            {
                                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                                {
                                    FileName = target,
                                    Arguments = args,
                                    UseShellExecute = true,
                                    WorkingDirectory = Path.GetDirectoryName(target) ?? ""
                                });
                                response.Success = true;
                                response.Message = string.Format("Berhasil meluncurkan: {0}", target);
                            }
                            else
                            {
                                response.Success = false;
                                response.Message = "Target aplikasi tidak boleh kosong.";
                            }
                        }
                        catch (Exception ex)
                        {
                            response.Success = false;
                            response.Message = string.Format("Gagal meluncurkan {0}: {1}", target, ex.Message);
                        }
                        Log(response.Message);
                    }
                    else if (payload != null && payload.Action == "REMOTE_POWERSHELL")
                    {
                        string script = !string.IsNullOrEmpty(payload.ScriptContent) ? payload.ScriptContent : (payload.LaunchArguments ?? "");
                        try
                        {
                            if (!string.IsNullOrEmpty(script))
                            {
                                string tempPs1 = Path.Combine(Path.GetTempPath(), "airset_remote_" + Guid.NewGuid().ToString("N") + ".ps1");
                                File.WriteAllText(tempPs1, script, Encoding.UTF8);

                                var psi = new System.Diagnostics.ProcessStartInfo
                                {
                                    FileName = "powershell.exe",
                                    Arguments = string.Format("-NoProfile -ExecutionPolicy Bypass -File \"{0}\"", tempPs1),
                                    UseShellExecute = false,
                                    RedirectStandardOutput = true,
                                    RedirectStandardError = true,
                                    CreateNoWindow = true
                                };
                                using (var proc = System.Diagnostics.Process.Start(psi))
                                {
                                    string stdout = await proc.StandardOutput.ReadToEndAsync();
                                    string stderr = await proc.StandardError.ReadToEndAsync();
                                    proc.WaitForExit(60000);
                                    response.Success = (proc.ExitCode == 0);
                                    string output = (!string.IsNullOrEmpty(stdout) ? stdout.Trim() : "");
                                    if (!string.IsNullOrEmpty(stderr))
                                    {
                                        output += (output.Length > 0 ? "\n" : "") + "ERR: " + stderr.Trim();
                                    }
                                    response.OutputLog = output;
                                    response.Message = response.Success ? "PowerShell sukses dieksekusi." : "PowerShell selesai (ExitCode " + proc.ExitCode + ")";
                                }

                                try { File.Delete(tempPs1); } catch { }
                            }
                            else
                            {
                                response.Success = false;
                                response.Message = "Perintah PowerShell tidak boleh kosong.";
                            }
                        }
                        catch (Exception ex)
                        {
                            response.Success = false;
                            response.Message = "Gagal menjalankan PowerShell: " + ex.Message;
                        }
                        Log(response.Message);
                    }
                    else if (payload != null && payload.Action == "LOCK_SCREEN")
                    {
                        try
                        {
                            FormLockScreen.LockScreen();
                            response.Success = true;
                            response.Message = "Layar dan input device (keyboard & mouse) berhasil dikunci.";
                        }
                        catch (Exception ex)
                        {
                            response.Success = false;
                            response.Message = "Gagal mengunci layar: " + ex.Message;
                        }
                        Log(response.Message);
                    }
                    else if (payload != null && payload.Action == "UNLOCK_SCREEN")
                    {
                        try
                        {
                            FormLockScreen.UnlockScreen();
                            response.Success = true;
                            response.Message = "Layar dan input device berhasil dibuka kembali.";
                        }
                        catch (Exception ex)
                        {
                            response.Success = false;
                            response.Message = "Gagal membuka layar: " + ex.Message;
                        }
                        Log(response.Message);
                    }
                    else if (payload != null && (payload.Action == "FILE_TRANSFER" || payload.Action == "FILE_DISTRIBUTION"))
                    {
                        try
                        {
                            if (string.IsNullOrEmpty(payload.FileName) || payload.FileDataBase64 == null)
                            {
                                response.Success = false;
                                response.Message = "Data file tidak valid (nama file atau data kosong).";
                            }
                            else
                            {
                                string targetDir = ResolveTargetDirectory(payload.TargetDirectory);
                                if (!Directory.Exists(targetDir))
                                {
                                    Directory.CreateDirectory(targetDir);
                                }

                                string safeFileName = Path.GetFileName(CleanPath(payload.FileName));
                                string fullPath = Path.Combine(targetDir, safeFileName);
                                byte[] fileBytes = Convert.FromBase64String(payload.FileDataBase64);
                                File.WriteAllBytes(fullPath, fileBytes);

                                response.Success = true;
                                response.Message = string.Format("File '{0}' berhasil disimpan di {1}", safeFileName, targetDir);
                                Log(response.Message);
                            }
                        }
                        catch (Exception ex)
                        {
                            response.Success = false;
                            response.Message = string.Format("Gagal menyimpan file: {0}", ex.Message);
                            Log(response.Message);
                        }
                    }
                    else if (payload != null && payload.Action == "COLLECT_WORK")
                    {
                        try
                        {
                            string rawSource = payload.CollectSourceFolder;
                            string sourcePath = CleanPath(rawSource);
                            if (string.IsNullOrEmpty(sourcePath))
                            {
                                sourcePath = "Desktop";
                            }

                            var candidateFolders = ResolveTargetCandidateDirectories(sourcePath);
                            if (!candidateFolders.Contains(sourcePath, StringComparer.OrdinalIgnoreCase) && Directory.Exists(sourcePath))
                            {
                                candidateFolders.Insert(0, sourcePath);
                            }

                            string pattern = (payload.CollectFilePattern ?? "").Trim();
                            if (string.IsNullOrEmpty(pattern))
                            {
                                pattern = "*.*";
                            }

                            var matchedFiles = new List<string>();
                            var patterns = pattern.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);

                            foreach (var folder in candidateFolders)
                            {
                                if (Directory.Exists(folder))
                                {
                                    foreach (var p in patterns)
                                    {
                                        string trimmedP = p.Trim();
                                        if (string.IsNullOrEmpty(trimmedP)) continue;
                                        try
                                        {
                                            var found = Directory.GetFiles(folder, trimmedP, SearchOption.TopDirectoryOnly);
                                            matchedFiles.AddRange(found);
                                        }
                                        catch { }
                                    }
                                }
                                else if (File.Exists(folder))
                                {
                                    matchedFiles.Add(folder);
                                }
                            }

                            matchedFiles = matchedFiles.Distinct(StringComparer.OrdinalIgnoreCase).ToList();

                            if (matchedFiles.Count == 0)
                            {
                                string searchedFoldersSummary = string.Join(", ", candidateFolders.Take(2));
                                response.Success = false;
                                response.Message = string.Format("Tidak ditemukan file '{0}' di folder '{1}'", pattern, searchedFoldersSummary);
                                Log(response.Message);
                            }
                            else
                            {
                                string tempZip = Path.Combine(Path.GetTempPath(), string.Format("Tugas_{0}_{1}_{2}.zip", System.Net.Dns.GetHostName(), DateTime.Now.ToString("yyyyMMdd_HHmmss"), Guid.NewGuid().ToString("N").Substring(0, 4)));
                                if (File.Exists(tempZip)) File.Delete(tempZip);

                                using (var zipStream = new FileStream(tempZip, FileMode.Create))
                                using (var archive = new System.IO.Compression.ZipArchive(zipStream, System.IO.Compression.ZipArchiveMode.Create))
                                {
                                    var usedNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                                    foreach (var f in matchedFiles)
                                    {
                                        string entryName = Path.GetFileName(f);
                                        if (usedNames.Contains(entryName))
                                        {
                                            entryName = Path.GetFileNameWithoutExtension(f) + "_" + Guid.NewGuid().ToString("N").Substring(0, 4) + Path.GetExtension(f);
                                        }
                                        usedNames.Add(entryName);
                                        archive.CreateEntryFromFile(f, entryName, System.IO.Compression.CompressionLevel.Fastest);
                                    }
                                }

                                byte[] zipBytes = File.ReadAllBytes(tempZip);
                                response.FileDataResponseBase64 = Convert.ToBase64String(zipBytes);
                                response.ResultFileName = Path.GetFileName(tempZip);
                                response.Success = true;
                                response.Message = string.Format("Berhasil mengumpulkan {0} file '{1}' ({2:N1} KB)", matchedFiles.Count, pattern, zipBytes.Length / 1024.0);

                                try { File.Delete(tempZip); } catch { }

                                // Kirim respon terlebih dahulu secara chunked dengan 4-byte length prefix
                                string jsonCollectResp = new System.Web.Script.Serialization.JavaScriptSerializer { MaxJsonLength = int.MaxValue }.Serialize(response);
                                byte[] respBytesCollect = Encoding.UTF8.GetBytes(jsonCollectResp);
                                byte[] lengthPrefix = BitConverter.GetBytes(respBytesCollect.Length);

                                await stream.WriteAsync(lengthPrefix, 0, lengthPrefix.Length);
                                int chunkOffset = 0;
                                int chunkLimit = 65536; // 64 KB per chunk
                                while (chunkOffset < respBytesCollect.Length)
                                {
                                    int bytesToSend = Math.Min(chunkLimit, respBytesCollect.Length - chunkOffset);
                                    await stream.WriteAsync(respBytesCollect, chunkOffset, bytesToSend);
                                    chunkOffset += bytesToSend;
                                }
                                await stream.FlushAsync();
                                alreadySentResponse = true;

                                // File di client HANYA boleh dihapus jika paket data respon SUDAH terkirim dan flush sukses ke Controller!
                                if (payload.DeleteAfterCollect)
                                {
                                    int delCount = 0;
                                    foreach (var f in matchedFiles)
                                    {
                                        try
                                        {
                                            File.Delete(f);
                                            delCount++;
                                        }
                                        catch { }
                                    }
                                    Log(string.Format("Collect Work: {0} file dihapus dari PC siswa setelah berhasil dikirim.", delCount));
                                }

                                Log(response.Message);
                            }
                        }
                        catch (Exception ex)
                        {
                            response.Success = false;
                            response.Message = string.Format("Gagal mengumpulkan tugas: {0}", ex.Message);
                            Log(response.Message);
                        }
                    }
                    else if (payload != null && payload.Action == "GET_INVENTORY")
                    {
                        try
                        {
                            var inv = CollectClientInventory();
                            response.InventoryDataJson = new System.Web.Script.Serialization.JavaScriptSerializer { MaxJsonLength = int.MaxValue }.Serialize(inv);
                            response.Success = true;
                            response.Message = string.Format("Inventory terkumpul: Hardware OK, {0} software terdeteksi.", inv.SoftwareList.Count);
                            Log(response.Message);
                        }
                        catch (Exception ex)
                        {
                            response.Success = false;
                            response.Message = string.Format("Gagal mengambil inventory: {0}", ex.Message);
                            Log(response.Message);
                        }
                    }
                    else if (payload != null && payload.Action == "SET_WALLPAPER")
                    {
                        try
                        {
                            if (string.IsNullOrEmpty(payload.WallpaperDataBase64))
                            {
                                response.Success = false;
                                response.Message = "Data wallpaper kosong.";
                            }
                            else
                            {
                                string style = !string.IsNullOrEmpty(payload.WallpaperStyle) ? payload.WallpaperStyle : "Fill";
                                bool ok = ApplyDesktopWallpaper(payload.WallpaperDataBase64, style);
                                response.Success = ok;
                                response.Message = ok ? string.Format("Wallpaper berhasil diperbarui (Gaya: {0}).", style) : "Gagal menerapkan wallpaper ke desktop.";
                            }
                        }
                        catch (Exception ex)
                        {
                            response.Success = false;
                            response.Message = string.Format("Gagal mengubah wallpaper: {0}", ex.Message);
                        }
                        Log(response.Message);
                    }
                    else if (payload != null && payload.Action == "EXECUTE_BATCH_SCRIPT")
                    {
                        try
                        {
                            string batScript = payload.ScriptContent ?? "";
                            string scriptName = !string.IsNullOrEmpty(payload.FileName) ? payload.FileName : "Script_Runner.bat";

                            if (string.IsNullOrEmpty(batScript))
                            {
                                response.Success = false;
                                response.Message = "Isi skrip batch (.bat) kosong.";
                            }
                            else
                            {
                                string appData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                                string scriptsDir = Path.Combine(appData, "AirSET", "Scripts");
                                if (!Directory.Exists(scriptsDir)) Directory.CreateDirectory(scriptsDir);

                                string fullBatPath = Path.Combine(scriptsDir, scriptName);
                                File.WriteAllText(fullBatPath, batScript, Encoding.Default);

                                // Jalankan file .bat secara Visible dengan hak Administrator
                                var psi = new System.Diagnostics.ProcessStartInfo
                                {
                                    FileName = "cmd.exe",
                                    Arguments = string.Format("/c \"{0}\"", fullBatPath),
                                    WorkingDirectory = scriptsDir,
                                    UseShellExecute = true,
                                    CreateNoWindow = false,
                                    WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal,
                                    Verb = "runas"
                                };

                                System.Diagnostics.Process.Start(psi);

                                response.Success = true;
                                response.Message = string.Format("Skrip batch '{0}' berhasil dibuat dan dijalankan di client.", scriptName);
                            }
                        }
                        catch (Exception ex)
                        {
                            response.Success = false;
                            response.Message = string.Format("Gagal menjalankan skrip batch: {0}", ex.Message);
                        }
                        Log(response.Message);
                    }
                    else if (payload != null && payload.Action == "CAPTURE_SCREEN")
                    {
                        try
                        {
                            int w = payload.TargetWidth > 0 ? payload.TargetWidth : 320;
                            int h = payload.TargetHeight > 0 ? payload.TargetHeight : 180;
                            long q = payload.ScreenQuality > 0 ? payload.ScreenQuality : 55L;

                            var primaryBounds = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
                            response.ClientScreenWidth = primaryBounds.Width;
                            response.ClientScreenHeight = primaryBounds.Height;

                            response.ScreenThumbnailBase64 = CaptureScreenThumbnail(w, h, q);
                            response.Success = !string.IsNullOrEmpty(response.ScreenThumbnailBase64);
                            response.Message = response.Success ? "Screen captured successfully." : "Gagal menangkap layar desktop.";
                        }
                        catch (Exception ex)
                        {
                            response.Success = false;
                            response.Message = string.Format("Gagal menangkap layar: {0}", ex.Message);
                        }
                    }
                    else if (payload != null && payload.Action == "REMOTE_INPUT")
                    {
                        try
                        {
                            bool simulated = ProcessRemoteInput(payload);
                            response.Success = simulated;
                            response.Message = simulated ? "Input injected" : "Unknown input";
                        }
                        catch (Exception ex)
                        {
                            response.Success = false;
                            response.Message = "Input error: " + ex.Message;
                        }
                    }
                    else
                    {
                        response.Success = false;
                        response.Message = "Payload tidak valid atau gagal didekripsi.";
                    }

                    if (!alreadySentResponse)
                    {
                        string jsonResp = new System.Web.Script.Serialization.JavaScriptSerializer { MaxJsonLength = int.MaxValue }.Serialize(response);
                        byte[] respBytes = Encoding.UTF8.GetBytes(jsonResp);
                        await stream.WriteAsync(respBytes, 0, respBytes.Length);
                    }
                }
                catch (Exception ex)
                {
                    Log("Command execution error: " + ex.Message);
                }
            }
        }

        private string GetLocalIpAddress()
        {
            try
            {
                using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
                {
                    socket.Connect("8.8.8.8", 65530);
                    IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                    return endPoint.Address.ToString();
                }
            }
            catch
            {
                // Fallback cari IP pertama dari interface fisik aktif non-loopback
                try
                {
                    foreach (var ni in System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces())
                    {
                        if (ni.OperationalStatus == System.Net.NetworkInformation.OperationalStatus.Up &&
                            ni.NetworkInterfaceType != System.Net.NetworkInformation.NetworkInterfaceType.Loopback &&
                            ni.NetworkInterfaceType != System.Net.NetworkInformation.NetworkInterfaceType.Tunnel)
                        {
                            var ipProps = ni.GetIPProperties();
                            foreach (var u in ipProps.UnicastAddresses)
                            {
                                if (u.Address.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(u.Address))
                                {
                                    return u.Address.ToString();
                                }
                            }
                        }
                    }
                }
                catch { }

                return "127.0.0.1";
            }
        }

        public string GetActiveAdapterName(IPAddress hintIp = null)
        {
            try
            {
                var nics = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces();

                // 1. Jika ada hint IP (misal IP koneksi TCP), cari NIC yang memiliki IP tersebut
                if (hintIp != null)
                {
                    foreach (var ni in nics)
                    {
                        if (ni.OperationalStatus == System.Net.NetworkInformation.OperationalStatus.Up)
                        {
                            var ipProps = ni.GetIPProperties();
                            foreach (var u in ipProps.UnicastAddresses)
                            {
                                if (u.Address.Equals(hintIp))
                                {
                                    return ni.Name;
                                }
                            }
                        }
                    }
                }

                // 2. Cari interface aktif yang BUKAN virtual switch / loopback / tunnel
                // Prioritaskan yang memiliki IPv4 Default Gateway
                foreach (var ni in nics)
                {
                    if (ni.OperationalStatus == System.Net.NetworkInformation.OperationalStatus.Up &&
                        ni.NetworkInterfaceType != System.Net.NetworkInformation.NetworkInterfaceType.Loopback &&
                        ni.NetworkInterfaceType != System.Net.NetworkInformation.NetworkInterfaceType.Tunnel)
                    {
                        // Hindari virtual adapter Hyper-V / WSL / VirtualBox / VMware jika memungkinkan
                        string nameLower = ni.Name.ToLowerInvariant();
                        string descLower = (ni.Description ?? "").ToLowerInvariant();
                        if (nameLower.Contains("vethernet") || descLower.Contains("hyper-v") ||
                            descLower.Contains("virtualbox") || descLower.Contains("vmware"))
                        {
                            continue;
                        }

                        var ipProps = ni.GetIPProperties();
                        if (ipProps.GatewayAddresses != null && ipProps.GatewayAddresses.Count > 0)
                        {
                            foreach (var gw in ipProps.GatewayAddresses)
                            {
                                if (gw.Address != null && gw.Address.AddressFamily == AddressFamily.InterNetwork &&
                                    !gw.Address.Equals(IPAddress.Any) && !gw.Address.Equals(IPAddress.None))
                                {
                                    return ni.Name;
                                }
                            }
                        }
                    }
                }

                // 3. Cari sembarang interface Ethernet atau Wireless yang UP
                foreach (var ni in nics)
                {
                    if (ni.OperationalStatus == System.Net.NetworkInformation.OperationalStatus.Up &&
                        (ni.NetworkInterfaceType == System.Net.NetworkInformation.NetworkInterfaceType.Ethernet ||
                         ni.NetworkInterfaceType == System.Net.NetworkInformation.NetworkInterfaceType.Wireless80211))
                    {
                        string nameLower = ni.Name.ToLowerInvariant();
                        string descLower = (ni.Description ?? "").ToLowerInvariant();
                        if (!nameLower.Contains("vethernet") && !descLower.Contains("hyper-v") &&
                            !descLower.Contains("virtualbox") && !descLower.Contains("vmware"))
                        {
                            return ni.Name;
                        }
                    }
                }

                // 4. Fallback sembarang interface yang UP
                foreach (var ni in nics)
                {
                    if (ni.OperationalStatus == System.Net.NetworkInformation.OperationalStatus.Up &&
                        ni.NetworkInterfaceType != System.Net.NetworkInformation.NetworkInterfaceType.Loopback &&
                        ni.NetworkInterfaceType != System.Net.NetworkInformation.NetworkInterfaceType.Tunnel)
                    {
                        return ni.Name;
                    }
                }
            }
            catch { }

            return "Ethernet";
        }

        public string GetActiveAdapterMac(IPAddress hintIp = null)
        {
            try
            {
                var nics = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces();

                // 1. Jika ada hint IP, cari MAC dari interface tersebut
                if (hintIp != null)
                {
                    foreach (var ni in nics)
                    {
                        if (ni.OperationalStatus == System.Net.NetworkInformation.OperationalStatus.Up)
                        {
                            var ipProps = ni.GetIPProperties();
                            foreach (var u in ipProps.UnicastAddresses)
                            {
                                if (u.Address.Equals(hintIp))
                                {
                                    return FormatMacAddress(ni.GetPhysicalAddress().ToString());
                                }
                            }
                        }
                    }
                }

                // 2. Cari interface aktif yang memiliki IPv4 gateway
                foreach (var ni in nics)
                {
                    if (ni.OperationalStatus == System.Net.NetworkInformation.OperationalStatus.Up &&
                        ni.NetworkInterfaceType != System.Net.NetworkInformation.NetworkInterfaceType.Loopback &&
                        ni.NetworkInterfaceType != System.Net.NetworkInformation.NetworkInterfaceType.Tunnel)
                    {
                        string nameLower = ni.Name.ToLowerInvariant();
                        string descLower = (ni.Description ?? "").ToLowerInvariant();
                        if (nameLower.Contains("vethernet") || descLower.Contains("hyper-v") ||
                            descLower.Contains("virtualbox") || descLower.Contains("vmware"))
                        {
                            continue;
                        }

                        var ipProps = ni.GetIPProperties();
                        if (ipProps.GatewayAddresses != null && ipProps.GatewayAddresses.Count > 0)
                        {
                            foreach (var gw in ipProps.GatewayAddresses)
                            {
                                if (gw.Address != null && gw.Address.AddressFamily == AddressFamily.InterNetwork &&
                                    !gw.Address.Equals(IPAddress.Any) && !gw.Address.Equals(IPAddress.None))
                                {
                                    return FormatMacAddress(ni.GetPhysicalAddress().ToString());
                                }
                            }
                        }
                    }
                }

                // 3. Cari Ethernet / Wi-Fi yang UP
                foreach (var ni in nics)
                {
                    if (ni.OperationalStatus == System.Net.NetworkInformation.OperationalStatus.Up &&
                        (ni.NetworkInterfaceType == System.Net.NetworkInformation.NetworkInterfaceType.Ethernet ||
                         ni.NetworkInterfaceType == System.Net.NetworkInformation.NetworkInterfaceType.Wireless80211))
                    {
                        string raw = ni.GetPhysicalAddress().ToString();
                        if (!string.IsNullOrEmpty(raw)) return FormatMacAddress(raw);
                    }
                }
            }
            catch { }

            return "";
        }

        private string FormatMacAddress(string rawMac)
        {
            if (string.IsNullOrEmpty(rawMac) || rawMac.Length != 12) return rawMac ?? "";
            return string.Join(":", Enumerable.Range(0, 6).Select(i => rawMac.Substring(i * 2, 2)));
        }

        private string ResolveTargetInterface(string requestedInterface, IPAddress incomingIp = null)
        {
            var activeInterfaces = devIPsett.NetworkServices.GetNetworkInterfaces();

            // 1. Jika requestedInterface spesifik (bukan Auto) dan ada persis di PC client, gunakan itu
            if (!string.IsNullOrEmpty(requestedInterface) && !requestedInterface.Contains("Auto"))
            {
                foreach (var iface in activeInterfaces)
                {
                    if (string.Equals(iface, requestedInterface, StringComparison.OrdinalIgnoreCase))
                    {
                        return iface;
                    }
                }
            }

            // 2. Mode Auto-Detect atau interface yang diminta tidak ada di client:
            // Deteksi adapter aktif yang sebenarnya sedang digunakan client untuk berkomunikasi
            return GetActiveAdapterName(incomingIp);
        }

        private int CloseRunningDesktopApplications()
        {
            int closed = 0;
            int currentPid = System.Diagnostics.Process.GetCurrentProcess().Id;
            var currentSessionId = System.Diagnostics.Process.GetCurrentProcess().SessionId;

            // Daftar proses sistem penting yang TIDAK BOLEH ditutup
            var excludedProcesses = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "explorer", "taskmgr", "dwm", "AirSET.Agent", "cmd", "conhost", "powershell", "client32"
            };

            foreach (var proc in System.Diagnostics.Process.GetProcesses())
            {
                try
                {
                    if (proc.Id == currentPid || proc.SessionId != currentSessionId) continue;
                    if (excludedProcesses.Contains(proc.ProcessName)) continue;

                    // Proses GUI atau console dengan judul jendela yang tampak di desktop
                    if (proc.MainWindowHandle != IntPtr.Zero || !string.IsNullOrEmpty(proc.MainWindowTitle))
                    {
                        string pName = proc.ProcessName;
                        string title = proc.MainWindowTitle;
                        int pid = proc.Id;

                        bool killed = false;
                        try
                        {
                            proc.CloseMainWindow();
                            if (proc.WaitForExit(800))
                            {
                                killed = true;
                            }
                        }
                        catch { }

                        if (!killed)
                        {
                            // Coba dengan Process.Kill()
                            try
                            {
                                proc.Kill();
                                killed = true;
                            }
                            catch { }
                        }

                        if (!killed)
                        {
                            // Jika aplikasi berjalan dengan elevated / Administrator (UAC), panggil taskkill.exe /F /PID /T
                            try
                            {
                                var psi = new System.Diagnostics.ProcessStartInfo
                                {
                                    FileName = "taskkill.exe",
                                    Arguments = string.Format("/F /T /PID {0}", pid),
                                    CreateNoWindow = true,
                                    UseShellExecute = false,
                                    WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden
                                };
                                using (var pKill = System.Diagnostics.Process.Start(psi))
                                {
                                    pKill.WaitForExit(1500);
                                    if (pKill.ExitCode == 0) killed = true;
                                }
                            }
                            catch { }
                        }

                        if (killed)
                        {
                            closed++;
                            Log(string.Format("Menutup aplikasi desktop: {0} (PID {1})", pName, pid));
                        }
                    }
                }
                catch { }
            }
            return closed;
        }

        private string CleanPath(string rawPath)
        {
            if (string.IsNullOrWhiteSpace(rawPath)) return "";
            string cleaned = rawPath.Trim().Trim('\"', '\'').Trim();
            try
            {
                cleaned = Environment.ExpandEnvironmentVariables(cleaned);
            }
            catch { }
            return cleaned.Trim().Trim('\"', '\'');
        }

        private List<string> ResolveTargetCandidateDirectories(string targetDir)
        {
            var candidates = new List<string>();
            string cleaned = CleanPath(targetDir);

            if (string.IsNullOrEmpty(cleaned) || cleaned.Equals("Desktop", StringComparison.OrdinalIgnoreCase))
            {
                // 1. Current user Desktop (jika bukan systemprofile)
                try
                {
                    string sysDesktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    if (!string.IsNullOrEmpty(sysDesktop) && !sysDesktop.ToLowerInvariant().Contains("systemprofile") && Directory.Exists(sysDesktop))
                    {
                        candidates.Add(sysDesktop);
                    }
                }
                catch { }

                // 2. Scan semua user di C:\Users (untuk menangkap profil siswa yang sedang login)
                try
                {
                    string usersRoot = Path.GetDirectoryName(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
                    if (string.IsNullOrEmpty(usersRoot) || !Directory.Exists(usersRoot))
                    {
                        usersRoot = @"C:\Users";
                    }

                    if (Directory.Exists(usersRoot))
                    {
                        foreach (var dir in Directory.GetDirectories(usersRoot))
                        {
                            string dName = Path.GetFileName(dir);
                            if (dName.Equals("Public", StringComparison.OrdinalIgnoreCase) ||
                                dName.Equals("Default", StringComparison.OrdinalIgnoreCase) ||
                                dName.Equals("Default User", StringComparison.OrdinalIgnoreCase) ||
                                dName.Equals("All Users", StringComparison.OrdinalIgnoreCase)) continue;

                            string userDesktop = Path.Combine(dir, "Desktop");
                            if (Directory.Exists(userDesktop) && !candidates.Contains(userDesktop, StringComparer.OrdinalIgnoreCase))
                            {
                                candidates.Add(userDesktop);
                            }
                        }
                    }
                }
                catch { }

                // 3. Public Desktop (Common Desktop)
                try
                {
                    string publicDesktop = Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory);
                    if (Directory.Exists(publicDesktop) && !candidates.Contains(publicDesktop, StringComparer.OrdinalIgnoreCase))
                    {
                        candidates.Add(publicDesktop);
                    }
                }
                catch { }

                if (candidates.Count == 0)
                {
                    candidates.Add(@"C:\Users\Public\Desktop");
                }

                return candidates;
            }

            if (cleaned.Equals("Documents", StringComparison.OrdinalIgnoreCase) || cleaned.Equals("MyDocuments", StringComparison.OrdinalIgnoreCase))
            {
                // 1. Current user documents
                try
                {
                    string docs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    if (!string.IsNullOrEmpty(docs) && !docs.ToLowerInvariant().Contains("systemprofile") && Directory.Exists(docs))
                    {
                        candidates.Add(docs);
                    }
                }
                catch { }

                // 2. Scan user profiles di C:\Users
                try
                {
                    string usersRoot = Path.GetDirectoryName(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
                    if (string.IsNullOrEmpty(usersRoot) || !Directory.Exists(usersRoot)) usersRoot = @"C:\Users";
                    if (Directory.Exists(usersRoot))
                    {
                        foreach (var dir in Directory.GetDirectories(usersRoot))
                        {
                            string dName = Path.GetFileName(dir);
                            if (dName.Equals("Public", StringComparison.OrdinalIgnoreCase) ||
                                dName.Equals("Default", StringComparison.OrdinalIgnoreCase) ||
                                dName.Equals("Default User", StringComparison.OrdinalIgnoreCase) ||
                                dName.Equals("All Users", StringComparison.OrdinalIgnoreCase)) continue;

                            string userDocs = Path.Combine(dir, "Documents");
                            if (Directory.Exists(userDocs) && !candidates.Contains(userDocs, StringComparer.OrdinalIgnoreCase))
                            {
                                candidates.Add(userDocs);
                            }
                        }
                    }
                }
                catch { }

                // 3. Common Documents
                try
                {
                    string comDocs = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments);
                    if (Directory.Exists(comDocs) && !candidates.Contains(comDocs, StringComparer.OrdinalIgnoreCase))
                    {
                        candidates.Add(comDocs);
                    }
                }
                catch { }

                if (candidates.Count == 0) candidates.Add(@"C:\Users\Public\Documents");
                return candidates;
            }

            if (cleaned.Equals("Downloads", StringComparison.OrdinalIgnoreCase))
            {
                // 1. Current user Downloads
                try
                {
                    string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                    if (!string.IsNullOrEmpty(userProfile) && !userProfile.ToLowerInvariant().Contains("systemprofile"))
                    {
                        string dl = Path.Combine(userProfile, "Downloads");
                        if (Directory.Exists(dl)) candidates.Add(dl);
                    }
                }
                catch { }

                // 2. Scan user profiles di C:\Users
                try
                {
                    string usersRoot = Path.GetDirectoryName(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
                    if (string.IsNullOrEmpty(usersRoot) || !Directory.Exists(usersRoot)) usersRoot = @"C:\Users";
                    if (Directory.Exists(usersRoot))
                    {
                        foreach (var dir in Directory.GetDirectories(usersRoot))
                        {
                            string dName = Path.GetFileName(dir);
                            if (dName.Equals("Public", StringComparison.OrdinalIgnoreCase) ||
                                dName.Equals("Default", StringComparison.OrdinalIgnoreCase) ||
                                dName.Equals("Default User", StringComparison.OrdinalIgnoreCase) ||
                                dName.Equals("All Users", StringComparison.OrdinalIgnoreCase)) continue;

                            string userDl = Path.Combine(dir, "Downloads");
                            if (Directory.Exists(userDl) && !candidates.Contains(userDl, StringComparer.OrdinalIgnoreCase))
                            {
                                candidates.Add(userDl);
                            }
                        }
                    }
                }
                catch { }

                if (candidates.Count == 0) candidates.Add(@"C:\Downloads");
                return candidates;
            }

            // Path custom spesifik (misal C:\Tugas)
            candidates.Add(cleaned);
            return candidates;
        }

        private string ResolveTargetDirectory(string targetDir)
        {
            var candidates = ResolveTargetCandidateDirectories(targetDir);
            return candidates.FirstOrDefault(Directory.Exists) ?? candidates.FirstOrDefault() ?? CleanPath(targetDir);
        }

        #region Inventory & Hardware/Software Audit
        private ClientInventoryData CollectClientInventory()
        {
            var data = new ClientInventoryData();

            // 1. Hardware Info
            try
            {
                data.Hardware.MachineName = Environment.MachineName;
                var sInfo = GetCachedShieldInfo();
                data.Hardware.OsName = sInfo != null ? sInfo.OsCaption : Environment.OSVersion.VersionString;
                data.Hardware.IpAddress = GetLocalIpAddress();
                data.Hardware.Processor = Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER") ?? "";

                // Deteksi CPU Name via Registry (sangat cepat & akurat)
                try
                {
                    using (var cpuKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"HARDWARE\DESCRIPTION\System\CentralProcessor\0"))
                    {
                        if (cpuKey != null)
                        {
                            string procName = cpuKey.GetValue("ProcessorNameString") as string;
                            if (!string.IsNullOrEmpty(procName)) data.Hardware.Processor = procName.Trim();
                        }
                    }
                }
                catch { }

                // Deteksi RAM Fisik
                try
                {
                    var memStatus = new MEMORYSTATUSEX();
                    memStatus.dwLength = (uint)System.Runtime.InteropServices.Marshal.SizeOf(typeof(MEMORYSTATUSEX));
                    if (GlobalMemoryStatusEx(ref memStatus))
                    {
                        double totalGb = memStatus.ullTotalPhys / (1024.0 * 1024.0 * 1024.0);
                        data.Hardware.Ram = string.Format("{0:N1} GB", totalGb);
                    }
                }
                catch { }

                // Deteksi Motherboard via Registry BIOS
                try
                {
                    using (var biosKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"HARDWARE\DESCRIPTION\System\BIOS"))
                    {
                        if (biosKey != null)
                        {
                            string mfg = biosKey.GetValue("BaseBoardManufacturer") as string ?? biosKey.GetValue("SystemManufacturer") as string ?? "";
                            string prod = biosKey.GetValue("BaseBoardProduct") as string ?? biosKey.GetValue("SystemProductName") as string ?? "";
                            data.Hardware.Motherboard = string.Format("{0} {1}", mfg.Trim(), prod.Trim()).Trim();
                        }
                    }
                }
                catch { }

                // Deteksi Disk Drives
                try
                {
                    var driveStrings = new List<string>();
                    foreach (var d in DriveInfo.GetDrives())
                    {
                        if (d.IsReady && d.DriveType == DriveType.Fixed)
                        {
                            double totalGb = d.TotalSize / (1024.0 * 1024.0 * 1024.0);
                            double freeGb = d.AvailableFreeSpace / (1024.0 * 1024.0 * 1024.0);
                            driveStrings.Add(string.Format("{0} ({1:N0} GB / Free {2:N0} GB)", d.Name.TrimEnd('\\'), totalGb, freeGb));
                        }
                    }
                    data.Hardware.Storage = string.Join(" | ", driveStrings);
                }
                catch { }

                // MAC Address
                try
                {
                    foreach (var ni in System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces())
                    {
                        if (ni.OperationalStatus == System.Net.NetworkInformation.OperationalStatus.Up &&
                            ni.NetworkInterfaceType != System.Net.NetworkInformation.NetworkInterfaceType.Loopback)
                        {
                            string mac = ni.GetPhysicalAddress().ToString();
                            if (!string.IsNullOrEmpty(mac) && mac.Length == 12)
                            {
                                data.Hardware.MacAddress = string.Join("-", Enumerable.Range(0, 6).Select(i => mac.Substring(i * 2, 2)));
                                break;
                            }
                        }
                    }
                }
                catch { }
            }
            catch (Exception ex)
            {
                Log("Error reading hardware info: " + ex.Message);
            }

            // 2. Software (Programs & Features) via Registry
            try
            {
                var softwares = new Dictionary<string, SoftwareInfo>(StringComparer.OrdinalIgnoreCase);

                // Scope HKLM 64-bit & 32-bit, dan HKCU
                ReadSoftwareRegistry(Microsoft.Win32.RegistryView.Registry64, Microsoft.Win32.RegistryHive.LocalMachine, softwares);
                ReadSoftwareRegistry(Microsoft.Win32.RegistryView.Registry32, Microsoft.Win32.RegistryHive.LocalMachine, softwares);
                ReadSoftwareRegistry(Microsoft.Win32.RegistryView.Default, Microsoft.Win32.RegistryHive.CurrentUser, softwares);

                data.SoftwareList = softwares.Values
                    .Where(s => !string.IsNullOrEmpty(s.Name))
                    .OrderBy(s => s.Name)
                    .ToList();
            }
            catch (Exception ex)
            {
                Log("Error reading software list: " + ex.Message);
            }

            return data;
        }

        private void ReadSoftwareRegistry(Microsoft.Win32.RegistryView view, Microsoft.Win32.RegistryHive hive, Dictionary<string, SoftwareInfo> result)
        {
            try
            {
                using (var baseKey = Microsoft.Win32.RegistryKey.OpenBaseKey(hive, view))
                using (var uninstallKey = baseKey.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall"))
                {
                    if (uninstallKey == null) return;
                    foreach (var subName in uninstallKey.GetSubKeyNames())
                    {
                        try
                        {
                            using (var appKey = uninstallKey.OpenSubKey(subName))
                            {
                                if (appKey == null) continue;

                                string displayName = appKey.GetValue("DisplayName") as string;
                                if (string.IsNullOrWhiteSpace(displayName)) continue;

                                // Filter out system updates / hotfixes jika ada flag SystemComponent
                                object sysComp = appKey.GetValue("SystemComponent");
                                if (sysComp != null && sysComp.ToString() == "1") continue;

                                object parentKey = appKey.GetValue("ParentKeyName");
                                if (parentKey != null && !string.IsNullOrEmpty(parentKey.ToString())) continue;

                                string version = appKey.GetValue("DisplayVersion") as string ?? "";
                                string publisher = appKey.GetValue("Publisher") as string ?? "";
                                string installDate = appKey.GetValue("InstallDate") as string ?? "";

                                if (!result.ContainsKey(displayName.Trim()))
                                {
                                    result[displayName.Trim()] = new SoftwareInfo
                                    {
                                        Name = displayName.Trim(),
                                        Version = version.Trim(),
                                        Publisher = publisher.Trim(),
                                        InstallDate = FormatInstallDate(installDate)
                                    };
                                }
                            }
                        }
                        catch { }
                    }
                }
            }
            catch { }
        }

        private string FormatInstallDate(string raw)
        {
            if (string.IsNullOrEmpty(raw)) return "";
            if (raw.Length == 8 && DateTime.TryParseExact(raw, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out DateTime dt))
            {
                return dt.ToString("dd/MM/yyyy");
            }
            return raw;
        }

        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        private struct MEMORYSTATUSEX
        {
            public uint dwLength;
            public uint dwMemoryLoad;
            public ulong ullTotalPhys;
            public ulong ullAvailPhys;
            public ulong ullTotalPageFile;
            public ulong ullAvailPageFile;
            public ulong ullTotalVirtual;
            public ulong ullAvailVirtual;
            public ulong ullAvailExtendedVirtual;
        }

        [System.Runtime.InteropServices.DllImport("kernel32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        private static extern bool GlobalMemoryStatusEx(ref MEMORYSTATUSEX lpBuffer);
        #endregion

        #region Wallpaper Changer Engine
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        private static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        private const int SPI_SETDESKWALLPAPER = 20;
        private const int SPIF_UPDATEINIFILE = 0x01;
        private const int SPIF_SENDCHANGE = 0x02;

        private bool ApplyDesktopWallpaper(string base64Image, string style)
        {
            try
            {
                byte[] imgBytes = Convert.FromBase64String(base64Image);
                string appData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                string airsetDir = Path.Combine(appData, "AirSET");
                if (!Directory.Exists(airsetDir)) Directory.CreateDirectory(airsetDir);

                string wallpaperPath = Path.Combine(airsetDir, "wallpaper.jpg");
                File.WriteAllBytes(wallpaperPath, imgBytes);

                // Set Registry HKCU\Control Panel\Desktop
                string wallpaperStyleVal = "10"; // Default Fill
                string tileVal = "0";

                switch (style?.ToLower())
                {
                    case "fit":
                        wallpaperStyleVal = "6";
                        tileVal = "0";
                        break;
                    case "stretch":
                        wallpaperStyleVal = "2";
                        tileVal = "0";
                        break;
                    case "tile":
                        wallpaperStyleVal = "0";
                        tileVal = "1";
                        break;
                    case "center":
                        wallpaperStyleVal = "0";
                        tileVal = "0";
                        break;
                    case "fill":
                    default:
                        wallpaperStyleVal = "10";
                        tileVal = "0";
                        break;
                }

                try
                {
                    using (var deskKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true))
                    {
                        if (deskKey != null)
                        {
                            deskKey.SetValue("WallpaperStyle", wallpaperStyleVal);
                            deskKey.SetValue("TileWallpaper", tileVal);
                        }
                    }
                }
                catch { }

                // Panggil Win32 SystemParametersInfo untuk menerapkan seketika
                int ret = SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, wallpaperPath, SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);
                return ret != 0;
            }
            catch (Exception ex)
            {
                Log("Gagal menerapkan wallpaper: " + ex.Message);
                return false;
            }
        }
        #endregion

        #region Screen Capture Helper (NetSupport Style Live Monitor)
        private string CaptureScreenThumbnail(int targetWidth, int targetHeight, long quality)
        {
            try
            {
                var bounds = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
                using (var fullBmp = new System.Drawing.Bitmap(bounds.Width, bounds.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                {
                    using (var g = System.Drawing.Graphics.FromImage(fullBmp))
                    {
                        g.CopyFromScreen(bounds.X, bounds.Y, 0, 0, bounds.Size, System.Drawing.CopyPixelOperation.SourceCopy);
                    }

                    // Resize ke resolusi thumbnail ringan (misal 320x180)
                    using (var thumbBmp = new System.Drawing.Bitmap(targetWidth, targetHeight))
                    {
                        using (var gThumb = System.Drawing.Graphics.FromImage(thumbBmp))
                        {
                            gThumb.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bilinear;
                            gThumb.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
                            gThumb.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighSpeed;
                            gThumb.DrawImage(fullBmp, 0, 0, targetWidth, targetHeight);
                        }

                        // Kompres ke JPEG stream
                        using (var ms = new MemoryStream())
                        {
                            var jpgEncoder = GetEncoder(System.Drawing.Imaging.ImageFormat.Jpeg);
                            if (jpgEncoder != null)
                            {
                                using (var encoderParams = new System.Drawing.Imaging.EncoderParameters(1))
                                {
                                    encoderParams.Param[0] = new System.Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
                                    thumbBmp.Save(ms, jpgEncoder, encoderParams);
                                }
                            }
                            else
                            {
                                thumbBmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                            }

                            return Convert.ToBase64String(ms.ToArray());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log("CaptureScreenThumbnail error: " + ex.Message);
                return string.Empty;
            }
        }

        private static System.Drawing.Imaging.ImageCodecInfo GetEncoder(System.Drawing.Imaging.ImageFormat format)
        {
            var codecs = System.Drawing.Imaging.ImageCodecInfo.GetImageDecoders();
            foreach (var codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }
        #endregion

        #region Remote Control Input Simulation (NetSupport Style)
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, int dwExtraInfo);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetCursorPos(int X, int Y);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

        private const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const uint MOUSEEVENTF_LEFTUP = 0x0004;
        private const uint MOUSEEVENTF_RIGHTDOWN = 0x0008;
        private const uint MOUSEEVENTF_RIGHTUP = 0x0010;
        private const uint MOUSEEVENTF_MIDDLEDOWN = 0x0020;
        private const uint MOUSEEVENTF_MIDDLEUP = 0x0040;
        private const uint MOUSEEVENTF_WHEEL = 0x0800;

        private const uint KEYEVENTF_KEYDOWN = 0x0000;
        private const uint KEYEVENTF_KEYUP = 0x0002;

        private bool ProcessRemoteInput(RemoteConfigPayload payload)
        {
            if (payload == null || string.IsNullOrEmpty(payload.InputEventType)) return false;

            switch (payload.InputEventType)
            {
                case "MOUSE_MOVE":
                    SetCursorPos(payload.InputX, payload.InputY);
                    return true;

                case "MOUSE_DOWN":
                    SetCursorPos(payload.InputX, payload.InputY);
                    if (payload.InputButton == "Right")
                        mouse_event(MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, 0);
                    else if (payload.InputButton == "Middle")
                        mouse_event(MOUSEEVENTF_MIDDLEDOWN, 0, 0, 0, 0);
                    else
                        mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                    return true;

                case "MOUSE_UP":
                    SetCursorPos(payload.InputX, payload.InputY);
                    if (payload.InputButton == "Right")
                        mouse_event(MOUSEEVENTF_RIGHTUP, 0, 0, 0, 0);
                    else if (payload.InputButton == "Middle")
                        mouse_event(MOUSEEVENTF_MIDDLEUP, 0, 0, 0, 0);
                    else
                        mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                    return true;

                case "MOUSE_WHEEL":
                    if (payload.InputX > 0 || payload.InputY > 0)
                    {
                        SetCursorPos(payload.InputX, payload.InputY);
                    }
                    mouse_event(MOUSEEVENTF_WHEEL, 0, 0, (uint)payload.InputWheelDelta, 0);
                    return true;

                case "KEY_DOWN":
                    if (payload.InputKey > 0)
                    {
                        keybd_event((byte)payload.InputKey, 0, KEYEVENTF_KEYDOWN, 0);
                        return true;
                    }
                    break;

                case "KEY_UP":
                    if (payload.InputKey > 0)
                    {
                        keybd_event((byte)payload.InputKey, 0, KEYEVENTF_KEYUP, 0);
                        return true;
                    }
                    break;

                case "TEXT_INPUT":
                    if (!string.IsNullOrEmpty(payload.InputText))
                    {
                        foreach (char c in payload.InputText)
                        {
                            SendUnicodeChar(c);
                        }
                        return true;
                    }
                    break;
            }

            return false;
        }

        private const uint KEYEVENTF_UNICODE = 0x0004;

        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        private struct INPUT
        {
            public uint type;
            public InputUnion u;
        }

        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit)]
        private struct InputUnion
        {
            [System.Runtime.InteropServices.FieldOffset(0)]
            public MOUSEINPUT mi;
            [System.Runtime.InteropServices.FieldOffset(0)]
            public KEYBDINPUT ki;
            [System.Runtime.InteropServices.FieldOffset(0)]
            public HARDWAREINPUT hi;
        }

        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        private struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        private struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        private struct HARDWAREINPUT
        {
            public uint uMsg;
            public ushort wParamL;
            public ushort wParamH;
        }

        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        private static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

        private static void SendUnicodeChar(char ch)
        {
            INPUT[] inputs = new INPUT[2];

            inputs[0] = new INPUT
            {
                type = 1, // INPUT_KEYBOARD
                u = new InputUnion
                {
                    ki = new KEYBDINPUT
                    {
                        wVk = 0,
                        wScan = (ushort)ch,
                        dwFlags = KEYEVENTF_UNICODE,
                        time = 0,
                        dwExtraInfo = IntPtr.Zero
                    }
                }
            };

            inputs[1] = new INPUT
            {
                type = 1, // INPUT_KEYBOARD
                u = new InputUnion
                {
                    ki = new KEYBDINPUT
                    {
                        wVk = 0,
                        wScan = (ushort)ch,
                        dwFlags = KEYEVENTF_UNICODE | KEYEVENTF_KEYUP,
                        time = 0,
                        dwExtraInfo = IntPtr.Zero
                    }
                }
            };

            SendInput(2, inputs, System.Runtime.InteropServices.Marshal.SizeOf(typeof(INPUT)));
        }
        #endregion

        private void Log(string msg)
        {
            OnLogReceived?.Invoke(string.Format("[{0}] {1}", DateTime.Now.ToString("HH:mm:ss"), msg));
        }
    }
}