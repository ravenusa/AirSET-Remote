using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace AirSET.Agent
{
    public class FormLockScreen : Form
    {
        private static FormLockScreen currentInstance;
        private static readonly object lockObj = new object();

        // Win32 API Hook Constants & Delegates
        private const int WH_KEYBOARD_LL = 13;
        private const int WH_MOUSE_LL = 14;

        private delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);
        private static HookProc keyboardProc;
        private static HookProc mouseProc;
        private static IntPtr keyboardHookId = IntPtr.Zero;
        private static IntPtr mouseHookId = IntPtr.Zero;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll")]
        private static extern bool ClipCursor(ref RECT lpRect);

        [DllImport("user32.dll")]
        private static extern bool ClipCursor(IntPtr lpRect);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);
        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowText(IntPtr hWnd, System.Text.StringBuilder lpString, int nMaxCount);

        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOMOVE = 0x0002;
        private const uint SWP_SHOWWINDOW = 0x0040;
        private const int SW_MINIMIZE = 6;

        private const int WS_EX_TOPMOST = 0x00000008;
        private const int WS_EX_TOOLWINDOW = 0x00000080;

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= WS_EX_TOPMOST | WS_EX_TOOLWINDOW;
                return cp;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct KBDLLHOOKSTRUCT
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public IntPtr dwExtraInfo;
        }

        private System.Windows.Forms.Timer topTimer;

        public FormLockScreen()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Normal;
            this.StartPosition = FormStartPosition.Manual;
            this.Bounds = Screen.PrimaryScreen.Bounds;
            this.TopMost = true;
            this.ShowInTaskbar = false;
            this.BackColor = Color.FromArgb(15, 23, 42); // Slate dark
            this.DoubleBuffered = true;

            this.Location = new Point(0, 0);
            this.Size = Screen.PrimaryScreen.Bounds.Size;

            SetupUI();

            topTimer = new System.Windows.Forms.Timer { Interval = 250 };
            topTimer.Tick += (s, e) =>
            {
                if (!this.TopMost) this.TopMost = true;
                EnforceTopMost();
                LockMouseCursor();
            };
            topTimer.Start();
        }

        private void EnforceTopMost()
        {
            try
            {
                SetWindowPos(this.Handle, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);
                SetForegroundWindow(this.Handle);
                this.BringToFront();
            }
            catch { }
        }

        public static void MinimizeOtherWindows(IntPtr myHandle)
        {
            try
            {
                EnumWindows((hWnd, lParam) =>
                {
                    if (hWnd != myHandle && IsWindowVisible(hWnd))
                    {
                        var sb = new System.Text.StringBuilder(256);
                        GetWindowText(hWnd, sb, 256);
                        string title = sb.ToString();
                        // Jangan minimize shell/taskbar/desktop
                        if (!string.IsNullOrEmpty(title) && title != "Program Manager" && title != "AirSET Lock Screen")
                        {
                            ShowWindow(hWnd, SW_MINIMIZE);
                        }
                    }
                    return true;
                }, IntPtr.Zero);
            }
            catch { }
        }

        private void SetupUI()
        {
            var pnlCenter = new Panel
            {
                Size = new Size(700, 420),
                BackColor = Color.FromArgb(30, 41, 59),
                BorderStyle = BorderStyle.None
            };
            pnlCenter.Location = new Point(
                (this.ClientSize.Width - pnlCenter.Width) / 2,
                (this.ClientSize.Height - pnlCenter.Height) / 2
            );

            // Logo Perusahaan / Kampus (Pengganti ikon gembok)
            var picLogo = new PictureBox
            {
                Size = new Size(110, 95),
                Location = new Point((pnlCenter.Width - 110) / 2, 20),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent
            };

            try
            {
                // 1. Coba load dari embedded resource di dalam .exe
                var currentAsm = System.Reflection.Assembly.GetExecutingAssembly();
                using (var stream = currentAsm.GetManifestResourceStream("AirSET.Agent.logo_lock.png"))
                {
                    if (stream != null)
                    {
                        picLogo.Image = Image.FromStream(stream);
                    }
                }

                // 2. Fallback: file lokal jika stream null
                if (picLogo.Image == null)
                {
                    string localPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logo_lock.png");
                    if (File.Exists(localPath)) picLogo.Image = Image.FromFile(localPath);
                }
            }
            catch { }

            var lblTitle = new Label
            {
                Text = "ATTENTION / PERHATIAN",
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(700, 40),
                Location = new Point(0, 130)
            };

            var lblMessage = new Label
            {
                Text = "Komputer ini sedang dikunci oleh Dosen / Instruktur Laboratorium.\nHarap perhatikan materi atau demonstrasi di layar depan.",
                Font = new Font("Segoe UI", 12F, FontStyle.Regular),
                ForeColor = Color.FromArgb(226, 232, 240),
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(640, 60),
                Location = new Point(30, 180)
            };

            var lblHost = new Label
            {
                Text = string.Format("Komputer: {0}  |  AirSET Classroom Management", Environment.MachineName),
                Font = new Font("Segoe UI", 10F, FontStyle.Italic),
                ForeColor = Color.FromArgb(148, 163, 184),
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(700, 30),
                Location = new Point(0, 260)
            };

            var pnlBadge = new Panel
            {
                Size = new Size(260, 36),
                BackColor = Color.FromArgb(185, 28, 28),
                Location = new Point((pnlCenter.Width - 260) / 2, 315)
            };
            var lblBadge = new Label
            {
                Text = "🔒 INPUT DEVICE LOCKED",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.White,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
            pnlBadge.Controls.Add(lblBadge);

            // Copyright & Identity Label (di dalam kartu tengah)
            var lblCopyright = new Label
            {
                Text = "© 2026 Ravenusa | 20.11.3623 | UPT Lab Amikom Yogyakarta",
                Font = new Font("Segoe UI", 8.5F, FontStyle.Regular),
                ForeColor = Color.FromArgb(100, 116, 139), // Slate muted
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(700, 25),
                Location = new Point(0, 365)
            };

            pnlCenter.Controls.AddRange(new Control[] { picLogo, lblTitle, lblMessage, lblHost, pnlBadge, lblCopyright });
            this.Controls.Add(pnlCenter);

            this.Resize += (s, e) =>
            {
                pnlCenter.Location = new Point(
                    (this.ClientSize.Width - pnlCenter.Width) / 2,
                    (this.ClientSize.Height - pnlCenter.Height) / 2
                );
            };
        }

        private void LockMouseCursor()
        {
            try
            {
                int centerX = this.Left + (this.Width / 2);
                int centerY = this.Top + (this.Height / 2);
                RECT rect;
                rect.Left = centerX - 2;
                rect.Top = centerY - 2;
                rect.Right = centerX + 2;
                rect.Bottom = centerY + 2;
                ClipCursor(ref rect);
            }
            catch { }
        }

        private static void ReleaseMouseCursor()
        {
            try
            {
                ClipCursor(IntPtr.Zero);
            }
            catch { }
        }

        #region Windows Low-Level Input Hooks
        private static void InstallHooks()
        {
            if (keyboardHookId == IntPtr.Zero)
            {
                keyboardProc = KeyboardHookCallback;
                using (var curProcess = Process.GetCurrentProcess())
                using (var curModule = curProcess.MainModule)
                {
                    keyboardHookId = SetWindowsHookEx(WH_KEYBOARD_LL, keyboardProc, GetModuleHandle(curModule.ModuleName), 0);
                }
            }

            if (mouseHookId == IntPtr.Zero)
            {
                mouseProc = MouseHookCallback;
                using (var curProcess = Process.GetCurrentProcess())
                using (var curModule = curProcess.MainModule)
                {
                    mouseHookId = SetWindowsHookEx(WH_MOUSE_LL, mouseProc, GetModuleHandle(curModule.ModuleName), 0);
                }
            }
        }

        private static void UninstallHooks()
        {
            if (keyboardHookId != IntPtr.Zero)
            {
                UnhookWindowsHookEx(keyboardHookId);
                keyboardHookId = IntPtr.Zero;
            }

            if (mouseHookId != IntPtr.Zero)
            {
                UnhookWindowsHookEx(mouseHookId);
                mouseHookId = IntPtr.Zero;
            }
        }

        private static IntPtr KeyboardHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                KBDLLHOOKSTRUCT kbd = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(KBDLLHOOKSTRUCT));

                bool altPressed = (kbd.flags & 0x20) != 0;
                bool ctrlPressed = (Control.ModifierKeys & Keys.Control) != 0;

                // 1. Blokir Alt+Tab
                if (altPressed && kbd.vkCode == 9) return (IntPtr)1;

                // 2. Blokir Alt+Escape
                if (altPressed && kbd.vkCode == 27) return (IntPtr)1;

                // 3. Blokir Ctrl+Escape
                if (ctrlPressed && kbd.vkCode == 27) return (IntPtr)1;

                // 4. Blokir Windows Key kiri & kanan
                if (kbd.vkCode == 91 || kbd.vkCode == 92) return (IntPtr)1;

                // 5. Blokir Alt+F4
                if (altPressed && kbd.vkCode == 115) return (IntPtr)1;

                // 6. Blokir Windows + Space
                if (kbd.vkCode == 32 && altPressed) return (IntPtr)1;

                // Blokir SEMUA input keyboard saat layar terkunci
                return (IntPtr)1;
            }
            return CallNextHookEx(keyboardHookId, nCode, wParam, lParam);
        }

        private static IntPtr MouseHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                int msg = wParam.ToInt32();
                const int WM_LBUTTONDOWN = 0x0201;
                const int WM_LBUTTONUP = 0x0202;
                const int WM_RBUTTONDOWN = 0x0204;
                const int WM_RBUTTONUP = 0x0205;
                const int WM_MBUTTONDOWN = 0x0207;
                const int WM_MBUTTONUP = 0x0208;
                const int WM_MOUSEWHEEL = 0x020A;
                const int WM_LBUTTONDBLCLK = 0x0203;
                const int WM_RBUTTONDBLCLK = 0x0206;

                if (msg == WM_LBUTTONDOWN || msg == WM_LBUTTONUP ||
                    msg == WM_RBUTTONDOWN || msg == WM_RBUTTONUP ||
                    msg == WM_MBUTTONDOWN || msg == WM_MBUTTONUP ||
                    msg == WM_MOUSEWHEEL || msg == WM_LBUTTONDBLCLK ||
                    msg == WM_RBUTTONDBLCLK)
                {
                    return (IntPtr)1;
                }
            }
            return CallNextHookEx(mouseHookId, nCode, wParam, lParam);
        }
        private static void SetTaskManagerDisabled(bool disabled)
        {
            try
            {
                using (var key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\System"))
                {
                    if (key != null)
                    {
                        if (disabled)
                        {
                            key.SetValue("DisableTaskMgr", 1, Microsoft.Win32.RegistryValueKind.DWord);
                        }
                        else
                        {
                            key.DeleteValue("DisableTaskMgr", false);
                        }
                    }
                }
            }
            catch { }
        }

        #endregion

        #region Public Static API
        private static System.Threading.Thread lockUiThread;

        public static bool IsLocked
        {
            get
            {
                lock (lockObj)
                {
                    return currentInstance != null && !currentInstance.IsDisposed && currentInstance.Visible;
                }
            }
        }

        public static void LockScreen()
        {
            lock (lockObj)
            {
                // Nonaktifkan Task Manager agar mahasiswa tidak bisa mematikan proses saat layar dikunci
                SetTaskManagerDisabled(true);

                if (currentInstance != null && !currentInstance.IsDisposed)
                {
                    try
                    {
                        currentInstance.BeginInvoke((Action)(() =>
                        {
                            currentInstance.BringToFront();
                            currentInstance.TopMost = true;
                            currentInstance.LockMouseCursor();
                        }));
                    }
                    catch { }
                    return;
                }

                // Jalankan di Dedicated STA Thread dengan Application.Run() agar message loop aktif
                // Hook WH_KEYBOARD_LL dan WH_MOUSE_LL mewajibkan Message Loop aktif!
                lockUiThread = new System.Threading.Thread(() =>
                {
                    try
                    {
                        currentInstance = new FormLockScreen();
                        currentInstance.Shown += (s, e) =>
                        {
                            InstallHooks();
                            MinimizeOtherWindows(currentInstance.Handle);
                            currentInstance.EnforceTopMost();
                            currentInstance.LockMouseCursor();
                        };
                        currentInstance.FormClosed += (s, e) =>
                        {
                            UninstallHooks();
                            ReleaseMouseCursor();
                            SetTaskManagerDisabled(false);
                        };

                        Application.Run(currentInstance);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("LockScreen UI Thread error: " + ex.Message);
                    }
                    finally
                    {
                        UninstallHooks();
                        ReleaseMouseCursor();
                        SetTaskManagerDisabled(false);
                        currentInstance = null;
                    }
                });

                lockUiThread.SetApartmentState(System.Threading.ApartmentState.STA);
                lockUiThread.IsBackground = true;
                lockUiThread.Start();
            }
        }

        public static void UnlockScreen()
        {
            lock (lockObj)
            {
                UninstallHooks();
                ReleaseMouseCursor();
                SetTaskManagerDisabled(false);

                if (currentInstance != null && !currentInstance.IsDisposed)
                {
                    try
                    {
                        currentInstance.BeginInvoke((Action)(() =>
                        {
                            try
                            {
                                if (currentInstance.topTimer != null) currentInstance.topTimer.Stop();
                                currentInstance.Close();
                                currentInstance.Dispose();
                            }
                            catch { }
                            finally
                            {
                                currentInstance = null;
                            }
                        }));
                    }
                    catch
                    {
                        try
                        {
                            currentInstance.Close();
                            currentInstance.Dispose();
                        }
                        catch { }
                        finally
                        {
                            currentInstance = null;
                        }
                    }
                }
            }
        }
        #endregion
    }
}
