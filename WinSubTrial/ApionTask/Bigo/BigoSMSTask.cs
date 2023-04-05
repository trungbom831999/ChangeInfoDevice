
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
    class BigoSMSTask : BaseActivity
    {
        public bool isStopAuto = false;

        public TaskResult BigoSMSAuto(string serial)
        {
            string numberphone = GetRandomBigoNumber();
            if (numberphone == null)
            {
                Common.SetStatus(serial, "Call API fail. Out of number");
                return TaskResult.StopAuto;
            }
            Adb.SendKey(serial, "KEYCODE_HOME");
            Common.SetStatus(serial, "Open get code API");
            FillInfoGetCodeAPI(serial, numberphone, EnumBrandApp.bigo, "net1");
            OpenBigoApp(serial);
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
                        GetOTP(serial);
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
        public string GetRandomBigoNumber()
        {
            try
            {
                string[] info = MyFile.GetLine(filePath: "Data\\05-B1LoginSMS.txt", index: 1, remove: true).Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                return info[0];
            }
            catch { return null; }
        }
        private void CloseAllApp(string serial)
        {
            CloseApp(serial, "bigo");
            CloseApp(serial, "getcodeapi");
        }
    }
}
