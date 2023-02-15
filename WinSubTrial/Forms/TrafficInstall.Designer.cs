
namespace WinSubTrial
{
    partial class TrafficInstall
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
            this.label1 = new System.Windows.Forms.Label();
            this.cboTask = new System.Windows.Forms.ComboBox();
            this.chkConnectWifi = new System.Windows.Forms.CheckBox();
            this.txtWifiPass = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cboIpTool = new System.Windows.Forms.ComboBox();
            this.cboCountry = new System.Windows.Forms.ComboBox();
            this.txtMailApi = new System.Windows.Forms.TextBox();
            this.chkChangeMail = new System.Windows.Forms.CheckBox();
            this.cboPhoneService = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtPhoneServiceApi = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtNamePackage = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.cboScriptName = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtPackage = new System.Windows.Forms.TextBox();
            this.chkBackupRestorePackage = new System.Windows.Forms.CheckBox();
            this.btnStartAll = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Task";
            // 
            // cboTask
            // 
            this.cboTask.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTask.FormattingEnabled = true;
            this.cboTask.Items.AddRange(new object[] {
            "Install",
            "RRS"});
            this.cboTask.Location = new System.Drawing.Point(86, 23);
            this.cboTask.Name = "cboTask";
            this.cboTask.Size = new System.Drawing.Size(287, 21);
            this.cboTask.TabIndex = 1;
            // 
            // chkConnectWifi
            // 
            this.chkConnectWifi.AutoSize = true;
            this.chkConnectWifi.Location = new System.Drawing.Point(15, 67);
            this.chkConnectWifi.Name = "chkConnectWifi";
            this.chkConnectWifi.Size = new System.Drawing.Size(109, 17);
            this.chkConnectWifi.TabIndex = 2;
            this.chkConnectWifi.Text = "Connect Wifi App";
            this.chkConnectWifi.UseVisualStyleBackColor = true;
            // 
            // txtWifiPass
            // 
            this.txtWifiPass.Location = new System.Drawing.Point(215, 64);
            this.txtWifiPass.Name = "txtWifiPass";
            this.txtWifiPass.Size = new System.Drawing.Size(158, 20);
            this.txtWifiPass.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(148, 67);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Wifi|Pass";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 111);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(83, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "IP Fake Method";
            // 
            // cboIpTool
            // 
            this.cboIpTool.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboIpTool.FormattingEnabled = true;
            this.cboIpTool.Items.AddRange(new object[] {
            "NordVPN",
            "Key Tinsoft"});
            this.cboIpTool.Location = new System.Drawing.Point(116, 108);
            this.cboIpTool.Name = "cboIpTool";
            this.cboIpTool.Size = new System.Drawing.Size(163, 21);
            this.cboIpTool.TabIndex = 6;
            // 
            // cboCountry
            // 
            this.cboCountry.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCountry.FormattingEnabled = true;
            this.cboCountry.Location = new System.Drawing.Point(285, 108);
            this.cboCountry.Name = "cboCountry";
            this.cboCountry.Size = new System.Drawing.Size(88, 21);
            this.cboCountry.TabIndex = 7;
            // 
            // txtMailApi
            // 
            this.txtMailApi.Location = new System.Drawing.Point(179, 146);
            this.txtMailApi.Name = "txtMailApi";
            this.txtMailApi.Size = new System.Drawing.Size(194, 20);
            this.txtMailApi.TabIndex = 9;
            // 
            // chkChangeMail
            // 
            this.chkChangeMail.AutoSize = true;
            this.chkChangeMail.Location = new System.Drawing.Point(15, 146);
            this.chkChangeMail.Name = "chkChangeMail";
            this.chkChangeMail.Size = new System.Drawing.Size(150, 17);
            this.chkChangeMail.TabIndex = 8;
            this.chkChangeMail.Text = "Change Mail, API Get Mail";
            this.chkChangeMail.UseVisualStyleBackColor = true;
            // 
            // cboPhoneService
            // 
            this.cboPhoneService.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPhoneService.FormattingEnabled = true;
            this.cboPhoneService.Items.AddRange(new object[] {
            "No",
            "RentCode"});
            this.cboPhoneService.Location = new System.Drawing.Point(116, 184);
            this.cboPhoneService.Name = "cboPhoneService";
            this.cboPhoneService.Size = new System.Drawing.Size(257, 21);
            this.cboPhoneService.TabIndex = 11;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 184);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(77, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Phone Service";
            // 
            // txtPhoneServiceApi
            // 
            this.txtPhoneServiceApi.Location = new System.Drawing.Point(116, 226);
            this.txtPhoneServiceApi.Name = "txtPhoneServiceApi";
            this.txtPhoneServiceApi.Size = new System.Drawing.Size(257, 20);
            this.txtPhoneServiceApi.TabIndex = 13;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 226);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(45, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "API Key";
            // 
            // txtNamePackage
            // 
            this.txtNamePackage.Location = new System.Drawing.Point(116, 263);
            this.txtNamePackage.Multiline = true;
            this.txtNamePackage.Name = "txtNamePackage";
            this.txtNamePackage.Size = new System.Drawing.Size(257, 124);
            this.txtNamePackage.TabIndex = 15;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 266);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(80, 13);
            this.label6.TabIndex = 14;
            this.label6.Text = "Name|Package";
            // 
            // cboScriptName
            // 
            this.cboScriptName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboScriptName.FormattingEnabled = true;
            this.cboScriptName.Location = new System.Drawing.Point(116, 408);
            this.cboScriptName.Name = "cboScriptName";
            this.cboScriptName.Size = new System.Drawing.Size(257, 21);
            this.cboScriptName.TabIndex = 17;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 416);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(65, 13);
            this.label7.TabIndex = 16;
            this.label7.Text = "Script Name";
            // 
            // txtPackage
            // 
            this.txtPackage.Location = new System.Drawing.Point(173, 463);
            this.txtPackage.Name = "txtPackage";
            this.txtPackage.Size = new System.Drawing.Size(200, 20);
            this.txtPackage.TabIndex = 19;
            // 
            // chkBackupRestorePackage
            // 
            this.chkBackupRestorePackage.AutoSize = true;
            this.chkBackupRestorePackage.Location = new System.Drawing.Point(15, 463);
            this.chkBackupRestorePackage.Name = "chkBackupRestorePackage";
            this.chkBackupRestorePackage.Size = new System.Drawing.Size(152, 17);
            this.chkBackupRestorePackage.TabIndex = 18;
            this.chkBackupRestorePackage.Text = "Backup, Restore Package";
            this.chkBackupRestorePackage.UseVisualStyleBackColor = true;
            // 
            // btnStartAll
            // 
            this.btnStartAll.BackColor = System.Drawing.Color.DodgerBlue;
            this.btnStartAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStartAll.Font = new System.Drawing.Font("Segoe UI", 9.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStartAll.ForeColor = System.Drawing.Color.White;
            this.btnStartAll.Location = new System.Drawing.Point(117, 510);
            this.btnStartAll.Name = "btnStartAll";
            this.btnStartAll.Size = new System.Drawing.Size(147, 36);
            this.btnStartAll.TabIndex = 20;
            this.btnStartAll.Text = "START ALL";
            this.btnStartAll.UseVisualStyleBackColor = false;
            // 
            // TrafficInstall
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(387, 556);
            this.Controls.Add(this.btnStartAll);
            this.Controls.Add(this.txtPackage);
            this.Controls.Add(this.chkBackupRestorePackage);
            this.Controls.Add(this.cboScriptName);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txtNamePackage);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtPhoneServiceApi);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.cboPhoneService);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtMailApi);
            this.Controls.Add(this.chkChangeMail);
            this.Controls.Add(this.cboCountry);
            this.Controls.Add(this.cboIpTool);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtWifiPass);
            this.Controls.Add(this.chkConnectWifi);
            this.Controls.Add(this.cboTask);
            this.Controls.Add(this.label1);
            this.Name = "TrafficInstall";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Traffic Install";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cboTask;
        private System.Windows.Forms.CheckBox chkConnectWifi;
        private System.Windows.Forms.TextBox txtWifiPass;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cboIpTool;
        private System.Windows.Forms.ComboBox cboCountry;
        private System.Windows.Forms.TextBox txtMailApi;
        private System.Windows.Forms.CheckBox chkChangeMail;
        private System.Windows.Forms.ComboBox cboPhoneService;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtPhoneServiceApi;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtNamePackage;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cboScriptName;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtPackage;
        private System.Windows.Forms.CheckBox chkBackupRestorePackage;
        private System.Windows.Forms.Button btnStartAll;
    }
}