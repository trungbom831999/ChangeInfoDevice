using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinSubTrial.Utilities;

namespace WinSubTrial
{
    public partial class SystemPackages : Form
    {
        string serial;
        public SystemPackages(string serial)
        {
            InitializeComponent();
            this.serial = serial;
        }

        private void SystemPackages_Load(object sender, EventArgs e)
        {
            txtPackage.Lines = Adb.GetSystemPackages(serial);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
