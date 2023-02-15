using Newtonsoft.Json.Linq;
using SharpAdbClient;
using SharpAdbClient.DeviceCommands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Utils;

namespace WinSubTrial
{
    public static class Adb
    {
        private static AdbClient adbClient;

        public static bool GetFile(string serial, string remoteFile, string localFile)
        {
            return !Run(serial, $"pull {remoteFile} {localFile}").Contains("No such file");
        }

        internal static void UninstallApp(string serial, string package)
        {
            Shell(serial, $"pm uninstall {package}");
        }

        public static string GetIP(string serial)
        {
            string result = Shell(serial, "curl http://ip-api.com/json", 15);
            return result.Contains("\"query\"") ? JObject.Parse(result.Substring(0, result.IndexOf("}") + 1))["query"].ToString() : "Coud not get ip";
        }

        public static string GetCountry(string serial)
        {
            string result = Shell(serial, $"curl http://ip-api.com/json", 15);
            return result.Contains("\"country\"") ? JObject.Parse(result.Substring(0, result.IndexOf("}") + 1))["country"].ToString() : "Coud not get ip";
        }

        internal static void RebootFast(string serial)
        {
            ShellSu(serial, "setprop sys.boot_completed 0; setprop ctl.restart surfaceflinger; setprop ctl.restart zygote");
            
            while (!IsDeviceOnine(serial))
            {
                Common.Sleep(10000);
            }
            Run(serial, "wait-for-device");
            
            while (!Shell(serial, "getprop sys.boot_completed").Equals("1"))
            {
                Common.Sleep(8000);
            }
            
        }

        public static bool ContainsIgnoreCase(string source, string subString)
        {
            return Regex.IsMatch(source, subString, RegexOptions.IgnoreCase);
        }

        public static bool IsDeviceOnine(string serial)
        {
            return Run(serial, "devices", 15).Contains(serial);
        }

        internal static string[] GetInstalledPackages(string serial)
        {
            return Shell(serial, "pm list packages -3 | cut -f 2 -d :").Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
        }

        internal static string[] GetSystemPackages(string serial)
        {
            return Shell(serial, "pm list packages| cut -f 2 -d :").Split('\n');
        }

        internal static void OpenApp(string serial, string app)
        {
            //Shell(serial, $"am start {app}"); // Old syntax
            Shell(serial, $"am start -n {app}/.MainActivity");
        }

        public static bool Reboot(string serial, string mode = "")
        {
            return !Run(serial, $"reboot {mode}", 5).Contains("not found");
        }

        public static bool SendFile(string serial, string localFile, string remoteFile)
        {
            return !Regex.IsMatch(Run(serial, $"push {localFile} {remoteFile}", 1800), "error:|No such file");
        }

        internal static void WipeApp(string serial, string app)
        {
            Shell(serial, $"pm clear {app}");

        }

        public static string Shell(string serial, string cmd, int timeout = 900)
        {
            return ShellSocket(serial, cmd, timeout);
        }
        
        public static string ShellCmd(string serial, string cmd, int timeout = 900)
        {
            return Run(serial, $"shell \"{cmd}\"", timeout);
        }


        public static string ShellSocket(string serial, string cmd, int timeout = 900)
        {
            int tempTimeout = timeout;
            ConsoleOutputReceiver receiver = new ConsoleOutputReceiver();
            try
            {
                Utils.Debug.Log($"[{serial}_Cmd]: {cmd}");
                new AdbClient().ExecuteShellCommand(new DeviceData() { Serial = serial }, cmd, receiver);
                string result = receiver.ToString().TrimEnd();
                Utils.Debug.Log($"[{serial}_Result]: {result}");
                return result;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Thread was being aborted")) return "";
                Utils.Debug.Log($"ShellSocket error: {ex.Message}");
                tempTimeout -= 50;
                Common.SetStatus(serial, $"Error {ex.Message.Replace("An error occurred while reading a response from ADB: ", "")}, reboot after: {tempTimeout / 5}");
                Common.Sleep(10000);
                if (IsRecovery(serial))
                {
                    Common.SetStatus(serial, "Device comes into recovery mode, wake up device");
                    Common.Sleep(5000);
                    Reboot(serial);
                    while (!IsDeviceOnine(serial))
                    {
                        Common.Sleep(2000);
                    }
                    Run(serial, "wait-for-device");
                    Common.Sleep(10000);
                    return "";
                }
                if (tempTimeout < 100)
                {
                    Common.SetStatus(serial, "Retry run shell too many times. Reboot!");
                    Common.Sleep(5000);
                    Reboot(serial);
                    while (!IsDeviceOnine(serial))
                    {
                        Common.Sleep(2000);
                    }
                    Run(serial, "wait-for-device");
                    Common.Sleep(10000);
                    return "";
                }

                return Shell(serial, cmd, tempTimeout);
            }
        }

