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
    class AutoClickTacTask
    {
        public bool isStopAuto = false;
        private string TextDump;
        private readonly Random _rand = new Random();
        private string[] searchArray = { "honda%scrv", "xe%shonda%scrv", "xe%shonda%scr-v", "honda%scr-v" };
        private string[] linkArray = {
            "hondalongbien.net.vn",
            "hondacrv.vn",
            "hondaoto-hanoi.com",
            "hondaotolongbienhn.com",
            "hondaotoankhanh.com.vn",
            "hondaoto-mydinh.com.vn",
            "honda-hanoi.com.vn",
            "honda-tayho.vn",
            "hondagiaiphonghanoi.vn",
            "hondaotohanoi5s.com",
            "hondaotohanoi-longbien.com",
            "honda5s.com",
            "hondaotohaiphongvn.com",
            "hondalongbienoto.com",
            "hondaotohanoi5s.com",
            "hondaotovietnam.vn",
            "hondaankhanh3s.com",
            "hondamydinh.gianhangvn.com.vn",
            "giaxeotohonda.vn",
            "hondamydinh.com.vn",
            "hondatayho.com.vn",
            "honda-otomydinh.com",
            "hondagiaiphong.net",
            "hondaotomydinh.com",
            "hondaoto-hanoi.com",
            "honda-hanoi.com.vn",

            "hondagiaiphong.vn",
            "hondalongbien.vn",
            "hondaotohanoi.net",
            "subaru.asia",
            "hondaankhanh3s.com"

        };

        public TaskResult ClickLink(string serial)
        {
            Adb.SendKeyOnce(serial, "KEYCODE_HOME");
            Common.SetStatus(serial, "Open Google Chrome");
            Common.Sleep(_rand.Next(1000, 2000));
            OpenChrome(serial);
            Common.Sleep(_rand.Next(3000, 4000));
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
                if ((currentTime - startTime).TotalSeconds > 300)
                {
                    Common.SetStatus(serial, "Timeout, click auto, reset device");
                    return TaskResult.Failure;
                }

                DumpUi(serial);
                // Error first
                if (ContainsIgnoreCase(TextDump, "Accept &amp; continue"))
                {
                    Common.SetStatus(serial, "Welcome to Chrome, Accept & continue");
                    TapDynamic(serial, "\"Accept &amp; continue\"");
                    Common.Sleep(1000);
                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "No thanks"))
                {
                    Common.SetStatus(serial, "Turn on sync? No thanks");
                    TapDynamic(serial, "\"No thanks\"");
                    Common.Sleep(1000);
                    continue;
                }

                if (ContainsIgnoreCase(TextDump, "search_box_text") && ContainsIgnoreCase(TextDump, "Search or type web address"))
                {
                    Common.SetStatus(serial, "Search or type web address");
                    InputDynamic(serial, "search_box_text", searchArray[_rand.Next(0, searchArray.Length)]);
                    Common.Sleep(_rand.Next(2000, 3000));
                    Enter(serial);
                    Common.Sleep(_rand.Next(3000, 4000));
                    DumpUi(serial);
                    Common.Sleep(_rand.Next(500, 1000));
                    foreach (string path in linkArray)
                    {
                        if (ContainsIgnoreCase(TextDump, path))
                        {
                            Point point = GetPointFromUi(path);
                            Common.SetStatus(serial, $"{path}: toa do X: {point.X}, toa do Y: {point.Y}");
                            Common.Sleep(1000);
                            if ((point.X == 0 && point.Y == 0) || (point.Y > 2560))
                            {
                                Common.SetStatus(serial, "Khong the click toa do nay");
                                Common.Sleep(1000);
                            }
                            else
                            {
                                Common.SetStatus(serial, $"Tapped {path}");
                                TapDynamic(serial, path);
                                Common.Sleep(1000);
                                break;
                            }
                        }
                    }
                    Common.Sleep(_rand.Next(9000, 10000));
                    SwipeUp(serial);
                    Common.Sleep(_rand.Next(6000, 7000));
                    Adb.SendKeyOnce(serial, "KEYCODE_BACK");
                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "Google") && ContainsIgnoreCase(TextDump, "edittext"))
                {
                    Common.SetStatus(serial, "Click edit text Honda crv");
                    InputDynamic(serial, "edittext", searchArray[_rand.Next(0, searchArray.Length)]);
                    Common.Sleep(_rand.Next(2000, 3000));
                    Enter(serial);
                    Common.Sleep(_rand.Next(3000, 4000));
                    DumpUi(serial);
                    Common.Sleep(_rand.Next(500, 1000));
                    foreach (string path in linkArray)
                    {
                        if (ContainsIgnoreCase(TextDump, path))
                        {
                            Point point = GetPointFromUi(path);
                            Common.SetStatus(serial, $"{path}: toa do X: {point.X}, toa do Y: {point.Y}");
                            Common.Sleep(1000);
                            if ((point.X == 0 && point.Y == 0) || (point.Y > 2560))
                            {
                                Common.SetStatus(serial, "Khong the click toa do nay");
                                Common.Sleep(1000);
                            }
                            else
                            {
                                Common.SetStatus(serial, $"Tapped {path}");
                                TapDynamic(serial, path);
                                Common.Sleep(1000);
                                break;
                            }
                        }
                    }
                    Common.Sleep(_rand.Next(9000, 10000));
                    SwipeUp(serial);
                    Common.Sleep(_rand.Next(6000, 7000));
                    Adb.SendKeyOnce(serial, "KEYCODE_BACK");
                    continue;
                }


                if (ContainsIgnoreCase(TextDump, "Get Facebook for Android and browse faster."))
                {
                    Common.SetStatus(serial, "Get Facebook for Android and browse faster.");
                    Adb.SendKeyOnce(serial, "KEYCODE_BACK");
                    Common.Sleep(5000);
                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "com.android.launcher3"))
                {
                    Common.SetStatus(serial, "Kickout Home, Reloading");
                    OpenChrome(serial);
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
            Adb.SendKeyOnce(serial, "KEYCODE_ENTER");
        }

        private void TapPosition(string serial, Point point)
        {
            point.X += _rand.Next(5, 25);
            point.Y += _rand.Next(5, 25);
            Adb.Shell(serial, $"input tap {point.X} {point.Y}");
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

        private void OpenChrome(string serial)
        {
            Adb.CloseApp(serial, "com.google.android.gms");
            Adb.Shell(serial, "am start -n com.android.chrome/com.google.android.apps.chrome.Main");
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
