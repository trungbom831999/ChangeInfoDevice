
namespace WinSubTrial
{
    partial class SubTrialSupport
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
            this.btnRun = new System.Windows.Forms.Button();
            this.txtNamePackage = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtMailFilePath = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cboCountry = new System.Windows.Forms.ComboBox();
            this.cboIpTool = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtWifiPass = new System.Windows.Forms.TextBox();
            this.chkConnectWifi = new System.Windows.Forms.CheckBox();
            this.cboTask = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnRun
            // 
            this.btnRun.BackColor = System.Drawing.Color.DodgerBlue;
            this.btnRun.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRun.Font = new System.Drawing.Font("Segoe UI", 9.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRun.ForeColor = System.Drawing.Color.White;
            this.btnRun.Location = new System.Drawing.Point(154, 321);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(104, 36);
            this.btnRun.TabIndex = 33;
            this.btnRun.Text = "RUN";
            this.btnRun.UseVisualStyleBackColor = false;
            // 
            // txtNamePackage
            // 
            this.txtNamePackage.Location = new System.Drawing.Point(119, 172);
            this.txtNamePackage.Multiline = true;
            this.txtNamePackage.Name = "txtNamePackage";
            this.txtNamePackage.Size = new System.Drawing.Size(257, 124);
            this.txtNamePackage.TabIndex = 32;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(15, 175);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(80, 13);
            this.label6.TabIndex = 31;
            this.label6.Text = "Name|Package";
            // 
            // txtMailFilePath
            // 
            this.txtMailFilePath.Location = new System.Drawing.Point(119, 135);
            this.txtMailFilePath.Name = "txtMailFilePath";
            this.txtMailFilePath.Size = new System.Drawing.Size(257, 20);
            this.txtMailFilePath.TabIndex = 30;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(15, 135);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(70, 13);
            this.label5.TabIndex = 29;
            this.label5.Text = "Mail Path File";
            // 
            // cboCountry
            // 
            this.cboCountry.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCountry.FormattingEnabled = true;
            this.cboCountry.Location = new System.Drawing.Point(288, 96);
            this.cboCountry.Name = "cboCountry";
            this.cboCountry.Size = new System.Drawing.Size(88, 21);
            this.cboCountry.TabIndex = 28;
            // 
            // cboIpTool
            // 
            this.cboIpTool.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboIpTool.FormattingEnabled = true;
            this.cboIpTool.Items.AddRange(new object[] {
            "Manual",
            "NordVPN",
            "Key Tinsoft"});
            this.cboIpTool.Location = new System.Drawing.Point(119, 96);
            this.cboIpTool.Name = "cboIpTool";
            this.cboIpTool.Size = new System.Drawing.Size(163, 21);
            this.cboIpTool.TabIndex = 27;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 99);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(83, 13);
            this.label3.TabIndex = 26;
            this.label3.Text = "IP Fake Method";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(151, 55);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 13);
            this.label2.TabIndex = 25;
            this.label2.Text = "Wifi|Pass";
            // 
            // txtWifiPass
            // 
            this.txtWifiPass.Location = new System.Drawing.Point(207, 52);
            this.txtWifiPass.Name = "txtWifiPass";
            this.txtWifiPass.Size = new System.Drawing.Size(169, 20);
            this.txtWifiPass.TabIndex = 24;
            // 
            // chkConnectWifi
            // 
            this.chkConnectWifi.AutoSize = true;
            this.chkConnectWifi.Location = new System.Drawing.Point(18, 55);
            this.chkConnectWifi.Name = "chkConnectWifi";
            this.chkConnectWifi.Size = new System.Drawing.Size(109, 17);
            this.chkConnectWifi.TabIndex = 23;
            this.chkConnectWifi.Text = "Connect Wifi App";
            this.chkConnectWifi.UseVisualStyleBackColor = true;
            // 
            // cboTask
            // 
            this.cboTask.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTask.FormattingEnabled = true;
            this.cboTask.Items.AddRange(new object[] {
            "Wipe Recovery",
            "Login + Add PTTT + Backup",
            "Login + Add PTTT + Sub + Backup",
            "Change Backup Payment"});
            this.cboTask.Location = new System.Drawing.Point(89, 11);
            this.cboTask.Name = "cboTask";
            this.cboTask.Size = new System.Drawing.Size(287, 21);
            this.cboTask.TabIndex = 22;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 13);
            this.label1.TabIndex = 21;
            this.label1.Text = "Task";
            // 
            // SubTrialSupport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(396, 369);
            this.Controls.Add(this.btnRun);
            this.Controls.Add(this.txtNamePackage);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtMailFilePath);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.cboCountry);
            this.Controls.Add(this.cboIpTool);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtWifiPass);
            this.Controls.Add(this.chkConnectWifi);
            this.Controls.Add(this.cboTask);
            this.Controls.Add(this.label1);
            this.Name = "SubTrialSupport";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Sub Trial Support";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.TextBox txtNamePackage;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtMailFilePath;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cboCountry;
        private System.Windows.Forms.ComboBox cboIpTool;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtWifiPass;
        private System.Windows.Forms.CheckBox chkConnectWifi;
        private System.Windows.Forms.ComboBox cboTask;
        private System.Windows.Forms.Label label1;
    }
}