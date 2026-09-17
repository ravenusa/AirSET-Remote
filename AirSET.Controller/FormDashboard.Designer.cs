namespace AirSET.Controller
{
    partial class FormDashboard
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
            this.pnlTop = new System.Windows.Forms.Panel();
            this.btnChangeAppPass = new System.Windows.Forms.Button();
            this.lblSub = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.grpConfig = new System.Windows.Forms.GroupBox();
            this.btnDeleteLab = new System.Windows.Forms.Button();
            this.btnSaveLab = new System.Windows.Forms.Button();
            this.btnAddLab = new System.Windows.Forms.Button();
            this.txtWorkgroup = new System.Windows.Forms.TextBox();
            this.lblWorkgroup = new System.Windows.Forms.Label();
            this.txtPrefix = new System.Windows.Forms.TextBox();
            this.lblPrefix = new System.Windows.Forms.Label();
            this.txtDns = new System.Windows.Forms.TextBox();
            this.lblDns = new System.Windows.Forms.Label();
            this.txtGateway = new System.Windows.Forms.TextBox();
            this.lblGateway = new System.Windows.Forms.Label();
            this.txtSubnet = new System.Windows.Forms.TextBox();
            this.lblSubnet = new System.Windows.Forms.Label();
            this.txtBaseIp = new System.Windows.Forms.TextBox();
            this.lblBaseIp = new System.Windows.Forms.Label();
            this.cmbLab = new System.Windows.Forms.ComboBox();
            this.lblLab = new System.Windows.Forms.Label();
            this.grpBatch = new System.Windows.Forms.GroupBox();
            this.btnRefreshInterface = new System.Windows.Forms.Button();
            this.cmbInterface = new System.Windows.Forms.ComboBox();
            this.lblInterface = new System.Windows.Forms.Label();
            this.btnPushConfig = new System.Windows.Forms.Button();
            this.chkRestart = new System.Windows.Forms.CheckBox();
            this.chkDisableLogon = new System.Windows.Forms.CheckBox();
            this.chkAutoLogon = new System.Windows.Forms.CheckBox();
            this.txtNewPass = new System.Windows.Forms.TextBox();
            this.lblPass = new System.Windows.Forms.Label();
            this.chkChangePass = new System.Windows.Forms.CheckBox();
            this.grpGrid = new System.Windows.Forms.GroupBox();
            this.btnDirectAdd = new System.Windows.Forms.Button();
            this.txtDirectIp = new System.Windows.Forms.TextBox();
            this.dgvClients = new System.Windows.Forms.DataGridView();
            this.colCheck = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colHostname = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colIp = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTargetIp = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTargetHostname = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblTotalOnline = new System.Windows.Forms.Label();
            this.chkSelectAll = new System.Windows.Forms.CheckBox();
            this.btnScan = new System.Windows.Forms.Button();
            this.grpLog = new System.Windows.Forms.GroupBox();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.lblCopyright = new System.Windows.Forms.Label();
            this.tabMain = new System.Windows.Forms.TabControl();
            this.tabNetwork = new System.Windows.Forms.TabPage();
            this.tabShield = new System.Windows.Forms.TabPage();
            this.grpShieldAction = new System.Windows.Forms.GroupBox();
            this.btnUwfUninstall = new System.Windows.Forms.Button();
            this.btnUwfInstall = new System.Windows.Forms.Button();
            this.btnShieldUnlock = new System.Windows.Forms.Button();
            this.btnShieldLock = new System.Windows.Forms.Button();
            this.btnShieldRestart = new System.Windows.Forms.Button();
            this.txtDfcPassword = new System.Windows.Forms.TextBox();
            this.lblDfcPass = new System.Windows.Forms.Label();
            this.lblShieldInfo = new System.Windows.Forms.Label();
            this.grpShieldGrid = new System.Windows.Forms.GroupBox();
            this.btnShieldDirectAdd = new System.Windows.Forms.Button();
            this.txtShieldDirectIp = new System.Windows.Forms.TextBox();
            this.dgvShieldClients = new System.Windows.Forms.DataGridView();
            this.colShieldCheck = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colShieldNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colShieldHostname = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colShieldIp = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colShieldOs = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colShieldStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colShieldAction = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblShieldTotal = new System.Windows.Forms.Label();
            this.chkShieldSelectAll = new System.Windows.Forms.CheckBox();
            this.btnShieldScan = new System.Windows.Forms.Button();
            this.tabPower = new System.Windows.Forms.TabPage();
            this.grpDesktop = new System.Windows.Forms.GroupBox();
            this.lblDesktopDesc = new System.Windows.Forms.Label();
            this.tblDesktop = new System.Windows.Forms.TableLayoutPanel();
            this.btnLockScreen = new System.Windows.Forms.Button();
            this.btnUnlockScreen = new System.Windows.Forms.Button();
            this.btnClearDesktop = new System.Windows.Forms.Button();
            this.grpPower = new System.Windows.Forms.GroupBox();
            this.lblPowerDesc = new System.Windows.Forms.Label();
            this.tblPower = new System.Windows.Forms.TableLayoutPanel();
            this.btnShutdown = new System.Windows.Forms.Button();
            this.btnRestart = new System.Windows.Forms.Button();
            this.btnWakeOnLan = new System.Windows.Forms.Button();
            this.lblWolHint = new System.Windows.Forms.Label();
            this.grpLivePower = new System.Windows.Forms.GroupBox();
            this.btnPowerScan = new System.Windows.Forms.Button();
            this.chkPowerSelectAll = new System.Windows.Forms.CheckBox();
            this.lblPowerTotal = new System.Windows.Forms.Label();
            this.dgvPowerClients = new System.Windows.Forms.DataGridView();
            this.colPowerCheck = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colPowerNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPowerHostname = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPowerIp = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPowerStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabQuickLaunch = new System.Windows.Forms.TabPage();
            this.grpPresets = new System.Windows.Forms.GroupBox();
            this.tblPresets = new System.Windows.Forms.TableLayoutPanel();
            this.btnRemotePs = new System.Windows.Forms.Button();
            this.btnEdge = new System.Windows.Forms.Button();
            this.btnCmd = new System.Windows.Forms.Button();
            this.btnNotepad = new System.Windows.Forms.Button();
            this.grpCustom = new System.Windows.Forms.GroupBox();
            this.tblCustom = new System.Windows.Forms.TableLayoutPanel();
            this.pnlInputs = new System.Windows.Forms.Panel();
            this.lblTarget = new System.Windows.Forms.Label();
            this.tblTargetRow = new System.Windows.Forms.TableLayoutPanel();
            this.txtLaunchTarget = new System.Windows.Forms.TextBox();
            this.btnBrowseLaunch = new System.Windows.Forms.Button();
            this.lblArgs = new System.Windows.Forms.Label();
            this.txtLaunchArgs = new System.Windows.Forms.TextBox();
            this.btnExecuteCustom = new System.Windows.Forms.Button();
            this.grpLiveLaunch = new System.Windows.Forms.GroupBox();
            this.btnLaunchScan = new System.Windows.Forms.Button();
            this.chkLaunchSelectAll = new System.Windows.Forms.CheckBox();
            this.lblLaunchTotal = new System.Windows.Forms.Label();
            this.dgvLaunchClients = new System.Windows.Forms.DataGridView();
            this.colLaunchCheck = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colLaunchNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLaunchHostname = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLaunchIp = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLaunchStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabFileManager = new System.Windows.Forms.TabPage();
            this.grpTransfer = new System.Windows.Forms.GroupBox();
            this.tblTransfer = new System.Windows.Forms.TableLayoutPanel();
            this.pnlTransLeft = new System.Windows.Forms.Panel();
            this.lblTargetPc = new System.Windows.Forms.Label();
            this.cmbTargetClient = new System.Windows.Forms.ComboBox();
            this.btnRefreshTargetPc = new System.Windows.Forms.Button();
            this.lblDest1 = new System.Windows.Forms.Label();
            this.cmbDest1 = new System.Windows.Forms.ComboBox();
            this.tblTransFile = new System.Windows.Forms.TableLayoutPanel();
            this.lblFile1 = new System.Windows.Forms.Label();
            this.txtFile1 = new System.Windows.Forms.TextBox();
            this.btnBrowse1 = new System.Windows.Forms.Button();
            this.btnSendSingle = new System.Windows.Forms.Button();
            this.grpDist = new System.Windows.Forms.GroupBox();
            this.tblDist = new System.Windows.Forms.TableLayoutPanel();
            this.pnlDistLeft = new System.Windows.Forms.Panel();
            this.lblDestDist = new System.Windows.Forms.Label();
            this.cmbDestDist = new System.Windows.Forms.ComboBox();
            this.rdoSelectedClients = new System.Windows.Forms.RadioButton();
            this.rdoAllClients = new System.Windows.Forms.RadioButton();
            this.tblDistFile = new System.Windows.Forms.TableLayoutPanel();
            this.lblFileDist = new System.Windows.Forms.Label();
            this.txtFileDist = new System.Windows.Forms.TextBox();
            this.btnBrowseDist = new System.Windows.Forms.Button();
            this.btnDistribute = new System.Windows.Forms.Button();
            this.grpCollect = new System.Windows.Forms.GroupBox();
            this.tblCollect = new System.Windows.Forms.TableLayoutPanel();
            this.pnlCollLeft = new System.Windows.Forms.Panel();
            this.lblSource = new System.Windows.Forms.Label();
            this.cmbCollectSource = new System.Windows.Forms.ComboBox();
            this.txtSourceDir = new System.Windows.Forms.TextBox();
            this.lblPattern = new System.Windows.Forms.Label();
            this.txtCollectPattern = new System.Windows.Forms.TextBox();
            this.chkDeleteAfterCollect = new System.Windows.Forms.CheckBox();
            this.tblSaveRow = new System.Windows.Forms.TableLayoutPanel();
            this.lblSaveHost = new System.Windows.Forms.Label();
            this.txtSaveHost = new System.Windows.Forms.TextBox();
            this.btnBrowseHost = new System.Windows.Forms.Button();
            this.btnCollect = new System.Windows.Forms.Button();
            this.grpLiveFile = new System.Windows.Forms.GroupBox();
            this.btnFileScan = new System.Windows.Forms.Button();
            this.chkFileSelectAll = new System.Windows.Forms.CheckBox();
            this.lblFileTotal = new System.Windows.Forms.Label();
            this.dgvFileClients = new System.Windows.Forms.DataGridView();
            this.colFileCheck = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colFileNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFileHostname = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFileIp = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFileStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlTop.SuspendLayout();
            this.grpConfig.SuspendLayout();
            this.grpBatch.SuspendLayout();
            this.grpGrid.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvClients)).BeginInit();
            this.grpLog.SuspendLayout();
            this.tabMain.SuspendLayout();
            this.tabNetwork.SuspendLayout();
            this.tabShield.SuspendLayout();
            this.grpShieldAction.SuspendLayout();
            this.grpShieldGrid.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvShieldClients)).BeginInit();
            this.tabPower.SuspendLayout();
            this.grpDesktop.SuspendLayout();
            this.tblDesktop.SuspendLayout();
            this.grpPower.SuspendLayout();
            this.tblPower.SuspendLayout();
            this.grpLivePower.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPowerClients)).BeginInit();
            this.tabQuickLaunch.SuspendLayout();
            this.grpPresets.SuspendLayout();
            this.tblPresets.SuspendLayout();
            this.grpCustom.SuspendLayout();
            this.tblCustom.SuspendLayout();
            this.pnlInputs.SuspendLayout();
            this.tblTargetRow.SuspendLayout();
            this.grpLiveLaunch.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLaunchClients)).BeginInit();
            this.tabFileManager.SuspendLayout();
            this.grpTransfer.SuspendLayout();
            this.tblTransfer.SuspendLayout();
            this.pnlTransLeft.SuspendLayout();
            this.tblTransFile.SuspendLayout();
            this.grpDist.SuspendLayout();
            this.tblDist.SuspendLayout();
            this.pnlDistLeft.SuspendLayout();
            this.tblDistFile.SuspendLayout();
            this.grpCollect.SuspendLayout();
            this.tblCollect.SuspendLayout();
            this.pnlCollLeft.SuspendLayout();
            this.tblSaveRow.SuspendLayout();
            this.grpLiveFile.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFileClients)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlTop
            // 
            this.pnlTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(118)))), ((int)(((byte)(110)))));
            this.pnlTop.Controls.Add(this.btnChangeAppPass);
            this.pnlTop.Controls.Add(this.lblSub);
            this.pnlTop.Controls.Add(this.lblTitle);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new System.Drawing.Size(984, 55);
            this.pnlTop.TabIndex = 0;
            // 
            // btnChangeAppPass
            // 
            this.btnChangeAppPass.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnChangeAppPass.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(184)))), ((int)(((byte)(166)))));
            this.btnChangeAppPass.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnChangeAppPass.FlatAppearance.BorderSize = 0;
            this.btnChangeAppPass.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnChangeAppPass.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnChangeAppPass.ForeColor = System.Drawing.Color.White;
            this.btnChangeAppPass.Location = new System.Drawing.Point(800, 12);
            this.btnChangeAppPass.Name = "btnChangeAppPass";
            this.btnChangeAppPass.Size = new System.Drawing.Size(168, 30);
            this.btnChangeAppPass.TabIndex = 2;
            this.btnChangeAppPass.Text = "🔑 Ubah Password App";
            this.btnChangeAppPass.UseVisualStyleBackColor = false;
            this.btnChangeAppPass.Click += new System.EventHandler(this.btnChangeAppPass_Click);
            // 
            // lblSub
            // 
            this.lblSub.AutoSize = true;
            this.lblSub.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblSub.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(251)))), ((int)(((byte)(241)))));
            this.lblSub.Location = new System.Drawing.Point(18, 33);
            this.lblSub.Name = "lblSub";
            this.lblSub.Size = new System.Drawing.Size(448, 15);
            this.lblSub.TabIndex = 1;
            this.lblSub.Text = "Remote Management & Automated Basic Infrastructure Configuration - Lab Amikom";
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(16, 8);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(298, 25);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "AirSET Controller (Host Manager)";
            // 
            // grpConfig
            // 
            this.grpConfig.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.grpConfig.Controls.Add(this.btnDeleteLab);
            this.grpConfig.Controls.Add(this.btnSaveLab);
            this.grpConfig.Controls.Add(this.btnAddLab);
            this.grpConfig.Controls.Add(this.txtWorkgroup);
            this.grpConfig.Controls.Add(this.lblWorkgroup);
            this.grpConfig.Controls.Add(this.txtPrefix);
            this.grpConfig.Controls.Add(this.lblPrefix);
            this.grpConfig.Controls.Add(this.txtDns);
            this.grpConfig.Controls.Add(this.lblDns);
            this.grpConfig.Controls.Add(this.txtGateway);
            this.grpConfig.Controls.Add(this.lblGateway);
            this.grpConfig.Controls.Add(this.txtSubnet);
            this.grpConfig.Controls.Add(this.lblSubnet);
            this.grpConfig.Controls.Add(this.txtBaseIp);
            this.grpConfig.Controls.Add(this.lblBaseIp);
            this.grpConfig.Controls.Add(this.cmbLab);
            this.grpConfig.Controls.Add(this.lblLab);
            this.grpConfig.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.grpConfig.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.grpConfig.Location = new System.Drawing.Point(6, 6);
            this.grpConfig.Name = "grpConfig";
            this.grpConfig.Size = new System.Drawing.Size(460, 195);
            this.grpConfig.TabIndex = 0;
            this.grpConfig.TabStop = false;
            this.grpConfig.Text = "Profil & Konfigurasi LAB";
            // 
            // btnDeleteLab
            // 
            this.btnDeleteLab.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(254)))), ((int)(((byte)(242)))), ((int)(((byte)(242)))));
            this.btnDeleteLab.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDeleteLab.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnDeleteLab.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
            this.btnDeleteLab.Location = new System.Drawing.Point(400, 22);
            this.btnDeleteLab.Name = "btnDeleteLab";
            this.btnDeleteLab.Size = new System.Drawing.Size(35, 25);
            this.btnDeleteLab.TabIndex = 16;
            this.btnDeleteLab.Text = "🗑️";
            this.btnDeleteLab.UseVisualStyleBackColor = false;
            this.btnDeleteLab.Visible = false;
            this.btnDeleteLab.Click += new System.EventHandler(this.btnDeleteLab_Click);
            // 
            // btnSaveLab
            // 
            this.btnSaveLab.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(254)))), ((int)(((byte)(243)))), ((int)(((byte)(199)))));
            this.btnSaveLab.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveLab.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnSaveLab.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(83)))), ((int)(((byte)(9)))));
            this.btnSaveLab.Location = new System.Drawing.Point(310, 156);
            this.btnSaveLab.Name = "btnSaveLab";
            this.btnSaveLab.Size = new System.Drawing.Size(130, 28);
            this.btnSaveLab.TabIndex = 15;
            this.btnSaveLab.Text = "💾 Save Config";
            this.btnSaveLab.UseVisualStyleBackColor = false;
            this.btnSaveLab.Visible = false;
            this.btnSaveLab.Click += new System.EventHandler(this.btnSaveLab_Click);
            // 
            // btnAddLab
            // 
            this.btnAddLab.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.btnAddLab.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddLab.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnAddLab.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.btnAddLab.Location = new System.Drawing.Point(355, 22);
            this.btnAddLab.Name = "btnAddLab";
            this.btnAddLab.Size = new System.Drawing.Size(35, 25);
            this.btnAddLab.TabIndex = 14;
            this.btnAddLab.Text = "➕";
            this.btnAddLab.UseVisualStyleBackColor = false;
            this.btnAddLab.Click += new System.EventHandler(this.btnAddLab_Click);
            // 
            // txtWorkgroup
            // 
            this.txtWorkgroup.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtWorkgroup.Location = new System.Drawing.Point(310, 126);
            this.txtWorkgroup.Name = "txtWorkgroup";
            this.txtWorkgroup.Size = new System.Drawing.Size(130, 23);
            this.txtWorkgroup.TabIndex = 13;
            // 
            // lblWorkgroup
            // 
            this.lblWorkgroup.AutoSize = true;
            this.lblWorkgroup.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblWorkgroup.Location = new System.Drawing.Point(235, 130);
            this.lblWorkgroup.Name = "lblWorkgroup";
            this.lblWorkgroup.Size = new System.Drawing.Size(70, 15);
            this.lblWorkgroup.TabIndex = 12;
            this.lblWorkgroup.Text = "Workgroup:";
            // 
            // txtPrefix
            // 
            this.txtPrefix.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtPrefix.Location = new System.Drawing.Point(100, 126);
            this.txtPrefix.Name = "txtPrefix";
            this.txtPrefix.Size = new System.Drawing.Size(120, 23);
            this.txtPrefix.TabIndex = 11;
            // 
            // lblPrefix
            // 
            this.lblPrefix.AutoSize = true;
            this.lblPrefix.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblPrefix.Location = new System.Drawing.Point(15, 130);
            this.lblPrefix.Name = "lblPrefix";
            this.lblPrefix.Size = new System.Drawing.Size(86, 15);
            this.lblPrefix.TabIndex = 10;
            this.lblPrefix.Text = "Prefix H-name:";
            // 
            // txtDns
            // 
            this.txtDns.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtDns.Location = new System.Drawing.Point(310, 91);
            this.txtDns.Name = "txtDns";
            this.txtDns.Size = new System.Drawing.Size(130, 23);
            this.txtDns.TabIndex = 9;
            // 
            // lblDns
            // 
            this.lblDns.AutoSize = true;
            this.lblDns.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblDns.Location = new System.Drawing.Point(235, 95);
            this.lblDns.Name = "lblDns";
            this.lblDns.Size = new System.Drawing.Size(68, 15);
            this.lblDns.TabIndex = 8;
            this.lblDns.Text = "DNS Server:";
            // 
            // txtGateway
            // 
            this.txtGateway.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtGateway.Location = new System.Drawing.Point(100, 91);
            this.txtGateway.Name = "txtGateway";
            this.txtGateway.Size = new System.Drawing.Size(120, 23);
            this.txtGateway.TabIndex = 7;
            // 
            // lblGateway
            // 
            this.lblGateway.AutoSize = true;
            this.lblGateway.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblGateway.Location = new System.Drawing.Point(15, 95);
            this.lblGateway.Name = "lblGateway";
            this.lblGateway.Size = new System.Drawing.Size(55, 15);
            this.lblGateway.TabIndex = 6;
            this.lblGateway.Text = "Gateway:";
            // 
            // txtSubnet
            // 
            this.txtSubnet.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtSubnet.Location = new System.Drawing.Point(310, 56);
            this.txtSubnet.Name = "txtSubnet";
            this.txtSubnet.Size = new System.Drawing.Size(130, 23);
            this.txtSubnet.TabIndex = 5;
            // 
            // lblSubnet
            // 
            this.lblSubnet.AutoSize = true;
            this.lblSubnet.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblSubnet.Location = new System.Drawing.Point(235, 60);
            this.lblSubnet.Name = "lblSubnet";
            this.lblSubnet.Size = new System.Drawing.Size(47, 15);
            this.lblSubnet.TabIndex = 4;
            this.lblSubnet.Text = "Subnet:";
            // 
            // txtBaseIp
            // 
            this.txtBaseIp.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtBaseIp.Location = new System.Drawing.Point(100, 56);
            this.txtBaseIp.Name = "txtBaseIp";
            this.txtBaseIp.Size = new System.Drawing.Size(120, 23);
            this.txtBaseIp.TabIndex = 3;
            // 
            // lblBaseIp
            // 
            this.lblBaseIp.AutoSize = true;
            this.lblBaseIp.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblBaseIp.Location = new System.Drawing.Point(15, 60);
            this.lblBaseIp.Name = "lblBaseIp";
            this.lblBaseIp.Size = new System.Drawing.Size(47, 15);
            this.lblBaseIp.TabIndex = 2;
            this.lblBaseIp.Text = "Base IP:";
            // 
            // cmbLab
            // 
            this.cmbLab.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbLab.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.cmbLab.FormattingEnabled = true;
            this.cmbLab.IntegralHeight = false;
            this.cmbLab.Location = new System.Drawing.Point(90, 22);
            this.cmbLab.MaxDropDownItems = 10;
            this.cmbLab.Name = "cmbLab";
            this.cmbLab.Size = new System.Drawing.Size(250, 25);
            this.cmbLab.TabIndex = 1;
            this.cmbLab.SelectedIndexChanged += new System.EventHandler(this.cmbLab_SelectedIndexChanged);
            // 
            // lblLab
            // 
            this.lblLab.AutoSize = true;
            this.lblLab.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblLab.Location = new System.Drawing.Point(15, 26);
            this.lblLab.Name = "lblLab";
            this.lblLab.Size = new System.Drawing.Size(69, 15);
            this.lblLab.TabIndex = 0;
            this.lblLab.Text = "Lokasi LAB:";
            // 
            // grpBatch
            // 
            this.grpBatch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.grpBatch.Controls.Add(this.btnRefreshInterface);
            this.grpBatch.Controls.Add(this.cmbInterface);
            this.grpBatch.Controls.Add(this.lblInterface);
            this.grpBatch.Controls.Add(this.btnPushConfig);
            this.grpBatch.Controls.Add(this.chkRestart);
            this.grpBatch.Controls.Add(this.chkDisableLogon);
            this.grpBatch.Controls.Add(this.chkAutoLogon);
            this.grpBatch.Controls.Add(this.txtNewPass);
            this.grpBatch.Controls.Add(this.lblPass);
            this.grpBatch.Controls.Add(this.chkChangePass);
            this.grpBatch.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.grpBatch.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.grpBatch.Location = new System.Drawing.Point(474, 6);
            this.grpBatch.Name = "grpBatch";
            this.grpBatch.Size = new System.Drawing.Size(480, 195);
            this.grpBatch.TabIndex = 1;
            this.grpBatch.TabStop = false;
            this.grpBatch.Text = "Opsi Konfigurasi Masal & Remote Action";
            // 
            // btnRefreshInterface
            // 
            this.btnRefreshInterface.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.btnRefreshInterface.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRefreshInterface.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefreshInterface.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnRefreshInterface.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.btnRefreshInterface.Location = new System.Drawing.Point(427, 51);
            this.btnRefreshInterface.Name = "btnRefreshInterface";
            this.btnRefreshInterface.Size = new System.Drawing.Size(33, 25);
            this.btnRefreshInterface.TabIndex = 9;
            this.btnRefreshInterface.Text = "🔄";
            this.btnRefreshInterface.UseVisualStyleBackColor = false;
            this.btnRefreshInterface.Click += new System.EventHandler(this.btnRefreshInterface_Click);
            // 
            // cmbInterface
            // 
            this.cmbInterface.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbInterface.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cmbInterface.FormattingEnabled = true;
            this.cmbInterface.Items.AddRange(new object[] {
            "[Auto-Detect LAN (Active)]",
            "Ethernet",
            "Local Area Connection",
            "Wi-Fi"});
            this.cmbInterface.Location = new System.Drawing.Point(248, 52);
            this.cmbInterface.Name = "cmbInterface";
            this.cmbInterface.Size = new System.Drawing.Size(175, 23);
            this.cmbInterface.TabIndex = 8;
            // 
            // lblInterface
            // 
            this.lblInterface.AutoSize = true;
            this.lblInterface.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.lblInterface.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(118)))), ((int)(((byte)(110)))));
            this.lblInterface.Location = new System.Drawing.Point(245, 29);
            this.lblInterface.Name = "lblInterface";
            this.lblInterface.Size = new System.Drawing.Size(94, 15);
            this.lblInterface.TabIndex = 7;
            this.lblInterface.Text = "Target Adapter:";
            // 
            // btnPushConfig
            // 
            this.btnPushConfig.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(118)))), ((int)(((byte)(110)))));
            this.btnPushConfig.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPushConfig.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnPushConfig.ForeColor = System.Drawing.Color.White;
            this.btnPushConfig.Location = new System.Drawing.Point(20, 148);
            this.btnPushConfig.Name = "btnPushConfig";
            this.btnPushConfig.Size = new System.Drawing.Size(440, 36);
            this.btnPushConfig.TabIndex = 6;
            this.btnPushConfig.Text = "⚡ Push Configuration ke Seluruh PC Terpilih";
            this.btnPushConfig.UseVisualStyleBackColor = false;
            this.btnPushConfig.Click += new System.EventHandler(this.btnPushConfig_Click);
            // 
            // chkRestart
            // 
            this.chkRestart.AutoSize = true;
            this.chkRestart.Checked = true;
            this.chkRestart.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkRestart.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.chkRestart.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
            this.chkRestart.Location = new System.Drawing.Point(20, 118);
            this.chkRestart.Name = "chkRestart";
            this.chkRestart.Size = new System.Drawing.Size(266, 19);
            this.chkRestart.TabIndex = 5;
            this.chkRestart.Text = "Auto-Restart PC Client Setelah Selesai Push";
            this.chkRestart.UseVisualStyleBackColor = true;
            // 
            // chkDisableLogon
            // 
            this.chkDisableLogon.AutoSize = true;
            this.chkDisableLogon.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.chkDisableLogon.Location = new System.Drawing.Point(200, 88);
            this.chkDisableLogon.Name = "chkDisableLogon";
            this.chkDisableLogon.Size = new System.Drawing.Size(137, 19);
            this.chkDisableLogon.TabIndex = 4;
            this.chkDisableLogon.Text = "Matikan Auto-Logon";
            this.chkDisableLogon.UseVisualStyleBackColor = true;
            this.chkDisableLogon.CheckedChanged += new System.EventHandler(this.chkDisableLogon_CheckedChanged);
            // 
            // chkAutoLogon
            // 
            this.chkAutoLogon.AutoSize = true;
            this.chkAutoLogon.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.chkAutoLogon.Location = new System.Drawing.Point(20, 88);
            this.chkAutoLogon.Name = "chkAutoLogon";
            this.chkAutoLogon.Size = new System.Drawing.Size(172, 19);
            this.chkAutoLogon.TabIndex = 3;
            this.chkAutoLogon.Text = "Aktifkan Auto-Logon Client";
            this.chkAutoLogon.UseVisualStyleBackColor = true;
            this.chkAutoLogon.CheckedChanged += new System.EventHandler(this.chkAutoLogon_CheckedChanged);
            // 
            // txtNewPass
            // 
            this.txtNewPass.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtNewPass.Location = new System.Drawing.Point(115, 52);
            this.txtNewPass.Name = "txtNewPass";
            this.txtNewPass.Size = new System.Drawing.Size(120, 23);
            this.txtNewPass.TabIndex = 2;
            // 
            // lblPass
            // 
            this.lblPass.AutoSize = true;
            this.lblPass.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblPass.Location = new System.Drawing.Point(20, 56);
            this.lblPass.Name = "lblPass";
            this.lblPass.Size = new System.Drawing.Size(87, 15);
            this.lblPass.TabIndex = 1;
            this.lblPass.Text = "Password Baru:";
            // 
            // chkChangePass
            // 
            this.chkChangePass.AutoSize = true;
            this.chkChangePass.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkChangePass.Location = new System.Drawing.Point(20, 28);
            this.chkChangePass.Name = "chkChangePass";
            this.chkChangePass.Size = new System.Drawing.Size(159, 19);
            this.chkChangePass.TabIndex = 0;
            this.chkChangePass.Text = "Ubah Password Windows";
            this.chkChangePass.UseVisualStyleBackColor = true;
            this.chkChangePass.CheckedChanged += new System.EventHandler(this.chkChangePass_CheckedChanged);
            // 
            // grpGrid
            // 
            this.grpGrid.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.grpGrid.Controls.Add(this.btnDirectAdd);
            this.grpGrid.Controls.Add(this.txtDirectIp);
            this.grpGrid.Controls.Add(this.dgvClients);
            this.grpGrid.Controls.Add(this.lblTotalOnline);
            this.grpGrid.Controls.Add(this.chkSelectAll);
            this.grpGrid.Controls.Add(this.btnScan);
            this.grpGrid.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.grpGrid.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.grpGrid.Location = new System.Drawing.Point(6, 207);
            this.grpGrid.Name = "grpGrid";
            this.grpGrid.Size = new System.Drawing.Size(948, 265);
            this.grpGrid.TabIndex = 2;
            this.grpGrid.TabStop = false;
            this.grpGrid.Text = "Daftar Komputer Terdeteksi (Live Dashboard)";
            // 
            // btnDirectAdd
            // 
            this.btnDirectAdd.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.btnDirectAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDirectAdd.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.btnDirectAdd.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(118)))), ((int)(((byte)(110)))));
            this.btnDirectAdd.Location = new System.Drawing.Point(340, 24);
            this.btnDirectAdd.Name = "btnDirectAdd";
            this.btnDirectAdd.Size = new System.Drawing.Size(115, 27);
            this.btnDirectAdd.TabIndex = 6;
            this.btnDirectAdd.Text = "+ Ping / Add IP";
            this.btnDirectAdd.UseVisualStyleBackColor = false;
            this.btnDirectAdd.Click += new System.EventHandler(this.btnDirectAdd_Click);
            // 
            // txtDirectIp
            // 
            this.txtDirectIp.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtDirectIp.Location = new System.Drawing.Point(186, 26);
            this.txtDirectIp.Name = "txtDirectIp";
            this.txtDirectIp.Size = new System.Drawing.Size(145, 23);
            this.txtDirectIp.TabIndex = 5;
            // 
            // dgvClients
            // 
            this.dgvClients.AllowUserToAddRows = false;
            this.dgvClients.AllowUserToDeleteRows = false;
            this.dgvClients.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.dgvClients.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvClients.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvClients.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colCheck,
            this.colNumber,
            this.colHostname,
            this.colIp,
            this.colTargetIp,
            this.colTargetHostname,
            this.colStatus});
            this.dgvClients.Location = new System.Drawing.Point(18, 60);
            this.dgvClients.Name = "dgvClients";
            this.dgvClients.RowHeadersVisible = false;
            this.dgvClients.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvClients.Size = new System.Drawing.Size(912, 192);
            this.dgvClients.TabIndex = 3;
            // 
            // colCheck
            // 
            this.colCheck.HeaderText = "Pilih";
            this.colCheck.Name = "colCheck";
            this.colCheck.Width = 50;
            // 
            // colNumber
            // 
            this.colNumber.HeaderText = "No PC";
            this.colNumber.Name = "colNumber";
            this.colNumber.Width = 65;
            // 
            // colHostname
            // 
            this.colHostname.HeaderText = "Hostname Saat Ini";
            this.colHostname.Name = "colHostname";
            this.colHostname.ReadOnly = true;
            this.colHostname.Width = 160;
            // 
            // colIp
            // 
            this.colIp.HeaderText = "IP Address Saat Ini";
            this.colIp.Name = "colIp";
            this.colIp.ReadOnly = true;
            this.colIp.Width = 150;
            // 
            // colTargetIp
            // 
            this.colTargetIp.HeaderText = "Target IP Baru";
            this.colTargetIp.Name = "colTargetIp";
            this.colTargetIp.ReadOnly = true;
            this.colTargetIp.Width = 150;
            // 
            // colTargetHostname
            // 
            this.colTargetHostname.HeaderText = "Target Hostname";
            this.colTargetHostname.Name = "colTargetHostname";
            this.colTargetHostname.ReadOnly = true;
            this.colTargetHostname.Width = 160;
            // 
            // colStatus
            // 
            this.colStatus.HeaderText = "Status";
            this.colStatus.Name = "colStatus";
            this.colStatus.ReadOnly = true;
            this.colStatus.Width = 150;
            // 
            // lblTotalOnline
            // 
            this.lblTotalOnline.AutoSize = true;
            this.lblTotalOnline.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblTotalOnline.ForeColor = System.Drawing.Color.DarkGreen;
            this.lblTotalOnline.Location = new System.Drawing.Point(750, 28);
            this.lblTotalOnline.Name = "lblTotalOnline";
            this.lblTotalOnline.Size = new System.Drawing.Size(141, 17);
            this.lblTotalOnline.TabIndex = 2;
            this.lblTotalOnline.Text = "Total PC Terdeteksi: 0";
            // 
            // chkSelectAll
            // 
            this.chkSelectAll.AutoSize = true;
            this.chkSelectAll.Checked = true;
            this.chkSelectAll.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSelectAll.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.chkSelectAll.Location = new System.Drawing.Point(475, 28);
            this.chkSelectAll.Name = "chkSelectAll";
            this.chkSelectAll.Size = new System.Drawing.Size(134, 19);
            this.chkSelectAll.TabIndex = 1;
            this.chkSelectAll.Text = "Pilih Semua PC ([✓])";
            this.chkSelectAll.UseVisualStyleBackColor = true;
            this.chkSelectAll.CheckedChanged += new System.EventHandler(this.chkSelectAll_CheckedChanged);
            // 
            // btnScan
            // 
            this.btnScan.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(118)))), ((int)(((byte)(110)))));
            this.btnScan.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnScan.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnScan.ForeColor = System.Drawing.Color.White;
            this.btnScan.Location = new System.Drawing.Point(18, 22);
            this.btnScan.Name = "btnScan";
            this.btnScan.Size = new System.Drawing.Size(160, 30);
            this.btnScan.TabIndex = 0;
            this.btnScan.Text = "🔍 Scan Otomatis (Auto)";
            this.btnScan.UseVisualStyleBackColor = false;
            this.btnScan.Click += new System.EventHandler(this.btnScan_Click);
            // 
            // grpLog
            // 
            this.grpLog.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.grpLog.Controls.Add(this.txtLog);
            this.grpLog.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.grpLog.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.grpLog.Location = new System.Drawing.Point(12, 570);
            this.grpLog.Name = "grpLog";
            this.grpLog.Size = new System.Drawing.Size(960, 95);
            this.grpLog.TabIndex = 4;
            this.grpLog.TabStop = false;
            this.grpLog.Text = "Activity Log Controller";
            // 
            // txtLog
            // 
            this.txtLog.BackColor = System.Drawing.Color.Black;
            this.txtLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtLog.Font = new System.Drawing.Font("Consolas", 9F);
            this.txtLog.ForeColor = System.Drawing.Color.LightYellow;
            this.txtLog.Location = new System.Drawing.Point(14, 20);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(932, 65);
            this.txtLog.TabIndex = 0;
            // 
            // lblCopyright
            // 
            this.lblCopyright.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblCopyright.Font = new System.Drawing.Font("Calibri", 10F);
            this.lblCopyright.ForeColor = System.Drawing.Color.Black;
            this.lblCopyright.Location = new System.Drawing.Point(0, 658);
            this.lblCopyright.Name = "lblCopyright";
            this.lblCopyright.Size = new System.Drawing.Size(984, 32);
            this.lblCopyright.TabIndex = 5;
            this.lblCopyright.Text = "Developed By Ravenusa | Student ID 20.11.3623 | UPT Lab Amikom Yogyakarta";
            this.lblCopyright.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tabMain
            // 
            this.tabMain.Controls.Add(this.tabNetwork);
            this.tabMain.Controls.Add(this.tabShield);
            this.tabMain.Controls.Add(this.tabPower);
            this.tabMain.Controls.Add(this.tabQuickLaunch);
            this.tabMain.Controls.Add(this.tabFileManager);
            this.tabMain.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.tabMain.Location = new System.Drawing.Point(8, 58);
            this.tabMain.Name = "tabMain";
            this.tabMain.SelectedIndex = 0;
            this.tabMain.Size = new System.Drawing.Size(968, 508);
            this.tabMain.TabIndex = 1;
            // 
            // tabNetwork
            // 
            this.tabNetwork.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.tabNetwork.Controls.Add(this.grpConfig);
            this.tabNetwork.Controls.Add(this.grpBatch);
            this.tabNetwork.Controls.Add(this.grpGrid);
            this.tabNetwork.Location = new System.Drawing.Point(4, 26);
            this.tabNetwork.Name = "tabNetwork";
            this.tabNetwork.Padding = new System.Windows.Forms.Padding(3);
            this.tabNetwork.Size = new System.Drawing.Size(960, 478);
            this.tabNetwork.TabIndex = 0;
            this.tabNetwork.Text = " 🌐 Konfigurasi Jaringan & PC ";
            // 
            // tabShield
            // 
            this.tabShield.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.tabShield.Controls.Add(this.grpShieldAction);
            this.tabShield.Controls.Add(this.grpShieldGrid);
            this.tabShield.Location = new System.Drawing.Point(4, 26);
            this.tabShield.Name = "tabShield";
            this.tabShield.Padding = new System.Windows.Forms.Padding(3);
            this.tabShield.Size = new System.Drawing.Size(960, 478);
            this.tabShield.TabIndex = 1;
            this.tabShield.Text = " 🛡️ DeepFry (UWF) ";
            // 
            // grpShieldAction
            // 
            this.grpShieldAction.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.grpShieldAction.Controls.Add(this.btnShieldRestart);
            this.grpShieldAction.Controls.Add(this.btnUwfUninstall);
            this.grpShieldAction.Controls.Add(this.btnUwfInstall);
            this.grpShieldAction.Controls.Add(this.btnShieldUnlock);
            this.grpShieldAction.Controls.Add(this.btnShieldLock);
            this.grpShieldAction.Controls.Add(this.txtDfcPassword);
            this.grpShieldAction.Controls.Add(this.lblDfcPass);
            this.grpShieldAction.Controls.Add(this.lblShieldInfo);
            this.grpShieldAction.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.grpShieldAction.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.grpShieldAction.Location = new System.Drawing.Point(6, 6);
            this.grpShieldAction.Name = "grpShieldAction";
            this.grpShieldAction.Size = new System.Drawing.Size(948, 115);
            this.grpShieldAction.TabIndex = 0;
            this.grpShieldAction.TabStop = false;
            this.grpShieldAction.Text = "Kontrol Proteksi Disk (Microsoft Unified Write Filter - UWF)";
            // 
            // btnShieldRestart
            // 
            this.btnShieldRestart.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(119)))), ((int)(((byte)(6)))));
            this.btnShieldRestart.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnShieldRestart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnShieldRestart.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnShieldRestart.ForeColor = System.Drawing.Color.White;
            this.btnShieldRestart.Location = new System.Drawing.Point(758, 55);
            this.btnShieldRestart.Name = "btnShieldRestart";
            this.btnShieldRestart.Size = new System.Drawing.Size(175, 38);
            this.btnShieldRestart.TabIndex = 7;
            this.btnShieldRestart.Text = "🔄 Restart PC";
            this.btnShieldRestart.UseVisualStyleBackColor = false;
            this.btnShieldRestart.Click += new System.EventHandler(this.btnShieldRestart_Click);
            // 
            // btnUwfUninstall
            // 
            this.btnUwfUninstall.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.btnUwfUninstall.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnUwfUninstall.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUwfUninstall.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnUwfUninstall.ForeColor = System.Drawing.Color.White;
            this.btnUwfUninstall.Location = new System.Drawing.Point(573, 55);
            this.btnUwfUninstall.Name = "btnUwfUninstall";
            this.btnUwfUninstall.Size = new System.Drawing.Size(175, 38);
            this.btnUwfUninstall.TabIndex = 6;
            this.btnUwfUninstall.Text = "🗑️ Uninstall UWF";
            this.btnUwfUninstall.UseVisualStyleBackColor = false;
            this.btnUwfUninstall.Click += new System.EventHandler(this.btnUwfUninstall_Click);
            // 
            // btnUwfInstall
            // 
            this.btnUwfInstall.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.btnUwfInstall.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnUwfInstall.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUwfInstall.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnUwfInstall.ForeColor = System.Drawing.Color.White;
            this.btnUwfInstall.Location = new System.Drawing.Point(388, 55);
            this.btnUwfInstall.Name = "btnUwfInstall";
            this.btnUwfInstall.Size = new System.Drawing.Size(175, 38);
            this.btnUwfInstall.TabIndex = 5;
            this.btnUwfInstall.Text = "⚙️ Install UWF";
            this.btnUwfInstall.UseVisualStyleBackColor = false;
            this.btnUwfInstall.Click += new System.EventHandler(this.btnUwfInstall_Click);
            // 
            // btnShieldUnlock
            // 
            this.btnShieldUnlock.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(184)))), ((int)(((byte)(166)))));
            this.btnShieldUnlock.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnShieldUnlock.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnShieldUnlock.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnShieldUnlock.ForeColor = System.Drawing.Color.White;
            this.btnShieldUnlock.Location = new System.Drawing.Point(203, 55);
            this.btnShieldUnlock.Name = "btnShieldUnlock";
            this.btnShieldUnlock.Size = new System.Drawing.Size(175, 38);
            this.btnShieldUnlock.TabIndex = 4;
            this.btnShieldUnlock.Text = "🔓 Unlock Disk (Thaw C:)";
            this.btnShieldUnlock.UseVisualStyleBackColor = false;
            this.btnShieldUnlock.Click += new System.EventHandler(this.btnShieldUnlock_Click);
            // 
            // btnShieldLock
            // 
            this.btnShieldLock.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
            this.btnShieldLock.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnShieldLock.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnShieldLock.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnShieldLock.ForeColor = System.Drawing.Color.White;
            this.btnShieldLock.Location = new System.Drawing.Point(18, 55);
            this.btnShieldLock.Name = "btnShieldLock";
            this.btnShieldLock.Size = new System.Drawing.Size(175, 38);
            this.btnShieldLock.TabIndex = 3;
            this.btnShieldLock.Text = "🔒 Lock Disk (Protect C:)";
            this.btnShieldLock.UseVisualStyleBackColor = false;
            this.btnShieldLock.Click += new System.EventHandler(this.btnShieldLock_Click);
            // 
            // txtDfcPassword
            // 
            this.txtDfcPassword.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtDfcPassword.Location = new System.Drawing.Point(206, 54);
            this.txtDfcPassword.Name = "txtDfcPassword";
            this.txtDfcPassword.PasswordChar = '●';
            this.txtDfcPassword.Size = new System.Drawing.Size(195, 23);
            this.txtDfcPassword.TabIndex = 2;
            this.txtDfcPassword.Visible = false;
            // 
            // lblDfcPass
            // 
            this.lblDfcPass.AutoSize = true;
            this.lblDfcPass.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.lblDfcPass.Location = new System.Drawing.Point(15, 58);
            this.lblDfcPass.Name = "lblDfcPass";
            this.lblDfcPass.Size = new System.Drawing.Size(136, 15);
            this.lblDfcPass.TabIndex = 1;
            this.lblDfcPass.Text = "Password Deep Freeze:";
            this.lblDfcPass.Visible = false;
            // 
            // lblShieldInfo
            // 
            this.lblShieldInfo.AutoSize = true;
            this.lblShieldInfo.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblShieldInfo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblShieldInfo.Location = new System.Drawing.Point(15, 25);
            this.lblShieldInfo.Name = "lblShieldInfo";
            this.lblShieldInfo.Size = new System.Drawing.Size(812, 15);
            this.lblShieldInfo.TabIndex = 0;
            this.lblShieldInfo.Text = "💡 Many thanks to Gusti Padaka. This project (UWF), which is a further developmen" +
    "t of https://github.com/GPadaka19/DeepFry | 22.11.5020 | 20.11.3623";
            // 
            // grpShieldGrid
            // 
            this.grpShieldGrid.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.grpShieldGrid.Controls.Add(this.btnShieldDirectAdd);
            this.grpShieldGrid.Controls.Add(this.txtShieldDirectIp);
            this.grpShieldGrid.Controls.Add(this.dgvShieldClients);
            this.grpShieldGrid.Controls.Add(this.lblShieldTotal);
            this.grpShieldGrid.Controls.Add(this.chkShieldSelectAll);
            this.grpShieldGrid.Controls.Add(this.btnShieldScan);
            this.grpShieldGrid.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.grpShieldGrid.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.grpShieldGrid.Location = new System.Drawing.Point(6, 137);
            this.grpShieldGrid.Name = "grpShieldGrid";
            this.grpShieldGrid.Size = new System.Drawing.Size(948, 335);
            this.grpShieldGrid.TabIndex = 1;
            this.grpShieldGrid.TabStop = false;
            this.grpShieldGrid.Text = "Status Komputer & Proteksi Disk (Live Dashboard)";
            // 
            // btnShieldDirectAdd
            // 
            this.btnShieldDirectAdd.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.btnShieldDirectAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnShieldDirectAdd.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.btnShieldDirectAdd.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(118)))), ((int)(((byte)(110)))));
            this.btnShieldDirectAdd.Location = new System.Drawing.Point(340, 24);
            this.btnShieldDirectAdd.Name = "btnShieldDirectAdd";
            this.btnShieldDirectAdd.Size = new System.Drawing.Size(115, 27);
            this.btnShieldDirectAdd.TabIndex = 5;
            this.btnShieldDirectAdd.Text = "+ Ping / Add IP";
            this.btnShieldDirectAdd.UseVisualStyleBackColor = false;
            this.btnShieldDirectAdd.Click += new System.EventHandler(this.btnShieldDirectAdd_Click);
            // 
            // txtShieldDirectIp
            // 
            this.txtShieldDirectIp.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtShieldDirectIp.Location = new System.Drawing.Point(186, 26);
            this.txtShieldDirectIp.Name = "txtShieldDirectIp";
            this.txtShieldDirectIp.Size = new System.Drawing.Size(145, 23);
            this.txtShieldDirectIp.TabIndex = 4;
            // 
            // dgvShieldClients
            // 
            this.dgvShieldClients.AllowUserToAddRows = false;
            this.dgvShieldClients.AllowUserToDeleteRows = false;
            this.dgvShieldClients.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.dgvShieldClients.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvShieldClients.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvShieldClients.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colShieldCheck,
            this.colShieldNumber,
            this.colShieldHostname,
            this.colShieldIp,
            this.colShieldOs,
            this.colShieldStatus,
            this.colShieldAction});
            this.dgvShieldClients.Location = new System.Drawing.Point(18, 60);
            this.dgvShieldClients.Name = "dgvShieldClients";
            this.dgvShieldClients.RowHeadersVisible = false;
            this.dgvShieldClients.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvShieldClients.Size = new System.Drawing.Size(912, 260);
            this.dgvShieldClients.TabIndex = 3;
            // 
            // colShieldCheck
            // 
            this.colShieldCheck.HeaderText = "Pilih";
            this.colShieldCheck.Name = "colShieldCheck";
            this.colShieldCheck.Width = 50;
            // 
            // colShieldNumber
            // 
            this.colShieldNumber.HeaderText = "No PC";
            this.colShieldNumber.Name = "colShieldNumber";
            this.colShieldNumber.Width = 65;
            // 
            // colShieldHostname
            // 
            this.colShieldHostname.HeaderText = "Hostname";
            this.colShieldHostname.Name = "colShieldHostname";
            this.colShieldHostname.ReadOnly = true;
            this.colShieldHostname.Width = 160;
            // 
            // colShieldIp
            // 
            this.colShieldIp.HeaderText = "IP Address";
            this.colShieldIp.Name = "colShieldIp";
            this.colShieldIp.ReadOnly = true;
            this.colShieldIp.Width = 140;
            // 
            // colShieldOs
            // 
            this.colShieldOs.HeaderText = "Edisi Windows (OS)";
            this.colShieldOs.Name = "colShieldOs";
            this.colShieldOs.ReadOnly = true;
            this.colShieldOs.Width = 180;
            // 
            // colShieldStatus
            // 
            this.colShieldStatus.HeaderText = "Status Proteksi (Disk Shield)";
            this.colShieldStatus.Name = "colShieldStatus";
            this.colShieldStatus.ReadOnly = true;
            this.colShieldStatus.Width = 180;
            // 
            // colShieldAction
            // 
            this.colShieldAction.HeaderText = "Action Status";
            this.colShieldAction.Name = "colShieldAction";
            this.colShieldAction.ReadOnly = true;
            this.colShieldAction.Width = 130;
            // 
            // lblShieldTotal
            // 
            this.lblShieldTotal.AutoSize = true;
            this.lblShieldTotal.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblShieldTotal.ForeColor = System.Drawing.Color.DarkGreen;
            this.lblShieldTotal.Location = new System.Drawing.Point(750, 28);
            this.lblShieldTotal.Name = "lblShieldTotal";
            this.lblShieldTotal.Size = new System.Drawing.Size(141, 17);
            this.lblShieldTotal.TabIndex = 2;
            this.lblShieldTotal.Text = "Total PC Terdeteksi: 0";
            // 
            // chkShieldSelectAll
            // 
            this.chkShieldSelectAll.AutoSize = true;
            this.chkShieldSelectAll.Checked = true;
            this.chkShieldSelectAll.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkShieldSelectAll.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.chkShieldSelectAll.Location = new System.Drawing.Point(475, 28);
            this.chkShieldSelectAll.Name = "chkShieldSelectAll";
            this.chkShieldSelectAll.Size = new System.Drawing.Size(134, 19);
            this.chkShieldSelectAll.TabIndex = 1;
            this.chkShieldSelectAll.Text = "Pilih Semua PC ([✓])";
            this.chkShieldSelectAll.UseVisualStyleBackColor = true;
            this.chkShieldSelectAll.CheckedChanged += new System.EventHandler(this.chkShieldSelectAll_CheckedChanged);
            // 
            // btnShieldScan
            // 
            this.btnShieldScan.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(118)))), ((int)(((byte)(110)))));
            this.btnShieldScan.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnShieldScan.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnShieldScan.ForeColor = System.Drawing.Color.White;
            this.btnShieldScan.Location = new System.Drawing.Point(18, 22);
            this.btnShieldScan.Name = "btnShieldScan";
            this.btnShieldScan.Size = new System.Drawing.Size(160, 30);
            this.btnShieldScan.TabIndex = 0;
            this.btnShieldScan.Text = "🔍 Scan Otomatis (Auto)";
            this.btnShieldScan.UseVisualStyleBackColor = false;
            this.btnShieldScan.Click += new System.EventHandler(this.btnScan_Click);
            // 
            // tabPower
            // 
            this.tabPower.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.tabPower.Controls.Add(this.grpDesktop);
            this.tabPower.Controls.Add(this.grpPower);
            this.tabPower.Controls.Add(this.grpLivePower);
            this.tabPower.Location = new System.Drawing.Point(4, 26);
            this.tabPower.Name = "tabPower";
            this.tabPower.Padding = new System.Windows.Forms.Padding(4);
            this.tabPower.Size = new System.Drawing.Size(960, 478);
            this.tabPower.TabIndex = 2;
            this.tabPower.Text = " ⚡ Power & Desktop ";
            // 
            // grpDesktop
            // 
            this.grpDesktop.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpDesktop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.grpDesktop.Controls.Add(this.lblDesktopDesc);
            this.grpDesktop.Controls.Add(this.tblDesktop);
            this.grpDesktop.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.grpDesktop.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.grpDesktop.Location = new System.Drawing.Point(6, 6);
            this.grpDesktop.Name = "grpDesktop";
            this.grpDesktop.Size = new System.Drawing.Size(948, 86);
            this.grpDesktop.TabIndex = 0;
            this.grpDesktop.TabStop = false;
            this.grpDesktop.Text = "Student Screen & Activity Control (Layar & Input Siswa)";
            // 
            // lblDesktopDesc
            // 
            this.lblDesktopDesc.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDesktopDesc.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblDesktopDesc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblDesktopDesc.Location = new System.Drawing.Point(15, 20);
            this.lblDesktopDesc.Name = "lblDesktopDesc";
            this.lblDesktopDesc.Size = new System.Drawing.Size(918, 18);
            this.lblDesktopDesc.TabIndex = 0;
            this.lblDesktopDesc.Text = "💡 Kunci Layar: Membekukan layar siswa dan memblokir total, keyboard serta mouse " +
    "secara langsung.";
            // 
            // tblDesktop
            // 
            this.tblDesktop.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tblDesktop.ColumnCount = 3;
            this.tblDesktop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.tblDesktop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.tblDesktop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.34F));
            this.tblDesktop.Controls.Add(this.btnLockScreen, 0, 0);
            this.tblDesktop.Controls.Add(this.btnUnlockScreen, 1, 0);
            this.tblDesktop.Controls.Add(this.btnClearDesktop, 2, 0);
            this.tblDesktop.Location = new System.Drawing.Point(15, 38);
            this.tblDesktop.Name = "tblDesktop";
            this.tblDesktop.RowCount = 1;
            this.tblDesktop.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblDesktop.Size = new System.Drawing.Size(918, 40);
            this.tblDesktop.TabIndex = 1;
            // 
            // btnLockScreen
            // 
            this.btnLockScreen.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
            this.btnLockScreen.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLockScreen.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnLockScreen.FlatAppearance.BorderSize = 0;
            this.btnLockScreen.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLockScreen.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnLockScreen.ForeColor = System.Drawing.Color.White;
            this.btnLockScreen.Location = new System.Drawing.Point(0, 0);
            this.btnLockScreen.Margin = new System.Windows.Forms.Padding(0, 0, 5, 0);
            this.btnLockScreen.Name = "btnLockScreen";
            this.btnLockScreen.Size = new System.Drawing.Size(300, 40);
            this.btnLockScreen.TabIndex = 0;
            this.btnLockScreen.Text = "🔒 Lock Screen && Input";
            this.btnLockScreen.UseVisualStyleBackColor = false;
            // 
            // btnUnlockScreen
            // 
            this.btnUnlockScreen.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(118)))), ((int)(((byte)(110)))));
            this.btnUnlockScreen.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnUnlockScreen.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnUnlockScreen.FlatAppearance.BorderSize = 0;
            this.btnUnlockScreen.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUnlockScreen.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnUnlockScreen.ForeColor = System.Drawing.Color.White;
            this.btnUnlockScreen.Location = new System.Drawing.Point(308, 0);
            this.btnUnlockScreen.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.btnUnlockScreen.Name = "btnUnlockScreen";
            this.btnUnlockScreen.Size = new System.Drawing.Size(299, 40);
            this.btnUnlockScreen.TabIndex = 1;
            this.btnUnlockScreen.Text = "🔓 Unlock Screen && Input";
            this.btnUnlockScreen.UseVisualStyleBackColor = false;
            // 
            // btnClearDesktop
            // 
            this.btnClearDesktop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.btnClearDesktop.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClearDesktop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnClearDesktop.FlatAppearance.BorderSize = 0;
            this.btnClearDesktop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClearDesktop.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnClearDesktop.ForeColor = System.Drawing.Color.White;
            this.btnClearDesktop.Location = new System.Drawing.Point(615, 0);
            this.btnClearDesktop.Margin = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.btnClearDesktop.Name = "btnClearDesktop";
            this.btnClearDesktop.Size = new System.Drawing.Size(303, 40);
            this.btnClearDesktop.TabIndex = 2;
            this.btnClearDesktop.Text = "🧹 Clear Students Desktop";
            this.btnClearDesktop.UseVisualStyleBackColor = false;
            // 
            // grpPower
            // 
            this.grpPower.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpPower.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.grpPower.Controls.Add(this.lblPowerDesc);
            this.grpPower.Controls.Add(this.tblPower);
            this.grpPower.Controls.Add(this.lblWolHint);
            this.grpPower.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.grpPower.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.grpPower.Location = new System.Drawing.Point(6, 94);
            this.grpPower.Name = "grpPower";
            this.grpPower.Size = new System.Drawing.Size(948, 114);
            this.grpPower.TabIndex = 1;
            this.grpPower.TabStop = false;
            this.grpPower.Text = "Remote Power Operations (Shutdown, Restart & Wake-on-LAN)";
            // 
            // lblPowerDesc
            // 
            this.lblPowerDesc.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPowerDesc.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblPowerDesc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblPowerDesc.Location = new System.Drawing.Point(15, 18);
            this.lblPowerDesc.Name = "lblPowerDesc";
            this.lblPowerDesc.Size = new System.Drawing.Size(918, 18);
            this.lblPowerDesc.TabIndex = 0;
            this.lblPowerDesc.Text = "⚡ Kelola power komputer lab secara bersamaan dari Controller. Shutdown/Restart ak" +
    "an dikirim ke PC yang dicentang di bawah.";
            // 
            // tblPower
            // 
            this.tblPower.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tblPower.ColumnCount = 3;
            this.tblPower.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.tblPower.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.tblPower.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.34F));
            this.tblPower.Controls.Add(this.btnShutdown, 0, 0);
            this.tblPower.Controls.Add(this.btnRestart, 1, 0);
            this.tblPower.Controls.Add(this.btnWakeOnLan, 2, 0);
            this.tblPower.Location = new System.Drawing.Point(15, 38);
            this.tblPower.Name = "tblPower";
            this.tblPower.RowCount = 1;
            this.tblPower.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblPower.Size = new System.Drawing.Size(918, 42);
            this.tblPower.TabIndex = 1;
            // 
            // btnShutdown
            // 
            this.btnShutdown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
            this.btnShutdown.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnShutdown.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnShutdown.FlatAppearance.BorderSize = 0;
            this.btnShutdown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnShutdown.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnShutdown.ForeColor = System.Drawing.Color.White;
            this.btnShutdown.Location = new System.Drawing.Point(0, 0);
            this.btnShutdown.Margin = new System.Windows.Forms.Padding(0, 0, 5, 0);
            this.btnShutdown.Name = "btnShutdown";
            this.btnShutdown.Size = new System.Drawing.Size(300, 42);
            this.btnShutdown.TabIndex = 0;
            this.btnShutdown.Text = "🛑 Shutdown (PC Terpilih)";
            this.btnShutdown.UseVisualStyleBackColor = false;
            // 
            // btnRestart
            // 
            this.btnRestart.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(119)))), ((int)(((byte)(6)))));
            this.btnRestart.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRestart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnRestart.FlatAppearance.BorderSize = 0;
            this.btnRestart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRestart.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnRestart.ForeColor = System.Drawing.Color.White;
            this.btnRestart.Location = new System.Drawing.Point(308, 0);
            this.btnRestart.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.btnRestart.Name = "btnRestart";
            this.btnRestart.Size = new System.Drawing.Size(299, 42);
            this.btnRestart.TabIndex = 1;
            this.btnRestart.Text = "🔄 Restart (PC Terpilih)";
            this.btnRestart.UseVisualStyleBackColor = false;
            // 
            // btnWakeOnLan
            // 
            this.btnWakeOnLan.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(118)))), ((int)(((byte)(110)))));
            this.btnWakeOnLan.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnWakeOnLan.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnWakeOnLan.FlatAppearance.BorderSize = 0;
            this.btnWakeOnLan.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnWakeOnLan.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnWakeOnLan.ForeColor = System.Drawing.Color.White;
            this.btnWakeOnLan.Location = new System.Drawing.Point(615, 0);
            this.btnWakeOnLan.Margin = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.btnWakeOnLan.Name = "btnWakeOnLan";
            this.btnWakeOnLan.Size = new System.Drawing.Size(303, 42);
            this.btnWakeOnLan.TabIndex = 2;
            this.btnWakeOnLan.Text = "⚡ Power On";
            this.btnWakeOnLan.UseVisualStyleBackColor = false;
            // 
            // lblWolHint
            // 
            this.lblWolHint.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblWolHint.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Italic);
            this.lblWolHint.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.lblWolHint.Location = new System.Drawing.Point(15, 87);
            this.lblWolHint.Name = "lblWolHint";
            this.lblWolHint.Size = new System.Drawing.Size(918, 18);
            this.lblWolHint.TabIndex = 2;
            this.lblWolHint.Text = "ℹ️ Catatan WOL: Pastikan fitur Wake-on-LAN / PCIe Wake diaktifkan pada BIOS/UEFI " +
    "dan Network Adapter Client.";
            // 
            // grpLivePower
            // 
            this.grpLivePower.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpLivePower.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.grpLivePower.Controls.Add(this.btnPowerScan);
            this.grpLivePower.Controls.Add(this.chkPowerSelectAll);
            this.grpLivePower.Controls.Add(this.lblPowerTotal);
            this.grpLivePower.Controls.Add(this.dgvPowerClients);
            this.grpLivePower.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.grpLivePower.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.grpLivePower.Location = new System.Drawing.Point(6, 214);
            this.grpLivePower.Name = "grpLivePower";
            this.grpLivePower.Size = new System.Drawing.Size(948, 258);
            this.grpLivePower.TabIndex = 2;
            this.grpLivePower.TabStop = false;
            this.grpLivePower.Text = "Daftar Komputer(Live Dashboard)";
            // 
            // btnPowerScan
            // 
            this.btnPowerScan.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(118)))), ((int)(((byte)(110)))));
            this.btnPowerScan.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPowerScan.FlatAppearance.BorderSize = 0;
            this.btnPowerScan.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPowerScan.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnPowerScan.ForeColor = System.Drawing.Color.White;
            this.btnPowerScan.Location = new System.Drawing.Point(18, 22);
            this.btnPowerScan.Name = "btnPowerScan";
            this.btnPowerScan.Size = new System.Drawing.Size(160, 30);
            this.btnPowerScan.TabIndex = 0;
            this.btnPowerScan.Text = "🔍 Scan Otomatis (Auto)";
            this.btnPowerScan.UseVisualStyleBackColor = false;
            // 
            // chkPowerSelectAll
            // 
            this.chkPowerSelectAll.AutoSize = true;
            this.chkPowerSelectAll.Checked = true;
            this.chkPowerSelectAll.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkPowerSelectAll.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.chkPowerSelectAll.Location = new System.Drawing.Point(200, 28);
            this.chkPowerSelectAll.Name = "chkPowerSelectAll";
            this.chkPowerSelectAll.Size = new System.Drawing.Size(134, 19);
            this.chkPowerSelectAll.TabIndex = 1;
            this.chkPowerSelectAll.Text = "Pilih Semua PC ([✓])";
            this.chkPowerSelectAll.UseVisualStyleBackColor = true;
            // 
            // lblPowerTotal
            // 
            this.lblPowerTotal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPowerTotal.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblPowerTotal.ForeColor = System.Drawing.Color.DarkGreen;
            this.lblPowerTotal.Location = new System.Drawing.Point(733, 26);
            this.lblPowerTotal.Name = "lblPowerTotal";
            this.lblPowerTotal.Size = new System.Drawing.Size(200, 20);
            this.lblPowerTotal.TabIndex = 2;
            this.lblPowerTotal.Text = "Total PC Terdeteksi: 0";
            this.lblPowerTotal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // dgvPowerClients
            // 
            this.dgvPowerClients.AllowUserToAddRows = false;
            this.dgvPowerClients.AllowUserToDeleteRows = false;
            this.dgvPowerClients.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvPowerClients.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.dgvPowerClients.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvPowerClients.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPowerClients.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colPowerCheck,
            this.colPowerNumber,
            this.colPowerHostname,
            this.colPowerIp,
            this.colPowerStatus});
            this.dgvPowerClients.EnableHeadersVisualStyles = false;
            this.dgvPowerClients.Location = new System.Drawing.Point(15, 56);
            this.dgvPowerClients.Name = "dgvPowerClients";
            this.dgvPowerClients.RowHeadersVisible = false;
            this.dgvPowerClients.RowTemplate.Height = 28;
            this.dgvPowerClients.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvPowerClients.Size = new System.Drawing.Size(918, 187);
            this.dgvPowerClients.TabIndex = 3;
            // 
            // colPowerCheck
            // 
            this.colPowerCheck.HeaderText = "Pilih";
            this.colPowerCheck.Name = "colPowerCheck";
            this.colPowerCheck.Width = 50;
            // 
            // colPowerNumber
            // 
            this.colPowerNumber.HeaderText = "No PC";
            this.colPowerNumber.Name = "colPowerNumber";
            this.colPowerNumber.ReadOnly = true;
            this.colPowerNumber.Width = 65;
            // 
            // colPowerHostname
            // 
            this.colPowerHostname.HeaderText = "Hostname";
            this.colPowerHostname.Name = "colPowerHostname";
            this.colPowerHostname.ReadOnly = true;
            this.colPowerHostname.Width = 220;
            // 
            // colPowerIp
            // 
            this.colPowerIp.HeaderText = "IP Address";
            this.colPowerIp.Name = "colPowerIp";
            this.colPowerIp.ReadOnly = true;
            this.colPowerIp.Width = 160;
            // 
            // colPowerStatus
            // 
            this.colPowerStatus.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colPowerStatus.HeaderText = "Status";
            this.colPowerStatus.Name = "colPowerStatus";
            this.colPowerStatus.ReadOnly = true;
            // 
            // tabQuickLaunch
            // 
            this.tabQuickLaunch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.tabQuickLaunch.Controls.Add(this.grpPresets);
            this.tabQuickLaunch.Controls.Add(this.grpCustom);
            this.tabQuickLaunch.Controls.Add(this.grpLiveLaunch);
            this.tabQuickLaunch.Location = new System.Drawing.Point(4, 26);
            this.tabQuickLaunch.Name = "tabQuickLaunch";
            this.tabQuickLaunch.Padding = new System.Windows.Forms.Padding(4);
            this.tabQuickLaunch.Size = new System.Drawing.Size(960, 478);
            this.tabQuickLaunch.TabIndex = 3;
            this.tabQuickLaunch.Text = " 🚀 Quick Launch ";
            // 
            // grpPresets
            // 
            this.grpPresets.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpPresets.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.grpPresets.Controls.Add(this.tblPresets);
            this.grpPresets.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.grpPresets.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.grpPresets.Location = new System.Drawing.Point(6, 6);
            this.grpPresets.Name = "grpPresets";
            this.grpPresets.Size = new System.Drawing.Size(948, 75);
            this.grpPresets.TabIndex = 0;
            this.grpPresets.TabStop = false;
            this.grpPresets.Text = "Preset Peluncur Cepat (Quick Shortcuts)";
            // 
            // tblPresets
            // 
            this.tblPresets.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tblPresets.ColumnCount = 4;
            this.tblPresets.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tblPresets.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tblPresets.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tblPresets.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tblPresets.Controls.Add(this.btnRemotePs, 0, 0);
            this.tblPresets.Controls.Add(this.btnEdge, 1, 0);
            this.tblPresets.Controls.Add(this.btnCmd, 2, 0);
            this.tblPresets.Controls.Add(this.btnNotepad, 3, 0);
            this.tblPresets.Location = new System.Drawing.Point(15, 23);
            this.tblPresets.Name = "tblPresets";
            this.tblPresets.RowCount = 1;
            this.tblPresets.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblPresets.Size = new System.Drawing.Size(918, 40);
            this.tblPresets.TabIndex = 0;
            // 
            // btnRemotePs
            // 
            this.btnRemotePs.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(118)))), ((int)(((byte)(110)))));
            this.btnRemotePs.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRemotePs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnRemotePs.FlatAppearance.BorderSize = 0;
            this.btnRemotePs.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRemotePs.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnRemotePs.ForeColor = System.Drawing.Color.White;
            this.btnRemotePs.Location = new System.Drawing.Point(0, 0);
            this.btnRemotePs.Margin = new System.Windows.Forms.Padding(0, 0, 4, 0);
            this.btnRemotePs.Name = "btnRemotePs";
            this.btnRemotePs.Size = new System.Drawing.Size(225, 40);
            this.btnRemotePs.TabIndex = 0;
            this.btnRemotePs.Text = "⚡ Remote PowerShell";
            this.btnRemotePs.UseVisualStyleBackColor = false;
            // 
            // btnEdge
            // 
            this.btnEdge.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.btnEdge.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEdge.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnEdge.FlatAppearance.BorderSize = 0;
            this.btnEdge.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEdge.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnEdge.ForeColor = System.Drawing.Color.White;
            this.btnEdge.Location = new System.Drawing.Point(232, 0);
            this.btnEdge.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.btnEdge.Name = "btnEdge";
            this.btnEdge.Size = new System.Drawing.Size(223, 40);
            this.btnEdge.TabIndex = 1;
            this.btnEdge.Text = "🌍 Microsoft Edge";
            this.btnEdge.UseVisualStyleBackColor = false;
            // 
            // btnCmd
            // 
            this.btnCmd.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.btnCmd.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCmd.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnCmd.FlatAppearance.BorderSize = 0;
            this.btnCmd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCmd.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnCmd.ForeColor = System.Drawing.Color.White;
            this.btnCmd.Location = new System.Drawing.Point(461, 0);
            this.btnCmd.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.btnCmd.Name = "btnCmd";
            this.btnCmd.Size = new System.Drawing.Size(223, 40);
            this.btnCmd.TabIndex = 2;
            this.btnCmd.Text = "💻 CMD / Terminal";
            this.btnCmd.UseVisualStyleBackColor = false;
            // 
            // btnNotepad
            // 
            this.btnNotepad.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.btnNotepad.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNotepad.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnNotepad.FlatAppearance.BorderSize = 0;
            this.btnNotepad.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNotepad.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnNotepad.ForeColor = System.Drawing.Color.White;
            this.btnNotepad.Location = new System.Drawing.Point(691, 0);
            this.btnNotepad.Margin = new System.Windows.Forms.Padding(4, 0, 0, 0);
            this.btnNotepad.Name = "btnNotepad";
            this.btnNotepad.Size = new System.Drawing.Size(227, 40);
            this.btnNotepad.TabIndex = 3;
            this.btnNotepad.Text = "📝 Notepad";
            this.btnNotepad.UseVisualStyleBackColor = false;
            // 
            // grpCustom
            // 
            this.grpCustom.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpCustom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.grpCustom.Controls.Add(this.tblCustom);
            this.grpCustom.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.grpCustom.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.grpCustom.Location = new System.Drawing.Point(6, 86);
            this.grpCustom.Name = "grpCustom";
            this.grpCustom.Size = new System.Drawing.Size(948, 138);
            this.grpCustom.TabIndex = 1;
            this.grpCustom.TabStop = false;
            this.grpCustom.Text = "Luncurkan Aplikasi / URL Website / File Custom";
            // 
            // tblCustom
            // 
            this.tblCustom.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tblCustom.ColumnCount = 2;
            this.tblCustom.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblCustom.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 220F));
            this.tblCustom.Controls.Add(this.pnlInputs, 0, 0);
            this.tblCustom.Controls.Add(this.btnExecuteCustom, 1, 0);
            this.tblCustom.Location = new System.Drawing.Point(15, 20);
            this.tblCustom.Name = "tblCustom";
            this.tblCustom.RowCount = 1;
            this.tblCustom.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblCustom.Size = new System.Drawing.Size(918, 108);
            this.tblCustom.TabIndex = 0;
            // 
            // pnlInputs
            // 
            this.pnlInputs.Controls.Add(this.lblTarget);
            this.pnlInputs.Controls.Add(this.tblTargetRow);
            this.pnlInputs.Controls.Add(this.lblArgs);
            this.pnlInputs.Controls.Add(this.txtLaunchArgs);
            this.pnlInputs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlInputs.Location = new System.Drawing.Point(0, 0);
            this.pnlInputs.Margin = new System.Windows.Forms.Padding(0);
            this.pnlInputs.Name = "pnlInputs";
            this.pnlInputs.Size = new System.Drawing.Size(698, 108);
            this.pnlInputs.TabIndex = 0;
            // 
            // lblTarget
            // 
            this.lblTarget.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.lblTarget.Location = new System.Drawing.Point(0, 0);
            this.lblTarget.Name = "lblTarget";
            this.lblTarget.Size = new System.Drawing.Size(500, 18);
            this.lblTarget.TabIndex = 0;
            this.lblTarget.Text = "Target Eksekusi (Nama Aplikasi, Full Path .exe, atau Link URL Website):";
            // 
            // tblTargetRow
            // 
            this.tblTargetRow.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tblTargetRow.ColumnCount = 2;
            this.tblTargetRow.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblTargetRow.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 95F));
            this.tblTargetRow.Controls.Add(this.txtLaunchTarget, 0, 0);
            this.tblTargetRow.Controls.Add(this.btnBrowseLaunch, 1, 0);
            this.tblTargetRow.Location = new System.Drawing.Point(0, 20);
            this.tblTargetRow.Name = "tblTargetRow";
            this.tblTargetRow.RowCount = 1;
            this.tblTargetRow.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblTargetRow.Size = new System.Drawing.Size(698, 28);
            this.tblTargetRow.TabIndex = 1;
            // 
            // txtLaunchTarget
            // 
            this.txtLaunchTarget.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLaunchTarget.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.txtLaunchTarget.Location = new System.Drawing.Point(3, 3);
            this.txtLaunchTarget.Name = "txtLaunchTarget";
            this.txtLaunchTarget.Size = new System.Drawing.Size(597, 24);
            this.txtLaunchTarget.TabIndex = 0;
            // 
            // btnBrowseLaunch
            // 
            this.btnBrowseLaunch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.btnBrowseLaunch.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBrowseLaunch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnBrowseLaunch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBrowseLaunch.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.btnBrowseLaunch.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(118)))), ((int)(((byte)(110)))));
            this.btnBrowseLaunch.Location = new System.Drawing.Point(609, 0);
            this.btnBrowseLaunch.Margin = new System.Windows.Forms.Padding(6, 0, 0, 0);
            this.btnBrowseLaunch.Name = "btnBrowseLaunch";
            this.btnBrowseLaunch.Size = new System.Drawing.Size(89, 28);
            this.btnBrowseLaunch.TabIndex = 1;
            this.btnBrowseLaunch.Text = "📁 Browse...";
            this.btnBrowseLaunch.UseVisualStyleBackColor = false;
            // 
            // lblArgs
            // 
            this.lblArgs.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.lblArgs.Location = new System.Drawing.Point(0, 52);
            this.lblArgs.Name = "lblArgs";
            this.lblArgs.Size = new System.Drawing.Size(250, 18);
            this.lblArgs.TabIndex = 2;
            this.lblArgs.Text = "Argumen Tambahan (Opsional):";
            // 
            // txtLaunchArgs
            // 
            this.txtLaunchArgs.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLaunchArgs.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.txtLaunchArgs.Location = new System.Drawing.Point(0, 72);
            this.txtLaunchArgs.Name = "txtLaunchArgs";
            this.txtLaunchArgs.Size = new System.Drawing.Size(698, 24);
            this.txtLaunchArgs.TabIndex = 3;
            // 
            // btnExecuteCustom
            // 
            this.btnExecuteCustom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(118)))), ((int)(((byte)(110)))));
            this.btnExecuteCustom.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnExecuteCustom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnExecuteCustom.FlatAppearance.BorderSize = 0;
            this.btnExecuteCustom.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExecuteCustom.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnExecuteCustom.ForeColor = System.Drawing.Color.White;
            this.btnExecuteCustom.Location = new System.Drawing.Point(710, 18);
            this.btnExecuteCustom.Margin = new System.Windows.Forms.Padding(12, 18, 0, 10);
            this.btnExecuteCustom.Name = "btnExecuteCustom";
            this.btnExecuteCustom.Size = new System.Drawing.Size(208, 80);
            this.btnExecuteCustom.TabIndex = 1;
            this.btnExecuteCustom.Text = "🚀 Luncurkan ke\nPC Terpilih";
            this.btnExecuteCustom.UseVisualStyleBackColor = false;
            // 
            // grpLiveLaunch
            // 
            this.grpLiveLaunch.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpLiveLaunch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.grpLiveLaunch.Controls.Add(this.btnLaunchScan);
            this.grpLiveLaunch.Controls.Add(this.chkLaunchSelectAll);
            this.grpLiveLaunch.Controls.Add(this.lblLaunchTotal);
            this.grpLiveLaunch.Controls.Add(this.dgvLaunchClients);
            this.grpLiveLaunch.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.grpLiveLaunch.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.grpLiveLaunch.Location = new System.Drawing.Point(6, 230);
            this.grpLiveLaunch.Name = "grpLiveLaunch";
            this.grpLiveLaunch.Size = new System.Drawing.Size(948, 242);
            this.grpLiveLaunch.TabIndex = 2;
            this.grpLiveLaunch.TabStop = false;
            this.grpLiveLaunch.Text = "Daftar Komputer (Live Dashboard)";
            // 
            // btnLaunchScan
            // 
            this.btnLaunchScan.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(118)))), ((int)(((byte)(110)))));
            this.btnLaunchScan.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLaunchScan.FlatAppearance.BorderSize = 0;
            this.btnLaunchScan.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLaunchScan.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnLaunchScan.ForeColor = System.Drawing.Color.White;
            this.btnLaunchScan.Location = new System.Drawing.Point(18, 22);
            this.btnLaunchScan.Name = "btnLaunchScan";
            this.btnLaunchScan.Size = new System.Drawing.Size(160, 30);
            this.btnLaunchScan.TabIndex = 0;
            this.btnLaunchScan.Text = "🔍 Scan Otomatis (Auto)";
            this.btnLaunchScan.UseVisualStyleBackColor = false;
            // 
            // chkLaunchSelectAll
            // 
            this.chkLaunchSelectAll.AutoSize = true;
            this.chkLaunchSelectAll.Checked = true;
            this.chkLaunchSelectAll.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkLaunchSelectAll.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.chkLaunchSelectAll.Location = new System.Drawing.Point(200, 28);
            this.chkLaunchSelectAll.Name = "chkLaunchSelectAll";
            this.chkLaunchSelectAll.Size = new System.Drawing.Size(134, 19);
            this.chkLaunchSelectAll.TabIndex = 1;
            this.chkLaunchSelectAll.Text = "Pilih Semua PC ([✓])";
            this.chkLaunchSelectAll.UseVisualStyleBackColor = true;
            // 
            // lblLaunchTotal
            // 
            this.lblLaunchTotal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblLaunchTotal.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblLaunchTotal.ForeColor = System.Drawing.Color.DarkGreen;
            this.lblLaunchTotal.Location = new System.Drawing.Point(733, 26);
            this.lblLaunchTotal.Name = "lblLaunchTotal";
            this.lblLaunchTotal.Size = new System.Drawing.Size(200, 20);
            this.lblLaunchTotal.TabIndex = 2;
            this.lblLaunchTotal.Text = "Total PC Terdeteksi: 0";
            this.lblLaunchTotal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // dgvLaunchClients
            // 
            this.dgvLaunchClients.AllowUserToAddRows = false;
            this.dgvLaunchClients.AllowUserToDeleteRows = false;
            this.dgvLaunchClients.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvLaunchClients.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.dgvLaunchClients.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvLaunchClients.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvLaunchClients.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colLaunchCheck,
            this.colLaunchNumber,
            this.colLaunchHostname,
            this.colLaunchIp,
            this.colLaunchStatus});
            this.dgvLaunchClients.EnableHeadersVisualStyles = false;
            this.dgvLaunchClients.Location = new System.Drawing.Point(15, 56);
            this.dgvLaunchClients.Name = "dgvLaunchClients";
            this.dgvLaunchClients.RowHeadersVisible = false;
            this.dgvLaunchClients.RowTemplate.Height = 28;
            this.dgvLaunchClients.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvLaunchClients.Size = new System.Drawing.Size(918, 171);
            this.dgvLaunchClients.TabIndex = 3;
            // 
            // colLaunchCheck
            // 
            this.colLaunchCheck.HeaderText = "Pilih";
            this.colLaunchCheck.Name = "colLaunchCheck";
            this.colLaunchCheck.Width = 50;
            // 
            // colLaunchNumber
            // 
            this.colLaunchNumber.HeaderText = "No PC";
            this.colLaunchNumber.Name = "colLaunchNumber";
            this.colLaunchNumber.ReadOnly = true;
            this.colLaunchNumber.Width = 65;
            // 
            // colLaunchHostname
            // 
            this.colLaunchHostname.HeaderText = "Hostname";
            this.colLaunchHostname.Name = "colLaunchHostname";
            this.colLaunchHostname.ReadOnly = true;
            this.colLaunchHostname.Width = 220;
            // 
            // colLaunchIp
            // 
            this.colLaunchIp.HeaderText = "IP Address";
            this.colLaunchIp.Name = "colLaunchIp";
            this.colLaunchIp.ReadOnly = true;
            this.colLaunchIp.Width = 160;
            // 
            // colLaunchStatus
            // 
            this.colLaunchStatus.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colLaunchStatus.HeaderText = "Status";
            this.colLaunchStatus.Name = "colLaunchStatus";
            this.colLaunchStatus.ReadOnly = true;
            // 
            // tabFileManager
            // 
            this.tabFileManager.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.tabFileManager.Controls.Add(this.grpTransfer);
            this.tabFileManager.Controls.Add(this.grpDist);
            this.tabFileManager.Controls.Add(this.grpCollect);
            this.tabFileManager.Controls.Add(this.grpLiveFile);
            this.tabFileManager.Location = new System.Drawing.Point(4, 26);
            this.tabFileManager.Name = "tabFileManager";
            this.tabFileManager.Padding = new System.Windows.Forms.Padding(4);
            this.tabFileManager.Size = new System.Drawing.Size(960, 478);
            this.tabFileManager.TabIndex = 4;
            this.tabFileManager.Text = " 📂 File Manager ";
            // 
            // grpTransfer
            // 
            this.grpTransfer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpTransfer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.grpTransfer.Controls.Add(this.tblTransfer);
            this.grpTransfer.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.grpTransfer.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.grpTransfer.Location = new System.Drawing.Point(6, 6);
            this.grpTransfer.Name = "grpTransfer";
            this.grpTransfer.Size = new System.Drawing.Size(948, 86);
            this.grpTransfer.TabIndex = 0;
            this.grpTransfer.TabStop = false;
            this.grpTransfer.Text = "1. File Transfer (Kirim File 1-ke-1 ke Komputer Tertentu)";
            // 
            // tblTransfer
            // 
            this.tblTransfer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tblTransfer.ColumnCount = 2;
            this.tblTransfer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblTransfer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            this.tblTransfer.Controls.Add(this.pnlTransLeft, 0, 0);
            this.tblTransfer.Controls.Add(this.btnSendSingle, 1, 0);
            this.tblTransfer.Location = new System.Drawing.Point(12, 18);
            this.tblTransfer.Name = "tblTransfer";
            this.tblTransfer.RowCount = 1;
            this.tblTransfer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblTransfer.Size = new System.Drawing.Size(924, 60);
            this.tblTransfer.TabIndex = 0;
            // 
            // pnlTransLeft
            // 
            this.pnlTransLeft.Controls.Add(this.lblTargetPc);
            this.pnlTransLeft.Controls.Add(this.cmbTargetClient);
            this.pnlTransLeft.Controls.Add(this.btnRefreshTargetPc);
            this.pnlTransLeft.Controls.Add(this.lblDest1);
            this.pnlTransLeft.Controls.Add(this.cmbDest1);
            this.pnlTransLeft.Controls.Add(this.tblTransFile);
            this.pnlTransLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlTransLeft.Location = new System.Drawing.Point(0, 0);
            this.pnlTransLeft.Margin = new System.Windows.Forms.Padding(0);
            this.pnlTransLeft.Name = "pnlTransLeft";
            this.pnlTransLeft.Size = new System.Drawing.Size(724, 60);
            this.pnlTransLeft.TabIndex = 0;
            // 
            // lblTargetPc
            // 
            this.lblTargetPc.Location = new System.Drawing.Point(0, 4);
            this.lblTargetPc.Name = "lblTargetPc";
            this.lblTargetPc.Size = new System.Drawing.Size(110, 20);
            this.lblTargetPc.TabIndex = 0;
            this.lblTargetPc.Text = "Target PC Client:";
            this.lblTargetPc.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cmbTargetClient
            // 
            this.cmbTargetClient.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTargetClient.Location = new System.Drawing.Point(115, 2);
            this.cmbTargetClient.Name = "cmbTargetClient";
            this.cmbTargetClient.Size = new System.Drawing.Size(190, 25);
            this.cmbTargetClient.TabIndex = 1;
            // 
            // btnRefreshTargetPc
            // 
            this.btnRefreshTargetPc.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefreshTargetPc.Location = new System.Drawing.Point(312, 1);
            this.btnRefreshTargetPc.Name = "btnRefreshTargetPc";
            this.btnRefreshTargetPc.Size = new System.Drawing.Size(75, 26);
            this.btnRefreshTargetPc.TabIndex = 2;
            this.btnRefreshTargetPc.Text = "🔄 Refresh";
            this.btnRefreshTargetPc.UseVisualStyleBackColor = true;
            // 
            // lblDest1
            // 
            this.lblDest1.Location = new System.Drawing.Point(400, 4);
            this.lblDest1.Name = "lblDest1";
            this.lblDest1.Size = new System.Drawing.Size(90, 20);
            this.lblDest1.TabIndex = 3;
            this.lblDest1.Text = "Folder Tujuan:";
            this.lblDest1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cmbDest1
            // 
            this.cmbDest1.Items.AddRange(new object[] {
            "Desktop",
            "Downloads",
            "Documents",
            "Ketik Lokasi Manual..."});
            this.cmbDest1.Location = new System.Drawing.Point(495, 2);
            this.cmbDest1.Name = "cmbDest1";
            this.cmbDest1.Size = new System.Drawing.Size(145, 25);
            this.cmbDest1.TabIndex = 4;
            // 
            // tblTransFile
            // 
            this.tblTransFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tblTransFile.ColumnCount = 3;
            this.tblTransFile.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 115F));
            this.tblTransFile.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblTransFile.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this.tblTransFile.Controls.Add(this.lblFile1, 0, 0);
            this.tblTransFile.Controls.Add(this.txtFile1, 1, 0);
            this.tblTransFile.Controls.Add(this.btnBrowse1, 2, 0);
            this.tblTransFile.Location = new System.Drawing.Point(0, 31);
            this.tblTransFile.Name = "tblTransFile";
            this.tblTransFile.RowCount = 1;
            this.tblTransFile.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblTransFile.Size = new System.Drawing.Size(724, 27);
            this.tblTransFile.TabIndex = 5;
            // 
            // lblFile1
            // 
            this.lblFile1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblFile1.Location = new System.Drawing.Point(0, 0);
            this.lblFile1.Margin = new System.Windows.Forms.Padding(0);
            this.lblFile1.Name = "lblFile1";
            this.lblFile1.Size = new System.Drawing.Size(115, 27);
            this.lblFile1.TabIndex = 0;
            this.lblFile1.Text = "Pilih File:";
            this.lblFile1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtFile1
            // 
            this.txtFile1.BackColor = System.Drawing.Color.White;
            this.txtFile1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtFile1.Location = new System.Drawing.Point(115, 2);
            this.txtFile1.Margin = new System.Windows.Forms.Padding(0, 2, 6, 2);
            this.txtFile1.Name = "txtFile1";
            this.txtFile1.ReadOnly = true;
            this.txtFile1.Size = new System.Drawing.Size(513, 24);
            this.txtFile1.TabIndex = 1;
            // 
            // btnBrowse1
            // 
            this.btnBrowse1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBrowse1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnBrowse1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBrowse1.Location = new System.Drawing.Point(634, 0);
            this.btnBrowse1.Margin = new System.Windows.Forms.Padding(0);
            this.btnBrowse1.Name = "btnBrowse1";
            this.btnBrowse1.Size = new System.Drawing.Size(90, 27);
            this.btnBrowse1.TabIndex = 2;
            this.btnBrowse1.Text = "📁 Browse...";
            this.btnBrowse1.UseVisualStyleBackColor = true;
            // 
            // btnSendSingle
            // 
            this.btnSendSingle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(118)))), ((int)(((byte)(110)))));
            this.btnSendSingle.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSendSingle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSendSingle.FlatAppearance.BorderSize = 0;
            this.btnSendSingle.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSendSingle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnSendSingle.ForeColor = System.Drawing.Color.White;
            this.btnSendSingle.Location = new System.Drawing.Point(734, 4);
            this.btnSendSingle.Margin = new System.Windows.Forms.Padding(10, 4, 0, 4);
            this.btnSendSingle.Name = "btnSendSingle";
            this.btnSendSingle.Size = new System.Drawing.Size(190, 52);
            this.btnSendSingle.TabIndex = 1;
            this.btnSendSingle.Text = "📤 Kirim ke PC Terpilih";
            this.btnSendSingle.UseVisualStyleBackColor = false;
            // 
            // grpDist
            // 
            this.grpDist.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpDist.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.grpDist.Controls.Add(this.tblDist);
            this.grpDist.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.grpDist.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.grpDist.Location = new System.Drawing.Point(6, 96);
            this.grpDist.Name = "grpDist";
            this.grpDist.Size = new System.Drawing.Size(948, 86);
            this.grpDist.TabIndex = 1;
            this.grpDist.TabStop = false;
            this.grpDist.Text = "2. File Distribution (Distribusi / Bagikan File Massal ke Siswa)";
            // 
            // tblDist
            // 
            this.tblDist.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tblDist.ColumnCount = 2;
            this.tblDist.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblDist.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            this.tblDist.Controls.Add(this.pnlDistLeft, 0, 0);
            this.tblDist.Controls.Add(this.btnDistribute, 1, 0);
            this.tblDist.Location = new System.Drawing.Point(12, 18);
            this.tblDist.Name = "tblDist";
            this.tblDist.RowCount = 1;
            this.tblDist.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblDist.Size = new System.Drawing.Size(924, 60);
            this.tblDist.TabIndex = 0;
            // 
            // pnlDistLeft
            // 
            this.pnlDistLeft.Controls.Add(this.lblDestDist);
            this.pnlDistLeft.Controls.Add(this.cmbDestDist);
            this.pnlDistLeft.Controls.Add(this.rdoSelectedClients);
            this.pnlDistLeft.Controls.Add(this.rdoAllClients);
            this.pnlDistLeft.Controls.Add(this.tblDistFile);
            this.pnlDistLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlDistLeft.Location = new System.Drawing.Point(0, 0);
            this.pnlDistLeft.Margin = new System.Windows.Forms.Padding(0);
            this.pnlDistLeft.Name = "pnlDistLeft";
            this.pnlDistLeft.Size = new System.Drawing.Size(724, 60);
            this.pnlDistLeft.TabIndex = 0;
            // 
            // lblDestDist
            // 
            this.lblDestDist.Location = new System.Drawing.Point(0, 4);
            this.lblDestDist.Name = "lblDestDist";
            this.lblDestDist.Size = new System.Drawing.Size(110, 20);
            this.lblDestDist.TabIndex = 1;
            this.lblDestDist.Text = "Folder Tujuan:";
            this.lblDestDist.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cmbDestDist
            // 
            this.cmbDestDist.Items.AddRange(new object[] {
            "Desktop",
            "Downloads",
            "Documents",
            "Ketik Lokasi Manual..."});
            this.cmbDestDist.Location = new System.Drawing.Point(115, 2);
            this.cmbDestDist.Name = "cmbDestDist";
            this.cmbDestDist.Size = new System.Drawing.Size(140, 25);
            this.cmbDestDist.TabIndex = 2;
            // 
            // rdoSelectedClients
            // 
            this.rdoSelectedClients.Checked = true;
            this.rdoSelectedClients.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.rdoSelectedClients.Location = new System.Drawing.Point(265, 3);
            this.rdoSelectedClients.Name = "rdoSelectedClients";
            this.rdoSelectedClients.Size = new System.Drawing.Size(255, 23);
            this.rdoSelectedClients.TabIndex = 3;
            this.rdoSelectedClients.TabStop = true;
            this.rdoSelectedClients.Text = "PC yang Dicentang di Tabel ([✓])";
            this.rdoSelectedClients.UseVisualStyleBackColor = true;
            // 
            // rdoAllClients
            // 
            this.rdoAllClients.Location = new System.Drawing.Point(525, 3);
            this.rdoAllClients.Name = "rdoAllClients";
            this.rdoAllClients.Size = new System.Drawing.Size(140, 23);
            this.rdoAllClients.TabIndex = 4;
            this.rdoAllClients.Text = "Semua PC Aktif";
            this.rdoAllClients.UseVisualStyleBackColor = true;
            // 
            // tblDistFile
            // 
            this.tblDistFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tblDistFile.ColumnCount = 3;
            this.tblDistFile.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 115F));
            this.tblDistFile.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblDistFile.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this.tblDistFile.Controls.Add(this.lblFileDist, 0, 0);
            this.tblDistFile.Controls.Add(this.txtFileDist, 1, 0);
            this.tblDistFile.Controls.Add(this.btnBrowseDist, 2, 0);
            this.tblDistFile.Location = new System.Drawing.Point(0, 31);
            this.tblDistFile.Name = "tblDistFile";
            this.tblDistFile.RowCount = 1;
            this.tblDistFile.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblDistFile.Size = new System.Drawing.Size(724, 27);
            this.tblDistFile.TabIndex = 0;
            // 
            // lblFileDist
            // 
            this.lblFileDist.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblFileDist.Location = new System.Drawing.Point(0, 0);
            this.lblFileDist.Margin = new System.Windows.Forms.Padding(0);
            this.lblFileDist.Name = "lblFileDist";
            this.lblFileDist.Size = new System.Drawing.Size(115, 27);
            this.lblFileDist.TabIndex = 0;
            this.lblFileDist.Text = "Pilih File Modul:";
            this.lblFileDist.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtFileDist
            // 
            this.txtFileDist.BackColor = System.Drawing.Color.White;
            this.txtFileDist.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtFileDist.Location = new System.Drawing.Point(115, 2);
            this.txtFileDist.Margin = new System.Windows.Forms.Padding(0, 2, 6, 2);
            this.txtFileDist.Name = "txtFileDist";
            this.txtFileDist.ReadOnly = true;
            this.txtFileDist.Size = new System.Drawing.Size(513, 24);
            this.txtFileDist.TabIndex = 1;
            // 
            // btnBrowseDist
            // 
            this.btnBrowseDist.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBrowseDist.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnBrowseDist.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBrowseDist.Location = new System.Drawing.Point(634, 0);
            this.btnBrowseDist.Margin = new System.Windows.Forms.Padding(0);
            this.btnBrowseDist.Name = "btnBrowseDist";
            this.btnBrowseDist.Size = new System.Drawing.Size(90, 27);
            this.btnBrowseDist.TabIndex = 2;
            this.btnBrowseDist.Text = "📁 Browse...";
            this.btnBrowseDist.UseVisualStyleBackColor = true;
            // 
            // btnDistribute
            // 
            this.btnDistribute.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(2)))), ((int)(((byte)(132)))), ((int)(((byte)(199)))));
            this.btnDistribute.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDistribute.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnDistribute.FlatAppearance.BorderSize = 0;
            this.btnDistribute.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDistribute.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnDistribute.ForeColor = System.Drawing.Color.White;
            this.btnDistribute.Location = new System.Drawing.Point(734, 4);
            this.btnDistribute.Margin = new System.Windows.Forms.Padding(10, 4, 0, 4);
            this.btnDistribute.Name = "btnDistribute";
            this.btnDistribute.Size = new System.Drawing.Size(190, 52);
            this.btnDistribute.TabIndex = 1;
            this.btnDistribute.Text = "📢 Distribusikan File Ke Semua PC";
            this.btnDistribute.UseVisualStyleBackColor = false;
            // 
            // grpCollect
            // 
            this.grpCollect.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpCollect.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.grpCollect.Controls.Add(this.tblCollect);
            this.grpCollect.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.grpCollect.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.grpCollect.Location = new System.Drawing.Point(6, 186);
            this.grpCollect.Name = "grpCollect";
            this.grpCollect.Size = new System.Drawing.Size(948, 115);
            this.grpCollect.TabIndex = 2;
            this.grpCollect.TabStop = false;
            this.grpCollect.Text = "3. Collect Work (Tarik & Kumpulkan Tugas Mahasiswa ke Host Controller)";
            // 
            // tblCollect
            // 
            this.tblCollect.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tblCollect.ColumnCount = 2;
            this.tblCollect.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblCollect.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            this.tblCollect.Controls.Add(this.pnlCollLeft, 0, 0);
            this.tblCollect.Controls.Add(this.btnCollect, 1, 0);
            this.tblCollect.Location = new System.Drawing.Point(12, 18);
            this.tblCollect.Name = "tblCollect";
            this.tblCollect.RowCount = 1;
            this.tblCollect.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblCollect.Size = new System.Drawing.Size(924, 90);
            this.tblCollect.TabIndex = 0;
            // 
            // pnlCollLeft
            // 
            this.pnlCollLeft.Controls.Add(this.lblSource);
            this.pnlCollLeft.Controls.Add(this.cmbCollectSource);
            this.pnlCollLeft.Controls.Add(this.txtSourceDir);
            this.pnlCollLeft.Controls.Add(this.lblPattern);
            this.pnlCollLeft.Controls.Add(this.txtCollectPattern);
            this.pnlCollLeft.Controls.Add(this.chkDeleteAfterCollect);
            this.pnlCollLeft.Controls.Add(this.tblSaveRow);
            this.pnlCollLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlCollLeft.Location = new System.Drawing.Point(0, 0);
            this.pnlCollLeft.Margin = new System.Windows.Forms.Padding(0);
            this.pnlCollLeft.Name = "pnlCollLeft";
            this.pnlCollLeft.Size = new System.Drawing.Size(724, 90);
            this.pnlCollLeft.TabIndex = 0;
            // 
            // lblSource
            // 
            this.lblSource.Location = new System.Drawing.Point(0, 4);
            this.lblSource.Name = "lblSource";
            this.lblSource.Size = new System.Drawing.Size(110, 20);
            this.lblSource.TabIndex = 0;
            this.lblSource.Text = "Folder di Siswa:";
            this.lblSource.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cmbCollectSource
            // 
            this.cmbCollectSource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCollectSource.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cmbCollectSource.FormattingEnabled = true;
            this.cmbCollectSource.Location = new System.Drawing.Point(115, 2);
            this.cmbCollectSource.Name = "cmbCollectSource";
            this.cmbCollectSource.Size = new System.Drawing.Size(140, 23);
            this.cmbCollectSource.TabIndex = 1;
            // 
            // txtSourceDir
            // 
            this.txtSourceDir.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtSourceDir.Location = new System.Drawing.Point(260, 2);
            this.txtSourceDir.Name = "txtSourceDir";
            this.txtSourceDir.Size = new System.Drawing.Size(130, 23);
            this.txtSourceDir.TabIndex = 2;
            this.txtSourceDir.Text = "C:\\\\Tugas";
            // 
            // lblPattern
            // 
            this.lblPattern.Location = new System.Drawing.Point(395, 4);
            this.lblPattern.Name = "lblPattern";
            this.lblPattern.Size = new System.Drawing.Size(85, 20);
            this.lblPattern.TabIndex = 3;
            this.lblPattern.Text = "Filter File:";
            this.lblPattern.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtCollectPattern
            // 
            this.txtCollectPattern.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCollectPattern.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtCollectPattern.Location = new System.Drawing.Point(485, 2);
            this.txtCollectPattern.Name = "txtCollectPattern";
            this.txtCollectPattern.Size = new System.Drawing.Size(239, 23);
            this.txtCollectPattern.TabIndex = 4;
            this.txtCollectPattern.Text = "*.*";
            // 
            // chkDeleteAfterCollect
            // 
            this.chkDeleteAfterCollect.AutoSize = true;
            this.chkDeleteAfterCollect.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.chkDeleteAfterCollect.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(194)))), ((int)(((byte)(65)))), ((int)(((byte)(12)))));
            this.chkDeleteAfterCollect.Location = new System.Drawing.Point(115, 33);
            this.chkDeleteAfterCollect.Name = "chkDeleteAfterCollect";
            this.chkDeleteAfterCollect.Size = new System.Drawing.Size(410, 19);
            this.chkDeleteAfterCollect.TabIndex = 5;
            this.chkDeleteAfterCollect.Text = "Delete files on Student\'s computer after collecting (Hapus setelah ditarik)";
            this.chkDeleteAfterCollect.UseVisualStyleBackColor = true;
            // 
            // tblSaveRow
            // 
            this.tblSaveRow.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tblSaveRow.ColumnCount = 3;
            this.tblSaveRow.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 115F));
            this.tblSaveRow.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblSaveRow.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this.tblSaveRow.Controls.Add(this.lblSaveHost, 0, 0);
            this.tblSaveRow.Controls.Add(this.txtSaveHost, 1, 0);
            this.tblSaveRow.Controls.Add(this.btnBrowseHost, 2, 0);
            this.tblSaveRow.Location = new System.Drawing.Point(0, 58);
            this.tblSaveRow.Name = "tblSaveRow";
            this.tblSaveRow.RowCount = 1;
            this.tblSaveRow.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblSaveRow.Size = new System.Drawing.Size(724, 27);
            this.tblSaveRow.TabIndex = 6;
            // 
            // lblSaveHost
            // 
            this.lblSaveHost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSaveHost.Location = new System.Drawing.Point(0, 0);
            this.lblSaveHost.Margin = new System.Windows.Forms.Padding(0);
            this.lblSaveHost.Name = "lblSaveHost";
            this.lblSaveHost.Size = new System.Drawing.Size(115, 27);
            this.lblSaveHost.TabIndex = 0;
            this.lblSaveHost.Text = "Simpan ke Host:";
            this.lblSaveHost.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtSaveHost
            // 
            this.txtSaveHost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSaveHost.Location = new System.Drawing.Point(115, 2);
            this.txtSaveHost.Margin = new System.Windows.Forms.Padding(0, 2, 6, 2);
            this.txtSaveHost.Name = "txtSaveHost";
            this.txtSaveHost.Size = new System.Drawing.Size(513, 24);
            this.txtSaveHost.TabIndex = 1;
            // 
            // btnBrowseHost
            // 
            this.btnBrowseHost.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBrowseHost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnBrowseHost.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBrowseHost.Location = new System.Drawing.Point(634, 0);
            this.btnBrowseHost.Margin = new System.Windows.Forms.Padding(0);
            this.btnBrowseHost.Name = "btnBrowseHost";
            this.btnBrowseHost.Size = new System.Drawing.Size(90, 27);
            this.btnBrowseHost.TabIndex = 2;
            this.btnBrowseHost.Text = "📁 Browse...";
            this.btnBrowseHost.UseVisualStyleBackColor = true;
            // 
            // btnCollect
            // 
            this.btnCollect.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(185)))), ((int)(((byte)(129)))));
            this.btnCollect.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCollect.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnCollect.FlatAppearance.BorderSize = 0;
            this.btnCollect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCollect.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnCollect.ForeColor = System.Drawing.Color.White;
            this.btnCollect.Location = new System.Drawing.Point(734, 4);
            this.btnCollect.Margin = new System.Windows.Forms.Padding(10, 4, 0, 4);
            this.btnCollect.Name = "btnCollect";
            this.btnCollect.Size = new System.Drawing.Size(190, 82);
            this.btnCollect.TabIndex = 1;
            this.btnCollect.Text = "📥 Tarik Tugas dari\nPC Dicentang";
            this.btnCollect.UseVisualStyleBackColor = false;
            // 
            // grpLiveFile
            // 
            this.grpLiveFile.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpLiveFile.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.grpLiveFile.Controls.Add(this.btnFileScan);
            this.grpLiveFile.Controls.Add(this.chkFileSelectAll);
            this.grpLiveFile.Controls.Add(this.lblFileTotal);
            this.grpLiveFile.Controls.Add(this.dgvFileClients);
            this.grpLiveFile.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.grpLiveFile.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.grpLiveFile.Location = new System.Drawing.Point(6, 307);
            this.grpLiveFile.Name = "grpLiveFile";
            this.grpLiveFile.Size = new System.Drawing.Size(948, 165);
            this.grpLiveFile.TabIndex = 3;
            this.grpLiveFile.TabStop = false;
            this.grpLiveFile.Text = "Daftar Komputer (Live Dashboard)";
            // 
            // btnFileScan
            // 
            this.btnFileScan.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(118)))), ((int)(((byte)(110)))));
            this.btnFileScan.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnFileScan.FlatAppearance.BorderSize = 0;
            this.btnFileScan.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFileScan.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnFileScan.ForeColor = System.Drawing.Color.White;
            this.btnFileScan.Location = new System.Drawing.Point(18, 22);
            this.btnFileScan.Name = "btnFileScan";
            this.btnFileScan.Size = new System.Drawing.Size(160, 30);
            this.btnFileScan.TabIndex = 0;
            this.btnFileScan.Text = "🔍 Scan Otomatis (Auto)";
            this.btnFileScan.UseVisualStyleBackColor = false;
            // 
            // chkFileSelectAll
            // 
            this.chkFileSelectAll.AutoSize = true;
            this.chkFileSelectAll.Checked = true;
            this.chkFileSelectAll.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkFileSelectAll.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.chkFileSelectAll.Location = new System.Drawing.Point(200, 28);
            this.chkFileSelectAll.Name = "chkFileSelectAll";
            this.chkFileSelectAll.Size = new System.Drawing.Size(134, 19);
            this.chkFileSelectAll.TabIndex = 1;
            this.chkFileSelectAll.Text = "Pilih Semua PC ([✓])";
            this.chkFileSelectAll.UseVisualStyleBackColor = true;
            // 
            // lblFileTotal
            // 
            this.lblFileTotal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblFileTotal.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblFileTotal.ForeColor = System.Drawing.Color.DarkGreen;
            this.lblFileTotal.Location = new System.Drawing.Point(733, 26);
            this.lblFileTotal.Name = "lblFileTotal";
            this.lblFileTotal.Size = new System.Drawing.Size(200, 20);
            this.lblFileTotal.TabIndex = 2;
            this.lblFileTotal.Text = "Total PC Terdeteksi: 0";
            this.lblFileTotal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // dgvFileClients
            // 
            this.dgvFileClients.AllowUserToAddRows = false;
            this.dgvFileClients.AllowUserToDeleteRows = false;
            this.dgvFileClients.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvFileClients.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.dgvFileClients.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvFileClients.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvFileClients.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colFileCheck,
            this.colFileNumber,
            this.colFileHostname,
            this.colFileIp,
            this.colFileStatus});
            this.dgvFileClients.EnableHeadersVisualStyles = false;
            this.dgvFileClients.Location = new System.Drawing.Point(15, 56);
            this.dgvFileClients.Name = "dgvFileClients";
            this.dgvFileClients.RowHeadersVisible = false;
            this.dgvFileClients.RowTemplate.Height = 28;
            this.dgvFileClients.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvFileClients.Size = new System.Drawing.Size(918, 123);
            this.dgvFileClients.TabIndex = 3;
            // 
            // colFileCheck
            // 
            this.colFileCheck.HeaderText = "Pilih";
            this.colFileCheck.Name = "colFileCheck";
            this.colFileCheck.Width = 50;
            // 
            // colFileNumber
            // 
            this.colFileNumber.HeaderText = "No PC";
            this.colFileNumber.Name = "colFileNumber";
            this.colFileNumber.ReadOnly = true;
            this.colFileNumber.Width = 65;
            // 
            // colFileHostname
            // 
            this.colFileHostname.HeaderText = "Hostname";
            this.colFileHostname.Name = "colFileHostname";
            this.colFileHostname.ReadOnly = true;
            this.colFileHostname.Width = 220;
            // 
            // colFileIp
            // 
            this.colFileIp.HeaderText = "IP Address";
            this.colFileIp.Name = "colFileIp";
            this.colFileIp.ReadOnly = true;
            this.colFileIp.Width = 160;
            // 
            // colFileStatus
            // 
            this.colFileStatus.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colFileStatus.HeaderText = "Status";
            this.colFileStatus.Name = "colFileStatus";
            this.colFileStatus.ReadOnly = true;
            // 
            // FormDashboard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.ClientSize = new System.Drawing.Size(984, 690);
            this.Controls.Add(this.tabMain);
            this.Controls.Add(this.lblCopyright);
            this.Controls.Add(this.grpLog);
            this.Controls.Add(this.pnlTop);
            this.MinimumSize = new System.Drawing.Size(1000, 720);
            this.Name = "FormDashboard";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "AirSET v2.0 - Remote Controller Dashboard";
            this.Load += new System.EventHandler(this.FormDashboard_Load);
            this.pnlTop.ResumeLayout(false);
            this.pnlTop.PerformLayout();
            this.grpConfig.ResumeLayout(false);
            this.grpConfig.PerformLayout();
            this.grpBatch.ResumeLayout(false);
            this.grpBatch.PerformLayout();
            this.grpGrid.ResumeLayout(false);
            this.grpGrid.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvClients)).EndInit();
            this.grpLog.ResumeLayout(false);
            this.grpLog.PerformLayout();
            this.tabMain.ResumeLayout(false);
            this.tabNetwork.ResumeLayout(false);
            this.tabShield.ResumeLayout(false);
            this.grpShieldAction.ResumeLayout(false);
            this.grpShieldAction.PerformLayout();
            this.grpShieldGrid.ResumeLayout(false);
            this.grpShieldGrid.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvShieldClients)).EndInit();
            this.tabPower.ResumeLayout(false);
            this.grpDesktop.ResumeLayout(false);
            this.tblDesktop.ResumeLayout(false);
            this.grpPower.ResumeLayout(false);
            this.tblPower.ResumeLayout(false);
            this.grpLivePower.ResumeLayout(false);
            this.grpLivePower.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPowerClients)).EndInit();
            this.tabQuickLaunch.ResumeLayout(false);
            this.grpPresets.ResumeLayout(false);
            this.tblPresets.ResumeLayout(false);
            this.grpCustom.ResumeLayout(false);
            this.tblCustom.ResumeLayout(false);
            this.pnlInputs.ResumeLayout(false);
            this.pnlInputs.PerformLayout();
            this.tblTargetRow.ResumeLayout(false);
            this.tblTargetRow.PerformLayout();
            this.grpLiveLaunch.ResumeLayout(false);
            this.grpLiveLaunch.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLaunchClients)).EndInit();
            this.tabFileManager.ResumeLayout(false);
            this.grpTransfer.ResumeLayout(false);
            this.tblTransfer.ResumeLayout(false);
            this.pnlTransLeft.ResumeLayout(false);
            this.tblTransFile.ResumeLayout(false);
            this.tblTransFile.PerformLayout();
            this.grpDist.ResumeLayout(false);
            this.tblDist.ResumeLayout(false);
            this.pnlDistLeft.ResumeLayout(false);
            this.tblDistFile.ResumeLayout(false);
            this.tblDistFile.PerformLayout();
            this.grpCollect.ResumeLayout(false);
            this.tblCollect.ResumeLayout(false);
            this.pnlCollLeft.ResumeLayout(false);
            this.pnlCollLeft.PerformLayout();
            this.tblSaveRow.ResumeLayout(false);
            this.tblSaveRow.PerformLayout();
            this.grpLiveFile.ResumeLayout(false);
            this.grpLiveFile.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFileClients)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblSub;
        private System.Windows.Forms.TabControl tabMain;
        private System.Windows.Forms.TabPage tabNetwork;
        private System.Windows.Forms.TabPage tabShield;
        private System.Windows.Forms.TabPage tabPower;
        private System.Windows.Forms.TabPage tabQuickLaunch;
        private System.Windows.Forms.TabPage tabFileManager;
        private System.Windows.Forms.GroupBox grpConfig;
        private System.Windows.Forms.Label lblLab;
        private System.Windows.Forms.ComboBox cmbLab;
        private System.Windows.Forms.Button btnAddLab;
        private System.Windows.Forms.Button btnSaveLab;
        private System.Windows.Forms.Button btnDeleteLab;
        private System.Windows.Forms.Label lblBaseIp;
        private System.Windows.Forms.TextBox txtBaseIp;
        private System.Windows.Forms.Label lblSubnet;
        private System.Windows.Forms.TextBox txtSubnet;
        private System.Windows.Forms.Label lblGateway;
        private System.Windows.Forms.TextBox txtGateway;
        private System.Windows.Forms.Label lblDns;
        private System.Windows.Forms.TextBox txtDns;
        private System.Windows.Forms.Label lblPrefix;
        private System.Windows.Forms.TextBox txtPrefix;
        private System.Windows.Forms.Label lblWorkgroup;
        private System.Windows.Forms.TextBox txtWorkgroup;
        private System.Windows.Forms.GroupBox grpBatch;
        private System.Windows.Forms.CheckBox chkChangePass;
        private System.Windows.Forms.Label lblPass;
        private System.Windows.Forms.TextBox txtNewPass;
        private System.Windows.Forms.Label lblInterface;
        private System.Windows.Forms.ComboBox cmbInterface;
        private System.Windows.Forms.Button btnRefreshInterface;
        private System.Windows.Forms.CheckBox chkAutoLogon;
        private System.Windows.Forms.CheckBox chkDisableLogon;
        private System.Windows.Forms.CheckBox chkRestart;
        private System.Windows.Forms.Button btnPushConfig;
        private System.Windows.Forms.GroupBox grpGrid;
        private System.Windows.Forms.Button btnScan;
        private System.Windows.Forms.TextBox txtDirectIp;
        private System.Windows.Forms.Button btnDirectAdd;
        private System.Windows.Forms.CheckBox chkSelectAll;
        private System.Windows.Forms.Label lblTotalOnline;
        private System.Windows.Forms.DataGridView dgvClients;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colCheck;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn colHostname;
        private System.Windows.Forms.DataGridViewTextBoxColumn colIp;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTargetIp;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTargetHostname;
        private System.Windows.Forms.DataGridViewTextBoxColumn colStatus;
        private System.Windows.Forms.GroupBox grpShieldAction;
        private System.Windows.Forms.Label lblShieldInfo;
        private System.Windows.Forms.Label lblDfcPass;
        private System.Windows.Forms.TextBox txtDfcPassword;
        private System.Windows.Forms.Button btnShieldLock;
        private System.Windows.Forms.Button btnShieldUnlock;
        private System.Windows.Forms.Button btnUwfInstall;
        private System.Windows.Forms.Button btnUwfUninstall;
        private System.Windows.Forms.Button btnShieldRestart;
        private System.Windows.Forms.GroupBox grpShieldGrid;
        private System.Windows.Forms.Button btnShieldScan;
        private System.Windows.Forms.TextBox txtShieldDirectIp;
        private System.Windows.Forms.Button btnShieldDirectAdd;
        private System.Windows.Forms.CheckBox chkShieldSelectAll;
        private System.Windows.Forms.Label lblShieldTotal;
        private System.Windows.Forms.DataGridView dgvShieldClients;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colShieldCheck;
        private System.Windows.Forms.DataGridViewTextBoxColumn colShieldNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn colShieldHostname;
        private System.Windows.Forms.DataGridViewTextBoxColumn colShieldIp;
        private System.Windows.Forms.DataGridViewTextBoxColumn colShieldOs;
        private System.Windows.Forms.DataGridViewTextBoxColumn colShieldStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn colShieldAction;
        private System.Windows.Forms.GroupBox grpLog;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.Label lblCopyright;
        private System.Windows.Forms.Button btnChangeAppPass;
        // Tab 3 (tabPower) Controls
        private System.Windows.Forms.GroupBox grpDesktop;
        private System.Windows.Forms.Label lblDesktopDesc;
        private System.Windows.Forms.TableLayoutPanel tblDesktop;
        private System.Windows.Forms.Button btnLockScreen;
        private System.Windows.Forms.Button btnUnlockScreen;
        private System.Windows.Forms.Button btnClearDesktop;
        private System.Windows.Forms.GroupBox grpPower;
        private System.Windows.Forms.Label lblPowerDesc;
        private System.Windows.Forms.TableLayoutPanel tblPower;
        private System.Windows.Forms.Button btnShutdown;
        private System.Windows.Forms.Button btnRestart;
        private System.Windows.Forms.Button btnWakeOnLan;
        private System.Windows.Forms.Label lblWolHint;
        private System.Windows.Forms.GroupBox grpLivePower;
        private System.Windows.Forms.Button btnPowerScan;
        private System.Windows.Forms.CheckBox chkPowerSelectAll;
        private System.Windows.Forms.Label lblPowerTotal;
        private System.Windows.Forms.DataGridView dgvPowerClients;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colPowerCheck;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPowerNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPowerHostname;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPowerIp;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPowerStatus;

        // Tab 4 (tabQuickLaunch) Controls
        private System.Windows.Forms.GroupBox grpPresets;
        private System.Windows.Forms.TableLayoutPanel tblPresets;
        private System.Windows.Forms.Button btnRemotePs;
        private System.Windows.Forms.Button btnEdge;
        private System.Windows.Forms.Button btnCmd;
        private System.Windows.Forms.Button btnNotepad;
        private System.Windows.Forms.GroupBox grpCustom;
        private System.Windows.Forms.TableLayoutPanel tblCustom;
        private System.Windows.Forms.Panel pnlInputs;
        private System.Windows.Forms.Label lblTarget;
        private System.Windows.Forms.TableLayoutPanel tblTargetRow;
        private System.Windows.Forms.TextBox txtLaunchTarget;
        private System.Windows.Forms.Button btnBrowseLaunch;
        private System.Windows.Forms.Label lblArgs;
        private System.Windows.Forms.TextBox txtLaunchArgs;
        private System.Windows.Forms.Button btnExecuteCustom;
        private System.Windows.Forms.GroupBox grpLiveLaunch;
        private System.Windows.Forms.Button btnLaunchScan;
        private System.Windows.Forms.CheckBox chkLaunchSelectAll;
        private System.Windows.Forms.Label lblLaunchTotal;
        private System.Windows.Forms.DataGridView dgvLaunchClients;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colLaunchCheck;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLaunchNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLaunchHostname;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLaunchIp;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLaunchStatus;

        // Tab 5 (tabFileManager) Controls
        private System.Windows.Forms.GroupBox grpTransfer;
        private System.Windows.Forms.TableLayoutPanel tblTransfer;
        private System.Windows.Forms.Panel pnlTransLeft;
        private System.Windows.Forms.Label lblTargetPc;
        private System.Windows.Forms.ComboBox cmbTargetClient;
        private System.Windows.Forms.Button btnRefreshTargetPc;
        private System.Windows.Forms.Label lblDest1;
        private System.Windows.Forms.ComboBox cmbDest1;
        private System.Windows.Forms.TableLayoutPanel tblTransFile;
        private System.Windows.Forms.Label lblFile1;
        private System.Windows.Forms.TextBox txtFile1;
        private System.Windows.Forms.Button btnBrowse1;
        private System.Windows.Forms.Button btnSendSingle;
        private System.Windows.Forms.GroupBox grpDist;
        private System.Windows.Forms.TableLayoutPanel tblDist;
        private System.Windows.Forms.Panel pnlDistLeft;
        private System.Windows.Forms.TableLayoutPanel tblDistFile;
        private System.Windows.Forms.Label lblFileDist;
        private System.Windows.Forms.TextBox txtFileDist;
        private System.Windows.Forms.Button btnBrowseDist;
        private System.Windows.Forms.Label lblDestDist;
        private System.Windows.Forms.ComboBox cmbDestDist;
        private System.Windows.Forms.RadioButton rdoSelectedClients;
        private System.Windows.Forms.RadioButton rdoAllClients;
        private System.Windows.Forms.Button btnDistribute;
        private System.Windows.Forms.GroupBox grpCollect;
        private System.Windows.Forms.TableLayoutPanel tblCollect;
        private System.Windows.Forms.Panel pnlCollLeft;
        private System.Windows.Forms.Label lblSource;
        private System.Windows.Forms.ComboBox cmbCollectSource;
        private System.Windows.Forms.TextBox txtSourceDir;
        private System.Windows.Forms.Label lblPattern;
        private System.Windows.Forms.TextBox txtCollectPattern;
        private System.Windows.Forms.CheckBox chkDeleteAfterCollect;
        private System.Windows.Forms.TableLayoutPanel tblSaveRow;
        private System.Windows.Forms.Label lblSaveHost;
        private System.Windows.Forms.TextBox txtSaveHost;
        private System.Windows.Forms.Button btnBrowseHost;
        private System.Windows.Forms.Button btnCollect;
        private System.Windows.Forms.GroupBox grpLiveFile;
        private System.Windows.Forms.Button btnFileScan;
        private System.Windows.Forms.CheckBox chkFileSelectAll;
        private System.Windows.Forms.Label lblFileTotal;
        private System.Windows.Forms.DataGridView dgvFileClients;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colFileCheck;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFileNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFileHostname;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFileIp;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFileStatus;
    }
}
