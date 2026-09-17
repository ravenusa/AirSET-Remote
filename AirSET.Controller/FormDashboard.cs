using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using AirSET.Core.Models;
using devIPsett;

namespace AirSET.Controller
{
    public partial class FormDashboard : Form
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern Int32 SendMessage(IntPtr hWnd, int msg, int wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);
        private const int EM_SETCUEBANNER = 0x1501;

        private NetworkScanner scanner;
        private List<LabProfile> labProfiles;
        private Dictionary<string, DiscoveryMessage> discoveredDevices = new Dictionary<string, DiscoveryMessage>();

        private bool isUpdatingFields = false;
        private bool isLecturerMode = true;
        private Button btnAdminMode;

        public FormDashboard()
        {
            InitializeComponent();

            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            }
            catch { }

            scanner = new NetworkScanner();
            scanner.OnDeviceDiscovered += Scanner_OnDeviceDiscovered;
            scanner.OnLog += Log;

            InitializeExtraTabs();
            ApplyModernTheme();

            // Pasang event TextChanged untuk auto-detect perubahan pada TextBox
            txtBaseIp.TextChanged += ConfigTextBox_TextChanged;
            txtSubnet.TextChanged += ConfigTextBox_TextChanged;
            txtGateway.TextChanged += ConfigTextBox_TextChanged;
            txtDns.TextChanged += ConfigTextBox_TextChanged;
            txtPrefix.TextChanged += ConfigTextBox_TextChanged;
            txtWorkgroup.TextChanged += ConfigTextBox_TextChanged;

            LoadLabProfiles();
            PopulateNetworkInterfaces();
            UpdatePasswordRelatedControls();

            // Atur placeholder / watermark text pada input IP langsung
            SendMessage(txtDirectIp.Handle, EM_SETCUEBANNER, 0, "Ex: 192.168.10.99");
            SendMessage(txtShieldDirectIp.Handle, EM_SETCUEBANNER, 0, "Ex: 192.168.11.213");

