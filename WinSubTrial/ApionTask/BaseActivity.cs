using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace WinSubTrial.ApionTask
{
    class BaseActivity
    {
        protected readonly Random _rand = new Random();
        protected string TextDump;
        public string RandomPasswordString()
        {
            int length = _rand.Next(12, 14);
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[_rand.Next(s.Length)]).ToArray());
        }

        protected void TapPosition(string serial, Point point)
        {
            point.X += _rand.Next(5, 20);
            point.Y += _rand.Next(5, 20);
            Adb.Shell(serial, $"input tap {point.X} {point.Y}");
        }

        protected void SwipeUp(string serial)
        {
            Adb.Shell(serial, "input swipe 500 1700 50 50");
        }

        protected void Enter(string serial)
        {
            Adb.SendKey(serial, "KEYCODE_ENTER");
        }
        protected void Tab(string serial)
        {
            Adb.SendKey(serial, "KEYCODE_TAB");
        }


        protected void Back(string serial)
        {
            Adb.SendKey(serial, "KEYCODE_BACK");
        }

        protected void InputDynamic(string serial, string query, string text)
        {
            Point point = GetPointFromUi(query);
            if (point != default) Adb.Shell(serial, $"input tap {point.X} {point.Y}");
            Input(serial, text);
        }

        protected void Input(string serial, string text, bool isClear = true)
        {
            if (isClear)
            {
                Adb.SendKey(serial, "123");
                Adb.SendKey(serial, "--longpress 67 67 67 67 67 67 67 67 67 67 67");
            }
            Adb.Shell(serial, $"input text {text}");
        }

        protected void InputClipboard(string serial, bool isClear = true)
        {
            if (isClear)
            {
                Adb.SendKey(serial, "123");
                Adb.SendKey(serial, "--longpress 67 67 67 67 67 67 67 67 67 67 67");
            }
            Adb.Shell(serial, $"input keyevent 279");
        }

        protected Point GetPointFromUi(string query)
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

        protected Point GetPointFromUiNotIgnore(string query)
        {
            Point point = default;
            try
            {
                string value = Regex.Match(TextDump, $@"({query}[^\>]+)>").Groups[1].Value;
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

        protected void TapDynamic(string serial, string text)
        {
            Point point = GetPointFromUi(text);
            point.X += _rand.Next(5, 15);
            point.Y += _rand.Next(5, 15);
            Adb.Shell(serial, $"input tap {point.X} {point.Y}");
        }
        protected void TapDynamicNotIgnore(string serial, string text)
        {
            Point point = GetPointFromUiNotIgnore(text);
            point.X += _rand.Next(5, 15);
            point.Y += _rand.Next(5, 15);
            Adb.Shell(serial, $"input tap {point.X} {point.Y}");
        }
        public bool ContainsIgnoreCase(string source, string subString)
        {
            return Regex.IsMatch(source, subString, RegexOptions.IgnoreCase);
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
        //Đóng app
        protected void CloseApp(string serial, string appName)
        {
            switch (appName)
            {
                case "snapchat":
                    Adb.Shell(serial, "pm clear com.snapchat.android");
                    break;
                case "tinder":
                    Adb.Shell(serial, "pm clear com.tinder");
                    break;
                case "bigo":
                    Adb.Shell(serial, "pm clear sg.bigo.live");
                    break;
                case "getcodeapi":
                    Adb.Shell(serial, "pm clear com.example.getcodeapi");
                    break;
            }
        }
        //Mở app
        protected void OpenGetCodeApi(string serial)
        {
            Adb.Shell(serial, "am start -n com.example.getcodeapi/.MainActivity");
        }

        protected void OpenTinderApp(string serial)
        {
            Adb.Shell(serial, " am start -n com.tinder/.activities.LoginActivity");
        }

        protected void OpenSnapchatApp(string serial)
        {
            Adb.Shell(serial, "am start -n com.snapchat.android/.LandingPageActivity");
        }

        protected void OpenBigoApp(string serial)
        {
            Adb.Shell(serial, "am start -n sg.bigo.live/.home.MainActivity");
        }
    }
}
