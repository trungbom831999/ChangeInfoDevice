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
    public partial class OpenLink : Form
    {
        public List<string> commands = new List<string> { "nothing" };

        string serial;

        public OpenLink(string _serial)
        {
            if (_serial.Length < 1)
            {
                Common.Info("Please select a device");
                return;
            }

            serial = _serial;
            InitializeComponent();
        }

        private void OpenLink_Load(object sender, EventArgs e)
        {
            try
            {
                txtProxy.Text = Common.ProxyHost + ":" + Common.ProxyPort;
                cboLinks.SelectedIndex = 0;

                cboInstalledApps.Items.AddRange(Adb.GetInstalledPackages(serial));
                cboInstalledApps.SelectedIndex = 0;
            }
            catch
            {
                Common.Info("No installed package found");
            }
        }

        private void btnCheckIp_Click(object sender, EventArgs e)
        {
            commands = new List<string>{ "openurl", "http://ip-api.com/line", "" };
            Close();
        }

        private void btnCheckIpApi_Click(object sender, EventArgs e)
        {
            commands = new List<string> { "checkipapi", "http://ip-api.com/line" };
            Close();
        }

        private void btnOpenStore_Click(object sender, EventArgs e)
        {
            commands = new List<string> { "openapp", "com.android.vending" };
            Close();
        }

        private void btnOpenLink_Click(object sender, EventArgs e)
        {
            commands = new List<string> {"openurl", cboLinks.Text, "com.android.chrome/com.google.android.apps.chrome.Main" };
            Close();
        }

        private void btnWipe_Click(object sender, EventArgs e)
        {
            commands = new List<string> { "wipeapp", cboInstalledApps.Text };
            Close();
        }

        private void btnOnProxy_Click(object sender, EventArgs e)
        {
            commands = new List<string> { "proxyon", txtProxy.Text };
            Close();
        }

        private void OpenLink_FormClosed(object sender, FormClosedEventArgs e)
        {

        }
    }
}
