using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WinSubTrial.Utilities;
using System.Diagnostics;


namespace WinSubTrial
{
    class FindExactlyContentAndTouch
    {
        public bool isStopAuto = false;
        private string TextDump;
        private readonly Random _rand = new Random();
        public string content { get; set; }
        public string content2 { get; set; }
        public string appPackage { get; set; }

        public bool TouchExactly(string serial)
        {
            Common.SetStatus(serial, $"Find exactly and touch {content} and {content2}");
            Common.Sleep(_rand.Next(500, 1000));
            DateTime startTime = DateTime.Now;
            // eslint-disable-next-line no-constant-condition
            while (true)
            {
                if (isStopAuto)
                {
                    Common.SetStatus(serial, "Stopped find exactly and touch");
                    return false;
                }
                DateTime currentTime = DateTime.Now;
                if ((currentTime - startTime).TotalSeconds > 240)
                {
                    Common.SetStatus(serial, "Timeout, Stopped find exactly");

                    return true;
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
                if (content == "" || content2 == "")
                {
                    Common.SetStatus(serial, $"Content 1 or 2 empty! Cant find and touch: {content2}");
                    return false;
                }
                if (ContainsIgnoreCase(TextDump, $"\"{content}\"") && ContainsIgnoreCase(TextDump, $"\"{content2}\""))
                {
                    Common.SetStatus(serial, $"Tapped: {content2}");
                    TapDynamic(serial, $"\"{content2}\"");
                    Common.Sleep(1000);
                    return true;
                }

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
