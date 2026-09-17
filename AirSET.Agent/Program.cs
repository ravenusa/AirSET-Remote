using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace AirSET.Agent
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (resolveSender, resolveArgs) =>
            {
                string assemblyName = new AssemblyName(resolveArgs.Name).Name;
                if (assemblyName.Equals("AirSET.Core", StringComparison.OrdinalIgnoreCase))
                {
                    // 1. Coba load dari embedded resource di dalam .exe
                    var currentAssembly = Assembly.GetExecutingAssembly();
                    foreach (string resName in currentAssembly.GetManifestResourceNames())
                    {
                        if (resName.EndsWith("AirSET.Core.dll", StringComparison.OrdinalIgnoreCase))
                        {
                            using (Stream stream = currentAssembly.GetManifestResourceStream(resName))
                            {
                                if (stream != null)
                                {
                                    byte[] assemblyData = new byte[stream.Length];
                                    stream.Read(assemblyData, 0, assemblyData.Length);
                                    return Assembly.Load(assemblyData);
                                }
                            }
                        }
                    }

                    // 2. Fallback: load jika ada file fisik di samping .exe
                    string diskPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AirSET.Core.dll");
                    if (File.Exists(diskPath))
                    {
                        return Assembly.LoadFrom(diskPath);
                    }
                }
                return null;
            };

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Validasi kunci enkripsi wajib disetel sebelum Agent berjalan
            string secretKey = Environment.GetEnvironmentVariable("AIRSET_SECRET_KEY");
            if (string.IsNullOrWhiteSpace(secretKey))
            {
                MessageBox.Show(
                    "Environment variable 'AIRSET_SECRET_KEY' belum disetel pada komputer client ini.\n\n" +
                    "Agent tidak dapat berkomunikasi secara aman tanpa kunci enkripsi bersama.\n" +
                    "Silakan setel environment variable 'AIRSET_SECRET_KEY' pada sistem.",
                    "AirSET Agent - Kunci Belum Dikonfigurasi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            string[] cmdArgs = Environment.GetCommandLineArgs();
            bool isWatchdog = cmdArgs.Length > 1 && cmdArgs[1].Equals("--watchdog", StringComparison.OrdinalIgnoreCase);

            if (isWatchdog)
            {
                // Mode Watchdog: Mengawasi proses utama, jika mati restart setelah 30 detik
                RunWatchdogSupervisor();
            }
            else
            {
                // Mode Utama: Pastikan single-instance dengan Mutex
                bool createdNew;
                using (var mutex = new System.Threading.Mutex(true, "AirSET_Agent_SingleInstance_Mutex_3623", out createdNew))
                {
                    if (!createdNew)
                    {
                        // Sudah ada instance yang jalan, keluar langsung
                        return;
                    }

                    // Luncurkan Watchdog di background
                    LaunchWatchdogProcess();

                    Application.Run(new TrayAppContext());
                }
            }
        }

        private static void LaunchWatchdogProcess()
        {
            try
            {
                int myPid = Process.GetCurrentProcess().Id;

                var psi = new ProcessStartInfo
                {
                    FileName = Application.ExecutablePath,
                    Arguments = "--watchdog " + myPid,
                    UseShellExecute = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true
                };
                Process.Start(psi);
            }
            catch { }
        }

        private static void RunWatchdogSupervisor()
        {
            string exePath = Application.ExecutablePath;
            string[] cmdArgs = Environment.GetCommandLineArgs();

            int parentPid = 0;
            if (cmdArgs.Length > 2)
            {
                int.TryParse(cmdArgs[2], out parentPid);
            }

            Process parentProcess = null;
            if (parentPid > 0)
            {
                try
                {
                    parentProcess = Process.GetProcessById(parentPid);
                }
                catch
                {
                    parentProcess = null;
                }
            }

            if (parentProcess != null)
            {
                // Tunggu sampai proses utama mati (baik crash ataupun di-kill mahasiswa)
                parentProcess.WaitForExit();
            }
            else
            {
                // Fallback polling jika parentPid tidak valid
                while (true)
                {
                    System.Threading.Thread.Sleep(3000);
                    bool isMainRunning = false;
                    try
                    {
                        using (var mutex = System.Threading.Mutex.OpenExisting("AirSET_Agent_SingleInstance_Mutex_3623"))
                        {
                            isMainRunning = true;
                        }
                    }
                    catch
                    {
                        isMainRunning = false;
                    }

                    if (!isMainRunning) break;
                }
            }

            // Cek apakah admin keluar secara sah (intentional exit)
            string exitFlag = Path.Combine(Path.GetTempPath(), "airset_agent_exit.flag");
            if (File.Exists(exitFlag))
            {
                try { File.Delete(exitFlag); } catch { }
                return; // Admin yang mematikan secara resmi via password, jangan hidupkan lagi
            }

            // Proses utama di-kill paksa oleh mahasiswa!
            // Tunggu tepat 30 detik sebelum auto-respawn sesuai permintaan user
            for (int i = 0; i < 30; i++)
            {
                System.Threading.Thread.Sleep(1000);
                if (File.Exists(exitFlag))
                {
                    try { File.Delete(exitFlag); } catch { }
                    return;
                }
            }

            // Hidupkan kembali proses utama
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = exePath,
                    UseShellExecute = true,
                    WorkingDirectory = Path.GetDirectoryName(exePath)
                };
                Process.Start(psi);
            }
            catch { }
        }
    }
}