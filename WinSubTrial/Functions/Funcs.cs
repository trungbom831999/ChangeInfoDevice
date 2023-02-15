using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utils;

namespace WinSubTrial.Utilities
{
    public class Funcs
    {
        static string ScrcpyPath = "C:\\win_scrcpy\\scrcpy-noconsole.exe";
        
        public static void WipeMail(string serial)
        {
            Adb.ShellSu(serial, "rm -rf /data/system/graphicsstats/* /data/system/dropbox/* /data/system/netstats/* /data/system/procstats/* /data/system/usagestats/* /data/system/syncmanager-log/* /data/system/0/* /data/system_ce/0/* /data/system_de/0/* /data/data/com.android.vending /data/user_de/0/com.android.vending /sdcard/Android/data/com.android.vending /data/data/com.google.android.gms/* /data/user_de/0/com.google.android.gms /sdcard/Android/data/com.google.android.gms /data/data/com.google.android.gsf /data/user_de/0/com.google.android.gsf /sdcard/Android/data/com.google.android.gsf /data/data/com.google.android.gsf.login /data/user_de/0/com.google.android.gsf.login /sdcard/Android/data/com.google.android.gsf.login /data/data/com.android.chrome /data/user_de/0/com.android.chrome /sdcard/Android/data/com.android.chrome");
        }

        public static void ShowVysor(string serial, string title = "")
        {
            Run($"{ScrcpyPath} -s {serial} --port 5{MyString.RandomNumber(4)} --window-title \"{title}_Screen\"", 9999);
        }

        public static void ProxyOff(string serial)
        {
            Adb.Shell(serial, "settings put global http_proxy :0");
            if (Adb.Shell(serial, "ls data/local/tmp").Contains("WIN_NETWORK"))
            {
                Adb.Shell(serial, "su -c \"system/bin/iptables -t nat -F OUTPUT; data/local/tmp/WIN_NETWORK/proxy.sh 'stop'\"");
                while (Adb.FileExist(serial, "data/local/tmp/WIN_NETWORK/redsocks.pid"))
                {
                    Common.Sleep(1000);
                }
            }
        }

        public static void ProxyOn(string serial, string host, string port, string type, string gateway, string dns)
        {
            ProxyOff(serial);

            if (!Adb.Shell(serial, "ls data/local/tmp").Contains("WIN_NETWORK"))
            {
                while(!Adb.DirPut(serial, "Tool\\WIN_NETWORK", "data/local/tmp/WIN_NETWORK"))
                {
                    Common.Sleep(1000);
                }
                Adb.Shell(serial, "chmod 777 -R data/local/tmp/WIN_NETWORK");
            }

            Adb.Shell(serial, $"su -c \"sh data/local/tmp/WIN_NETWORK/proxy.sh 'start' '{type}' '{host}' '{port}' 'false' '{gateway}' {dns} '' ''\"");
            while (!Adb.FileExist(serial, "data/local/tmp/WIN_NETWORK/redsocks.pid"))
            {
                Common.Sleep(1000);
            }
            Adb.Shell(serial, "su -c /system/bin/iptables -t nat -A OUTPUT -p tcp -j REDIRECT --to 8123");
        }

        public static string Run(string cmdCommand, int timeout = 900)
        {
            Utils.Debug.Log($"[cmd]{cmdCommand}");

            string result;
            try
            {
                Process process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory + "Tool",
                        Arguments = $"/c {cmdCommand}",
                        FileName = "cmd.exe",
                        CreateNoWindow = true,
                        UseShellExecute = false,
                        WindowStyle = ProcessWindowStyle.Hidden,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                    }
                };
                process.Start();
                process.WaitForExit(timeout * 1000);
                string text = process.StandardOutput.ReadToEnd() + process.StandardError.ReadToEnd();
                result = text;
            }
            catch
            {
                result = string.Empty;
            }
            Utils.Debug.Log($"[Result]{result}");

            return result;
        }

        internal static void CaptureScreen(string serial, string fileName)
        {
            string filePath = $"C:\\WINALL\\wincapt\\{fileName}.png";
            Adb.Shell(serial, "screencap -p /sdcard/screen.png");
            Adb.FileGet(serial, "/sdcard/screen.png", filePath);
            Process.Start(filePath);
        }

        public static void ResetDPI(string serial)
        {
            Adb.Shell(serial, "wm density reset");
        }
        
        public static string GetMd5Hash(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }

        internal static void LoginGmail(string serial)
        {
            Common.SetStatus(serial, $"Coding login gmail fo {serial}");
            Common.Sleep(5000);
        }

        internal static void OpenPaymentMethods(string serial)
        {
            Adb.Shell(serial, "am start -n com.android.vending/com.google.android.finsky.activities.MainActivity -a android.intent.action.VIEW -d \"https://play.google.com/store/paymentmethods\"");
        }
    }
}
