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
using Faker;

namespace WinSubTrial
{
    class RecoveryEmailTask
    {
        private string[] mailHostArray = { "@hotmail.com", "@outlook.com", "@yahoo.com", "@icloud.com", "@yahoo.com.vn" };

        public bool isStopAuto = false;
        public Gmail gmail { get; set; }

        private string TextDump;
        private readonly Random _rand = new Random();
        private bool lessSecureOn = false;

        public Gmail AddRecoveryEmail(string serial)
        {
            Adb.SendKey(serial, "KEYCODE_HOME");
            Common.SetStatus(serial, "Open Add Recovery");
            OpenRemoveAccount(serial);
            Common.Sleep(_rand.Next(500, 1000));
            DateTime startTime = DateTime.Now;
            // eslint-disable-next-line no-constant-condition
            while (true)
            {
                if (isStopAuto)
                {
                    Common.SetStatus(serial, "Stopped Add Recovery");
                    return null;
                }
                DateTime currentTime = DateTime.Now;
                if ((currentTime - startTime).TotalSeconds > 100000)
                {
                    Common.SetStatus(serial, "Timeout, Stopped");
                    return null;
                }
                DumpUi(serial);
                //Debug.WriteLine($"TextDump: {TextDump}");


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
                    Common.SetStatus(serial, "Tapped google account");
                    TapDynamic(serial, "\"google account\"");
                    Common.Sleep(1000);
                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "welcome, ") && ContainsIgnoreCase(TextDump, "get started"))
                {
                    Common.SetStatus(serial, "Tapped get started");
                    TapDynamic(serial, "\"get started\"");
                    Common.Sleep(1000);
                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "manage your info, privacy, and security to make google work better for you."))
                {
                    Common.SetStatus(serial, "Tapped security");
                    TapDynamic(serial, "\"security\"");
                    Common.Sleep(1000);
                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "you have security recommendations") && lessSecureOn == false)
                {
                    Common.SetStatus(serial, "Turn on less secure app access");
                    SwipeUp(serial);
                    Common.Sleep(1000);
                    DumpUi(serial);
                    TapDynamic(serial, "less secure app access");
                    Common.Sleep(1000);
                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "allow less secure apps: OFF") && lessSecureOn == false)
                {
                    Common.SetStatus(serial, "Turn on allow less secure apps");
                    TapDynamic(serial, "allow less secure apps: OFF");
                    Common.Sleep(1000);
                    Adb.SendKey(serial, "KEYCODE_BACK");
                    Common.Sleep(2000);
                    SwipeDown(serial);
                    Common.Sleep(200);
                    SwipeDown(serial);
                    Common.Sleep(1000);
                    lessSecureOn = true;
                    continue;
                }

                if (ContainsIgnoreCase(TextDump, "you have security recommendations") && lessSecureOn == true)
                {
                    Common.SetStatus(serial, "Tapped protect your account");
                    TapDynamic(serial, "protect your account");
                    Common.Sleep(1000);
                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "add a recovery email"))
                {
                    Common.SetStatus(serial, "Tapped add a recovery email");
                    TapDynamic(serial, "add a recovery email");
                    Common.Sleep(1000);
                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "welcome") && ContainsIgnoreCase(TextDump, "show password"))
                {
                    Common.SetStatus(serial, "Enter password to continue add recovery");
                    InputDynamic(serial, "edittext", gmail.Password);
                    Common.Sleep(1000);
                    Enter(serial);
                    Common.Sleep(1000);
                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "add recovery email") && ContainsIgnoreCase(TextDump, "learn more"))
                {
                    Common.SetStatus(serial, "Enter recovery email");
                    gmail.Recovery = GenerateEmail();
                    InputDynamic(serial, "edittext", gmail.Recovery);
                    Common.Sleep(1000);
                    Enter(serial);
                    Common.Sleep(1000);
                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "verify your recovery email") && ContainsIgnoreCase(TextDump, "send a new code"))
                {
                    Common.SetStatus(serial, "Verify your recovery email");
                    TapDynamic(serial, "verify your recovery email");
                    Common.Sleep(1000);
                    DumpUi(serial);
                    TapDynamic(serial, "verify later");
                    Common.Sleep(1000);
                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "your contact info"))
                {
                    Common.SetStatus(serial, "Tapped done your contact info");
                    TapDynamic(serial, "done");
                    string gmailDone = $"{gmail.Mail}|{gmail.Password}|{gmail.Recovery}";
                    MyFile.WriteAllText("Data\\creategmail.txt", gmailDone, true, "\r\n");
                    Common.Sleep(2000);
                    ClearSettingApp(serial);
                    return gmail;
                }

                Common.Sleep(_rand.Next(1000, 2000));
            }
        }

        private void SwipeUp(string serial)
        {
            Adb.Shell(serial, "input swipe 500 1700 50 50");
        }

        private void SwipeDown(string serial)
        {
            Adb.Shell(serial, "input swipe 400 400 500 1700");
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

        private void TapPosition(string serial, Point point)
        {
            point.X += _rand.Next(5, 25);
            point.Y += _rand.Next(5, 25);
            Adb.Shell(serial, $"input tap {point.X} {point.Y}");
        }

        private void OpenRemoveAccount(string serial)
        {
            Adb.Shell(serial, "am start -n com.android.settings/.Settings\\$AccountDashboardActivity");
        }

        private void ClearSettingApp(string serial)
        {
            Adb.Shell(serial, "pm clear com.android.settings");
        }

        public string GenerateEmail()
        {
            return Faker.Name.First() + Faker.Name.Last() + RandomNumber() + mailHostArray[_rand.Next(0, mailHostArray.Length)];
        }

        public string RandomNumber()
        {
            int length = _rand.Next(3, 7);
            const string chars = "0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[_rand.Next(s.Length)]).ToArray());
        }
    }
}
