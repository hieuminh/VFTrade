namespace VFTrade
{
    partial class frmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.btnTao = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.dgvAccounts = new System.Windows.Forms.DataGridView();
            this.accCol1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.accUsername = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.accFullName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.accComment = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.accLoaiGiayTo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.accCMND = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.accSanPham = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.accGoiPhi = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.accPhone = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.accEmail = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.accHanMuc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.accTao = new System.Windows.Forms.DataGridViewButtonColumn();
            this.accResult = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkRemember = new System.Windows.Forms.CheckBox();
            this.btnLogin = new System.Windows.Forms.Button();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnImport = new System.Windows.Forms.Button();
            this.txtFilePath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAccounts)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1197, 596);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.White;
            this.tabPage1.Controls.Add(this.btnTao);
            this.tabPage1.Controls.Add(this.groupBox3);
            this.tabPage1.Controls.Add(this.groupBox2);
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Controls.Add(this.btnImport);
            this.tabPage1.Controls.Add(this.txtFilePath);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1189, 570);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Tài Khoản";
            // 
            // btnTao
            // 
            this.btnTao.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTao.BackColor = System.Drawing.Color.Transparent;
            this.btnTao.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTao.Location = new System.Drawing.Point(1105, 172);
            this.btnTao.Name = "btnTao";
            this.btnTao.Size = new System.Drawing.Size(75, 23);
            this.btnTao.TabIndex = 10;
            this.btnTao.Text = "Tạo";
            this.btnTao.UseVisualStyleBackColor = false;
            this.btnTao.Click += new System.EventHandler(this.btnTao_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.dgvAccounts);
            this.groupBox3.Location = new System.Drawing.Point(8, 215);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(1175, 352);
            this.groupBox3.TabIndex = 9;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Danh sách tài khoản cần tạo";
            // 
            // dgvAccounts
            // 
            this.dgvAccounts.AllowUserToAddRows = false;
            this.dgvAccounts.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvAccounts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAccounts.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.accCol1,
            this.accUsername,
            this.accFullName,
            this.accComment,
            this.accLoaiGiayTo,
            this.accCMND,
            this.accSanPham,
            this.accGoiPhi,
            this.accPhone,
            this.accEmail,
            this.accHanMuc,
            this.accTao,
            this.accResult});
            this.dgvAccounts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvAccounts.Location = new System.Drawing.Point(3, 16);
            this.dgvAccounts.Name = "dgvAccounts";
            this.dgvAccounts.RowHeadersVisible = false;
            this.dgvAccounts.Size = new System.Drawing.Size(1169, 333);
            this.dgvAccounts.TabIndex = 0;
            // 
            // accCol1
            // 
            this.accCol1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.accCol1.HeaderText = "STT";
            this.accCol1.Name = "accCol1";
            this.accCol1.ReadOnly = true;
            this.accCol1.Width = 50;
            // 
            // accUsername
            // 
            this.accUsername.HeaderText = "User";
            this.accUsername.Name = "accUsername";
            this.accUsername.ReadOnly = true;
            // 
            // accFullName
            // 
            this.accFullName.HeaderText = "Họ Tên";
            this.accFullName.Name = "accFullName";
            this.accFullName.ReadOnly = true;
            // 
            // accComment
            // 
            this.accComment.HeaderText = "Ghi chú";
            this.accComment.Name = "accComment";
            this.accComment.ReadOnly = true;
            // 
            // accLoaiGiayTo
            // 
            this.accLoaiGiayTo.HeaderText = "ID Type";
            this.accLoaiGiayTo.Name = "accLoaiGiayTo";
            this.accLoaiGiayTo.ReadOnly = true;
            // 
            // accCMND
            // 
            this.accCMND.HeaderText = "CMND/CCCD";
            this.accCMND.Name = "accCMND";
            this.accCMND.ReadOnly = true;
            // 
            // accSanPham
            // 
            this.accSanPham.HeaderText = "Sản Phẩm";
            this.accSanPham.Name = "accSanPham";
            this.accSanPham.ReadOnly = true;
            // 
            // accGoiPhi
            // 
            this.accGoiPhi.HeaderText = "Gói Phí";
            this.accGoiPhi.Name = "accGoiPhi";
            this.accGoiPhi.ReadOnly = true;
            // 
            // accPhone
            // 
            this.accPhone.HeaderText = "Phone";
            this.accPhone.Name = "accPhone";
            this.accPhone.ReadOnly = true;
            // 
            // accEmail
            // 
            this.accEmail.HeaderText = "Email";
            this.accEmail.Name = "accEmail";
            this.accEmail.ReadOnly = true;
            // 
            // accHanMuc
            // 
            this.accHanMuc.HeaderText = "Hạn Mức";
            this.accHanMuc.Name = "accHanMuc";
            this.accHanMuc.ReadOnly = true;
            // 
            // accTao
            // 
            this.accTao.HeaderText = "Tạo";
            this.accTao.Name = "accTao";
            this.accTao.ReadOnly = true;
            // 
            // accResult
            // 
            this.accResult.HeaderText = "Result";
            this.accResult.Name = "accResult";
            this.accResult.ReadOnly = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.txtLog);
            this.groupBox2.Location = new System.Drawing.Point(278, 6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(911, 145);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Log";
            // 
            // txtLog
            // 
            this.txtLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLog.Location = new System.Drawing.Point(3, 16);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.Size = new System.Drawing.Size(905, 126);
            this.txtLog.TabIndex = 7;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkRemember);
            this.groupBox1.Controls.Add(this.btnLogin);
            this.groupBox1.Controls.Add(this.txtPassword);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtUsername);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(8, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(264, 145);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Tài khoản Tạo";
            // 
            // chkRemember
            // 
            this.chkRemember.AutoSize = true;
            this.chkRemember.Checked = true;
            this.chkRemember.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkRemember.Location = new System.Drawing.Point(79, 83);
            this.chkRemember.Name = "chkRemember";
            this.chkRemember.Size = new System.Drawing.Size(63, 17);
            this.chkRemember.TabIndex = 8;
            this.chkRemember.Text = "Ghi nhớ";
            this.chkRemember.UseVisualStyleBackColor = true;
            this.chkRemember.CheckedChanged += new System.EventHandler(this.chkRemember_CheckedChanged);
            // 
            // btnLogin
            // 
            this.btnLogin.BackColor = System.Drawing.Color.Transparent;
            this.btnLogin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLogin.Location = new System.Drawing.Point(79, 106);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(150, 27);
            this.btnLogin.TabIndex = 4;
            this.btnLogin.Text = "Đăng nhập";
            this.btnLogin.UseVisualStyleBackColor = false;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(79, 57);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(150, 20);
            this.txtPassword.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(21, 60);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Mật khẩu";
            // 
            // txtUsername
            // 
            this.txtUsername.Location = new System.Drawing.Point(79, 31);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(150, 20);
            this.txtUsername.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(21, 34);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Username";
            // 
            // btnImport
            // 
            this.btnImport.BackColor = System.Drawing.Color.Transparent;
            this.btnImport.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnImport.Location = new System.Drawing.Point(499, 173);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(75, 23);
            this.btnImport.TabIndex = 2;
            this.btnImport.Text = "Import";
            this.btnImport.UseVisualStyleBackColor = false;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // txtFilePath
            // 
            this.txtFilePath.Location = new System.Drawing.Point(72, 175);
            this.txtFilePath.Name = "txtFilePath";
            this.txtFilePath.Size = new System.Drawing.Size(421, 20);
            this.txtFilePath.TabIndex = 1;
            this.txtFilePath.Click += new System.EventHandler(this.txtFilePath_Clicked);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 178);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "File Excel";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1197, 596);
            this.Controls.Add(this.tabControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmMain";
            this.Text = "Automation Tools";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvAccounts)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.TextBox txtFilePath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.DataGridView dgvAccounts;
        private System.Windows.Forms.CheckBox chkRemember;
        private System.Windows.Forms.Button btnTao;
        private System.Windows.Forms.DataGridViewTextBoxColumn accCol1;
        private System.Windows.Forms.DataGridViewTextBoxColumn accUsername;
        private System.Windows.Forms.DataGridViewTextBoxColumn accFullName;
        private System.Windows.Forms.DataGridViewTextBoxColumn accComment;
        private System.Windows.Forms.DataGridViewTextBoxColumn accLoaiGiayTo;
        private System.Windows.Forms.DataGridViewTextBoxColumn accCMND;
        private System.Windows.Forms.DataGridViewTextBoxColumn accSanPham;
        private System.Windows.Forms.DataGridViewTextBoxColumn accGoiPhi;
        private System.Windows.Forms.DataGridViewTextBoxColumn accPhone;
        private System.Windows.Forms.DataGridViewTextBoxColumn accEmail;
        private System.Windows.Forms.DataGridViewTextBoxColumn accHanMuc;
        private System.Windows.Forms.DataGridViewButtonColumn accTao;
        private System.Windows.Forms.DataGridViewTextBoxColumn accResult;
    }
}

