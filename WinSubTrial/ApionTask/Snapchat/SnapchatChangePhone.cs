
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Utils;
using WinSubTrial.ApionTask;
using WinSubTrial.Enum;
using WinSubTrial.Utilities;

namespace WinSubTrial
{
    class SnapchatChangePhone : BaseActivity
    {
        public bool isStopAuto = false;

        public TaskResult ChangePhone(string serial, Device device)
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
                //DateTime currentTime = DateTime.Now;
                //if ((currentTime - startTime).TotalSeconds > 600)
                //{
                //    Common.SetStatus(serial, "Timeout, timed out");
                //    return TaskResult.Failure;
                //}

                DumpUi(serial);

                //Vào màn hình đăng nhập
                if (ContainsIgnoreCase(TextDump, "camera_and_storage_permission_text"))
                {
                    TapDynamic(serial, "turn_on_button");
                    Common.SetStatus(serial, "Turn on camera");
                    continue;
                }

                //Cho phép quyền
                if (ContainsIgnoreCase(TextDump, "permission_message") && ContainsIgnoreCase(TextDump, "permission_allow_button"))
                {
                    TapDynamic(serial, "permission_allow_button");
                    Common.SetStatus(serial, "Tapped permission_allow_button");
                    continue;
                }

                //Truy cập danh bạ
                if (TextDump == "")
                {
                    //Bấm ko cho phép truy cập danh bạ
                    TapPosition(serial, new Point(x: 650, y: 1900));
                    Common.Sleep(500);
                    //Bấm avatar
                    TapPosition(serial, new Point(x: 50, y: 100));
                    Common.Sleep(2000);
                    Common.SetStatus(serial, "Tap icon avatar 1");
                    continue;
                }

                //Ấn icon avatar
                if (ContainsIgnoreCase(TextDump, "neon_header_avatar_container"))
                {
                    //Bấm ko cho phép truy cập danh bạ
                    TapPosition(serial, new Point(x: 650, y: 1900));
                    TapDynamic(serial, "neon_header_avatar_container");
                    Common.SetStatus(serial, "Tap icon avatar 2");
                    Common.Sleep(3000);
                    TapPosition(serial, new Point(x: 1250, y: 100));
                    Common.SetStatus(serial, "Tap setting");
                    continue;
                }

                //Ấn số điện thoại
                if (ContainsIgnoreCase(TextDump, "settings_item_header")&& ContainsIgnoreCase(TextDump, "settings_item_text"))
                {
                    TapPosition(serial, new Point(x: 1000, y: 900));
                    Common.SetStatus(serial, "Tap phone number");
                    continue;
                }


                //Điền sđt
                if (ContainsIgnoreCase(TextDump, "phone_form_field"))
                {
                    Common.SetStatus(serial, "Change phone");
                    InputDynamic(serial, "phone_form_field", numberphone);
                    FillInfoGetCodeAPI(serial, numberphone, EnumBrandApp.snapchat);
                    OpenApp(serial);
                    DumpUi(serial);
                    TapDynamic(serial, "button_text");
                    Common.Sleep(500);
                    TapPosition(serial, new Point(x: 800, y: 1200));
                    Common.Sleep(5000);
                    GetOTP(serial);
                    OpenApp(serial);
                    DumpUi(serial);
                    TapDynamic(serial, "verify_code");
                    InputClipboard(serial);
                    Common.Sleep(3000);
                    DumpUi(serial);
                    if (ContainsIgnoreCase(TextDump, "code_request_err_text") && !ContainsIgnoreCase(TextDump, "verify_code"))
                    {
                        SavePhoneSuccess(numberphone);
                        return TaskResult.Success;
                    }
                    else if(ContainsIgnoreCase(TextDump, "verify_code"))
                    {
                        return TaskResult.OtpError;
                    }
                    continue;
                }

                //Điền password
                if (ContainsIgnoreCase(TextDump, "password_validation_password_field"))
                {
                    Common.SetStatus(serial, "Input password");
                    InputDynamic(serial, "password_validation_password_field", EnumPassword.passwordDefault);
                    DumpUi(serial);
                    TapDynamic(serial, "password_validation_continue_button");
                    Common.Sleep(3000);
                    DumpUi(serial);
                    if (ContainsIgnoreCase(TextDump, "phone_form_field") && ContainsIgnoreCase(TextDump, "button_text"))
                    {
                        SavePhoneSuccess(numberphone);
                        return TaskResult.Success;
                    }
                    continue;
                }

                //Cài đặt
                //Vì ko có id nên để mẹo xuống dưới cùng
                if (ContainsIgnoreCase(TextDump, "index=\"1\"")
                    && ContainsIgnoreCase(TextDump, "index=\"2\"")
                    && ContainsIgnoreCase(TextDump, "index=\"3\""))
                {
                    TapDynamic(serial, "index=\"2\"");
                    Common.SetStatus(serial, "Tap settings");
                    continue;
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
                info = MyFile.GetLine(filePath: "Data\\36-SN1IN.txt", index: 1, remove: true).Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                return info[0];
            }
            catch { return null; }
        }
        private void SavePhoneSuccess(string numberphone)
        {
            MyFile.WriteAllText("Data\\36-SN1INSucess.txt", numberphone, true);
        }
    }
}
