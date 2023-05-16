using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinSubTrial.Enum;
using WinSubTrial.Globals;

namespace WinSubTrial.Forms.Popup
{
    public partial class AutoPhonePopup : Form
    {
        public MainViewModel viewModel { get; set; }
        public AutoPhonePopup()
        {
            InitializeComponent();
            setNet();
        }
        public string SaveDir { get; internal set; }
        
        private void AutoMobile_Load(object sender, EventArgs e)
        {
            textBoxFolderBackup.Text = (string)Common.GlobalSettings["folderBackup"];
            textBoxFolderRestore.Text = (string)Common.GlobalSettings["folderRestore"];
        }

        private void setNet()
        {
            this.textBoxNet1.Text = getNet(1);
            EnumNET.NET1 = this.textBoxNet1.Text;
            this.textBoxNet2.Text = getNet(2);
            EnumNET.NET2 = this.textBoxNet2.Text;
        }

        private string getNet(int index)
        {
            string net = File.ReadLines("Data\\Net\\net.txt").Skip(index-1).Take(1).First();
            return net;
        }

        private void buttonSaveNetClick(object sender, EventArgs e)
        {
            string net1 = this.textBoxNet1.Text;
            string net2 = this.textBoxNet2.Text;
            File.WriteAllText("Data\\Net\\net.txt", net1 + "\n" + net2);
            setNet();
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

        private void ChametButtonClick(object sender, EventArgs e)
        {
            if (!viewModel.someDevicesSelected()) return;
            Thread thread = new Thread(new ThreadStart(() =>
            {
                foreach (Device device in viewModel.devicesModel.Where(x => x.isSelected == true))
                {
                    Task.Run(() =>
                    {
                        viewModel.deviceWaitForStop[device.Serial] = false;
                        viewModel.ChametAutomation(device.Serial);
                    });
                }
            }))
            { IsBackground = true };
            thread.Start();
        }
        private void CamScannerButtonClick(object sender, EventArgs e)
        {
            if (!viewModel.someDevicesSelected()) return;
            Thread thread = new Thread(new ThreadStart(() =>
            {
                foreach (Device device in viewModel.devicesModel.Where(x => x.isSelected == true))
                {
                    Task.Run(() =>
                    {
                        viewModel.deviceWaitForStop[device.Serial] = false;
                        viewModel.CamScannerAutomation(device.Serial);
                    });
                }
            }))
            { IsBackground = true };
            thread.Start();
        }

        private void BigoButtonSMSlick(object sender, EventArgs e)
        {
            BigoButtonClick();
        }

        private void BigoButtonRegisterClick(object sender, EventArgs e)
        {
            BigoButtonClick(false);
        }

        private void BigoButtonClick(bool isSMS = true)
        {
            if (!viewModel.someDevicesSelected()) return;
            Thread thread = new Thread(new ThreadStart(() =>
            {
                foreach (Device device in viewModel.devicesModel.Where(x => x.isSelected == true))
                {
                    Task.Run(() =>
                    {
                        viewModel.deviceWaitForStop[device.Serial] = false;
                        viewModel.BigoAutomation(device.Serial, isSMS);
                    });
                }
            }))
            { IsBackground = true };
            thread.Start();
        }

        private void BigoLiteButtonClick(object sender, EventArgs e)
        {
            if (!viewModel.someDevicesSelected()) return;
            GlobalVariable.isGetPhonenumber = false;
            viewModel.devicesModel.Where(x => x.isSelected == true).AsParallel().ForAll(device =>
            {
                Task.Run(() =>
                {
                    viewModel.deviceWaitForStop[device.Serial] = false;
                    viewModel.BigoLiteAutomation(device.Serial);
                });
            });
        }
        private void snapchatPasswordNET1(object sender, EventArgs e)
        {
            snapchatPasswordRetrieval("net1");
        }
        private void snapchatPasswordNET2(object sender, EventArgs e)
        {
            snapchatPasswordRetrieval("net2");
        }
        private void snapchatPasswordRetrieval(string net)
        {
            if (!viewModel.someDevicesSelected()) return;
            viewModel.devicesModel.Where(x => x.isSelected == true).AsParallel().ForAll(device =>
            {
                Task.Run(() =>
                {
                    viewModel.deviceWaitForStop[device.Serial] = false;
                    viewModel.SnapchatPasswordRetrieval(device.Serial, net);
                });
            });
        }

        private void buttonTelegramNet2_Click(object sender, EventArgs e)
        {
            TelegramRegister("net2");
        }

        private void TelegramRegister(string net)
        {
            if (!viewModel.someDevicesSelected()) return;
            viewModel.devicesModel.Where(x => x.isSelected == true).AsParallel().ForAll(device =>
            {
                Task.Run(() =>
                {
                    viewModel.deviceWaitForStop[device.Serial] = false;
                    viewModel.TelegramRegister(device.Serial, net);
                });
            });
        }

        private void buttonGlobalSmart_Click(object sender, EventArgs e)
        {
            if (!viewModel.someDevicesSelected()) return;
            viewModel.devicesModel.Where(x => x.isSelected == true).AsParallel().ForAll(device =>
            {
                Task.Run(() =>
                {
                    viewModel.deviceWaitForStop[device.Serial] = false;
                    viewModel.GlobalSmartRegister(device.Serial);
                });
            });
        }

        private void buttonXbank_Click(object sender, EventArgs e)
        {
            if (!viewModel.someDevicesSelected()) return;
            viewModel.devicesModel.Where(x => x.isSelected == true).AsParallel().ForAll(device =>
            {
                Task.Run(() =>
                {
                    viewModel.deviceWaitForStop[device.Serial] = false;
                    viewModel.XbankRegister(device.Serial);
                });
            });
        }

        private void buttonSnapchatLoginBackup_Click(object sender, EventArgs e)
        {
            if (!viewModel.someDevicesSelected()) return;
            if (string.IsNullOrEmpty(textBoxFolderBackup.Text))
            {
                Common.Info("Please choose a folder backup");
                return;
            }
            Common.GlobalSettings["folderBackup"] = textBoxFolderBackup.Text;
            viewModel.devicesModel.Where(x => x.isSelected == true).AsParallel().ForAll(device =>
            {
                Task.Run(() =>
                {
                    viewModel.deviceWaitForStop[device.Serial] = false;
                    viewModel.SnapchatLoginBackup(device.Serial);
                });
            });
        }

        private void buttonChooseFolderBackup_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                textBoxFolderBackup.Text = dialog.SelectedPath;
                Common.GlobalSettings["folderBackup"] = dialog.SelectedPath;
            }
            else
            {
                textBoxFolderBackup.Text = "";
            }
        }

