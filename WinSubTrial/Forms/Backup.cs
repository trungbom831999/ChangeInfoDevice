using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinSubTrial
{
    public partial class Backup : Form
    {
        public Backup()
        {
            InitializeComponent();
        }

        public string SaveDir { get; internal set; }

        private void Backup_Load(object sender, EventArgs e)
        {
            cboFolder.Text = Common.GlobalSettings["prevBackupDir"].ToString();
        }

        private void btnBackup_Click(object sender, EventArgs e)
        {
            SaveDir = cboFolder.Text;
            // if(!Regex.IsMatch(SaveDir, "^\\[a-zA-Z0-9]+]$"))
            // {
            //    Common.Info("Định dạng folder không đúng. VD chuẩn: \\NewBackup123");
            //    return;
            // }
            if (!Directory.Exists($@"C:\WINALL\winbackup{SaveDir}"))
            {
                Directory.CreateDirectory($@"C:\WINALL\winbackup{SaveDir}");
            }

            Common.GlobalSettings["prevBackupDir"] = SaveDir;
            Close();
        }

        private void cboFolder_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Return))
            {
                btnBackup_Click(null, null);
            }
        }
    }
}
