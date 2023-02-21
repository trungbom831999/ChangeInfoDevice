
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
    class TinderTask : BaseActivity
    {
        public bool isStopAuto = false;
        public string phonenumber { get; set; }

        private string[] peopleName = { "Duy", "Phuc", "Quan", "Nghia", "Tham", "Toai", "Trang", "Hoang", "Anh" };
        private string[] peopleFirst = { "Nguyen", "Trinh", "Tran", "Vu" };
        private string[] months = { "thg%s1", "thg%s2", "thg%s3", "thg%s4", "thg%s5", "thg%s6", "thg%s7", "thg%s8", "thg%s9", "thg%s10", "thg%s11", "thg%s12" };
        private string[] years = { "1990", "1991", "1992", "1993", "1994", "1995", "1996", "1997", "1998", "1999", "2000", "2001", "2002", "2003" };

        public TaskResult TinderAutomationRegister(string serial)
        {
            Adb.SendKey(serial, "KEYCODE_HOME");
            Common.SetStatus(serial, "Open get code API");
            OpenGetCodeApi(serial);
            //OpenTinderApp(serial);
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

                    InputDynamic(serial, "editBrand", "ti");
                    Common.SetStatus(serial, "Enter Brand");
                    //Common.Sleep(500);

                    TapDynamic(serial, "btnGetCode");
                    Common.SetStatus(serial, "tappeb btnGetCode");
                    //Common.Sleep(1000);
                    OpenTinderApp(serial);
                    Common.Sleep(2500);
                    continue;
                }

                //Lỗi sđt quá nhiều
                //Để lỗi lên đầu để nếu có lỗi sẽ Reboot luôn
                if (ContainsIgnoreCase(TextDump, "phoneNumberInputErrorTextView"))
                {
                    return TaskResult.OtpError;
                }

                //Nhấn đăng ký bằng số đt
                if (ContainsIgnoreCase(TextDump, "login_epoxy_recycler_view"))
                {
                    TapDynamic(serial, "THO");
                    Common.SetStatus(serial, "Tapped Register by phone number");
                    Common.Sleep(200);
                    continue;
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
                    //Lấy mã OTP
                    OpenGetCodeApi(serial);
                    Common.SetStatus(serial, "Enter 6-digit code");
                    DumpUi(serial);

                    TapDynamic(serial, "btnGetOtp");
                    Common.SetStatus(serial, "Tapped button get otp");
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
                    continue;
                }

                if (ContainsIgnoreCase(TextDump, "continueButton"))
                {
                    // tiep tuc
                    TapDynamic(serial, "continueButton");
                    Common.SetStatus(serial, "Tapped continueButton");
                    Common.Sleep(200);
                    continue;
                } 
            }
        }

        public string GetRandomNumberPhone()
        {
            try
            {
                string[] info = MyFile.GetLine(filePath: "Data\\tiktok.txt", index: 1, remove: true).Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                return info[0];
            }
            catch { return null; }
        }

        private void OpenGetCodeApi(string serial)
        {
            Adb.Shell(serial, "am start -n com.example.getcodeapi/.MainActivity");
        }

        private void OpenTinderApp(string serial)
        {
            Adb.Shell(serial, " am start -n com.tinder/.activities.LoginActivity");
        }
        
    }
}
