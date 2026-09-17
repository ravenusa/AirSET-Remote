namespace devIPsett
{
    partial class FormMain
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
            this.grpConfig = new System.Windows.Forms.GroupBox();
            this.chkDisableAutoLogon = new System.Windows.Forms.CheckBox();
            this.lblActiveUserVal = new System.Windows.Forms.Label();
            this.lblActiveUserLbl = new System.Windows.Forms.Label();
            this.chkAutoLogon = new System.Windows.Forms.CheckBox();
            this.btnToggleShowPassword = new System.Windows.Forms.Button();
            this.txtNewPassword = new System.Windows.Forms.TextBox();
            this.lblNewPassword = new System.Windows.Forms.Label();
            this.chkChangePassword = new System.Windows.Forms.CheckBox();
            this.lblLabSelect = new System.Windows.Forms.Label();
            this.cmbLabSelect = new System.Windows.Forms.ComboBox();
            this.btnAddLab = new System.Windows.Forms.Button();
            this.btnEditLab = new System.Windows.Forms.Button();
            this.btnDeleteLab = new System.Windows.Forms.Button();
            this.btnRefreshIf = new System.Windows.Forms.Button();
            this.lblBaseIp = new System.Windows.Forms.Label();
            this.txtBaseIp = new System.Windows.Forms.TextBox();
            this.lblSubnet = new System.Windows.Forms.Label();
            this.txtSubnet = new System.Windows.Forms.TextBox();
            this.lblDns = new System.Windows.Forms.Label();
            this.txtDns = new System.Windows.Forms.TextBox();
            this.lblPrefix = new System.Windows.Forms.Label();
            this.txtPrefix = new System.Windows.Forms.TextBox();
            this.lblGateway = new System.Windows.Forms.Label();
            this.txtGateway = new System.Windows.Forms.TextBox();
            this.lblWorkgroup = new System.Windows.Forms.Label();
            this.txtWorkgroup = new System.Windows.Forms.TextBox();
            this.lblInterface = new System.Windows.Forms.Label();
            this.cmbInterface = new System.Windows.Forms.ComboBox();
            this.grpAction = new System.Windows.Forms.GroupBox();
            this.lblCurrentHostname = new System.Windows.Forms.Label();
            this.lblPreviewHostname = new System.Windows.Forms.Label();
            this.lblPreviewIp = new System.Windows.Forms.Label();
            this.btnApply = new System.Windows.Forms.Button();
            this.txtPcNum = new System.Windows.Forms.TextBox();
            this.lblPcNum = new System.Windows.Forms.Label();
            this.grpLog = new System.Windows.Forms.GroupBox();
            this.txtLog = new System.Windows.Forms.RichTextBox();
            this.lblCopyright = new System.Windows.Forms.Label();
            this.grpConfig.SuspendLayout();
            this.grpAction.SuspendLayout();
            this.grpLog.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpConfig
            // 
            this.grpConfig.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.grpConfig.Controls.Add(this.lblCurrentHostname);
            this.grpConfig.Controls.Add(this.chkDisableAutoLogon);
            this.grpConfig.Controls.Add(this.lblActiveUserVal);
            this.grpConfig.Controls.Add(this.lblActiveUserLbl);
            this.grpConfig.Controls.Add(this.chkAutoLogon);
            this.grpConfig.Controls.Add(this.btnToggleShowPassword);
            this.grpConfig.Controls.Add(this.txtNewPassword);
            this.grpConfig.Controls.Add(this.lblNewPassword);
            this.grpConfig.Controls.Add(this.chkChangePassword);
            this.grpConfig.Controls.Add(this.lblLabSelect);
            this.grpConfig.Controls.Add(this.cmbLabSelect);
            this.grpConfig.Controls.Add(this.btnAddLab);
            this.grpConfig.Controls.Add(this.btnEditLab);
            this.grpConfig.Controls.Add(this.btnDeleteLab);
            this.grpConfig.Controls.Add(this.btnRefreshIf);
            this.grpConfig.Controls.Add(this.lblBaseIp);
            this.grpConfig.Controls.Add(this.txtBaseIp);
            this.grpConfig.Controls.Add(this.lblSubnet);
            this.grpConfig.Controls.Add(this.txtSubnet);
            this.grpConfig.Controls.Add(this.lblDns);
            this.grpConfig.Controls.Add(this.txtDns);
            this.grpConfig.Controls.Add(this.lblPrefix);
            this.grpConfig.Controls.Add(this.txtPrefix);
            this.grpConfig.Controls.Add(this.lblGateway);
            this.grpConfig.Controls.Add(this.txtGateway);
            this.grpConfig.Controls.Add(this.lblWorkgroup);
            this.grpConfig.Controls.Add(this.txtWorkgroup);
            this.grpConfig.Controls.Add(this.lblInterface);
            this.grpConfig.Controls.Add(this.cmbInterface);
            this.grpConfig.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.grpConfig.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.grpConfig.ImeMode = System.Windows.Forms.ImeMode.Hiragana;
            this.grpConfig.Location = new System.Drawing.Point(14, 12);
            this.grpConfig.Name = "grpConfig";
            this.grpConfig.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.grpConfig.Size = new System.Drawing.Size(559, 379);
            this.grpConfig.TabIndex = 0;
            this.grpConfig.TabStop = false;
            this.grpConfig.Text = "Basic Configuration";
            this.grpConfig.Enter += new System.EventHandler(this.grpConfig_Enter);
            // 
            // chkDisableAutoLogon
            // 
            this.chkDisableAutoLogon.AutoSize = true;
            this.chkDisableAutoLogon.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.chkDisableAutoLogon.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.chkDisableAutoLogon.Location = new System.Drawing.Point(303, 314);
            this.chkDisableAutoLogon.Name = "chkDisableAutoLogon";
            this.chkDisableAutoLogon.Size = new System.Drawing.Size(184, 19);
            this.chkDisableAutoLogon.TabIndex = 25;
            this.chkDisableAutoLogon.Text = "Disable Windows Auto-Logon";
            this.chkDisableAutoLogon.UseVisualStyleBackColor = true;
            this.chkDisableAutoLogon.Visible = false;
            this.chkDisableAutoLogon.CheckedChanged += new System.EventHandler(this.chkDisableAutoLogon_CheckedChanged);
            // 
            // lblActiveUserVal
            // 
            this.lblActiveUserVal.AutoSize = true;
            this.lblActiveUserVal.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lblActiveUserVal.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
            this.lblActiveUserVal.Location = new System.Drawing.Point(385, 205);
            this.lblActiveUserVal.Name = "lblActiveUserVal";
            this.lblActiveUserVal.Size = new System.Drawing.Size(87, 17);
            this.lblActiveUserVal.TabIndex = 24;
            this.lblActiveUserVal.Text = "Administrator";
            this.lblActiveUserVal.Visible = false;
            // 
            // lblActiveUserLbl
            // 
            this.lblActiveUserLbl.AutoSize = true;
            this.lblActiveUserLbl.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblActiveUserLbl.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.lblActiveUserLbl.Location = new System.Drawing.Point(300, 205);
            this.lblActiveUserLbl.Name = "lblActiveUserLbl";
            this.lblActiveUserLbl.Size = new System.Drawing.Size(72, 15);
            this.lblActiveUserLbl.TabIndex = 23;
            this.lblActiveUserLbl.Text = "Active User :";
            this.lblActiveUserLbl.Visible = false;
            // 
            // chkAutoLogon
            // 
            this.chkAutoLogon.AutoSize = true;
            this.chkAutoLogon.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.chkAutoLogon.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.chkAutoLogon.Location = new System.Drawing.Point(303, 288);
            this.chkAutoLogon.Name = "chkAutoLogon";
            this.chkAutoLogon.Size = new System.Drawing.Size(171, 19);
            this.chkAutoLogon.TabIndex = 22;
            this.chkAutoLogon.Text = "Auto-Logon (Bypass Login)";
            this.chkAutoLogon.UseVisualStyleBackColor = true;
            this.chkAutoLogon.Visible = false;
            this.chkAutoLogon.CheckedChanged += new System.EventHandler(this.chkAutoLogon_CheckedChanged);
            // 
            // btnToggleShowPassword
            // 
            this.btnToggleShowPassword.BackColor = System.Drawing.Color.White;
            this.btnToggleShowPassword.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnToggleShowPassword.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnToggleShowPassword.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.btnToggleShowPassword.Location = new System.Drawing.Point(508, 245);
            this.btnToggleShowPassword.Name = "btnToggleShowPassword";
            this.btnToggleShowPassword.Size = new System.Drawing.Size(30, 25);
            this.btnToggleShowPassword.TabIndex = 21;
            this.btnToggleShowPassword.Text = "👁️";
            this.btnToggleShowPassword.UseVisualStyleBackColor = false;
            this.btnToggleShowPassword.Visible = false;
            this.btnToggleShowPassword.Click += new System.EventHandler(this.btnToggleShowPassword_Click);
            // 
            // txtNewPassword
            // 
            this.txtNewPassword.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.txtNewPassword.Location = new System.Drawing.Point(389, 245);
            this.txtNewPassword.Name = "txtNewPassword";
            this.txtNewPassword.PasswordChar = '●';
            this.txtNewPassword.Size = new System.Drawing.Size(115, 25);
            this.txtNewPassword.TabIndex = 20;
            this.txtNewPassword.Visible = false;
            // 
            // lblNewPassword
            // 
            this.lblNewPassword.AutoSize = true;
            this.lblNewPassword.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblNewPassword.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.lblNewPassword.Location = new System.Drawing.Point(300, 248);
            this.lblNewPassword.Name = "lblNewPassword";
            this.lblNewPassword.Size = new System.Drawing.Size(90, 15);
            this.lblNewPassword.TabIndex = 19;
            this.lblNewPassword.Text = "New Password :";
            this.lblNewPassword.Visible = false;
            // 
            // chkChangePassword
            // 
            this.chkChangePassword.AutoSize = true;
            this.chkChangePassword.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.chkChangePassword.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(118)))), ((int)(((byte)(110)))));
            this.chkChangePassword.Location = new System.Drawing.Point(303, 153);
            this.chkChangePassword.Name = "chkChangePassword";
            this.chkChangePassword.Size = new System.Drawing.Size(151, 19);
            this.chkChangePassword.TabIndex = 18;
            this.chkChangePassword.Text = "Change User Password";
            this.chkChangePassword.UseVisualStyleBackColor = true;
            this.chkChangePassword.CheckedChanged += new System.EventHandler(this.chkChangePassword_CheckedChanged);
            // 
            // lblLabSelect
            // 
            this.lblLabSelect.AutoSize = true;
            this.lblLabSelect.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblLabSelect.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
            this.lblLabSelect.Location = new System.Drawing.Point(17, 36);
            this.lblLabSelect.Name = "lblLabSelect";
            this.lblLabSelect.Size = new System.Drawing.Size(87, 17);
            this.lblLabSelect.TabIndex = 13;
            this.lblLabSelect.Text = "Lab Location";
            // 
            // cmbLabSelect
            // 
            this.cmbLabSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbLabSelect.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.cmbLabSelect.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.cmbLabSelect.FormattingEnabled = true;
            this.cmbLabSelect.IntegralHeight = false;
            this.cmbLabSelect.MaxDropDownItems = 10;
            this.cmbLabSelect.Location = new System.Drawing.Point(127, 32);
            this.cmbLabSelect.Name = "cmbLabSelect";
            this.cmbLabSelect.Size = new System.Drawing.Size(231, 25);
            this.cmbLabSelect.TabIndex = 14;
            this.cmbLabSelect.SelectedIndexChanged += new System.EventHandler(this.cmbLabSelect_SelectedIndexChanged);
            // 
            // btnAddLab
            // 
            this.btnAddLab.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.btnAddLab.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddLab.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnAddLab.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.btnAddLab.Location = new System.Drawing.Point(368, 33);
            this.btnAddLab.Name = "btnAddLab";
            this.btnAddLab.Size = new System.Drawing.Size(30, 25);
            this.btnAddLab.TabIndex = 15;
            this.btnAddLab.Text = "➕";
            this.btnAddLab.UseVisualStyleBackColor = false;
            this.btnAddLab.Click += new System.EventHandler(this.btnAddLab_Click);
            // 
            // btnEditLab
            // 
            this.btnEditLab.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.btnEditLab.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEditLab.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnEditLab.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.btnEditLab.Location = new System.Drawing.Point(406, 33);
            this.btnEditLab.Name = "btnEditLab";
            this.btnEditLab.Size = new System.Drawing.Size(30, 25);
            this.btnEditLab.TabIndex = 17;
            this.btnEditLab.Text = "✏️";
            this.btnEditLab.UseVisualStyleBackColor = false;
            this.btnEditLab.Visible = false;
            this.btnEditLab.Click += new System.EventHandler(this.btnEditLab_Click);
            // 
            // btnDeleteLab
            // 
            this.btnDeleteLab.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(254)))), ((int)(((byte)(242)))), ((int)(((byte)(242)))));
            this.btnDeleteLab.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDeleteLab.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnDeleteLab.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
            this.btnDeleteLab.Location = new System.Drawing.Point(444, 33);
            this.btnDeleteLab.Name = "btnDeleteLab";
            this.btnDeleteLab.Size = new System.Drawing.Size(30, 25);
            this.btnDeleteLab.TabIndex = 16;
            this.btnDeleteLab.Text = "🗑️";
            this.btnDeleteLab.UseVisualStyleBackColor = false;
            this.btnDeleteLab.Visible = false;
            this.btnDeleteLab.Click += new System.EventHandler(this.btnDeleteLab_Click);
            // 
            // btnRefreshIf
            // 
            this.btnRefreshIf.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.btnRefreshIf.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefreshIf.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnRefreshIf.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.btnRefreshIf.Location = new System.Drawing.Point(268, 76);
            this.btnRefreshIf.Name = "btnRefreshIf";
            this.btnRefreshIf.Size = new System.Drawing.Size(30, 25);
            this.btnRefreshIf.TabIndex = 12;
            this.btnRefreshIf.Text = "🔄";
            this.btnRefreshIf.UseVisualStyleBackColor = false;
            this.btnRefreshIf.Click += new System.EventHandler(this.btnRefreshIf_Click);
            // 
            // lblBaseIp
            // 
            this.lblBaseIp.AutoSize = true;
            this.lblBaseIp.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblBaseIp.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.lblBaseIp.Location = new System.Drawing.Point(17, 123);
            this.lblBaseIp.Name = "lblBaseIp";
            this.lblBaseIp.Size = new System.Drawing.Size(85, 15);
            this.lblBaseIp.TabIndex = 3;
            this.lblBaseIp.Text = "BASE IP Prefix :";
            // 
            // txtBaseIp
            // 
            this.txtBaseIp.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.txtBaseIp.Location = new System.Drawing.Point(127, 120);
            this.txtBaseIp.Name = "txtBaseIp";
            this.txtBaseIp.ReadOnly = true;
            this.txtBaseIp.Size = new System.Drawing.Size(135, 25);
            this.txtBaseIp.TabIndex = 2;
            this.txtBaseIp.Text = "10.22.1";
            this.txtBaseIp.TextChanged += new System.EventHandler(this.InputConfig_Changed);
            // 
            // lblSubnet
            // 
            this.lblSubnet.AutoSize = true;
            this.lblSubnet.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblSubnet.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.lblSubnet.Location = new System.Drawing.Point(17, 165);
            this.lblSubnet.Name = "lblSubnet";
            this.lblSubnet.Size = new System.Drawing.Size(81, 15);
            this.lblSubnet.TabIndex = 5;
            this.lblSubnet.Text = "Subnet Mask :";
            // 
            // txtSubnet
            // 
            this.txtSubnet.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.txtSubnet.Location = new System.Drawing.Point(127, 162);
            this.txtSubnet.Name = "txtSubnet";
            this.txtSubnet.ReadOnly = true;
            this.txtSubnet.Size = new System.Drawing.Size(135, 25);
            this.txtSubnet.TabIndex = 4;
            this.txtSubnet.Text = "255.255.255.0";
            // 
            // lblDns
            // 
            this.lblDns.AutoSize = true;
            this.lblDns.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblDns.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.lblDns.Location = new System.Drawing.Point(17, 208);
            this.lblDns.Name = "lblDns";
            this.lblDns.Size = new System.Drawing.Size(71, 15);
            this.lblDns.TabIndex = 15;
            this.lblDns.Text = "DNS Server :";
            // 
            // txtDns
            // 
            this.txtDns.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.txtDns.Location = new System.Drawing.Point(127, 205);
            this.txtDns.Name = "txtDns";
            this.txtDns.ReadOnly = true;
            this.txtDns.Size = new System.Drawing.Size(135, 25);
            this.txtDns.TabIndex = 16;
            this.txtDns.Text = "10.1.1.111";
            // 
            // lblPrefix
            // 
            this.lblPrefix.AutoSize = true;
            this.lblPrefix.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblPrefix.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.lblPrefix.Location = new System.Drawing.Point(17, 252);
            this.lblPrefix.Name = "lblPrefix";
            this.lblPrefix.Size = new System.Drawing.Size(100, 15);
            this.lblPrefix.TabIndex = 11;
            this.lblPrefix.Text = "Prefix Hostname :";
            // 
            // txtPrefix
            // 
            this.txtPrefix.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.txtPrefix.Location = new System.Drawing.Point(127, 248);
            this.txtPrefix.Name = "txtPrefix";
            this.txtPrefix.ReadOnly = true;
            this.txtPrefix.Size = new System.Drawing.Size(135, 25);
            this.txtPrefix.TabIndex = 10;
            this.txtPrefix.Text = "Komputer-";
            this.txtPrefix.TextChanged += new System.EventHandler(this.InputConfig_Changed);
            // 
            // lblGateway
            // 
            this.lblGateway.AutoSize = true;
            this.lblGateway.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblGateway.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.lblGateway.Location = new System.Drawing.Point(17, 294);
            this.lblGateway.Name = "lblGateway";
            this.lblGateway.Size = new System.Drawing.Size(58, 15);
            this.lblGateway.TabIndex = 7;
            this.lblGateway.Text = "Gateway :";
            // 
            // txtGateway
            // 
            this.txtGateway.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.txtGateway.Location = new System.Drawing.Point(127, 290);
            this.txtGateway.Name = "txtGateway";
            this.txtGateway.ReadOnly = true;
            this.txtGateway.Size = new System.Drawing.Size(135, 25);
            this.txtGateway.TabIndex = 6;
            this.txtGateway.Text = "10.22.1.254";
            // 
            // lblWorkgroup
            // 
            this.lblWorkgroup.AutoSize = true;
            this.lblWorkgroup.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblWorkgroup.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.lblWorkgroup.Location = new System.Drawing.Point(17, 337);
            this.lblWorkgroup.Name = "lblWorkgroup";
            this.lblWorkgroup.Size = new System.Drawing.Size(73, 15);
            this.lblWorkgroup.TabIndex = 9;
            this.lblWorkgroup.Text = "Workgroup :";
            // 
            // txtWorkgroup
            // 
            this.txtWorkgroup.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.txtWorkgroup.Location = new System.Drawing.Point(127, 332);
            this.txtWorkgroup.Name = "txtWorkgroup";
            this.txtWorkgroup.ReadOnly = true;
            this.txtWorkgroup.Size = new System.Drawing.Size(135, 25);
            this.txtWorkgroup.TabIndex = 8;
            this.txtWorkgroup.Text = "Lab-2.2.1";
            // 
            // lblInterface
            // 
            this.lblInterface.AutoSize = true;
            this.lblInterface.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblInterface.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.lblInterface.Location = new System.Drawing.Point(17, 80);
            this.lblInterface.Name = "lblInterface";
            this.lblInterface.Size = new System.Drawing.Size(103, 15);
            this.lblInterface.TabIndex = 1;
            this.lblInterface.Text = "Network Settings :";
            // 
            // cmbInterface
            // 
            this.cmbInterface.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.cmbInterface.FormattingEnabled = true;
            this.cmbInterface.Location = new System.Drawing.Point(127, 76);
            this.cmbInterface.Name = "cmbInterface";
            this.cmbInterface.Size = new System.Drawing.Size(135, 25);
            this.cmbInterface.TabIndex = 0;
            // 
            // grpAction
            // 
            this.grpAction.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.grpAction.Controls.Add(this.lblPreviewHostname);
            this.grpAction.Controls.Add(this.lblPreviewIp);
            this.grpAction.Controls.Add(this.btnApply);
            this.grpAction.Controls.Add(this.txtPcNum);
            this.grpAction.Controls.Add(this.lblPcNum);
            this.grpAction.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.grpAction.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.grpAction.Location = new System.Drawing.Point(14, 397);
            this.grpAction.Name = "grpAction";
            this.grpAction.Size = new System.Drawing.Size(559, 107);
            this.grpAction.TabIndex = 1;
            this.grpAction.TabStop = false;
            this.grpAction.Text = "Set Computer Number";
            // 
            // lblCurrentHostname
            // 
            this.lblCurrentHostname.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lblCurrentHostname.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.lblCurrentHostname.Location = new System.Drawing.Point(299, 120);
            this.lblCurrentHostname.Name = "lblCurrentHostname";
            this.lblCurrentHostname.Size = new System.Drawing.Size(241, 23);
            this.lblCurrentHostname.TabIndex = 10;
            this.lblCurrentHostname.Text = "Current Hostname: Dektop-Pc";
            this.lblCurrentHostname.Click += new System.EventHandler(this.lblCurrentHostname_Click);
            // 
            // lblPreviewHostname
            // 
            this.lblPreviewHostname.AutoSize = true;
            this.lblPreviewHostname.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Italic);
            this.lblPreviewHostname.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(118)))), ((int)(((byte)(110)))));
            this.lblPreviewHostname.Location = new System.Drawing.Point(199, 66);
            this.lblPreviewHostname.Name = "lblPreviewHostname";
            this.lblPreviewHostname.Size = new System.Drawing.Size(185, 17);
            this.lblPreviewHostname.TabIndex = 4;
            this.lblPreviewHostname.Text = "Target Hostname: Komputer-00";
            // 
            // lblPreviewIp
            // 
            this.lblPreviewIp.AutoSize = true;
            this.lblPreviewIp.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Italic);
            this.lblPreviewIp.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(148)))), ((int)(((byte)(136)))));
            this.lblPreviewIp.Location = new System.Drawing.Point(199, 43);
            this.lblPreviewIp.Name = "lblPreviewIp";
            this.lblPreviewIp.Size = new System.Drawing.Size(116, 17);
            this.lblPreviewIp.TabIndex = 3;
            this.lblPreviewIp.Text = "Target IP: 10.22.1.0";
            // 
            // btnApply
            // 
            this.btnApply.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(118)))), ((int)(((byte)(110)))));
            this.btnApply.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnApply.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnApply.ForeColor = System.Drawing.Color.White;
            this.btnApply.Location = new System.Drawing.Point(440, 30);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(100, 55);
            this.btnApply.TabIndex = 2;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = false;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // txtPcNum
            // 
            this.txtPcNum.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
            this.txtPcNum.Location = new System.Drawing.Point(17, 59);
            this.txtPcNum.Name = "txtPcNum";
            this.txtPcNum.Size = new System.Drawing.Size(155, 31);
            this.txtPcNum.TabIndex = 1;
            this.txtPcNum.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtPcNum.TextChanged += new System.EventHandler(this.txtPcNum_TextChanged);
            // 
            // lblPcNum
            // 
            this.lblPcNum.AutoSize = true;
            this.lblPcNum.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblPcNum.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.lblPcNum.Location = new System.Drawing.Point(17, 37);
            this.lblPcNum.Name = "lblPcNum";
            this.lblPcNum.Size = new System.Drawing.Size(154, 15);
            this.lblPcNum.TabIndex = 0;
            this.lblPcNum.Text = "Computer Number (1 - 254)";
            // 
            // grpLog
            // 
            this.grpLog.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.grpLog.Controls.Add(this.txtLog);
            this.grpLog.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.grpLog.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.grpLog.Location = new System.Drawing.Point(14, 512);
            this.grpLog.Name = "grpLog";
            this.grpLog.Size = new System.Drawing.Size(559, 147);
            this.grpLog.TabIndex = 2;
            this.grpLog.TabStop = false;
            this.grpLog.Text = "Activity Log";
            // 
            // txtLog
            // 
            this.txtLog.BackColor = System.Drawing.Color.Black;
            this.txtLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtLog.Font = new System.Drawing.Font("Consolas", 9.75F);
            this.txtLog.ForeColor = System.Drawing.Color.LightYellow;
            this.txtLog.Location = new System.Drawing.Point(14, 34);
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.Size = new System.Drawing.Size(530, 100);
            this.txtLog.TabIndex = 0;
            this.txtLog.Text = "";
            // 
            // lblCopyright
            // 
            this.lblCopyright.AutoSize = true;
            this.lblCopyright.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.lblCopyright.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.lblCopyright.Location = new System.Drawing.Point(110, 668);
            this.lblCopyright.Name = "lblCopyright";
            this.lblCopyright.Size = new System.Drawing.Size(404, 13);
            this.lblCopyright.TabIndex = 3;
            this.lblCopyright.Text = "Developed By Ravenusa | Student ID: 20.11.3623 | UPT Lab Amikom Yogyakarta";
            this.lblCopyright.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.ClientSize = new System.Drawing.Size(586, 690);
            this.Controls.Add(this.lblCopyright);
            this.Controls.Add(this.grpLog);
            this.Controls.Add(this.grpAction);
            this.Controls.Add(this.grpConfig);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "AirSET | Amikom Infrastructure & Network Setter";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.grpConfig.ResumeLayout(false);
            this.grpConfig.PerformLayout();
            this.grpAction.ResumeLayout(false);
            this.grpAction.PerformLayout();
            this.grpLog.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox grpConfig;
        private System.Windows.Forms.Label lblLabSelect;
        private System.Windows.Forms.ComboBox cmbLabSelect;
        private System.Windows.Forms.Button btnAddLab;
        private System.Windows.Forms.Button btnEditLab;
        private System.Windows.Forms.Button btnDeleteLab;
        private System.Windows.Forms.Label lblInterface;
        private System.Windows.Forms.ComboBox cmbInterface;
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
        private System.Windows.Forms.Button btnRefreshIf;
        private System.Windows.Forms.Label lblActiveUserLbl;
        private System.Windows.Forms.Label lblActiveUserVal;
        private System.Windows.Forms.CheckBox chkChangePassword;
        private System.Windows.Forms.Label lblNewPassword;
        private System.Windows.Forms.TextBox txtNewPassword;
        private System.Windows.Forms.Button btnToggleShowPassword;
        private System.Windows.Forms.CheckBox chkAutoLogon;
        private System.Windows.Forms.CheckBox chkDisableAutoLogon;
        private System.Windows.Forms.GroupBox grpAction;
        private System.Windows.Forms.Label lblCurrentHostname;
        private System.Windows.Forms.Label lblPcNum;
        private System.Windows.Forms.TextBox txtPcNum;
        private System.Windows.Forms.Label lblPreviewIp;
        private System.Windows.Forms.Label lblPreviewHostname;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.GroupBox grpLog;
        private System.Windows.Forms.RichTextBox txtLog;
        private System.Windows.Forms.Label lblCopyright;
    }
}
