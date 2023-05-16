
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
    class SnapchatLoginBackup : BaseActivity
    {
        public bool isStopAuto = false;

        public TaskResult LoginBackup(string serial, Device device)
        {
            string numberphone = GetRandomNumberPhone();
            Adb.SendKey(serial, "KEYCODE_HOME");
            //FillInfoGetCodeAPI(serial, numberphone, EnumBrandApp.snapchat);
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
                
                //Ấn đăng nhập
                if (ContainsIgnoreCase(TextDump, "login_button_horizontal"))
                {
                    TapDynamic(serial, "login_button_horizontal");
                    Common.SetStatus(serial, "Tapped login");
                    continue;
                }
                
                //Dùng số điện thoại
                if (ContainsIgnoreCase(TextDump, "use_phone_instead"))
                {
                    TapDynamic(serial, "use_phone_instead");
                    Common.SetStatus(serial, "Tapped use phone");
                    continue;
                }

                //Cho phép quyền
                if (ContainsIgnoreCase(TextDump, "permission_message") && ContainsIgnoreCase(TextDump, "permission_allow_button"))
                {
                    TapDynamic(serial, "permission_allow_button");
                    Common.SetStatus(serial, "Tapped permission_allow_button");
                    continue;
                }

                //Đăng nhập
                if (ContainsIgnoreCase(TextDump, "phone_form_field") && ContainsIgnoreCase(TextDump, "password_field"))
                {
                    if(ContainsIgnoreCase(TextDump, "login_error_message"))
                    {
                        return TaskResult.Failure;
                    }
                    Common.SetStatus(serial, "Login");
                    InputDynamic(serial, "phone_form_field", numberphone);
                    InputDynamic(serial, "password_field", EnumPassword.passwordDefault);
                    if (ContainsIgnoreCase(TextDump, "one_tap_login_opt_in_checkmark"))
                    {
                        TapDynamic(serial, "one_tap_login_opt_in_checkmark");
                    }
                    DumpUi(serial);
                    TapDynamic(serial, "button_text");
                    Common.SetStatus(serial, "Waiting login");
                    Common.Sleep(12000);
                    continue;
                }

                //Đăng nhập đáng ngờ
                if (ContainsIgnoreCase(TextDump, "odlv_landing_page_title"))
                {
                    FillInfoGetCodeAPI(serial, numberphone, EnumBrandApp.snapchat);
                    OpenApp(serial);
                    DumpUi(serial);
                    TapDynamic(serial, "button_text");
                    Common.SetStatus(serial, "SMS xac minh");
                    Common.Sleep(7000);
                    continue;
                }

                //Xác thực OTP
                if (ContainsIgnoreCase(TextDump, "odlv_verifying_page_title"))
                {
                    GetOTP(serial);
                    OpenApp(serial);
                    DumpUi(serial);
                    TapDynamic(serial, "odlv_verifying_code_field");
                    InputClipboard(serial);
                    DumpUi(serial);
                    TapDynamic(serial, "button_text");
                    Common.Sleep(6000);
                    DumpUi(serial);
                    if (ContainsIgnoreCase(TextDump, "odlv_verifying_page_title"))
                    {
                        return TaskResult.OtpError;
                    }
                    continue;
                }
                
                //Vào màn hình đăng nhập => Backup
                if (ContainsIgnoreCase(TextDump, "camera_and_storage_permission_text"))
                {
                    string saveDir = (string)Common.GlobalSettings["folderBackup"];
                    string bk_packages = Common.Settings.AppBackup;
                    Common.SetStatus(serial, "Backuping...");
                    var resultBackup = new Functions.Backup { device = device }.Save(saveDir, bk_packages);
                    Common.SetStatus(serial, "Backup " + (resultBackup ? "done" : "fail"));
                    return resultBackup ? TaskResult.Success : TaskResult.Failure;                                
                }

                //if (ContainsIgnoreCase(TextDump, "abcxyz")) { }
            }
        }

        private void OpenApp(string serial)
        {
            OpenApp(serial, "snapchat");
        }

        private string GetRandomNumberPhone()
        {
            try
            {
                string[] info = new string[1];
                info = MyFile.GetLine(filePath: "Data\\35-SN1IN.txt", index: 1, remove: true).Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                return info[0];
            }
            catch { return null; }
        }
    }
}
