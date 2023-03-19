
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Utils;
using WinSubTrial.ApionTask;
using WinSubTrial.Enum;
using WinSubTrial.Utilities;
using static System.Net.Mime.MediaTypeNames;

namespace WinSubTrial
{
    class ChametTask : BaseActivity
    {
        public bool isStopAuto = false;

        public TaskResult ChametRegister(string serial)
        {
            string numberphone = GetRandomChametNumber();
            //string numberphone = "824879499";
            Common.SetStatus(serial, $"Get chamet phonenumber: {numberphone}");
            Adb.SendKey(serial, "KEYCODE_HOME");
            FillInfoGetCodeAPI(serial, numberphone, EnumBrandApp.chamet, "net1");
            OpenApp(serial, "chamet");
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

                //Chấn nhận quyền
                if (ContainsIgnoreCase(TextDump, "permission_allow_foreground_only_button"))
                {
                    TapDynamic(serial, "permission_allow_foreground_only_button");
                    Common.SetStatus(serial, "Tab permission");
                    continue;
                }

                //OK bật vị trí
                if (ContainsIgnoreCase(TextDump, "android:id/button1"))
                {
                    TapDynamic(serial, "android:id/button1");
                    Common.SetStatus(serial, "Tab OK location");
                    continue;
                }

                //Ấn nhiều hơn
                if (ContainsIgnoreCase(TextDump, "tv_more"))
                {
                    TapDynamic(serial, "tv_more");
                    Common.SetStatus(serial, "Tab More");
                    continue;
                }

                //Ấn Số điện thoại
                if (ContainsIgnoreCase(TextDump, "Tho")&& ContainsIgnoreCase(TextDump, "fl_phone_login"))
                {
                    TapDynamic(serial, "Tho");
                    Common.SetStatus(serial, "Tab So Dien Thoai");
                    continue;
                }

                //Nhập sđt
                if (ContainsIgnoreCase(TextDump, "edit_phone_number"))
                {
                    InputDynamic(serial, "edit_phone_number", numberphone);
                    Common.SetStatus(serial, "Input phonenumber");
                    TapDynamic(serial, "tv_next");

                    //Bị chặn ko cho gửi tin nhắn OTP
                    Common.Sleep(500);
                    DumpUi(serial);
                    if (ContainsIgnoreCase(TextDump, "edit_phone_number"))
                    {
                        Common.SetStatus(serial, "Limit OTP message");
                        return TaskResult.StopAuto;
                    }
                    continue;
                }

                //Nhập OTP
                if(ContainsIgnoreCase(TextDump, "tv_verification_tip"))
                {
                    //lấy OTP
                    OpenGetCodeApi(serial);
                    DumpUi(serial);
                    TapDynamic(serial, "btnGetOtp");
                    Common.SetStatus(serial, "Tapped button get otp");

                    //Quay lại điền OTP
                    OpenApp(serial, "chamet");
                    DumpUi(serial);

                    TapDynamic(serial, "EditText");
                    InputClipboard(serial);
                    Common.SetStatus(serial, "Input OTP");
                    TapDynamic(serial, "tv_next");

                    //Check lỗi ko lấy đc OTP
                    Common.Sleep(3000);
                    DumpUi(serial);
                    if (ContainsIgnoreCase(TextDump, "tv_verification_tip"))
                    {
                        CloseAllApp(serial);
                        return TaskResult.OtpError;
                    }
                    continue;
                }

                //Chọn giới tính
                if (ContainsIgnoreCase(TextDump, "tv_gender_hint"))
                {
                    //Random
                    if (int.Parse(numberphone) % 2 == 0)
                    {
                        TapDynamic(serial, "fl_male_select");
                    }
                    else
                    {
                        TapDynamic(serial, "fl_female_select");
                    }
                    TapDynamic(serial, "tv_confirm"); 
                    Common.SetStatus(serial, "Choose gender");
                    continue;
                }

                //Ngày tháng năm sinh
                if (ContainsIgnoreCase(TextDump, "tv_birthday_hint"))
                {
                    TapDynamic(serial, "tv_confirm");
                    Common.SetStatus(serial, "Confirm birthday");
                    continue;
                }

                //Nickname bỏ qua - Success
                if (ContainsIgnoreCase(TextDump, "tv_nick_hint"))
                {
                    TapDynamic(serial, "tv_skip_hint");
                    Common.SetStatus(serial, "Skip nick name");
                    SavePhoneSuccess(numberphone);
                    CloseAllApp(serial);
                    return TaskResult.Success;
                }
                
                //if (ContainsIgnoreCase(TextDump, "abcxyz"))
                //{

                //}
            }
        }

        private void CloseAllApp(string serial)
        {
            CloseApp(serial, "chamet");
            CloseApp(serial, "getcodeapi");
        }

        public string GetRandomChametNumber()
        {
            try
            {
                string[] info = new string[1];
                info = MyFile.GetLine(filePath: "Data\\15-CH1IN.txt", index: 1, remove: true).Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                return info[0];
            }
            catch { return null; }
        }
        private void SavePhoneSuccess(string phone)
        {
            File.AppendAllText("Data\\16-CH1Sucess.txt", phone + "\n");
        }

    }
}