        private void buttonSnapchatChangePhone_Click(object sender, EventArgs e)
        {
            if (!viewModel.someDevicesSelected()) return;
            string folderRestore = textBoxFolderRestore.Text;
            if (string.IsNullOrEmpty(folderRestore)) {
                Common.Info("Please choose a folder restore");
                return;
            }
            Common.GlobalSettings["folderRestore"] = folderRestore;
            Common.ListBackups = LoadBackupFileList(folderRestore);
            if (Common.ListBackups.Count == 0)
            {
                Common.Info("Folder is empty!");
                return;
            }
            viewModel.devicesModel.Where(x => x.isSelected == true).AsParallel().ForAll(device =>
            {
                Task.Run(() =>
                {
                    viewModel.deviceWaitForStop[device.Serial] = false;
                    viewModel.SnapchatChangePhone(device.Serial);
                });
            });
        }
        //Load danh sách các file backup (wbk) trong folder đã chọn
        internal static List<BackupData> LoadBackupFileList(string folder)
        {
            List<BackupData> ListBackups = new List<BackupData>();
            if (Directory.Exists(folder))
            {
                BackupData backupData;
                FileInfo fileInfo;
                foreach (string file in Directory.GetFiles(folder))
                {
                    if (file.Contains(".wbk"))
                    {
                        backupData = new BackupData
                        {
                            Folder = folder.Substring(folder.LastIndexOf("\\") + 1),
                            Name = Path.GetFileName(file)
                        };
                        fileInfo = new FileInfo(file);
                        backupData.Date = fileInfo.CreationTime.ToString("dd:MM HH:mm");
                        backupData.Size = Math.Round((double)fileInfo.Length / 1048576).ToString();
                        backupData.isRunning = false;
                        ListBackups.Add(backupData);
                    }
                }
            }
            return ListBackups;
        }

        private void buttonChooseFolderRestore_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if(dialog.ShowDialog() == DialogResult.OK) { 
                textBoxFolderRestore.Text = dialog.SelectedPath;
                Common.GlobalSettings["folderRestore"] = dialog.SelectedPath;
            }
            else
            {
                textBoxFolderRestore.Text = "";
            }
        }

    }
}
