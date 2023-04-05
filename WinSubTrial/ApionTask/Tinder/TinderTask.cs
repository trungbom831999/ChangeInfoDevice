
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
    class TinderTask : BaseActivity
    {
        public bool isStopAuto = false;
        public string phonenumber { get; set; }

        public TaskResult TinderAutomationRegister(string serial)
        {
            Adb.SendKey(serial, "KEYCODE_HOME");
            Common.SetStatus(serial, "Open get code API");
            FillInfoGetCodeAPI(serial, phonenumber, EnumBrandApp.tinder, "net1");
            OpenTinderApp(serial);
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
                if (ContainsIgnoreCase(TextDump, "phoneNumberInputErrorTextView"))
                {
                    return TaskResult.OtpError;
                }

                //Nhấn đăng ký bằng số đt
                if (ContainsIgnoreCase(TextDump, "login_epoxy_recycler_view"))
                {
                    if (ContainsIgnoreCase(TextDump, "THO"))
                    {
                        TapDynamic(serial, "THO");
                        Common.SetStatus(serial, "Tapped Register by phone number");
                        Common.Sleep(200);
                        continue;
                    }
                    else if(ContainsIgnoreCase(TextDump, "kh"))
                    {
                        TapDynamicNotIgnore(serial, "kh");
                        Common.SetStatus(serial, "Tapped other option register");
                        Common.Sleep(200);
                        continue;
                    }
                }

                //Nhập sđt
                if(ContainsIgnoreCase(TextDump, "phoneNumberInputView")){
                    InputDynamic(serial, "phoneNumberInputView", phonenumber);
                    Common.SetStatus(serial, "Input phone number");
                    if (ContainsIgnoreCase(TextDump, "continueButton"))
                    {
                        // tiếp tục
                        TapDynamic(serial, "continueButton");
                        Common.SetStatus(serial, "Tapped continueButton");
                        Common.Sleep(200);
                        continue;
                    }
                }

                //Mã OTP
                if (ContainsIgnoreCase(TextDump, "myCodeLabel") 
                    || ContainsIgnoreCase(TextDump, "phoneNumberLabel") 
                    || ContainsIgnoreCase(TextDump, "resendButton"))
                {
                    int countResendOTP = 3;
                    while (countResendOTP > 0)
                    {
                        //Lấy mã OTP
                        GetOTP(serial);
                        //Quay lại Tinder điền OTP
                        OpenTinderApp(serial);
                        DumpUi(serial);
                        TapDynamic(serial, "NAF");
                        InputClipboard(serial);
                        Common.SetStatus(serial, "Tapped code text filed");

                        //tiếp tục
                        if (ContainsIgnoreCase(TextDump, "continueButton"))
                        {
                            TapDynamic(serial, "continueButton");
                            Common.SetStatus(serial, "Tapped continueButton");
                        }
                        Common.Sleep(2000);
                        DumpUi(serial);
                        //Xác thực email - Coi như thành công
                        if (ContainsIgnoreCase(TextDump, "collect_email_title")
                            || ContainsIgnoreCase(TextDump, "collect_email_input_edit_text"))
                        {
                            //back về đăng ký
                            TapDynamic(serial, "collect_email_back_button");
                            Common.SetStatus(serial, "Tapped back after email");
                            CloseAllApp(serial);
                            return TaskResult.Success;
                        }

                        DumpUi(serial);
                        if (ContainsIgnoreCase(TextDump, "resendButton"))
                        {
                            TapDynamic(serial, "resendButton");
                            Common.SetStatus(serial, "Tapped resend OTP");
                        }
                        countResendOTP--;
                        if (countResendOTP <= 0)
                        {
                            CloseAllApp(serial);
                            return TaskResult.Failure;
                        }
                        continue;
                    }
                }

                //Xác thực email - Coi như thành công
                if(ContainsIgnoreCase(TextDump, "collect_email_title")
                    || ContainsIgnoreCase(TextDump, "collect_email_input_edit_text"))
                {
                    //back về đăng ký
                    TapDynamic(serial, "collect_email_back_button");
                    Common.SetStatus(serial, "Tapped back after email");
                    CloseAllApp(serial);
                    return TaskResult.Success;
                }

                if (ContainsIgnoreCase(TextDump, "continueButton"))
                {
                    // tiep tuc
                    TapDynamic(serial, "continueButton");
                    Common.SetStatus(serial, "Tapped continueButton");
                    Common.Sleep(200);
                    continue;
                }

                //if (ContainsIgnoreCase(TextDump, "abcxyz"))
                //{

                //}
            }
        }

        private void CloseAllApp(string serial)
        {
            CloseApp(serial, "tinder");
            CloseApp(serial, "getcodeapi");
        }
    }
}
