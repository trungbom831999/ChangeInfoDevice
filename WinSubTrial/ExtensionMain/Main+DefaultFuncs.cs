using Newtonsoft.Json;
using SharpAdbClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utils;
using WinSubTrial.Functions;
using WinSubTrial.Utilities;
using Debug = System.Diagnostics.Debug;

namespace WinSubTrial
{

    public partial class Main
    {

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            Common.SaveGlobalSettings(false);
        }



        #region ToolStripMenuItem events
        private void tabControl1_DrawItem(object sender, DrawItemEventArgs e)
        {
            Graphics g = e.Graphics;
            Brush _textBrush;

            // Get the item from the collection.
            TabPage _tabPage = tabControl1.TabPages[e.Index];

            // Get the real bounds for the tab rectangle.
            Rectangle _tabBounds = tabControl1.GetTabRect(e.Index);

            if (e.State == DrawItemState.Selected)
            {

                // Draw a different background color, and don't paint a focus rectangle.
                _textBrush = new SolidBrush(Color.FromArgb(0, 153, 250));
                g.FillRectangle(Brushes.AliceBlue, e.Bounds);
            }
            else
            {
                _textBrush = new SolidBrush(e.ForeColor);
                e.DrawBackground();
            }

            // Use our own font.
            Font _tabFont = new Font("Segoe UI", 12.0f, FontStyle.Regular, GraphicsUnit.Pixel);

            // Draw string. Center the text.
            StringFormat _stringFlags = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            g.DrawString(_tabPage.Text, _tabFont, _textBrush, _tabBounds, new StringFormat(_stringFlags));
        }

