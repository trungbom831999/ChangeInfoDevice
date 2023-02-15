using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinSubTrial.Utilities;

namespace WinSubTrial
{
    public partial class WinName : Form
    {
        public string Serial;
        public string Name = "";
        public WinName()
        {
            InitializeComponent();
        }

        private void btnChange_Click(object sender, EventArgs e)
        {
            if(txtName.Text.Length == 0 || !Regex.IsMatch(txtName.Text, "^[a-zA-Z0-9_-]+$"))
            {
                Common.Info("Tên chỉ được gồm các ký tự đơn giản: a-z, A-Z, 0-9, _, -");
                return;
            }

            Name = txtName.Text;
            new Device { Serial = Serial, Name = Name }.SetWinName();
            Close();
        }

        private void WinName_Load(object sender, EventArgs e)
        {
            txtName.Text = Name;
        }
    }
}
