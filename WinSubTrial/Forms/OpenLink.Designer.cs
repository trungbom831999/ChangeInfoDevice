
namespace WinSubTrial
{
    partial class OpenLink
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
            this.cboLinks = new System.Windows.Forms.ComboBox();
            this.cboInstalledApps = new System.Windows.Forms.ComboBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnOpenStore = new System.Windows.Forms.Button();
            this.btnCheckIpApi = new System.Windows.Forms.Button();
            this.btnCheckIp = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.btnOpenLink = new System.Windows.Forms.Button();
            this.btnWipe = new System.Windows.Forms.Button();
            this.panel4 = new System.Windows.Forms.Panel();
            this.btnOnProxy = new System.Windows.Forms.Button();
            this.txtProxy = new System.Windows.Forms.TextBox();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // cboLinks
            // 
            this.cboLinks.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLinks.FormattingEnabled = true;
            this.cboLinks.Items.AddRange(new object[] {
            "https://pay.google.com",
            "https://pay.google.com/gp/w/u/0/home/paymentmethods",
            "https://play.google.com/store/account/orderhistory"});
            this.cboLinks.Location = new System.Drawing.Point(24, 3);
            this.cboLinks.Name = "cboLinks";
            this.cboLinks.Size = new System.Drawing.Size(325, 21);
            this.cboLinks.TabIndex = 1;
            // 
            // cboInstalledApps
            // 
            this.cboInstalledApps.FormattingEnabled = true;
            this.cboInstalledApps.Location = new System.Drawing.Point(24, 3);
            this.cboInstalledApps.Name = "cboInstalledApps";
            this.cboInstalledApps.Size = new System.Drawing.Size(325, 21);
            this.cboInstalledApps.TabIndex = 5;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnOpenStore);
            this.panel1.Controls.Add(this.btnCheckIpApi);
            this.panel1.Controls.Add(this.btnCheckIp);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(370, 32);
            this.panel1.TabIndex = 6;
            // 
            // btnOpenStore
            // 
            this.btnOpenStore.BackColor = System.Drawing.Color.DodgerBlue;
            this.btnOpenStore.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnOpenStore.ForeColor = System.Drawing.Color.White;
            this.btnOpenStore.Location = new System.Drawing.Point(259, 3);
            this.btnOpenStore.Name = "btnOpenStore";
            this.btnOpenStore.Size = new System.Drawing.Size(90, 23);
            this.btnOpenStore.TabIndex = 7;
            this.btnOpenStore.Text = "CH PLay App";
            this.btnOpenStore.UseVisualStyleBackColor = false;
            this.btnOpenStore.Click += new System.EventHandler(this.btnOpenStore_Click);
            // 
            // btnCheckIpApi
            // 
            this.btnCheckIpApi.BackColor = System.Drawing.Color.DodgerBlue;
            this.btnCheckIpApi.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCheckIpApi.ForeColor = System.Drawing.Color.White;
            this.btnCheckIpApi.Location = new System.Drawing.Point(150, 3);
            this.btnCheckIpApi.Name = "btnCheckIpApi";
            this.btnCheckIpApi.Size = new System.Drawing.Size(78, 23);
            this.btnCheckIpApi.TabIndex = 6;
            this.btnCheckIpApi.Text = "Check Ip API";
            this.btnCheckIpApi.UseVisualStyleBackColor = false;
            this.btnCheckIpApi.Click += new System.EventHandler(this.btnCheckIpApi_Click);
            // 
            // btnCheckIp
            // 
            this.btnCheckIp.BackColor = System.Drawing.Color.DodgerBlue;
            this.btnCheckIp.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCheckIp.ForeColor = System.Drawing.Color.White;
            this.btnCheckIp.Location = new System.Drawing.Point(36, 3);
            this.btnCheckIp.Name = "btnCheckIp";
            this.btnCheckIp.Size = new System.Drawing.Size(75, 23);
            this.btnCheckIp.TabIndex = 5;
            this.btnCheckIp.Text = "Check Ip";
            this.btnCheckIp.UseVisualStyleBackColor = false;
            this.btnCheckIp.Click += new System.EventHandler(this.btnCheckIp_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btnOpenLink);
            this.panel2.Controls.Add(this.cboLinks);
            this.panel2.Location = new System.Drawing.Point(12, 59);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(370, 60);
            this.panel2.TabIndex = 7;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.btnWipe);
            this.panel3.Controls.Add(this.cboInstalledApps);
            this.panel3.Location = new System.Drawing.Point(12, 134);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(370, 60);
            this.panel3.TabIndex = 8;
            // 
            // btnOpenLink
            // 
            this.btnOpenLink.BackColor = System.Drawing.Color.DodgerBlue;
            this.btnOpenLink.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnOpenLink.ForeColor = System.Drawing.Color.White;
            this.btnOpenLink.Location = new System.Drawing.Point(132, 30);
            this.btnOpenLink.Name = "btnOpenLink";
            this.btnOpenLink.Size = new System.Drawing.Size(75, 23);
            this.btnOpenLink.TabIndex = 6;
            this.btnOpenLink.Text = "Chrome";
            this.btnOpenLink.UseVisualStyleBackColor = false;
            this.btnOpenLink.Click += new System.EventHandler(this.btnOpenLink_Click);
            // 
            // btnWipe
            // 
            this.btnWipe.BackColor = System.Drawing.Color.DodgerBlue;
            this.btnWipe.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnWipe.ForeColor = System.Drawing.Color.White;
            this.btnWipe.Location = new System.Drawing.Point(132, 30);
            this.btnWipe.Name = "btnWipe";
            this.btnWipe.Size = new System.Drawing.Size(75, 23);
            this.btnWipe.TabIndex = 7;
            this.btnWipe.Text = "Wipe";
            this.btnWipe.UseVisualStyleBackColor = false;
            this.btnWipe.Click += new System.EventHandler(this.btnWipe_Click);
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.txtProxy);
            this.panel4.Controls.Add(this.btnOnProxy);
            this.panel4.Location = new System.Drawing.Point(12, 209);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(370, 61);
            this.panel4.TabIndex = 9;
            // 
            // btnOnProxy
            // 
            this.btnOnProxy.BackColor = System.Drawing.Color.DodgerBlue;
            this.btnOnProxy.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnOnProxy.ForeColor = System.Drawing.Color.White;
            this.btnOnProxy.Location = new System.Drawing.Point(132, 29);
            this.btnOnProxy.Name = "btnOnProxy";
            this.btnOnProxy.Size = new System.Drawing.Size(75, 23);
            this.btnOnProxy.TabIndex = 7;
            this.btnOnProxy.Text = "Proxy";
            this.btnOnProxy.UseVisualStyleBackColor = false;
            this.btnOnProxy.Click += new System.EventHandler(this.btnOnProxy_Click);
            // 
            // txtProxy
            // 
            this.txtProxy.Location = new System.Drawing.Point(24, 3);
            this.txtProxy.Name = "txtProxy";
            this.txtProxy.Size = new System.Drawing.Size(325, 20);
            this.txtProxy.TabIndex = 8;
            // 
            // OpenLink
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(394, 277);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "OpenLink";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Open Link";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OpenLink_FormClosed);
            this.Load += new System.EventHandler(this.OpenLink_Load);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cboLinks;
        private System.Windows.Forms.ComboBox cboInstalledApps;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnOpenStore;
        private System.Windows.Forms.Button btnCheckIpApi;
        private System.Windows.Forms.Button btnCheckIp;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnOpenLink;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button btnWipe;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Button btnOnProxy;
        private System.Windows.Forms.TextBox txtProxy;
    }
}