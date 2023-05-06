using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utils;
using WinSubTrial.Utilities;

namespace WinSubTrial
{
    public static class Common
    {
        public static string ServerApi = "http://localhost:3000";

        public static JObject GlobalSettings;
        public static Settings Settings;
        public static Configs Configs;
        public static string[] DefaultWipePackages = new string[] {
            "com.android.vending",
            "com.android.chrome",
            "com.google.android.gsf",
            "com.google.android.gms",
            "com.google.android.gsf.login",
            "com.google.android.backuptransport",
            "com.android.htmlviewer",
            "com.qualcomm.location",
            "com.google.android.play.games",
            "com.google.android.gm",
            "com.sony.snei.np.android.account",
            "com.android.webview"
        };
        public static List<string> FullWipePackages;

        public static List<BackupData> ListBackups = new List<BackupData>();
        public static List<RestoreData> ListRestores = new List<RestoreData>();

        public static string ProxyHost = string.Empty;
        public static string ProxyPort = string.Empty;

        public delegate void CallbackStatusHandler(string serial, string status);
        public static event CallbackStatusHandler CallbackStatus;

        public delegate void CallbackDeviceNameHandler(Device device, List<Device> devices);
        public static event CallbackDeviceNameHandler CallbackName;
        public static List<Country> countryList = JsonConvert.DeserializeObject<List<Country>>(MyFile.ReadAllText("Data\\countriescode.json"));

        internal static void LoadDevice()
        {
            List<Device> Devices = new List<Device>();
            Devices.Clear();
            string[] serials = Adb.GetDevices();
            if(serials.Length > 0)
            {
                foreach(string s in from string s1 in serials where Regex.IsMatch(s1, "[a-zA-Z0-9]\t(device|offline)") select s1)
                {
                    Device device = new Device
                    {
                        Name = "Unknown",
                        Serial = s.Replace("\tdevice", string.Empty).Replace("\toffline", string.Empty),
                        Key = Funcs.GetMd5Hash(serials + "winelex2020")
                    };
                    
                    string Name = Adb.Shell(device.Serial, "echo $(getprop ro.product.model)", 10);
                    if(Name.Length > 1 && !Regex.IsMatch(Name, "error|ystem"))
                    {
                        device.Name = Name.Trim();
                    }
                    Devices.Add(device);
                }

            }
        }

        public static List<string> WipePackages;

        public static void LoadInfo()
        {
            LoadGlobalSettings(notify: false);
            Settings = JsonConvert.DeserializeObject<Settings>(GlobalSettings["settings"].ToString());
            try
            {
                Configs = JsonConvert.DeserializeObject<Configs>(GlobalSettings["configs"].ToString());
            }
            catch
            {
                Configs = new Configs { Brand = "Random", Country = "VN", Network = "Mobifone", SDK = "11" };
                GlobalSettings.Add("configs", JsonConvert.SerializeObject(Configs));
            }
            LoadWipePackages();
            if (!File.Exists("Data\\WipePackages.txt"))
            {
                MyFile.WriteAllText("Data\\WipePackages.txt", "");
            }
        }

        
        internal static void UpdateName(Device device, List<Device> devices)
        {
            if (CallbackName != null)
                CallbackName(device, devices);
        }

        private static void LoadWipePackages()
        {
            string[] temp = MyFile.ReadAllLines("Data\\WipePackages.txt");
            FullWipePackages = temp.ToList();
            if (temp.Length < 1)
            {
                temp = DefaultWipePackages;
            }
            WipePackages = temp.ToList();
        }

        public static void LoadGlobalSettings(bool notify = true)
        {
            GlobalSettings = JObject.Parse(MyFile.ReadAllText("C:\\WINALL\\settings.json"));
            if(notify) Info("Load Done!");
        }

        public static void SaveGlobalSettings(bool notify = true)
        {
            MyFile.WriteAllText("C:\\WINALL\\settings.json", GlobalSettings.ToString());
            if(notify) Info("Save Done!");
        }

        public static void Info(string text)
        {
            MessageBox.Show(text);
        }

        internal static void Sleep(int milisecon)
        {
            try
            {
                Thread.Sleep(milisecon);
            }
            catch { }
        }

        internal static void Log(string result)
        {
            Console.WriteLine($"[{DateTime.Now:G}]{result}");
        }

        public static void SetStatus(string serial, string status)
        {
            // do stuff....
            if (CallbackStatus != null)
                CallbackStatus(serial, status);
        }

        internal static void LoadBackupList()
        {
            ListBackups.Clear();
            string[] folders = Directory.GetDirectories(@"C:\WINALL\winbackup");
            if (folders.Length == 0) return;

            BackupData backupData;
            FileInfo fileInfo;
            foreach (string folder in folders)
            {
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

                        ListBackups.Add(backupData);
                    }
                }
            }
        }

        internal static void LoadRestoreList()
        {
            ListRestores.Clear();
            string[] folders = Directory.GetDirectories(@"C:\WINALL\winrestore");
            if (folders.Length == 0) return;

            RestoreData restoreData;
            FileInfo fileInfo;
            foreach (string folder in folders)
            {
                foreach(string file in Directory.GetFiles(folder))
                {
                    restoreData = new RestoreData
                    {
                        Folder = folder.Substring(folder.LastIndexOf("\\") + 1),
                        Name = Path.GetFileName(file)
                    };
                    fileInfo = new FileInfo(file);
                    restoreData.Date = fileInfo.CreationTime.ToString("dd:MM HH:mm");
                    restoreData.Size = Math.Round((double)fileInfo.Length / 1048576).ToString();

                    ListRestores.Add(restoreData);
                }
            }
        }
    }
}