        private void configurationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (Configuration form = new Configuration())
            {
                form.ShowDialog();
            }
        }

        private void checkFileBackupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //using (Configuration form = new Configuration())
            //{
            //    form.ShowDialog();
            //}
        }

        private void saveSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Common.SaveGlobalSettings();
        }

        private void loadSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Common.LoadGlobalSettings();
        }

        private void wipePackagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (WipePackages form = new WipePackages())
            {
                form.ShowDialog();
            }
        }

        private void installPackagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                if (!viewModel.selectedOneDevice()) return;
                using (InstalledPackages form = new InstalledPackages(viewModel.devicesModel.FirstOrDefault(x => x.isSelected == true).Serial))
                {
                    form.ShowDialog();
                }
            });
        }

        private void systemPackagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!viewModel.selectedOneDevice()) return;
            Task.Run(() =>
            {
                using (SystemPackages form = new SystemPackages(viewModel.devicesModel.FirstOrDefault(x => x.isSelected == true).Serial))
                {
                    form.ShowDialog();
                }
            });
        }
        #endregion ToolStripMenuItem events

        #region Button Events
        private void btnResetDPI_Click(object sender, EventArgs e)
        {
            if (!viewModel.selectedOneDevice()) return;
            Task.Run(() =>
            {
                Funcs.ResetDPI(viewModel.devicesModel.FirstOrDefault(x => x.isSelected == true).Serial);
            });
        }

        private void btnGoPaymentMethod_Click(object sender, EventArgs e)
        {
            if (!viewModel.someDevicesSelected()) return;

            Thread thread = new Thread(new ThreadStart(() =>
            {
                foreach (Device device in viewModel.devicesModel.Where(x => x.isSelected == true))
                {
                    Common.SetStatus(device.Serial, "Open payment method on all devices");
                    Task.Run(() =>
                    {
                        Funcs.OpenPaymentMethods(device.Serial);
                    });
                }
            }))
            { IsBackground = true };
            thread.Start();
        }

        private void btnLoginGmailAll_Click(object sender, EventArgs e)
        {
            if (!viewModel.someDevicesSelected()) return;
            Thread thread = new Thread(new ThreadStart(() =>
            {
                foreach (Device device in viewModel.devicesModel.Where(x => x.isSelected == true))
                {
                    Task.Run(() =>
                    {
                        viewModel.deviceWaitForStop[device.Serial] = false;
                        viewModel.LoginAll(device.Serial);
                    });
                }
            }))
            { IsBackground = true };
            thread.Start();
        }

        private void btnRandomInfoAll_Click(object sender, EventArgs e)
        {
            if (!viewModel.someDevicesSelected()) return;
            Thread thread = new Thread(new ThreadStart(() =>
            {
                foreach (Device device in viewModel.devicesModel.Where(x => x.isSelected == true))
                {
                    viewModel.deviceWaitForStop[device.Serial] = false;
                    viewModel.SaveInfoAll(device.Serial);
                }
            }))
            { IsBackground = true };
            thread.Start();
        }

        private void btnSaveInfoAll_Click(object sender, EventArgs e)
        {
            if (!viewModel.someDevicesSelected()) return;
            Thread thread = new Thread(new ThreadStart(() =>
            {
                foreach (Device device in viewModel.devicesModel.Where(x => x.isSelected == true))
                {
                    viewModel.deviceWaitForStop[device.Serial] = false;
                    viewModel.SaveInfoAll(device.Serial);
                }
            }))
            { IsBackground = true };
            thread.Start();
        }
        private void btnWipePackageAll_Click(object sender, EventArgs e)
        {
            string serial;
            if (!viewModel.someDevicesSelected()) return;
            foreach (Device element in viewModel.devicesModel.Where(x => x.isSelected == true))
            {
                serial = element.Serial;

                if (viewModel.IsDeviceInTask(serial))
                {
                    Common.SetStatus(serial, "Please wait current task completed");
                    continue;
                }

                Thread thread = new Thread(new ThreadStart(() =>
                {
                    Device device = viewModel.devicesModel.FirstOrDefault(x => x.Serial.Equals(serial));
                    new Change { device = device }.WipeAppsData();
                    Common.SetStatus(device.Serial, "Wipe packages done");
                }))
                { IsBackground = true };
                viewModel.deviceThreads[serial] = thread;
                thread.Start();
            }
        }

        private void btnWipeMailAll_Click(object sender, EventArgs e)
        {
            string serial;
            if (!viewModel.someDevicesSelected()) return;
            foreach (Device element in viewModel.devicesModel.Where(x => x.isSelected == true))
            {
                serial = element.Serial;
                if (viewModel.IsDeviceInTask(serial))
                {
                    Common.SetStatus(serial, "Please wait current task completed");
                    continue;
                }
                Thread thread = new Thread(new ThreadStart(() =>
                {
                    Device device = viewModel.devicesModel.FirstOrDefault(x => x.Serial.Equals(serial));
                    Common.SetStatus(device.Serial, "Wipe mail...");
                    new Change { device = device }.WipeApps();
                    Funcs.WipeMail(device.Serial);
                    Adb.RebootFast(device.Serial);
                    Common.SetStatus(device.Serial, "Wipe mail done");
                }))
                { IsBackground = true };
                viewModel.deviceThreads[serial] = thread;
                thread.Start();
            }
        }

        private void btnOffProxyAll_Click(object sender, EventArgs e)
        {
            
            string serial;
            if (!viewModel.someDevicesSelected()) return;
            foreach (Device element in viewModel.devicesModel.Where(x => x.isSelected == true))
            {
                serial = element.Serial;

                if (viewModel.IsDeviceInTask(serial))
                {
                    Common.SetStatus(serial, "Please wait current task completed");
                    continue;
                }

                Thread thread = new Thread(new ThreadStart(() =>
                {
                    Device device = viewModel.devicesModel.FirstOrDefault(x => x.Serial.Equals(serial));
                    Common.SetStatus(device.Serial, "Turn off proxy...");
                    Funcs.ProxyOff(device.Serial);
                    Common.SetStatus(device.Serial, "Turn off proxy done");
                }))
                { IsBackground = true };
                viewModel.deviceThreads[serial] = thread;
                thread.Start();
            }
        }

        private void btnHomeAll_Click(object sender, EventArgs e)
        {
            if (!viewModel.someDevicesSelected()) return;
            Thread thread = new Thread(new ThreadStart(() =>
            {
                foreach (Device device in viewModel.devicesModel.Where(x => x.isSelected == true))
                {
                    Task.Run(() =>
                    {
                        Adb.SendKey(device.Serial, "KEYCODE_HOME");
                    });
                }
            }))
            { IsBackground = true };
            thread.Start();
        }

        private void btnRestore_Click(object sender, EventArgs e)
        {
            string bk_packages = Common.Settings.AppBackup;
            if (!viewModel.selectedOneDevice()) return;
            string serial = serial = viewModel.devicesModel.FirstOrDefault(x => x.isSelected == true).Serial;
            if (viewModel.IsDeviceInTask(serial))
            {
                Common.Info("Vui lòng đợi thao tác trước đó hoàn thành");
                return;
            }
            BackupData backup = Common.ListBackups.FirstOrDefault(x => x.Name.Equals(cboBackupFiles.Text));
            if (backup == default)
            {
                Common.Info("File backup đã chọn không tồn tại!");
                return;
            }

            Thread thread = new Thread(new ThreadStart(() =>
            {
                Device device = viewModel.devicesModel.FirstOrDefault(x => x.Serial.Equals(serial));
                Common.SetStatus(device.Serial, $"Restoring...");
                bool ok = new Functions.Backup { device = device }.Restore($@"C:\WINALL\winbackup\{backup.Folder}\{backup.Name}", bk_packages);
                Common.SetStatus(device.Serial, $"Restore " + (ok ? "done" : "fail"));
            }))
            { IsBackground = true };
            viewModel.deviceThreads[serial] = thread;
            thread.Start();
        }

        private void btnCreateScript_Click(object sender, EventArgs e)
        {
            Process.Start("Tool\\Script_Creator.exe", @"C:\WINALL\winscript");
        }

        private void btnRunScript_Click(object sender, EventArgs e)
        {
            if (!viewModel.selectedOneDevice()) return;
            string serial = serial = viewModel.devicesModel.FirstOrDefault(x => x.isSelected == true).Serial;

            if (viewModel.IsDeviceInTask(serial))
            {
                Common.Info("Vui lòng đợi tác vụ trước đó hoàn thành");
                return;
            }
            using (RunScript runScript = new RunScript())
            {
                runScript.ShowDialog();
                if (runScript.IsRun)
                {
                    viewModel.deviceThreads[serial] = new Thread(new ThreadStart(() =>
                    {
                        Script script = new Script(serial, runScript.ScriptFile);
                        for (int i = 1; i <= runScript.RunTimes; i++)
                        {
                            Common.SetStatus(serial, $"Run Script #{i}");
                            script.Run();
                            Common.SetStatus(serial, $"Run Script #{i} done");
                            Common.Sleep(2000);
                        }
                    }))
                    { IsBackground = true };
                    viewModel.deviceThreads[serial].Start();
                }
            }
        }
        #endregion Button Events

        #region Combobox Events
        private void cboBackupFiles_MouseClick(object sender, MouseEventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            cboBackupFiles.Items.Clear();

            Common.LoadBackupList();
            if (Common.ListBackups.Count == 0) return;

            if (txtSearchBackup.Text.Equals("Search")) txtSearchBackup.Text = "";
            string searchText = txtSearchBackup.Text;

            cboBackupFiles.Items.AddRange((from BackupData backup in Common.ListBackups
                                           where backup.Name.Contains(searchText)
                                           select backup.Name).ToArray());
            if (cboBackupFiles.Items.Count > 0) cboBackupFiles.SelectedIndex = 0;
            Cursor.Current = Cursors.Default;
        }

        private void cboCountry_SelectedIndexChanged(object sender, EventArgs e)
        {
            viewModel.countryChosen = cboCountry.Text;
            viewModel.operatorNetWork = (from Carrier carrier in viewModel.networkList.First(x => x.code.Equals(viewModel.countryChosen)).operators select carrier.name).ToList();
            LoadCarrier();
        }
        private void networkSelectedIndexChanged(object sender, EventArgs e)
        {
            viewModel.operatorChosen = cboOperator.Text;
        }

        private void cboBrandSelectedIndexChanged(object sender, EventArgs e)
        {
            viewModel.brand = cboBrand.Text;
        }
        #endregion Combobox Events

        #region Checkbox Events
        private void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dgvDevice.Rows)
            {
                row.Cells["deviceSelect"].Value = chkSelectAll.Checked;
            }
            foreach (Device device in viewModel.devicesModel)
            {
                device.isSelected = chkSelectAll.Checked;
            }
        }
        #endregion Checkbox Events

        #region Datagridview Events
        private void dgvDevice_MouseDown(object sender, MouseEventArgs e)
        {
            Cursor.Current = Cursors.Hand;
        }
        private void dgvDevice_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvDevice.Rows.Count == 0 || dgvDevice.SelectedRows.Count == 0)
            {
                //Common.Info("SelectionChanged: " + selectedDevice);
                return;
            }
            DataGridViewRow selectedDevice = dgvDevice.SelectedRows[0];
            selectedDevice.Cells["taskStatus"].Style.ForeColor = Color.Red;
            //Common.Info("SelectionChanged: " + selectedDevice.Cells["deviceName"].Value);

            for (int i = 0; i < dgvDevice.Rows.Count; i++)
            {
                dgvDevice.Rows[i].DefaultCellStyle.BackColor = i % 2 == 0 ? SystemColors.Window : SystemColors.ButtonFace;
            }
            selectedDevice.DefaultCellStyle.BackColor = Color.FromArgb(196, 253, 236);
        }
        private void dgvDevice_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
            {
                return;
            }
            Action action = default;
            int rowIndex = e.RowIndex;
            DataGridViewRow row = dgvDevice.Rows[rowIndex];
            string serial = row.Cells["deviceSerial"].Value.ToString();
            string title = row.Cells["deviceName"].Value.ToString();
            switch (e.ColumnIndex)
            {
                case 0:
                    if (row.Cells["deviceSelect"].Value != null && (bool)row.Cells["deviceSelect"].Value)
                    {
                        row.Cells["deviceSelect"].Value = false;
                        viewModel.devicesModel.FirstOrDefault(x => x.Serial.Equals(serial)).isSelected = false;
                    }
                    else
                    {
                        row.Cells["deviceSelect"].Value = true;
                        viewModel.devicesModel.FirstOrDefault(x => x.Serial.Equals(serial)).isSelected = true;
                    }
                    break;
                default:
                    break;
            }
            if (action != default)
            {
                Task.Run(action);
            }
        }
        private void dgvDevice_MouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if ((e.Button == MouseButtons.Right) && (e.RowIndex != -1 && e.ColumnIndex != -1))
            {
                int rowIndex = e.RowIndex;
                DataGridViewRow row = dgvDevice.Rows[rowIndex];
                string serial = row.Cells["deviceSerial"].Value.ToString();
                string title = row.Cells["deviceName"].Value.ToString();

                ContextMenu m = new ContextMenu();
                MenuItem menuItem1 = new MenuItem("Remote");
                MenuItem menuItem2 = new MenuItem("Check IP");
                MenuItem menuItem3 = new MenuItem("Turn off Proxy");
                MenuItem menuItem4 = new MenuItem("Capture screen");
                MenuItem menuItem5 = new MenuItem("Open Link");
                MenuItem menuItem6 = new MenuItem("Reboot device");
                MenuItem menuItem7 = new MenuItem("Stop Auto");
                MenuItem menuItem8 = new MenuItem("Run Last Email");
                MenuItem menuItem9 = new MenuItem("Rename");

                menuItem1.Click += new System.EventHandler((c, d) => menuItem1_Click(sender, e, serial, title));
                menuItem2.Click += new System.EventHandler((c, d) => menuItem2_Click(sender, e, serial, title, rowIndex));
                menuItem3.Click += new System.EventHandler((c, d) => menuItem3_Click(sender, e, serial, title, rowIndex));
                menuItem4.Click += new System.EventHandler((c, d) => menuItem4_Click(sender, e, serial, title, rowIndex));
                menuItem5.Click += new System.EventHandler((c, d) => menuItem5_Click(sender, e, serial, title, rowIndex));
                menuItem6.Click += new System.EventHandler((c, d) => menuItem6_Click(sender, e, serial, title, rowIndex));
                menuItem7.Click += new System.EventHandler((c, d) => menuItem7_Click(sender, e, serial, title));
                menuItem8.Click += new System.EventHandler((c, d) => menuItem8_Click(sender, e, serial, title));
                menuItem9.Click += new System.EventHandler((c, d) => menuItem9_Click(sender, e, serial, title, rowIndex));
                MenuItem[] range = { menuItem1, menuItem2, menuItem3, menuItem4, menuItem5, menuItem6, menuItem7, menuItem8, menuItem9 };
                m.MenuItems.AddRange(range);
                m.Show(dgvDevice, dgvDevice.PointToClient(Cursor.Position));
            }
        }

        private void menuItem1_Click(object sender, System.EventArgs e, string serial, string title)
        {
            Funcs.ShowVysor(serial, title);
        }
        private void menuItem2_Click(object sender, System.EventArgs e, string serial, string title, int rowIndex)
        {
            DataGridViewRow row = dgvDevice.Rows[rowIndex];
            row.Cells["taskStatus"].Value = "Opening ip api url...";
            Adb.OpenUrl(serial, "http://ip-api.com/line");
            row.Cells["taskStatus"].Value = "Open ip api url done";
        }
        private void menuItem3_Click(object sender, System.EventArgs e, string serial, string title, int rowIndex)
        {
            DataGridViewRow row = dgvDevice.Rows[rowIndex];
            row.Cells["taskStatus"].Value = "Off Proxy done";
            Funcs.ProxyOff(serial);
            row.Cells["taskStatus"].Value = "Off Proxy done";
        }
        private void menuItem4_Click(object sender, System.EventArgs e, string serial, string title, int rowIndex)
        {
            DataGridViewRow row = dgvDevice.Rows[rowIndex];
            row.Cells["taskStatus"].Value = "Capture screen...";
            string fileName = "screen_" + serial;
            Funcs.CaptureScreen(serial, fileName);
            row.Cells["taskStatus"].Value = "Capture screen done";
        }
        private void menuItem5_Click(object sender, System.EventArgs e, string serial, string title, int rowIndex)
        {
            viewModel.OpenLinkForm(serial);
        }
        private void menuItem6_Click(object sender, System.EventArgs e, string serial, string title, int rowIndex)
        {
            DataGridViewRow row = dgvDevice.Rows[rowIndex];
            row.Cells["taskStatus"].Value = "Rebooting...";
            Adb.Reboot(serial);
            row.Cells["taskStatus"].Value = "Rebooted";
        }
        private void menuItem7_Click(object sender, System.EventArgs e, string serial, string title)
        {
            viewModel.StopTask(serial);
        }
        private void menuItem8_Click(object sender, System.EventArgs e, string serial, string title)
        {
            viewModel.RunLastEmailAndStop(serial);
        }
        private void menuItem9_Click(object sender, System.EventArgs e, string serial, string title, int rowIndex)
        {
            Device device = viewModel.devicesModel.First(x => x.Serial.Equals(serial));
            DataGridViewRow row = dgvDevice.Rows[rowIndex];

            using (WinName winName = new WinName())
            {
                winName.Serial = serial;
                winName.Name = row.Cells["deviceName"].Value.ToString();
                winName.ShowDialog();
                device.Name = winName.Name;
                row.Cells["deviceName"].Value = device.Name;
                Common.UpdateName(device, viewModel.devicesModel);
            }
        }
        #endregion Datagridview Events

        #region Textbox Events
        private void txtSearchBackup_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtSearchBackup_Enter(object sender, EventArgs e)
        {
            if (txtSearchBackup.Text.Equals("Search")) txtSearchBackup.Text = "";
        }

        private void txtSearchBackup_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Return))
            {
                cboBackupFiles.Items.Clear();
                Cursor.Current = Cursors.WaitCursor;

                Common.LoadBackupList();
                if (Common.ListBackups.Count == 0)
                {
                    Cursor.Current = Cursors.Default;
                    return;
                }

                if (txtSearchBackup.Text.Equals("Search")) txtSearchBackup.Text = "";
                string searchText = txtSearchBackup.Text;

                cboBackupFiles.Items.AddRange((from BackupData backup in Common.ListBackups
                                               where backup.Name.Contains(searchText)
                                               select backup.Name).ToArray());
                if (cboBackupFiles.Items.Count > 0) cboBackupFiles.SelectedIndex = 0;
                Cursor.Current = Cursors.Default;
            }
        }
        #endregion Textbox Events

        #region functions

        private void LoadNetwork()
        {
            cboCountry.Items.Clear();
            cboCountry.Items.AddRange(viewModel.listCountry.ToArray());
            cboCountry.Text = viewModel.countryChosen;
            viewModel.isLoaded = true;
            LoadCarrier();
        }
        private void LoadCarrier()
        {
            if (viewModel.isLoaded == false)
            {
                return;
            }
            cboOperator.Items.Clear();
            cboOperator.Items.AddRange(viewModel.operatorNetWork.ToArray());
            for (int i = 0; i < viewModel.operatorNetWork.Count; i++)
            {
                if (viewModel.operatorNetWork[i] == "Viettel Mobile")
                {
                    cboOperator.SelectedIndex = i;
                    return;
                }
            }
            cboOperator.SelectedIndex = 0;
        }

        private void FillListDevices()
        {
            lblDevicesCount.Text = viewModel.devicesModel.Count + "";
            if (dgvDevice.Rows.Count > 0)
            {
                foreach (var item in dgvDevice.Rows.Cast<DataGridViewRow>().Where(r => !viewModel.devicesModel.Exists(x => x.Serial.Equals(r.Cells["deviceSerial"].Value.ToString()))))
                {
                    if (!viewModel.IsDeviceInTask(item.Cells["deviceSerial"].Value.ToString())) dgvDevice.Rows.Remove(item);
                    /*
                    if (IsDeviceInTask(item.Cells["deviceSerial"].Value.ToString()))
                    {
                        deviceThreads[item.Cells["deviceSerial"].Value.ToString()].Abort();
                    }
                    */
                }
            }
            foreach (Device device in viewModel.devicesModel)
            {
                try
                {
                    if (dgvDevice.Rows
                        .Cast<DataGridViewRow>()
                        .Where(r => r.Cells["deviceSerial"].Value.ToString().Equals(device.Serial)).ToList().Count > 0)
                    {
                        continue;
                    }
                }
                catch (Exception ex)
                {
                    Utils.Debug.Log($"Error check existing device: {ex.Message}");
                }

                try
                {
                    int index = dgvDevice.Rows.Add();

                    DataGridViewRow row = dgvDevice.Rows[index];
                    row.Height = 39;
                    if (index % 2 == 1) row.DefaultCellStyle.BackColor = SystemColors.ButtonFace;

                    row.Cells["deviceName"].Value = device.Name;
                    row.Cells["deviceSerial"].Value = device.Serial;
                    row.Cells["deviceKey"].Value = device.Key;
                    row.Cells["taskStatus"].Value = "Ready";
                    row.Cells["taskStatus"].Style.ForeColor = Color.Red;
                    viewModel.deviceTasks.Add(device.Serial, null);
                    viewModel.deviceThreads.Add(device.Serial, null);
                    viewModel.deviceWaitForStop.Add(device.Serial, false);
                }
                catch (Exception ex)
                {
                    Utils.Debug.Log($"Error add device to dgv: {ex.Message}");
                }
            }
        }

        private void ShowDeviceTools()
        {
            using (DeviceTools tools = new DeviceTools())
            {
                tools.ShowDialog();
            }
        }

        private void StartMonitor()
        {
            if (!AdbServer.Instance.GetStatus().IsRunning) AdbServer.Instance.StartServer(AppDomain.CurrentDomain.BaseDirectory + "Tool\\adb.exe", false);
            monitor = new DeviceMonitor(new AdbSocket(new IPEndPoint(IPAddress.Loopback, AdbClient.AdbServerPort)));
            monitor.DeviceConnected += Monitor_DeviceChanged;
            //monitor.DeviceConnected += Monitor_DeviceChanged;
            //monitor.DeviceDisconnected += Monitor_DeviceChanged;
            monitor.Start();
        }
        private void Monitor_DeviceChanged(object sender, DeviceDataEventArgs e)
        {
            if (viewModel.threadMonitor?.IsAlive == true) return;
            viewModel.threadMonitor = new Thread(() =>
            {
                Utils.Debug.Log("List Devices changed");
                
                if (viewModel.isGetDevice == false)
                {
                    Device device;
                    string tempName;
                    List<DeviceData> newDeviceList = monitor.Devices.ToList();
                    for (int i = 0; i < newDeviceList.Count; i++)
                    {
                        device = new Device
                        {
                            autoIncrementId = i,
                            Model = Adb.Shell(newDeviceList[i].Serial, "echo $(getprop ro.product.model)", 10),
                            Serial = newDeviceList[i].Serial,
                            Key = Funcs.GetMd5Hash(newDeviceList[i].Serial + "winelex2020")
                        };
                        tempName = device.GetWinName();
                        device.Name = tempName.Length > 0 ? tempName : device.Model;

                        Utils.Debug.Log("New device added");
                        viewModel.devicesModel.Add(device);
                    }

                    viewModel.isGetDevice = true;
                    Invoke(new MethodInvoker(() => FillListDevices()));
                }
                
            })
            { IsBackground = true };
            viewModel.threadMonitor.Start();
        }


        #endregion functions


    }
}
