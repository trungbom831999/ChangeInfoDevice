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
    class PastePasswordTask
    {
        public bool isStopAuto = false;
        private string TextDump;
        private readonly Random _rand = new Random();
        public Gmail gmail { get; set; }
        public string appPackage { get; set; }
        private int rebootCounts = 0;

        public TaskResult PasteTask(string serial)
        {
            Common.SetStatus(serial, "Paste password");
            Common.Sleep(_rand.Next(500, 1000));
            DateTime startTime = DateTime.Now;
            // eslint-disable-next-line no-constant-condition
            while (true)
            {
                if (isStopAuto)
                {
                    Common.SetStatus(serial, "Stopped paste password");
                    return TaskResult.StopAuto;
                }
                DateTime currentTime = DateTime.Now;
                if ((currentTime - startTime).TotalSeconds > 240)
                {
                    Common.SetStatus(serial, "Timeout paste password, Run next app or remove account");
                    rebootCounts += 1;
                    startTime = currentTime;
                    if (rebootCounts > 3)
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
                if (ContainsIgnoreCase(TextDump, "com.android.launcher3"))
                {
                    Common.SetStatus(serial, "Kickout Home, reopen app");
                    Adb.OpenApp(serial, appPackage);
                    Common.Sleep(_rand.Next(3000, 4000));
                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "manage backup payment settings"))
                {
                    Common.SetStatus(serial, "Manage backup payment - No, thanks");
                    TapDynamic(serial, "\"no, thanks\"");
                    Common.Sleep(3000);
                    return TaskResult.Success;
                }
                if (ContainsIgnoreCase(TextDump, "error") && ContainsIgnoreCase(TextDump, "your order is still being processed"))
                {
                    Common.SetStatus(serial, "Your order is still being processed");
                    TapDynamic(serial, "\"OK\"");
                    Common.Sleep(3000);
                    return TaskResult.Success;
                }
                if (ContainsIgnoreCase(TextDump, "error") && ContainsIgnoreCase(TextDump, "already subscribed"))
                {
                    Common.SetStatus(serial, "You're already subscribed");
                    TapDynamic(serial, "\"OK\"");
                    Common.Sleep(3000);
                    return TaskResult.Success;
                }
                if (ContainsIgnoreCase(TextDump, "this payment method has been declined"))
                {
                    Common.SetStatus(serial, "This payment method has been declined");
                    Common.Sleep(3000);
                    return TaskResult.Failure;
                }
                if (ContainsIgnoreCase(TextDump, "error") && ContainsIgnoreCase(TextDump, "your transaction"))
                {
                    Common.SetStatus(serial, "Your transaction can't be completed");
                    TapDynamic(serial, "\"OK\"");
                    Common.Sleep(3000);
                    return TaskResult.Failure;
                }
                if (ContainsIgnoreCase(TextDump, "error") && ContainsIgnoreCase(TextDump, "connection timed out"))
                {
                    Common.SetStatus(serial, "Your order is still being processed");
                    TapDynamic(serial, "\"OK\"");
                    Common.Sleep(3000);
                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "error"))
                {
                    Common.SetStatus(serial, "Error, your transaction cant be completed");
                    Common.Sleep(3000);
                    return TaskResult.Failure;
                }
                if (ContainsIgnoreCase(TextDump, "payment successful") && ContainsIgnoreCase(TextDump, "\"yes, always\""))
                {
                    Common.SetStatus(serial, "Payment successful - No, thanks");
                    TapDynamic(serial, "\"no, thanks\"");
                    Common.Sleep(500);
                    TapDynamic(serial, "\"OK\"");
                    Common.Sleep(1000);
                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "connection time out"))
                {
                    Common.SetStatus(serial, "Connection time out");
                    TapDynamic(serial, "\"OK\"");
                    Common.Sleep(2000);
                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "\"subscribed\""))
                {
                    Common.SetStatus(serial, "Subscribed, wait");
                    Common.Sleep(2000);
                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "\"processing\""))
                {
                    Common.SetStatus(serial, "Waiting processing done");
                    Common.Sleep(7000);
                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "mobifone +") || ContainsIgnoreCase(TextDump, "viettel telecom +") || ContainsIgnoreCase(TextDump, "vinaphone +"))
                {
                    if (ContainsIgnoreCase(TextDump, "\"remember me on this device\""))
                    {
                        Common.SetStatus(serial, "Enter password to Subscribe");
                        InputDynamic(serial, "edittext", gmail.Password);
                        DumpUi(serial);
                        TapDynamic(serial, "\"Remember me on this device\"");
                        Common.Sleep(1000);
                        Enter(serial);
                        continue;
                    }
                }
                if (ContainsIgnoreCase(TextDump, "\"com.android.vending:id/button_group\""))
                {
                    Common.SetStatus(serial, "Tap vending button_group twice!");
                    Common.Sleep(3000);
                    TapDynamic(serial, "\"com.android.vending:id/button_group\"");
                    Common.Sleep(500);
                    TapDynamic(serial, "\"com.android.vending:id/button_group\"");
                    Common.Sleep(1000);
                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "\"subscribe\""))
                {
                    if (ContainsIgnoreCase(TextDump, "by tapping \"Subscribe\", you accept"))
                    {
                        Common.SetStatus(serial, "Tap position 742 2471 subscribe twice!");
                        Common.Sleep(3000);
                        TapPosition(serial, new Point(720, 2471));
                        Common.Sleep(500);
                        TapPosition(serial, new Point(720, 2471));
                    } else if (ContainsIgnoreCase(TextDump, "upcoming charges"))
                    {
                        Common.SetStatus(serial, "Tap button subscribe twice!");
                        Common.Sleep(3000);
                        TapSecondDynamic(serial, "\"subscribe\"");
                        Common.Sleep(500);
                        TapSecondDynamic(serial, "\"subscribe\"");
                    } else
                    {
                        Common.SetStatus(serial, "Tap button subscribe once!");
                        Common.Sleep(3000);
                        TapSecondDynamic(serial, "\"subscribe\"");
                    }
                    Common.Sleep(1000);
                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "Web View") && ContainsIgnoreCase(TextDump, "View site information"))
                {
                    Adb.SendKey(serial, "KEYCODE_BACK");
                    Common.Sleep(2000);
                    continue;
                }

                // Adding payment method
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
                            Common.Sleep(2000);
                            DumpUi(serial);
                            TapDynamic(serial, "\"Enable\"");
                        }
                        if (ContainsIgnoreCase(TextDump, "add viettel telecom billing"))
                        {
                            Common.SetStatus(serial, "Add Viettel telecom Billing");
                            TapDynamic(serial, "add viettel telecom billing\"");
                            Common.Sleep(2000);
                            DumpUi(serial);
                            TapDynamic(serial, "\"Enable\"");
                        }
                        if (ContainsIgnoreCase(TextDump, "add vinaphone billing"))
                        {
                            Common.SetStatus(serial, "Add Vinaphone Billing");
                            TapDynamic(serial, "add vinaphone billing\"");
                            Common.Sleep(2000);
                            DumpUi(serial);
                            TapDynamic(serial, "\"Enable\"");
                        }
                        Common.Sleep(1000);
                        continue;
                    }
                    else
                    {
                        // Không còn dòng add billing
                        continue;
                    }
                }
                // Bắt đầu thêm thanh toán
                if (ContainsIgnoreCase(TextDump, "add mobifone billing") || ContainsIgnoreCase(TextDump, "add viettel telecom billing") || ContainsIgnoreCase(TextDump, "add vinaphone billing"))
                {
                    if (ContainsIgnoreCase(TextDump, "add mobifone billing"))
                    {
                        Common.SetStatus(serial, "Add Mobifone Billing");
                        TapDynamic(serial, "add mobifone billing\"");
                        Common.Sleep(2000);
                        DumpUi(serial);
                        TapDynamic(serial, "\"Enable\"");
                    }
                    if (ContainsIgnoreCase(TextDump, "add viettel telecom billing"))
                    {
                        Common.SetStatus(serial, "Add Viettel telecom Billing");
                        TapDynamic(serial, "add viettel telecom billing\"");
                        Common.Sleep(2000);
                        DumpUi(serial);
                        TapDynamic(serial, "\"Enable\"");
                    }
                    if (ContainsIgnoreCase(TextDump, "add vinaphone billing"))
                    {
                        Common.SetStatus(serial, "Add Vinaphone Billing");
                        TapDynamic(serial, "add vinaphone billing\"");
                        Common.Sleep(2000);
                        DumpUi(serial);
                        TapDynamic(serial, "\"Enable\"");
                    }
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
                if (ContainsIgnoreCase(TextDump, "An error has occur, try again later"))
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
                Common.Sleep(_rand.Next(3000, 4000));
            }
        }

        private void TapPosition(string serial, Point point)
        {
            point.X += _rand.Next(5, 25);
            point.Y += _rand.Next(5, 25);
            Adb.Shell(serial, $"input tap {point.X} {point.Y}");
        }

        private void TapDynamic(string serial, string text)
        {
            Point point = GetPointFromUi(text);
            point.X += _rand.Next(5, 25);
            point.Y += _rand.Next(5, 25);
            Adb.Shell(serial, $"input tap {point.X} {point.Y}");
        }

        private void TapSecondDynamic(string serial, string text)
        {
            Point point = GetSecondPointFromUi(text);
            point.X += _rand.Next(5, 25);
            point.Y += _rand.Next(5, 25);
            Adb.Shell(serial, $"input tap {point.X} {point.Y}");
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

        private Point GetSecondPointFromUi(string query)
        {
            Point point = default;
            try
            {
                string value = Regex.Match(TextDump, $@"({query}[^\>]+)>", RegexOptions.IgnoreCase).Groups[1].Value;
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