        internal static void SendKey(string serial, string keycode, int times = 1)
        {
            string keycodes = keycode;
            for (int i = 1; i <= times; i++)
            {
                keycodes += $" {keycode}";
            }
            Shell(serial, $"input keyevent {keycodes}");
        }

        internal static void SendKeyOnce(string serial, string keycode)
        {
            string keycodes = keycode;
            Shell(serial, $"input keyevent {keycodes}");
        }

        public static string Twrp(string serial, string cmd)
        {
            return Shell(serial, $"twrp {cmd}");
        }

        public static string Run(string serial, string cmdCommand, int timeout = 900)
        {
            Common.Log($"[{serial}_Cmd] {cmdCommand}");
            string result;
            string device = serial.Length > 4? $"-s {serial}" : "";
            try
            {
                Process process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory + "Tool",
                        Arguments = $"/C adb {device} {cmdCommand}",
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
            Common.Log($"[{serial}_Result] {result}");
            return result.Trim();
        }

        internal static void AppInstall(string serial, string apkPath)
        {
            Run(serial, $"install -r {apkPath}");
        }

        internal static void AppInstallRemote(string serial, string apkPath)
        {
            Shell(serial, $"pm install -r {apkPath}");
        }

        public static string RunCmd(string serial, string cmdCommand, int timeout = 900)
        {
            Common.Log(cmdCommand);
            string result;
            try
            {
                Process process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory + "Tool",
                        FileName = "cmd.exe",
                        CreateNoWindow = true,
                        UseShellExecute = false,
                        WindowStyle = ProcessWindowStyle.Hidden,
                        RedirectStandardInput = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        Verb = "runas"
                    }
                };
                process.Start();
                process.StandardInput.WriteLine($"adb.exe -s {serial} {cmdCommand}");
                process.WaitForExit(timeout * 1000);
                process.StandardInput.Flush();
                process.StandardInput.Dispose();
                string text = process.StandardOutput.ReadToEnd() + process.StandardError.ReadToEnd();
                result = text;
            }
            catch
            {
                result = string.Empty;
            }
            Common.Log(result);
            return result;
        }

        internal static void EnableApp(string serial, string package)
        {
            Shell(serial, $"pm enable {package}");
        }

        internal static void DisableApp(string serial, string package)
        {
            Shell(serial, $"pm disable {package}");
        }

        internal static void TextSend(string serial, string text)
        {
            Shell(serial, $"input text {text}");
        }

        internal static void CloseApp(string serial, string app)
        {
            Shell(serial, $"am force-stop {app}");
        }

        public static bool WaitRecovery(string serial, int timeout = 0)
        {
            if (timeout == 0) timeout = 900;
            Run(serial, "wait-for-recovery", timeout);
            Common.Sleep(3000);
            return Run(serial, "get-state").Contains("recovery");
        }

        internal static bool IsRecovery(string serial, int timeout = 10)
        {
            return Run(serial, "get-state", timeout).Contains("recovery");
        }

        internal static bool FileExist(string serial, string path)
        {
            return Shell(serial, $"if [ -e {path} ]; then echo 'existed'; else echo 'Not exist'; fi").Contains("existed");
        }
    
        internal static string[] GetDevices()
        {
            return Run("", "devices", 15).Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        }

        internal static void OpenUrl(string serial, string link, string activity = "")
        {
            if (activity.Length > 5)
            {
                activity = "-n " + activity;
            }
            Shell(serial, $"am start {activity} -a android.intent.action.VIEW -d \"{link}\"");
        }

