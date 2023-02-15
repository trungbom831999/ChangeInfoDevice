using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utils;

namespace WinSubTrial
{
    public partial class WipePackages : Form
    {
        public WipePackages()
        {
            InitializeComponent();
        }

        private void WipePackages_Load(object sender, EventArgs e)
        {
            txtPackage.Lines = Common.FullWipePackages.ToArray();
            if(txtPackage.Lines.Length < 1)
            {
                txtPackage.Lines = Common.DefaultWipePackages;
                Common.FullWipePackages = Common.DefaultWipePackages.ToList();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Common.FullWipePackages = txtPackage.Lines.ToList();
            MyFile.WriteAllLines("Data\\WipePackages.txt", txtPackage.Lines);
            Close();
        }
    }
}
