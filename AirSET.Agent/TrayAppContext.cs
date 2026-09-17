using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace AirSET.Agent
{
    public class TrayAppContext : ApplicationContext
    {
        private const string TASK_NAME = "AirSETAgent";
        private NotifyIcon notifyIcon;
        private AgentListener agentListener;
        private FormStatus statusForm;
        private MenuItem autoStartMenuItem;

        public TrayAppContext()
        {
            agentListener = new AgentListener();
            agentListener.Start();

            // Otomatis daftarkan ke Task Scheduler (Run with Highest Privileges / Bypass UAC saat Logon)
            EnsureAutoStartRegistered();

            ContextMenu contextMenu = new ContextMenu();
            contextMenu.MenuItems.Add("Status Agent", (s, e) => ShowStatusForm());
            contextMenu.MenuItems.Add("-");

            autoStartMenuItem = new MenuItem(IsAutoStartRegistered() ? "✓ Auto-Start Aktif (Bypass UAC)" : "Aktifkan Auto-Start (Bypass UAC)", ToggleAutoStart);
            contextMenu.MenuItems.Add(autoStartMenuItem);

            contextMenu.MenuItems.Add("Hapus Auto-Start", (s, e) =>
            {
                if (PromptAdminPassword("Masukkan Password untuk menghapus auto-start:"))
                {
                    RemoveAutoStart(true);
                }
            });
            contextMenu.MenuItems.Add("-");
            contextMenu.MenuItems.Add("Keluar", (s, e) => ExitApplication());

            Icon icon = SystemIcons.Application;
            try
            {
                icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            }
            catch { }

            notifyIcon = new NotifyIcon
            {
                Icon = icon,
                ContextMenu = contextMenu,
                Visible = true,
                Text = "AirSET Agent v2.0 (Listening Port 3623)"
            };

            notifyIcon.DoubleClick += (s, e) => ShowStatusForm();
        }

        private const string WATCHDOG_TASK_NAME = "AirSETAgent_Watchdog";

        private bool IsAutoStartRegistered()
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "schtasks.exe",
                    Arguments = string.Format("/query /tn \"{0}\"", TASK_NAME),
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };
                using (var p = Process.Start(psi))
                {
                    p.WaitForExit(2000);
                    return p.ExitCode == 0;
                }
            }
            catch
            {
                return false;
            }
        }

        private bool IsWatchdogTaskRegistered()
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "schtasks.exe",
                    Arguments = string.Format("/query /tn \"{0}\"", WATCHDOG_TASK_NAME),
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };
                using (var p = Process.Start(psi))
                {
                    p.WaitForExit(2000);
                    return p.ExitCode == 0;
                }
            }
            catch
            {
                return false;
            }
        }

        private void EnsureAutoStartRegistered()
        {
            try
            {
                string appData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                string vbsPath = Path.Combine(appData, "AirSET", "watchdog.vbs");

                // Jika task utama belum ada, ATAU task watchdog belum ada, ATAU file vbs belum ada, daftarkan ulang secara silent
                if (!IsAutoStartRegistered() || !IsWatchdogTaskRegistered() || !File.Exists(vbsPath))
                {
                    RegisterAutoStart(false);
                }
            }
            catch { }
        }

        private void RegisterAutoStart(bool showMessage)
        {
            try
            {
                string exePath = Application.ExecutablePath;

                // 1. Task Logon: Menjalankan Agent saat Windows menyala / login (Delay 10 detik, Highest Privileges / Bypass UAC)
                string logonArgs = string.Format("/create /tn \"{0}\" /tr \"\"\"{1}\"\"\" /sc onlogon /delay 0000:10 /rl highest /f", TASK_NAME, exePath);
                var psiLogon = new ProcessStartInfo
                {
                    FileName = "schtasks.exe",
                    Arguments = logonArgs,
                    CreateNoWindow = true,
                    UseShellExecute = false
                };
                Process.Start(psiLogon)?.WaitForExit(3000);

                // 2. Task Watchdog Mandiri: Berjalan setiap 1 menit via Windows Task Scheduler
                // Memeriksa apakah AirSET.Agent sedang berjalan, jika di-kill mahasiswa, otomatis dihidupkan lagi!
                // Skrip VBS ringan tersembunyi dibuat di ProgramData agar eksekusi 100% silent tanpa pop-up CMD
                string appData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                string scriptsDir = Path.Combine(appData, "AirSET");
                if (!Directory.Exists(scriptsDir)) Directory.CreateDirectory(scriptsDir);

                string vbsPath = Path.Combine(scriptsDir, "watchdog.vbs");
                string vbsScript = string.Format(@"Set objWMIService = GetObject(""winmgmts:\\.\root\cimv2"")
Set colItems = objWMIService.ExecQuery(""Select * from Win32_Process Where Name = 'AirSET.Agent.exe'"")
Set fso = CreateObject(""Scripting.FileSystemObject"")
tempPath = fso.GetSpecialFolder(2) & ""\airset_agent_exit.flag""

If colItems.Count = 0 And Not fso.FileExists(tempPath) Then
    Set WshShell = CreateObject(""WScript.Shell"")
    WshShell.Run """"""{0}"""""", 1, False
End If", exePath.Replace("\"", "\"\""));

                File.WriteAllText(vbsPath, vbsScript);

                // Daftarkan Task Watchdog berjalan setiap 1 menit dengan hak akses Administrator
                // Format taskrun menggunakan sintaks standar Windows: wscript.exe "C:\Path\watchdog.vbs"
                string watchdogArgs = string.Format("/create /tn \"{0}\" /tr \"wscript.exe \\\"{1}\\\"\" /sc minute /mo 1 /rl highest /f", WATCHDOG_TASK_NAME, vbsPath);
                var psiWatchdog = new ProcessStartInfo
                {
                    FileName = "schtasks.exe",
                    Arguments = watchdogArgs,
                    CreateNoWindow = true,
                    UseShellExecute = false
                };
                using (var p = Process.Start(psiWatchdog))
                {
                    p.WaitForExit(3000);
                    if (autoStartMenuItem != null) autoStartMenuItem.Text = "✓ Auto-Start & Anti-Kill Aktif";
                    if (showMessage)
                    {
                        MessageBox.Show("Auto-Start & Anti-Kill Watchdog berhasil diaktifkan!\nJika mahasiswa mematikan Agent dari Task Manager, Windows akan otomatis menyalakannya kembali.", "AirSET Agent", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                if (showMessage)
                {
                    MessageBox.Show("Gagal mendaftarkan proteksi: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void RemoveAutoStart(bool showMessage)
        {
            try
            {
                // Hapus kedua task di Task Scheduler
                Process.Start(new ProcessStartInfo { FileName = "schtasks.exe", Arguments = string.Format("/delete /tn \"{0}\" /f", TASK_NAME), CreateNoWindow = true, UseShellExecute = false })?.WaitForExit(2000);
                Process.Start(new ProcessStartInfo { FileName = "schtasks.exe", Arguments = string.Format("/delete /tn \"{0}\" /f", WATCHDOG_TASK_NAME), CreateNoWindow = true, UseShellExecute = false })?.WaitForExit(2000);

                if (autoStartMenuItem != null) autoStartMenuItem.Text = "Aktifkan Auto-Start (Bypass UAC)";
                if (showMessage)
                {
                    MessageBox.Show("Auto-start & Watchdog berhasil dinonaktifkan.", "AirSET Agent", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                if (showMessage)
                {
                    MessageBox.Show("Gagal menghapus auto-start: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ToggleAutoStart(object sender, EventArgs e)
        {
            if (IsAutoStartRegistered())
            {
                if (PromptAdminPassword("Masukkan Password untuk menonaktifkan auto-start:"))
                {
                    RemoveAutoStart(true);
                }
            }
            else
            {
                RegisterAutoStart(true);
            }
        }

        private void ShowStatusForm()
        {
            if (statusForm == null || statusForm.IsDisposed)
            {
                statusForm = new FormStatus(agentListener);
            }
            statusForm.Show();
            statusForm.BringToFront();
        }

        private void ExitApplication()
        {
            if (!PromptAdminPassword("Masukkan Password untuk mematikan AirSET Agent:"))
            {
                return;
            }

            var result = MessageBox.Show(
                "Apakah Anda yakin ingin mematikan AirSET Agent?\nPC ini tidak akan bisa menerima remote konfigurasi dari Host sampai aplikasi dijalankan kembali.",
                "Konfirmasi Keluar Agent",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result == DialogResult.Yes)
            {
                try
                {
                    string exitFlag = Path.Combine(Path.GetTempPath(), "airset_agent_exit.flag");
                    File.WriteAllText(exitFlag, "EXIT");
                }
                catch { }

                agentListener.Stop();
                notifyIcon.Visible = false;
                Application.Exit();
            }
        }

        private bool PromptAdminPassword(string promptTitle)
        {
            using (var promptForm = new Form
            {
                Text = "Verifikasi Keamanan",
                Size = new Size(380, 180),
                StartPosition = FormStartPosition.CenterScreen,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                TopMost = true
            })
            {
                var lblPrompt = new Label
                {
                    Text = promptTitle,
                    Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                    Location = new Point(15, 15),
                    Size = new Size(335, 36)
                };

                var txtPass = new TextBox
                {
                    PasswordChar = '●',
                    Location = new Point(18, 55),
                    Size = new Size(330, 24),
                    Font = new Font("Segoe UI", 10F)
                };

                var btnOk = new Button
                {
                    Text = "Konfirmasi",
                    DialogResult = DialogResult.OK,
                    Location = new Point(165, 95),
                    Size = new Size(95, 30),
                    BackColor = Color.FromArgb(15, 118, 110),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };
                btnOk.FlatAppearance.BorderSize = 0;

                var btnCancel = new Button
                {
                    Text = "Batal",
                    DialogResult = DialogResult.Cancel,
                    Location = new Point(268, 95),
                    Size = new Size(80, 30)
                };

                promptForm.Controls.AddRange(new Control[] { lblPrompt, txtPass, btnOk, btnCancel });
                promptForm.AcceptButton = btnOk;
                promptForm.CancelButton = btnCancel;

                if (promptForm.ShowDialog() == DialogResult.OK)
                {
                    string entered = txtPass.Text;
                    // Default password laboran lab:"
                    if (entered == "4m1k0ml4b" || entered == "uptl4b4m1k0mj4y4" || entered == "ravenusasupport")
                    {
                        return true;
                    }

                    MessageBox.Show("Password salah! Akses ditolak.", "Keamanan AirSET", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return false;
            }
        }
    }
}