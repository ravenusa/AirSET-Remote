using System;
using System.Drawing;
using System.Windows.Forms;

namespace AirSET.Controller
{
    public partial class FormChangePassword : Form
    {
        public FormChangePassword()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string oldPass = txtOldPassword.Text.Trim();
            string newPass = txtNewPassword.Text.Trim();
            string confirmPass = txtConfirmPassword.Text.Trim();

            if (string.IsNullOrEmpty(oldPass))
            {
                MessageBox.Show("Silakan masukkan password lama Anda!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtOldPassword.Focus();
                return;
            }

            if (!PasswordAuthManager.VerifyPassword(oldPass))
            {
                MessageBox.Show("Password lama salah! Silakan periksa kembali.", "Verifikasi Gagal", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtOldPassword.Clear();
                txtOldPassword.Focus();
                return;
            }

            if (string.IsNullOrEmpty(newPass))
            {
                MessageBox.Show("Password baru tidak boleh kosong!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNewPassword.Focus();
                return;
            }

            if (newPass.Length < 4)
            {
                MessageBox.Show("Password baru minimal 4 karakter!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNewPassword.Focus();
                return;
            }

            if (newPass != confirmPass)
            {
                MessageBox.Show("Konfirmasi password baru tidak cocok!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtConfirmPassword.Clear();
                txtConfirmPassword.Focus();
                return;
            }

            PasswordAuthManager.SetPassword(newPass);
            MessageBox.Show("Password Controller berhasil diperbarui!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
