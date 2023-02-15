
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
    class SnapchatTask
    {
        public bool isStopAuto = false;
        public string phonenumber { get; set; }

        private string TextDump;
        private readonly Random _rand = new Random();
        private string[] peopleName = { "Duy", "Phuc", "Quan", "Nghia", "Tham", "Toai", "Trang", "Hoang", "Anh" };
        private string[] peopleFirst = { "Nguyen", "Trinh", "Tran", "Vu" };
        private string[] months = { "thg%s1", "thg%s2", "thg%s3", "thg%s4", "thg%s5", "thg%s6", "thg%s7", "thg%s8", "thg%s9", "thg%s10", "thg%s11", "thg%s12" };
        private string[] years = { "1990", "1991", "1992", "1993", "1994", "1995", "1996", "1997", "1998", "1999", "2000", "2001", "2002", "2003" };

        public TaskResult LoginSnapchat(string serial)
        {
            Adb.SendKey(serial, "KEYCODE_HOME");
            Common.SetStatus(serial, "Open Snapchat");
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

                if (ContainsIgnoreCase(TextDump, "url") && !ContainsIgnoreCase(TextDump, "103.114.107.7"))
                {
                    Common.SetStatus(serial, "Enter url");
                    InputDynamic(serial, "editUrl", "103.114.107.7");
                    Common.Sleep(500);

                    Common.SetStatus(serial, "Enter phone number");
                    InputDynamic(serial, "editPhone", phonenumber);
                    Common.Sleep(500);

                    Common.SetStatus(serial, "Enter Brand");
                    InputDynamic(serial, "editBrand", "sn");
                    Common.Sleep(500);

                    Common.SetStatus(serial, "tappeb btnGetCode");
                    TapDynamic(serial, "btnGetCode");
                    Common.Sleep(3000);
                    OpenSnapchatApp(serial);
                    Common.Sleep(2000);
                    continue;
                }

                if (ContainsIgnoreCase(TextDump, "signup_button_horizontal"))
                {
                    // dang ky
                    Common.SetStatus(serial, "Tapped signup_button_horizontal");
                    TapDynamic(serial, "signup_button_horizontal");
                    Common.Sleep(1000);
                    continue;
                }

                if (ContainsIgnoreCase(TextDump, "permission_message") && ContainsIgnoreCase(TextDump, "permission_allow_button"))
                {
                    // cho phep truy cap
                    // allow tiwce 
                    Common.SetStatus(serial, "Tapped permission_allow_button");
                    TapDynamic(serial, "permission_allow_button");
                    Common.Sleep(1000);
                    TapDynamic(serial, "permission_allow_button");
                    Common.Sleep(1000);
                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "pre_prompt_positive_button"))
                {
                    // cho phep danh ba .
                    Common.SetStatus(serial, "Tapped pre_prompt_positive_button");
                    TapDynamic(serial, "pre_prompt_positive_button");
                    Common.Sleep(1000);
                    continue;
                }

                if (ContainsIgnoreCase(TextDump, "display_name_first_name_field") && ContainsIgnoreCase(TextDump, "display_name_last_name_field"))
                {
                    // field first name
                    Common.SetStatus(serial, "Enter display_name_first_name_field");
                    InputDynamic(serial, "display_name_first_name_field", peopleFirst[_rand.Next(0, peopleFirst.Length)]);
                    Common.Sleep(500);
                    DumpUi(serial);
                    // field last name
                    Common.SetStatus(serial, "Enter display_name_last_name_field");
                    InputDynamic(serial, "display_name_last_name_field", peopleName[_rand.Next(0, peopleName.Length)]);
                    Common.Sleep(500);
                    DumpUi(serial);
                    Common.Sleep(500);
                    if (ContainsIgnoreCase(TextDump, "continue_button"))
                    {
                        Common.SetStatus(serial, "Tapped continue display_name");
                        TapDynamic(serial, "continue_button");
                        Common.Sleep(1000);
                    }
                    continue;
                }


                if (ContainsIgnoreCase(TextDump, "thg 2"))
                {
                    // dien th 3, enter
                    Common.SetStatus(serial, "Enter thg 2");
                    InputDynamic(serial, "thg 2", months[_rand.Next(0, months.Length)]);
                    Common.Sleep(500);
                    DumpUi(serial);
                    Common.Sleep(500);
                    InputDynamic(serial, "2005", years[_rand.Next(0, years.Length)]);
                    Common.Sleep(500);
                    DumpUi(serial);
                    Common.Sleep(500);
                    if (ContainsIgnoreCase(TextDump, "continue_button"))
                    {
                        Common.SetStatus(serial, "Tapped continue calendar");
                        TapDynamic(serial, "continue_button");
                        Common.Sleep(1000);
                    }
                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "username_form_field"))
                {
                    // dien th 3, enter
                    Common.SetStatus(serial, "Enter username_form_field");
                    InputDynamic(serial, "username_form_field", RandomPasswordString());
                    Common.Sleep(4000);
                    DumpUi(serial);
                    Common.Sleep(500);
                    if (ContainsIgnoreCase(TextDump, "continue_button"))
                    {
                        Common.SetStatus(serial, "Tapped continue username");
                        TapDynamic(serial, "continue_button");
                        Common.Sleep(1000);
                    }
                    continue;
                }

                
                if (ContainsIgnoreCase(TextDump, "password_form_field"))
                {
                    Common.SetStatus(serial, "Enter password_form_field");
                    InputDynamic(serial, "password_form_field", RandomPasswordString());
                    Common.Sleep(500);
                    Enter(serial);
                    Common.Sleep(500);
                    DumpUi(serial);
                    Common.Sleep(500);
                    if (ContainsIgnoreCase(TextDump, "continue_button"))
                    {
                        Common.SetStatus(serial, "Tapped continue password");
                        TapDynamic(serial, "continue_button");
                        Common.Sleep(1000);
                    }
                    continue;
                }

                if (ContainsIgnoreCase(TextDump, "signup_with_phone_instead"))
                {
                    // dang ky thay the bang sdt
                    Common.SetStatus(serial, "tapped signup_with_phone_instead");
                    TapDynamic(serial, "signup_with_phone_instead");
                    Common.Sleep(1000);
                    continue;
                }

                if (ContainsIgnoreCase(TextDump, "bottom_phone_form_field"))
                {
                    while (true)
                    {
                        DumpUi(serial);
                        Common.Sleep(1000);

                        if (ContainsIgnoreCase(TextDump, "bottom_phone_form_field"))
                        {
                            Common.SetStatus(serial, "Enter Phone number");
                            InputDynamic(serial, "bottom_phone_form_field", phonenumber);
                            Common.Sleep(3000);
                            DumpUi(serial);
                            Common.Sleep(500);
                            if (ContainsIgnoreCase(TextDump, "continue_button"))
                            {
                                Common.SetStatus(serial, "Tapped continue bottom phone");
                                TapDynamic(serial, "continue_button");
                            }
                            Common.Sleep(5000);
                            continue;
                        }

                        

                        if (ContainsIgnoreCase(TextDump, "code_field"))
                        {
                            Common.SetStatus(serial, "Enter 6-digit code");
                            OpenGetCodeApi(serial);
                            Common.Sleep(1000);
                            DumpUi(serial);

                            TapDynamic(serial, "btnGetOtp");
                            Common.SetStatus(serial, "Tapped button get otp");
                            Common.Sleep(5000);
                            OpenSnapchatApp(serial);
                            Common.Sleep(2000);

                            DumpUi(serial);
                            Common.Sleep(1000);
                            Common.SetStatus(serial, "Tapped code text filed");
                            TapDynamic(serial, "edittext");
                            InputClipboard(serial);
                            Common.Sleep(10000);

                            TapDynamic(serial, "back_button"); //nut back
                            Common.Sleep(2000);
                            DumpUi(serial);
                            Common.Sleep(500);
                            if (ContainsIgnoreCase(TextDump, "bottom_phone_form_field"))
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
                                OpenSnapchatApp(serial);
                                Common.Sleep(1000);
                                continue;
                            }
                            else
                            {
                                return TaskResult.Success;
                            }
                        }
                    }
                }




                if (ContainsIgnoreCase(TextDump, "continue_button"))
                {
                    // tiep tuc
                    Common.SetStatus(serial, "Tapped continue_button");
                    TapDynamic(serial, "continue_button");
                    Common.Sleep(1000);
                    continue;
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
        private void Tab(string serial)
        {
            Adb.SendKey(serial, "KEYCODE_TAB");
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