            // Setup Shortcut Keyboard Global & Seleksi Block Multi-Client
            SetupKeyboardShortcutsAndMultiSelect();
        }

        private void SetupKeyboardShortcutsAndMultiSelect()
        {
            this.KeyPreview = true;
            this.KeyDown += (s, e) =>
            {
                // Tekan 'R' atau 'Ctrl+R' untuk Refresh / Scan PC (kecuali sedang mengetik di TextBox/ComboBox)
                if (e.KeyCode == Keys.R && !e.Alt)
                {
                    Control active = this.ActiveControl;
                    while (active is ContainerControl container && container.ActiveControl != null)
                    {
                        active = container.ActiveControl;
                    }

                    if (!(active is TextBox) && !(active is ComboBox))
                    {
                        if (btnScan.Enabled)
                        {
                            e.Handled = true;
                            e.SuppressKeyPress = true;
                            Log("Shortcut [R] ditekan: Memulai Refresh Scan Komputer...");
                            btnScan_Click(btnScan, EventArgs.Empty);
                        }
                    }
                }
            };

            // Pasang event Multi-Selection (Block / Drag / Shift-Click) untuk mencentang otomatis
            AttachBlockSelectionHandler(dgvClients, 0);
            AttachBlockSelectionHandler(dgvShieldClients, 0);
            AttachBlockSelectionHandler(dgvPowerClients, 0);
            AttachBlockSelectionHandler(dgvLaunchClients, 0);
            AttachBlockSelectionHandler(dgvFileClients, 0);
            AttachBlockSelectionHandler(dgvInventoryClients, 0);
        }

        private void AttachBlockSelectionHandler(DataGridView dgv, int checkColIndex)
        {
            if (dgv == null) return;
            dgv.MultiSelect = true;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            bool isUpdatingSelection = false;

            // Saat baris di-block (drag mouse / Shift+Click / Ctrl+Click)
            dgv.SelectionChanged += (s, e) =>
            {
                if (isUpdatingSelection) return;
                if (dgv.SelectedRows.Count > 1)
                {
                    isUpdatingSelection = true;
                    try
                    {
                        foreach (DataGridViewRow r in dgv.SelectedRows)
                        {
                            r.Cells[checkColIndex].Value = true;
                        }
                        dgv.EndEdit();
                    }
                    finally
                    {
                        isUpdatingSelection = false;
                    }
                }
            };

            // Dukungan tombol Spacebar: Toggle centang semua baris yang ter-block
            dgv.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Space && dgv.SelectedRows.Count > 0)
                {
                    e.Handled = true;
                    bool targetState = true;
                    // Cek baris pertama yang dipilih untuk toggle
                    var firstRow = dgv.SelectedRows[0];
                    if (firstRow.Cells[checkColIndex].Value is bool b) targetState = !b;

                    isUpdatingSelection = true;
                    try
                    {
                        foreach (DataGridViewRow r in dgv.SelectedRows)
                        {
                            r.Cells[checkColIndex].Value = targetState;
                        }
                        dgv.EndEdit();
                    }
                    finally
                    {
                        isUpdatingSelection = false;
                    }
                }
            };
        }

        private void PopulateNetworkInterfaces()
        {
            try
            {
                string previousSelection = cmbInterface.SelectedItem != null ? cmbInterface.SelectedItem.ToString() : null;
                cmbInterface.Items.Clear();

                // Standar opsi rekomendasi & umum
                var list = new List<string>
                {
                    "[Auto-Detect LAN (Active)]",
                    "Ethernet",
                    "Local Area Connection",
                    "Wi-Fi"
                };

                // Deteksi interface fisik / virtual yang ada di sistem
                try
                {
                    var nics = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces();
                    foreach (var nic in nics)
                    {
                        if (nic.NetworkInterfaceType != System.Net.NetworkInformation.NetworkInterfaceType.Loopback &&
                            nic.NetworkInterfaceType != System.Net.NetworkInformation.NetworkInterfaceType.Tunnel)
                        {
                            if (!list.Contains(nic.Name))
                            {
                                list.Add(nic.Name);
                            }
                        }
                    }
                }
                catch
                {
                    // Abaikan jika wmi / network permission terbatas
                }

                foreach (var item in list)
                {
                    cmbInterface.Items.Add(item);
                }

                if (!string.IsNullOrEmpty(previousSelection) && cmbInterface.Items.Contains(previousSelection))
                {
                    cmbInterface.SelectedItem = previousSelection;
                }
                else if (cmbInterface.Items.Count > 0)
                {
                    cmbInterface.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                Log("Gagal memuat adapter jaringan: " + ex.Message);
            }
        }

        private void btnRefreshInterface_Click(object sender, EventArgs e)
        {
            PopulateNetworkInterfaces();
            Log("Daftar interface jaringan berhasil diperbarui.");
        }

        private void FormDashboard_Load(object sender, EventArgs e)
        {
            AdjustTab1Layout();
            AdjustTab2Layout();
            Log("AirSET Controller siap. Silakan klik 'Scan Otomatis' untuk mendeteksi PC Client.");
        }

        private void LoadLabProfiles(string selectCode = null)
        {
            try
            {
                isUpdatingFields = true;
                cmbLab.Items.Clear();
                labProfiles = LabData.GetProfiles();
                int selectedIndexToSet = 0;

                if (labProfiles != null)
                {
                    for (int i = 0; i < labProfiles.Count; i++)
                    {
                        var p = labProfiles[i];
                        cmbLab.Items.Add(p);
                        if (!string.IsNullOrEmpty(selectCode) && string.Equals(p.Code, selectCode, StringComparison.OrdinalIgnoreCase))
                        {
                            selectedIndexToSet = i;
                        }
                    }
                }
                if (cmbLab.Items.Count > 0)
                {
                    cmbLab.SelectedIndex = selectedIndexToSet;
                }
            }
            catch (Exception ex)
            {
                Log("Peringatan: Gagal memuat profil lab: " + ex.Message);
            }
            finally
            {
                isUpdatingFields = false;
            }
        }

        private void cmbLab_SelectedIndexChanged(object sender, EventArgs e)
        {
            LabProfile sel = cmbLab.SelectedItem as LabProfile;
            if (sel != null)
            {
                isUpdatingFields = true;
                try
                {
                    txtBaseIp.Text = sel.BaseIp;
                    txtSubnet.Text = sel.Subnet;
                    txtGateway.Text = sel.Gateway;
                    txtDns.Text = sel.Dns;
                    txtPrefix.Text = sel.HostnamePrefix;
                    txtWorkgroup.Text = sel.Workgroup;

                    bool isCustom = LabData.IsCustomProfile(sel.Code);

                    // 20 Lab default bawaan sistem di-ReadOnly agar aman
                    // Lab custom diizinkan untuk diedit langsung di TextBox
                    SetLabInputsReadOnly(!isCustom);

                    btnSaveLab.Visible = false;
                    btnDeleteLab.Visible = isCustom;

                    RecalculateTargetPreviews();
                }
                finally
                {
                    isUpdatingFields = false;
                }
            }
        }

        private void SetLabInputsReadOnly(bool readOnly)
        {
            txtBaseIp.ReadOnly = readOnly;
            txtSubnet.ReadOnly = readOnly;
            txtGateway.ReadOnly = readOnly;
            txtDns.ReadOnly = readOnly;
            txtPrefix.ReadOnly = readOnly;
            txtWorkgroup.ReadOnly = readOnly;

            Color bgColor = readOnly ? Color.FromArgb(241, 245, 249) : Color.White;
            txtBaseIp.BackColor = bgColor;
            txtSubnet.BackColor = bgColor;
            txtGateway.BackColor = bgColor;
            txtDns.BackColor = bgColor;
            txtPrefix.BackColor = bgColor;
            txtWorkgroup.BackColor = bgColor;
        }

        private void ConfigTextBox_TextChanged(object sender, EventArgs e)
        {
            if (isUpdatingFields) return;

            RecalculateTargetPreviews();

            if (cmbLab.SelectedItem is LabProfile sel && LabData.IsCustomProfile(sel.Code))
            {
                // Cek apakah ada perbedaan dengan data profil
                bool changed = (txtBaseIp.Text != sel.BaseIp ||
                                txtSubnet.Text != sel.Subnet ||
                                txtGateway.Text != sel.Gateway ||
                                txtDns.Text != sel.Dns ||
                                txtPrefix.Text != sel.HostnamePrefix ||
                                txtWorkgroup.Text != sel.Workgroup);

                btnSaveLab.Visible = changed;
            }
        }

        private async void btnSaveLab_Click(object sender, EventArgs e)
        {
            if (cmbLab.SelectedItem is LabProfile selectedProfile)
            {
                if (!LabData.IsCustomProfile(selectedProfile.Code))
                {
                    MessageBox.Show("LAB Bawaan sistem tidak dapat diedit!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var confirm = MessageBox.Show(
                    string.Format("Apakah Anda yakin ingin menyimpan perubahan konfigurasi untuk LAB '{0}' ({1}) ke Cloud Gist & Lokal?", 
                        selectedProfile.Name, selectedProfile.Code),
                    "Konfirmasi Simpan Perubahan LAB",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (confirm != DialogResult.Yes) return;

                btnSaveLab.Enabled = false;
                try
                {
                    selectedProfile.BaseIp = txtBaseIp.Text.Trim();
                    selectedProfile.Subnet = txtSubnet.Text.Trim();
                    selectedProfile.Gateway = txtGateway.Text.Trim();
                    selectedProfile.Dns = txtDns.Text.Trim();
                    selectedProfile.HostnamePrefix = txtPrefix.Text.Trim();
                    selectedProfile.Workgroup = txtWorkgroup.Text.Trim();

                    Log(string.Format("Menyimpan konfigurasi LAB '{0}' ke Cloud Gist & Lokal...", selectedProfile.Name));
                    bool savedCloud = await LabData.SaveCustomProfileAsync(selectedProfile);
                    if (savedCloud)
                    {
                        Log(string.Format("   [OK] Perubahan LAB '{0}' berhasil diunggah ke Cloud Gist!", selectedProfile.Name));
                    }
                    else
                    {
                        Log(string.Format("   [INFO] Perubahan LAB '{0}' disimpan ke file JSON lokal.", selectedProfile.Name));
                    }

                    btnSaveLab.Visible = false;
                    MessageBox.Show("Konfigurasi LAB berhasil diperbarui & disimpan!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                finally
                {
                    btnSaveLab.Enabled = true;
                }
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

        private async void btnDeleteLab_Click(object sender, EventArgs e)
        {
            if (cmbLab.SelectedItem is LabProfile selectedProfile)
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

        private void SetAllScanButtonsState(bool scanning)
        {
            Button[] scanButtons = new Button[] { btnScan, btnShieldScan, btnPowerScan, btnLaunchScan, btnFileScan, btnInventoryScan, btnRefreshScreensOnce };
            string text = scanning ? "🔍 Scanning..." : "🔍 Scan Otomatis (Auto)";
            foreach (var b in scanButtons)
            {
                if (b != null)
                {
                    b.Enabled = !scanning;
                    if (b != btnRefreshScreensOnce)
                    {
                        b.Text = text;
                    }
                }
            }
        }

        private void UpdateAllTotalLabels()
        {
            lblTotalOnline.Text = string.Format("Total PC Terdeteksi: {0}", dgvClients.Rows.Count);
            lblShieldTotal.Text = string.Format("Total PC Terdeteksi: {0}", dgvShieldClients.Rows.Count);
            if (lblPowerTotal != null) lblPowerTotal.Text = string.Format("Total PC Terdeteksi: {0}", dgvPowerClients?.Rows.Count ?? 0);
            if (lblLaunchTotal != null) lblLaunchTotal.Text = string.Format("Total PC Terdeteksi: {0}", dgvLaunchClients?.Rows.Count ?? 0);
            if (lblFileTotal != null) lblFileTotal.Text = string.Format("Total PC Terdeteksi: {0}", dgvFileClients?.Rows.Count ?? 0);
            if (lblInventoryTotal != null) lblInventoryTotal.Text = string.Format("Total PC Terdeteksi: {0}", dgvInventoryClients?.Rows.Count ?? 0);
            if (lblLiveScreenTotal != null) lblLiveScreenTotal.Text = string.Format("Total PC Terpantau: {0}", pcScreenCards.Count);
        }

        private async void btnScan_Click(object sender, EventArgs e)
        {
            SetAllScanButtonsState(true);
            dgvClients.Rows.Clear();
            dgvShieldClients.Rows.Clear();
            dgvPowerClients?.Rows.Clear();
            dgvLaunchClients?.Rows.Clear();
            dgvFileClients?.Rows.Clear();
            dgvInventoryClients?.Rows.Clear();
            ClearAllPcScreenCards();
            discoveredDevices.Clear();

            try
            {
                // Ekstrak base IP dari profile lab (misal: "10.22.1") atau subnet lokal
                string labBase = txtBaseIp.Text.Trim();
                int lastDot = labBase.LastIndexOf('.');
                string baseSubnet = (lastDot > 0) ? labBase.Substring(0, lastDot) : null;

                // 1. Jalankan UDP Broadcast Multi-Wave (Ditingkatkan ke 3000ms agar seluruh 70 PC lab sempat membalas)
                var udpTask = scanner.BroadcastDiscoveryAsync(3000);

                // 2. Bersamaan (Parallel), jalankan TCP Subnet Fast-Sweep ke seluruh range IP lokal adapter
                // Ditingkatkan timeout probe ke 1000ms dengan 100 paralelism untuk memastikan respon andal
                var tcpScanTask = scanner.ScanSubnetRangeAsync(baseSubnet, probeTimeoutMs: 1000, maxDegreeOfParallelism: 100);

                await Task.WhenAll(udpTask, tcpScanTask);
            }
            finally
            {
                SetAllScanButtonsState(false);
                UpdateAllTotalLabels();
            }
        }

        private async void btnDirectAdd_Click(object sender, EventArgs e)
        {
            await ExecuteDirectAddAsync(txtDirectIp.Text.Trim(), btnDirectAdd);
        }

        private async void btnShieldDirectAdd_Click(object sender, EventArgs e)
        {
            await ExecuteDirectAddAsync(txtShieldDirectIp.Text.Trim(), btnShieldDirectAdd);
        }

        private async Task ExecuteDirectAddAsync(string ip, Button btn)
        {
            if (string.IsNullOrEmpty(ip))
            {
                MessageBox.Show("Masukkan IP Address tujuan!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btn.Enabled = false;
            btn.Text = "Checking...";
            try
            {
                await scanner.ScanDirectIpAsync(ip, 2500);
            }
            finally
            {
                btn.Enabled = true;
                btn.Text = "+ Ping / Add IP";
                UpdateAllTotalLabels();
            }
        }

        private void Scanner_OnDeviceDiscovered(DiscoveryMessage msg, string ipAddress)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => Scanner_OnDeviceDiscovered(msg, ipAddress)));
                return;
            }

            if (!discoveredDevices.ContainsKey(ipAddress))
            {
                discoveredDevices[ipAddress] = msg;
                int extractedNumber = ExtractNumberFromHostname(msg.Hostname);

                // Tambahkan adapter client ke dropdown interface jika belum ada
                try
                {
                    if (!string.IsNullOrEmpty(msg.CurrentAdapter) && !cmbInterface.Items.Contains(msg.CurrentAdapter))
                    {
                        cmbInterface.Items.Add(msg.CurrentAdapter);
                    }
                }
                catch { }

                string osCaption = !string.IsNullOrEmpty(msg.OsCaption) ? msg.OsCaption : "Windows";
                string shieldStatus = !string.IsNullOrEmpty(msg.ShieldStatus) ? msg.ShieldStatus : "Unknown";

                // Sinkronkan ke Tab 1: dgvClients
                try
                {
                    if (dgvClients != null)
                    {
                        string baseIpStr = txtBaseIp?.Text?.Trim() ?? "";
                        string prefixStr = txtPrefix?.Text?.Trim() ?? "";
                        int rowIndex = dgvClients.Rows.Add(
                            true,
                            extractedNumber,
                            msg.Hostname,
                            ipAddress,
                            string.Format("{0}.{1}", baseIpStr, extractedNumber),
                            string.Format("{0}{1:D2}", prefixStr, extractedNumber),
                            "Online"
                        );
                        dgvClients.Rows[rowIndex].Cells["colStatus"].Style.ForeColor = Color.DarkGreen;
                        dgvClients.Rows[rowIndex].Cells["colStatus"].Style.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                    }
                }
                catch (Exception ex)
                {
                    Log(string.Format("[Warning] Gagal menambah ke Tab 1: {0}", ex.Message));
                }

                // Sinkronkan ke Tab 2: dgvShieldClients
                try
                {
                    if (dgvShieldClients != null)
                    {
                        int shieldRowIndex = dgvShieldClients.Rows.Add(
                            true,
                            extractedNumber,
                            msg.Hostname,
                            ipAddress,
                            osCaption,
                            shieldStatus,
                            "Ready"
                        );

                        // Beri warna status proteksi
                        if (shieldStatus.IndexOf("Lock", StringComparison.OrdinalIgnoreCase) >= 0 ||
                            shieldStatus.IndexOf("Protect", StringComparison.OrdinalIgnoreCase) >= 0 ||
                            shieldStatus.IndexOf("Freeze", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            dgvShieldClients.Rows[shieldRowIndex].Cells["colShieldStatus"].Style.ForeColor = Color.DarkGreen;
                        }
                        else if (shieldStatus.IndexOf("Unlock", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                 shieldStatus.IndexOf("Thaw", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            dgvShieldClients.Rows[shieldRowIndex].Cells["colShieldStatus"].Style.ForeColor = Color.DarkOrange;
                        }
                        else
                        {
                            dgvShieldClients.Rows[shieldRowIndex].Cells["colShieldStatus"].Style.ForeColor = Color.FromArgb(100, 116, 139);
                        }
                        dgvShieldClients.Rows[shieldRowIndex].Cells["colShieldStatus"].Style.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                    }
                }
                catch (Exception ex)
                {
                    Log(string.Format("[Warning] Gagal menambah ke Tab 2: {0}", ex.Message));
                }

                // Sinkronkan ke Tab 3: dgvPowerClients
                try
                {
                    if (dgvPowerClients != null)
                    {
                        int pIdx = dgvPowerClients.Rows.Add(
                            true,
                            extractedNumber,
                            msg.Hostname,
                            ipAddress,
                            "Online"
                        );
                        dgvPowerClients.Rows[pIdx].Cells[4].Style.ForeColor = Color.DarkGreen;
                        dgvPowerClients.Rows[pIdx].Cells[4].Style.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                    }
                }
                catch (Exception ex)
                {
                    Log(string.Format("[Warning] Gagal menambah ke Tab 3: {0}", ex.Message));
                }

                // Sinkronkan ke Tab 4: dgvLaunchClients
                try
                {
                    if (dgvLaunchClients != null)
                    {
                        int lIdx = dgvLaunchClients.Rows.Add(
                            true,
                            extractedNumber,
                            msg.Hostname,
                            ipAddress,
                            "Ready"
                        );
                        dgvLaunchClients.Rows[lIdx].Cells[4].Style.ForeColor = Color.DarkGreen;
                        dgvLaunchClients.Rows[lIdx].Cells[4].Style.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                    }
                }
                catch (Exception ex)
                {
                    Log(string.Format("[Warning] Gagal menambah ke Tab 4: {0}", ex.Message));
                }

                // Sinkronkan ke Tab 5: dgvFileClients
                try
                {
                    if (dgvFileClients != null)
                    {
                        int fIdx = dgvFileClients.Rows.Add(
                            true,
                            extractedNumber,
                            msg.Hostname,
                            ipAddress,
                            "Ready"
                        );
                        dgvFileClients.Rows[fIdx].Cells[4].Style.ForeColor = Color.DarkGreen;
                        dgvFileClients.Rows[fIdx].Cells[4].Style.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                    }
                }
                catch (Exception ex)
                {
                    Log(string.Format("[Warning] Gagal menambah ke Tab 5: {0}", ex.Message));
                }

                // Sinkronkan ke Tab 6: dgvInventoryClients
                try
                {
                    if (dgvInventoryClients != null)
                    {
                        int invIdx = dgvInventoryClients.Rows.Add(
                            true,
                            extractedNumber,
                            msg.Hostname,
                            ipAddress,
                            "Ready"
                        );
                        dgvInventoryClients.Rows[invIdx].Cells[4].Style.ForeColor = Color.DarkGreen;
                        dgvInventoryClients.Rows[invIdx].Cells[4].Style.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                    }
                }
                catch (Exception ex)
                {
                    Log(string.Format("[Warning] Gagal menambah ke Tab 6: {0}", ex.Message));
                }

                // Sinkronkan ke Tab 7: Live Screen View
                try
                {
                    AddOrUpdatePcScreenCard(ipAddress, msg.Hostname, extractedNumber);
                }
                catch (Exception ex)
                {
                    Log(string.Format("[Warning] Gagal menambah ke Tab 7: {0}", ex.Message));
                }

                // Simpan MAC Address ke file history lokal untuk Wake-on-LAN
                if (!string.IsNullOrEmpty(msg.MacAddress))
                {
                    SaveMacToWolHistory(msg.Hostname, ipAddress, msg.MacAddress);
                }

                UpdateAllTotalLabels();

                string adapterInfo = !string.IsNullOrEmpty(msg.CurrentAdapter) ? string.Format(" [Adapter: {0}]", msg.CurrentAdapter) : "";
                Log(string.Format("PC Terdeteksi: {0} ({1}){2} - OS: {3} - Shield: {4}", msg.Hostname, ipAddress, adapterInfo, osCaption, shieldStatus));
            }
        }

        private void SaveMacToWolHistory(string hostname, string ip, string mac)
        {
            try
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wol_history.txt");
                var lines = File.Exists(path) ? File.ReadAllLines(path).ToList() : new List<string>();
                string newEntry = string.Format("{0}|{1}|{2}", mac.ToUpper().Trim(), hostname ?? "PC", ip ?? "");

                // Cek jika MAC sudah ada di file, update entri
                int existingIdx = lines.FindIndex(l => l.StartsWith(mac.ToUpper().Trim() + "|"));
                if (existingIdx >= 0)
                {
                    lines[existingIdx] = newEntry;
                }
                else
                {
                    lines.Add(newEntry);
                }
                File.WriteAllLines(path, lines);
            }
            catch { }
        }

        private int ExtractNumberFromHostname(string hostname)
        {
            if (string.IsNullOrEmpty(hostname)) return dgvClients.Rows.Count + 1;
            var match = Regex.Match(hostname, @"\d+");
            if (match.Success && int.TryParse(match.Value, out int num))
            {
                return num;
            }
            return dgvClients.Rows.Count + 1;
        }

        private void RecalculateTargetPreviews()
        {
            string baseIp = txtBaseIp.Text.Trim();
            string prefix = txtPrefix.Text.Trim();

            foreach (DataGridViewRow row in dgvClients.Rows)
            {
                if (row.Cells["colNumber"].Value != null && int.TryParse(row.Cells["colNumber"].Value.ToString(), out int pcNum))
                {
                    row.Cells["colTargetIp"].Value = string.Format("{0}.{1}", baseIp, pcNum);
                    row.Cells["colTargetHostname"].Value = string.Format("{0}{1:D2}", prefix, pcNum);
                }
            }
        }

        private async void btnPushConfig_Click(object sender, EventArgs e)
        {
            int selectedCount = 0;
            foreach (DataGridViewRow row in dgvClients.Rows)
            {
                if (Convert.ToBoolean(row.Cells["colCheck"].Value)) selectedCount++;
            }

            if (selectedCount == 0)
            {
                MessageBox.Show("Pilih minimal satu komputer untuk dikonfigurasi!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show(
                string.Format("Apakah Anda yakin ingin mengirim konfigurasi ke {0} komputer sekaligus?", selectedCount),
                "Konfirmasi Batch Remote Push",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (confirm != DialogResult.Yes) return;

            btnPushConfig.Enabled = false;
            btnScan.Enabled = false;

            string baseIp = txtBaseIp.Text.Trim();
            string subnet = txtSubnet.Text.Trim();
            string gateway = txtGateway.Text.Trim();
            string dns = txtDns.Text.Trim();
            string prefix = txtPrefix.Text.Trim();
            string workgroup = txtWorkgroup.Text.Trim();
            bool changePass = chkChangePass.Checked;
            string newPass = txtNewPass.Text.Trim();
            bool autoLogon = chkAutoLogon.Checked;
            bool disableLogon = chkDisableLogon.Checked;
            bool restartAfter = chkRestart.Checked;
            string targetInterface = cmbInterface.SelectedItem != null ? cmbInterface.SelectedItem.ToString() : "Auto";

            if (changePass && string.IsNullOrEmpty(newPass))
            {
                MessageBox.Show(
                    "Password baru tidak boleh kosong!\n\nSilakan masukkan password Windows pada kolom 'Password Baru' terlebih dahulu sebelum melakukan push konfigurasi.",
                    "Password Wajib Diisi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                txtNewPass.Focus();
                btnPushConfig.Enabled = true;
                btnScan.Enabled = true;
                return;
            }

            Log("=================================================");
            Log(string.Format("Memulai Batch Push serentak ke {0} PC...", selectedCount));

            var targetRows = new List<DataGridViewRow>();
            foreach (DataGridViewRow row in dgvClients.Rows)
            {
                if (Convert.ToBoolean(row.Cells["colCheck"].Value))
                {
                    targetRows.Add(row);
                    row.Cells["colStatus"].Value = "Queued...";
                    row.Cells["colStatus"].Style.ForeColor = Color.DarkOrange;
                }
            }

            var throttle = new SemaphoreSlim(15);
            var tasks = targetRows.Select(async row =>
            {
                await throttle.WaitAsync();
                try
                {
                    string clientIp = row.Cells["colIp"].Value.ToString();
                    int pcNum = Convert.ToInt32(row.Cells["colNumber"].Value);
                    string hostname = row.Cells["colHostname"].Value.ToString();

                    this.BeginInvoke((Action)(() =>
                    {
                        row.Cells["colStatus"].Value = "Applying...";
                        row.Cells["colStatus"].Style.ForeColor = Color.DarkOrange;
                    }));

                    var payload = new RemoteConfigPayload
                    {
                        Action = "APPLY_CONFIG",
                        PcNumber = pcNum,
                        BaseIp = baseIp,
                        Subnet = subnet,
                        Gateway = gateway,
                        Dns = dns,
                        Prefix = prefix,
                        Workgroup = workgroup,
                        TargetInterface = targetInterface,
                        ChangePassword = changePass,
                        NewPassword = newPass,
                        AutoLogon = autoLogon,
                        DisableAutoLogon = disableLogon,
                        RestartAfterApply = restartAfter
                    };

                    var resp = await scanner.SendRemoteConfigAsync(clientIp, payload);

                    this.BeginInvoke((Action)(() =>
                    {
                        if (resp.Success)
                        {
                            if (restartAfter)
                            {
                                row.Cells["colStatus"].Value = "Rebooting...";
                                row.Cells["colStatus"].Style.ForeColor = Color.DarkOrange;
                            }
                            else
                            {
                                row.Cells["colStatus"].Value = "Success";
                                row.Cells["colStatus"].Style.ForeColor = Color.DarkGreen;
                            }
                            Log(string.Format("{0} ({1}): {2}", hostname, clientIp, resp.Message));
                        }
                        else
                        {
                            if (restartAfter && resp.Message != null && (resp.Message.Contains("Timeout") || resp.Message.Contains("menutup koneksi")))
                            {
                                row.Cells["colStatus"].Value = "Rebooting...";
                                row.Cells["colStatus"].Style.ForeColor = Color.DarkOrange;
                                Log(string.Format("{0} ({1}): Konfigurasi terkirim dan PC sedang proses restart (Koneksi ditutup oleh Client).", hostname, clientIp));
                            }
                            else
                            {
                                row.Cells["colStatus"].Value = "Failed";
                                row.Cells["colStatus"].Style.ForeColor = Color.Red;
                                Log(string.Format("{0} ({1}): {2}", hostname, clientIp, resp.Message));
                            }
                        }
                    }));
                }
                finally
                {
                    throttle.Release();
                }
            });

            await Task.WhenAll(tasks);

            Log("Batch Remote Configuration Selesai!");
            Log("=================================================");
            btnPushConfig.Enabled = true;
            btnScan.Enabled = true;
        }

        private void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            bool check = chkSelectAll.Checked;
            foreach (DataGridViewRow row in dgvClients.Rows)
            {
                row.Cells["colCheck"].Value = check;
            }
        }

        private void chkChangePass_CheckedChanged(object sender, EventArgs e)
        {
            UpdatePasswordRelatedControls();
        }

        private void UpdatePasswordRelatedControls()
        {
            bool isChangePass = chkChangePass.Checked;
            txtNewPass.Enabled = isChangePass;
            chkAutoLogon.Enabled = isChangePass;
            chkDisableLogon.Enabled = isChangePass;

            if (!isChangePass)
            {
                chkAutoLogon.Checked = false;
                chkDisableLogon.Checked = false;
                txtNewPass.Clear();
            }
        }

        private void chkAutoLogon_CheckedChanged(object sender, EventArgs e)
        {
            if (chkAutoLogon.Checked)
            {
                chkDisableLogon.Checked = false;
            }
        }

        private void chkDisableLogon_CheckedChanged(object sender, EventArgs e)
        {
            if (chkDisableLogon.Checked)
            {
                chkAutoLogon.Checked = false;
            }
        }

        private void Log(string msg)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => Log(msg)));
                return;
            }
            txtLog.AppendText(msg + Environment.NewLine);
            txtLog.SelectionStart = txtLog.Text.Length;
            txtLog.ScrollToCaret();
        }

        private void btnChangeAppPass_Click(object sender, EventArgs e)
        {
            using (FormChangePassword formChange = new FormChangePassword())
            {
                if (formChange.ShowDialog(this) == DialogResult.OK)
                {
                    Log("Password login Controller berhasil diperbarui.");
                }
            }
        }

        private void BuildAdminModeControls()
        {
            try
            {
                btnAdminMode = new Button
                {
                    Name = "btnAdminMode",
                    Size = new Size(160, 30),
                    Location = new Point(pnlTop.ClientSize.Width - 175, 12),
                    Anchor = AnchorStyles.Top | AnchorStyles.Right,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                    ForeColor = Color.White,
                    BackColor = Color.FromArgb(13, 148, 136),
                    Cursor = Cursors.Hand,
                    Text = "\uD83D\uDD10 Mode Laboran"
                };
                btnAdminMode.FlatAppearance.BorderSize = 0;
                btnAdminMode.Click += btnAdminMode_Click;

                pnlTop.Controls.Add(btnAdminMode);

                pnlTop.Resize += (s, e) => UpdateTopBarButtonsLayout();
                UpdateTopBarButtonsLayout();
            }
            catch (Exception ex)
            {
                Log("BuildAdminModeControls error: " + ex.Message);
            }
        }


        private void UpdateTopBarButtonsLayout()
        {
            try
            {
                int pad = 12;
                if (isLecturerMode)
                {
                    if (btnChangeAppPass != null) btnChangeAppPass.Visible = false;
                    if (btnAdminMode != null)
                    {
                        btnAdminMode.Location = new Point(pnlTop.ClientSize.Width - btnAdminMode.Width - pad, 12);
                    }
                }
                else
                {
                    if (btnChangeAppPass != null)
                    {
                        btnChangeAppPass.Visible = true;
                        btnChangeAppPass.Location = new Point(pnlTop.ClientSize.Width - btnChangeAppPass.Width - pad, 12);
                    }
                    if (btnAdminMode != null && btnChangeAppPass != null)
                    {
                        btnAdminMode.Location = new Point(btnChangeAppPass.Left - btnAdminMode.Width - 8, 12);
                    }
                }
            }
            catch { }
        }

        private void btnAdminMode_Click(object sender, EventArgs e)
        {
            if (isLecturerMode)
            {
                using (FormLogin login = new FormLogin())
                {
                    if (login.ShowDialog(this) == DialogResult.OK)
                    {
                        SetLecturerMode(false);
                        Log("\uD83D\uDD13 Mode Administrator / Laboran berhasil dibuka.");
                    }
                }
            }
            else
            {
                SetLecturerMode(true);
                Log("\uD83D\uDD12 Kembali ke Mode Pengajar (Dosen / Asisten Dosen).");
            }
        }

        private void SetLecturerMode(bool lecturerMode)
        {
            try
            {
                isLecturerMode = lecturerMode;

                if (isLecturerMode)
                {
                    if (tabMain.TabPages.Contains(tabNetwork)) tabMain.TabPages.Remove(tabNetwork);
                    if (tabMain.TabPages.Contains(tabShield)) tabMain.TabPages.Remove(tabShield);

                    if (tabMain.TabPages.Contains(tabPower))
                    {
                        tabMain.SelectedTab = tabPower;
                    }

                    if (btnAdminMode != null)
                    {
                        btnAdminMode.Text = "\uD83D\uDD10 Mode Laboran";
                        btnAdminMode.BackColor = Color.FromArgb(13, 148, 136);
                    }
                }
                else
                {
                    if (!tabMain.TabPages.Contains(tabNetwork))
                    {
                        tabMain.TabPages.Insert(0, tabNetwork);
                    }
                    if (!tabMain.TabPages.Contains(tabShield))
                    {
                        tabMain.TabPages.Insert(1, tabShield);
                    }

                    if (btnAdminMode != null)
                    {
                        btnAdminMode.Text = "\uD83D\uDD12 Lecturer Mode";
                        btnAdminMode.BackColor = Color.FromArgb(217, 119, 6);
                    }
                }

                // Fitur Batch Wallpaper Changer hanya aktif di Mode Laboran
                if (grpWallpaper != null)
                {
                    grpWallpaper.Enabled = !isLecturerMode;
                    if (isLecturerMode)
                    {
                        grpWallpaper.Text = "🖼️ Batch Wallpaper Changer (🔒 Khusus Mode Laboran)";
                        grpWallpaper.ForeColor = Color.FromArgb(148, 163, 184);
                    }
                    else
                    {
                        grpWallpaper.Text = "🖼️ Batch Wallpaper Changer (Ganti Latar Belakang Layar Siswa)";
                        grpWallpaper.ForeColor = Color.FromArgb(30, 41, 59);
                    }
                }

                // Tombol ke-2 di Preset Quick Launch: Mode Pengajar = Microsoft Edge, Mode Laboran = 
                if (btnEdge != null)
                {
                    if (isLecturerMode)
                    {
                        btnEdge.Text = "🌍 Microsoft Edge";
                        btnEdge.BackColor = Color.FromArgb(30, 41, 59);
                    }
                    else
                    {
                        btnEdge.Text = "Aktivasi Win && Office (Permanent)";
                        btnEdge.BackColor = Color.FromArgb(15, 118, 110);
                    }
                }

                UpdateTopBarButtonsLayout();
            }
            catch (Exception ex)
            {
                Log("SetLecturerMode error: " + ex.Message);
            }
        }
        private void chkShieldSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            bool check = chkShieldSelectAll.Checked;
            foreach (DataGridViewRow row in dgvShieldClients.Rows)
            {
                row.Cells["colShieldCheck"].Value = check;
            }
        }

        private async void btnShieldLock_Click(object sender, EventArgs e)
        {
            await ExecuteShieldActionAsync("LOCK");
        }

        private async void btnShieldUnlock_Click(object sender, EventArgs e)
        {
            await ExecuteShieldActionAsync("UNLOCK");
        }

        private async void btnShieldRestart_Click(object sender, EventArgs e)
        {
            var targetIps = new List<string>();
            foreach (DataGridViewRow row in dgvShieldClients.Rows)
            {
                if (Convert.ToBoolean(row.Cells["colShieldCheck"].Value))
                {
                    string ip = row.Cells["colShieldIp"].Value?.ToString();
                    if (!string.IsNullOrEmpty(ip)) targetIps.Add(ip);
                }
            }

            if (targetIps.Count == 0)
            {
                MessageBox.Show("Pilih minimal satu komputer untuk perintah Restart!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show(
                string.Format("Apakah Anda yakin ingin me-restart {0} PC yang dicentang secara bersamaan?", targetIps.Count),
                "Konfirmasi Remote Restart (Disk Shield)",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );
            if (confirm != DialogResult.Yes) return;

            btnShieldRestart.Enabled = false;
            Log("=================================================");
            Log(string.Format("Mengirim perintah RESTART serentak ke {0} PC dari Tab Disk Shield...", targetIps.Count));

            var throttle = new SemaphoreSlim(15);
            var tasks = targetIps.Select(async ip =>
            {
                await throttle.WaitAsync();
                try
                {
                    var payload = new RemoteConfigPayload { Action = "POWER_REBOOT" };
                    var resp = await scanner.SendRemoteConfigAsync(ip, payload);
                    this.BeginInvoke((Action)(() =>
                    {
                        Log(string.Format("{0} ({1}): {2}", resp.Success ? "Sukses" : "Gagal", ip, resp.Message));
                    }));
                }
                finally
                {
                    throttle.Release();
                }
            });

            await Task.WhenAll(tasks);
            btnShieldRestart.Enabled = true;
            Log("Selesai mengirim perintah restart.");
        }

        private async void btnUwfInstall_Click(object sender, EventArgs e)
        {
            int selectedCount = 0;
            foreach (DataGridViewRow row in dgvShieldClients.Rows)
            {
                if (Convert.ToBoolean(row.Cells["colShieldCheck"].Value)) selectedCount++;
            }

            if (selectedCount == 0)
            {
                MessageBox.Show("Pilih minimal satu komputer untuk instalasi UWF!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show(
                string.Format("Apakah Anda yakin ingin menginstal fitur Unified Write Filter (UWF) pada {0} PC terpilih?\n(PC Client akan restart otomatis setelah instalasi selesai)", selectedCount),
                "Konfirmasi Install UWF",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (confirm != DialogResult.Yes) return;

            btnUwfInstall.Enabled = false;
            btnShieldLock.Enabled = false;
            btnShieldUnlock.Enabled = false;

            Log("=================================================");
            Log(string.Format("Memulai Instalasi UWF serentak pada {0} PC Client...", selectedCount));

            var targetRows = new List<DataGridViewRow>();
            foreach (DataGridViewRow row in dgvShieldClients.Rows)
            {
                if (Convert.ToBoolean(row.Cells["colShieldCheck"].Value))
                {
                    targetRows.Add(row);
                    row.Cells["colShieldAction"].Value = "Queued...";
                    row.Cells["colShieldAction"].Style.ForeColor = Color.DarkOrange;
                }
            }

            var throttle = new SemaphoreSlim(15);
            var tasks = targetRows.Select(async row =>
            {
                await throttle.WaitAsync();
                try
                {
                    string clientIp = row.Cells["colShieldIp"].Value.ToString();
                    string hostname = row.Cells["colShieldHostname"].Value.ToString();

                    this.BeginInvoke((Action)(() =>
                    {
                        row.Cells["colShieldAction"].Value = "Installing...";
                        row.Cells["colShieldAction"].Style.ForeColor = Color.DarkOrange;
                    }));

                    var payload = new RemoteConfigPayload
                    {
                        Action = "SHIELD_ACTION",
                        ShieldCommand = "UWF_INSTALL",
                        RestartAfterApply = true
                    };

                    var resp = await scanner.SendRemoteConfigAsync(clientIp, payload, timeoutMs: 120000);

                    this.BeginInvoke((Action)(() =>
                    {
                        if (resp.Success)
                        {
                            row.Cells["colShieldAction"].Value = "Rebooting...";
                            row.Cells["colShieldAction"].Style.ForeColor = Color.DarkOrange;
                            Log(string.Format("{0} ({1}): {2}", hostname, clientIp, resp.Message));
                        }
                        else
                        {
                            row.Cells["colShieldAction"].Value = "Failed";
                            row.Cells["colShieldAction"].Style.ForeColor = Color.Red;
                            Log(string.Format("{0} ({1}): {2}", hostname, clientIp, resp.Message));
                        }
                    }));
                }
                finally
                {
                    throttle.Release();
                }
            });

            await Task.WhenAll(tasks);

            Log("Perintah Instalasi UWF selesai dikirimkan.");
            Log("=================================================");
            btnUwfInstall.Enabled = true;
            btnUwfUninstall.Enabled = true;
            btnShieldLock.Enabled = true;
            btnShieldUnlock.Enabled = true;
        }

        private async void btnUwfUninstall_Click(object sender, EventArgs e)
        {
            int selectedCount = 0;
            foreach (DataGridViewRow row in dgvShieldClients.Rows)
            {
                if (Convert.ToBoolean(row.Cells["colShieldCheck"].Value)) selectedCount++;
            }

            if (selectedCount == 0)
            {
                MessageBox.Show("Pilih minimal satu komputer untuk uninstall UWF!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show(
                string.Format("Apakah Anda yakin ingin MENG-UNINSTALL fitur Unified Write Filter (UWF) pada {0} PC terpilih?\n(UWF akan dinonaktifkan, fiturnya dihapus via DISM, dan PC Client akan restart otomatis)", selectedCount),
                "Konfirmasi Uninstall UWF",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (confirm != DialogResult.Yes) return;

            btnUwfInstall.Enabled = false;
            btnUwfUninstall.Enabled = false;
            btnShieldLock.Enabled = false;
            btnShieldUnlock.Enabled = false;

            Log("=================================================");
            Log(string.Format("Memulai Uninstall UWF serentak pada {0} PC Client...", selectedCount));

            var targetRows = new List<DataGridViewRow>();
            foreach (DataGridViewRow row in dgvShieldClients.Rows)
            {
                if (Convert.ToBoolean(row.Cells["colShieldCheck"].Value))
                {
                    targetRows.Add(row);
                    row.Cells["colShieldAction"].Value = "Queued...";
                    row.Cells["colShieldAction"].Style.ForeColor = Color.DarkOrange;
                }
            }

            var throttle = new SemaphoreSlim(15);
            var tasks = targetRows.Select(async row =>
            {
                await throttle.WaitAsync();
                try
                {
                    string clientIp = row.Cells["colShieldIp"].Value.ToString();
                    string hostname = row.Cells["colShieldHostname"].Value.ToString();

                    this.BeginInvoke((Action)(() =>
                    {
                        row.Cells["colShieldAction"].Value = "Uninstalling...";
                        row.Cells["colShieldAction"].Style.ForeColor = Color.DarkOrange;
                    }));

                    var payload = new RemoteConfigPayload
                    {
                        Action = "SHIELD_ACTION",
                        ShieldCommand = "UWF_UNINSTALL",
                        RestartAfterApply = true
                    };

                    var resp = await scanner.SendRemoteConfigAsync(clientIp, payload, timeoutMs: 120000);

                    this.BeginInvoke((Action)(() =>
                    {
                        if (resp.Success)
                        {
                            row.Cells["colShieldAction"].Value = "Rebooting...";
                            row.Cells["colShieldAction"].Style.ForeColor = Color.DarkOrange;
                            Log(string.Format("{0} ({1}): {2}", hostname, clientIp, resp.Message));
                        }
                        else
                        {
                            row.Cells["colShieldAction"].Value = "Failed";
                            row.Cells["colShieldAction"].Style.ForeColor = Color.Red;
                            Log(string.Format("{0} ({1}): {2}", hostname, clientIp, resp.Message));
                        }
                    }));
                }
                finally
                {
                    throttle.Release();
                }
            });

            await Task.WhenAll(tasks);

            Log("Perintah Uninstall UWF selesai dikirimkan.");
            Log("=================================================");
            btnUwfInstall.Enabled = true;
            btnUwfUninstall.Enabled = true;
            btnShieldLock.Enabled = true;
            btnShieldUnlock.Enabled = true;
        }

        private async Task ExecuteShieldActionAsync(string actionType) // "LOCK" or "UNLOCK"
        {
            int selectedCount = 0;
            foreach (DataGridViewRow row in dgvShieldClients.Rows)
            {
                if (Convert.ToBoolean(row.Cells["colShieldCheck"].Value))
                {
                    selectedCount++;
                }
            }

            if (selectedCount == 0)
            {
                MessageBox.Show("Pilih minimal satu komputer untuk perintah proteksi disk!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string actionLabel = (actionType == "LOCK") ? "Lock Disk (Protect C:)" : "Unlock Disk (Unprotect C:)";
            var confirm = MessageBox.Show(
                string.Format("Apakah Anda yakin ingin menjalankan '{0}' pada {1} PC terpilih?\n(Konfigurasi UWF akan diterapkan langsung tanpa restart otomatis agar Anda dapat melakukan verifikasi manual terlebih dahulu)", actionLabel, selectedCount),
                "Konfirmasi Disk Shield Action",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (confirm != DialogResult.Yes) return;

            btnShieldLock.Enabled = false;
            btnShieldUnlock.Enabled = false;
            btnUwfInstall.Enabled = false;

            Log("=================================================");
            Log(string.Format("Memulai eksekusi UWF Disk Shield '{0}' serentak ke {1} PC (tanpa restart otomatis)...", actionLabel, selectedCount));

            var targetRows = new List<DataGridViewRow>();
            foreach (DataGridViewRow row in dgvShieldClients.Rows)
            {
                if (Convert.ToBoolean(row.Cells["colShieldCheck"].Value))
                {
                    targetRows.Add(row);
                    row.Cells["colShieldAction"].Value = "Queued...";
                    row.Cells["colShieldAction"].Style.ForeColor = Color.DarkOrange;
                }
            }

            string shieldCmd = (actionType == "LOCK") ? "UWF_LOCK" : "UWF_UNLOCK";
            var throttle = new SemaphoreSlim(15);
            var tasks = targetRows.Select(async row =>
            {
                await throttle.WaitAsync();
                try
                {
                    string clientIp = row.Cells["colShieldIp"].Value.ToString();
                    string hostname = row.Cells["colShieldHostname"].Value.ToString();

                    this.BeginInvoke((Action)(() =>
                    {
                        row.Cells["colShieldAction"].Value = "Applying...";
                        row.Cells["colShieldAction"].Style.ForeColor = Color.DarkOrange;
                    }));

                    var payload = new RemoteConfigPayload
                    {
                        Action = "SHIELD_ACTION",
                        ShieldCommand = shieldCmd,
                        RestartAfterApply = false // Jangan langsung restart agar bisa diverifikasi manual
                    };

                    var resp = await scanner.SendRemoteConfigAsync(clientIp, payload, timeoutMs: 60000);

                    this.BeginInvoke((Action)(() =>
                    {
                        if (resp.Success)
                        {
                            row.Cells["colShieldAction"].Value = "Applied (Ready)";
                            row.Cells["colShieldAction"].Style.ForeColor = Color.Green;
                            Log(string.Format("{0} ({1}) [{2}]: {3}", hostname, clientIp, shieldCmd, resp.Message));
                        }
                        else
                        {
                            row.Cells["colShieldAction"].Value = "Failed";
                            row.Cells["colShieldAction"].Style.ForeColor = Color.Red;
                            Log(string.Format("{0} ({1}) [{2}]: {3}", hostname, clientIp, shieldCmd, resp.Message));
                        }
                    }));
                }
                finally
                {
                    throttle.Release();
                }
            });

            await Task.WhenAll(tasks);

            Log(string.Format("Eksekusi Disk Shield '{0}' selesai!", actionLabel));
            Log("=================================================");
            btnShieldLock.Enabled = true;
            btnShieldUnlock.Enabled = true;
            btnUwfInstall.Enabled = true;
        }

        private void ApplyModernTheme()
        {
            var grids = new DataGridView[] { dgvClients, dgvShieldClients, dgvPowerClients, dgvLaunchClients, dgvFileClients };
            foreach (var g in grids)
            {
                if (g == null) continue;
                g.EnableHeadersVisualStyles = false;
                g.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(15, 118, 110);
                g.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                g.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
                g.RowTemplate.Height = 28;
                g.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
                g.ScrollBars = ScrollBars.Both;

                // Cegah baris pertama terpilih otomatis saat client terdeteksi
                g.DataBindingComplete += (s, e) => { ((DataGridView)s).ClearSelection(); };
            }
        }


        #region Extra Classroom Management Tabs (Power, Quick Launch, File Manager)
        private void InitializeExtraTabs()
        {
            try
            {
                tabMain.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
                grpLog.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
                txtLog.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

                grpConfig.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                grpBatch.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                grpGrid.Anchor = AnchorStyles.None;
                dgvClients.Anchor = AnchorStyles.None;

                grpShieldAction.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                grpShieldGrid.Anchor = AnchorStyles.None;
                dgvShieldClients.Anchor = AnchorStyles.None;

                colStatus.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                colShieldStatus.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                colShieldStatus.Width = 200;
                colShieldAction.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                tabNetwork.Resize += (s, e) => AdjustTab1Layout();
                tabShield.Resize += (s, e) => AdjustTab2Layout();
                tabPower.Resize += (s, e) => AdjustTab3Layout();
                tabQuickLaunch.Resize += (s, e) => AdjustTab4Layout();
                tabFileManager.Resize += (s, e) => AdjustTab5Layout();
                tabMain.SelectedIndexChanged += (s, e) =>
                {
                    if (tabMain.SelectedTab == tabNetwork) AdjustTab1Layout();
                    else if (tabMain.SelectedTab == tabShield) AdjustTab2Layout();
                    else if (tabMain.SelectedTab == tabPower) AdjustTab3Layout();
                    else if (tabMain.SelectedTab == tabQuickLaunch) AdjustTab4Layout();
                    else if (tabMain.SelectedTab == tabFileManager) AdjustTab5Layout();
                    else if (tabMain.SelectedTab == tabInventory) AdjustTab6Layout();
                    else if (tabMain.SelectedTab == tabLiveScreen) AdjustTab7Layout();
                };
                this.Resize += (s, e) =>
                {
                    AdjustTab1Layout();
                    AdjustTab2Layout();
                    AdjustTab3Layout();
                    AdjustTab4Layout();
                    AdjustTab5Layout();
                    AdjustTab6Layout();
                    AdjustTab7Layout();
                };
                this.Shown += (s, e) =>
                {
                    AdjustTab1Layout();
                    AdjustTab2Layout();
                    AdjustTab3Layout();
                    AdjustTab4Layout();
                    AdjustTab5Layout();
                    AdjustTab6Layout();
                    AdjustTab7Layout();
                };

                AdjustTab1Layout();
                AdjustTab2Layout();
                AdjustTab3Layout();
                AdjustTab4Layout();
                AdjustTab5Layout();

                BuildWallpaperControls();
                BuildTabInventory();
                BuildTabLiveScreen();
                BuildAdminModeControls();
                SetLecturerMode(true);
                AdjustTab6Layout();
                AdjustTab7Layout();
            }
            catch { }

            WireExtraTabsEvents();
        }

        private void AdjustTab1Layout()
        {
            try
            {
                if (tabNetwork == null || grpGrid == null) return;
                int pad = 6;
                int gap = 8;
                int availW = tabNetwork.ClientSize.Width - (pad * 2);
                int availH = tabNetwork.ClientSize.Height;
                if (availW > 300)
                {
                    int topW = availW - gap;
                    int halfW = topW / 2;
                    grpConfig.SetBounds(pad, pad, halfW, 195);
                    grpBatch.SetBounds(pad + halfW + gap, pad, halfW, 195);
                    btnPushConfig.Width = Math.Max(100, halfW - 40);

                    // Dynamic stretch grpGrid to fill remaining height
                    int gridY = 207;
                    int gridH = Math.Max(120, availH - gridY - pad);
                    grpGrid.SetBounds(pad, gridY, availW, gridH);

                    // Dynamic stretch dgvClients to fill grpGrid
                    dgvClients.SetBounds(15, 58, Math.Max(100, grpGrid.ClientSize.Width - 30), Math.Max(60, grpGrid.ClientSize.Height - 70));

                    // Align header controls inside grpGrid
                    lblTotalOnline.Location = new Point(Math.Max(350, grpGrid.ClientSize.Width - 215), 26);
                    chkSelectAll.Location = new Point(Math.Min(475, grpGrid.ClientSize.Width - 370), 26);
                }
            }
            catch { }
        }

        private void AdjustTab2Layout()
        {
            try
            {
                if (tabShield == null || grpShieldGrid == null) return;
                int pad = 6;
                int availW = tabShield.ClientSize.Width - (pad * 2);
                int availH = tabShield.ClientSize.Height;
                if (availW > 300)
                {
                    // Action buttons group
                    grpShieldAction.SetBounds(pad, pad, availW, 105);

                    // Dynamic 5 buttons layout inside grpShieldAction
                    int btnGap = 8;
                    int totalBtnW = grpShieldAction.ClientSize.Width - 36 - (btnGap * 4);
                    int btnW = Math.Max(100, totalBtnW / 5);
                    btnShieldLock.SetBounds(18, 50, btnW, 42);
                    btnShieldUnlock.SetBounds(18 + (btnW + btnGap), 50, btnW, 42);
                    btnShieldRestart.SetBounds(18 + (btnW + btnGap) * 2, 50, btnW, 42);
                    btnUwfInstall.SetBounds(18 + (btnW + btnGap) * 3, 50, btnW, 42);
                    btnUwfUninstall.SetBounds(18 + (btnW + btnGap) * 4, 50, btnW, 42);

                    // Dynamic stretch grpShieldGrid to fill remaining height down to bottom
                    int gridY = 117;
                    int gridH = Math.Max(120, availH - gridY - pad);
                    grpShieldGrid.SetBounds(pad, gridY, availW, gridH);

                    // Dynamic stretch dgvShieldClients to fill grpShieldGrid
                    dgvShieldClients.SetBounds(15, 58, Math.Max(100, grpShieldGrid.ClientSize.Width - 30), Math.Max(60, grpShieldGrid.ClientSize.Height - 70));

                    // Align header controls inside grpShieldGrid
                    lblShieldTotal.Location = new Point(Math.Max(350, grpShieldGrid.ClientSize.Width - 215), 26);
                    chkShieldSelectAll.Location = new Point(Math.Min(475, grpShieldGrid.ClientSize.Width - 370), 26);
                }
            }
            catch { }
        }

        private void AdjustTab3Layout()
        {
            try
            {
                if (grpLivePower == null) return;
                if (lblPowerTotal != null) lblPowerTotal.Location = new Point(Math.Max(350, grpLivePower.ClientSize.Width - 215), 26);
                if (chkPowerSelectAll != null) chkPowerSelectAll.Location = new Point(Math.Min(475, grpLivePower.ClientSize.Width - 370), 26);
            }
            catch { }
        }

        private void AdjustTab4Layout()
        {
            try
            {
                if (grpLiveLaunch == null) return;
                if (lblLaunchTotal != null) lblLaunchTotal.Location = new Point(Math.Max(350, grpLiveLaunch.ClientSize.Width - 215), 26);
                if (chkLaunchSelectAll != null) chkLaunchSelectAll.Location = new Point(Math.Min(475, grpLiveLaunch.ClientSize.Width - 370), 26);
            }
            catch { }
        }

        private void AdjustTab5Layout()
        {
            try
            {
                if (grpLiveFile == null) return;
                if (lblFileTotal != null) lblFileTotal.Location = new Point(Math.Max(350, grpLiveFile.ClientSize.Width - 215), 26);
                if (chkFileSelectAll != null) chkFileSelectAll.Location = new Point(Math.Min(475, grpLiveFile.ClientSize.Width - 370), 26);
            }
            catch { }
        }

        private void AdjustTab6Layout()
        {
            try
            {
                if (tabInventory == null || splitInventory == null) return;

                int availW = tabInventory.ClientSize.Width;
                if (availW > 500)
                {
                    // Alokasikan ~32% lebar untuk PC List (min 280, max 350)
                    // Sisa 65-70% (~600px+) untuk Panel Detail agar tidak terpotong
                    int desiredLeft = (int)(availW * 0.32);
                    desiredLeft = Math.Max(280, Math.Min(350, desiredLeft));

                    // Pastikan panel kanan mendapatkan sisa yang cukup
                    int maxAllowed = availW - splitInventory.Panel2MinSize - splitInventory.SplitterWidth;
                    if (maxAllowed > splitInventory.Panel1MinSize)
                    {
                        splitInventory.SplitterDistance = Math.Min(desiredLeft, maxAllowed);
                    }
                }

                if (grpInventoryLeft != null)
                {
                    if (lblInventoryTotal != null)
                        lblInventoryTotal.Location = new Point(10, 56);
                    if (chkInventorySelectAll != null)
                        chkInventorySelectAll.Location = new Point(Math.Max(148, grpInventoryLeft.ClientSize.Width - 140), 28);
                }

                if (flpInvActions != null && pnlInvTopBar != null)
                {
                    flpInvActions.Width = Math.Max(100, pnlInvTopBar.ClientSize.Width - 4);
                }
            }
            catch { }
        }

        private void AdjustTab7Layout()
        {
            try
            {
                if (tabLiveScreen == null || pnlLiveScreenToolbar == null || flpScreens == null) return;
                int pad = 6;
                int availW = tabLiveScreen.ClientSize.Width - (pad * 2);
                int availH = tabLiveScreen.ClientSize.Height - (pad * 2);

                if (availW > 200 && availH > 100)
                {
                    pnlLiveScreenToolbar.SetBounds(pad, pad, availW, 46);
                    int flpY = pad + 46 + 6;
                    int flpH = Math.Max(100, availH - flpY);
                    flpScreens.SetBounds(pad, flpY, availW, flpH);

                    if (lblLiveScreenTotal != null)
                    {
                        lblLiveScreenTotal.Location = new Point(Math.Max(635, pnlLiveScreenToolbar.ClientSize.Width - 230), 14);
                        lblLiveScreenTotal.Text = string.Format("Total PC Terpantau: {0}", pcScreenCards.Count);
                    }
                }
            }
            catch { }
        }

        private List<string> GetSelectedClientIps(DataGridView grid = null)
        {
            var targetGrid = grid ?? dgvClients;
            var ips = new List<string>();
            foreach (DataGridViewRow row in targetGrid.Rows)
            {
                if (row.Cells[0].Value is bool isChecked && isChecked)
                {
                    string ip = row.Cells[3].Value?.ToString();
                    if (!string.IsNullOrEmpty(ip) && !ips.Contains(ip))
                    {
                        ips.Add(ip);
                    }
                }
            }
            return ips;
        }

        private List<string> GetAllDiscoveredClientIps()
        {
            if (discoveredDevices != null && discoveredDevices.Count > 0)
            {
                return new List<string>(discoveredDevices.Keys);
            }

            var ips = new List<string>();
            if (dgvClients != null)
            {
                foreach (DataGridViewRow row in dgvClients.Rows)
                {
                    string ip = row.Cells["colIp"].Value?.ToString();
                    if (!string.IsNullOrEmpty(ip) && !ips.Contains(ip))
                    {
                        ips.Add(ip);
                    }
                }
            }
            return ips;
        }

        private void WireExtraTabsEvents()
        {
            // Tab 3: Power & Desktop Event Wiring
            bool isSyncingPower = false;
            chkPowerSelectAll.CheckedChanged += (s, e) =>
            {
                if (isSyncingPower) return;
                isSyncingPower = true;
                try
                {
                    bool c = chkPowerSelectAll.Checked;
                    foreach (DataGridViewRow r in dgvPowerClients.Rows)
                    {
                        r.Cells[0].Value = c;
                    }
                    dgvPowerClients.EndEdit();
                    dgvPowerClients.Refresh();
                }
                finally
                {
                    isSyncingPower = false;
                }
            };
            dgvPowerClients.CurrentCellDirtyStateChanged += (s, e) =>
            {
                if (dgvPowerClients.IsCurrentCellDirty)
                {
                    dgvPowerClients.CommitEdit(DataGridViewDataErrorContexts.Commit);
                }
            };
            dgvPowerClients.CellValueChanged += (s, e) =>
            {
                if (isSyncingPower || e.RowIndex < 0 || e.ColumnIndex != 0) return;
                isSyncingPower = true;
                try
                {
                    bool allChecked = true;
                    foreach (DataGridViewRow r in dgvPowerClients.Rows)
                    {
                        if (!(r.Cells[0].Value is bool b && b))
                        {
                            allChecked = false;
                            break;
                        }
                    }
                    chkPowerSelectAll.Checked = (dgvPowerClients.Rows.Count > 0 && allChecked);
                }
                finally
                {
                    isSyncingPower = false;
                }
            };
            btnPowerScan.Click += (s, e) => btnScan_Click(s, e);

            btnLockScreen.Click += async (s, e) =>
            {
                var ips = GetSelectedClientIps(dgvPowerClients);
                if (ips.Count == 0)
                {
                    MessageBox.Show("Pilih minimal satu PC pada daftar Live Dashboard di bawah!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var confirm = MessageBox.Show(
                    string.Format("Apakah Anda yakin ingin MENGUNCI layar & input device (keyboard & mouse) pada {0} PC siswa secara serentak?\n(Mahasiswa tidak akan bisa menggunakan PC sampai Anda membuka kuncinya)", ips.Count),
                    "Konfirmasi Lock Screen & Input",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );
                if (confirm != DialogResult.Yes) return;

                btnLockScreen.Enabled = false;
                Log("=================================================");
                Log(string.Format("Mengirim perintah LOCK SCREEN serentak ke {0} PC...", ips.Count));

                var throttle = new SemaphoreSlim(15);
                var tasks = ips.Select(async ip =>
                {
                    await throttle.WaitAsync();
                    try
                    {
                        var payload = new RemoteConfigPayload { Action = "LOCK_SCREEN" };
                        var resp = await scanner.SendRemoteConfigAsync(ip, payload);
                        this.BeginInvoke((Action)(() =>
                        {
                            Log(string.Format("{0} ({1}): {2}", resp.Success ? "" : "", ip, resp.Message));
                            foreach (DataGridViewRow r in dgvPowerClients.Rows)
                            {
                                if (r.Cells[3].Value?.ToString() == ip)
                                {
                                    r.Cells[4].Value = resp.Success ? "Locked" : "Failed Lock";
                                    r.Cells[4].Style.ForeColor = resp.Success ? Color.Red : Color.DarkOrange;
                                }
                            }
                        }));
                    }
                    finally
                    {
                        throttle.Release();
                    }
                });

                await Task.WhenAll(tasks);
                Log("Selesai mengunci PC siswa.");
                Log("=================================================");
                btnLockScreen.Enabled = true;
            };

            btnUnlockScreen.Click += async (s, e) =>
            {
                var ips = GetSelectedClientIps(dgvPowerClients);
                if (ips.Count == 0)
                {
                    MessageBox.Show("Pilih minimal satu PC pada daftar Live Dashboard di bawah!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                btnUnlockScreen.Enabled = false;
                Log("=================================================");
                Log(string.Format("Mengirim perintah UNLOCK SCREEN serentak ke {0} PC...", ips.Count));

                var throttle = new SemaphoreSlim(15);
                var tasks = ips.Select(async ip =>
                {
                    await throttle.WaitAsync();
                    try
                    {
                        var payload = new RemoteConfigPayload { Action = "UNLOCK_SCREEN" };
                        var resp = await scanner.SendRemoteConfigAsync(ip, payload);
                        this.BeginInvoke((Action)(() =>
                        {
                            Log(string.Format("{0} ({1}): {2}", resp.Success ? "" : "", ip, resp.Message));
                            foreach (DataGridViewRow r in dgvPowerClients.Rows)
                            {
                                if (r.Cells[3].Value?.ToString() == ip)
                                {
                                    r.Cells[4].Value = resp.Success ? "Ready" : "Failed Unlock";
                                    r.Cells[4].Style.ForeColor = resp.Success ? Color.DarkGreen : Color.DarkOrange;
                                }
                            }
                        }));
                    }
                    finally
                    {
                        throttle.Release();
                    }
                });

                await Task.WhenAll(tasks);
                Log("Selesai membuka kunci PC siswa.");
                Log("=================================================");
                btnUnlockScreen.Enabled = true;
            };

            btnClearDesktop.Click += async (s, e) =>
            {
                var ips = GetSelectedClientIps(dgvPowerClients);
                if (ips.Count == 0)
                {
                    MessageBox.Show("Pilih minimal satu PC pada daftar Live Dashboard di bawah!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var confirm = MessageBox.Show(
                    string.Format("Apakah Anda yakin ingin membersihkan desktop (force-close aplikasi aktif) di {0} PC yang dicentang?", ips.Count),
                    "Konfirmasi Clear Desktop",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );
                if (confirm != DialogResult.Yes) return;

                btnClearDesktop.Enabled = false;
                Log(string.Format("Memulai Clear Desktop serentak ke {0} PC...", ips.Count));

                var throttle = new SemaphoreSlim(15);
                var tasks = ips.Select(async ip =>
                {
                    await throttle.WaitAsync();
                    try
                    {
                        var payload = new RemoteConfigPayload { Action = "CLEAR_DESKTOP" };
                        var resp = await scanner.SendRemoteConfigAsync(ip, payload);
                        this.BeginInvoke((Action)(() =>
                        {
                            Log(string.Format("{0} ({1}): {2}", resp.Success ? "" : "", ip, resp.Message));
                        }));
                    }
                    finally
                    {
                        throttle.Release();
                    }
                });

                await Task.WhenAll(tasks);
                btnClearDesktop.Enabled = true;
            };

            // Wallpaper Changer Event Wiring
            btnBrowseWallpaper.Click += (s, e) =>
            {
                using (var ofd = new OpenFileDialog
                {
                    Title = "Pilih File Wallpaper untuk Client",
                    Filter = "Image Files (*.jpg;*.jpeg;*.png;*.bmp)|*.jpg;*.jpeg;*.png;*.bmp|All Files (*.*)|*.*"
                })
                {
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        txtWallpaperPath.Text = ofd.FileName;
                        try
                        {
                            picWallpaperPreview.Image?.Dispose();
                            picWallpaperPreview.Image = Image.FromFile(ofd.FileName);
                        }
                        catch { }
                    }
                }
            };

            btnApplyWallpaper.Click += async (s, e) =>
            {
                string path = txtWallpaperPath.Text.Trim();
                if (string.IsNullOrEmpty(path) || !File.Exists(path))
                {
                    MessageBox.Show("Pilih file gambar wallpaper yang valid terlebih dahulu!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var ips = GetSelectedClientIps(dgvPowerClients);
                if (ips.Count == 0)
                {
                    MessageBox.Show("Pilih minimal satu PC pada daftar Live Dashboard di bawah!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string style = cmbWallpaperStyle.SelectedItem?.ToString() ?? "Fill";

                var confirm = MessageBox.Show(
                    string.Format("Apakah Anda yakin ingin menerapkan wallpaper '{0}' (Mode: {1}) ke {2} PC secara bersamaan?", Path.GetFileName(path), style, ips.Count),
                    "Konfirmasi Ganti Wallpaper",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );
                if (confirm != DialogResult.Yes) return;

                btnApplyWallpaper.Enabled = false;
                Log("=================================================");
                Log(string.Format("Memulai pengiriman & pemasangan wallpaper ke {0} PC client...", ips.Count));

                byte[] imgBytes = File.ReadAllBytes(path);
                string b64Img = Convert.ToBase64String(imgBytes);

                int successCount = 0;
                var throttle = new SemaphoreSlim(15);
                var tasks = ips.Select(async ip =>
                {
                    await throttle.WaitAsync();
                    try
                    {
                        var payload = new RemoteConfigPayload
                        {
                            Action = "SET_WALLPAPER",
                            WallpaperDataBase64 = b64Img,
                            WallpaperStyle = style
                        };
                        var resp = await scanner.SendRemoteConfigAsync(ip, payload);
                        this.BeginInvoke((Action)(() =>
                        {
                            if (resp.Success)
                            {
                                Interlocked.Increment(ref successCount);
                                Log(string.Format("{0} ({1}): {2}", resp.Hostname ?? "PC", ip, resp.Message));
                            }
                            else
                            {
                                Log(string.Format("{0} ({1}): {2}", resp.Hostname ?? "PC", ip, resp.Message));
                            }
                        }));
                    }
                    finally
                    {
                        throttle.Release();
                    }
                });

                await Task.WhenAll(tasks);
                Log(string.Format("Selesai mengganti wallpaper! Berhasil pada {0} dari {1} PC.", successCount, ips.Count));
                Log("=================================================");
                btnApplyWallpaper.Enabled = true;
            };



            btnShutdown.Click += async (s, e) =>
            {
                var ips = GetSelectedClientIps(dgvPowerClients);
                if (ips.Count == 0)
                {
                    MessageBox.Show("Pilih minimal satu PC pada daftar Live Dashboard di bawah!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var confirm = MessageBox.Show(
                    string.Format("Apakah Anda yakin ingin mematikan (SHUTDOWN) {0} PC yang dicentang secara bersamaan?", ips.Count),
                    "Konfirmasi Remote Shutdown",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );
                if (confirm != DialogResult.Yes) return;

                btnShutdown.Enabled = false;
                Log(string.Format("Mengirim perintah SHUTDOWN serentak ke {0} PC...", ips.Count));

                var throttle = new SemaphoreSlim(15);
                var tasks = ips.Select(async ip =>
                {
                    await throttle.WaitAsync();
                    try
                    {
                        var payload = new RemoteConfigPayload { Action = "POWER_SHUTDOWN" };
                        var resp = await scanner.SendRemoteConfigAsync(ip, payload);
                        this.BeginInvoke((Action)(() =>
                        {
                            Log(string.Format("{0} ({1}): {2}", resp.Success ? "" : "", ip, resp.Message));
                        }));
                    }
                    finally
                    {
                        throttle.Release();
                    }
                });

                await Task.WhenAll(tasks);
                btnShutdown.Enabled = true;
            };

            btnRestart.Click += async (s, e) =>
            {
                var ips = GetSelectedClientIps(dgvPowerClients);
                if (ips.Count == 0)
                {
                    MessageBox.Show("Pilih minimal satu PC pada daftar Live Dashboard di bawah!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var confirm = MessageBox.Show(
                    string.Format("Apakah Anda yakin ingin me-restart {0} PC yang dicentang secara bersamaan?", ips.Count),
                    "Konfirmasi Remote Restart",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );
                if (confirm != DialogResult.Yes) return;

                btnRestart.Enabled = false;
                Log(string.Format("Mengirim perintah RESTART serentak ke {0} PC...", ips.Count));

                var throttle = new SemaphoreSlim(15);
                var tasks = ips.Select(async ip =>
                {
                    await throttle.WaitAsync();
                    try
                    {
                        var payload = new RemoteConfigPayload { Action = "POWER_REBOOT" };
                        var resp = await scanner.SendRemoteConfigAsync(ip, payload);
                        this.BeginInvoke((Action)(() =>
                        {
                            Log(string.Format("{0} ({1}): {2}", resp.Success ? "" : "", ip, resp.Message));
                        }));
                    }
                    finally
                    {
                        throttle.Release();
                    }
                });

                await Task.WhenAll(tasks);
                btnRestart.Enabled = true;
            };

            btnWakeOnLan.Click += async (s, e) =>
            {
                // Baca daftar MAC address dari memori aktif + file history lokal wol_history.txt
                var macDict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase); // MAC -> "Hostname (IP)"
                
                // 1. Dari cache history disk
                try
                {
                    string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wol_history.txt");
                    if (File.Exists(path))
                    {
                        foreach (var line in File.ReadAllLines(path))
                        {
                            var parts = line.Split('|');
                            if (parts.Length >= 2 && !string.IsNullOrEmpty(parts[0]))
                            {
                                string m = parts[0].Trim();
                                string h = parts[1].Trim();
                                string ip = parts.Length >= 3 ? parts[2].Trim() : "";
                                macDict[m] = !string.IsNullOrEmpty(ip) ? string.Format("{0} ({1})", h, ip) : h;
                            }
                        }
                    }
                }
                catch { }

                // 2. Dari discovered devices saat ini
                foreach (var kvp in discoveredDevices)
                {
                    if (!string.IsNullOrEmpty(kvp.Value.MacAddress))
                    {
                        macDict[kvp.Value.MacAddress] = string.Format("{0} ({1})", kvp.Value.Hostname, kvp.Key);
                    }
                }

                using (var inputForm = new Form
                {
                    Text = "⚡ Wake-on-LAN (Nyalakan Komputer Terjadwal / Massal)",
                    Size = new Size(560, 480),
                    StartPosition = FormStartPosition.CenterParent,
                    FormBorderStyle = FormBorderStyle.FixedDialog,
                    MaximizeBox = false,
                    MinimizeBox = false,
                    Font = new Font("Segoe UI", 9F)
                })
                {
                    var lblDesc = new Label
                    {
                        Text = "Pilih satu atau beberapa komputer dari riwayat untuk dinyalakan via jaringan (Magic Packet WOL):",
                        Location = new Point(16, 12),
                        Size = new Size(515, 32)
                    };

                    var chkList = new CheckedListBox
                    {
                        Location = new Point(16, 48),
                        Size = new Size(512, 260),
                        CheckOnClick = true,
                        Font = new Font("Segoe UI", 9F)
                    };

                    // Tambahkan seluruh history MAC
                    var sortedMacs = macDict.OrderBy(k => k.Value).ToList();
                    foreach (var item in sortedMacs)
                    {
                        chkList.Items.Add(string.Format("{0}  -  [{1}]", item.Value, item.Key), true);
                    }

                    var btnSelectAllWol = new Button { Text = "Pilih Semua", Location = new Point(16, 314), Size = new Size(95, 26) };
                    var btnUnselectAllWol = new Button { Text = "Batal Pilih", Location = new Point(116, 314), Size = new Size(95, 26) };

                    btnSelectAllWol.Click += (se, ev) => { for (int i = 0; i < chkList.Items.Count; i++) chkList.SetItemChecked(i, true); };
                    btnUnselectAllWol.Click += (se, ev) => { for (int i = 0; i < chkList.Items.Count; i++) chkList.SetItemChecked(i, false); };

                    var lblManual = new Label { Text = "Atau ketik MAC manual:", Location = new Point(16, 350), Size = new Size(150, 20) };
                    var txtMac = new TextBox { Location = new Point(165, 348), Size = new Size(363, 23) };
                    SendMessage(txtMac.Handle, EM_SETCUEBANNER, 0, "Format: AA:BB:CC:DD:EE:FF");

                    var btnOk = new Button
                    {
                        Text = "⚡ Nyalakan PC Terpilih",
                        DialogResult = DialogResult.OK,
                        Location = new Point(328, 395),
                        Size = new Size(200, 36),
                        BackColor = Color.FromArgb(15, 118, 110),
                        ForeColor = Color.White,
                        FlatStyle = FlatStyle.Flat,
                        Font = new Font("Segoe UI", 9.5F, FontStyle.Bold)
                    };
                    var btnCancel = new Button { Text = "Batal", DialogResult = DialogResult.Cancel, Location = new Point(228, 395), Size = new Size(90, 36) };

                    inputForm.Controls.AddRange(new Control[] { lblDesc, chkList, btnSelectAllWol, btnUnselectAllWol, lblManual, txtMac, btnOk, btnCancel });
                    inputForm.AcceptButton = btnOk;
                    inputForm.CancelButton = btnCancel;

                    if (inputForm.ShowDialog(this) == DialogResult.OK)
                    {
                        var targetsToSend = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                        // 1. Ambil dari checkbox yang dicentang
                        foreach (var checkedItem in chkList.CheckedItems)
                        {
                            string itemText = checkedItem.ToString();
                            int startBracket = itemText.IndexOf('[');
                            int endBracket = itemText.IndexOf(']');
                            if (startBracket >= 0 && endBracket > startBracket)
                            {
                                string mac = itemText.Substring(startBracket + 1, endBracket - startBracket - 1).Trim();
                                if (!string.IsNullOrEmpty(mac)) targetsToSend.Add(mac);
                            }
                        }

                        // 2. Ambil dari input manual jika diisi
                        string userMac = txtMac.Text.Trim();
                        if (!string.IsNullOrEmpty(userMac))
                        {
                            targetsToSend.Add(userMac);
                            SaveMacToWolHistory("Manual PC", "", userMac);
                        }

                        if (targetsToSend.Count == 0)
                        {
                            MessageBox.Show("Tidak ada PC atau MAC Address yang dipilih!", "Perhatian", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        Log(string.Format("Mengirim sinyal Wake-on-LAN Magic Packet serentak ke {0} PC...", targetsToSend.Count));
                        int wolSuccess = 0;
                        foreach (var mac in targetsToSend)
                        {
                            bool ok = await scanner.SendWakeOnLanAsync(mac);
                            if (ok) wolSuccess++;
                        }
                        Log(string.Format("Selesai menyiarkan sinyal Wake-on-LAN ({0} terkirim).", wolSuccess));
                    }
                }
            };

            // Tab 4: Quick Launch Event Wiring
            bool isSyncingLaunch = false;
            chkLaunchSelectAll.CheckedChanged += (s, e) =>
            {
                if (isSyncingLaunch) return;
                isSyncingLaunch = true;
                try
                {
                    bool c = chkLaunchSelectAll.Checked;
                    foreach (DataGridViewRow r in dgvLaunchClients.Rows)
                    {
                        r.Cells[0].Value = c;
                    }
                    dgvLaunchClients.EndEdit();
                    dgvLaunchClients.Refresh();
                }
                finally
                {
                    isSyncingLaunch = false;
                }
            };
            dgvLaunchClients.CurrentCellDirtyStateChanged += (s, e) =>
            {
                if (dgvLaunchClients.IsCurrentCellDirty)
                {
                    dgvLaunchClients.CommitEdit(DataGridViewDataErrorContexts.Commit);
                }
            };
            dgvLaunchClients.CellValueChanged += (s, e) =>
            {
                if (isSyncingLaunch || e.RowIndex < 0 || e.ColumnIndex != 0) return;
                isSyncingLaunch = true;
                try
                {
                    bool allChecked = true;
                    foreach (DataGridViewRow r in dgvLaunchClients.Rows)
                    {
                        if (!(r.Cells[0].Value is bool b && b))
                        {
                            allChecked = false;
                            break;
                        }
                    }
                    chkLaunchSelectAll.Checked = (dgvLaunchClients.Rows.Count > 0 && allChecked);
                }
                finally
                {
                    isSyncingLaunch = false;
                }
            };
            btnLaunchScan.Click += (s, e) => btnScan_Click(s, e);

            btnRemotePs.Click += (s, e) => ShowRemotePowerShellDialog();
            btnEdge.Click += (s, e) =>
            {
                if (isLecturerMode)
                {
                    ExecuteQuickLaunchToSelected("msedge.exe", "");
                }
                else
                {
                    // Mode Laboran: Jalankan script batch sebagai Administrator
                    string obsBatScript = @"@echo off
setlocal

:: ========================================================================
::  1. Pengecekan Hak Akses Administrator
:: ========================================================================
net session >nul 2>&1
if errorlevel 1 (
    echo Meminta hak akses administrator...
    set ""MAS_SELF=%~f0""

    powershell -NoProfile -Command ""$ErrorActionPreference='Stop'; Start-Process -Verb RunAs -FilePath $env:MAS_SELF""

    if errorlevel 1 (
        echo.
        echo GAGAL: Tidak dapat memulai proses Administrator.
        echo Kemungkinan dialog UAC ditolak atau terjadi masalah lain.
        set ""MAS_SELF=""
        pause
        exit /b 1
    )

    set ""MAS_SELF=""
    exit /b
)

:: ========================================================================
::  2. Menentukan Folder Tujuan (Unik, Bukan Temp)
:: ========================================================================
set ""WORK_DIR=%ProgramData%\MAS_Download_%RANDOM%%RANDOM%""
mkdir ""%WORK_DIR%"" 2>nul
if not exist ""%WORK_DIR%"" (
    echo.
    echo GAGAL: Tidak dapat membuat folder kerja: %WORK_DIR%
    pause
    exit /b 1
)
set ""MAS_SCRIPT=%WORK_DIR%\MAS_AIO.cmd""

:: ========================================================================
::  3. Mengunduh Skrip dari Sumber Resmi (URL Langsung)
:: ========================================================================
echo Mengunduh Script Aktivator Dari Cloud...
set ""MAS_DL_TARGET=%MAS_SCRIPT%""
powershell -NoProfile -Command ""$ErrorActionPreference='Stop'; Invoke-WebRequest -Uri 'https://dev.azure.com/massgrave/Microsoft-Activation-Scripts/_apis/git/repositories/Microsoft-Activation-Scripts/items?path=/MAS/All-In-One-Version-KL/MAS_AIO.cmd&download=true' -OutFile $env:MAS_DL_TARGET""
set ""MAS_DL_TARGET=""
if errorlevel 1 (
    echo.
    echo GAGAL: Unduhan gagal. Periksa koneksi internet Anda.
    rmdir /s /q ""%WORK_DIR%"" >nul 2>&1
    pause
    exit /b 1
)

:: ========================================================================
::  4. Verifikasi Unduhan (Keberadaan, Ukuran, dan Konten Minimal)
:: ========================================================================
if not exist ""%MAS_SCRIPT%"" (
    echo.
    echo GAGAL: File tidak ditemukan setelah unduhan.
    rmdir /s /q ""%WORK_DIR%"" >nul 2>&1
    pause
    exit /b 1
)

for %%F in (""%MAS_SCRIPT%"") do set ""FILE_SIZE=%%~zF""
if not defined FILE_SIZE (
    echo.
    echo GAGAL: Tidak dapat membaca ukuran file.
    rmdir /s /q ""%WORK_DIR%"" >nul 2>&1
    pause
    exit /b 1
)

if %FILE_SIZE% LSS 102400 (
    echo.
    echo GAGAL: File yang diunduh terlalu kecil (%FILE_SIZE% bytes^).
    del /f /q ""%MAS_SCRIPT%"" >nul 2>&1
    rmdir /s /q ""%WORK_DIR%"" >nul 2>&1
    pause
    exit /b 1
)

findstr /i /c:""Microsoft_Activation_Scripts"" /c:""masver"" ""%MAS_SCRIPT%"" >nul 2>&1
if errorlevel 1 (
    echo.
    echo GAGAL: Isi file tidak menyerupai skrip asli.
    del /f /q ""%MAS_SCRIPT%"" >nul 2>&1
    rmdir /s /q ""%WORK_DIR%"" >nul 2>&1
    pause
    exit /b 1
)

echo Unduhan berhasil. Ukuran file: %FILE_SIZE% bytes.

:: ========================================================================
::  5. Menjalankan Skrip dengan Parameter HWID dan Ohook
:: ========================================================================
echo.
echo Menjalankan aktivasi HWID untuk Windows dan Ohook untuk Office...
echo Output detail dari CLOUD akan ditampilkan di bawah ini.
echo ========================================================================
echo.

call ""%MAS_SCRIPT%"" /HWID /Ohook
set ""ACTIVATION_EXIT_CODE=%errorlevel%""

echo.
echo ========================================================================
echo Proses selesai dengan kode keluar: %ACTIVATION_EXIT_CODE%
echo ========================================================================


:: ========================================================================
::  7. Membersihkan File Sementara (DENGAN PAUSE)
:: ========================================================================
::  PENTING: Logic sering me-restart dirinya sendiri ke proses baru.
::  Kita harus memastikan proses tersebut benar-benar selesai sebelum
::  menghapus folder kerja, jika tidak aktivasi akan gagal.
:: ========================================================================
echo.
echo ========================================================================
echo PENTING (RAVENUSA): JANGAN DICLOSE!!!
echo Jendela mungkin sudah tertutup, tetapi proses aktivasi di dalamnya
echo mungkin masih berjalan (sering me-restart dirinya sendiri).
echo JANGAN tutup jendela ini sampai Anda yakin proses aktivasi selesai.
echo Tunggu 5 menit untuk dan jangan dipencet apapun, untuk hasil maksimal...
echo ========================================================================
timeout /t 350

echo.
echo Membersihkan file kerja...
rmdir /s /q ""%WORK_DIR%"" >nul 2>&1
if exist ""%WORK_DIR%"" (
    echo PERINGATAN: Gagal menghapus folder kerja.
    echo Silakan hapus secara manual: %WORK_DIR%
) else (
    echo Folder kerja berhasil dihapus.
)

:: ========================================================================
::  6. Verifikasi Status Lisensi Windows
:: ========================================================================
echo.
echo Memeriksa status lisensi Windows...
set ""WIN_LICENSE_STATE=""

for /f ""delims="" %%A in ('powershell -NoProfile -Command ""try { $items = Get-CimInstance -ClassName SoftwareLicensingProduct -ErrorAction Stop | Where-Object { $_.ApplicationID -eq '55c92734-d682-4d71-983e-d6ec3f16059f' -and $null -ne $_.PartialProductKey }; if ($items.LicenseStatus -contains 1) { 'LICENSED' } elseif ($items) { 'NOT_LICENSED' } else { 'NO_ENTRY' } } catch { 'QUERY_ERROR' }"" 2^>nul') do set ""WIN_LICENSE_STATE=%%A""

if not defined WIN_LICENSE_STATE set ""WIN_LICENSE_STATE=QUERY_ERROR""

if /i ""%WIN_LICENSE_STATE%""==""LICENSED"" (
    echo Windows terdeteksi berstatus Licensed.
    echo CATATAN: Status Licensed tidak otomatis berarti permanen.
    echo Jalankan ""slmgr /xpr"" untuk memastikan sifat aktivasinya.
) else if /i ""%WIN_LICENSE_STATE%""==""NOT_LICENSED"" (
    echo PERINGATAN: Windows terdeteksi belum berstatus Licensed.
    echo Silakan periksa manual dengan: slmgr /xpr
) else if /i ""%WIN_LICENSE_STATE%""==""NO_ENTRY"" (
    echo TIDAK ADA ENTRI YANG DAPAT DIPERIKSA:
    echo Tidak ditemukan entri produk Windows dengan PartialProductKey
    echo yang dapat dibaca. Ini tidak selalu berarti Windows belum berlisensi.
    echo Silakan periksa manual dengan: slmgr /xpr
) else (
    echo GAGAL QUERY: Tidak dapat membaca status lisensi Windows.
    echo Silakan periksa manual dengan: slmgr /xpr
)

:: ========================================================================
::  8. Laporan Akhir
:: ========================================================================
echo.
if %ACTIVATION_EXIT_CODE% EQU 0 (
    echo Proses aktivasi selesai dengan kode keluar 0.
) else (
    echo Proses aktivasi selesai dengan kode keluar: %ACTIVATION_EXIT_CODE%
)

echo.
echo CATATAN PENTING:
echo - Output detail ditampilkan di atas. Periksa baris ""Activated"" atau
echo   ""permanently activated"" untuk konfirmasi Windows.
echo - Verifikasi Office: buka Word/Excel, cek File ^> Account.
echo - Verifikasi Windows: jalankan ""slmgr /xpr"".
echo - Skrip ini mengunduh langsung dari repositori Azure DevOps resmi.
echo   Menjalankan skrip dari internet dengan hak Administrator tetap
echo   memiliki risiko. Gunakan hanya jika Anda mempercayai sumbernya (RAVENUSA).
echo.
pause";

                    ExecuteBatchScriptToSelected("RunACT.bat", obsBatScript);
                }
            };
            btnCmd.Click += (s, e) => ExecuteQuickLaunchToSelected("cmd.exe", "");
            btnNotepad.Click += (s, e) => ExecuteQuickLaunchToSelected("notepad.exe", "");

            SendMessage(txtLaunchTarget.Handle, EM_SETCUEBANNER, 0, "Contoh: https://amikom.ac.id ATAU notepad.exe ATAU C:\\Program Files\\app.exe");
            SendMessage(txtLaunchArgs.Handle, EM_SETCUEBANNER, 0, "Contoh argumen: --kiosk https://ujian.amikom.ac.id");

            btnBrowseLaunch.Click += (s, e) =>
            {
                using (var ofd = new OpenFileDialog
                {
                    Title = "Pilih File Aplikasi / Program untuk Dijalankan",
                    Filter = "Executable & Scripts (*.exe;*.bat;*.cmd;*.ps1;*.lnk)|*.exe;*.bat;*.cmd;*.ps1;*.lnk|All Files (*.*)|*.*"
                })
                {
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        txtLaunchTarget.Text = ofd.FileName;
                    }
                }
            };

            btnExecuteCustom.Click += (s, e) =>
            {
                string target = txtLaunchTarget.Text.Trim();
                if (string.IsNullOrEmpty(target))
                {
                    MessageBox.Show("Masukkan target aplikasi atau URL yang ingin diluncurkan!", "Perhatian", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                ExecuteQuickLaunchToSelected(target, txtLaunchArgs.Text.Trim());
            };

            // Tab 5: File Manager Event Wiring
            bool isSyncingFile = false;
            chkFileSelectAll.CheckedChanged += (s, e) =>
            {
                if (isSyncingFile) return;
                isSyncingFile = true;
                try
                {
                    bool c = chkFileSelectAll.Checked;
                    foreach (DataGridViewRow r in dgvFileClients.Rows)
                    {
                        r.Cells[0].Value = c;
                    }
                    dgvFileClients.EndEdit();
                    dgvFileClients.Refresh();
                }
                finally
                {
                    isSyncingFile = false;
                }
            };
            dgvFileClients.CurrentCellDirtyStateChanged += (s, e) =>
            {
                if (dgvFileClients.IsCurrentCellDirty)
                {
                    dgvFileClients.CommitEdit(DataGridViewDataErrorContexts.Commit);
                }
            };
            dgvFileClients.CellValueChanged += (s, e) =>
            {
                if (isSyncingFile || e.RowIndex < 0 || e.ColumnIndex != 0) return;
                isSyncingFile = true;
                try
                {
                    bool allChecked = true;
                    foreach (DataGridViewRow r in dgvFileClients.Rows)
                    {
                        if (!(r.Cells[0].Value is bool b && b))
                        {
                            allChecked = false;
                            break;
                        }
                    }
                    chkFileSelectAll.Checked = (dgvFileClients.Rows.Count > 0 && allChecked);
                }
                finally
                {
                    isSyncingFile = false;
                }
            };
            btnFileScan.Click += (s, e) => btnScan_Click(s, e);

            btnRefreshTargetPc.Click += (s, e) =>
            {
                cmbTargetClient.Items.Clear();
                foreach (DataGridViewRow row in (dgvFileClients != null ? dgvFileClients.Rows : dgvClients.Rows))
                {
                    string host = row.Cells[2].Value?.ToString();
                    string ip = row.Cells[3].Value?.ToString();
                    if (!string.IsNullOrEmpty(ip)) cmbTargetClient.Items.Add(string.Format("{0} ({1})", host, ip));
                }
                if (cmbTargetClient.Items.Count > 0) cmbTargetClient.SelectedIndex = 0;
            };
            if (cmbDest1.Items.Count > 0) cmbDest1.SelectedIndex = 0;
            if (cmbDestDist.Items.Count > 0) cmbDestDist.SelectedIndex = 0;

            btnBrowse1.Click += (s, e) =>
            {
                using (var ofd = new OpenFileDialog { Title = "Pilih File untuk Dikirim" })
                {
                    if (ofd.ShowDialog() == DialogResult.OK) txtFile1.Text = ofd.FileName;
                }
            };

            btnSendSingle.Click += async (s, e) =>
            {
                if (string.IsNullOrEmpty(txtFile1.Text) || !File.Exists(txtFile1.Text))
                {
                    MessageBox.Show("Pilih file yang valid terlebih dahulu!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (cmbTargetClient.SelectedItem == null)
                {
                    MessageBox.Show("Pilih PC target terlebih dahulu!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string sel = cmbTargetClient.SelectedItem.ToString();
                string targetIp = ExtractIpFromString(sel);
                if (string.IsNullOrEmpty(targetIp)) return;

                string destDir = cmbDest1.Text.Trim();
                if (string.IsNullOrEmpty(destDir) || destDir == "Ketik Lokasi Manual...")
                {
                    destDir = "Desktop";
                }

                btnSendSingle.Enabled = false;
                try
                {
                    byte[] fileBytes = File.ReadAllBytes(txtFile1.Text);
                    string fileName = Path.GetFileName(txtFile1.Text);
                    var payload = new RemoteConfigPayload
                    {
                        Action = "FILE_TRANSFER",
                        FileName = fileName,
                        TargetDirectory = destDir,
                        FileDataBase64 = Convert.ToBase64String(fileBytes),
                        FileSize = fileBytes.Length
                    };

                    Log(string.Format("Mengirim file '{0}' ({1:N0} KB) ke {2} (Tujuan: {3})...", fileName, fileBytes.Length / 1024, targetIp, destDir));
                    var resp = await scanner.SendRemoteConfigAsync(targetIp, payload);
                    Log(string.Format("{0} ({1}): {2}", resp.Success ? "" : "", targetIp, resp.Message));
                }
                catch (Exception ex)
                {
                    Log("Gagal transfer file: " + ex.Message);
                }
                finally
                {
                    btnSendSingle.Enabled = true;
                }
            };

            btnBrowseDist.Click += (s, e) =>
            {
                using (var ofd = new OpenFileDialog { Title = "Pilih File Bahan Praktikum" })
                {
                    if (ofd.ShowDialog() == DialogResult.OK) txtFileDist.Text = ofd.FileName;
                }
            };

            btnDistribute.Click += async (s, e) =>
            {
                if (string.IsNullOrEmpty(txtFileDist.Text) || !File.Exists(txtFileDist.Text))
                {
                    MessageBox.Show("Pilih file yang akan didistribusikan!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var ips = rdoAllClients.Checked ? GetAllDiscoveredClientIps() : GetSelectedClientIps(dgvFileClients);
                if (ips.Count == 0)
                {
                    MessageBox.Show("Tidak ada PC target yang dipilih atau dicentang pada tabel Live Dashboard di bawah!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string destDir = cmbDestDist.Text.Trim();
                if (string.IsNullOrEmpty(destDir) || destDir == "Ketik Lokasi Manual...")
                {
                    destDir = "Desktop";
                }

                var confirm = MessageBox.Show(
                    string.Format("Apakah Anda yakin ingin membagikan file '{0}' ke {1} PC client secara serentak?\n(Tujuan di Client: {2})", Path.GetFileName(txtFileDist.Text), ips.Count, destDir),
                    "Konfirmasi Distribusi File Massal",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );
                if (confirm != DialogResult.Yes) return;

                btnDistribute.Enabled = false;
                try
                {
                    byte[] fileBytes = File.ReadAllBytes(txtFileDist.Text);
                    string fileName = Path.GetFileName(txtFileDist.Text);
                    var payload = new RemoteConfigPayload
                    {
                        Action = "FILE_DISTRIBUTION",
                        FileName = fileName,
                        TargetDirectory = destDir,
                        FileDataBase64 = Convert.ToBase64String(fileBytes),
                        FileSize = fileBytes.Length
                    };

                    Log(string.Format("Mendistribusikan file '{0}' ({1:N0} MB) serentak ke {2} PC client (Tujuan: {3})...", fileName, fileBytes.Length / (1024.0 * 1024.0), ips.Count, destDir));

                    // Hitung timeout proporsional: minimal 30 detik + 3 detik per MB
                    int sendTimeoutMs = Math.Max(30000, 30000 + (fileBytes.Length / (1024 * 1024)) * 3000);

                    // Gunakan concurrency terkontrol (4 client serentak) agar bandwidth LAN dan buffer TCP tidak jebol
                    var throttle = new SemaphoreSlim(4);
                    var tasks = ips.Select(async ip =>
                    {
                        await throttle.WaitAsync();
                        try
                        {
                            var resp = await scanner.SendRemoteConfigAsync(ip, payload, timeoutMs: sendTimeoutMs);
                            this.BeginInvoke((Action)(() =>
                            {
                                Log(string.Format("{0} ({1}): {2}", resp.Success ? "Sukses" : "Gagal", ip, resp.Message));
                            }));
                        }
                        finally
                        {
                            throttle.Release();
                        }
                    });

                    await Task.WhenAll(tasks);
                    Log("Selesai mendistribusikan file massal.");
                }
                catch (Exception ex)
                {
                    Log("Gagal distribusi file: " + ex.Message);
                }
                finally
                {
                    btnDistribute.Enabled = true;
                }
            };

            cmbCollectSource.Items.AddRange(new object[] {
                "Desktop",
                "Documents",
                "Downloads",
                @"C:\Tugas",
                "Ketik Lokasi Manual..."
            });
            cmbCollectSource.SelectedIndex = 0; // Default: Desktop
            txtSourceDir.Visible = false; // Hidden unless manual
            cmbCollectSource.SelectedIndexChanged += (s, e) =>
            {
                if (cmbCollectSource.SelectedItem?.ToString() == "Ketik Lokasi Manual...")
                {
                    txtSourceDir.Visible = true;
                    txtSourceDir.Focus();
                }
                else
                {
                    txtSourceDir.Visible = false;
                }
            };

            SendMessage(txtSourceDir.Handle, EM_SETCUEBANNER, 0, @"Contoh: C:\Tugas atau D:\Ujian");
            SendMessage(txtCollectPattern.Handle, EM_SETCUEBANNER, 0, @"Contoh: *.* atau *.docx;*.pdf");

            btnBrowseHost.Click += (s, e) =>
            {
                using (var fbd = new FolderBrowserDialog { Description = "Pilih Folder Penyimpanan Tugas di Controller" })
                {
                    if (fbd.SelectedPath != null && fbd.ShowDialog() == DialogResult.OK) txtSaveHost.Text = fbd.SelectedPath;
                }
            };

            btnCollect.Click += async (s, e) =>
            {
                var ips = GetSelectedClientIps(dgvFileClients);
                if (ips.Count == 0)
                {
                    ips = GetAllDiscoveredClientIps();
                }

                if (ips.Count == 0)
                {
                    MessageBox.Show("Tidak ada PC aktif atau dicentang pada tabel Live Dashboard di bawah!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string selSource = cmbCollectSource.SelectedItem?.ToString() ?? "Desktop";
                string clientFolder = (selSource == "Ketik Lokasi Manual...")
                    ? txtSourceDir.Text.Trim().Trim('\"', '\'').Trim()
                    : selSource;

                if (string.IsNullOrEmpty(clientFolder))
                {
                    MessageBox.Show("Tentukan folder sumber tugas di PC siswa!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string pattern = txtCollectPattern.Text.Trim();
                if (string.IsNullOrEmpty(pattern)) pattern = "*.*";
                bool deleteAfter = chkDeleteAfterCollect.Checked;

                string hostDir = txtSaveHost.Text.Trim().Trim('\"', '\'').Trim();
                if (string.IsNullOrEmpty(hostDir))
                {
                    MessageBox.Show("Tentukan folder tujuan penyimpanan di Host Controller!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                try
                {
                    if (!Directory.Exists(hostDir)) Directory.CreateDirectory(hostDir);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Gagal membuat folder penyimpanan di Host: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                btnCollect.Enabled = false;
                Log("=================================================");
                Log(string.Format("Memulai proses penarikan tugas serentak dari {0} PC siswa (Sumber: '{1}', Filter: '{2}', Hapus di Siswa: {3})...", 
                    ips.Count, clientFolder, pattern, deleteAfter ? "Ya" : "Tidak"));

                int successCount = 0;
                var throttle = new SemaphoreSlim(4);
                var tasks = ips.Select(async ip =>
                {
                    await throttle.WaitAsync();
                    try
                    {
                        var payload = new RemoteConfigPayload
                        {
                            Action = "COLLECT_WORK",
                            CollectSourceFolder = clientFolder,
                            CollectFilePattern = pattern,
                            DeleteAfterCollect = deleteAfter
                        };

                        var resp = await scanner.SendRemoteConfigAsync(ip, payload, timeoutMs: 300000);

                        this.BeginInvoke((Action)(() =>
                        {
                            if (resp.Success && !string.IsNullOrEmpty(resp.FileDataResponseBase64))
                            {
                                try
                                {
                                    byte[] zipBytes = Convert.FromBase64String(resp.FileDataResponseBase64);
                                    string cleanHost = !string.IsNullOrEmpty(resp.Hostname) ? resp.Hostname : "PC";
                                    string safeIp = ip.Replace('.', '_').Replace(':', '_');
                                    string saveName = string.Format("Tugas_{0}_{1}_{2}.zip", cleanHost, safeIp, DateTime.Now.ToString("yyyyMMdd_HHmmss_fff"));
                                    string fullHostPath = Path.Combine(hostDir, saveName);

                                    File.WriteAllBytes(fullHostPath, zipBytes);
                                    Interlocked.Increment(ref successCount);
                                    Log(string.Format("{0} ({1}): Tugas berhasil diunduh ({2:N0} KB) -> {3}", cleanHost, ip, zipBytes.Length / 1024, saveName));
                                }
                                catch (Exception ex)
                                {
                                    Log(string.Format("{0} ({1}): Gagal menyimpan file ZIP: {2}", resp.Hostname ?? "PC", ip, ex.Message));
                                }
                            }
                            else
                            {
                                Log(string.Format("{0} ({1}): {2}", resp.Hostname ?? ip, ip, resp.Message));
                            }
                        }));
                    }
                    finally
                    {
                        throttle.Release();
                    }
                });

                await Task.WhenAll(tasks);

                Log("=================================================");
                Log(string.Format("Selesai menarik tugas! Berhasil mengumpulkan {0} dari {1} PC ke folder: {2}", successCount, ips.Count, hostDir));
                btnCollect.Enabled = true;

                if (successCount > 0)
                {
                    try { System.Diagnostics.Process.Start("explorer.exe", hostDir); } catch { }
                }
            };

            // Klik baris pada Live Dashboard otomatis memilih PC di Target PC combo
            dgvFileClients.CellClick += (s, e) =>
            {
                if (e.RowIndex >= 0 && e.RowIndex < dgvFileClients.Rows.Count)
                {
                    string host = dgvFileClients.Rows[e.RowIndex].Cells[2].Value?.ToString();
                    string ip = dgvFileClients.Rows[e.RowIndex].Cells[3].Value?.ToString();
                    if (!string.IsNullOrEmpty(ip))
                    {
                        string targetText = string.Format("{0} ({1})", host, ip);
                        if (!cmbTargetClient.Items.Contains(targetText))
                        {
                            cmbTargetClient.Items.Add(targetText);
                        }
                        cmbTargetClient.SelectedItem = targetText;
                    }
                }
            };

            // Tab 6: Inventory & Audit Event Wiring
            bool isSyncingInventory = false;
            if (chkInventorySelectAll != null && dgvInventoryClients != null)
            {
                chkInventorySelectAll.CheckedChanged += (s, e) =>
                {
                    if (isSyncingInventory) return;
                    isSyncingInventory = true;
                    try
                    {
                        bool c = chkInventorySelectAll.Checked;
                        foreach (DataGridViewRow r in dgvInventoryClients.Rows)
                        {
                            r.Cells[0].Value = c;
                        }
                        dgvInventoryClients.EndEdit();
                        dgvInventoryClients.Refresh();
                    }
                    finally
                    {
                        isSyncingInventory = false;
                    }
                };

                dgvInventoryClients.CurrentCellDirtyStateChanged += (s, e) =>
                {
                    if (dgvInventoryClients.IsCurrentCellDirty)
                    {
                        dgvInventoryClients.CommitEdit(DataGridViewDataErrorContexts.Commit);
                    }
                };

                dgvInventoryClients.CellValueChanged += (s, e) =>
                {
                    if (isSyncingInventory || e.RowIndex < 0 || e.ColumnIndex != 0) return;
                    isSyncingInventory = true;
                    try
                    {
                        bool allChecked = true;
                        foreach (DataGridViewRow r in dgvInventoryClients.Rows)
                        {
                            if (!(r.Cells[0].Value is bool b && b))
                            {
                                allChecked = false;
                                break;
                            }
                        }
                        chkInventorySelectAll.Checked = (dgvInventoryClients.Rows.Count > 0 && allChecked);
                    }
                    finally
                    {
                        isSyncingInventory = false;
                    }
                };

                dgvInventoryClients.CellClick += (s, e) =>
                {
                    if (e.RowIndex >= 0 && e.RowIndex < dgvInventoryClients.Rows.Count)
                    {
                        string ip = dgvInventoryClients.Rows[e.RowIndex].Cells[3].Value?.ToString();
                        if (!string.IsNullOrEmpty(ip) && clientInventoryStore.ContainsKey(ip))
                        {
                            DisplayClientInventory(clientInventoryStore[ip]);
                        }
                    }
                };
            }

            if (btnInventoryScan != null)
                btnInventoryScan.Click += (s, e) => btnScan_Click(s, e);

            if (btnInventoryAuditSelected != null)
            {
                btnInventoryAuditSelected.Click += async (s, e) =>
                {
                    var ips = GetSelectedClientIps(dgvInventoryClients);
                    if (ips.Count == 0)
                    {
                        MessageBox.Show("Pilih minimal satu PC pada daftar komputer di bawah!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    await ExecuteAuditClientsAsync(ips);
                };
            }

            if (btnInventoryAuditAll != null && dgvInventoryClients != null)
            {
                btnInventoryAuditAll.Click += async (s, e) =>
                {
                    var ips = new List<string>();
                    foreach (DataGridViewRow r in dgvInventoryClients.Rows)
                    {
                        string ip = r.Cells[3].Value?.ToString();
                        if (!string.IsNullOrEmpty(ip) && !ips.Contains(ip)) ips.Add(ip);
                    }
                    if (ips.Count == 0)
                    {
                        MessageBox.Show("Tidak ada PC yang terdeteksi untuk diaudit!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    await ExecuteAuditClientsAsync(ips);
                };
            }

            if (txtSoftwareSearch != null)
            {
                txtSoftwareSearch.TextChanged += (s, e) =>
                {
                    FilterSoftwareGrid();
                };
            }

            if (btnExportCsv != null)
                btnExportCsv.Click += (s, e) => ExportInventoryToCsv();

            if (btnExportTxt != null)
                btnExportTxt.Click += (s, e) => ExportInventoryToTxt();
        }

        private async void ExecuteQuickLaunchToSelected(string target, string args)
        {
            var ips = GetSelectedClientIps(dgvLaunchClients);
            if (ips.Count == 0)
            {
                MessageBox.Show("Pilih minimal satu PC pada daftar Live Dashboard di bawah!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Log(string.Format("Meluncurkan '{0}' secara serentak ke {1} PC client terpilih...", target, ips.Count));
            var throttle = new SemaphoreSlim(15);
            var tasks = ips.Select(async ip =>
            {
                await throttle.WaitAsync();
                try
                {
                    var payload = new RemoteConfigPayload
                    {
                        Action = "QUICK_LAUNCH",
                        LaunchTarget = target,
                        LaunchArguments = args
                    };
                    var resp = await scanner.SendRemoteConfigAsync(ip, payload);
                    this.BeginInvoke((Action)(() =>
                    {
                        Log(string.Format("{0} ({1}): {2}", resp.Success ? "" : "", ip, resp.Message));
                    }));
                }
                finally
                {
                    throttle.Release();
                }
            });

            await Task.WhenAll(tasks);
        }

        private async void ExecuteBatchScriptToSelected(string scriptName, string batContent)
        {
            var ips = GetSelectedClientIps(dgvLaunchClients);
            if (ips.Count == 0)
            {
                MessageBox.Show("Pilih minimal satu PC pada daftar Live Dashboard di bawah!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show(
                string.Format("Apakah Anda yakin ingin menjalankan skrip batch '{0}' ke {1} PC client terpilih?", scriptName, ips.Count),
                "Konfirmasi Eksekusi Skrip Batch",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );
            if (confirm != DialogResult.Yes) return;

            Log(string.Format("Mengirim dan menjalankan skrip batch '{0}' ke {1} PC client...", scriptName, ips.Count));
            var throttle = new SemaphoreSlim(15);
            var tasks = ips.Select(async ip =>
            {
                await throttle.WaitAsync();
                try
                {
                    var payload = new RemoteConfigPayload
                    {
                        Action = "EXECUTE_BATCH_SCRIPT",
                        FileName = scriptName,
                        ScriptContent = batContent
                    };
                    var resp = await scanner.SendRemoteConfigAsync(ip, payload);
                    this.BeginInvoke((Action)(() =>
                    {
                        Log(string.Format("{0} ({1}): {2}", resp.Success ? "OK" : "GAGAL", ip, resp.Message));
                    }));
                }
                finally
                {
                    throttle.Release();
                }
            });

            await Task.WhenAll(tasks);
        }

        private void ShowRemotePowerShellDialog()
        {
            var selectedIps = GetSelectedClientIps(dgvLaunchClients);
            var allIps = GetAllDiscoveredClientIps();
            var targets = selectedIps.Count > 0 ? selectedIps : allIps;

            if (targets.Count == 0)
            {
                MessageBox.Show("Tidak ada PC client yang terdeteksi atau dipilih!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var dlg = new Form
            {
                Text = "Remote PowerShell (Semua / PC Terpilih)",
                Size = new Size(620, 520),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = Color.FromArgb(241, 245, 249)
            })
            {
                var lblHeader = new Label
                {
                    Text = string.Format("Menjalankan script PowerShell secara senyap di background client.\nTarget saat ini: {0} PC ({1})",
                        targets.Count, selectedIps.Count > 0 ? "PC Dicentang" : "Semua PC Aktif"),
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                    Location = new Point(15, 12),
                    Size = new Size(570, 36)
                };

                var pnlTarget = new Panel { Location = new Point(15, 52), Size = new Size(570, 28) };
                var rdoSelected = new RadioButton { Text = string.Format("PC Terpilih ({0})", selectedIps.Count), Location = new Point(0, 4), Size = new Size(160, 20), Checked = selectedIps.Count > 0, Enabled = selectedIps.Count > 0 };
                var rdoAll = new RadioButton { Text = string.Format("Semua PC Aktif ({0})", allIps.Count), Location = new Point(170, 4), Size = new Size(160, 20), Checked = selectedIps.Count == 0 };
                pnlTarget.Controls.AddRange(new Control[] { rdoSelected, rdoAll });

                var cmbPresets = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Location = new Point(15, 85), Size = new Size(570, 25) };
                cmbPresets.Items.AddRange(new object[] {
                    "-- Pilih Contoh / Preset Script Cepat --",
                    "Buka Website Browser: Start-Process 'msedge.exe' 'https://amikom.ac.id'",
                    "Kill Aplikasi Tertentu: Stop-Process -Name 'chrome','msedge','notepad' -Force -ErrorAction SilentlyContinue",
                    "Cek Info Windows & Hostname: hostname; systeminfo | Select-String 'OS Name','Total Physical Memory'",
                    "Flush DNS & Renew IP: ipconfig /flushdns; ipconfig /renew",
                    "Tampilkan Pesan Pop-up: msg * 'Pemberitahuan dari Dosen/Laboran: Harap perhatikan layar depan!'"
                });
                cmbPresets.SelectedIndex = 0;

                var txtScript = new TextBox
                {
                    Multiline = true,
                    ScrollBars = ScrollBars.Both,
                    Font = new Font("Consolas", 10F),
                    Location = new Point(15, 118),
                    Size = new Size(570, 305),
                    BackColor = Color.FromArgb(15, 23, 42),
                    ForeColor = Color.FromArgb(226, 232, 240)
                };

                cmbPresets.SelectedIndexChanged += (s, e) =>
                {
                    if (cmbPresets.SelectedIndex == 1) txtScript.Text = "Start-Process 'msedge.exe' 'https://amikom.ac.id'";
                    else if (cmbPresets.SelectedIndex == 2) txtScript.Text = "Stop-Process -Name 'chrome','msedge','notepad' -Force -ErrorAction SilentlyContinue";
                    else if (cmbPresets.SelectedIndex == 3) txtScript.Text = "hostname\r\nGet-CimInstance Win32_OperatingSystem | Select-Object Caption, OSArchitecture, Version";
                    else if (cmbPresets.SelectedIndex == 4) txtScript.Text = "ipconfig /flushdns\r\nipconfig /renew";
                    else if (cmbPresets.SelectedIndex == 5) txtScript.Text = "msg * 'Pemberitahuan dari Dosen/Laboran: Harap perhatikan layar depan!'";
                };

                var btnCancel = new Button { Text = "Batal", DialogResult = DialogResult.Cancel, Location = new Point(360, 435), Size = new Size(100, 34) };
                var btnExecute = new Button
                {
                    Text = "Jalankan Script",
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                    BackColor = Color.FromArgb(15, 118, 110),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Location = new Point(470, 435),
                    Size = new Size(115, 34)
                };
                btnExecute.FlatAppearance.BorderSize = 0;
                btnExecute.Click += async (s, e) =>
                {
                    string script = txtScript.Text.Trim();
                    if (string.IsNullOrEmpty(script))
                    {
                        MessageBox.Show("Script PowerShell tidak boleh kosong!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    targets = rdoSelected.Checked ? selectedIps : allIps;
                    if (targets.Count == 0)
                    {
                        MessageBox.Show("Tidak ada target PC yang dipilih!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    var confirm = MessageBox.Show(
                        string.Format("Apakah Anda yakin ingin menjalankan script PowerShell ini ke {0} PC client?", targets.Count),
                        "Konfirmasi Eksekusi Remote PowerShell",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question
                    );
                    if (confirm != DialogResult.Yes) return;

                    dlg.Close();

                    Log("=================================================");
                    Log(string.Format("Memulai eksekusi Remote PowerShell serentak ke {0} PC client...", targets.Count));
                    Log(string.Format("Perintah: {0}", script.Replace("\r\n", " | ")));

                    int successCount = 0;
                    var throttle = new SemaphoreSlim(15);
                    var tasks = targets.Select(async ip =>
                    {
                        await throttle.WaitAsync();
                        try
                        {
                            var payload = new RemoteConfigPayload
                            {
                                Action = "REMOTE_POWERSHELL",
                                ScriptContent = script
                            };
                            var resp = await scanner.SendRemoteConfigAsync(ip, payload);
                            this.BeginInvoke((Action)(() =>
                            {
                                if (resp.Success)
                                {
                                    Interlocked.Increment(ref successCount);
                                    Log(string.Format("{0} ({1}): {2}", resp.Hostname ?? "PC", ip, resp.Message));
                                    if (!string.IsNullOrEmpty(resp.OutputLog))
                                    {
                                        Log(string.Format("   [Output {0}]:\n{1}", resp.Hostname ?? ip, resp.OutputLog));
                                    }
                                }
                                else
                                {
                                    Log(string.Format("{0} ({1}): {2}", resp.Hostname ?? "PC", ip, resp.Message));
                                    if (!string.IsNullOrEmpty(resp.OutputLog))
                                    {
                                        Log(string.Format("   [Error {0}]: {1}", resp.Hostname ?? ip, resp.OutputLog));
                                    }
                                }
                            }));
                        }
                        finally
                        {
                            throttle.Release();
                        }
                    });

                    await Task.WhenAll(tasks);
                    Log(string.Format("Selesai mengeksekusi Remote PowerShell ({0}/{1} berhasil).", successCount, targets.Count));
                    Log("=================================================");
                };

                dlg.Controls.AddRange(new Control[] { lblHeader, pnlTarget, cmbPresets, txtScript, btnCancel, btnExecute });
                dlg.ShowDialog(this);
            }
        }

        private string ExtractIpFromString(string input)
        {
            if (string.IsNullOrEmpty(input)) return "";
            var match = Regex.Match(input, @"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b");
            return match.Success ? match.Value : "";
        }
        #endregion

        #region Inventory & Audit Helper Methods
        private Dictionary<string, ClientInventoryData> clientInventoryStore = new Dictionary<string, ClientInventoryData>(StringComparer.OrdinalIgnoreCase);
        private ClientInventoryData currentSelectedInventory = null;

        private async Task ExecuteAuditClientsAsync(List<string> ips)
        {
            btnInventoryAuditSelected.Enabled = false;
            btnInventoryAuditAll.Enabled = false;

            Log("=================================================");
            Log(string.Format("📋 Memulai Audit Hardware & Software terinstall pada {0} PC client...", ips.Count));

            int successCount = 0;
            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer { MaxJsonLength = int.MaxValue };
            var throttle = new SemaphoreSlim(15);

            var tasks = ips.Select(async ip =>
            {
                await throttle.WaitAsync();
                try
                {
                    var payload = new RemoteConfigPayload { Action = "GET_INVENTORY" };
                    var resp = await scanner.SendRemoteConfigAsync(ip, payload);

                    this.BeginInvoke((Action)(() =>
                    {
                        if (resp.Success && !string.IsNullOrEmpty(resp.InventoryDataJson))
                        {
                            try
                            {
                                var inv = serializer.Deserialize<ClientInventoryData>(resp.InventoryDataJson);
                                if (inv != null)
                                {
                                    inv.Hardware.IpAddress = ip;
                                    clientInventoryStore[ip] = inv;
                                    Interlocked.Increment(ref successCount);

                                    // Update baris dgvInventoryClients
                                    foreach (DataGridViewRow r in dgvInventoryClients.Rows)
                                    {
                                        if (r.Cells[3].Value?.ToString() == ip)
                                        {
                                            r.Cells[4].Value = string.Format("Audit OK ({0} App)", inv.SoftwareList.Count);
                                            r.Cells[4].Style.ForeColor = Color.DarkGreen;
                                        }
                                    }

                                    Log(string.Format("✅ {0} ({1}): Audit sukses ({2} software)", resp.Hostname ?? "PC", ip, inv.SoftwareList.Count));

                                    // Jika sedang melihat PC ini atau belum ada yang dipilih, tampilkan langsung
                                    if (currentSelectedInventory == null || currentSelectedInventory.Hardware.IpAddress == ip)
                                    {
                                        DisplayClientInventory(inv);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Log(string.Format("❌ {0} ({1}): Gagal mengurai data inventory: {2}", resp.Hostname ?? "PC", ip, ex.Message));
                            }
                        }
                        else
                        {
                            Log(string.Format("❌ {0} ({1}): {2}", resp.Hostname ?? "PC", ip, resp.Message));
                            foreach (DataGridViewRow r in dgvInventoryClients.Rows)
                            {
                                if (r.Cells[3].Value?.ToString() == ip)
                                {
                                    r.Cells[4].Value = "Audit Gagal";
                                    r.Cells[4].Style.ForeColor = Color.DarkRed;
                                }
                            }
                        }
                    }));
                }
                finally
                {
                    throttle.Release();
                }
            });

            await Task.WhenAll(tasks);
            Log(string.Format("📋 Selesai Audit! Berhasil mengumpulkan inventory dari {0} / {1} PC client.", successCount, ips.Count));
            Log("=================================================");

            btnInventoryAuditSelected.Enabled = true;
            btnInventoryAuditAll.Enabled = true;
        }

        private void DisplayClientInventory(ClientInventoryData inv)
        {
            if (inv == null) return;
            currentSelectedInventory = inv;

            lblInvSelectedClient.Text = string.Format("Detail Komputer: {0} ({1})", inv.Hardware.MachineName ?? "PC", inv.Hardware.IpAddress ?? "-");

            // Isi Data Hardware ke Grid/Tabel Hardware
            dgvHardwareDetail.Rows.Clear();
            dgvHardwareDetail.Rows.Add("Machine Name", inv.Hardware.MachineName ?? "-");
            dgvHardwareDetail.Rows.Add("Operating System (OS)", inv.Hardware.OsName ?? "-");
            dgvHardwareDetail.Rows.Add("IP Address", inv.Hardware.IpAddress ?? "-");
            dgvHardwareDetail.Rows.Add("MAC Address", inv.Hardware.MacAddress ?? "-");
            dgvHardwareDetail.Rows.Add("Processor (CPU)", inv.Hardware.Processor ?? "-");
            dgvHardwareDetail.Rows.Add("Total Physical RAM", inv.Hardware.Ram ?? "-");
            dgvHardwareDetail.Rows.Add("Motherboard / BaseBoard", inv.Hardware.Motherboard ?? "-");
            dgvHardwareDetail.Rows.Add("Disk Drives / Storage", inv.Hardware.Storage ?? "-");

            FilterSoftwareGrid();
        }

        private void FilterSoftwareGrid()
        {
            if (currentSelectedInventory == null) return;

            dgvSoftwareList.Rows.Clear();
            string keyword = txtSoftwareSearch.Text.Trim();

            var list = currentSelectedInventory.SoftwareList;
            if (!string.IsNullOrEmpty(keyword))
            {
                list = list.Where(s => (s.Name != null && s.Name.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0) ||
                                       (s.Publisher != null && s.Publisher.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)).ToList();
            }

            int no = 1;
            foreach (var sw in list)
            {
                dgvSoftwareList.Rows.Add(no++, sw.Name, sw.Version, sw.Publisher, sw.InstallDate);
            }

            lblSoftwareCount.Text = string.Format("Total Software: {0} (Ditampilkan: {1})", currentSelectedInventory.SoftwareList.Count, list.Count);
        }

        private void ExportInventoryToCsv()
        {
            if (clientInventoryStore.Count == 0)
            {
                MessageBox.Show("Belum ada data inventory yang diaudit! Silakan klik 'Audit' terlebih dahulu.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var sfd = new SaveFileDialog
            {
                Title = "Export Inventory ke CSV (Excel)",
                Filter = "CSV Files (*.csv)|*.csv|All Files (*.*)|*.*",
                FileName = string.Format("AirSET_Inventory_{0}.csv", DateTime.Now.ToString("yyyyMMdd_HHmmss"))
            })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var sb = new System.Text.StringBuilder();
                        // CSV Header
                        sb.AppendLine("Hostname,IP Address,MAC Address,OS,Processor,RAM,Motherboard,Storage,Software Name,Version,Publisher,Install Date");

                        foreach (var kvp in clientInventoryStore)
                        {
                            var inv = kvp.Value;
                            string hwPart = string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\"",
                                EscapeCsv(inv.Hardware.MachineName),
                                EscapeCsv(inv.Hardware.IpAddress),
                                EscapeCsv(inv.Hardware.MacAddress),
                                EscapeCsv(inv.Hardware.OsName),
                                EscapeCsv(inv.Hardware.Processor),
                                EscapeCsv(inv.Hardware.Ram),
                                EscapeCsv(inv.Hardware.Motherboard),
                                EscapeCsv(inv.Hardware.Storage));

                            if (inv.SoftwareList.Count == 0)
                            {
                                sb.AppendLine(string.Format("{0},\"-\",\"-\",\"-\",\"-\"", hwPart));
                            }
                            else
                            {
                                foreach (var sw in inv.SoftwareList)
                                {
                                    sb.AppendLine(string.Format("{0},\"{1}\",\"{2}\",\"{3}\",\"{4}\"",
                                        hwPart,
                                        EscapeCsv(sw.Name),
                                        EscapeCsv(sw.Version),
                                        EscapeCsv(sw.Publisher),
                                        EscapeCsv(sw.InstallDate)));
                                }
                            }
                        }

                        File.WriteAllText(sfd.FileName, sb.ToString(), System.Text.Encoding.UTF8);
                        MessageBox.Show("Data inventory berhasil diexport ke CSV:\n" + sfd.FileName, "Export Berhasil", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        try { System.Diagnostics.Process.Start(sfd.FileName); } catch { }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Gagal mengexport CSV: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void ExportInventoryToTxt()
        {
            if (clientInventoryStore.Count == 0)
            {
                MessageBox.Show("Belum ada data inventory yang diaudit! Silakan klik 'Audit' terlebih dahulu.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var sfd = new SaveFileDialog
            {
                Title = "Export Inventory ke File Teks (TXT)",
                Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*",
                FileName = string.Format("AirSET_Inventory_{0}.txt", DateTime.Now.ToString("yyyyMMdd_HHmmss"))
            })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var sb = new System.Text.StringBuilder();
                        sb.AppendLine("================================================================================");
                        sb.AppendLine("               AIRSET CLASSROOM - HARDWARE & SOFTWARE INVENTORY AUDIT            ");
                        sb.AppendLine(string.Format("Tanggal Ekspor : {0}", DateTime.Now.ToString("dd MMMM yyyy HH:mm:ss")));
                        sb.AppendLine(string.Format("Total PC Lab   : {0} PC Terdata", clientInventoryStore.Count));
                        sb.AppendLine("================================================================================");
                        sb.AppendLine();

                        int pcNum = 1;
                        foreach (var kvp in clientInventoryStore)
                        {
                            var inv = kvp.Value;
                            sb.AppendLine(string.Format("[{0}] KOMPUTER: {1} ({2})", pcNum++, inv.Hardware.MachineName, inv.Hardware.IpAddress));
                            sb.AppendLine("--------------------------------------------------------------------------------");
                            sb.AppendLine(string.Format("  - Sistem Operasi : {0}", inv.Hardware.OsName));
                            sb.AppendLine(string.Format("  - MAC Address    : {0}", inv.Hardware.MacAddress));
                            sb.AppendLine(string.Format("  - Processor      : {0}", inv.Hardware.Processor));
                            sb.AppendLine(string.Format("  - RAM Fisik      : {0}", inv.Hardware.Ram));
                            sb.AppendLine(string.Format("  - Motherboard    : {0}", inv.Hardware.Motherboard));
                            sb.AppendLine(string.Format("  - Storage Drive  : {0}", inv.Hardware.Storage));
                            sb.AppendLine(string.Format("  - Total Software : {0} Aplikasi Terinstall", inv.SoftwareList.Count));
                            sb.AppendLine();
                            sb.AppendLine("  DAFTAR APLIKASI (PROGRAMS & FEATURES):");
                            sb.AppendLine(string.Format("  {0,-4} | {1,-45} | {2,-18} | {3}", "No", "Nama Software", "Versi", "Publisher"));
                            sb.AppendLine("  " + new string('-', 95));

                            int swNo = 1;
                            foreach (var sw in inv.SoftwareList)
                            {
                                string name = (sw.Name.Length > 45) ? sw.Name.Substring(0, 42) + "..." : sw.Name;
                                string ver = (sw.Version.Length > 18) ? sw.Version.Substring(0, 15) + "..." : sw.Version;
                                sb.AppendLine(string.Format("  {0,-4} | {1,-45} | {2,-18} | {3}", swNo++, name, ver, sw.Publisher));
                            }
                            sb.AppendLine();
                            sb.AppendLine(new string('=', 80));
                            sb.AppendLine();
                        }

                        File.WriteAllText(sfd.FileName, sb.ToString(), System.Text.Encoding.UTF8);
                        MessageBox.Show("Data inventory berhasil diexport ke TXT:\n" + sfd.FileName, "Export Berhasil", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        try { System.Diagnostics.Process.Start(sfd.FileName); } catch { }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Gagal mengexport TXT: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private string EscapeCsv(string s)
        {
            if (string.IsNullOrEmpty(s)) return "";
            return s.Replace("\"", "\"\"");
        }

        #region Dynamic UI Builder: Wallpaper & Tab 6 Inventory
        // Wallpaper controls
        private GroupBox grpWallpaper;
        private TextBox txtWallpaperPath;
        private Button btnBrowseWallpaper;
        private ComboBox cmbWallpaperStyle;
        private Button btnApplyWallpaper;
        private PictureBox picWallpaperPreview;

        // Tab 6 Inventory controls
        private TabPage tabInventory;
        private SplitContainer splitInventory;
        private GroupBox grpInventoryLeft;
        private Button btnInventoryScan;
        private CheckBox chkInventorySelectAll;
        private Label lblInventoryTotal;
        private DataGridView dgvInventoryClients;

        private Panel pnlInventoryRight;
        private Panel pnlInvTopBar;
        private FlowLayoutPanel flpInvActions;
        private Label lblInvSelectedClient;
        private Button btnInventoryAuditSelected;
        private Button btnInventoryAuditAll;
        private Button btnExportCsv;
        private Button btnExportTxt;

        private TabControl tabInvDetails;
        private TabPage tabInvHardware;
        private TabPage tabInvSoftware;

        private DataGridView dgvHardwareDetail;
        private Panel pnlSoftwareSearch;
        private Label lblSoftwareSearch;
        private TextBox txtSoftwareSearch;
        private Label lblSoftwareCount;
        private DataGridView dgvSoftwareList;

        // Tab 7 Live Screen View (NetSupport Style Multi-PC Grid)
        private TabPage tabLiveScreen;
        private FlowLayoutPanel flpScreens;
        private Panel pnlLiveScreenToolbar;
        private Button btnToggleLiveMonitor;
        private Button btnRefreshScreensOnce;
        private ComboBox cmbRefreshInterval;
        private ComboBox cmbThumbSize;
        private Label lblLiveScreenTotal;
        private Label lblLiveScreenInterval;
        private Label lblLiveScreenSize;
        private System.Windows.Forms.Timer tmrScreenRefresh;
        private bool isScreenMonitoring = false;
        private bool isScreenRefreshing = false;
        private Dictionary<string, PcScreenCard> pcScreenCards = new Dictionary<string, PcScreenCard>();

        private void BuildWallpaperControls()
        {
            try
            {
                // Adjust height of grpPower to 80px
                grpPower.Height = 80;
                tblPower.Height = 36;
                lblWolHint.Visible = false;

                // Buat GroupBox Wallpaper di bawah grpPower
                grpWallpaper = new GroupBox
                {
                    Text = "🖼️ Batch Wallpaper Changer (Ganti Latar Belakang Layar Siswa)",
                    Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(30, 41, 59),
                    BackColor = Color.FromArgb(248, 250, 252),
                    Location = new Point(6, 178),
                    Size = new Size(grpPower.Width, 76),
                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
                };

                var lblFile = new Label
                {
                    Text = "File Gambar:",
                    Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                    Location = new Point(15, 26),
                    Size = new Size(80, 24),
                    TextAlign = ContentAlignment.MiddleLeft
                };

                txtWallpaperPath = new TextBox
                {
                    Location = new Point(100, 26),
                    Size = new Size(240, 24),
                    Font = new Font("Segoe UI", 9F)
                };

                btnBrowseWallpaper = new Button
                {
                    Text = "📁 Browse...",
                    Location = new Point(345, 25),
                    Size = new Size(85, 26),
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand,
                    Font = new Font("Segoe UI", 9F)
                };

                picWallpaperPreview = new PictureBox
                {
                    Location = new Point(435, 20),
                    Size = new Size(42, 34),
                    SizeMode = PictureBoxSizeMode.Zoom,
                    BorderStyle = BorderStyle.FixedSingle,
                    BackColor = Color.FromArgb(226, 232, 240)
                };

                var lblStyle = new Label
                {
                    Text = "Mode:",
                    Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                    Location = new Point(485, 26),
                    Size = new Size(45, 24),
                    TextAlign = ContentAlignment.MiddleRight
                };

                cmbWallpaperStyle = new ComboBox
                {
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    Location = new Point(535, 26),
                    Size = new Size(95, 24),
                    Font = new Font("Segoe UI", 9F)
                };
                cmbWallpaperStyle.Items.AddRange(new object[] { "Fill", "Fit", "Stretch", "Tile", "Center" });
                cmbWallpaperStyle.SelectedIndex = 0;

                btnApplyWallpaper = new Button
                {
                    Text = "🎨 Pasang Wallpaper ke PC Dicentang",
                    Location = new Point(640, 22),
                    Size = new Size(295, 32),
                    BackColor = Color.FromArgb(15, 118, 110),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                    Cursor = Cursors.Hand,
                    Anchor = AnchorStyles.Top | AnchorStyles.Right
                };
                btnApplyWallpaper.FlatAppearance.BorderSize = 0;

                grpWallpaper.Controls.AddRange(new Control[] {
                    lblFile, txtWallpaperPath, btnBrowseWallpaper, picWallpaperPreview,
                    lblStyle, cmbWallpaperStyle, btnApplyWallpaper
                });

                tabPower.Controls.Add(grpWallpaper);

                // Geser grpLivePower ke Y = 258 agar tidak tumpang tindih
                grpLivePower.Location = new Point(6, 258);
                grpLivePower.Size = new Size(tabPower.Width - 12, tabPower.Height - 264);
            }
            catch (Exception ex)
            {
                Log("BuildWallpaperControls error: " + ex.Message);
            }
        }

        private void BuildTabInventory()
        {
            try
            {
                tabInventory = new TabPage
                {
                    Text = " 📋 Inventory ",
                    BackColor = Color.FromArgb(241, 245, 249),
                    Padding = new Padding(4)
                };

                // Split horizontal: Kiri (Daftar PC) & Kanan (Detail NetSupport style)
                splitInventory = new SplitContainer
                {
                    Dock = DockStyle.Fill,
                    Orientation = Orientation.Vertical,
                    SplitterWidth = 6,
                    BackColor = Color.FromArgb(226, 232, 240)
                };
                var split = splitInventory;

                #region Kiri: Daftar PC
                grpInventoryLeft = new GroupBox
                {
                    Text = "Daftar Komputer Lab (Live)",
                    Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(30, 41, 59),
                    BackColor = Color.FromArgb(248, 250, 252),
                    Dock = DockStyle.Fill,
                    Padding = new Padding(8)
                };

                btnInventoryScan = new Button
                {
                    Text = "🔍 Scan Otomatis",
                    Location = new Point(10, 24),
                    Size = new Size(130, 28),
                    BackColor = Color.FromArgb(15, 118, 110),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                    Cursor = Cursors.Hand
                };
                btnInventoryScan.FlatAppearance.BorderSize = 0;

                chkInventorySelectAll = new CheckBox
                {
                    Text = "Pilih Semua ([✓])",
                    Location = new Point(148, 28),
                    Size = new Size(130, 20),
                    Checked = true,
                    Font = new Font("Segoe UI", 8.5F)
                };

                lblInventoryTotal = new Label
                {
                    Text = "Total PC: 0",
                    Location = new Point(10, 56),
                    Size = new Size(200, 18),
                    ForeColor = Color.DarkGreen,
                    Font = new Font("Segoe UI", 8.5F, FontStyle.Bold)
                };

                dgvInventoryClients = new DataGridView
                {
                    Location = new Point(8, 78),
                    Size = new Size(grpInventoryLeft.Width - 16, grpInventoryLeft.Height - 86),
                    Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                    AllowUserToAddRows = false,
                    AllowUserToDeleteRows = false,
                    BackgroundColor = Color.FromArgb(241, 245, 249),
                    BorderStyle = BorderStyle.None,
                    RowHeadersVisible = false,
                    SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                    MultiSelect = false
                };

                var cCheck = new DataGridViewCheckBoxColumn { HeaderText = "✓", Width = 30 };
                var cNo = new DataGridViewTextBoxColumn { HeaderText = "No", Width = 45, ReadOnly = true };
                var cHost = new DataGridViewTextBoxColumn { HeaderText = "Hostname", Width = 110, ReadOnly = true };
                var cIp = new DataGridViewTextBoxColumn { HeaderText = "IP Address", Width = 100, ReadOnly = true };
                var cStatus = new DataGridViewTextBoxColumn { HeaderText = "Status Audit", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, ReadOnly = true };

                dgvInventoryClients.Columns.AddRange(new DataGridViewColumn[] { cCheck, cNo, cHost, cIp, cStatus });

                grpInventoryLeft.Controls.AddRange(new Control[] {
                    btnInventoryScan, chkInventorySelectAll, lblInventoryTotal, dgvInventoryClients
                });
                split.Panel1.Controls.Add(grpInventoryLeft);
                #endregion

                #region Kanan: Detail Inventory (NetSupport Style)
                pnlInventoryRight = new Panel
                {
                    Dock = DockStyle.Fill,
                    BackColor = Color.FromArgb(248, 250, 252),
                    Padding = new Padding(6)
                };

                // Top action bar
                pnlInvTopBar = new Panel
                {
                    Dock = DockStyle.Top,
                    Height = 68,
                    BackColor = Color.FromArgb(241, 245, 249)
                };

                lblInvSelectedClient = new Label
                {
                    Text = "Detail Komputer: (Pilih PC di sebelah kiri)",
                    Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(15, 118, 110),
                    Location = new Point(4, 4),
                    Size = new Size(500, 24)
                };

                flpInvActions = new FlowLayoutPanel
                {
                    Location = new Point(2, 30),
                    Size = new Size(pnlInvTopBar.Width - 4, 34),
                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                    BackColor = Color.Transparent,
                    WrapContents = false,
                    AutoScroll = false,
                    Margin = new Padding(0)
                };

                btnInventoryAuditSelected = new Button
                {
                    Text = "🔍 Audit PC Terpilih",
                    Size = new Size(135, 30),
                    Margin = new Padding(2, 2, 4, 2),
                    BackColor = Color.FromArgb(15, 118, 110),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                    Cursor = Cursors.Hand
                };
                btnInventoryAuditSelected.FlatAppearance.BorderSize = 0;

                btnInventoryAuditAll = new Button
                {
                    Text = "⚡ Audit Semua PC",
                    Size = new Size(130, 30),
                    Margin = new Padding(2, 2, 4, 2),
                    BackColor = Color.FromArgb(20, 184, 166),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                    Cursor = Cursors.Hand
                };
                btnInventoryAuditAll.FlatAppearance.BorderSize = 0;

                btnExportCsv = new Button
                {
                    Text = "💾 Export CSV (Excel)",
                    Size = new Size(140, 30),
                    Margin = new Padding(2, 2, 4, 2),
                    BackColor = Color.FromArgb(16, 185, 129),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                    Cursor = Cursors.Hand
                };
                btnExportCsv.FlatAppearance.BorderSize = 0;

                btnExportTxt = new Button
                {
                    Text = "📄 Export TXT",
                    Size = new Size(110, 30),
                    Margin = new Padding(2, 2, 4, 2),
                    BackColor = Color.FromArgb(71, 85, 105),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                    Cursor = Cursors.Hand
                };
                btnExportTxt.FlatAppearance.BorderSize = 0;

                flpInvActions.Controls.AddRange(new Control[] {
                    btnInventoryAuditSelected, btnInventoryAuditAll, btnExportCsv, btnExportTxt
                });

                pnlInvTopBar.Controls.AddRange(new Control[] {
                    lblInvSelectedClient, flpInvActions
                });

                // Detail Tabs (Hardware & Software)
                tabInvDetails = new TabControl
                {
                    Dock = DockStyle.Fill,
                    Font = new Font("Segoe UI", 9F)
                };

                // SubTab 1: Hardware
                tabInvHardware = new TabPage { Text = " 🖥️ Hardware Machine ", BackColor = Color.White };
                dgvHardwareDetail = new DataGridView
                {
                    Dock = DockStyle.Fill,
                    BackgroundColor = Color.White,
                    BorderStyle = BorderStyle.None,
                    AllowUserToAddRows = false,
                    AllowUserToDeleteRows = false,
                    RowHeadersVisible = false,
                    SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                    ReadOnly = true
                };
                var colHwProp = new DataGridViewTextBoxColumn { HeaderText = "Komponen Hardware", Width = 200 };
                var colHwVal = new DataGridViewTextBoxColumn { HeaderText = "Spesifikasi / Nilai Terdeteksi", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill };
                dgvHardwareDetail.Columns.AddRange(new DataGridViewColumn[] { colHwProp, colHwVal });
                tabInvHardware.Controls.Add(dgvHardwareDetail);

                // SubTab 2: Software (Programs & Features)
                tabInvSoftware = new TabPage { Text = " 📦 Software (Programs & Features) ", BackColor = Color.White };

                pnlSoftwareSearch = new Panel
                {
                    Dock = DockStyle.Top,
                    Height = 36,
                    BackColor = Color.FromArgb(248, 250, 252)
                };
                lblSoftwareSearch = new Label
                {
                    Text = "Cari Software:",
                    Location = new Point(6, 9),
                    Size = new Size(85, 20),
                    Font = new Font("Segoe UI", 9F)
                };
                txtSoftwareSearch = new TextBox
                {
                    Location = new Point(95, 6),
                    Size = new Size(250, 23),
                    Font = new Font("Segoe UI", 9F)
                };
                lblSoftwareCount = new Label
                {
                    Text = "Total Software: 0",
                    Location = new Point(360, 9),
                    Size = new Size(240, 20),
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(71, 85, 105)
                };
                pnlSoftwareSearch.Controls.AddRange(new Control[] { lblSoftwareSearch, txtSoftwareSearch, lblSoftwareCount });

                dgvSoftwareList = new DataGridView
                {
                    Dock = DockStyle.Fill,
                    BackgroundColor = Color.White,
                    BorderStyle = BorderStyle.None,
                    AllowUserToAddRows = false,
                    AllowUserToDeleteRows = false,
                    RowHeadersVisible = false,
                    SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                    ReadOnly = true
                };
                var colSwNo = new DataGridViewTextBoxColumn { HeaderText = "No", Width = 45 };
                var colSwName = new DataGridViewTextBoxColumn { HeaderText = "Nama Software / Aplikasi", Width = 260 };
                var colSwVer = new DataGridViewTextBoxColumn { HeaderText = "Versi", Width = 110 };
                var colSwPub = new DataGridViewTextBoxColumn { HeaderText = "Publisher", Width = 160 };
                var colSwDate = new DataGridViewTextBoxColumn { HeaderText = "Tanggal Install", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill };
                dgvSoftwareList.Columns.AddRange(new DataGridViewColumn[] { colSwNo, colSwName, colSwVer, colSwPub, colSwDate });

                tabInvSoftware.Controls.Add(dgvSoftwareList);
                tabInvSoftware.Controls.Add(pnlSoftwareSearch);

                tabInvDetails.TabPages.AddRange(new TabPage[] { tabInvHardware, tabInvSoftware });

                pnlInventoryRight.Controls.Add(tabInvDetails);
                pnlInventoryRight.Controls.Add(pnlInvTopBar);
                split.Panel2.Controls.Add(pnlInventoryRight);
                #endregion

                tabInventory.Controls.Add(split);
                tabInventory.Resize += (s, e) => AdjustTab6Layout();
                tabMain.TabPages.Add(tabInventory);

                // Terapkan modern theme ke grid baru
                var newGrids = new DataGridView[] { dgvInventoryClients, dgvHardwareDetail, dgvSoftwareList };
                foreach (var g in newGrids)
                {
                    g.EnableHeadersVisualStyles = false;
                    g.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(15, 118, 110);
                    g.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                    g.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                    g.RowTemplate.Height = 26;
                    g.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
                }
            }
            catch (Exception ex)
            {
                Log("BuildTabInventory error: " + ex.Message);
            }
        }
        #endregion

        #region Tab 7: Live Screen View (NetSupport Style Multi-PC Monitor)
        private void BuildTabLiveScreen()
        {
            try
            {
                tabLiveScreen = new TabPage
                {
                    Text = " 🖥️ Live Screen View ",
                    BackColor = Color.FromArgb(241, 245, 249),
                    Padding = new Padding(4)
                };

                // Toolbar kontrol di bagian atas
                pnlLiveScreenToolbar = new Panel
                {
                    BackColor = Color.FromArgb(248, 250, 252),
                    BorderStyle = BorderStyle.FixedSingle,
                    Location = new Point(6, 6),
                    Size = new Size(tabLiveScreen.ClientSize.Width - 12, 46)
                };

                btnToggleLiveMonitor = new Button
                {
                    Text = "▶️ Mulai Pantau Layar",
                    Location = new Point(10, 8),
                    Size = new Size(160, 30),
                    BackColor = Color.FromArgb(15, 118, 110),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                    Cursor = Cursors.Hand
                };
                btnToggleLiveMonitor.FlatAppearance.BorderSize = 0;
                btnToggleLiveMonitor.Click += (s, e) => ToggleLiveScreenMonitoring();

                btnRefreshScreensOnce = new Button
                {
                    Text = "🔄 Refresh 1x",
                    Location = new Point(178, 8),
                    Size = new Size(110, 30),
                    BackColor = Color.FromArgb(30, 41, 59),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                    Cursor = Cursors.Hand
                };
                btnRefreshScreensOnce.FlatAppearance.BorderSize = 0;
                btnRefreshScreensOnce.Click += async (s, e) => await RefreshAllClientScreensAsync();

                lblLiveScreenInterval = new Label
                {
                    Text = "Interval:",
                    Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                    Location = new Point(300, 14),
                    Size = new Size(55, 20),
                    TextAlign = ContentAlignment.MiddleRight
                };

                cmbRefreshInterval = new ComboBox
                {
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    Location = new Point(360, 11),
                    Size = new Size(85, 23),
                    Font = new Font("Segoe UI", 9F)
                };
                cmbRefreshInterval.Items.AddRange(new object[] { "1 Detik", "2 Detik", "3 Detik", "5 Detik" });
                cmbRefreshInterval.SelectedIndex = 1; // Default 2 Detik
                cmbRefreshInterval.SelectedIndexChanged += (s, e) =>
                {
                    if (tmrScreenRefresh != null)
                    {
                        int sec = 2;
                        if (cmbRefreshInterval.SelectedIndex == 0) sec = 1;
                        else if (cmbRefreshInterval.SelectedIndex == 1) sec = 2;
                        else if (cmbRefreshInterval.SelectedIndex == 2) sec = 3;
                        else if (cmbRefreshInterval.SelectedIndex == 3) sec = 5;
                        tmrScreenRefresh.Interval = sec * 1000;
                    }
                };

                lblLiveScreenSize = new Label
                {
                    Text = "Ukuran:",
                    Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                    Location = new Point(455, 14),
                    Size = new Size(55, 20),
                    TextAlign = ContentAlignment.MiddleRight
                };

                cmbThumbSize = new ComboBox
                {
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    Location = new Point(515, 11),
                    Size = new Size(110, 23),
                    Font = new Font("Segoe UI", 9F)
                };
                cmbThumbSize.Items.AddRange(new object[] { "Kecil (240x135)", "Sedang (320x180)", "Besar (400x225)" });
                cmbThumbSize.SelectedIndex = 1; // Default Sedang
                cmbThumbSize.SelectedIndexChanged += (s, e) => UpdateThumbnailSizes();

                lblLiveScreenTotal = new Label
                {
                    Text = "Total PC Terpantau: 0",
                    Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(15, 118, 110),
                    Location = new Point(650, 14),
                    Size = new Size(220, 20),
                    TextAlign = ContentAlignment.MiddleRight,
                    Anchor = AnchorStyles.Top | AnchorStyles.Right
                };

                pnlLiveScreenToolbar.Controls.AddRange(new Control[] {
                    btnToggleLiveMonitor,
                    btnRefreshScreensOnce,
                    lblLiveScreenInterval,
                    cmbRefreshInterval,
                    lblLiveScreenSize,
                    cmbThumbSize,
                    lblLiveScreenTotal
                });

                // FlowLayoutPanel untuk Grid Kartu PC (Auto-scroll & Double Buffered)
                flpScreens = new DoubleBufferedFlowLayoutPanel
                {
                    AutoScroll = true,
                    BackColor = Color.FromArgb(226, 232, 240),
                    BorderStyle = BorderStyle.None,
                    Location = new Point(6, 58),
                    Size = new Size(tabLiveScreen.ClientSize.Width - 12, tabLiveScreen.ClientSize.Height - 64),
                    Padding = new Padding(10)
                };

                tabLiveScreen.Controls.Add(pnlLiveScreenToolbar);
                tabLiveScreen.Controls.Add(flpScreens);
                tabLiveScreen.Resize += (s, e) => AdjustTab7Layout();

                // Timer background refresh
                tmrScreenRefresh = new System.Windows.Forms.Timer
                {
                    Interval = 2000
                };
                tmrScreenRefresh.Tick += async (s, e) =>
                {
                    if (isScreenMonitoring && !isScreenRefreshing)
                    {
                        await RefreshAllClientScreensAsync();
                    }
                };

                tabMain.TabPages.Add(tabLiveScreen);
            }
            catch (Exception ex)
            {
                Log("BuildTabLiveScreen error: " + ex.Message);
            }
        }

        private void ToggleLiveScreenMonitoring()
        {
            isScreenMonitoring = !isScreenMonitoring;
            if (isScreenMonitoring)
            {
                btnToggleLiveMonitor.Text = "⏹️ Hentikan Pemantauan";
                btnToggleLiveMonitor.BackColor = Color.FromArgb(220, 38, 38);
                tmrScreenRefresh.Start();
                Log("Pemantauan layar client aktif (Auto-refresh realtime).");
                // Langsung trigger 1x refresh seketika
                _ = RefreshAllClientScreensAsync();
            }
            else
            {
                btnToggleLiveMonitor.Text = "▶️ Mulai Pantau Layar";
                btnToggleLiveMonitor.BackColor = Color.FromArgb(15, 118, 110);
                tmrScreenRefresh.Stop();
                Log("Pemantauan layar client dinonaktifkan.");
            }
        }

        private void AddOrUpdatePcScreenCard(string ip, string hostname, int pcNumber)
        {
            if (flpScreens == null) return;

            if (!pcScreenCards.ContainsKey(ip))
            {
                var card = new PcScreenCard(ip, hostname, pcNumber);
                card.CardDoubleClicked += (s, selectedIp) => ShowLargeScreenPreview(selectedIp);
                pcScreenCards[ip] = card;

                // Sort children by pcNumber agar berurutan rapi
                var sortedCards = pcScreenCards.Values.OrderBy(c => c.PcNumber).ToList();
                flpScreens.SuspendLayout();
                flpScreens.Controls.Clear();
                foreach (var sc in sortedCards)
                {
                    flpScreens.Controls.Add(sc.Container);
                }
                flpScreens.ResumeLayout();

                UpdateThumbnailSizes();
                if (lblLiveScreenTotal != null)
                {
                    lblLiveScreenTotal.Text = string.Format("Total PC Terpantau: {0}", pcScreenCards.Count);
                }
            }
            else
            {
                pcScreenCards[ip].UpdateInfo(hostname, pcNumber);
            }
        }

        private void ClearAllPcScreenCards()
        {
            if (flpScreens != null)
            {
                flpScreens.SuspendLayout();
                flpScreens.Controls.Clear();
                flpScreens.ResumeLayout();
            }
            foreach (var card in pcScreenCards.Values)
            {
                card.DisposeImage();
            }
            pcScreenCards.Clear();
            if (lblLiveScreenTotal != null)
            {
                lblLiveScreenTotal.Text = "Total PC Terpantau: 0";
            }
        }

        private void UpdateThumbnailSizes()
        {
            if (pcScreenCards == null || cmbThumbSize == null) return;

            int w = 320;
            int h = 180;
            if (cmbThumbSize.SelectedIndex == 0) { w = 240; h = 135; }
            else if (cmbThumbSize.SelectedIndex == 2) { w = 400; h = 225; }

            foreach (var card in pcScreenCards.Values)
            {
                card.SetCardSize(w, h);
            }
        }

        private async Task RefreshAllClientScreensAsync()
        {
            if (isScreenRefreshing || pcScreenCards.Count == 0) return;
            isScreenRefreshing = true;

            try
            {
                var ips = pcScreenCards.Keys.ToList();
                var throttle = new SemaphoreSlim(12);

                var tasks = ips.Select(async ip =>
                {
                    await throttle.WaitAsync();
                    try
                    {
                        var payload = new RemoteConfigPayload { Action = "CAPTURE_SCREEN" };
                        var resp = await scanner.SendRemoteConfigAsync(ip, payload, timeoutMs: 2500);

                        this.BeginInvoke((Action)(() =>
                        {
                            if (pcScreenCards.TryGetValue(ip, out var card))
                            {
                                if (resp.Success && !string.IsNullOrEmpty(resp.ScreenThumbnailBase64))
                                {
                                    if (resp.ClientScreenWidth > 0 && resp.ClientScreenHeight > 0)
                                    {
                                        card.ClientActualWidth = resp.ClientScreenWidth;
                                        card.ClientActualHeight = resp.ClientScreenHeight;
                                    }
                                    card.UpdateThumbnailFromBase64(resp.ScreenThumbnailBase64);
                                }
                                else
                                {
                                    card.SetOfflineOrError(resp.Message);
                                }
                            }
                        }));
                    }
                    catch { }
                    finally
                    {
                        throttle.Release();
                    }
                });

                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                Log("RefreshAllClientScreens error: " + ex.Message);
            }
            finally
            {
                isScreenRefreshing = false;
            }
        }

        private void ShowLargeScreenPreview(string ip)
        {
            if (!pcScreenCards.TryGetValue(ip, out var card)) return;

            var formPreview = new Form
            {
                Text = string.Format("🖥️ Remote Control & View - {0} ({1})", card.Hostname, ip),
                Size = new Size(1060, 680),
                StartPosition = FormStartPosition.CenterParent,
                BackColor = Color.FromArgb(15, 23, 42),
                MinimizeBox = false,
                KeyPreview = true
            };
            formPreview.Icon = this.Icon;

            var picLarge = new PictureBox
            {
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Black,
                Cursor = Cursors.Default
            };

            var pnlBottom = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 72,
                BackColor = Color.FromArgb(30, 41, 59),
                Padding = new Padding(8, 4, 8, 4)
            };

            // Baris 1: Kontrol streaming dan kendali
            var btnRefreshNow = new Button
            {
                Text = "🔄 Refresh 1x",
                Location = new Point(10, 6),
                Size = new Size(100, 28),
                BackColor = Color.FromArgb(15, 118, 110),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnRefreshNow.FlatAppearance.BorderSize = 0;

            var chkAutoStream = new CheckBox
            {
                Text = "▶️ Streaming Otomatis",
                Location = new Point(118, 9),
                Size = new Size(160, 22),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Checked = false // Default OFF agar tidak membebani jaringan/TCP saat baru dibuka
            };

            var lblSpeed = new Label
            {
                Text = "Speed:",
                Location = new Point(280, 10),
                Size = new Size(48, 20),
                ForeColor = Color.FromArgb(203, 213, 225),
                Font = new Font("Segoe UI", 8.5F)
            };

            var cmbStreamSpeed = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(330, 7),
                Size = new Size(125, 23),
                Font = new Font("Segoe UI", 8.5F)
            };
            cmbStreamSpeed.Items.AddRange(new object[] { "Tinggi (~4-5 FPS)", "Sedang (~2 FPS)", "Normal (1 FPS)", "Hemat (2s)" });
            cmbStreamSpeed.SelectedIndex = 1; // Default Sedang (500ms)

            var chkControlMode = new CheckBox
            {
                Text = "🎮 Kendali Penuh (Mouse & Keyboard)",
                Location = new Point(465, 9),
                Size = new Size(245, 22),
                ForeColor = Color.FromArgb(251, 191, 36), // Gold warning
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Checked = false
            };

            var btnSendWinKey = new Button
            {
                Text = "⊞ Kirim Win Key",
                Location = new Point(718, 6),
                Size = new Size(120, 28),
                BackColor = Color.FromArgb(71, 85, 105),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSendWinKey.FlatAppearance.BorderSize = 0;
            btnSendWinKey.Click += async (s, e) =>
            {
                var pDown = new RemoteConfigPayload { Action = "REMOTE_INPUT", InputEventType = "KEY_DOWN", InputKey = 0x5B }; // VK_LWIN
                await scanner.SendRemoteConfigAsync(ip, pDown, timeoutMs: 1500);
                await Task.Delay(50);
                var pUp = new RemoteConfigPayload { Action = "REMOTE_INPUT", InputEventType = "KEY_UP", InputKey = 0x5B };
                await scanner.SendRemoteConfigAsync(ip, pUp, timeoutMs: 1500);
            };

            // Baris 2: Status bar informatif yang tidak tumpang tindih
            var lblPreviewStatus = new Label
            {
                Text = "Mode: Hanya Lihat (View Only)",
                ForeColor = Color.FromArgb(148, 163, 184),
                Font = new Font("Segoe UI", 9F),
                Location = new Point(12, 42),
                Size = new Size(1020, 22),
                TextAlign = ContentAlignment.MiddleLeft,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            bool isFetching = false;
            Func<Task> doRefreshPreview = async () =>
            {
                if (isFetching || formPreview.IsDisposed) return;
                isFetching = true;
                try
                {
                    // Sesuaikan kualitas dan resolusi berdasarkan setting speed agar lancar
                    int q = 65;
                    int targetW = 1280;
                    int targetH = 720;
                    if (cmbStreamSpeed.SelectedIndex == 0) // Fast 4-5 FPS
                    {
                        q = 50;
                        targetW = 1024;
                        targetH = 576;
                    }

                    var payload = new RemoteConfigPayload 
                    { 
                        Action = "CAPTURE_SCREEN",
                        TargetWidth = targetW,
                        TargetHeight = targetH,
                        ScreenQuality = q
                    };
                    var resp = await scanner.SendRemoteConfigAsync(ip, payload, timeoutMs: 2500);
                    if (resp.Success && !string.IsNullOrEmpty(resp.ScreenThumbnailBase64) && !formPreview.IsDisposed)
                    {
                        if (resp.ClientScreenWidth > 0 && resp.ClientScreenHeight > 0)
                        {
                            card.ClientActualWidth = resp.ClientScreenWidth;
                            card.ClientActualHeight = resp.ClientScreenHeight;
                        }

                        byte[] bytes = Convert.FromBase64String(resp.ScreenThumbnailBase64);
                        using (var ms = new MemoryStream(bytes))
                        {
                            var newBmp = new Bitmap(ms);
                            var old = picLarge.Image;
                            picLarge.Image = newBmp;
                            old?.Dispose();
                        }
                        if (!formPreview.IsDisposed)
                        {
                            lblPreviewStatus.Text = string.Format("Status: {0} | Resolusi Asli Client: {1}x{2} | Kecepatan: {3} | Update Terakhir: {4}", 
                                chkControlMode.Checked ? "🎮 KENDALI PENUH AKTIF" : "👀 Mode Pantau (Hanya Lihat)",
                                card.ClientActualWidth, card.ClientActualHeight,
                                cmbStreamSpeed.SelectedItem != null ? cmbStreamSpeed.SelectedItem.ToString() : "Sedang",
                                DateTime.Now.ToString("HH:mm:ss"));
                        }
                    }
                }
                catch { }
                finally
                {
                    isFetching = false;
                }
            };

            var streamTimer = new System.Windows.Forms.Timer { Interval = 500 };
            streamTimer.Tick += async (s, e) =>
            {
                if (chkAutoStream.Checked)
                {
                    await doRefreshPreview();
                }
            };

            cmbStreamSpeed.SelectedIndexChanged += (s, e) =>
            {
                int intervalMs = 500;
                switch (cmbStreamSpeed.SelectedIndex)
                {
                    case 0: intervalMs = 220; break; // ~4-5 FPS
                    case 1: intervalMs = 500; break; // ~2 FPS
                    case 2: intervalMs = 1000; break; // 1 FPS
                    case 3: intervalMs = 2000; break; // Hemat 2s
                }
                streamTimer.Interval = intervalMs;
            };

            chkAutoStream.CheckedChanged += (s, e) =>
            {
                if (chkAutoStream.Checked) streamTimer.Start();
                else streamTimer.Stop();
            };

            chkControlMode.CheckedChanged += (s, e) =>
            {
                picLarge.Cursor = chkControlMode.Checked ? Cursors.Cross : Cursors.Default;
                lblPreviewStatus.Text = string.Format("Status: {0} | Resolusi Asli Client: {1}x{2}", 
                    chkControlMode.Checked ? "🎮 KENDALI PENUH AKTIF" : "👀 Mode Pantau (Hanya Lihat)",
                    card.ClientActualWidth, card.ClientActualHeight);
                lblPreviewStatus.ForeColor = chkControlMode.Checked ? Color.FromArgb(74, 222, 128) : Color.FromArgb(148, 163, 184);

                if (chkControlMode.Checked)
                {
                    int[] keysToRelease = new int[] { 0x5B, 0x5C, 0x10, 0x11, 0x12 }; // LWIN, RWIN, SHIFT, CTRL, ALT
                    foreach (var k in keysToRelease)
                    {
                        var pRelease = new RemoteConfigPayload { Action = "REMOTE_INPUT", InputEventType = "KEY_UP", InputKey = k };
                        _ = scanner.SendRemoteConfigAsync(ip, pRelease, timeoutMs: 1000);
                    }
                }
            };

            btnRefreshNow.Click += async (s, e) => await doRefreshPreview();

            // Koordinat mapping helper dari picturebox zoom mode ke client actual resolution
            Func<Point, Point?> getClientCoordinates = (Point clickPoint) =>
            {
                if (picLarge.Image == null) return null;

                int imgW = picLarge.Image.Width;
                int imgH = picLarge.Image.Height;
                int boxW = picLarge.ClientSize.Width;
                int boxH = picLarge.ClientSize.Height;

                if (imgW <= 0 || imgH <= 0 || boxW <= 0 || boxH <= 0) return null;

                float scale = Math.Min((float)boxW / imgW, (float)boxH / imgH);
                float displayedW = imgW * scale;
                float displayedH = imgH * scale;

                float offsetX = (boxW - displayedW) / 2f;
                float offsetY = (boxH - displayedH) / 2f;

                if (clickPoint.X < offsetX || clickPoint.X > offsetX + displayedW ||
                    clickPoint.Y < offsetY || clickPoint.Y > offsetY + displayedH)
                {
                    return null;
                }

                // Normalisasi posisi klik 0.0 - 1.0 terhadap area gambar yang sedang ditampilkan
                float normX = (clickPoint.X - offsetX) / displayedW;
                float normY = (clickPoint.Y - offsetY) / displayedH;

                normX = Math.Max(0f, Math.Min(1f, normX));
                normY = Math.Max(0f, Math.Min(1f, normY));

                int actualW = card.ClientActualWidth > 0 ? card.ClientActualWidth : 1920;
                int actualH = card.ClientActualHeight > 0 ? card.ClientActualHeight : 1080;

                int targetX = (int)Math.Round(normX * actualW);
                int targetY = (int)Math.Round(normY * actualH);

                return new Point(targetX, targetY);
            };

            // Event Mouse Down & Up
            picLarge.MouseDown += (s, e) =>
            {
                formPreview.Focus();
                if (!chkControlMode.Checked) return;
                var pt = getClientCoordinates(e.Location);
                if (pt.HasValue)
                {
                    string btn = "Left";
                    if (e.Button == MouseButtons.Right) btn = "Right";
                    else if (e.Button == MouseButtons.Middle) btn = "Middle";

                    var payload = new RemoteConfigPayload
                    {
                        Action = "REMOTE_INPUT",
                        InputEventType = "MOUSE_DOWN",
                        InputX = pt.Value.X,
                        InputY = pt.Value.Y,
                        InputButton = btn
                    };
                    _ = scanner.SendRemoteConfigAsync(ip, payload, timeoutMs: 1500);
                }
            };

            picLarge.MouseUp += (s, e) =>
            {
                if (!chkControlMode.Checked) return;
                var pt = getClientCoordinates(e.Location);
                if (pt.HasValue)
                {
                    string btn = "Left";
                    if (e.Button == MouseButtons.Right) btn = "Right";
                    else if (e.Button == MouseButtons.Middle) btn = "Middle";

                    var payload = new RemoteConfigPayload
                    {
                        Action = "REMOTE_INPUT",
                        InputEventType = "MOUSE_UP",
                        InputX = pt.Value.X,
                        InputY = pt.Value.Y,
                        InputButton = btn
                    };
                    _ = scanner.SendRemoteConfigAsync(ip, payload, timeoutMs: 1500);
                }
            };

            // Event Mouse Wheel (Scroll)
            picLarge.MouseWheel += (s, e) =>
            {
                if (!chkControlMode.Checked) return;
                var pt = getClientCoordinates(e.Location);
                int coordX = pt.HasValue ? pt.Value.X : 0;
                int coordY = pt.HasValue ? pt.Value.Y : 0;

                var payload = new RemoteConfigPayload
                {
                    Action = "REMOTE_INPUT",
                    InputEventType = "MOUSE_WHEEL",
                    InputX = coordX,
                    InputY = coordY,
                    InputWheelDelta = e.Delta
                };
                _ = scanner.SendRemoteConfigAsync(ip, payload, timeoutMs: 1500);
            };

            // Event Keyboard: Langsung tangkap seluruh tombol keyboard (huruf, angka, navigasi, modifier)
            Action<PreviewKeyDownEventArgs> handlePreviewKey = (pk) =>
            {
                if (!chkControlMode.Checked) return;
                // Cegah WinForms menelan tombol dialog (Enter, Return, Tab, Arrows, Escape)
                pk.IsInputKey = true;
            };

            formPreview.PreviewKeyDown += (s, e) => handlePreviewKey(e);
            picLarge.PreviewKeyDown += (s, e) => handlePreviewKey(e);

            Action<KeyEventArgs> handleKeyDown = (e) =>
            {
                if (!chkControlMode.Checked) return;

                int vkCode = (int)e.KeyCode;
                if (vkCode > 0)
                {
                    var payload = new RemoteConfigPayload
                    {
                        Action = "REMOTE_INPUT",
                        InputEventType = "KEY_DOWN",
                        InputKey = vkCode
                    };
                    _ = scanner.SendRemoteConfigAsync(ip, payload, timeoutMs: 1500);
                }
                e.Handled = true;
                e.SuppressKeyPress = true;
            };

            Action<KeyEventArgs> handleKeyUp = (e) =>
            {
                if (!chkControlMode.Checked) return;

                int vkCode = (int)e.KeyCode;
                if (vkCode > 0)
                {
                    var payload = new RemoteConfigPayload
                    {
                        Action = "REMOTE_INPUT",
                        InputEventType = "KEY_UP",
                        InputKey = vkCode
                    };
                    _ = scanner.SendRemoteConfigAsync(ip, payload, timeoutMs: 1500);
                }
                e.Handled = true;
                e.SuppressKeyPress = true;
            };

            formPreview.KeyDown += (s, e) => handleKeyDown(e);
            formPreview.KeyUp += (s, e) => handleKeyUp(e);
            picLarge.KeyDown += (s, e) => handleKeyDown(e);
            picLarge.KeyUp += (s, e) => handleKeyUp(e);

            formPreview.FormClosing += (s, e) =>
            {
                streamTimer.Stop();
                streamTimer.Dispose();

                // Pastikan key modifier di client bersih saat form ditutup
                int[] keysToRelease = new int[] { 0x5B, 0x5C, 0x10, 0x11, 0x12 }; // LWIN, RWIN, SHIFT, CTRL, ALT
                foreach (var k in keysToRelease)
                {
                    var pRelease = new RemoteConfigPayload { Action = "REMOTE_INPUT", InputEventType = "KEY_UP", InputKey = k };
                    _ = scanner.SendRemoteConfigAsync(ip, pRelease, timeoutMs: 1000);
                }
            };

            pnlBottom.Controls.AddRange(new Control[] {
                btnRefreshNow,
                chkAutoStream,
                lblSpeed,
                cmbStreamSpeed,
                chkControlMode,
                btnSendWinKey,
                lblPreviewStatus
            });
            formPreview.Controls.Add(picLarge);
            formPreview.Controls.Add(pnlBottom);

            if (card.CurrentBitmap != null)
            {
                picLarge.Image = (Image)card.CurrentBitmap.Clone();
            }

            formPreview.Shown += async (s, e) =>
            {
                await doRefreshPreview();
                if (chkAutoStream.Checked) streamTimer.Start();
            };
            formPreview.ShowDialog(this);
        }
        #endregion

        #region Helper Classes: DoubleBufferedFlowLayoutPanel & PcScreenCard
        public class DoubleBufferedFlowLayoutPanel : FlowLayoutPanel
        {
            public DoubleBufferedFlowLayoutPanel()
            {
                DoubleBuffered = true;
                SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
                UpdateStyles();
            }
        }

        public class PcScreenCard
        {
            public string Ip { get; private set; }
            public string Hostname { get; private set; }
            public int PcNumber { get; private set; }
            public Panel Container { get; private set; }
            public PictureBox Picture { get; private set; }
            public Label LblHeader { get; private set; }
            public Label LblStatus { get; private set; }
            public Bitmap CurrentBitmap { get; private set; }
            public int ClientActualWidth { get; set; }
            public int ClientActualHeight { get; set; }

            public event EventHandler<string> CardDoubleClicked;

            public PcScreenCard(string ip, string hostname, int pcNumber)
            {
                Ip = ip;
                Hostname = hostname;
                PcNumber = pcNumber;
                ClientActualWidth = 1920;
                ClientActualHeight = 1080;

                Container = new Panel
                {
                    BackColor = Color.FromArgb(30, 41, 59),
                    Margin = new Padding(6),
                    BorderStyle = BorderStyle.FixedSingle,
                    Cursor = Cursors.Hand
                };

                LblHeader = new Label
                {
                    Dock = DockStyle.Top,
                    Height = 26,
                    BackColor = Color.FromArgb(15, 118, 110),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                    TextAlign = ContentAlignment.MiddleLeft,
                    Padding = new Padding(6, 0, 0, 0),
                    Text = string.Format("#{0:D2} {1} ({2})", pcNumber, hostname, ip),
                    Cursor = Cursors.Hand
                };

                LblStatus = new Label
                {
                    Dock = DockStyle.Bottom,
                    Height = 22,
                    BackColor = Color.FromArgb(15, 23, 42),
                    ForeColor = Color.FromArgb(148, 163, 184),
                    Font = new Font("Segoe UI", 8F),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Text = "Menunggu cuplikan layar...",
                    Cursor = Cursors.Hand
                };

                Picture = new PictureBox
                {
                    Dock = DockStyle.Fill,
                    SizeMode = PictureBoxSizeMode.Zoom,
                    BackColor = Color.FromArgb(15, 23, 42),
                    Cursor = Cursors.Hand
                };

                Container.Controls.Add(Picture);
                Container.Controls.Add(LblHeader);
                Container.Controls.Add(LblStatus);

                // Wire click events
                Action onDblClick = () => CardDoubleClicked?.Invoke(this, Ip);
                Container.DoubleClick += (s, e) => onDblClick();
                Picture.DoubleClick += (s, e) => onDblClick();
                LblHeader.DoubleClick += (s, e) => onDblClick();
                LblStatus.DoubleClick += (s, e) => onDblClick();

                SetCardSize(320, 180);
            }

            public void SetCardSize(int thumbW, int thumbH)
            {
                Container.Size = new Size(thumbW + 2, thumbH + 50);
            }

            public void UpdateInfo(string hostname, int pcNumber)
            {
                Hostname = hostname;
                PcNumber = pcNumber;
                LblHeader.Text = string.Format("#{0:D2} {1} ({2})", pcNumber, hostname, Ip);
            }

            public void UpdateThumbnailFromBase64(string base64)
            {
                try
                {
                    byte[] bytes = Convert.FromBase64String(base64);
                    using (var ms = new MemoryStream(bytes))
                    {
                        var newBmp = new Bitmap(ms);
                        var old = CurrentBitmap;
                        CurrentBitmap = newBmp;
                        Picture.Image = CurrentBitmap;
                        old?.Dispose();
                    }
                    LblStatus.Text = string.Format("Live: {0}", DateTime.Now.ToString("HH:mm:ss"));
                    LblStatus.ForeColor = Color.FromArgb(34, 197, 94); // Hijau terang
                }
                catch (Exception ex)
                {
                    LblStatus.Text = "Format rusak: " + ex.Message;
                    LblStatus.ForeColor = Color.FromArgb(239, 68, 68);
                }
            }

            public void SetOfflineOrError(string msg)
            {
                LblStatus.Text = string.IsNullOrEmpty(msg) ? "Tidak merespon" : msg;
                LblStatus.ForeColor = Color.FromArgb(239, 68, 68);
            }

            public void DisposeImage()
            {
                Picture.Image = null;
                CurrentBitmap?.Dispose();
                CurrentBitmap = null;
            }
        }
        #endregion
        #endregion
    }
}