        public static bool DirPut(string serial, string localDir, string remoteDir, bool create = false)
        {
            if (create) 
            {
                DirCreate(serial, remoteDir);
            }
            else
            {
                FileDelete(serial, $"{remoteDir}/*");
            }

            foreach (string fileName in Directory.GetFiles(localDir).Select(Path.GetFileName).ToArray())
            {
                if (!FilePut(serial, $@"{localDir}\{fileName}", $"{remoteDir}/{fileName}")) return false;
            }

            return true;
        }

        public static bool FilePut(string serial, string localPath, string remotePath, bool removeOld = false)
        {
            try
            {
                if (removeOld) FileDelete(serial, remotePath);
                using (SyncService service = new SyncService(new AdbSocket(new IPEndPoint(IPAddress.Loopback, AdbClient.AdbServerPort)), new DeviceData { Serial = serial }))
                using (Stream stream = File.OpenRead(localPath))
                {
                    service.Push(stream, remotePath, 777, DateTime.Now, null, CancellationToken.None);
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public static bool DirCreate(string serial, string path)
        {
            return Shell(serial, $"mkdir {path};ls {path}").Contains(path);
        }

        public static bool FileDelete(string serial, string file)
        {
            return Shell(serial, $"rm -rf {file}; if [ -e {file} ]; then echo 'Existed'; else echo 'Not found'; fi").Equals("Not found");
        }

        public static bool FileGet(string serial, string remotePath, string localPath)
        {
            //if (Share.Debug) Share.Notify($"FileGet: {remotePath} => {localPath}");
            try
            {
                if (File.Exists(localPath)) File.Delete(localPath);
                using (SyncService service = new SyncService(new AdbSocket(new IPEndPoint(IPAddress.Loopback, AdbClient.AdbServerPort)), new DeviceData { Serial = serial }))
                {
                    using (Stream stream = File.OpenWrite(localPath))
                    {
                        service.Pull(remotePath, stream, null, CancellationToken.None);
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        internal static string ShellSu(string serial, string v)
        {
            return Shell(serial, $"su -c \"{v}\"");
        }

        public static Point GetPointFromUi(string query, string TextDump, bool findExactly = false)
        {
            Point point = default;
            try
            {
                RegexOptions options;
                string findQuery, value;

                if (findExactly)
                {
                    options = RegexOptions.None;
                    findQuery = $"(['\"]{query}['\"][^\\>]+)>";
                }
                else
                {
                    options = RegexOptions.IgnoreCase;
                    findQuery = $@"({query}[^\>]+)>";
                }

                value = Regex.Match(TextDump, findQuery, options).Groups[1].Value;

                Match match = Regex.Match(value, @"\[(\d+),(\d+)\]\[(\d+),(\d+)\]");
                if (!match.Success) return point;

                string[] coords = new string[] { match.Groups[1].Value, match.Groups[2].Value };
                string[] sizes = new string[] { match.Groups[3].Value, match.Groups[4].Value };

                int x = (int.Parse(coords[0]) + int.Parse(sizes[0])) / 2;
                int y = (int.Parse(coords[1]) + int.Parse(sizes[1])) / 2;
                point = new Point(x, y);
            }
            catch (Exception ex)
            {
                Utils.Debug.Log(ex.Message);
            }

            return point;
        }
        
        public static string DumpUi(string serial)
        {
            string TextDump = "";
            if (Shell(serial, "rm -f /sdcard/window_dump.xml; uiautomator dump --compressed").Contains("UI hierchary dumped to"))
            {
                TextDump = Shell(serial, "cat sdcard/window_dump.xml");
            }
            return TextDump;
        }

        public static bool TapDynamic(string serial, string text, bool findExactly = false)
        {
            Random _rand = new Random();
            Point point = GetPointFromUi(text, DumpUi(serial), findExactly);
            if (point == null) return false;
            point.X += _rand.Next(5, 25);
            point.Y += _rand.Next(5, 25);
            Shell(serial, $"input tap {point.X} {point.Y}");
            return true;
        }
    }
}
