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
    class WifiProxyTask
    {
        public bool isStopAuto = false;

        private string TextDump;
        private readonly Random _rand = new Random();

        public TaskResult LoginWifiProxy(string serial)
        {
            Adb.SendKey(serial, "KEYCODE_HOME");
            Common.SetStatus(serial, "Open Air plane");
            Adb.Shell(serial, "am start -a android.settings.AIRPLANE_MODE_SETTINGS");
            Common.Sleep(_rand.Next(500, 1000));
            DateTime startTime = DateTime.Now;
            // eslint-disable-next-line no-constant-condition
            while (true)
            {
                if (isStopAuto)
                {
                    Common.SetStatus(serial, "Stopped Auto");

                    return TaskResult.StopAuto;
                }
                DateTime currentTime = DateTime.Now;
                if ((currentTime - startTime).TotalSeconds > 600)
                {
                    return TaskResult.Failure;
                }
                DumpUi(serial);

                if (ContainsIgnoreCase(TextDump, "off") && ContainsIgnoreCase(TextDump, "Airplane mode"))
                {
                    Common.SetStatus(serial, "Tapped wifi");
                    TapDynamic(serial, "off");
                    Common.Sleep(1000);
                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "not connected") && ContainsIgnoreCase(TextDump, "Airplane mode"))
                {
                    Common.SetStatus(serial, "Tapped wifi");
                    TapDynamic(serial, "not connected");
                    Common.Sleep(1000);
                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "\"com.android.settings:id/switch_text\"") && ContainsIgnoreCase(TextDump, "1500wifi"))
                {
                    Common.SetStatus(serial, "Tapped 42 vo thi sau");
                    TapDynamic(serial, "1500wifi");
                    Common.Sleep(1000);
                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "\"com.android.settings:id/switch_text\"") && !ContainsIgnoreCase(TextDump, "1500wifi"))
                {
                    Common.SetStatus(serial, "Tapped use wifi on");
                    TapDynamic(serial, "com.android.settings:id/switch_text");
                    Common.Sleep(10000);
                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "com.android.settings:id/proxy_hostname") && ContainsIgnoreCase(TextDump, "proxy hostname"))
                {
                    Common.SetStatus(serial, "Input Proxy hostname");
                    InputDynamic(serial, "com.android.settings:id/proxy_hostname", "apollo.p.shifter.io");
                    SwipeUp(serial);
                    Common.Sleep(1000);
                    DumpUi(serial);
                    Common.Sleep(1000);
                    InputDynamic(serial, "com.android.settings:id/proxy_port", $"265{_rand.Next(30, 80)}");
                    Common.Sleep(1000);
                    TapDynamic(serial, "android:id/button1");
                    Common.Sleep(1000);

                    return TaskResult.Success;
                }

                if (ContainsIgnoreCase(TextDump, "android:id/text1") && ContainsIgnoreCase(TextDump, "Manual"))
                {
                    Common.SetStatus(serial, "Tapped Proxy Manual");
                    TapDynamic(serial, "Manual");
                    Common.Sleep(1000);
                    SwipeUp(serial);
                    Common.Sleep(1000);
                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "android:id/text1") && ContainsIgnoreCase(TextDump, "None"))
                {
                    Common.SetStatus(serial, "Tapped Proxy None");
                    TapDynamic(serial, "None");
                    Common.Sleep(1000);
                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "com.android.settings:id/wifi_advanced_toggle"))
                {
                    Common.SetStatus(serial, "Enter password 1500wifi");
                    InputDynamic(serial, "edittext", "Mabugau2612");
                    Common.Sleep(1000);
                    Common.SetStatus(serial, "Tapped advanced toggle");
                    TapDynamic(serial, "com.android.settings:id/wifi_advanced_toggle");
                    Common.Sleep(1000);
                    SwipeUp(serial);
                    Common.Sleep(1000);
                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "1500wifi") && !ContainsIgnoreCase(TextDump, "Show password"))
                {
                    Common.SetStatus(serial, "Enter password 1500wifi");
                    InputDynamic(serial, "edittext", "Mabugau2612");
                    Common.Sleep(1000);
                    continue;
                }

                Common.Sleep(_rand.Next(3000, 4000));

            }
        }
        private void SwipeUp(string serial)
        {
            Adb.Shell(serial, "input swipe 200 500 200 0");
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

        private void TapDynamic(string serial, string text)
        {
            Point point = GetPointFromUi(text);
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
    }
}
