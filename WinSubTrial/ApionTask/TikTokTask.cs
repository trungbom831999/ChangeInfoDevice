using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Utils;
using WinSubTrial.Utilities;

namespace WinSubTrial
{
    class TikTokTask
    {
        public bool isStopAuto = false;
        public string phonenumber { get; set; }

        private string TextDump;
        private readonly Random _rand = new Random();

        public TaskResult LoginTiktok(string serial)
        {
            Adb.SendKey(serial, "KEYCODE_HOME");
            Common.SetStatus(serial, "Open Get code api");
            OpenGetCodeApi(serial);
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
                if ((currentTime - startTime).TotalSeconds > 600)
                {
                    Common.SetStatus(serial, "Timeout, timed out");
                    return TaskResult.Failure;
                }

                DumpUi(serial);
                // Error first
                if (ContainsIgnoreCase(TextDump, "url") && !ContainsIgnoreCase(TextDump, "103.179.187.93"))
                {
                    Common.SetStatus(serial, "Enter url");
                    InputDynamic(serial, "editUrl", "103.179.187.93");
                    Common.Sleep(500);

                    Common.SetStatus(serial, "Enter phone number");
                    InputDynamic(serial, "editPhone", phonenumber);
                    Common.Sleep(500);

                    Common.SetStatus(serial, "Enter Brand");
                    InputDynamic(serial, "editBrand", "T");
                    Common.Sleep(500);

                    Common.SetStatus(serial, "tappeb btnGetCode");
                    TapDynamic(serial, "btnGetCode");
                    Common.Sleep(3000);
                    OpenTikTokApp(serial);
                    Common.Sleep(5000);
                    continue;
                }

                if (ContainsIgnoreCase(TextDump, "Agree and continue"))
                {
                    Common.SetStatus(serial, "Tapped Agree and continue");
                    TapPosition(serial, new Point(610, 1500));
                    Common.Sleep(2000);
                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "Skip"))
                {
                    Common.SetStatus(serial, "Tapped Skip");
                    TapDynamic(serial, "Skip");
                    Common.Sleep(2000);
                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "Swipe up"))
                {
                    Common.SetStatus(serial, "Tapped Swipe up");
                    SwipeUp(serial);
                    Common.Sleep(3000);
                    SwipeUp(serial);
                    SwipeUp(serial);
                    Common.Sleep(5000);
                    DumpUi(serial);

                    if (ContainsIgnoreCase(TextDump, "Log in or sign up"))
                    {
                        Common.SetStatus(serial, "Tapped Log in or sign up");
                        TapDynamic(serial, "Log in or sign up");
                        Common.Sleep(2000);
                        DumpUi(serial);
                        continue;
                    }

                    Common.SetStatus(serial, "tappeb profile");
                    TapPosition(serial, new Point(1177, 2520));
                    Common.Sleep(3000);
                    DumpUi(serial);

                    if (ContainsIgnoreCase(TextDump, "com.ss.android.ugc.trill:id/acz"))
                    {
                        Common.SetStatus(serial, "Tapped Sigh up");
                        TapDynamic(serial, "com.ss.android.ugc.trill:id/acz");
                        Common.Sleep(3000);
                        continue;
                    }
                    continue;
                }



                if (ContainsIgnoreCase(TextDump, "Use phone or email"))
                {
                    Common.SetStatus(serial, "tapped Use phone or email");
                    TapDynamic(serial, "com.ss.android.ugc.trill:id/akc");
                    Common.Sleep(3000);
                    continue;
                }

