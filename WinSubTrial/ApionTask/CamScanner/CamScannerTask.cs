
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
    class CamScannerTask : BaseActivity
    {
        public bool isStopAuto = false;

        public TaskResult CamScannerRegister(string serial)
        {
            string numberphone = GetRandomNumberPhone();
            Common.SetStatus(serial, $"Get CamScanner phonenumber: {numberphone}");
            Adb.SendKey(serial, "KEYCODE_HOME");
            FillInfoGetCodeAPI(serial, numberphone, EnumBrandApp.camscanner, "net1");
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
                //Lướt màn hình ban đầu
                if (ContainsIgnoreCase(TextDump, "vp_guide_gp_pages"))
                {
                    Common.SetStatus(serial, "Swipe left");
                    for(int i = 0; i<3; i++)
                    {
                        SwipeLeft(serial);
                        Common.Sleep(500);
                    }
                    Point pointButtonX = new Point(x: 1400,y: 0);
                    TapPosition(serial, pointButtonX);
                    continue;
                }

                //Sử dụng ngay
                if (ContainsIgnoreCase(TextDump, "tv_guide_gp_register"))
                {
                    TapDynamic(serial, "tv_guide_gp_register");
                    Common.SetStatus(serial, "Tap use now");
                    continue;
                }

                //Quyền truy cập vị trí
                if (ContainsIgnoreCase(TextDump, "permission_allow_button"))
                {
                    TapDynamic(serial, "permission_allow_button");
                    Common.SetStatus(serial, "Tap allow permission");
                    continue;
                }

                //Bấm X
                if (ContainsIgnoreCase(TextDump, "ll_close")&& ContainsIgnoreCase(TextDump, "tv_guide_tips"))
                {
                    TapDynamic(serial, "ll_close");
                    Common.SetStatus(serial, "Tap button X");
                    continue;
                }

                //Tab Tôi khi đang ở home
                if (ContainsIgnoreCase(TextDump, "main_bottom_tab_text") && ContainsIgnoreCase(TextDump, "main_home_top_search"))
                {
                    Point pointTabMe = new Point(x: 1250, y: 2500);
                    TapPosition(serial, pointTabMe);
                    Common.SetStatus(serial, "Tap tab me");
                    continue;
                }

                //Đăng nhập/Đăng ký
                if (ContainsIgnoreCase(TextDump, "tv_item_me_page_header_account"))
                {
                    TapDynamic(serial, "tv_item_me_page_header_account");
                    Common.SetStatus(serial, "Register");
                    continue;
                }

                //Điền sđt
                if (ContainsIgnoreCase(TextDump, "tv_login_main_account"))
                {
                    InputDynamic(serial, "tv_login_main_account", numberphone);
                    Common.SetStatus(serial, "Input phonenumber");
                    TapDynamic(serial, "btn_login_main_next");
                    continue;
                }
                //Bấm tiếp theo lần sau
                if (ContainsIgnoreCase(TextDump, "btn_area_code_confirm_next"))
                {
                    TapDynamic(serial, "btn_area_code_confirm_next");
                    continue;
                }

                //OTP
                if (ContainsIgnoreCase(TextDump, "tv_verify_code_register_account"))
                {
                    //lấy OTP
                    GetOTP(serial);

                    //Quay lại điền OTP
                    OpenApp(serial);
                    DumpUi(serial);
                    InputClipboard(serial);
                    Common.SetStatus(serial, "Input OTP");

                    //Check lỗi ko lấy đc OTP
                    Common.Sleep(5000);
                    DumpUi(serial);
                    if (ContainsIgnoreCase(TextDump, "tv_verify_code_register_account"))
                    {
                        CloseAllApp(serial);
                        return TaskResult.OtpError;
                    }
                    continue;
                }

                //Thành công
                if (ContainsIgnoreCase(TextDump, "et_setting_pwd_password"))
                {
                    InputDynamic(serial, "et_setting_pwd_password", EnumPassword.passwordDefault);
                    Common.SetStatus(serial, "Input password, save numberphone.");
                    TapDynamic(serial, "btn_setting_pwd_start");
                    Common.Sleep(2000);
                    SavePhoneSuccess(numberphone);
                    CloseAllApp(serial);
                    return TaskResult.Success;
                }

                //Ấn nút x, tắt tùy chọn khác
                if (ContainsIgnoreCase(TextDump, "iv_cancel"))
                {
                    TapDynamic(serial, "iv_cancel");
                    continue;
                }

                //if (ContainsIgnoreCase(TextDump, "abcxyz")){}
            }
        }

        private void CloseAllApp(string serial)
        {
            CloseApp(serial, "camscanner");
            CloseApp(serial, "getcodeapi");
        }

        private void OpenApp(string serial)
        {
            OpenApp(serial, "camscanner");
        }

        public string GetRandomNumberPhone()
        {
            try
            {
                string[] info = new string[1];
                info = MyFile.GetLine(filePath: "Data\\10-CS1IN.txt", index: 1, remove: true).Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                return info[0];
            }
            catch { return null; }
        }
        private void SavePhoneSuccess(string phone)
        {
            File.AppendAllText("Data\\11-CS1Success.txt", phone + "\n");
        }

    }
}
