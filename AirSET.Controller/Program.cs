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
                // Pembatasan hak akses Controller ditegakkan lewat autentikasi login (PasswordAuthManager),
                // bukan lewat identitas mesin/IP yang mudah dipalsukan.

                // Validasi keberadaan kunci enkripsi sebelum membuka aplikasi
                string secretKey = Environment.GetEnvironmentVariable("AIRSET_SECRET_KEY");
                if (string.IsNullOrWhiteSpace(secretKey))
                {
                    MessageBox.Show(
                        "Environment variable 'AIRSET_SECRET_KEY' belum disetel pada sistem ini.\n\n" +
                        "Silakan setel environment variable 'AIRSET_SECRET_KEY' di Windows sebelum menjalankan Controller.\n" +
                        "Pastikan kunci yang sama juga digunakan pada seluruh PC Agent.",
                        "Kunci Enkripsi Belum Dikonfigurasi",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
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
    }
}