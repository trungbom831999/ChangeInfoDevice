using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WinSubTrial.Utilities;
using System.Diagnostics;
using Utils;

namespace WinSubTrial
{
    class InstallAppTask
    {
        public bool isStopAuto = false;
        private string TextDump;
        private readonly Random _rand = new Random();
        public string appPackage { get; set; }
        private int rebootCounts = 0;
        public Gmail gmail { get; set; }

        public TaskResult InstallApp(string serial)
        {
            Common.SetStatus(serial, "Start install app");
            OpenInstallApp(serial, "https://play.google.com/store/apps/details?id=" + appPackage);
            Common.Sleep(_rand.Next(1000, 2000));
            DateTime startTime = DateTime.Now;
            // eslint-disable-next-line no-constant-condition
            while (true)
            {
                if (isStopAuto)
                {
                    Common.SetStatus(serial, "Stopped find exactly and touch");
                    return TaskResult.StopAuto;
                }
                DateTime currentTime = DateTime.Now;
                if ((currentTime - startTime).TotalSeconds > 240)
                {
                    Common.SetStatus(serial, "Timeout, Install app timed out");
                    rebootCounts += 1;
                    startTime = currentTime;
                    if (rebootCounts > 2)
                    {
                        string gmailError = $"{gmail.Mail}|{gmail.Password}|{gmail.Recovery}    Install app failed";
                        MyFile.WriteAllText("Data\\gmail_error.txt", gmailError, true, "\r\n");
                        Common.SetStatus(serial, "Saved to gmail error done");
                        return TaskResult.Failure;
                    }
                    else
                    {
                        Adb.Reboot(serial);
                        Adb.Run(serial, "wait-for-device");
                        continue;
                    }
                }
                
                DumpUi(serial);
                //Debug.WriteLine($"TextDump: {TextDump}");

                // Error first
                if (appPackage == "")
                {
                    Common.SetStatus(serial, $"Link app empty! Cant install: {appPackage}");
                    return TaskResult.Failure;
                }
                if (ContainsIgnoreCase(TextDump, "\"got it\""))
                {
                    Common.SetStatus(serial, "Cannot install app, got it");
                    TapDynamic(serial, "\"got it\"");
                    //OpenInstallApp(serial, "https://play.google.com/store/apps/details?id=" + appPackage);
                    Common.Sleep(1000);
                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "\"details\""))
                {
                    Common.SetStatus(serial, "Details description app, reload game");
                    Adb.SendKey(serial, "KEYCODE_BACK");
                    Common.Sleep(1000);
                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "\"try again\""))
                {
                    Common.SetStatus(serial, "Try again");
                    TapDynamic(serial, "\"try again\"");
                    //OpenInstallApp(serial, "https://play.google.com/store/apps/details?id=" + appPackage);
                    Common.Sleep(1000);
                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "\"try again\"") && ContainsIgnoreCase(TextDump, "\"notify me\""))
                {
                    Common.SetStatus(serial, "Cannot install app, got it");
                    TapDynamic(serial, "\"try again\"");
                    //OpenInstallApp(serial, "https://play.google.com/store/apps/details?id=" + appPackage);
                    Common.Sleep(1000);
                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "\"play while you wait\""))
                {
                    Common.SetStatus(serial, "Play while you wait");
                    TapDynamic(serial, "\"try again\"");
                    //OpenInstallApp(serial, "https://play.google.com/store/apps/details?id=" + appPackage);
                    Common.Sleep(1000);
                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "connection lost"))
                {
                    Common.SetStatus(serial, "Connection has been lost, reopen app link");
                    TapDynamic(serial, "\"retry\"");
                    //OpenInstallApp(serial, "https://play.google.com/store/apps/details?id=" + appPackage);
                    Common.Sleep(1000);
                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "connection timed out."))
                {
                    Common.SetStatus(serial, "Connection timed out, retry");
                    TapDynamic(serial, "\"Try again\"");
                    //OpenInstallApp(serial, "https://play.google.com/store/apps/details?id=" + appPackage);
                    Common.Sleep(1000);
                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "\"install\""))
                {
                    Common.SetStatus(serial, "Tapped: install");
                    TapDynamic(serial, "\"install\"");
                    Common.Sleep(1000);
                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "\"uninstall\"") && ContainsIgnoreCase(TextDump, "\"Open\""))
                {
                    Common.SetStatus(serial, "Tapped Open finish");
                    TapDynamic(serial, "\"Open\"");
                    Common.Sleep(15000);
                    return TaskResult.Success;
                }
                if (ContainsIgnoreCase(TextDump, "\"uninstall\"") && ContainsIgnoreCase(TextDump, "\"Play\""))
                {
                    Common.SetStatus(serial, "Tapped Play finish");
                    TapDynamic(serial, "\"Play\"");
                    Common.Sleep(15000);
                    return TaskResult.Success;
                }
                if (ContainsIgnoreCase(TextDump, "com.android.launcher3"))
                {
                    Common.SetStatus(serial, "Kickout Home, Reloading");
                    OpenInstallApp(serial, "https://play.google.com/store/apps/details?id=" + appPackage);
                    Common.Sleep(_rand.Next(3000, 4000));
                    continue;
                }

                //TapPosition(serial, new Point(783, 687));

                Common.Sleep(_rand.Next(3000, 4000));
            }
        }

        private void TapDynamic(string serial, string text)
        {
            Point point = GetPointFromUi(text);
            point.X += _rand.Next(5, 25);
            point.Y += _rand.Next(5, 25);
            Adb.Shell(serial, $"input tap {point.X} {point.Y}");
        }

        private void TapPosition(string serial, Point point)
        {
            point.X += _rand.Next(5, 25);
            point.Y += _rand.Next(5, 25);
            Adb.Shell(serial, $"input tap {point.X} {point.Y}");
        }

        private Point GetPointFromUi(string query)
        {
            Point point = default;
            try
            {
                string value = Regex.Match(TextDump, $@"({query}[^\>]+)>", RegexOptions.IgnoreCase).Groups[1].Value;
                if (ContainsIgnoreCase(query, "identifierid")) Utils.Debug.Log("value: " + value);
                Match match = Regex.Match(value, @"\[(\d+),(\d+)\]\[(\d+),(\d+)\]");
                if (!match.Success) return point;

                string[] coords = new string[] { match.Groups[1].Value, match.Groups[2].Value };
                string[] sizes = new string[] { match.Groups[3].Value, match.Groups[4].Value };
                if (ContainsIgnoreCase(query, "identifierid")) Utils.Debug.Log("coords: " + string.Join(",", coords));
                if (ContainsIgnoreCase(query, "identifierid")) Utils.Debug.Log("sizes: " + string.Join(",", sizes));

                int x = (int.Parse(coords[0]) + int.Parse(sizes[0])) / 2;
                int y = (int.Parse(coords[1]) + int.Parse(sizes[1])) / 2;
                point = new Point(x, y);
                if (ContainsIgnoreCase(query, "identifierid")) Utils.Debug.Log("coords: " + string.Join(",", point));
            }
            catch (Exception ex)
            {
                Utils.Debug.Log(ex.Message);
            }

            return point;
        }

        public string DumpUi(string serial)
        {
            TextDump = "";
            if (Adb.Shell(serial, "rm -f /sdcard/window_dump.xml; uiautomator dump --compressed").Contains("UI hierchary dumped to"))
            {
                TextDump = Adb.Shell(serial, "cat sdcard/window_dump.xml");
            }
            return TextDump;
        }

        public bool ContainsIgnoreCase(string source, string subString)
        {
            return Regex.IsMatch(source, subString, RegexOptions.IgnoreCase);
        }

        private void OpenInstallApp(string serial, string link)
        {
            Common.SetStatus(serial, $"Open url {link}");
            Adb.OpenUrl(serial, link);
        }
    }
}
