
namespace WinSubTrial
{
    partial class RunScript
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
            this.label2 = new System.Windows.Forms.Label();
            this.cboScript = new System.Windows.Forms.ComboBox();
            this.btnRun = new System.Windows.Forms.Button();
            this.numRunTimes = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.numRunTimes)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Tên Script";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Số Lần Chạy";
            // 
            // cboScript
            // 
            this.cboScript.FormattingEnabled = true;
            this.cboScript.Location = new System.Drawing.Point(86, 6);
            this.cboScript.Name = "cboScript";
            this.cboScript.Size = new System.Drawing.Size(185, 21);
            this.cboScript.TabIndex = 2;
            // 
            // btnRun
            // 
            this.btnRun.BackColor = System.Drawing.Color.DodgerBlue;
            this.btnRun.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnRun.ForeColor = System.Drawing.SystemColors.Window;
            this.btnRun.Location = new System.Drawing.Point(99, 68);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(81, 23);
            this.btnRun.TabIndex = 4;
            this.btnRun.Text = "Bắt Đầu Chạy";
            this.btnRun.UseVisualStyleBackColor = false;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // numRunTimes
            // 
            this.numRunTimes.Location = new System.Drawing.Point(86, 34);
            this.numRunTimes.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numRunTimes.Name = "numRunTimes";
            this.numRunTimes.Size = new System.Drawing.Size(185, 20);
            this.numRunTimes.TabIndex = 5;
            this.numRunTimes.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // RunScript
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(283, 103);
            this.Controls.Add(this.numRunTimes);
            this.Controls.Add(this.btnRun);
            this.Controls.Add(this.cboScript);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "RunScript";
            this.Text = "RunScript";
            this.Load += new System.EventHandler(this.RunScript_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numRunTimes)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cboScript;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.NumericUpDown numRunTimes;
    }
}