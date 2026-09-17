using System;
using System.Threading;
using System.Windows.Forms;

namespace AirSET.Controller
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                string assemblyName = new System.Reflection.AssemblyName(args.Name).Name;
                if (assemblyName.Equals("AirSET.Core", StringComparison.OrdinalIgnoreCase))
                {
                    // 1. Coba load dari embedded resource di dalam .exe
                    var currentAssembly = System.Reflection.Assembly.GetExecutingAssembly();
                    foreach (string resName in currentAssembly.GetManifestResourceNames())
                    {
                        if (resName.EndsWith("AirSET.Core.dll", StringComparison.OrdinalIgnoreCase))
                        {
                            using (System.IO.Stream stream = currentAssembly.GetManifestResourceStream(resName))
                            {
                                if (stream != null)
                                {
                                    byte[] assemblyData = new byte[stream.Length];
                                    stream.Read(assemblyData, 0, assemblyData.Length);
                                    return System.Reflection.Assembly.Load(assemblyData);
                                }
                            }
                        }
                    }

                    // 2. Fallback jika ada file fisik di samping .exe
                    string[] possiblePaths = new string[]
                    {
                        System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AirSET.Core.dll"),
                        System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "AirSET.Core.dll"),
                        System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "AirSET.Core", "bin", "Release", "AirSET.Core.dll"),
                        System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "AirSET.Core", "bin", "Debug", "AirSET.Core.dll")
                    };

                    foreach (var path in possiblePaths)
                    {
                        if (System.IO.File.Exists(path))
                        {
                            return System.Reflection.Assembly.LoadFrom(path);
                        }
                    }
                }
                return null;
            };

            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += (s, e) =>
            {
                MessageBox.Show("Terjadi error saat menjalankan Controller:\n" + e.Exception.ToString(), "Error AirSET Controller", MessageBoxButtons.OK, MessageBoxIcon.Error);
            };

            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                MessageBox.Show("Fatal error:\n" + (e.ExceptionObject != null ? e.ExceptionObject.ToString() : "Unknown"), "Fatal Error AirSET Controller", MessageBoxButtons.OK, MessageBoxIcon.Error);
            };

            try
            {
                // Verifikasi Otorisasi Komputer Controller:
                // Hanya boleh dijalankan pada komputer dengan Hostname 'Komputer-presentasi'
                // ATAU memiliki IP address berakhiran .90 (misal 10.22.1.90, 10.23.3.90, 10.73.1.90, dll)
                if (!IsAuthorizedControllerMachine())
                {
                    MessageBox.Show(
                        "Akses ditolak! Komputer ini tidak memenuhi syarat untuk membuka aplikasi.",
                        "Keamanan AirSET Controller",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Stop
                    );
                    return;
                }

                // Default langsung membuka Dashboard (Mode Pengajar) tanpa form login awal.
                // Akses Mode Laboran / Admin dapat dibuka melalui tombol di header dashboard.
                Application.Run(new FormDashboard());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saat memuat aplikasi:\n" + ex.ToString(), "Startup Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static bool IsAuthorizedControllerMachine()
        {
            string hostName = System.Net.Dns.GetHostName();
            if (string.Equals(hostName, "Komputer-presentasi", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            try
            {
                foreach (var ni in System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (ni.OperationalStatus == System.Net.NetworkInformation.OperationalStatus.Up &&
                        ni.NetworkInterfaceType != System.Net.NetworkInformation.NetworkInterfaceType.Loopback)
                    {
                        foreach (var u in ni.GetIPProperties().UnicastAddresses)
                        {
                            if (u.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                            {
                                string ipStr = u.Address.ToString();
                                if (ipStr.EndsWith(".90"))
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            catch { }

            return false;
        }
    }
}