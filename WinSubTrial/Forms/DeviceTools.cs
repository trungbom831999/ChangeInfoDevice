using SharpAdbClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinSubTrial.Functions;
using WinSubTrial.Utilities;

namespace WinSubTrial
{
    public partial class DeviceTools : Form
    {
        private DataGridViewImageColumn colRemoteDevice, colCaptureScreen, colOpenLink, colWipeApp, colChangeName;
        private Dictionary<string, Task> deviceTasks = new Dictionary<string, Task>();
        private Dictionary<string, Thread> deviceThreads = new Dictionary<string, Thread>();
        private List<Device> devicesModel = new List<Device>();
        private static DeviceMonitor monitor;
        public Thread threadMonitor;

        private void dgvDevice_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1) return;

            Action action = default;
            int rowIndex = e.RowIndex;
            DataGridViewRow row = dgvDevice.Rows[rowIndex];
            string serial = row.Cells["deviceSerial"].Value.ToString();
            string title = row.Cells["deviceName"].Value.ToString();
            Device device = devicesModel.First(x => x.Serial.Equals(serial));

            if (e.ColumnIndex == 0)
            {
                if (row.Cells["deviceSelect"].Value != null && (bool)row.Cells["deviceSelect"].Value)
                {
                    row.Cells["deviceSelect"].Value = false;
                }
                else
                {
                    row.Cells["deviceSelect"].Value = true;
                }
                return;
            }
            switch (e.ColumnIndex)
            {
                case 5:
                    action = () => Funcs.ShowVysor(serial, title);
                    break;
                case 6:
                    action = () =>
                    {
                        row.Cells["taskStatus"].Value = "Capture screen...";
                        string fileName = "screen_" + serial;
                        Funcs.CaptureScreen(serial, fileName);
                        row.Cells["taskStatus"].Value = "Capture screen done";
                    };
                    break;
                case 7:
                    action = () =>
                    {
                        row.Cells["taskStatus"].Value = "Wipe apps...";
                        new Change { device = device }.WipeApps();
                        row.Cells["taskStatus"].Value = "Wipe apps done";
                    };
                    break;
                case 8:
                    using (WinName winName = new WinName())
                    {
                        winName.Serial = serial;
                        winName.Name = row.Cells["deviceName"].Value.ToString();
                        winName.ShowDialog();
                        device.Name = winName.Name;
                        row.Cells["deviceName"].Value = device.Name;
                        Common.UpdateName(device, devicesModel);
                    }
                    break;
                case 9:
                    action = OpenLinkForm(serial);
                    break;
                default:
                    break;
            }

