using System.Drawing;

namespace devIPsett
{
    partial class FormAddLab
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.lblCode = new System.Windows.Forms.Label();
            this.txtCode = new System.Windows.Forms.TextBox();
            this.lblName = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.lblBaseIp = new System.Windows.Forms.Label();
            this.txtBaseIp = new System.Windows.Forms.TextBox();
            this.lblSubnet = new System.Windows.Forms.Label();
            this.txtSubnet = new System.Windows.Forms.TextBox();
            this.lblGateway = new System.Windows.Forms.Label();
            this.txtGateway = new System.Windows.Forms.TextBox();
            this.lblDns = new System.Windows.Forms.Label();
            this.txtDns = new System.Windows.Forms.TextBox();
            this.lblWorkgroup = new System.Windows.Forms.Label();
            this.txtWorkgroup = new System.Windows.Forms.TextBox();
            this.lblPrefix = new System.Windows.Forms.Label();
            this.txtPrefix = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblCode
            // 
            this.lblCode.AutoSize = true;
            this.lblCode.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblCode.Location = new System.Drawing.Point(20, 20);
            this.lblCode.Name = "lblCode";
            this.lblCode.Size = new System.Drawing.Size(133, 15);
            this.lblCode.TabIndex = 0;
            this.lblCode.Text = "Kode LAB (misal L811):";
            // 
            // txtCode
            // 
            this.txtCode.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.txtCode.Location = new System.Drawing.Point(180, 17);
            this.txtCode.Name = "txtCode";
            this.txtCode.Size = new System.Drawing.Size(220, 25);
            this.txtCode.TabIndex = 1;
            this.txtCode.Text = "L811";
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblName.Location = new System.Drawing.Point(20, 56);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(154, 15);
            this.lblName.TabIndex = 2;
            this.lblName.Text = "Nama LAB (misal Lab 8.1.1):";
            // 
            // txtName
            // 
            this.txtName.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.txtName.Location = new System.Drawing.Point(180, 53);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(220, 25);
            this.txtName.TabIndex = 3;
            this.txtName.Text = "Lab 8.1.1";
            // 
            // lblBaseIp
            // 
            this.lblBaseIp.AutoSize = true;
            this.lblBaseIp.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblBaseIp.Location = new System.Drawing.Point(20, 92);
            this.lblBaseIp.Name = "lblBaseIp";
            this.lblBaseIp.Size = new System.Drawing.Size(82, 15);
            this.lblBaseIp.TabIndex = 4;
            this.lblBaseIp.Text = "BASE IP Prefix:";
            // 
            // txtBaseIp
            // 
            this.txtBaseIp.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.txtBaseIp.Location = new System.Drawing.Point(180, 89);
            this.txtBaseIp.Name = "txtBaseIp";
            this.txtBaseIp.Size = new System.Drawing.Size(220, 25);
            this.txtBaseIp.TabIndex = 5;
            this.txtBaseIp.Text = "10.81.1";
            // 
            // lblSubnet
            // 
            this.lblSubnet.AutoSize = true;
            this.lblSubnet.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblSubnet.Location = new System.Drawing.Point(20, 128);
            this.lblSubnet.Name = "lblSubnet";
            this.lblSubnet.Size = new System.Drawing.Size(78, 15);
            this.lblSubnet.TabIndex = 6;
            this.lblSubnet.Text = "Subnet Mask:";
            // 
            // txtSubnet
            // 
            this.txtSubnet.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.txtSubnet.Location = new System.Drawing.Point(180, 125);
            this.txtSubnet.Name = "txtSubnet";
            this.txtSubnet.Size = new System.Drawing.Size(220, 25);
            this.txtSubnet.TabIndex = 7;
            this.txtSubnet.Text = "255.255.255.0";
            // 
            // lblGateway
            // 
            this.lblGateway.AutoSize = true;
            this.lblGateway.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblGateway.Location = new System.Drawing.Point(20, 164);
            this.lblGateway.Name = "lblGateway";
            this.lblGateway.Size = new System.Drawing.Size(55, 15);
            this.lblGateway.TabIndex = 8;
            this.lblGateway.Text = "Gateway:";
            // 
            // txtGateway
            // 
            this.txtGateway.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.txtGateway.Location = new System.Drawing.Point(180, 161);
            this.txtGateway.Name = "txtGateway";
            this.txtGateway.Size = new System.Drawing.Size(220, 25);
            this.txtGateway.TabIndex = 9;
            this.txtGateway.Text = "10.81.1.254";
            // 
            // lblDns
            // 
            this.lblDns.AutoSize = true;
            this.lblDns.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblDns.Location = new System.Drawing.Point(20, 200);
            this.lblDns.Name = "lblDns";
            this.lblDns.Size = new System.Drawing.Size(68, 15);
            this.lblDns.TabIndex = 10;
            this.lblDns.Text = "DNS Server:";
            // 
            // txtDns
            // 
            this.txtDns.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.txtDns.Location = new System.Drawing.Point(180, 197);
            this.txtDns.Name = "txtDns";
            this.txtDns.Size = new System.Drawing.Size(220, 25);
            this.txtDns.TabIndex = 11;
            this.txtDns.Text = "10.1.1.111";
            // 
            // lblWorkgroup
            // 
            this.lblWorkgroup.AutoSize = true;
            this.lblWorkgroup.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblWorkgroup.Location = new System.Drawing.Point(20, 236);
            this.lblWorkgroup.Name = "lblWorkgroup";
            this.lblWorkgroup.Size = new System.Drawing.Size(70, 15);
            this.lblWorkgroup.TabIndex = 12;
            this.lblWorkgroup.Text = "Workgroup:";
            // 
            // txtWorkgroup
            // 
            this.txtWorkgroup.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.txtWorkgroup.Location = new System.Drawing.Point(180, 233);
            this.txtWorkgroup.Name = "txtWorkgroup";
            this.txtWorkgroup.Size = new System.Drawing.Size(220, 25);
            this.txtWorkgroup.TabIndex = 13;
            this.txtWorkgroup.Text = "Lab-8.1.1";
            // 
            // lblPrefix
            // 
            this.lblPrefix.AutoSize = true;
            this.lblPrefix.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblPrefix.Location = new System.Drawing.Point(20, 272);
            this.lblPrefix.Name = "lblPrefix";
            this.lblPrefix.Size = new System.Drawing.Size(97, 15);
            this.lblPrefix.TabIndex = 14;
            this.lblPrefix.Text = "Prefix Hostname:";
            // 
            // txtPrefix
            // 
            this.txtPrefix.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.txtPrefix.Location = new System.Drawing.Point(180, 269);
            this.txtPrefix.Name = "txtPrefix";
            this.txtPrefix.Size = new System.Drawing.Size(220, 25);
            this.txtPrefix.TabIndex = 15;
            this.txtPrefix.Text = "Komputer-";
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.Color.LightBlue;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnSave.ForeColor = System.Drawing.Color.Black;
            this.btnSave.Location = new System.Drawing.Point(180, 318);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(105, 32);
            this.btnSave.TabIndex = 16;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.BtnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(295, 318);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(105, 32);
            this.btnCancel.TabIndex = 17;
            this.btnCancel.Text = "Batal";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // FormAddLab
            // 
            this.ClientSize = new System.Drawing.Size(424, 371);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.txtPrefix);
            this.Controls.Add(this.lblPrefix);
            this.Controls.Add(this.txtWorkgroup);
            this.Controls.Add(this.lblWorkgroup);
            this.Controls.Add(this.txtDns);
            this.Controls.Add(this.lblDns);
            this.Controls.Add(this.txtGateway);
            this.Controls.Add(this.lblGateway);
            this.Controls.Add(this.txtSubnet);
            this.Controls.Add(this.lblSubnet);
            this.Controls.Add(this.txtBaseIp);
            this.Controls.Add(this.lblBaseIp);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.txtCode);
            this.Controls.Add(this.lblCode);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormAddLab";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Tambah Profil LAB Baru";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblCode;
        private System.Windows.Forms.TextBox txtCode;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label lblBaseIp;
        private System.Windows.Forms.TextBox txtBaseIp;
        private System.Windows.Forms.Label lblSubnet;
        private System.Windows.Forms.TextBox txtSubnet;
        private System.Windows.Forms.Label lblGateway;
        private System.Windows.Forms.TextBox txtGateway;
        private System.Windows.Forms.Label lblDns;
        private System.Windows.Forms.TextBox txtDns;
        private System.Windows.Forms.Label lblWorkgroup;
        private System.Windows.Forms.TextBox txtWorkgroup;
        private System.Windows.Forms.Label lblPrefix;
        private System.Windows.Forms.TextBox txtPrefix;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
    }
}
