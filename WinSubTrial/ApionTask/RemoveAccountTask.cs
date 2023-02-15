using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WinSubTrial.Utilities;
using Utils;
using Newtonsoft.Json;

namespace WinSubTrial
{
    class RemoveAccountTask
    {
        public bool isStopAuto = false;
        private string TextDump;
        public Gmail gmail { get; set; }
        private readonly Random _rand = new Random();
        public TaskResult pastePasswordOk = TaskResult.Success;
        public MailTask mailTask = MailTask.loginMail;

        public bool RemoveAccount(string serial)
        {
            Adb.SendKey(serial, "KEYCODE_HOME");
            Common.SetStatus(serial, "Open Remove account");
            Common.Sleep(_rand.Next(500, 1000));
            OpenRemoveAccount(serial);
            DateTime startTime = DateTime.Now;
            // eslint-disable-next-line no-constant-condition
            while (true)
            {
                if (isStopAuto)
                {
                    Common.SetStatus(serial, "Stopped Auto");

                    return false;
                }
                DateTime currentTime = DateTime.Now;
                if ((currentTime - startTime).TotalSeconds > 240)
                {
                    startTime = currentTime;
                    Common.SetStatus(serial, "Timeout Remove account, Reboot");
                    Adb.Reboot(serial);
                    Adb.Run(serial, "wait-for-device");
                    continue;
                }
                DumpUi(serial);

                // Error first
                if (ContainsIgnoreCase(TextDump, "\"google\"") && ContainsIgnoreCase(TextDump, "\"add account\""))
                {
                    Common.SetStatus(serial, "Tapped this account");
                    TapDynamic(serial, "\"Google\"");
                    Common.Sleep(1000);
                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "\"account sync\""))
                {
                    Common.SetStatus(serial, "Tapped confirm remove account");
                    TapDynamic(serial, "\"remove account\"");
                    Common.Sleep(1000);
                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "\"cancel\"") && ContainsIgnoreCase(TextDump, "\"remove account\""))
                {
                    Common.SetStatus(serial, "Tapped remove account second");
                    TapDynamic(serial, "\"remove account\"");
                    Common.Sleep(1000);
                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "\"accounts for owner\""))
                {
                    DateTime timeNow = DateTime.Now;
                    if (pastePasswordOk == TaskResult.Success)
                    {
                        // Luu gmail thanh cong
                        if (mailTask == MailTask.createMail)
                        {
                            string countryJson = Adb.Shell(serial, "curl http://ip-api.com/json/?fields=country");
                            Dictionary<string, string> dictionaryCountry = JsonConvert.DeserializeObject<Dictionary<string, string>>(countryJson);
                            string gmailCreated = $"{gmail.Mail}@gmail.com|{gmail.Password}|{gmail.Recovery}    {timeNow.ToString("dd/MM/yyyy HH:mm:ss")}    {dictionaryCountry["country"]}";
                            MyFile.WriteAllText("Data\\gmail_created.txt", gmailCreated, true, "\r\n");
                            Common.SetStatus(serial, "Saved to gmail created done");
                            Common.Sleep(_rand.Next(500, 2000));
                        }
                        else
                        {
                            string gmailDone = $"{gmail.Mail}|{gmail.Password}|{gmail.Recovery}";
                            MyFile.WriteAllText($"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\1\\data\\emails.txt", gmailDone, true, "\r\n");
                            Common.SetStatus(serial, "Saved to add card emails done");
                            Common.Sleep(_rand.Next(500, 2000));

                            string gmailCompleted = $"{gmail.Mail}|{gmail.Password}|{gmail.Recovery}    {timeNow.ToString("dd/MM/yyyy HH:mm:ss")}";
                            MyFile.WriteAllText("Data\\gmailCompletelySubscribed.txt", gmailCompleted, true, "\r\n");
                            Common.SetStatus(serial, "Saved to Completely Subscribed done");
                            Common.Sleep(_rand.Next(500, 2000));
                        }
                    } else
                    {
                        string gmailError = $"{gmail.Mail}|{gmail.Password}|{gmail.Recovery}    Khong sub duoc";
                        MyFile.WriteAllText("Data\\gmail_error.txt", gmailError, true, "\r\n");
                        Common.SetStatus(serial, "Saved to gmail error done");
                        Common.Sleep(_rand.Next(500, 2000));
                    }
                    
                    return true;
                }
                if (ContainsIgnoreCase(TextDump, "welcome, ") && ContainsIgnoreCase(TextDump, "get started"))
                {              
                    Adb.SendKey(serial, "KEYCODE_BACK");
                    Common.Sleep(2000);
                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "com.android.launcher3"))
                {
                    Common.SetStatus(serial, "Kickout Home, Reloading");
                    OpenRemoveAccount(serial);
                    Common.Sleep(_rand.Next(1000, 2000));
                    continue;
                }


                Common.Sleep(_rand.Next(3000, 4000));
            }
        }

        private void SwipeUp(string serial)
        {
            Adb.Shell(serial, "input swipe 500 1700 50 50");
        }

        private void Enter(string serial)
        {
            Adb.SendKey(serial, "KEYCODE_ENTER");
        }

        private void InputDynamic(string serial, string query, string text)
        {
            Point point = GetPointFromUi(query);
            if (point != default) Adb.Shell(serial, $"input tap {point.X} {point.Y}");
            Input(serial, text);
        }

        private void Input(string serial, string text, bool isClear = true)
        {
            if (isClear)
            {
                Adb.SendKey(serial, "123");
                Adb.SendKey(serial, "--longpress 67 67 67 67 67 67 67 67 67 67 67");
            }
            Adb.Shell(serial, $"input text {text}");
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

        private void TapDynamic(string serial, string text)
        {
            Point point = GetPointFromUi(text);
            point.X += _rand.Next(5, 25);
            point.Y += _rand.Next(5, 25);
            Adb.Shell(serial, $"input tap {point.X} {point.Y}");
        }

        private bool CompareAccountWithAccountInsideDevice(string serial, string email)
        {
            return Adb.Shell(serial, $"dumpsys account|grep {email}").Contains(email);
        }

        private void OpenRemoveAccount(string serial)
        {
            Adb.Shell(serial, "am start -n com.android.settings/.Settings\\$AccountDashboardActivity");
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
    }
}
