using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace devIPsett
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
            SetWindowIcon();
            ApplyModernStyling();
        }

        private void SetWindowIcon()
        {
            try
            {
                var mainModule = System.Diagnostics.Process.GetCurrentProcess().MainModule;
                string exeLocation = mainModule != null ? mainModule.FileName : string.Empty;

                if (!string.IsNullOrEmpty(exeLocation) && File.Exists(exeLocation))
                {
                    Icon appIcon = Icon.ExtractAssociatedIcon(exeLocation);
                    if (appIcon != null)
                    {
                        this.Icon = appIcon;
                        return;
                    }
                }
            }
            catch
            {
            }
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            Log("Aplikasi Siap. Memuat profil LAB dan daftar kartu jaringan...");
            lblActiveUserVal.Text = Environment.UserName;
            lblCurrentHostname.Text = string.Format("Current Hostname: {0}", System.Net.Dns.GetHostName());
            LoadLabProfiles();
            LoadNetworkInterfaces();
            UpdatePreview();
        }

        private void LoadLabProfiles(string selectCode = null)
        {
            cmbLabSelect.Items.Clear();
            var profiles = LabData.GetProfiles();
            int selectedIndexToSet = 0;

            for (int i = 0; i < profiles.Count; i++)
            {
                cmbLabSelect.Items.Add(profiles[i]);
                if (!string.IsNullOrEmpty(selectCode) && profiles[i].Code.Equals(selectCode, StringComparison.OrdinalIgnoreCase))
                {
                    selectedIndexToSet = i;
                }
            }

            if (cmbLabSelect.Items.Count > 0)
            {
                cmbLabSelect.SelectedIndex = selectedIndexToSet;
            }
        }

        private void cmbLabSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbLabSelect.SelectedItem is LabProfile selectedProfile)
            {
                txtBaseIp.Text = selectedProfile.BaseIp;
                txtSubnet.Text = selectedProfile.Subnet;
                txtGateway.Text = selectedProfile.Gateway;
                txtDns.Text = selectedProfile.Dns;
                txtWorkgroup.Text = selectedProfile.Workgroup;
                txtPrefix.Text = selectedProfile.HostnamePrefix;

                bool isCustom = LabData.IsCustomProfile(selectedProfile.Code);
                btnEditLab.Visible = isCustom;
                btnDeleteLab.Visible = isCustom;

                Log(string.Format("Profil terpilih: {0} ({1})", selectedProfile.Name, selectedProfile.Code));
                UpdatePreview();
            }
        }

        private async void btnAddLab_Click(object sender, EventArgs e)
        {
            using (FormAddLab addForm = new FormAddLab())
            {
                if (addForm.ShowDialog(this) == DialogResult.OK && addForm.CreatedProfile != null)
                {
                    Log(string.Format("Menyimpan LAB Baru '{0}' ke Cloud/Lokal...", addForm.CreatedProfile.Name));
                    bool savedCloud = await LabData.SaveCustomProfileAsync(addForm.CreatedProfile);
                    if (savedCloud)
                    {
                        Log(string.Format("   [OK] LAB Baru '{0}' berhasil disimpan & disinkronkan ke Cloud!", addForm.CreatedProfile.Name));
                    }
                    else
                    {
                        Log(string.Format("   [INFO] LAB Baru '{0}' disimpan ke file JSON lokal.", addForm.CreatedProfile.Name));
                    }
                    LoadLabProfiles(addForm.CreatedProfile.Code);
                }
            }
        }

        private async void btnEditLab_Click(object sender, EventArgs e)
        {
            if (cmbLabSelect.SelectedItem is LabProfile selectedProfile)
            {
                if (!LabData.IsCustomProfile(selectedProfile.Code))
                {
                    MessageBox.Show("LAB Bawaan sistem tidak dapat diedit!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (FormAddLab editForm = new FormAddLab(selectedProfile))
                {
                    if (editForm.ShowDialog(this) == DialogResult.OK && editForm.CreatedProfile != null)
                    {
                        Log(string.Format("Meng-update Profil LAB '{0}' di Cloud/Lokal...", editForm.CreatedProfile.Name));
                        bool savedCloud = await LabData.SaveCustomProfileAsync(editForm.CreatedProfile);
                        if (savedCloud)
                        {
                            Log(string.Format("   [OK] LAB '{0}' berhasil di-update ke Cloud!", editForm.CreatedProfile.Name));
                        }
                        else
                        {
                            Log(string.Format("   [INFO] LAB '{0}' di-update di file JSON lokal.", editForm.CreatedProfile.Name));
                        }
                        LoadLabProfiles(editForm.CreatedProfile.Code);
                    }
                }
            }
        }

        private async void btnDeleteLab_Click(object sender, EventArgs e)
        {
            if (cmbLabSelect.SelectedItem is LabProfile selectedProfile)
            {
                if (!LabData.IsCustomProfile(selectedProfile.Code))
                {
                    MessageBox.Show("LAB Bawaan sistem tidak dapat dihapus!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DialogResult result = MessageBox.Show(
                    string.Format("Apakah Anda yakin ingin menghapus profil LAB '{0}' ({1})?", selectedProfile.Name, selectedProfile.Code),
                    "Konfirmasi Hapus LAB",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (result == DialogResult.Yes)
                {
                    Log(string.Format("Menghapus LAB Custom '{0}'...", selectedProfile.Name));
                    bool deleted = await LabData.DeleteCustomProfileAsync(selectedProfile.Code);
                    if (deleted)
                    {
                        Log(string.Format("   [OK] LAB '{0}' berhasil dihapus dari Cloud Gist & Lokal!", selectedProfile.Name));
                    }
                    else
                    {
                        Log(string.Format("   [INFO] LAB '{0}' dihapus dari penyimpanan lokal.", selectedProfile.Name));
                    }
                    LoadLabProfiles();
                }
            }
        }

        private void LoadNetworkInterfaces()
        {
            cmbInterface.Items.Clear();
            var interfaces = NetworkServices.GetNetworkInterfaces();
            foreach (var iface in interfaces)
            {
                cmbInterface.Items.Add(iface);
            }

            if (cmbInterface.Items.Count > 0)
            {
                cmbInterface.SelectedIndex = 0;
            }
        }

        private void btnRefreshIf_Click(object sender, EventArgs e)
        {
            LoadNetworkInterfaces();
            lblCurrentHostname.Text = string.Format("Current Hostname: {0}", System.Net.Dns.GetHostName());
            Log("Daftar Interface & Hostname Diperbarui.");
        }

        private void txtPcNum_TextChanged(object sender, EventArgs e)
        {
            UpdatePreview();
        }

        private void InputConfig_Changed(object sender, EventArgs e)
        {
            UpdatePreview();
        }

        private void chkChangePassword_CheckedChanged(object sender, EventArgs e)
        {
            bool showPasswordOptions = chkChangePassword.Checked;
            lblActiveUserLbl.Visible = showPasswordOptions;
            lblActiveUserVal.Visible = showPasswordOptions;
            lblNewPassword.Visible = showPasswordOptions;
            txtNewPassword.Visible = showPasswordOptions;
            btnToggleShowPassword.Visible = showPasswordOptions;
            chkAutoLogon.Visible = showPasswordOptions;
            chkDisableAutoLogon.Visible = showPasswordOptions;

            if (showPasswordOptions)
            {
                lblActiveUserVal.Text = Environment.UserName;
                txtNewPassword.Focus();
            }
            else
            {
                txtNewPassword.Clear();
                chkAutoLogon.Checked = false;
                chkDisableAutoLogon.Checked = false;
            }
        }

        private void chkAutoLogon_CheckedChanged(object sender, EventArgs e)
        {
            if (chkAutoLogon.Checked)
            {
                chkDisableAutoLogon.Checked = false;
            }
        }

        private void chkDisableAutoLogon_CheckedChanged(object sender, EventArgs e)
        {
            if (chkDisableAutoLogon.Checked)
            {
                chkAutoLogon.Checked = false;
            }
        }

        private void btnToggleShowPassword_Click(object sender, EventArgs e)
        {
            if (txtNewPassword.PasswordChar == '●')
            {
                txtNewPassword.PasswordChar = '\0'; // Tampilkan Password Teks Biasa
                btnToggleShowPassword.Text = "🙈";
            }
            else
            {
                txtNewPassword.PasswordChar = '●'; // Sembunyikan dengan Asterisk/Dot
                btnToggleShowPassword.Text = "👁️";
            }
        }

        private string GetTargetHostname(string prefix, int pcNum)
        {
            if (pcNum == 90)
            {
                return "Komputer-Presentasi";
            }
            if (pcNum == 111)
            {
                return "Komputer-Ravenusa";
            }
            return string.Format("{0}{1:D2}", prefix, pcNum);
        }

        private void UpdatePreview()
        {
            string pcNumRaw = txtPcNum.Text.Trim();
            string pcNumDisplay = pcNumRaw;

            int pcNumVal;
            string targetHostname;
            string prefix = txtPrefix.Text.Trim();

            if (int.TryParse(pcNumRaw, out pcNumVal) && pcNumVal > 0)
            {
                pcNumDisplay = pcNumVal.ToString();
                targetHostname = GetTargetHostname(prefix, pcNumVal);
            }
            else
            {
                pcNumDisplay = "0";
                targetHostname = string.Format("{0}00", prefix);
            }

            string baseIp = txtBaseIp.Text.Trim();

            lblPreviewIp.Text = string.Format("Target IP: {0}.{1}", baseIp, pcNumDisplay);
            lblPreviewHostname.Text = string.Format("Target Hostname: {0}", targetHostname);
        }

        private async void btnApply_Click(object sender, EventArgs e)
        {
            string pcNumText = txtPcNum.Text.Trim();
            int pcNum;
            if (!int.TryParse(pcNumText, out pcNum) || pcNum <= 0 || pcNum > 254)
            {
                MessageBox.Show("Mohon masukkan nomor komputer yang valid (1 - 254)!", "Input Tidak Valid", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPcNum.Focus();
                return;
            }

            // Validasi Password jika Checkbox Ubah Password dicentang
            string newPassword = txtNewPassword.Text.Trim();
            if (chkChangePassword.Checked)
            {
                if (string.IsNullOrEmpty(newPassword))
                {
                    MessageBox.Show(
                        string.Format("Peringatan: Password baru tidak boleh kosong jika fitur Ubah Password dicentang untuk User '{0}'!", Environment.UserName),
                        "Password Kosong",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    txtNewPassword.Focus();
                    return;
                }
            }

            string interfaceName = cmbInterface.Text.Trim();
            string baseIp = txtBaseIp.Text.Trim();
            string subnet = txtSubnet.Text.Trim();
            string gateway = txtGateway.Text.Trim();
            string dns = txtDns.Text.Trim();
            string workgroup = txtWorkgroup.Text.Trim();
            string prefix = txtPrefix.Text.Trim();

            if (string.IsNullOrEmpty(interfaceName) || string.IsNullOrEmpty(baseIp) || string.IsNullOrEmpty(subnet) || string.IsNullOrEmpty(gateway))
            {
                MessageBox.Show("Mohon lengkapi seluruh konfigurasi dasar jaringan!", "Konfigurasi Belum Lengkap", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string targetIp = string.Format("{0}.{1}", baseIp, pcNum);
            string targetHostname = GetTargetHostname(prefix, pcNum);
            string labName = cmbLabSelect.SelectedItem is LabProfile lab ? lab.Name : "Lab";

            string passOptionInfo = chkChangePassword.Checked ? string.Format("\n• Ubah Password User ({0}): [YA]", Environment.UserName) : "";
            string autoLogonInfo = "";
            if (chkChangePassword.Checked)
            {
                if (chkAutoLogon.Checked) autoLogonInfo = "\n• Auto-Logon Windows: [AKTIF / BYPASS]";
                else if (chkDisableAutoLogon.Checked) autoLogonInfo = "\n• Auto-Logon Windows: [NONAKTIF / MEMINTA PASSWORD]";
            }

            DialogResult confirm = MessageBox.Show(
                string.Format(
                    "Apakah Anda yakin ingin menerapkan pengaturan untuk {0}?\n\n" +
                    "• Interface: {1}\n" +
                    "• IP Address: {2}\n" +
                    "• Subnet Mask: {3}\n" +
                    "• Gateway: {4}\n" +
                    "• DNS Server: {5}\n" +
                    "• Current Hostname: {6}\n" +
                    "• Target Hostname: {7}\n" +
                    "• Workgroup: {8}" +
                    "{9}{10}",
                    labName, interfaceName, targetIp, subnet, gateway, dns, System.Net.Dns.GetHostName(), targetHostname, workgroup, passOptionInfo, autoLogonInfo
                ),
                "Konfirmasi Pengaturan",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (confirm != DialogResult.Yes) return;

            SetFormEnabled(false);
            Log("=================================================");
            Log(string.Format("[+] Memulai Pengaturan untuk {0} - Hostname {1}...", labName, targetHostname));

            // 1. IP Address
            Log(string.Format("1. Menerapkan IP Address: {0} pada interface '{1}'...", targetIp, interfaceName));
            var ipResult = await NetworkServices.ApplyIpAddressAsync(interfaceName, targetIp, subnet, gateway);
            if (ipResult.Success)
            {
                Log("   [OK] IP Address berhasil diterapkan!");
            }
            else
            {
                Log(string.Format("   [ERROR] Gagal menerapkan IP: {0}", ipResult.Error));
            }

            // 2. DNS Server
            Log(string.Format("2. Menerapkan DNS Server: {0}...", dns));
            var dnsResult = await NetworkServices.ApplyDnsAsync(interfaceName, dns);
            if (dnsResult.Success)
            {
                Log("   [OK] DNS Server berhasil diterapkan!");
            }
            else
            {
                Log(string.Format("   [INFO] Status DNS: {0} {1}", dnsResult.Output, dnsResult.Error));
            }

            // 3. Hostname / Computer Name
            Log(string.Format("3. Mengubah Nama Komputer dari '{0}' menjadi '{1}'...", System.Net.Dns.GetHostName(), targetHostname));
            var hostResult = await NetworkServices.RenameComputerAsync(targetHostname);
            if (hostResult.Success)
            {
                Log("   [OK] Nama Komputer berhasil diubah!");
            }
            else
            {
                Log(string.Format("   [INFO] Status Rename: {0} {1}", hostResult.Output, hostResult.Error));
            }

            // 4. Workgroup
            Log(string.Format("4. Mengubah Workgroup menjadi '{0}'...", workgroup));
            var wgResult = await NetworkServices.ChangeWorkgroupAsync(workgroup);
            if (wgResult.Success)
            {
                Log("   [OK] Workgroup berhasil diubah!");
            }
            else
            {
                Log(string.Format("   [INFO] Status Workgroup: {0} {1}", wgResult.Output, wgResult.Error));
            }

            // 5. Ubah Password User & Password Never Expired
            if (chkChangePassword.Checked)
            {
                Log(string.Format("5. Mengubah Password User '{0}' & Mengeset Password Never Expired...", Environment.UserName));
                var passResult = await NetworkServices.ChangeUserPasswordAsync(newPassword);
                if (passResult.Success)
                {
                    Log(string.Format("   [OK] Password User '{0}' berhasil diubah & Status Password set to NEVER EXPIRED!", Environment.UserName));
                }
                else
                {
                    Log(string.Format("   [INFO] Status Ubah Password: {0} {1}", passResult.Output, passResult.Error));
                }

                // 6. Config Auto-Logon (Bypass Screen Login)
                if (chkAutoLogon.Checked)
                {
                    Log(string.Format("6. Mengonfigurasi Windows Auto-Logon (Bypass Login) untuk User '{0}'...", Environment.UserName));
                    var autoResult = NetworkServices.ConfigureAutoLogon(newPassword);
                    if (autoResult.Success)
                    {
                        Log("   [OK] Auto-Logon Windows Berhasil Diaktifkan!");
                    }
                    else
                    {
                        Log(string.Format("   [ERROR] Gagal mengaktifkan Auto-Logon: {0}", autoResult.Error));
                    }
                }
                else if (chkDisableAutoLogon.Checked)
                {
                    Log("6. Mematikan Windows Auto-Logon (Meminta Password saat Login)...");
                    var disableResult = NetworkServices.DisableAutoLogon();
                    if (disableResult.Success)
                    {
                        Log("   [OK] Auto-Logon Windows Berhasil Dimatikan!");
                    }
                    else
                    {
                        Log(string.Format("   [ERROR] Gagal mematikan Auto-Logon: {0}", disableResult.Error));
                    }
                }
            }

            Log("=================================================");
            Log("PENGATURAN SELESAI!");
            Log("Catatan: Perubahan Nama Komputer & Workgroup memerlukan Restart PC.");

            SetFormEnabled(true);

            DialogResult restartConfirm = MessageBox.Show(
                "Pengaturan telah selesai diterapkan!\n\nPerubahan Nama Komputer, Workgroup, & Auto-Logon akan aktif sepenuhnya setelah PC di-restart.\n\nApakah Anda ingin me-restart komputer sekarang?",
                "Pengaturan Selesai - Restart PC?",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Information
            );

            if (restartConfirm == DialogResult.Yes)
            {
                Log("Memulai prosedur restart PC dalam 5 detik...");
                NetworkServices.RestartSystem(5, "Restart by AirV");
            }
        }

        private void SetFormEnabled(bool enabled)
        {
            grpConfig.Enabled = enabled;
            grpAction.Enabled = enabled;
        }

        private void Log(string message)
        {
            string timeStamp = DateTime.Now.ToString("HH:mm:ss");
            txtLog.AppendText(string.Format("[{0}] {1}\n", timeStamp, message));
            txtLog.SelectionStart = txtLog.Text.Length;
            txtLog.ScrollToCaret();
        }

        private void ApplyModernStyling()
        {
            // Set Rounded Region & Paint untuk GroupBox & Controls
            int radius = 16;
            
            grpConfig.Paint += (s, e) => DrawRoundedCard(grpConfig, e.Graphics, "Basic Configuration", radius);
            grpAction.Paint += (s, e) => DrawRoundedCard(grpAction, e.Graphics, "Set Computer Number", radius);
            grpLog.Paint += (s, e) => DrawRoundedCard(grpLog, e.Graphics, "Activity Log", radius);

            // ToolTip & Event Hover untuk Button
            ToolTip toolTip = new ToolTip();
            toolTip.SetToolTip(btnAddLab, "Tambah Profil LAB Baru");
            toolTip.SetToolTip(btnEditLab, "Edit Profil LAB Terpilih");
            toolTip.SetToolTip(btnDeleteLab, "Hapus Profil LAB Terpilih");
            toolTip.SetToolTip(btnRefreshIf, "Refresh Daftar Interface Jaringan");

            SetupButtonHover(btnApply, Color.FromArgb(15, 118, 110), Color.FromArgb(13, 148, 136), Color.White, "Apply", 18);
            SetupButtonHover(btnAddLab, Color.FromArgb(241, 245, 249), Color.FromArgb(203, 213, 225), Color.FromArgb(51, 65, 85), "➕", 10);
            SetupButtonHover(btnEditLab, Color.FromArgb(241, 245, 249), Color.FromArgb(203, 213, 225), Color.FromArgb(51, 65, 85), "✏️", 10);
            SetupButtonHover(btnDeleteLab, Color.FromArgb(254, 242, 242), Color.FromArgb(254, 202, 202), Color.FromArgb(185, 28, 28), "🗑️", 10);
            SetupButtonHover(btnRefreshIf, Color.FromArgb(241, 245, 249), Color.FromArgb(203, 213, 225), Color.FromArgb(51, 65, 85), "🔄", 10);
            SetupButtonHover(btnToggleShowPassword, Color.White, Color.FromArgb(241, 245, 249), Color.FromArgb(100, 116, 139), btnToggleShowPassword.Text, 8);

            // Terapkan Rounded Region pada GroupBox Card & Control Input agar benar-benar Rounded 100%
            ApplyRoundedRegionToControl(grpConfig, 16);
            ApplyRoundedRegionToControl(grpAction, 16);
            ApplyRoundedRegionToControl(grpLog, 16);

            ApplyRoundedRegionToControl(txtPrefix, 8);
            ApplyRoundedRegionToControl(txtSubnet, 8);
            ApplyRoundedRegionToControl(txtDns, 8);
            ApplyRoundedRegionToControl(txtGateway, 8);
            ApplyRoundedRegionToControl(txtWorkgroup, 8);
            ApplyRoundedRegionToControl(txtNewPassword, 8);
            ApplyRoundedRegionToControl(txtPcNum, 12);
            ApplyRoundedRegionToControl(txtLog, 12);
            ApplyRoundedRegionToControl(cmbLabSelect, 8);
            ApplyRoundedRegionToControl(cmbInterface, 8);
        }

        private void SetupButtonHover(Button btn, Color normalBg, Color hoverBg, Color textColor, string text, int radius)
        {
            bool isHovered = false;
            btn.MouseEnter += (s, e) => { isHovered = true; btn.Invalidate(); };
            btn.MouseLeave += (s, e) => { isHovered = false; btn.Invalidate(); };
            btn.Paint += (s, e) => DrawRoundedButton(btn, e.Graphics, isHovered ? hoverBg : normalBg, textColor, btn.Text, radius);
        }

        private void ApplyRoundedRegionToControl(Control ctrl, int radius)
        {
            Rectangle rect = new Rectangle(0, 0, ctrl.Width, ctrl.Height);
            using (System.Drawing.Drawing2D.GraphicsPath path = GetRoundedPath(rect, radius))
            {
                ctrl.Region = new Region(path);
            }
        }

        private void DrawRoundedCard(GroupBox box, Graphics g, string title, int radius)
        {
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.Clear(box.Parent.BackColor);

            Rectangle rect = new Rectangle(0, 0, box.Width - 1, box.Height - 1);
            using (System.Drawing.Drawing2D.GraphicsPath path = GetRoundedPath(rect, radius))
            {
                using (SolidBrush bgBrush = new SolidBrush(box.BackColor))
                {
                    g.FillPath(bgBrush, path);
                }
                using (Pen borderPen = new Pen(Color.FromArgb(226, 232, 240), 1.5f))
                {
                    g.DrawPath(borderPen, path);
                }
            }

            if (!string.IsNullOrEmpty(title))
            {
                using (Font font = new Font("Segoe UI", 10.5f, FontStyle.Bold))
                using (SolidBrush textBrush = new SolidBrush(Color.FromArgb(30, 41, 59)))
                {
                    g.DrawString(title, font, textBrush, new PointF(16, 10));
                }
            }
        }

        private void DrawRoundedButton(Button btn, Graphics g, Color bgColor, Color textColor, string text, int radius)
        {
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.Clear(btn.Parent.BackColor);

            Rectangle rect = new Rectangle(0, 0, btn.Width - 1, btn.Height - 1);
            using (System.Drawing.Drawing2D.GraphicsPath path = GetRoundedPath(rect, radius))
            {
                using (SolidBrush bgBrush = new SolidBrush(bgColor))
                {
                    g.FillPath(bgBrush, path);
                }
                using (Pen borderPen = new Pen(Color.FromArgb(203, 213, 225), 1f))
                {
                    g.DrawPath(borderPen, path);
                }

                TextRenderer.DrawText(
                    g,
                    text,
                    btn.Font,
                    rect,
                    textColor,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
                );
            }
        }

        private System.Drawing.Drawing2D.GraphicsPath GetRoundedPath(Rectangle rect, int radius)
        {
            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            int diameter = radius * 2;
            Rectangle arc = new Rectangle(rect.X, rect.Y, diameter, diameter);

            // Top Left
            path.AddArc(arc, 180, 90);

            // Top Right
            arc.X = rect.Right - diameter;
            path.AddArc(arc, 270, 90);

            // Bottom Right
            arc.Y = rect.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            // Bottom Left
            arc.X = rect.X;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }

        private void lblCurrentHostname_Click(object sender, EventArgs e)
        {

        }

        private void grpConfig_Enter(object sender, EventArgs e)
        {

        }
    }
}
