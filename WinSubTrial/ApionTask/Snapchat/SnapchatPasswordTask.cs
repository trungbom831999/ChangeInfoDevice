
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
    class SnapchatPasswordTask : BaseActivity
    {
        public bool isStopAuto = false;

        public TaskResult SnapchatPasswordRetrieval(string serial, string net)
        {
            string numberphone = GetRandomSnapchatPasswordNumber(net); 
            Common.SetStatus(serial, $"Get snapchat phonenumber: {numberphone}");
            Adb.SendKey(serial, "KEYCODE_HOME");
            Common.SetStatus(serial, "Open get code API");
            FillInfoGetCodeAPI(serial, numberphone, EnumBrandApp.snapchat, net, net == "net2" ? "63" : "");
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

                //Lỗi sđt quá nhiều
                //Để lỗi lên đầu để nếu có lỗi sẽ Reboot luôn
                //if (ContainsIgnoreCase(TextDump, "phoneNumberInputErrorTextView"))
                //{
                //    return TaskResult.OtpError;
                //}

                //Đăng nhập
                if (ContainsIgnoreCase(TextDump, "login_button_horizontal"))
                {
                    TapDynamic(serial, "login_button_horizontal");
                    Common.SetStatus(serial, "Tapped login button");
                    continue;
                }

                //Ấn quên mật khẩu
                //màn hình chưa hiện dialog để ấn nút 'Qua điện thoại'
                if (ContainsIgnoreCase(TextDump, "forgot_password_button") && !ContainsIgnoreCase(TextDump, "alert_dialog_description"))
                {
                    TapDynamic(serial, "forgot_password_button");
                    Common.SetStatus(serial, "Tapped forgot password");
                    continue;
                }

                if (ContainsIgnoreCase(TextDump, "button_text")&&ContainsIgnoreCase(TextDump, "alert_dialog_description"))
                {
                    //TapDynamicNotIgnore(serial, "Tho");
                    TapPosition(serial, new Point(x: 600, y: 1250));
                    Common.SetStatus(serial, "Tapped qua đt");
                    continue;
                }

                //Cho phép quyền
                if (ContainsIgnoreCase(TextDump, "permission_message") && ContainsIgnoreCase(TextDump, "permission_allow_button"))
                {
                    TapDynamic(serial, "permission_allow_button");
                    Common.SetStatus(serial, "Tapped permission_allow_button");
                    continue;
                }

                //Điền sđt
                if (ContainsIgnoreCase(TextDump, "input_field_edit_text") && ContainsIgnoreCase(TextDump, "recovery_phone_continue"))
                {
                    //Net2 đổi khu vực
                    if (net == "net2")
                    {
                        TapDynamic(serial, "input_field_country_code");
                        Common.Sleep(300);
                        DumpUi(serial);
                        InputDynamic(serial, "input_field_edit_text", "Phi");
                        DumpUi(serial);
                        TapDynamic(serial, "country_code_cell");
                        DumpUi(serial);
                    }
                    InputDynamic(serial, "input_field_edit_text", numberphone);
                    Common.SetStatus(serial, "Tapped nhập sđt");
                    DumpUi(serial);
                    TapDynamic(serial, "recovery_phone_continue");
                    Common.Sleep(3000);
                    DumpUi(serial);
                    //if (ContainsIgnoreCase(TextDump, "recovery_phone_error_message") && ContainsIgnoreCase(TextDump, "Vui"))
                    //Lỗi ip hoặc lỗi mất mạng
                    if (ContainsIgnoreCase(TextDump, "recovery_phone_error_message") && ContainsIgnoreCase(TextDump, "recovery_phone_continue"))
                    {
                        if(ContainsIgnoreCase(TextDump, "email")) //Số điện thoại ko hợp lệ
                        {
                            SavePhoneFail(numberphone, net);
                        }
                        CloseAllApp(serial);
                        return TaskResult.OtpError;
                    }
                    Common.Sleep(1000);
                    continue;
                }

                //Nhập OTP
                if (ContainsIgnoreCase(TextDump, "recovery_verify_input"))
                {
                    //lấy otp ko đc thì làm lại từ đầu
                    //lấy OTP
                    GetOTP(serial);
                    //Quay lại điền
                    OpenApp(serial);
                    DumpUi(serial);
                    TapDynamic(serial, "recovery_verify_input");
                    InputClipboard(serial);
                    Common.SetStatus(serial, "Input OTP");
                    Common.Sleep(3500);
                    DumpUi(serial);

                    if (ContainsIgnoreCase(TextDump, "recovery_verify_input"))
                    {
                        CloseAllApp(serial);
                        return TaskResult.Failure;
                    }
                    continue;
                    }

                //Nhập mật khẩu
                if (ContainsIgnoreCase(TextDump, "reset_password_scroll_view"))
                {
                    //string newPassword = EnumPassword.passwordDefault;
                    string newPassword = RandomPasswordString();
                    InputDynamic(serial, "input_field_edit_text", newPassword);
                    Adb.SendKey(serial, "KEYCODE_DPAD_DOWN");
                    Input(serial, newPassword);
                    DumpUi(serial);
                    TapDynamic(serial, "reset_password_continue");
                    Common.Sleep(3000);
                    DumpUi(serial);
                    if(ContainsIgnoreCase(TextDump, "result_text"))
                    {
                        CloseAllApp(serial);
                        return TaskResult.Failure;
                    }
                    else if (ContainsIgnoreCase(TextDump, "forgot_password_button"))
                    {
                        SavePhoneSuccess(numberphone, net);
                        CloseAllApp(serial);
                        return TaskResult.Success;
                    }
                    continue;
                }

                //Nút tiếp tục
                if (ContainsIgnoreCase(TextDump, "continue_button"))
                {
                    TapDynamic(serial, "continue_button");
                    Common.SetStatus(serial, "Tapped continue_button");
                    Common.Sleep(200);
                    continue;
                }

                //if (ContainsIgnoreCase(TextDump, "abcxyz"))
                //{

                //}
            }
        }

        private void OpenApp(string serial)
        {
            OpenApp(serial, "snapchat");
        }

        private void CloseAllApp(string serial)
        {
            CloseApp(serial, "snapchat");
            CloseApp(serial, "getcodeapi");
        }

        public string GetRandomSnapchatPasswordNumber(string net)
        {
            try
            {
                string[] info = new string[1];
                switch (net)
                {
                    case "net1":
                        info = MyFile.GetLine(filePath: "Data\\03-SN1ForgotPassword.txt", index: 1, remove: true).Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                        break;
                    case "net2":
                        info = MyFile.GetLine(filePath: "Data\\14-SN2ForgotPassword.txt", index: 1, remove: true).Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                        break;
                }
                return info[0];
            }
            catch { return null; }
        }

        //Lưu lại số
        private void SavePhoneSuccess(string numberphone, string net)
        {
            DateTime today = DateTime.Today; // As DateTime
            string s_today = today.ToString("dd-MM-yyyy");
            switch (net)
            {
                case "net1":
                    MyFile.WriteAllText(@"Data\\SnapchatSuccess\\03-SN1ForgotPasswordSuccess_"+ s_today +".txt", numberphone, true);
                    break;
                case "net2":
                    MyFile.WriteAllText(@"Data\\SnapchatSuccess\\14-SN2ForgotPasswordSuccess_"+ s_today +".txt", numberphone, true);
                    break;
            }
        }

        //Lưu lại số bị lỗi
        private void SavePhoneFail(string numberphone, string net)
        {
            switch (net)
            {
                case "net1":
                    MyFile.WriteAllText("Data\\03-SN1ForgotPasswordFail.txt", numberphone, true);
                    break;
                case "net2":
                    break;
            }
        }

    }
}
