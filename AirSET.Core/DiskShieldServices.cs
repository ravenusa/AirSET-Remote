using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace devIPsett
{
    public class OsAndShieldInfo
    {
        public string OsCaption { get; set; }
        public bool IsEnterpriseOrEdu { get; set; }
        public string ShieldType { get; set; }   // "UWF" | "DeepFreeze" | "None"
        public string ShieldStatus { get; set; } // "Locked (UWF)" | "Unlocked (UWF)" | "UWF Not Installed" | "Frozen (DFC)" | "Thawed (DFC)" | "Not Protected"

        public OsAndShieldInfo()
        {
            OsCaption = "Windows";
            IsEnterpriseOrEdu = false;
            ShieldType = "None";
            ShieldStatus = "Not Protected";
        }
    }

    public static class DiskShieldServices
    {
        /// <summary>
        /// Mendeteksi edisi Windows dan status proteksi saat ini (UWF / Deep Freeze).
        /// </summary>
        public static OsAndShieldInfo DetectOsAndShieldStatus()
        {
            var info = new OsAndShieldInfo();

            // 1. Deteksi Edisi OS melalui Registry Native 64-Bit (Sangat Cepat & Akurat)
            try
            {
                using (var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
                using (var subKey = baseKey.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion"))
                {
                    if (subKey != null)
                    {
                        string prodName = subKey.GetValue("ProductName") as string;
                        string buildStr = (subKey.GetValue("CurrentBuildNumber") ?? subKey.GetValue("CurrentBuild")) as string;

                        if (!string.IsNullOrEmpty(prodName))
                        {
                            info.OsCaption = prodName;

                            // Microsoft secara sengaja mengunci nilai ProductName menjadi "Windows 10 ..." pada Windows 11 demi kompatibilitas aplikasi.
                            // Di Windows 11, Build Number selalu >= 22000.
                            if (int.TryParse(buildStr, out int buildNum) && buildNum >= 22000)
                            {
                                if (info.OsCaption.StartsWith("Windows 10", StringComparison.OrdinalIgnoreCase))
                                {
                                    info.OsCaption = "Windows 11" + info.OsCaption.Substring("Windows 10".Length);
                                }
                            }
                        }
                    }
                }
            }
            catch { }

            // Cek apakah Enterprise / Education / IoT
            string captionLower = info.OsCaption.ToLowerInvariant();
            info.IsEnterpriseOrEdu = captionLower.Contains("enterprise") || 
                                     captionLower.Contains("education") || 
                                     captionLower.Contains("iot");

            // Fokus 100% pada Microsoft Unified Write Filter (UWF) untuk seluruh edisi Windows
            string uwfPath = FindUwfmgrExecutable();
            // Cek keberadaan file eksekutabel uwfmgr.exe secara fisik di System32/Sysnative
            bool isUwfmgrPresent = !string.IsNullOrEmpty(uwfPath) && File.Exists(uwfPath);

            int regUwfEnabled = -1;
            try
            {
                using (var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
                using (var srvKey = baseKey.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\uwfvol"))
                {
                    if (srvKey != null)
                    {
                        using (var staticKey = srvKey.OpenSubKey(@"parameters\static\copy0"))
                        {
                            if (staticKey != null)
                            {
                                object val = staticKey.GetValue("UwfEnabled");
                                if (val != null) regUwfEnabled = Convert.ToInt32(val);
                            }
                        }
                    }
                }
            }
            catch { }

            // UWF hanya dianggap TERPASANG jika uwfmgr.exe ada secara fisik ATAU driver aktif dan ada file uwfmgr.exe
            if (isUwfmgrPresent)
            {
                info.ShieldType = "UWF";

                // Coba query real-time status filter via uwfmgr.exe get-config
                try
                {
                    var psi = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = uwfPath,
                        Arguments = "get-config",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };
                    using (var p = System.Diagnostics.Process.Start(psi))
                    {
                        string stdout = p.StandardOutput.ReadToEnd();
                        string stderr = p.StandardError.ReadToEnd();
                        p.WaitForExit(4000);

                        string combined = (stdout + " " + stderr).ToLowerInvariant();
                        if (combined.Contains("filter state"))
                        {
                            if (combined.Contains("filter state: on") || combined.Contains("filter state:   on"))
                            {
                                info.ShieldStatus = "Locked (UWF)";
                                return info;
                            }
                            else if (combined.Contains("filter state: off") || combined.Contains("filter state:   off"))
                            {
                                info.ShieldStatus = "Unlocked (UWF)";
                                return info;
                            }
                        }
                    }
                }
                catch { }

                // Fallback ke registry jika uwfmgr get-config tidak memberikan teks output
                if (regUwfEnabled == 1)
                {
                    info.ShieldStatus = "Locked (UWF)";
                }
                else
                {
                    info.ShieldStatus = "Unlocked (UWF)";
                }
                return info;
            }

            // Jika uwfmgr.exe tidak ada, fitur UWF sudah di-uninstall / tidak terpasang
            info.ShieldType = "UWF";
            info.ShieldStatus = "UWF Not Installed";
            return info;
        }

        /// <summary>
        /// Menginstal / mengaktifkan fitur Windows Unified Write Filter (UWF).
        /// Mendukung Windows 10/11 Enterprise/Edu serta Windows 10/11 Pro (melalui upgrade package/edition otomatis).
        /// </summary>
        public static async Task<SystemExecutionResult> InstallUwfFeatureAsync()
        {
            string dismExe = FindDismExecutable();
            // 1. Coba aktifkan fitur langsung via DISM (bekerja langsung di Enterprise/Edu/IoT)
            string args = "/Online /Enable-Feature /FeatureName:Client-UnifiedWriteFilter /All /NoRestart";
            var res = await NetworkServices.RunProcessAsync(dismExe, args, 120000);

            if (res.ExitCode == 0 || res.ExitCode == 3010)
            {
                res.Success = true;
                res.Error = null;
                return res;
            }

            // 2. Jika di Windows Pro (DISM gagal karena paket feature disembunyikan di edition Pro),
            // kita gunakan upgrade edition in-place ke Enterprise menggunakan Generic KMS Key resmi Microsoft
            // (NPPR9-FWDCX-D2C8J-H872K-2YT43). Ini instan tanpa instal ulang, membuka semua fitur Enterprise termasuk UWF!
            try
            {
                var checkEdition = await NetworkServices.RunProcessAsync("powershell.exe", "-NoProfile -Command \"(Get-ItemProperty 'HKLM:\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion').EditionID\"", 10000);
                string edition = (checkEdition.Output ?? "").Trim();

                if (edition.IndexOf("Pro", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    // Terapkan GVLK Enterprise resmi menggunakan cscript agar tidak memunculkan popup GUI slmgr
                    string slmgrScript = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "slmgr.vbs");
                    await NetworkServices.RunProcessAsync("cscript.exe", string.Format("//nologo \"{0}\" /ipk NPPR9-FWDCX-D2C8J-H872K-2YT43", slmgrScript), 60000);
                    
                    // Coba enable feature UWF kembali setelah edition key terpasang
                    res = await NetworkServices.RunProcessAsync(dismExe, args, 120000);
                    if (res.ExitCode == 0 || res.ExitCode == 3010)
                    {
                        res.Success = true;
                        res.Error = null;
                    }
                }
            }
            catch { }

            return res;
        }

        /// <summary>
        /// Meng-uninstall / menonaktifkan fitur Unified Write Filter (UWF) dari sistem Windows via DISM.
        /// </summary>
        public static async Task<SystemExecutionResult> UninstallUwfFeatureAsync()
        {
            // 1. Pastikan filter UWF di-disable terlebih dahulu jika uwfmgr.exe tersedia
            try
            {
                string exe = FindUwfmgrExecutable();
                if (!string.IsNullOrEmpty(exe) && File.Exists(exe))
                {
                    await NetworkServices.RunProcessAsync(exe, "filter disable", 10000);
                }
            }
            catch { }

            // 2. Hapus / Disable fitur Client-UnifiedWriteFilter melalui DISM
            string dismExe = FindDismExecutable();
            string args = "/Online /Disable-Feature /FeatureName:Client-UnifiedWriteFilter /NoRestart";
            var res = await NetworkServices.RunProcessAsync(dismExe, args, 120000);

            // Exit code 0 (sukses) atau 3010 (sukses butuh reboot)
            if (res.ExitCode == 0 || res.ExitCode == 3010)
            {
                res.Success = true;
                res.Error = null;

                // Bersihkan orphan registry parameters\static\copy0\UwfEnabled agar status tidak tertipu "Unlocked"
                try
                {
                    using (var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
                    using (var srvKey = baseKey.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\uwfvol\parameters\static\copy0", true))
                    {
                        if (srvKey != null)
                        {
                            srvKey.SetValue("UwfEnabled", 0, RegistryValueKind.DWord);
                        }
                    }
                }
                catch { }
            }

            return res;
        }

        public static string FindDismExecutable()
        {
            string windir = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
            string sysnative = Path.Combine(windir, "Sysnative", "dism.exe");
            if (File.Exists(sysnative)) return sysnative;

            string system32 = Path.Combine(windir, "System32", "dism.exe");
            if (File.Exists(system32)) return system32;

            return "dism.exe";
        }

        public static string FindUwfmgrExecutable()
        {
            // Pada Windows 64-bit jika aplikasi berjalan sebagai 32-bit (SysWOW64),
            // direktori System32 dialihkan sehingga uwfmgr.exe tidak ditemukan.
            // Gunakan alias Sysnative untuk mengakses System32 64-bit native secara langsung!
            string windir = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
            string sysnative = Path.Combine(windir, "Sysnative", "uwfmgr.exe");
            if (File.Exists(sysnative)) return sysnative;

            string system32 = Path.Combine(windir, "System32", "uwfmgr.exe");
            if (File.Exists(system32)) return system32;

            return null;
        }

        /// <summary>
        /// Mengonfigurasi overlay UWF (Disk 100GB, Thresholds), memproteksi Drive C:, dan mengaktifkan filter (Lock Disk).
        /// Jika settingan sebelumnya sudah ada, nilai ini akan langsung menimpa (overwrite) ke nilai baru.
        /// </summary>
        public static async Task<SystemExecutionResult> EnableUwfAsync()
        {
            string exe = FindUwfmgrExecutable();
            if (string.IsNullOrEmpty(exe))
            {
                return new SystemExecutionResult
                {
                    Success = false,
                    Error = "Fitur Unified Write Filter (UWF) belum terpasang di komputer client! Silakan klik tombol '⚙️ Install / Enable UWF' terlebih dahulu di Controller."
                };
            }

            // 1. Matikan Hibernation & Fast Startup secara total di level kernel (non-critical, fail-safe)
            try { await NetworkServices.RunProcessAsync("powercfg.exe", "/h off", 5000); } catch { }

            // 2. Pause & Nonaktifkan Windows Update secara permanen hingga tahun 2050
            // Ini mencegah background update, pending reboot, dan bootloop korup saat proteksi disk aktif
            try
            {
                // A. Registry UX Settings Pause sampai 2050
                using (var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
                {
                    using (var uxKey = baseKey.CreateSubKey(@"SOFTWARE\Microsoft\WindowsUpdate\UX\Settings"))
                    {
                        if (uxKey != null)
                        {
                            uxKey.SetValue("PauseFeatureUpdatesStartTime", "2026-01-01T00:00:00Z", RegistryValueKind.String);
                            uxKey.SetValue("PauseFeatureUpdatesEndTime", "2050-12-31T23:59:59Z", RegistryValueKind.String);
                            uxKey.SetValue("PauseQualityUpdatesStartTime", "2026-01-01T00:00:00Z", RegistryValueKind.String);
                            uxKey.SetValue("PauseQualityUpdatesEndTime", "2050-12-31T23:59:59Z", RegistryValueKind.String);
                            uxKey.SetValue("PauseUpdatesStartTime", "2026-01-01T00:00:00Z", RegistryValueKind.String);
                            uxKey.SetValue("PauseUpdatesExpiryTime", "2050-12-31T23:59:59Z", RegistryValueKind.String);
                        }
                    }

                    // B. Policy NoAutoUpdate & AUOptions = 2 (Notify only)
                    using (var auKey = baseKey.CreateSubKey(@"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate\AU"))
                    {
                        if (auKey != null)
                        {
                            auKey.SetValue("NoAutoUpdate", 1, RegistryValueKind.DWord);
                            auKey.SetValue("AUOptions", 2, RegistryValueKind.DWord);
                        }
                    }
                }

                // C. Pastikan services berada pada mode manual (demand) dan hentikan proses yang sedang berjalan
                // Mode 'demand' memungkinkan UI Settings membaca status Pause dan tombol Resume tanpa memunculkan error 'Something went wrong',
                // sementara Policy NoAutoUpdate & UX Settings menjamin Windows tidak akan mendownload update di background.
                await NetworkServices.RunProcessAsync("sc.exe", "config wuauserv start= demand", 4000);
                await NetworkServices.RunProcessAsync("sc.exe", "stop wuauserv", 4000);
                await NetworkServices.RunProcessAsync("sc.exe", "config UsoSvc start= demand", 4000);
                await NetworkServices.RunProcessAsync("sc.exe", "stop UsoSvc", 4000);
                await NetworkServices.RunProcessAsync("sc.exe", "config WaaSMedicSvc start= demand", 4000);
                await NetworkServices.RunProcessAsync("sc.exe", "stop WaaSMedicSvc", 4000);
            }
            catch { }

            // 3. Nonaktifkan Hibernasi & Fast Startup secara TOTAL
            // FAST STARTUP adalah penyebab utama BSOD uwfs.sys (0x3B) saat mode DISK Overlay aktif!
            try
            {
                await NetworkServices.RunProcessAsync("powercfg.exe", "/h off", 5000);
                using (var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
                using (var pwrKey = baseKey.CreateSubKey(@"SYSTEM\CurrentControlSet\Control\Session Manager\Power"))
                {
                    if (pwrKey != null)
                    {
                        pwrKey.SetValue("HiberbootEnabled", 0, RegistryValueKind.DWord);
                    }
                }
            }
            catch { }

            // 4. Nonaktifkan Volume Shadow Copy / System Restore pada Drive C: (incompatible dengan UWF)
            try
            {
                await NetworkServices.RunProcessAsync("powershell.exe", "-NoProfile -ExecutionPolicy Bypass -Command \"Disable-ComputerRestore -Drive 'C:\\' -ErrorAction SilentlyContinue\"", 10000);
            }
            catch { }

            // 5. Bersihkan Exclusion yang berbahaya bagi DISK Overlay (pagefile.sys & swapfile.sys)
            // Jika sebelumnya PC pernah mendaftarkan kedua file ini, harus dihapus otomatis agar tidak tabrakan di kernel
            try { await NetworkServices.RunProcessAsync(exe, "file remove-exclusion \"C:\\pagefile.sys\"", 5000); } catch { }
            try { await NetworkServices.RunProcessAsync(exe, "file remove-exclusion \"C:\\swapfile.sys\"", 5000); } catch { }

            // 6. Daftarkan File & Folder Exclusions Penting untuk integritas Bootloader & Driver
            string[] exclusions = new string[]
            {
                @"C:\Windows\System32\winevt\Logs",
                @"C:\Windows\Panther",
                @"C:\Program Files\Windows Defender",
                @"C:\ProgramData\Microsoft\Windows Defender",
                @"C:\Boot",
                @"C:\EFI"
            };

            foreach (var exc in exclusions)
            {
                try
                {
                    await NetworkServices.RunProcessAsync(exe, string.Format("file add-exclusion \"{0}\"", exc), 5000);
                }
                catch { }
            }

            // Daftarkan Registry Exclusions standar sistem waktu dan time-zone
            string[] regExclusions = new string[]
            {
                @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\W32Time",
                @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\TimeZoneInformation"
            };

            foreach (var reg in regExclusions)
            {
                try
                {
                    await NetworkServices.RunProcessAsync(exe, string.Format("registry add-exclusion \"{0}\"", reg), 5000);
                }
                catch { }
            }

            // 6. Proteksi Volume Drive C:
            try { await NetworkServices.RunProcessAsync(exe, "volume protect C:", 10000); } catch { }

            // 7. Hitung Ukuran Overlay Dinamis (10% Kapasitas Disk C:, 50% Warning, 80% Critical)
            string overlayLog = string.Empty;
            try
            {
                var driveC = new DriveInfo("C");
                if (driveC.IsReady)
                {
                    long totalMb = driveC.TotalSize / (1024 * 1024);
                    long freeMb = driveC.AvailableFreeSpace / (1024 * 1024);

                    // 10% dari total size, minimal 1024 MB (1 GB)
                    long overlayMb = Math.Max(1024, (long)(totalMb * 0.10));

                    // Safety guard: Jangan melebihi 70% dari sisa ruang kosong saat ini
                    if (overlayMb > (long)(freeMb * 0.70))
                    {
                        overlayMb = Math.Max(1024, (long)(freeMb * 0.50));
                    }

                    // 50% warning threshold & 80% critical threshold dari overlay size
                    long warnMb = Math.Max(512, (long)(overlayMb * 0.50));
                    long critMb = Math.Max(768, (long)(overlayMb * 0.80));

                    overlayLog = string.Format(" [C: {0}GB | Overlay DISK: {1}MB | Warn: {2}MB | Crit: {3}MB]", totalMb / 1024, overlayMb, warnMb, critMb);

                    // Terapkan konfigurasi overlay DISK (Kapasitas luas untuk Adobe & aplikasi berat)
                    await NetworkServices.RunProcessAsync(exe, "overlay set-type disk", 5000);
                    await NetworkServices.RunProcessAsync(exe, string.Format("overlay set-size {0}", overlayMb), 5000);
                    await NetworkServices.RunProcessAsync(exe, string.Format("overlay set-warningthreshold {0}", warnMb), 5000);
                    await NetworkServices.RunProcessAsync(exe, string.Format("overlay set-criticalthreshold {0}", critMb), 5000);
                }
            }
            catch { }

            // 7. Aktifkan Filter UWF (Utama)
            var resEnable = await NetworkServices.RunProcessAsync(exe, "filter enable", 15000);
            
            // Windows uwfmgr terkadang mengembalikan exit code non-zero jika sudah aktif, atau menghasilkan output informational.
            // Cek apakah output mengindikasikan filter sudah aktif, atau periksa registry static copy0 UwfEnabled.
            string outText = ((resEnable.Output ?? "") + " " + (resEnable.Error ?? "")).ToLowerInvariant();
            bool isSuccessKeywords = outText.Contains("enabled") || outText.Contains("success") || 
                                     outText.Contains("restart") || outText.Contains("filter state") || 
                                     outText.Contains("already") || outText.Contains("aktif") ||
                                     outText.Contains("reboot");

            // Cek langsung ke Registry apakah UwfEnabled static copy0 diset ke 1 (dipastikan Locked setelah restart)
            bool isRegistryLocked = false;
            try
            {
                using (var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
                using (var sKey = baseKey.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\uwfvol\parameters\static\copy0"))
                {
                    if (sKey != null)
                    {
                        object v = sKey.GetValue("UwfEnabled");
                        if (v != null && Convert.ToInt32(v) == 1) isRegistryLocked = true;
                    }
                }
            }
            catch { }

            if (resEnable.ExitCode == 0 || isSuccessKeywords || isRegistryLocked)
            {
                resEnable.Success = true;
                resEnable.Error = null; // Bersihkan error agar tidak dianggap gagal
            }

            if (!string.IsNullOrEmpty(overlayLog))
            {
                resEnable.Output = (resEnable.Output ?? "") + overlayLog;
            }

            return resEnable;
        }

        /// <summary>
        /// Menonaktifkan UWF (Unlock Disk / Thaw).
        /// </summary>
        public static async Task<SystemExecutionResult> DisableUwfAsync()
        {
            string exe = FindUwfmgrExecutable();
            if (string.IsNullOrEmpty(exe))
            {
                return new SystemExecutionResult
                {
                    Success = false,
                    Error = "Fitur Unified Write Filter (UWF) belum terpasang di komputer client! Silakan klik tombol '⚙️ Install / Enable UWF' terlebih dahulu di Controller."
                };
            }

            var resDisable = await NetworkServices.RunProcessAsync(exe, "filter disable", 10000);
            string outText = ((resDisable.Output ?? "") + " " + (resDisable.Error ?? "")).ToLowerInvariant();
            bool isSuccessKeywords = outText.Contains("disabled") || outText.Contains("success") || 
                                     outText.Contains("restart") || outText.Contains("filter state") || 
                                     outText.Contains("already") || outText.Contains("nonaktif") ||
                                     outText.Contains("reboot");

            bool isRegistryUnlocked = false;
            try
            {
                using (var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
                using (var sKey = baseKey.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\uwfvol\parameters\static\copy0"))
                {
                    if (sKey != null)
                    {
                        object v = sKey.GetValue("UwfEnabled");
                        if (v != null && Convert.ToInt32(v) == 0) isRegistryUnlocked = true;
                    }
                }
            }
            catch { }

            if (resDisable.ExitCode == 0 || isSuccessKeywords || isRegistryUnlocked)
            {
                resDisable.Success = true;
                resDisable.Error = null;
            }
            return resDisable;
        }

        public static string FindDfcExecutable()
        {
            // 1. Cek jika DFC.exe ada di folder aplikasi saat ini
            string localPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DFC.exe");
            if (File.Exists(localPath)) return localPath;

            // 2. Cek lokasi instalasi Faronics standar
            string[] searchPaths = new string[]
            {
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), @"Faronics\Deep Freeze\Install C-0\DFC.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"Faronics\Deep Freeze\Install C-0\DFC.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), @"Faronics\Deep Freeze\Off-Line\DFC.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"Faronics\Deep Freeze\Off-Line\DFC.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), @"Faronics\Deep Freeze 6\DFC.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"Faronics\Deep Freeze 6\DFC.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "DFC.exe"),
                @"C:\Windows\DFC.exe",
                @"C:\Program Files (x86)\Faronics\Deep Freeze\Install C-0\DFC.exe",
                @"C:\Program Files (x86)\Faronics\Deep Freeze\DFC.exe",
                @"C:\Program Files\Faronics\Deep Freeze\DFC.exe"
            };

            foreach (var p in searchPaths)
            {
                try
                {
                    if (File.Exists(p)) return p;
                }
                catch { }
            }

            // 3. Cek apakah ada di System PATH
            try
            {
                string pathEnv = Environment.GetEnvironmentVariable("PATH");
                if (!string.IsNullOrEmpty(pathEnv))
                {
                    foreach (var folder in pathEnv.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        string fullPath = Path.Combine(folder.Trim(), "DFC.exe");
                        if (File.Exists(fullPath)) return fullPath;
                    }
                }
            }
            catch { }

            // Tidak ditemukan di disk
            return null;
        }

        /// <summary>
        /// Mengeksekusi Deep Freeze CLI untuk Freeze (Lock Disk).
        /// </summary>
        public static async Task<SystemExecutionResult> FreezeDeepFreezeAsync(string password)
        {
            string dfcExe = FindDfcExecutable();
            if (string.IsNullOrEmpty(dfcExe))
            {
                return new SystemExecutionResult
                {
                    Success = false,
                    Error = "File 'DFC.exe' (Deep Freeze Command Line Control) tidak ditemukan di komputer client! Silakan letakkan file DFC.exe di satu folder dengan AirSET.Agent.exe atau di C:\\Windows."
                };
            }

            string cleanPass = (password ?? "").Trim().Replace("\"", "");
            string args = string.IsNullOrEmpty(cleanPass) ? "/BOOTFROZEN" : string.Format("\"{0}\" /BOOTFROZEN", cleanPass);
            var res = await NetworkServices.RunProcessAsync(dfcExe, args, 10000);

            // DFC Exit Code Mapping untuk perintah aksi (/BOOTFROZEN, /BOOTTHAWED):
            // ExitCode 0 adalah SATU-SATUNYA kode keberhasilan eksekusi perubahan boot state!
            // Kode non-nol (>0) adalah indikasi error / penolakan driver:
            // 1: Query True (bukan untuk perubahan state, jika muncul pada aksi berarti ditolak/tidak berubah)
            // 2: Administrator rights required
            // 3: Command Line Control disabled di workstation Deep Freeze
            // 4: Syntax / parameter tidak valid
            // 5+: Password salah atau driver menolak perintah
            if (res.ExitCode == 0)
            {
                res.Success = true;
                res.Output = string.Format("DFC BootFrozen sukses (ExitCode: {0}). Output: {1}", res.ExitCode, res.Output);
            }
            else
            {
                res.Success = false;
                string reason = GetDfcErrorDescription(res.ExitCode);
                res.Error = string.Format("DFC Gagal (ExitCode: {0}) - {1}. Detail: {2} {3}", res.ExitCode, reason, res.Error, res.Output).Trim();
            }

            return res;
        }

        /// <summary>
        /// Mengeksekusi Deep Freeze CLI untuk Thaw (Unlock Disk).
        /// </summary>
        public static async Task<SystemExecutionResult> ThawDeepFreezeAsync(string password)
        {
            string dfcExe = FindDfcExecutable();
            if (string.IsNullOrEmpty(dfcExe))
            {
                return new SystemExecutionResult
                {
                    Success = false,
                    Error = "File 'DFC.exe' (Deep Freeze Command Line Control) tidak ditemukan di komputer client! Silakan letakkan file DFC.exe di satu folder dengan AirSET.Agent.exe atau di C:\\Windows."
                };
            }

            string cleanPass = (password ?? "").Trim().Replace("\"", "");
            string args = string.IsNullOrEmpty(cleanPass) ? "/BOOTTHAWED" : string.Format("\"{0}\" /BOOTTHAWED", cleanPass);
            var res = await NetworkServices.RunProcessAsync(dfcExe, args, 10000);

            if (res.ExitCode == 0)
            {
                res.Success = true;
                res.Output = string.Format("DFC BootThawed sukses (ExitCode: {0}). Output: {1}", res.ExitCode, res.Output);
            }
            else
            {
                res.Success = false;
                string reason = GetDfcErrorDescription(res.ExitCode);
                res.Error = string.Format("DFC Gagal (ExitCode: {0}) - {1}. Detail: {2} {3}", res.ExitCode, reason, res.Error, res.Output).Trim();
            }

            return res;
        }

        private static string GetDfcErrorDescription(int exitCode)
        {
            switch (exitCode)
            {
                case 1:
                    return "Status tidak berubah / DFC mengembalikan kode 1 (Perintah diabaikan atau gagal)";
                case 2:
                    return "Diperlukan Hak Akses Administrator (Run as Administrator)";
                case 3:
                    return "Opsi 'Disable Command Line Control' aktif di Deep Freeze Client (Buka Deep Freeze Administrator -> uncheck Disable Command Line Control)";
                case 4:
                    return "Format perintah / parameter DFC tidak valid";
                default:
                    return "Password Deep Freeze salah atau driver Deep Freeze menolak perintah";
            }
        }
    }
}
