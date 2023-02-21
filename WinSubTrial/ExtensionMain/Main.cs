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

namespace WinSubTrial
{

    public partial class Main : Form
    {
        private DataGridViewImageColumn colRestore;
        private DataGridViewImageColumn colMoveToRestore;
        private DataGridViewImageColumn colDelete;
        private DataGridViewImageColumn colMoveToBackup;
        private DataGridViewImageColumn colDeleteRestore;
        private readonly Random _rand = new Random();
        private static DeviceMonitor monitor;

        private MainViewModel viewModel = new MainViewModel();

        public Main()
        {
            InitializeComponent();
            InitDGVBackupsColumns();
            InitDGVRestoresColumns();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            Common.LoadInfo();
            viewModel.fetch();
            Common.CallbackStatus += new Common.CallbackStatusHandler(SetMainStatus);
            Common.CallbackName += new Common.CallbackDeviceNameHandler(UpdateMainName);

            cboBrand.Text = Common.Configs.Brand;
            cboCountry.Text = Common.Configs.Country;
            cboOperator.Text = Common.Configs.Network;
            cboSDK.Text = Common.Configs.SDK;
            LoadNetwork();

            StartMonitor();
            Monitor_DeviceChanged(null, null);
        }
        private void runButtonTapped(object sender, EventArgs e)
        {
            if (!viewModel.someDevicesSelected()) return;
            Thread thread = new Thread(new ThreadStart(() =>
            {
                foreach (Device device in viewModel.devicesModel.Where(x => x.isSelected == true))
                {
                    Task.Run(() =>
                    {
                        viewModel.deviceWaitForStop[device.Serial] = false;
                        viewModel.LoginGmail(device.Serial);
                    });
                }
            }))
            { IsBackground = true };
            thread.Start();
        }

        private void createMailButtonTapped(object sender, EventArgs e)
        {
            if (!viewModel.someDevicesSelected()) return;

            Thread thread = new Thread(new ThreadStart(() =>
            {
                foreach (Device device in viewModel.devicesModel.Where(x => x.isSelected == true))
                {
                    viewModel.deviceWaitForStop[device.Serial] = false;
                    viewModel.CreateMail(device.Serial);
                }
            }))
            { IsBackground = true };
            thread.Start();
        }

        private void runLastAllButtonTapped(object sender, EventArgs e)
        {
            if (!viewModel.someDevicesSelected()) return;

            Thread thread = new Thread(new ThreadStart(() =>
            {
                foreach (Device device in viewModel.devicesModel.Where(x => x.isSelected == true))
                {
                    Task.Run(() =>
                    {
                        viewModel.deviceWaitForStop[device.Serial] = true;
                        Common.SetStatus(device.Serial, "Waiting run last email done all");
                    });
                }
            }))
            { IsBackground = true };
            thread.Start();
        }

        private void rebootAllButtonTapped(object sender, EventArgs e)
        {
            if (!viewModel.someDevicesSelected()) return;
            Thread thread = new Thread(new ThreadStart(() =>
            {
                foreach (Device device in viewModel.devicesModel.Where(x => x.isSelected == true))
                {
                    Task.Run(() =>
                    {
                        Common.SetStatus(device.Serial, "Reboot all devices");
                        Adb.Reboot(device.Serial);
                    });
                }
            }))
            { IsBackground = true };
            thread.Start();
        }

        private void powerOffButtonTapped(object sender, EventArgs e)
        {
            if (!viewModel.someDevicesSelected()) return;
            Thread thread = new Thread(new ThreadStart(() =>
            {
                foreach (Device device in viewModel.devicesModel.Where(x => x.isSelected == true))
                {
                    Task.Run(() =>
                    {
                        Common.SetStatus(device.Serial, "Reboot all devices");
                        Adb.Shell(device.Serial, "reboot -p");
                    });
                }
            }))
            { IsBackground = true };
            thread.Start();
        }

        private void airPlaneModeTapped(object sender, EventArgs e)
        {
            if (!viewModel.someDevicesSelected()) return;
            Thread thread = new Thread(new ThreadStart(() =>
            {
                foreach (Device device in viewModel.devicesModel.Where(x => x.isSelected == true))
                {
                    Task.Run(() =>
                    {
                        TaskResult airplaneMode = new AirplaneModeTask { }.TurnOnAirPlane(device.Serial);
                        if (airplaneMode == TaskResult.Success)
                        {
                            Common.SetStatus(device.Serial, "Turn On Air Plane done");
                        }
                    });
                }
            }))
            { IsBackground = true };
            thread.Start();
        }

        private void wifiProxyOnTapped(object sender, EventArgs e)
        {
            if (!viewModel.someDevicesSelected()) return;
            Thread thread = new Thread(new ThreadStart(() =>
            {
                foreach (Device device in viewModel.devicesModel.Where(x => x.isSelected == true))
                {
                    Task.Run(() =>
                    {
                        TaskResult wifiProxyOK = new WifiProxyTask { }.LoginWifiProxy(device.Serial);
                        if (wifiProxyOK == TaskResult.Success)
                        {
                            Common.SetStatus(device.Serial, "Turn wifi proxy done");
                        }
                    });
                }
            }))
            { IsBackground = true };
            thread.Start();
        }

