using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Utils;
using WinSubTrial.Utilities;

namespace WinSubTrial
{
    class GmailTask
    {
        public bool isStopAuto = false;
        public Gmail gmail { get; set; }

        private string TextDump;
        private readonly Random _rand = new Random();
        
        public TaskResult Login(string serial)
        {
            Adb.SendKey(serial, "KEYCODE_HOME");
            Common.SetStatus(serial, "Open Login");
            OpenAddAccount(serial);
            Common.Sleep(_rand.Next(3000, 4000));
            DateTime startTime = DateTime.Now;
            // eslint-disable-next-line no-constant-condition
            while (true)
            {
                if (CompareAccountWithAccountInsideDevice(serial, gmail.Mail))
                {
                    return TaskResult.Success;
                }
                if (isStopAuto)
                {
                    Common.SetStatus(serial, "Stopped Auto");

                    return TaskResult.StopAuto;
                }
                DateTime currentTime = DateTime.Now;
                if ((currentTime - startTime).TotalSeconds > 600)
                {
                    Common.SetStatus(serial, "Timeout, Login gmail timed out");
                    string gmailError = $"{gmail.Mail}|{gmail.Password}|{gmail.Recovery}    Login gmail timed out 5p";
                    MyFile.WriteAllText("Data\\gmail_error.txt", gmailError, true, "\r\n");
                    Common.SetStatus(serial, "Saved to gmail error done");
                    return TaskResult.Failure;
                }

                DumpUi(serial);
                // Error first
                if (ContainsIgnoreCase(TextDump, "\"android system\""))
                {
                    Common.SetStatus(serial, "Android system error");
                    TapDynamic(serial, "\"ok\"");
                    Common.Sleep(2000);
                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "\"error\""))
                {
                    Common.SetStatus(serial, "Your account wasnt added");
                    TapDynamic(serial, "\"next\"");
                    Common.Sleep(2000);
                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "country") && ContainsIgnoreCase(TextDump, "edittext"))
                {
                    Common.SetStatus(serial, "Verphone, Stopped Auto");
                    string gmailError = $"{gmail.Mail}|{gmail.Password}|{gmail.Recovery}    Verphone Stopped Auto";
                    MyFile.WriteAllText("Data\\gmail_error.txt", gmailError, true, "\r\n");
                    Common.SetStatus(serial, "Saved to gmail error done");
                    return TaskResult.Failure;
                }
                if (ContainsIgnoreCase(TextDump, "your account has been disable"))
                {
                    Common.SetStatus(serial, "Your account has been disable");
                    string gmailError = $"{gmail.Mail}|{gmail.Password}|{gmail.Recovery}    Your account has been disable";
                    MyFile.WriteAllText("Data\\gmail_error.txt", gmailError, true, "\r\n");
                    Common.SetStatus(serial, "Saved to gmail error done");
                    return TaskResult.Failure;
                }
                if (ContainsIgnoreCase(TextDump, "Wrong password"))
                {
                    Common.SetStatus(serial, "Wrong password");
                    string gmailError = $"{gmail.Mail}|{gmail.Password}|{gmail.Recovery}    Wrong password";
                    MyFile.WriteAllText("Data\\gmail_error.txt", gmailError, true, "\r\n");
                    Common.SetStatus(serial, "Saved to gmail error done");
                    return TaskResult.Failure;
                }
                if (ContainsIgnoreCase(TextDump, "not signed in") && ContainsIgnoreCase(TextDump, "try again"))
                {
                    Common.SetStatus(serial, "You are not signed in");
                    TapDynamic(serial, "try again");
                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "never lose your contacts") && ContainsIgnoreCase(TextDump, "sync device contacts"))
                {
                    Common.SetStatus(serial, "Skip sync contacts");
                    SwipeUp(serial);
                    DumpUi(serial);
                    TapDynamic(serial, "SYNC DEVICE CONTACTS\"");
                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "terms of service") && ContainsIgnoreCase(TextDump, "accept"))
                {
                    Common.SetStatus(serial, "Terms Of Service");
                    TapDynamic(serial, "accept");
                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "stay in the loop"))
                {
                    TapDynamic(serial, "yes,");
                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "add an account") && ContainsIgnoreCase(TextDump, "google"))
                {
                    TapDynamic(serial, "google");
                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "verify") && ContainsIgnoreCase(TextDump, "edittext"))
                {
                    Common.SetStatus(serial, "Recovery Mail");
                    InputDynamic(serial, "edittext", gmail.Recovery);
                    Common.Sleep(3000);
                    Enter(serial);
                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "sign in") && ContainsIgnoreCase(TextDump, "identifierid"))
                {

                    Common.SetStatus(serial, "Mail");
                    InputDynamic(serial, "identifierid", gmail.Mail);
                    Common.Sleep(3000);
                    Enter(serial);
                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "password") && ContainsIgnoreCase(TextDump, "edittext"))
                {
                    Common.SetStatus(serial, "Pass");
                    InputDynamic(serial, "edittext", gmail.Password);
                    Common.Sleep(3000);
                    Enter(serial);
                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "keep your account updated with this phone"))
                {
                    Common.SetStatus(serial, "Keep your account updated with this phone");
                    SwipeUp(serial);
                    SwipeUp(serial);
                    DumpUi(serial);
                    TapDynamic(serial, "yes,");
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
                if (ContainsIgnoreCase(TextDump, "google services") && ContainsIgnoreCase(TextDump, "tap to learn"))
                {
                    // tapDynamic(serial, 'button');
                    // tapDynamic(serial, 'button');
                    return TaskResult.Success;
                }
                if (ContainsIgnoreCase(TextDump, "confirm your recovery"))
                {
                    Common.SetStatus(serial, "Confirm Recovery Mail");
                    TapDynamic(serial, "confirm your recovery");
                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "\"new account features\""))
                {
                    Common.SetStatus(serial, "Account Features");
                    SwipeUp(serial);
                    SwipeUp(serial);
                    DumpUi(serial);
                    TapDynamic(serial, "\"i agree\"");
                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "something went wrong"))
                {
                    Common.SetStatus(serial, "Something Went Wrong");
                    OpenAddAccount(serial);
                    Common.Sleep(2000);
                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "\"couldn't sign in\""))
                {
                    Common.SetStatus(serial, "Couldn't sign in, air plane mode");
                    /*
                    Adb.Shell(serial, "am start -a android.settings.AIRPLANE_MODE_SETTINGS");
                    Common.Sleep(500);
                    DumpUi(serial);
                    TapDynamic(serial, "\"airplane mode\"");
                    Common.Sleep(1000);
                    TapDynamic(serial, "\"airplane mode\"");
                    Common.Sleep(500);
                    */
                    Adb.SendKey(serial, "KEYCODE_HOME");
                    Common.Sleep(3000);
                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "i agree") && ContainsIgnoreCase(TextDump, "button"))
                {
                    Common.SetStatus(serial, "I Agree");
                    TapDynamic(serial, "signinconsentnext");
                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "\"try again\"") && ContainsIgnoreCase(TextDump, "you are not signed in"))
                {
                    Common.SetStatus(serial, "You are not signed in, try again");
                    TapDynamic(serial, "\"try again\"");
                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "com.android.launcher3"))
                {
                    Common.SetStatus(serial, "Kickout Home, Reloading");
                    OpenAddAccount(serial);
                    Common.Sleep(_rand.Next(3000, 4000));
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

        private void OpenAddAccount(string serial)
        {
            Adb.CloseApp(serial, "com.google.android.gms");
            Adb.Shell(serial, "am start -n com.google.android.gms/.auth.uiflows.addaccount.AccountIntroActivity");
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
