using System;
using System.Drawing;
using System.Windows.Forms;

namespace devIPsett
{
    public partial class FormAddLab : Form
    {
        public LabProfile CreatedProfile { get; private set; }
        private bool isEditMode = false;
        private string originalCode = string.Empty;

        public FormAddLab()
        {
            InitializeComponent();
            ApplyModernStyling();
        }

        public FormAddLab(LabProfile profileToEdit) : this()
        {
            if (profileToEdit != null)
            {
                isEditMode = true;
                originalCode = profileToEdit.Code;
                CreatedProfile = profileToEdit;
                LoadProfileData();
            }
        }

        private void LoadProfileData()
        {
            if (CreatedProfile != null)
            {
                this.Text = "Edit Profil LAB Custom";
                txtCode.Text = CreatedProfile.Code;
                txtCode.ReadOnly = true;
                txtName.Text = CreatedProfile.Name;
                txtBaseIp.Text = CreatedProfile.BaseIp;
                txtSubnet.Text = CreatedProfile.Subnet;
                txtGateway.Text = CreatedProfile.Gateway;
                txtDns.Text = CreatedProfile.Dns;
                txtWorkgroup.Text = CreatedProfile.Workgroup;
                txtPrefix.Text = CreatedProfile.HostnamePrefix;
                btnSave.Text = "Update";
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            string code = txtCode.Text.Trim();
            string name = txtName.Text.Trim();
            string baseIp = txtBaseIp.Text.Trim();
            string subnet = txtSubnet.Text.Trim();
            string gateway = txtGateway.Text.Trim();
            string dns = txtDns.Text.Trim();
            string workgroup = txtWorkgroup.Text.Trim();
            string prefix = txtPrefix.Text.Trim();

            if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(baseIp) || string.IsNullOrEmpty(subnet) || string.IsNullOrEmpty(gateway))
            {
                MessageBox.Show("Mohon lengkapi seluruh field!", "Data Belum Lengkap", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 1. Cek apakah Nama / Kode yang diinputkan bertabrakan dengan 20 LAB Bawaan Sistem
            if (!isEditMode && LabData.IsDefaultProfile(code, name))
            {
                MessageBox.Show(
                    string.Format("Ditolak! Kode '{0}' atau Nama '{1}' merupakan bagian dari 20 LAB Aktiv Saat ini yang tidak boleh diubah!.", code, name),
                    "Nama / Kode LAB Dilindungi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }

            // 2. Cek apakah bertabrakan dengan profil Custom JSON yang sudah ada
            if (!isEditMode)
            {
                LabProfile existingCustom = LabData.FindExistingProfile(code, name);
                if (existingCustom != null)
                {
                    DialogResult confirm = MessageBox.Show(
                        string.Format("Nama atau Kode LAB sudah ada di database ('{0}' - {1}).\n\nApakah Anda ingin menimpa (overwrite) data LAB lama tersebut dengan data baru ini?", existingCustom.Name, existingCustom.Code),
                        "Konfirmasi Overwrite LAB",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question
                    );

                    if (confirm != DialogResult.Yes)
                    {
                        return; // Batal simpan jika user memilih No
                    }
                }
            }

            CreatedProfile = new LabProfile
            {
                Code = code,
                Name = name,
                BaseIp = baseIp,
                Subnet = subnet,
                Gateway = gateway,
                Dns = string.IsNullOrEmpty(dns) ? "10.1.1.111" : dns,
                Workgroup = workgroup,
                HostnamePrefix = string.IsNullOrEmpty(prefix) ? "Komputer-" : prefix
            };

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void ApplyModernStyling()
        {
            this.BackColor = Color.FromArgb(241, 245, 249);
            this.ForeColor = Color.FromArgb(30, 41, 59);

            SetupButtonHover(btnSave, Color.FromArgb(15, 118, 110), Color.FromArgb(13, 148, 136), Color.White, btnSave.Text, 14);
            SetupButtonHover(btnCancel, Color.FromArgb(226, 232, 240), Color.FromArgb(203, 213, 225), Color.FromArgb(51, 65, 85), "Batal", 14);

            ApplyRoundedRegionToControl(txtCode, 8);
            ApplyRoundedRegionToControl(txtName, 8);
            ApplyRoundedRegionToControl(txtBaseIp, 8);
            ApplyRoundedRegionToControl(txtSubnet, 8);
            ApplyRoundedRegionToControl(txtGateway, 8);
            ApplyRoundedRegionToControl(txtDns, 8);
            ApplyRoundedRegionToControl(txtWorkgroup, 8);
            ApplyRoundedRegionToControl(txtPrefix, 8);
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
    }
}
