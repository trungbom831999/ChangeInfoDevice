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
    class CreateMailTask
    {
        private string[] monthArray = { "January", "February", "March", "April", "May", "June", "July", "August", "September" };
        private string[] genderArray = { "Male", "Female" };

        public bool isStopAuto = false;
        public Gmail gmail { get; set; }

        private string TextDump;
        private readonly Random _rand = new Random();
        private bool monthSelected = false;
        private bool genderSelected = false;
        private int rebootCounts = 0;

        public TaskResult CreateMail(string serial)
        {
            Adb.SendKey(serial, "KEYCODE_HOME");
            Common.SetStatus(serial, "Open Create mail");
            Common.Sleep(_rand.Next(2000, 3000));
            DateTime startTime = DateTime.Now;
            // eslint-disable-next-line no-constant-condition
            while (true)
            {
                if (isStopAuto)
                {
                    Common.SetStatus(serial, "Stopped create mail");
                    return TaskResult.StopAuto;
                }
                DateTime currentTime = DateTime.Now;
                if ((currentTime - startTime).TotalSeconds > 300)
                {
                    Common.SetStatus(serial, "Timeout, Create mail timed out");
                    rebootCounts += 1;
                    startTime = currentTime;
                    if (rebootCounts > 1)
                    {
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
                if (ContainsIgnoreCase(TextDump, "error") && ContainsIgnoreCase(TextDump, "sorry,"))
                {
                    Common.SetStatus(serial, "Error, cannot create your account");
                    Adb.SendKey(serial, "KEYCODE_HOME");
                    Common.Sleep(2000);
                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "\"android system\""))
                {
                    Common.SetStatus(serial, "Android system error");
                    TapDynamic(serial, "\"ok\"");
                    Common.Sleep(2000);
                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "\"couldn't sign in\""))
                {
                    Common.SetStatus(serial, "Couldn't sign in, Open Air plane");
                    Adb.Shell(serial, "am start -a android.settings.AIRPLANE_MODE_SETTINGS");
                    Common.Sleep(2000);
                    DumpUi(serial);
                    if (ContainsIgnoreCase(TextDump, "not connected") || ContainsIgnoreCase(TextDump, "Airplane mode"))
                    {
                        Common.SetStatus(serial, "Tapped wifi not connected");
                        TapDynamic(serial, "not connected");
                        Common.Sleep(2000);
                        DumpUi(serial);
                    }
                    if (ContainsIgnoreCase(TextDump, "\"com.android.settings:id/switch_text\"") && ContainsIgnoreCase(TextDump, "1500wifi"))
                    {
                        Common.SetStatus(serial, "Tapped 1500wifi");
                        TapDynamic(serial, "1500wifi");
                        Common.Sleep(1000);
                        for (int i = 0; i < 10; i++)
                        {
                            Common.Sleep(2000);
                            DumpUi(serial);
                            if (ContainsIgnoreCase(TextDump, "connected") && !ContainsIgnoreCase(TextDump, "no internet"))
                            {
                                startTime = DateTime.Now;
                                break;
                            }
                            Common.Sleep(5000);
                        }
                    }
                    Adb.SendKey(serial, "KEYCODE_HOME");
                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "sign in") && ContainsIgnoreCase(TextDump, "identifierid"))
                {
                    Common.SetStatus(serial, "Create account");
                    Point point = TapPointDynamic(serial, "\"create account\"");
                    Common.Sleep(1000);
                    DumpUi(serial);
                    Common.Sleep(1000);

                    if (ContainsIgnoreCase(TextDump, "for myself\""))
                    {
                        TapDynamic(serial, "for myself\"");
                    } else
                    {
                        point.Y = point.Y - 200;
                        TapPosition(serial, point);
                    }
                    Common.Sleep(1000);
                    continue;
                }

                if (ContainsIgnoreCase(TextDump, "create a google account") && ContainsIgnoreCase(TextDump, "enter your name"))
                {
                    string firstName = Faker.Name.First();
                    string lastName = Faker.Name.Last();

                    gmail = new Gmail();
                    gmail.Mail = firstName + lastName + RandomNumber();
                    gmail.Password = Faker.Address.City().Replace(" ", string.Empty).Replace("-", string.Empty) + RandomNumber();
                    gmail.Recovery = Faker.Name.First().ToLower() + Faker.Name.First().ToLower() + _rand.Next(1000, 9999) + "@yahoo.com";

                    Common.SetStatus(serial, "Enter First name and last name");
                    InputDynamic(serial, "firstName", firstName);
                    Common.Sleep(1000);
                    Enter(serial);

                    InputDynamic(serial, "lastName", lastName);
                    Common.Sleep(1000);
                    Enter(serial);

                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "basic information") && ContainsIgnoreCase(TextDump, "enter your birthday and gender"))
                {
                    if (ContainsIgnoreCase(TextDump, "month-label") && monthSelected == false)
                    {
                        Common.SetStatus(serial, "Choose month");
                        TapDynamic(serial, "\"month\"");
                        Common.Sleep(1000);
                        DumpUi(serial);
                        TapDynamic(serial, $"\"{monthArray[_rand.Next(0, monthArray.Length)]}\"");
                        monthSelected = true;
                        continue;
                    }
                    if (ContainsIgnoreCase(TextDump, "gender-label") && genderSelected == false)
                    {
                        Common.SetStatus(serial, "Choose gender");
                        TapDynamic(serial, "\"gender\"");
                        Common.Sleep(1000);
                        DumpUi(serial);
                        TapDynamic(serial, $"\"{genderArray[_rand.Next(0, genderArray.Length)]}\"");
                        genderSelected = true;
                        continue;
                    }
                    if (monthSelected == true && genderSelected == true)
                    {
                        Common.SetStatus(serial, "Enter birthday");
                        InputDynamic(serial, "edittext", _rand.Next(1, 29) + "");
                        Common.Sleep(1000);
                        Enter(serial);

                        InputDynamic(serial, "year", _rand.Next(1970, 2003) + "");
                        Common.Sleep(1000);
                        Enter(serial);
                        continue;

                    }
                }
                if (ContainsIgnoreCase(TextDump, "choose your gmail address") && ContainsIgnoreCase(TextDump, "pick a gmail address or create your own"))
                {
                    Common.SetStatus(serial, "Choose your gmail address");
                    TapDynamic(serial, "\"create your own gmail address\"");
                    Common.Sleep(1000);
                    DumpUi(serial);
                    InputDynamic(serial, "edittext", gmail.Mail);
                    Common.Sleep(1000);
                    Enter(serial);
                    monthSelected = false;
                    genderSelected = false;
                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "create a gmail address for signing in to your google account"))
                {
                    Common.SetStatus(serial, "Choose your gmail address");
                    InputDynamic(serial, "edittext", gmail.Mail);
                    Common.Sleep(1000);
                    Enter(serial);
                    monthSelected = false;
                    genderSelected = false;
                    continue;
                }

                if (ContainsIgnoreCase(TextDump, "create a strong password") && ContainsIgnoreCase(TextDump, "with a mix of letters, numbers and symbols"))
                {
                    Common.SetStatus(serial, "Create a strong password");
                    InputDynamic(serial, "edittext", gmail.Password);
                    Common.Sleep(1000);
                    Enter(serial);
                    Common.Sleep(1000);
                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "get a verification code sent to your phone"))
                {
                    monthSelected = false;
                    genderSelected = false;
                    return TaskResult.Failure;
                }
                if (ContainsIgnoreCase(TextDump, "add phone number to account"))
                {
                    Common.SetStatus(serial, "Skip Add Phone");
                    TapDynamic(serial, "skip");
                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "add phone") && ContainsIgnoreCase(TextDump, "skip"))
                {
                    Common.SetStatus(serial, "Skip Add Phone");
                    SwipeUp(serial);
                    SwipeUp(serial);
                    DumpUi(serial);
                    TapDynamic(serial, "yes,");
                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "choose personalization settings") && ContainsIgnoreCase(TextDump, "next"))
                {
                    Common.SetStatus(serial, "Choose personalization");
                    TapDynamic(serial, "express personalization");
                    Common.Sleep(1000);
                    TapDynamic(serial, "next");
                    Common.Sleep(1000);
                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "confirm personalization settings and cookies"))
                {
                    Common.SetStatus(serial, "Confirm personalization");
                    SwipeUp(serial);
                    SwipeUp(serial);
                    Common.Sleep(1000);
                    DumpUi(serial);
                    TapDynamic(serial, "\"confirm\"");
                    Common.Sleep(1000);
                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "something went wrong"))
                {
                    Common.SetStatus(serial, "something went wrong");
                    TapDynamic(serial, "\"next\"");
                    monthSelected = false;
                    genderSelected = false;
                    Common.Sleep(1000);
                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "review your account info") && ContainsIgnoreCase(TextDump, "next"))
                {
                    Common.SetStatus(serial, "review your account info");
                    TapDynamic(serial, "\"next\"");
                    Common.Sleep(1000);
                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "\"privacy and terms\""))
                {
                    Common.SetStatus(serial, "Privacy and terms");
                    SwipeUp(serial);
                    SwipeUp(serial);
                    Common.Sleep(1000);
                    DumpUi(serial);
                    if (ContainsIgnoreCase(TextDump, "\"i agree\""))
                    {
                        TapDynamic(serial, "\"i agree\"");
                    } else
                    {
                        Point pont = new Point(1100, 2400);
                        TapPosition(serial, pont);
                    }
                    Common.Sleep(1000);
                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "com.android.launcher3"))
                {
                    Common.SetStatus(serial, "Kickout Home, Reloading");
                    //Adb.Shell(serial, $"settings put global http_proxy apollo.p.shifter.io:265{_rand.Next(30, 80)}");
                    OpenAddAccount(serial);
                    Common.Sleep(_rand.Next(3000, 4000));
                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "google services") && ContainsIgnoreCase(TextDump, "tap to learn"))
                {
                    // tapDynamic(serial, 'button');
                    // tapDynamic(serial, 'button');

                    return TaskResult.Success;
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

        private Point TapPointDynamic(string serial, string text)
        {
            Point point = GetPointFromUi(text);
            point.X += _rand.Next(5, 25);
            point.Y += _rand.Next(5, 25);
            Adb.Shell(serial, $"input tap {point.X} {point.Y}");
            return point;
        }

        private void TapPosition(string serial, Point point)
        {
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

        private void OpenAddAccount(string serial)
        {
            Adb.CloseApp(serial, "com.google.android.gms");
            Adb.Shell(serial, "am start -n com.google.android.gms/.auth.uiflows.addaccount.AccountIntroActivity");
        }

        public string RandomPasswordString()
        {
            int length = _rand.Next(12, 14);
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[_rand.Next(s.Length)]).ToArray());
        }

        public string RandomNumber()
        {
            int length = _rand.Next(4, 7);
            const string chars = "0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[_rand.Next(s.Length)]).ToArray());
        }
    }
}
