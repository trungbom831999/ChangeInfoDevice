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
    class TouchPositionTask
    {
        public bool isStopAuto = false;
        private string TextDump;
        private readonly Random _rand = new Random();

        public Point point1 { get; set; }
        public Point point2 { get; set; }
        public Point point3 { get; set; }

        public string appPackage { get; set; }

        public bool TouchPosition(string serial)
        {
            Common.SetStatus(serial, "Touch position by point");
            Common.Sleep(_rand.Next(1000, 2000));
            DateTime startTime = DateTime.Now;
            TouchAllPosition(serial);

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
                    Adb.Reboot(serial);
                    Adb.Run(serial, "wait-for-device");
                    continue;
                }
                DumpUi(serial);


                //Debug.WriteLine($"TextDump: {TextDump}");

                // Error first
                if (ContainsIgnoreCase(TextDump, "com.android.launcher3"))
                {
                    Common.SetStatus(serial, "Kickout Home, reopen app");
                    Adb.OpenApp(serial, appPackage);
                    Common.Sleep(_rand.Next(3000, 4000));
                    TouchAllPosition(serial);
                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "\"com.android.vending:id/button_group\""))
                {
                    Common.SetStatus(serial, "Contain vending button_group!");
                    return true;
                }
                else if (ContainsIgnoreCase(TextDump, "\"subscribe\""))
                {
                    Common.SetStatus(serial, "Contain subscribe button");
                    return true;
                }
                else
                {
                    Adb.SendKey(serial, "KEYCODE_HOME");
                    Adb.CloseApp(serial, appPackage);

                    Common.Sleep(_rand.Next(1000, 2000));
                    OpenAppWithoutActivity(serial);

                    Common.Sleep(_rand.Next(1000, 2000));
                    TouchAllPosition(serial);
                    continue;
                }

                Common.Sleep(_rand.Next(3000, 4000));
            }
        }

        private void TouchAllPosition(string serial)
        {
            TapPosition(serial, point1);
            TapPosition(serial, point1);
            Common.Sleep(_rand.Next(1000, 2000));
            TapPosition(serial, point2);
            TapPosition(serial, point2);
            Common.Sleep(_rand.Next(1000, 2000));
            TapPosition(serial, point3);
            Common.Sleep(_rand.Next(1000, 2000));
        }

        private void OpenAppWithoutActivity(string serial)
        {
            Adb.Shell(serial, $"monkey -p {appPackage} -c android.intent.category.LAUNCHER 1");
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
    }
}
