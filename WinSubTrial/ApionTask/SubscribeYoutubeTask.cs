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
    class SubscribeYoutubeTask
    {
        public bool isStopAuto = false;
        private string TextDump;
        private readonly Random _rand = new Random();

        private string[] searchArray = { "nugenesis-nails-vietnam" };

        public TaskResult LikeAndSubscribe(string serial)
        {
            Adb.SendKeyOnce(serial, "KEYCODE_HOME");
            Common.SetStatus(serial, "Open Youtube");
            Common.Sleep(_rand.Next(1000, 2000));
            OpenYoutubeAndSearch(serial);
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
                    Common.SetStatus(serial, "Timeout, LikeAndSubscribe, reset device");
                    return TaskResult.Failure;
                }

                DumpUi(serial);
                // Error first
                if (ContainsIgnoreCase(TextDump, "channel_name"))
                {
                    Common.SetStatus(serial, "channel_name");
                    TapDynamic(serial, "channel_name");
                    Common.Sleep(_rand.Next(1000, 2000));
                    DumpUi(serial);
                    Common.Sleep(_rand.Next(1000, 2000));
                    if (ContainsIgnoreCase(TextDump, "VIDEOS"))
                    {
                        Common.SetStatus(serial, "VIDEOS");
                        TapDynamic(serial, "\"VIDEOS\"");
                        Common.Sleep(_rand.Next(1000, 3000));
                    }
                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "ago - play video"))
                {
                    Common.SetStatus(serial, "ago - play video");
                    TapDynamic(serial, "ago - play video");
                    Common.Sleep(_rand.Next(1000, 2000));
                    continue;
                }

                if (ContainsIgnoreCase(TextDump, "skip_ad_button_text"))
                {
                    Common.SetStatus(serial, "skip_ad_button_text");
                    TapDynamic(serial, "skip_ad_button_text");
                    //Common.Sleep(10*60*1000);
                    Common.Sleep(_rand.Next(11000, 12000));
                    if (ContainsIgnoreCase(TextDump, "skip_ad_button_text"))
                    {
                        Common.SetStatus(serial, "skip_ad_button_text");
                        TapDynamic(serial, "skip_ad_button_text");
                        Common.Sleep(_rand.Next(5000, 6000));
                    }
                    if (ContainsIgnoreCase(TextDump, "like_button"))
                    {
                        Common.SetStatus(serial, "like_button");
                        TapDynamic(serial, "like_button");
                        Common.Sleep(_rand.Next(5000, 6000));
                        if (ContainsIgnoreCase(TextDump, "subscribe to"))
                        {
                            TapDynamic(serial, "subscribe to");
                            Common.Sleep(_rand.Next(5000, 6000));
                        }
                        Common.Sleep(_rand.Next(5000, 6000));

                        //OpenYoutubeAndSearch(serial);
                        continue;
                    }
                    continue;
                }
                
                



                if (ContainsIgnoreCase(TextDump, "com.android.launcher3"))
                {
                    Common.SetStatus(serial, "Kickout Home, Reloading");
                    OpenYoutubeAndSearch(serial);
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

        private void OpenYoutubeAndSearch(string serial)
        {
            Adb.CloseApp(serial, "com.google.android.gms");
            Adb.Shell(serial, "am start -a com.google.android.youtube.action.open.search -n com.google.android.youtube/com.google.android.apps.youtube.app.WatchWhileActivity");
            Common.Sleep(_rand.Next(3000, 4000));
            Input(serial, searchArray[_rand.Next(0, searchArray.Length)]);
            Common.Sleep(_rand.Next(2000, 3000));
            Enter(serial);
            Common.Sleep(_rand.Next(3000, 4000));
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
