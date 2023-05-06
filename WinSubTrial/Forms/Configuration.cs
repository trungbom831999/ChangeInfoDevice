using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Utils;
using WinSubTrial.Enum;

namespace WinSubTrial
{
    public partial class Configuration : Form
    {
        public Configuration()
        {
            InitializeComponent();
        }

        private void Configuration_Load(object sender, EventArgs e)
        {
            if (Common.GlobalSettings.ContainsKey("settings"))
            {
                Common.Settings = JsonConvert.DeserializeObject<Settings>(Common.GlobalSettings["settings"].ToString());
            }

            chkRandomCarrierCode.Checked = Common.Settings.RandomCarrier;
            chkRandomPhone.Checked = Common.Settings.RandomPhone;
            chkCheckIponVysor.Checked = Common.Settings.CheckIpVysor;
            chkFixPadFgo.Checked = Common.Settings.FixPadFgo;
            chkWipeMailAfterChange.Checked = Common.Settings.ChangeLogoutMail;
            chkMoveAfterRestore.Checked = Common.Settings.MoveToRestore;
            chkRandomSdk.Checked = Common.Settings.RandomSDK;
            chkBypassSatetyNet.Checked = Common.Settings.BypassSafetyNet;

            txtWinAllFolder.Text = EnumWinAllFolder.disk;

            txtPackage.Text = Common.Settings.AppBackup;
            chkRemovePackageAfterChange.Checked = Common.Settings.ChangeRemoveApp;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Common.Settings.RandomCarrier = chkRandomCarrierCode.Checked;
            Common.Settings.RandomPhone = chkRandomPhone.Checked;
            Common.Settings.CheckIpVysor = chkCheckIponVysor.Checked;
            Common.Settings.FixPadFgo = chkFixPadFgo.Checked;
            Common.Settings.ChangeLogoutMail = chkWipeMailAfterChange.Checked;
            Common.Settings.MoveToRestore = chkMoveAfterRestore.Checked;
            Common.Settings.RandomSDK = chkRandomSdk.Checked;
            Common.Settings.BypassSafetyNet = chkBypassSatetyNet.Checked;

            txtWinAllFolder.Text = EnumWinAllFolder.disk;

            Common.Settings.AppBackup = txtPackage.Text;
            Common.Settings.ChangeRemoveApp = chkRemovePackageAfterChange.Checked;

            string settings = JsonConvert.SerializeObject(Common.Settings);
            Common.GlobalSettings["settings"] = settings;

            Common.SaveGlobalSettings(notify: false);

            Close();
        }
    }
}
