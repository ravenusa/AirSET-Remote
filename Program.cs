using System;
using System.Diagnostics;
using System.Security.Principal;
using System.Windows.Forms;

namespace devIPsett
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (!IsAdministrator())
            {
                try
                {
                    ProcessStartInfo procInfo = new ProcessStartInfo
                    {
                        FileName = Application.ExecutablePath,
                        UseShellExecute = true,
                        Verb = "runas"
                    };
                    Process.Start(procInfo);
                }
                catch
                {
                    MessageBox.Show(
                        "ERROR: Akses Ditolak!\n\nAplikasi ini membutuhkan hak akses Administrator untuk mengonfigurasi IP, DNS, Hostname, dan Workgroup.\n\nSilakan jalankan kembali aplikasi dengan Klik Kanan -> Run as administrator.",
                        "Akses Ditolak - Run as Administrator",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormMain());
        }

        private static bool IsAdministrator()
        {
            try
            {
                WindowsIdentity identity = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch
            {
                return false;
            }
        }
    }
}