
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
using WinSubTrial.ApionTask;
using WinSubTrial.Enum;
using WinSubTrial.Utilities;

namespace WinSubTrial
{
    class SnapchatTask : BaseActivity
    {
        public bool isStopAuto = false;
        public string phonenumber { get; set; }

        private string[] peopleName = { "Duy", "Phuc", "Quan", "Nghia", "Tham", "Toai", "Trang", "Hoang", "Anh" };
        private string[] peopleFirst = { "Nguyen", "Trinh", "Tran", "Vu" };
        private string[] months = { "thg%s1", "thg%s2", "thg%s3", "thg%s4", "thg%s5", "thg%s6", "thg%s7", "thg%s8", "thg%s9", "thg%s10", "thg%s11", "thg%s12" };
        private string[] years = { "1990", "1991", "1992", "1993", "1994", "1995", "1996", "1997", "1998", "1999", "2000", "2001", "2002", "2003" };

        public TaskResult LoginSnapchat(string serial)
        {
            Adb.SendKey(serial, "KEYCODE_HOME");
            FillInfoGetCodeAPI(serial, phonenumber, EnumBrandApp.snapchat);
            Common.SetStatus(serial, "Open Snapchat");
            OpenApp(serial);
            DateTime startTime = DateTime.Now;
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
                
                //Nhấn đăng ký
                if (ContainsIgnoreCase(TextDump, "signup_button_horizontal"))
                {
                    // dang ky
                    TapDynamic(serial, "signup_button_horizontal");
                    Common.SetStatus(serial, "Tapped signup_button_horizontal");
                    Common.Sleep(200);
                    continue;
                }

                //Cho phép quyền
                if (ContainsIgnoreCase(TextDump, "permission_message") && ContainsIgnoreCase(TextDump, "permission_allow_button"))
                {
                    // cho phep truy cap
                    // allow tiwce 
                    TapDynamic(serial, "permission_allow_button");
                    Common.SetStatus(serial, "Tapped permission_allow_button");
                    //Common.Sleep(1000);
                    TapDynamic(serial, "permission_allow_button");
                    //Common.Sleep(200);
                    continue;
                }

                //Đồng bộ danh bạ
                if (ContainsIgnoreCase(TextDump, "pre_prompt_positive_button"))
                {
                    TapDynamic(serial, "pre_prompt_positive_button");
                    Common.SetStatus(serial, "Tapped pre_prompt_positive_button");
                    //Common.Sleep(200);
                    continue;
                }

                //Điền họ tên
                if (ContainsIgnoreCase(TextDump, "display_name_first_name_field") && ContainsIgnoreCase(TextDump, "display_name_last_name_field"))
                {
                    // field first name
                    InputDynamic(serial, "display_name_first_name_field", peopleFirst[_rand.Next(0, peopleFirst.Length)]);
                    Common.SetStatus(serial, "Enter display_name_first_name_field");
                    //Common.Sleep(500);
                    DumpUi(serial);
                    // field last name
                    InputDynamic(serial, "display_name_last_name_field", peopleName[_rand.Next(0, peopleName.Length)]);
                    Common.SetStatus(serial, "Enter display_name_last_name_field");
                    //Common.Sleep(500);
                    //DumpUi(serial);
                    //Common.Sleep(500);
                    if (ContainsIgnoreCase(TextDump, "continue_button"))
                    {
                        TapDynamic(serial, "continue_button");
                        Common.SetStatus(serial, "Tapped continue display_name");
                        //Common.Sleep(1000);
                    }
                    continue;
                }

                //Ngày tháng năm sinh
                if (ContainsIgnoreCase(TextDump, "thg"))
                {
                    InputDynamic(serial, "thg", months[_rand.Next(0, months.Length)]);
                    Common.SetStatus(serial, "Enter birthday");
                    //Common.Sleep(500);
                    DumpUi(serial);
                    //Common.Sleep(500);
                    //InputDynamic(serial, "2005", years[_rand.Next(0, years.Length)]);
                    //Common.Sleep(500);
                    //DumpUi(serial);
                    if (ContainsIgnoreCase(TextDump, "continue_button"))
                    {
                        Common.SetStatus(serial, "Tapped continue calendar");
                        TapDynamic(serial, "continue_button");
                        Common.Sleep(200);
                    }
                    continue;
                }
                //Tên đăng nhập
                if (ContainsIgnoreCase(TextDump, "username_form_field"))
                {
                    InputDynamic(serial, "username_form_field", RandomPasswordString());
                    Common.SetStatus(serial, "Enter username_form_field");
                    //Common.Sleep(4000);
                    //DumpUi(serial);
                    //Common.Sleep(500);
                    if (ContainsIgnoreCase(TextDump, "continue_button"))
                    {
                        Common.SetStatus(serial, "Tapped continue username");
                        TapDynamic(serial, "continue_button");
                        Common.Sleep(200);
                    }
                    continue;
                }

                //Mật khẩu
                if (ContainsIgnoreCase(TextDump, "password_form_field"))
                {
                    InputDynamic(serial, "password_form_field", RandomPasswordString());
                    Common.SetStatus(serial, "Enter password_form_field");
                    //Common.Sleep(500);
                    Enter(serial);
                    //Common.Sleep(500);
                    //DumpUi(serial);
                    //Common.Sleep(500);
                    if (ContainsIgnoreCase(TextDump, "continue_button"))
                    {
                        TapDynamic(serial, "continue_button");
                        Common.SetStatus(serial, "Tapped continue password");
                        Common.Sleep(1000);
                    }
                    DumpUi(serial);
                    Common.Sleep(500);
                    if (ContainsIgnoreCase(TextDump, "password_error_message"))
                    {
                        return TaskResult.ProxyError;
                    }
                    else
                    {
                        continue;
                    }
                }

                // Đăng ký thay thế bằng sđt
                if (ContainsIgnoreCase(TextDump, "signup_with_phone_instead"))
                {
                    TapDynamic(serial, "signup_with_phone_instead");
                    Common.SetStatus(serial, "tapped signup_with_phone_instead");
                    Common.Sleep(200);
                    continue;
                }

                if (ContainsIgnoreCase(TextDump, "bottom_phone_form_field"))
                {
                    while (true)
                    {
                        DumpUi(serial);
                        //Common.Sleep(1000);

                        if (ContainsIgnoreCase(TextDump, "bottom_phone_form_field"))
                        {
                            if (ContainsIgnoreCase(TextDump, "phone_error_message"))
                            {
                                return TaskResult.OtpError;
                            }
                            InputDynamic(serial, "bottom_phone_form_field", phonenumber);
                            Common.SetStatus(serial, "Enter Phone number");
                            //Common.Sleep(3000);
                            DumpUi(serial);
                            //Common.Sleep(500);
                            if (ContainsIgnoreCase(TextDump, "continue_button"))
                            {
                                Common.SetStatus(serial, "Tapped continue bottom phone");
                                TapDynamic(serial, "continue_button");
                            }
                            Common.Sleep(1500);
                            continue;
                        }

                        //Điền mã xác nhận
                        if (ContainsIgnoreCase(TextDump, "code_field"))
                        {
                            GetOTP(serial);
                            //Common.Sleep(5000);
                            OpenApp(serial);
                            //Common.Sleep(2000);

                            DumpUi(serial);
                            //Common.Sleep(1000);
                            TapDynamic(serial, "edittext");
                            InputClipboard(serial);
                            Common.SetStatus(serial, "Tapped code text filed");
                            Common.Sleep(6000);

                            //Xử lý khi ko lấy đc OTP
                            TapDynamic(serial, "back_button"); //nut back
                            Common.Sleep(2000);
                            DumpUi(serial);
                            Common.Sleep(500);
                            if (ContainsIgnoreCase(TextDump, "bottom_phone_form_field"))
                            {
                                phonenumber = GetRandomSnapchatNumber();
                                OpenApp(serial, "getcodeapi");
                                Common.Sleep(1000);
                                DumpUi(serial);

                                Common.SetStatus(serial, "Enter phone number api");
                                InputDynamic(serial, "editPhone", phonenumber);
                                Common.Sleep(500);

                                Common.SetStatus(serial, "tappeb btnGetCode");
                                TapDynamic(serial, "btnGetCode");
                                Common.Sleep(2000);
                                OpenApp(serial);
                                Common.Sleep(1000);
                                continue;
                            }
                            else
                            {
                                MyFile.WriteAllText("Data\\02-SN2Sucess.txt", phonenumber, true);
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
                    Common.Sleep(200);
                    continue;
                }

                if (ContainsIgnoreCase(TextDump, "com.android.launcher3"))
                {
                    Common.SetStatus(serial, "Kickout Home, Reloading");
                    OpenApp(serial, "getcodeapi");
                    Common.Sleep(_rand.Next(3000, 4000));
                    continue;
                }
                Common.Sleep(_rand.Next(3000, 4000));
            }
        }

        private void OpenApp(string serial)
        {
            OpenApp(serial, "snapchat");
        }

        public string GetRandomSnapchatNumber()
        {
            try
            {
                string[] info = MyFile.GetLine(filePath: "Data\\01-SN1IN.txt", index: 1, remove: true).Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                return info[0];
            }
            catch { return null; }
        }
    }
}
