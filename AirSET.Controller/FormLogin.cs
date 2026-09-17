using System;
using System.Drawing;
using System.Windows.Forms;

namespace AirSET.Controller
{
    public partial class FormLogin : Form
    {
        private bool isFirstRun;

        public FormLogin()
        {
            InitializeComponent();

            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            }
            catch { }

            isFirstRun = !PasswordAuthManager.IsPasswordSet();

            if (isFirstRun)
            {
                lblTitle.Text = "Buat Password Controller";
                lblSubtitle.Text = "Aplikasi pertama kali dijalankan. Tentukan password baru untuk mengamankan Controller:";
                lblConfirm.Visible = true;
                txtConfirm.Visible = true;
                btnLogin.Text = "Simpan & Lanjutkan";
            }
            else
            {
                lblTitle.Text = "Login AirSET Controller";
                lblSubtitle.Text = "Masukkan password pengamanan Controller untuk melanjutkan:";
                lblConfirm.Visible = false;
                txtConfirm.Visible = false;
                btnLogin.Text = "Masuk";
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string password = txtPassword.Text.Trim();

            if (isFirstRun)
            {
                string confirm = txtConfirm.Text.Trim();
                if (string.IsNullOrEmpty(password))
                {
                    MessageBox.Show("Password tidak boleh kosong!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (password.Length < 4)
                {
                    MessageBox.Show("Password minimal 4 karakter!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (password != confirm)
                {
                    MessageBox.Show("Konfirmasi password tidak cocok!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                PasswordAuthManager.SetPassword(password);
                MessageBox.Show("Password Controller berhasil dibuat dan disimpan!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                if (PasswordAuthManager.VerifyPassword(password))
                {
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Password salah! Silakan coba lagi.", "Akses Ditolak", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtPassword.Clear();
                    txtPassword.Focus();
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}