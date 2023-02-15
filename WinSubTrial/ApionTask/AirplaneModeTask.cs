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
    class AirplaneModeTask
    {
        public bool isStopAuto = false;

        private string TextDump;
        private readonly Random _rand = new Random();

        public TaskResult TurnOnAirPlane(string serial)
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

                Common.Sleep(500);
                TapDynamic(serial, "\"airplane mode\"");
                Common.Sleep(1000);
                
                Adb.SendKey(serial, "KEYCODE_HOME");
                return TaskResult.Success;

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
