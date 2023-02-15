
namespace WinSubTrial
{
    partial class DeviceTools
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            this.chkSelectAll = new System.Windows.Forms.CheckBox();
            this.label9 = new System.Windows.Forms.Label();
            this.dgvDevice = new System.Windows.Forms.DataGridView();
            this.deviceSelect = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.deviceName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.deviceSerial = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.deviceKey = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.taskStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDevice)).BeginInit();
            this.SuspendLayout();
            // 
            // chkSelectAll
            // 
            this.chkSelectAll.AutoSize = true;
            this.chkSelectAll.Location = new System.Drawing.Point(24, 25);
            this.chkSelectAll.Name = "chkSelectAll";
            this.chkSelectAll.Size = new System.Drawing.Size(15, 14);
            this.chkSelectAll.TabIndex = 47;
            this.chkSelectAll.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(243)))), ((int)(((byte)(244)))));
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.5F, System.Drawing.FontStyle.Bold);
            this.label9.ForeColor = System.Drawing.Color.DimGray;
            this.label9.Location = new System.Drawing.Point(635, 24);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(53, 15);
            this.label9.TabIndex = 46;
            this.label9.Text = "Actions";
            // 
            // dgvDevice
            // 
            this.dgvDevice.AllowUserToAddRows = false;
            this.dgvDevice.AllowUserToDeleteRows = false;
            this.dgvDevice.AllowUserToResizeColumns = false;
            this.dgvDevice.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.ButtonFace;
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.DimGray;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(253)))), ((int)(((byte)(236)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.DimGray;
            this.dgvDevice.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvDevice.BackgroundColor = System.Drawing.Color.White;
            this.dgvDevice.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgvDevice.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dgvDevice.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(243)))), ((int)(((byte)(244)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.DimGray;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.ControlDarkDark;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.WindowFrame;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvDevice.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvDevice.ColumnHeadersHeight = 39;
            this.dgvDevice.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvDevice.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.deviceSelect,
            this.deviceName,
            this.deviceSerial,
            this.deviceKey,
            this.taskStatus});
            this.dgvDevice.Cursor = System.Windows.Forms.Cursors.Default;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.DimGray;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(253)))), ((int)(((byte)(236)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.DimGray;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvDevice.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgvDevice.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvDevice.EnableHeadersVisualStyles = false;
            this.dgvDevice.GridColor = System.Drawing.Color.Salmon;
            this.dgvDevice.Location = new System.Drawing.Point(9, 9);
            this.dgvDevice.Margin = new System.Windows.Forms.Padding(0);
            this.dgvDevice.MultiSelect = false;
            this.dgvDevice.Name = "dgvDevice";
            this.dgvDevice.ReadOnly = true;
            this.dgvDevice.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.ActiveBorder;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvDevice.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dgvDevice.RowHeadersVisible = false;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.Transparent;
            this.dgvDevice.RowsDefaultCellStyle = dataGridViewCellStyle5;
            this.dgvDevice.RowTemplate.DefaultCellStyle.BackColor = System.Drawing.Color.Transparent;
            this.dgvDevice.RowTemplate.Height = 25;
            this.dgvDevice.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvDevice.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dgvDevice.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvDevice.ShowEditingIcon = false;
            this.dgvDevice.Size = new System.Drawing.Size(834, 269);
            this.dgvDevice.TabIndex = 45;
            this.dgvDevice.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvDevice_CellContentClick);
            // 
            // deviceSelect
            // 
            this.deviceSelect.HeaderText = "";
            this.deviceSelect.MinimumWidth = 42;
            this.deviceSelect.Name = "deviceSelect";
            this.deviceSelect.ReadOnly = true;
            this.deviceSelect.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.deviceSelect.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.deviceSelect.Width = 42;
            // 
            // deviceName
            // 
            this.deviceName.HeaderText = "Device";
            this.deviceName.Name = "deviceName";
            this.deviceName.ReadOnly = true;
            this.deviceName.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.deviceName.Width = 110;
            // 
            // deviceSerial
            // 
            this.deviceSerial.HeaderText = "Serial";
            this.deviceSerial.Name = "deviceSerial";
            this.deviceSerial.ReadOnly = true;
            this.deviceSerial.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.deviceSerial.Width = 95;
            // 
            // deviceKey
            // 
            this.deviceKey.HeaderText = "Key";
            this.deviceKey.Name = "deviceKey";
            this.deviceKey.ReadOnly = true;
            this.deviceKey.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.deviceKey.Width = 199;
            // 
            // taskStatus
            // 
            this.taskStatus.HeaderText = "Status";
            this.taskStatus.Name = "taskStatus";
            this.taskStatus.ReadOnly = true;
            this.taskStatus.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.taskStatus.Width = 225;
            // 
            // DeviceTools
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(853, 288);
            this.Controls.Add(this.chkSelectAll);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.dgvDevice);
            this.MaximizeBox = false;
            this.Name = "DeviceTools";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Devices";
            this.Load += new System.EventHandler(this.DeviceTools_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDevice)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkSelectAll;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.DataGridView dgvDevice;
        private System.Windows.Forms.DataGridViewCheckBoxColumn deviceSelect;
        private System.Windows.Forms.DataGridViewTextBoxColumn deviceName;
        private System.Windows.Forms.DataGridViewTextBoxColumn deviceSerial;
        private System.Windows.Forms.DataGridViewTextBoxColumn deviceKey;
        private System.Windows.Forms.DataGridViewTextBoxColumn taskStatus;
    }
}