                if (ContainsIgnoreCase(TextDump, "com.google.android.gms:id/cancel"))
                {
                    Common.SetStatus(serial, "tapped None of above");
                    TapDynamic(serial, "com.google.android.gms:id/cancel");
                    Common.Sleep(3000);
                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "Phone number") && ContainsIgnoreCase(TextDump, "edittext"))
                {
                    while (true)
                    {
                        DumpUi(serial);
                        Common.Sleep(1000);

                        if (ContainsIgnoreCase(TextDump, "Phone number") && ContainsIgnoreCase(TextDump, "edittext"))
                        {
                            Common.SetStatus(serial, "Enter Phone number");
                            InputDynamic(serial, "edittext", phonenumber);
                            Common.Sleep(3000);
                            Common.SetStatus(serial, "Tapped Send code");
                            TapDynamic(serial, "Send code");
                            Common.Sleep(10000);
                            continue;
                        }

                        if (ContainsIgnoreCase(TextDump, "Captcha"))
                        {
                            Common.SetStatus(serial, "Captcha detected, screen shot");
                            string randomFile = RandomPasswordString();
                            Adb.Run(serial, $"exec-out screencap -p > {randomFile}.png");
                            Common.Sleep(3000);
                            Bitmap bmp = new Bitmap($"Tool\\{randomFile}.png");
                            if (bmp == null)
                            {
                                Common.SetStatus(serial, "Bitmap null");
                            }
                            else
                            {
                                Common.SetStatus(serial, "Generate bitmap succeeded");
                            }
                            Common.Sleep(1000);

                            string response = UploadImage("http://hamai00.tk:9999/obj", bmp);
                            Common.Sleep(2000);

                            Common.SetStatus(serial, response);
                            CaptchaPoint captchaPoint = JsonConvert.DeserializeObject<CaptchaPoint>(response);
                            TapPosition(serial, new Point(Int32.Parse(captchaPoint.x1), Int32.Parse(captchaPoint.y1)));
                            Common.Sleep(1000);
                            TapPosition(serial, new Point(Int32.Parse(captchaPoint.x2), Int32.Parse(captchaPoint.y2)));

                            Common.Sleep(10000);
                            /*
                            if (File.Exists($"Tool\\{randomFile}.png"))
                            {
                                File.Delete($"Tool\\{randomFile}.png");
                            }
                            */

                            continue;
                        }

                        if (ContainsIgnoreCase(TextDump, "Enter 6-digit code"))
                        {
                            Common.SetStatus(serial, "Enter 6-digit code");
                            OpenGetCodeApi(serial);
                            Common.Sleep(1000);
                            DumpUi(serial);

                            TapDynamic(serial, "btnGetOtp");
                            Common.SetStatus(serial, "Tapped button get otp");
                            Common.Sleep(2000);
                            OpenTikTokApp(serial);
                            Common.Sleep(2000);

                            DumpUi(serial);
                            Common.Sleep(1000);
                            Common.SetStatus(serial, "Tapped code text filed");
                            TapDynamic(serial, "edittext");
                            InputClipboard(serial);
                            Common.Sleep(10000);

                            TapPosition(serial, new Point(40, 120)); //nut back
                            Common.Sleep(2000);
                            DumpUi(serial);    
                            Common.Sleep(500);
                            if (ContainsIgnoreCase(TextDump, "Phone number") && ContainsIgnoreCase(TextDump, "edittext"))
                            {
                                phonenumber = GetRandomTiktokNumber();
                                OpenGetCodeApi(serial);
                                Common.Sleep(1000);
                                DumpUi(serial);

                                Common.SetStatus(serial, "Enter phone number api");
                                InputDynamic(serial, "editPhone", phonenumber);
                                Common.Sleep(500);

                                Common.SetStatus(serial, "tappeb btnGetCode");
                                TapDynamic(serial, "btnGetCode");
                                Common.Sleep(2000);
                                OpenTikTokApp(serial);
                                Common.Sleep(1000);
                                continue;
                            } else
                            {
                                return TaskResult.Success;
                            }
                        }
                    }
                }


                if (ContainsIgnoreCase(TextDump, "com.android.launcher3"))
                {
                    Common.SetStatus(serial, "Kickout Home, Reloading");
                    OpenGetCodeApi(serial);
                    Common.Sleep(_rand.Next(3000, 4000));
                    continue;
                }
                Common.Sleep(_rand.Next(3000, 4000));
            }
        }

        public string UploadImage(string url, Bitmap bmp)
        {
            using (WebClient client = new WebClient())
            using (var ms = new MemoryStream())
            {
                bmp.Save(ms, ImageFormat.Png);
                byte[] result = client.UploadData(url, ms.ToArray());
                return Encoding.UTF8.GetString(result);
            }

        }

        public string RandomPasswordString()
        {
            int length = _rand.Next(12, 14);
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[_rand.Next(s.Length)]).ToArray());
        }

        public string GetRandomTiktokNumber()
        {
            try
            {
                string[] info = MyFile.GetLine(filePath: "Data\\tiktok.txt", index: 1, remove: true).Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                return info[0];
            }
            catch { return null; }
        }

        private void TapPosition(string serial, Point point)
        {
            point.X += _rand.Next(5, 20);
            point.Y += _rand.Next(5, 20);
            Adb.Shell(serial, $"input tap {point.X} {point.Y}");
        }

        private void SwipeUp(string serial)
        {
            Adb.Shell(serial, "input swipe 500 1700 50 50");
        }

        private void Enter(string serial)
        {
            Adb.SendKey(serial, "KEYCODE_ENTER");
        }

        private void Back(string serial)
        {
            Adb.SendKey(serial, "KEYCODE_BACK");
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

        private void InputClipboard(string serial, bool isClear = true)
        {
            if (isClear)
            {
                Adb.SendKey(serial, "123");
                Adb.SendKey(serial, "--longpress 67 67 67 67 67 67 67 67 67 67 67");
            }
            Adb.Shell(serial, $"input keyevent 279");
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
            point.X += _rand.Next(5, 15);
            point.Y += _rand.Next(5, 15);
            Adb.Shell(serial, $"input tap {point.X} {point.Y}");
        }

        private void OpenGetCodeApi(string serial)
        {
            Adb.Shell(serial, "am start -n com.example.getcodeapi/.MainActivity");
        }


        private void OpenTikTokApp(string serial)
        {
            Adb.Shell(serial, "am start -n com.ss.android.ugc.trill/com.ss.android.ugc.aweme.splash.SplashActivity");
        }
        private void OpenSnapchatApp(string serial)
        {
            Adb.Shell(serial, "am start -n com.snapchat.android/.LandingPageActivity");
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