            if (action != default)
            {
                Task.Run(action);
            }
        }

        private void DeviceTools_Load(object sender, EventArgs e)
        {
            StartMonitor();
            Monitor_DeviceChanged(null, null);
            FillListDevices();
        }

        public DeviceTools()
        {
            InitializeComponent();

            colRemoteDevice = new DataGridViewImageColumn
            {
                ToolTipText = "Remote",
                Width = 26,
                Resizable = DataGridViewTriState.False,
                Image = Properties.Resources.vysor
            };
            dgvDevice.Columns.Add(colRemoteDevice);

            colCaptureScreen = new DataGridViewImageColumn
            {
                ToolTipText = "Capture Screen",
                Width = 26,
                Resizable = DataGridViewTriState.False,
                Image = Properties.Resources.capture
            };
            dgvDevice.Columns.Add(colCaptureScreen);

            colWipeApp = new DataGridViewImageColumn
            {
                ToolTipText = "Wipe Apps",
                Width = 26,
                Resizable = DataGridViewTriState.False,
                Image = Properties.Resources.wipe
            };
            dgvDevice.Columns.Add(colWipeApp);

            colChangeName = new DataGridViewImageColumn
            {
                ToolTipText = "Change Name",
                Width = 26,
                Resizable = DataGridViewTriState.False,
                Image = Properties.Resources.editName
            };
            dgvDevice.Columns.Add(colChangeName);

            colOpenLink = new DataGridViewImageColumn
            {
                ToolTipText = "Open Link",
                Width = 26,
                Resizable = DataGridViewTriState.False,
                Image = Properties.Resources.link
            };
            dgvDevice.Columns.Add(colOpenLink);
        }

        private void FillListDevices()
        {
            if (dgvDevice.Rows.Count > 0)
            {
                foreach (var item in dgvDevice.Rows.Cast<DataGridViewRow>().Where(r => !devicesModel.Exists(x => x.Serial.Equals(r.Cells["deviceSerial"].Value.ToString()))))
                {
                    if (!IsDeviceInTask(item.Cells["deviceSerial"].Value.ToString())) dgvDevice.Rows.Remove(item);
                }
            }

            DataGridViewRow row;
            foreach (Device device in devicesModel)
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
                catch { }

                row = new DataGridViewRow { Height = 39 };
                int index = dgvDevice.Rows.Add(row);
                row = dgvDevice.Rows[index];

                row.Cells["deviceName"].Value = device.Name;
                row.Cells["deviceSerial"].Value = device.Serial;
                row.Cells["deviceKey"].Value = device.Key;
                row.Cells["taskStatus"].Value = "Ready";
                row.Cells["taskStatus"].Style.ForeColor = Color.DarkRed;
                deviceTasks.Add(device.Serial, null);
                deviceThreads.Add(device.Serial, null);
            }
        }

        private bool IsDeviceInTask(string serial)
        {
            return deviceTasks[serial]?.Status == TaskStatus.Running || deviceThreads[serial]?.IsAlive == true;
        }

        private Action OpenLinkForm(string serial)
        {
            return () =>
            {
                using (OpenLink ol = new OpenLink(serial))
                {
                    ol.ShowDialog();
                    switch (ol.commands[0].ToString())
                    {
                        case "openurl":
                            Common.SetStatus(serial, "Opening link...");
                            Adb.OpenUrl(serial, ol.commands[1], ol.commands[2]);
                            Common.SetStatus(serial, "Open link done");
                            break;
                        case "checkipapi":
                            Common.SetStatus(serial, "Checking api...");
                            string proxy = default;
                            string resultIpApi = Adb.Shell(serial, $"curl {proxy} http://ip-api.com/line");
                            if (Common.ProxyHost.Length > 1) proxy = $"--socks5 {Common.ProxyHost}:{Common.ProxyPort}";
                            Common.SetStatus(serial, "Check ip api done. See popup window");
                            break;
                        case "openapp":
                            Common.SetStatus(serial, "Opening app...");
                            Adb.OpenApp(serial, "com.android.vending");
                            Common.SetStatus(serial, "Open app done");
                            break;
                        case "wipeapp":
                            Common.SetStatus(serial, "Wiping app...");
                            Adb.WipeApp(serial, ol.commands[1]);
                            Common.SetStatus(serial, "Wiping app done");
                            break;
                        case "proxyon":
                            Common.SetStatus(serial, "Settings proxy...");
                            Common.ProxyHost = ol.commands[1].Split(':')[0];
                            Common.ProxyPort = ol.commands[1].Split(':')[1];
                            Funcs.ProxyOn(serial, Common.ProxyHost, Common.ProxyPort, "socks5", "192.168.1.1", "8.8.8.8");
                            Common.SetStatus(serial, "Set proxy done: " + Common.ProxyHost + ":" + Common.ProxyPort);
                            break;
                    }
                }
            };
        }

        private void StartMonitor()
        {
            if (!AdbServer.Instance.GetStatus().IsRunning) AdbServer.Instance.StartServer(AppDomain.CurrentDomain.BaseDirectory + "Tool\\adb.exe", false);
            monitor = new DeviceMonitor(new AdbSocket(new IPEndPoint(IPAddress.Loopback, AdbClient.AdbServerPort)));
            monitor.DeviceChanged += Monitor_DeviceChanged;
            //monitor.DeviceConnected += Monitor_DeviceChanged;
            //monitor.DeviceDisconnected += Monitor_DeviceChanged;
            monitor.Start();
        }
        private void Monitor_DeviceChanged(object sender, DeviceDataEventArgs e)
        {
            if (threadMonitor?.IsAlive == true) return;
            threadMonitor = new Thread(() =>
            {
                Utils.Debug.Log("List Devices changed");
                Device device;
                string tempName;
                List<DeviceData> newDeviceList = monitor.Devices.ToList();

                foreach (DeviceData device1 in newDeviceList)
                {
                    device = new Device
                    {
                        Model = Adb.Shell(device1.Serial, "echo $(getprop ro.product.model)", 10),
                        Serial = device1.Serial,
                        Key = Funcs.GetMd5Hash(device1.Serial + "winelex2020")
                    };
                    tempName = device.GetWinName();
                    device.Name = tempName.Length > 0 ? tempName : device.Model;
                    if (devicesModel.Contains(device))
                    {
                        Utils.Debug.Log("This device already in list");
                    }
                    else
                    {
                        Utils.Debug.Log("New device added");
                        devicesModel.Add(device);
                    }
                }
                Invoke(new MethodInvoker(() => FillListDevices()));
            })
            { IsBackground = true };
            threadMonitor.Start();
        }

    }
}
