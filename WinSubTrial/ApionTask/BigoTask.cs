
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
using WinSubTrial.Utilities;

namespace WinSubTrial
{
    class BigoTask : BaseActivity
    {
        public bool isStopAuto = false;
        public string phonenumber { get; set; }

        public TaskResult BigoAutoRegister(string serial)
        {
            Adb.SendKey(serial, "KEYCODE_HOME");
            Common.SetStatus(serial, "Open get code API");
            OpenGetCodeApi(serial);
            //OpenBigoApp(serial);
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
                
                //Nhập lấy Code của Get Code Api
                if (ContainsIgnoreCase(TextDump, "url") && !ContainsIgnoreCase(TextDump, "103.114.107.7"))
                {
                    InputDynamic(serial, "editUrl", "103.114.107.7");
                    Common.SetStatus(serial, "Enter url");
                    //Common.Sleep(500);

                    InputDynamic(serial, "editPhone", phonenumber);
                    Common.SetStatus(serial, "Enter phone number");
                    //Common.Sleep(500);

                    InputDynamic(serial, "editBrand", "BI");
                    Common.SetStatus(serial, "Enter Brand");
                    //Common.Sleep(500);

                    TapDynamic(serial, "btnGetCode");
                    Common.SetStatus(serial, "tappeb btnGetCode");
                    OpenBigoApp(serial);
                    Common.Sleep(2000);
                    continue;
                }

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
                    InputDynamic(serial, "et_phone", phonenumber);
                    Common.SetStatus(serial, "Input phone number");
                    DumpUi(serial);
                    TapDynamic(serial, "tv_sign_or_login");
                    Common.SetStatus(serial, "tappeb skip");
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
                    Common.SetStatus(serial, "tappeb send oin code");
                    Common.Sleep(3000);
                    //lấy otp 1 lần ko đc thì làm lại từ đầu
                    int countgetdOTP = 1;
                    while (countgetdOTP > 0)
                    {
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
                        Common.Sleep(3000);
                        DumpUi(serial);

                        //Sang trang chủ là thành công
                        if (ContainsIgnoreCase(TextDump, "viewpager_btn_carousel"))
                        {
                            CloseAllApp(serial);
                            return TaskResult.Success;
                        }
                        countgetdOTP--;
                        if (countgetdOTP <= 0)
                        {
                            CloseAllApp(serial);
                            return TaskResult.OtpError;
                        }
                        continue;
                    }
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
                    if (ContainsIgnoreCase(TextDump, "nhá"))
                    {
                        TapDynamic(serial, "nhá");
                         Common.SetStatus(serial, "Tapped login in tab personal");
                    }
                        continue;
                }


                //if (ContainsIgnoreCase(TextDump, "abcxyz"))
                //{

                //}
                
            }
        }

        private void CloseAllApp(string serial)
        {
            CloseApp(serial, "bigo");
            CloseApp(serial, "getcodeapi");
        }
    }
}
