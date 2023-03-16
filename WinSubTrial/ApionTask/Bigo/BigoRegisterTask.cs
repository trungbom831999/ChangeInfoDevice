
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
    class BigoRegisterTask : BaseActivity
    {
        public bool isStopAuto = false;

        public TaskResult BigoAutoRegister(string serial)
        {
            string numberphone = GetRandomBigoNumber();
            //string numberphone = "357090751";
            if (numberphone == null)
            {
                Common.SetStatus(serial, "Call API fail. Out of number");
                return TaskResult.StopAuto;
            }
            Adb.SendKey(serial, "KEYCODE_HOME");
            Common.SetStatus(serial, "Open get code API");
            FillInfoGetCodeAPI(serial, numberphone, EnumBrandApp.bigo, "net1");
            OpenBigoApp(serial);
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

                //Truy cập vị trí
                if (ContainsIgnoreCase(TextDump, "location_permission_guide_btn"))
                {
                    TapDynamic(serial, "location_permission_guide_btn");
                    Common.SetStatus(serial, "tappeb location");
                    continue;
                }
                if (ContainsIgnoreCase(TextDump, "permission_allow_foreground_only_button"))
                {
                    TapDynamic(serial, "permission_allow_foreground_only_button");
                    Common.SetStatus(serial, "tappeb permission location");
                    continue;
                }

                //Bỏ qua màn đầu tiên
                if (ContainsIgnoreCase(TextDump, "race_info_skip_btn"))
                {
                    TapDynamic(serial, "race_info_skip_btn");
                    Common.SetStatus(serial, "tappeb skip");
                    continue;
                }

                //Màn điền sđt đầu tiên
                if (ContainsIgnoreCase(TextDump, "et_phone") && ContainsIgnoreCase(TextDump, "tv_sign_or_login"))
                {
                    InputDynamic(serial, "et_phone", numberphone);
                    Common.SetStatus(serial, "Input phone number");
                    DumpUi(serial);
                    TapDynamic(serial, "tv_sign_or_login");
                    Common.SetStatus(serial, "tappeb skip");
                    Common.Sleep(2000);
                    DumpUi(serial);
                    //Vào màn đăng nhập thì thành công->Lưu lại
                    if (ContainsIgnoreCase(TextDump, "tv_use_pin_code_login"))
                    {
                        SavePhoneNumberExist(numberphone);
                        return TaskResult.Success;
                    }
                    //Chuyển sang đăng nhập
                    else if (ContainsIgnoreCase(TextDump, "action_login"))
                    {
                        TapDynamic(serial, "action_login");
                        Common.SetStatus(serial, "Switch Login");
                    }
                    continue;
                }

                //Ấn xác nhận
                if (ContainsIgnoreCase(TextDump, "android:id/button1"))
                {
                    TapDynamic(serial, "android:id/button1");
                    Common.Sleep(1000);
                    DumpUi(serial);
                    if (ContainsIgnoreCase(TextDump, "action_login"))
                    {
                        TapDynamic(serial, "action_login");
                        Common.SetStatus(serial, "Switch Login");
                    }
                    continue;
                }

                //Ấn đăng nhập qua mã xác minh
                if (ContainsIgnoreCase(TextDump, "tv_use_pin_code_login"))
                {
                    TapDynamic(serial, "tv_use_pin_code_login");
                    Common.SetStatus(serial, "tappeb login by pin code");
                    continue;
                }

                //Gửi mã xác minh
                if (ContainsIgnoreCase(TextDump, "btn_resend") && ContainsIgnoreCase(TextDump, "ll_phone_resend"))
                {
                    TapDynamic(serial, "btn_resend");
                    Common.SetStatus(serial, "tappeb send pin code");
                    Common.Sleep(2000);
                    DumpUi(serial);
                    if (ContainsIgnoreCase(TextDump, "android:id/button1"))
                    {
                        CloseAllApp(serial);
                        return TaskResult.OtpError;
                    }

                    //lấy OTP
                    OpenGetCodeApi(serial);
                    Common.SetStatus(serial, "Enter 6-digit code");
                    DumpUi(serial);
                    TapDynamic(serial, "btnGetOtp");
                    Common.SetStatus(serial, "Tapped button get otp");
                    //Quay lại điền
                    OpenBigoApp(serial);
                    DumpUi(serial);
                    TapDynamic(serial, "et_pin");
                    InputClipboard(serial);
                    Common.SetStatus(serial, "Input OTP");
                    TapDynamic(serial, "tv_next");
                    Common.Sleep(5000);
                    DumpUi(serial);
                    if ((ContainsIgnoreCase(TextDump, "btn_resend") && ContainsIgnoreCase(TextDump, "ll_phone_resend"))
                        || ContainsIgnoreCase(TextDump, "android:id/button1"))
                    {
                        CloseAllApp(serial);
                        return TaskResult.OtpError;
                    }
                    continue;
                }

                //Trang chủ 
                if (ContainsIgnoreCase(TextDump, "tab_text_view"))
                {
                    if (ContainsIgnoreCase(TextDump, "tab_personal_text"))
                    {
                        TapDynamic(serial, "tab_personal_text");
                        Common.SetStatus(serial, "Tapped personal");
                    }
                    Common.Sleep(2000);
                    DumpUi(serial);
                    if(ContainsIgnoreCase(TextDump, "iv_setting"))
                    {
                        TapDynamic(serial, "iv_setting");
                        continue;
                    }
                    continue;
                }

                //Đăng xuất
                if (ContainsIgnoreCase(TextDump, "btn_logout"))
                {
                    TapDynamic(serial, "btn_logout");
                    Common.Sleep(1000);
                    DumpUi(serial);
                    TapDynamic(serial, "common_btn_text");
                    Common.SetStatus(serial, "Logout");
                    continue;
                }

                if (ContainsIgnoreCase(TextDump, "et_password"))
                {
                    InputDynamic(serial, "et_password", EnumPassword.bigoRegister);
                    Common.SetStatus(serial, "Input password");
                    DumpUi(serial);
                    TapDynamic(serial, "tv_next");
                    Common.Sleep(3000);
                    continue;
                }

                //đăng ký thành công
                if(ContainsIgnoreCase(TextDump, "view_pager"))
                {
                    SavePhoneNumberRegisterSuccess(numberphone);
                    CloseAllApp(serial);
                    return TaskResult.Success;
                }


                if (ContainsIgnoreCase(TextDump, "abcxyz"))
                {

                }

            }
        }
        
        public string GetRandomBigoNumber()
        {
            try
            {
                string[] info = MyFile.GetLine(filePath: "Data\\08-B1IN.txt", index: 1, remove: true).Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                return info[0];
            }
            catch { return null; }
        }
        private void SavePhoneNumberExist(string numberphone)
        {
            File.AppendAllText("Data\\07-B1Sucess.txt", numberphone + "\n");
        }
        private void SavePhoneNumberRegisterSuccess(string numberphone)
        {
            File.AppendAllText("Data\\09-B1NewSucess.txt", numberphone + "\n");
        }
        private void CloseAllApp(string serial)
        {
            CloseApp(serial, "bigo");
            CloseApp(serial, "getcodeapi");
        }
    }
}
