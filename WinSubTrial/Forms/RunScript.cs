using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinSubTrial
{
    public partial class RunScript : Form
    {
        public string ScriptFile = "";
        public int RunTimes = 1;
        public bool IsRun = false;
        
        public RunScript()
        {
            InitializeComponent();
        }

        private void RunScript_Load(object sender, EventArgs e)
        {
            cboScript.Items.AddRange((from string file in Directory.EnumerateFiles(@"C:\WINALL\winscript") select (Path.GetFileName(file))).ToArray());
            if (cboScript.Items.Count > 0) cboScript.SelectedIndex = 0;
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            IsRun = true;
            ScriptFile = cboScript.Text;
            RunTimes = (int) numRunTimes.Value;
            Utils.Debug.Log($"IsRun: {IsRun} | ScriptFile:{ScriptFile} | RunTimes:{RunTimes}");
            Close();
        }
    }
}
