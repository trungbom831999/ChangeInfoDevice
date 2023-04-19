
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
    class Xbank : BaseActivity
    {
        public bool isStopAuto = false;

        public TaskResult XbankRegister(string serial)
        {
            string numberphone = GetRandomNumberPhone();
            if (numberphone == null)
            {
                Common.SetStatus(serial, "Call API fail. Out of number");
                return TaskResult.StopAuto;
            }
            Adb.SendKey(serial, "KEYCODE_HOME");
            FillInfoGetCodeAPI(serial, numberphone, EnumBrandApp.xbank);
            OpenApp(serial);
            Common.Sleep(2000);
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

                //Nhấn vào avatar
                if (ContainsIgnoreCase(TextDump, "RNE__Input__text-input") && ContainsIgnoreCase(TextDump, "Search"))
                {
                    TapPosition(serial, new Point(x: 150, y: 200));
                    Common.SetStatus(serial, "Tap avatar");
                    continue;
                }

                //Nhấn vào để đăng ký
                if (ContainsIgnoreCase(TextDump, "Sign in"))
                {
                    TapDynamic(serial, "Sign in");
                    Common.SetStatus(serial, "Tap Sign in");
                    continue;
                }
                else if(ContainsIgnoreCase(TextDump, "Log Out")) //Đăng xuất
                {
                    TapDynamic(serial, "Log Out");
                    Common.SetStatus(serial, "Log out");
                    continue;
                }

                //Số điện thoại
                if (ContainsIgnoreCase(TextDump, "RNE__Input__text-input") && ContainsIgnoreCase(TextDump, "Phone") && ContainsIgnoreCase(TextDump, "Email"))
                {
                    //đổi vùng
                    Common.SetStatus(serial, "Choose location");
                    TapPosition(serial, new Point(x: 200, y: 1200));
                    DumpUi(serial);
                    InputDynamic(serial, "RNE__Input__text-input", "Vie");
                    TapPosition(serial, new Point(x: 500, y: 1150));
                    TapPosition(serial, new Point(x: 500, y: 1150));
                    DumpUi(serial);
                    Common.SetStatus(serial, "Input number phone");
                    InputDynamic(serial, "RNE__Input__text-input", numberphone);
                    DumpUi(serial);
                    TapDynamic(serial, "Continue");
                    DumpUi(serial);
                    TapPosition(serial, new Point(x: 350, y: 2100));
                    TapDynamic(serial, "Continue");
                    Common.Sleep(1000);
                    continue;
                }

                //Nhập mật khẩu
                if (ContainsIgnoreCase(TextDump, "RNE__Input__text-input") && ContainsIgnoreCase(TextDump, "Register"))
                {
                    Common.SetStatus(serial, "Input password");
                    InputDynamic(serial, "index=\"2\"", EnumPassword.passwordDefault);
                    InputDynamic(serial, "index=\"6\"", EnumPassword.passwordDefault);
                    TapDynamic(serial, "Continue");
                    Common.Sleep(3000);
                    continue;
                }

                //Nhập OTP
                if (ContainsIgnoreCase(TextDump, "RNE__Input__text-input") && ContainsIgnoreCase(TextDump, "Verification Code"))
                {
                    //Lấy OTP
                    GetOTP(serial);
                    OpenApp(serial);
                    InputClipboard(serial);
                    Common.Sleep(3500);
                    DumpUi(serial);
                    if (ContainsIgnoreCase(TextDump, "RNE__Input__text-input") && ContainsIgnoreCase(TextDump, "Search"))
                    {
                        SavePhoneSuccess(numberphone);
                        CloseAllApp(serial);
                        return TaskResult.Success;
                    }
                    else if (ContainsIgnoreCase(TextDump, "RNE__Input__text-input") && ContainsIgnoreCase(TextDump, "Verification Code"))
                    {
                        CloseAllApp(serial);
                        return TaskResult.OtpError;
                    }
                        continue;
                }

                    if (ContainsIgnoreCase(TextDump, "abcxyz")){}

            }
        }

        private string GetRandomNumberPhone()
        {
            try
            {
                string[] info = new string[1];
                info = MyFile.GetLine(filePath: "Data\\27-XB1IN.txt", index: 1, remove: true).Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                return info[0];
            }
            catch { return null; }
        }
        private void SavePhoneSuccess(string phone)
        {
            MyFile.WriteAllText("Data\\28-XB1Sucess.txt", phone, true);
        }
        private void OpenApp(string serial)
        {
            OpenApp(serial, "xbank");
        }
        private void CloseAllApp(string serial)
        {
            CloseApp(serial, "xbank");
            CloseApp(serial, "getcodeapi");
        }
    }
}