        public void SetMainStatus(string serial, string status)
        {
            if (viewModel.devicesModel.Count == 0)
            {
                return;
            }
            Device device = viewModel.devicesModel.First(x => x.Serial.Equals(serial));
            DataGridViewRow row;
            if (dgvDevice == null)
                return;
            if (device == null)
                return;
            if (dgvDevice.Rows.Count <= device.autoIncrementId)
                return;
            try
            {
                if (dgvDevice.InvokeRequired)
                {
                    dgvDevice.Invoke(new MethodInvoker(() =>
                    {
                        row = dgvDevice.Rows[device.autoIncrementId];
                        if (row == null || row == default) return;
                        row.Cells["taskStatus"].Value = status;
                    }));
                }
                row = dgvDevice.Rows[device.autoIncrementId];
                if (row == null || row == default) return;
                row.Cells["taskStatus"].Value = status;
            }
            catch
            {
                Utils.Debug.Log("Set status falure, crash app");
            }
        }

        public void UpdateMainName(Device device, List<Device> devices)
        {
            if (dgvDevice == null)
                return;
            if (dgvDevice.Rows.Count <= device.autoIncrementId)
                return;
            devices.First(x => x.Serial.Equals(device.Serial)).Name = device.Name;
            try
            {
                DataGridViewRow row = dgvDevice.Rows[device.autoIncrementId];
                if (row == null || row == default) return;

                row.Cells["deviceName"].Value = device.Name;
            }
            catch
            {
                Utils.Debug.Log("Update name falure, crash app");
            }
#if DEBUG
            //Info("New name: " + Devices.First(x => x.Serial.Equals(device.Serial)).Name);
#endif
        }

        private void AutoClickTacButtonTapped(object sender, EventArgs e)
        {
            if (!viewModel.someDevicesSelected()) return;
            Thread thread = new Thread(new ThreadStart(() =>
            {
                foreach (Device device in viewModel.devicesModel.Where(x => x.isSelected == true))
                {
                    viewModel.AutoClickTac(device.Serial);
                }
            }))
            { IsBackground = true };
            thread.Start();
        }

        private void SubscribeYoutubeButtonTapped(object sender, EventArgs e)
        {
            if (!viewModel.someDevicesSelected()) return;
            Thread thread = new Thread(new ThreadStart(() =>
            {
                foreach (Device device in viewModel.devicesModel.Where(x => x.isSelected == true))
                {
                    viewModel.SubscribeYoutube(device.Serial);
                }
            }))
            { IsBackground = true };
            thread.Start();
        }

        private void tiktokButtonTapped(object sender, EventArgs e)
        {
            if (!viewModel.someDevicesSelected()) return;

            Thread thread = new Thread(new ThreadStart(() =>
            {
                foreach (Device device in viewModel.devicesModel.Where(x => x.isSelected == true))
                {
                    Task.Run(() =>
                    {
                        viewModel.deviceWaitForStop[device.Serial] = false;
                        viewModel.TiktokApp(device.Serial);


                        /*
                        Device device = viewModel.devicesModel.FirstOrDefault(x => x.Serial.Equals(device.Serial));
                        int indexDevice = viewModel.devicesModel.FindIndex(element => element.Serial == device.Serial);
                        int times = 10000000;
                        while (times > 0)
                        {
                            string randomFile = viewModel.RandomPasswordString();
                            Adb.Run(device.Serial, $"exec-out screencap -p > {randomFile}.png");
                            Common.Sleep(3000);

                            Bitmap bmp = new Bitmap($"Tool\\{randomFile}.png");
                            Common.Sleep(1000);

                            string response = viewModel.UploadImage("http://hamai00.tk:9999/obj", bmp);
                            Common.Sleep(2000);

                            Common.SetStatus(device.Serial, response);
                            pictureBox1.Image = bmp;

                            Common.Sleep(40000000);


                        }
                        */

                    });
                }
            }))
            { IsBackground = true };
            thread.Start();
        }

        private void snapchatButtonTapped(object sender, EventArgs e)
        {
            if (!viewModel.someDevicesSelected()) return;

            Thread thread = new Thread(new ThreadStart(() =>
            {
                foreach (Device device in viewModel.devicesModel.Where(x => x.isSelected == true))
                {
                    Task.Run(() =>
                    {
                        viewModel.deviceWaitForStop[device.Serial] = false;
                        viewModel.SnapchatApp(device.Serial);
                    });
                }
            }))
            { IsBackground = true };
            thread.Start();
        }

        private void TinderButtonClick(object sender, EventArgs e)
        {
            if (!viewModel.someDevicesSelected()) return;
            Thread thread = new Thread(new ThreadStart(() =>
            {
                foreach (Device device in viewModel.devicesModel.Where(x => x.isSelected == true))
                {
                    Task.Run(() =>
                    {
                        viewModel.deviceWaitForStop[device.Serial] = false;
                        viewModel.TinderAutomation(device.Serial);
                    });
                }
            }))
            { IsBackground = true };
            thread.Start();
        }
    }
}
