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
    class AddingPaymentTask
    {
        public bool isStopAuto = false;
        public Gmail gmail { get; set; }

        private string TextDump;
        private readonly Random _rand = new Random();
        private int rebootCounts = 0;

        public TaskResult AddPaymentMethod(string serial)
        {
            Common.SetStatus(serial, "Open payments method..");
            Funcs.OpenPaymentMethods(serial);
            Common.Sleep(_rand.Next(1000, 2000));
            DateTime startTime = DateTime.Now;
            // eslint-disable-next-line no-constant-condition
            while (true)
            {
                if (isStopAuto)
                {
                    Common.SetStatus(serial, "Stopped Add payment");
                    return TaskResult.StopAuto;
                }
                DateTime currentTime = DateTime.Now;
                if ((currentTime - startTime).TotalSeconds > 240)
                {
                    Common.SetStatus(serial, "Timeout, Add payment timed out");
                    rebootCounts += 1;
                    startTime = currentTime;
                    if (rebootCounts > 3)
                    {
                        string gmailError = $"{gmail.Mail}|{gmail.Password}|{gmail.Recovery}    Add payment take too long";
                        MyFile.WriteAllText("Data\\gmail_error.txt", gmailError, true, "\r\n");
                        Common.SetStatus(serial, "Saved to gmail error done");
                        return TaskResult.Failure;
                    } else
                    {
                        Adb.Reboot(serial);
                        Adb.Run(serial, "wait-for-device");
                        continue;
                    }
                }
                DumpUi(serial);
                //Debug.WriteLine($"TextDump: {TextDump}");


                // Error first
                if (ContainsIgnoreCase(TextDump, "Settings") && ContainsIgnoreCase(TextDump, "play protect"))
                {
                    Common.SetStatus(serial, "Settings, play protect");
                    Adb.SendKey(serial, "KEYCODE_HOME");
                    Common.Sleep(1000);
                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "google services") && ContainsIgnoreCase(TextDump, "tap to learn"))
                {
                    Adb.SendKey(serial, "KEYCODE_HOME");
                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "\"android system\""))
                {
                    Common.SetStatus(serial, "Android system error");
                    TapDynamic(serial, "\"ok\"");
                    Common.Sleep(2000);
                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "terms of service") && ContainsIgnoreCase(TextDump, "decline"))
                {
                    Common.SetStatus(serial, "Accept terms of service");
                    TapDynamic(serial, "accept");
                    Common.Sleep(1000);
                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "add paypal"))
                {
                    Common.SetStatus(serial, "Add paypal");
                    string gmailError = $"{gmail.Mail}|{gmail.Password}|{gmail.Recovery}    Add paypal gmail";
                    MyFile.WriteAllText("Data\\gmail_error.txt", gmailError, true, "\r\n");
                    Common.SetStatus(serial, "Saved to gmail error done");
                    return TaskResult.Failure;
                }
                
                if (ContainsIgnoreCase(TextDump, "visa"))
                {
                    Common.SetStatus(serial, "Gmail da co visa");
                    string gmailError = $"{gmail.Mail}|{gmail.Password}|{gmail.Recovery}    Gmail da co visa";
                    MyFile.WriteAllText("Data\\gmail_error.txt", gmailError, true, "\r\n");
                    Common.SetStatus(serial, "Saved to gmail error done");
                    return TaskResult.Failure;
                }
                if (ContainsIgnoreCase(TextDump, "you can add purchases directly") && ContainsIgnoreCase(TextDump, "by continuing, you agree to the google payments"))
                {
                    Common.SetStatus(serial, "Enable billing");
                    TapDynamic(serial, "\"enable\"");
                    Common.Sleep(1000);
                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "sending account verification text to"))
                {
                    Common.SetStatus(serial, "Waiting sending account verification text");
                    Common.Sleep(3000);
                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "try again") && ContainsIgnoreCase(TextDump, "play while you wait"))
                {
                    Common.SetStatus(serial, "play while you wait, try again");
                    TapDynamic(serial, "\"try again\"");
                    Common.Sleep(1000);
                    continue;
                }

                if (ContainsIgnoreCase(TextDump, "contact to remove"))
                {
                    Common.SetStatus(serial, "Contact to remove");
                    TapDynamic(serial, "\"OK\"");
                    Common.Sleep(1000);
                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "try again later"))
                {
                    Common.SetStatus(serial, "Can't verify account, try again later");
                    TapDynamic(serial, "\"OK\"");
                    Common.Sleep(1000);
                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "error"))
                {
                    Common.SetStatus(serial, "An error has occur, try again later");
                    TapDynamic(serial, "\"OK\"");
                    Common.Sleep(1000);
                    continue;
                }


                if (ContainsIgnoreCase(TextDump, "\"try again\""))
                {
                    Common.SetStatus(serial, "No internet, try again");
                    TapDynamic(serial, "\"try again\"");
                    Common.Sleep(1000);
                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "no internet connection") && ContainsIgnoreCase(TextDump, "retry"))
                {
                    Common.SetStatus(serial, "no internet connection");
                    TapDynamic(serial, "\"Retry\"");
                    Common.Sleep(1000);
                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "confirm mobifone billing details")
                    || ContainsIgnoreCase(TextDump, "confirm viettel telecom billing details") 
                    || ContainsIgnoreCase(TextDump, "confirm vinaphone billing details"))
                {
                    Common.SetStatus(serial, "Confirm name and postal code billing details");
                    TapDynamic(serial, "\"Save\"");
                    Common.Sleep(1000);
                    continue;
                }
                // Khi đã thêm phương thức thanh toán nhà mạng
                if (ContainsIgnoreCase(TextDump, "mobifone +") || ContainsIgnoreCase(TextDump, "viettel telecom +") || ContainsIgnoreCase(TextDump, "vinaphone +"))
                {
                    if (ContainsIgnoreCase(TextDump, "add mobifone billing") || ContainsIgnoreCase(TextDump, "add viettel telecom billing") || ContainsIgnoreCase(TextDump, "add vinaphone billing"))
                    {
                        // Nhưng vẫn có dòng add billing (unavailable)
                        if (ContainsIgnoreCase(TextDump, "add mobifone billing"))
                        {
                            Common.SetStatus(serial, "Add Mobifone Billing");
                            TapDynamic(serial, "add mobifone billing\"");
                        }
                        if (ContainsIgnoreCase(TextDump, "add viettel telecom billing"))
                        {
                            Common.SetStatus(serial, "Add Viettel telecom Billing");
                            TapDynamic(serial, "add viettel telecom billing\"");
                        }
                        if (ContainsIgnoreCase(TextDump, "add vinaphone billing"))
                        {
                            Common.SetStatus(serial, "Add Vinaphone Billing");
                            TapDynamic(serial, "add vinaphone billing\"");
                        }
                        Common.Sleep(1000);
                        continue;
                    } else
                    {
                        // Không còn dòng add billing
                        return TaskResult.Success;
                    }
                }
                // Bắt đầu thêm thanh toán
                if (ContainsIgnoreCase(TextDump, "add mobifone billing") || ContainsIgnoreCase(TextDump, "add viettel telecom billing") || ContainsIgnoreCase(TextDump, "add vinaphone billing"))
                {
                    if (ContainsIgnoreCase(TextDump, "add mobifone billing"))
                    {
                        Common.SetStatus(serial, "Add Mobifone Billing");
                        TapDynamic(serial, "add mobifone billing\"");
                    }
                    if (ContainsIgnoreCase(TextDump, "add viettel telecom billing"))
                    {
                        Common.SetStatus(serial, "Add Viettel telecom Billing");
                        TapDynamic(serial, "add viettel telecom billing\"");
                    }
                    if (ContainsIgnoreCase(TextDump, "add vinaphone billing"))
                    {
                        Common.SetStatus(serial, "Add Vinaphone Billing");
                        TapDynamic(serial, "add vinaphone billing\"");
                    }
                    Common.Sleep(1000);
                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "com.android.launcher3"))
                {
                    Common.SetStatus(serial, "Kickout Home, Reloading");
                    Funcs.OpenPaymentMethods(serial);
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

