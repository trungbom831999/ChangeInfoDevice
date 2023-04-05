
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
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
    class TelegramTask : BaseActivity
    {
        public bool isStopAuto = false;

        public TaskResult TelegramRegister(string serial, string net)
        {
            string numberphone = GetRandomNumberPhone(net); 
            Common.SetStatus(serial, $"Get snapchat phonenumber: {numberphone}");
            Adb.SendKey(serial, "KEYCODE_HOME");
            Common.SetStatus(serial, "Open get code API");
            FillInfoGetCodeAPI(serial, numberphone, EnumBrandApp.telegram, net);
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

                //Bắt đầu 
                if (ContainsIgnoreCase(TextDump, "Start Messaging")){
                    TapDynamic(serial, "Start Messaging");
                    Common.SetStatus(serial, "Start Messaging");
                    continue;
                }

                //Điền sđt, bấm tiếp
                //Ko có id nên dùng index
                if (ContainsIgnoreCase(TextDump, "Your phone number")){
                    InputDynamic(serial, "index=\"5\"", numberphone);
                    Common.SetStatus(serial, "Input phonenumber");
                    TapPosition(serial, new Point(x: 1250, y: 1600));
                    DumpUi(serial);
                    TapDynamic(serial, "text=\"Yes\"");
                    Common.Sleep(5000);
                    continue;
                }

                //Nhập OTP
                if (ContainsIgnoreCase(TextDump, "Enter code") || ContainsIgnoreCase(TextDump, "Check your Telegram messages"))
                {
                    GetOTP(serial);
                    //Quay lại điền OTP
                    OpenApp(serial);
                    DumpUi(serial);
                    TapDynamic(serial, "index=\"2\"");
                    TapPosition(serial, new Point(x: 400, y: 800));
                    //InputClipboard(serial);
                    //InputDynamic(serial, "index=\"2\"", "nội dung clipboard"
                    Common.SetStatus(serial, "Input OTP");
                    Common.Sleep(2000);
                    if (ContainsIgnoreCase(TextDump, "Enter code") || ContainsIgnoreCase(TextDump, "Check your Telegram messages"))
                    {
                        //OTP thất bại
                        SaveBackNumberPhone(numberphone, net);
                        CloseAllApp(serial);
                        return TaskResult.OtpError;
                    }
                    continue;
                }

                //Lỗi messages
                //if (ContainsIgnoreCase(TextDump, "Check your Telegram messages"))
                //{
                //    //TapPosition(serial, new Point(x: 100, y: 200));
                //    CloseAllApp(serial);
                //    return TaskResult.Failure;
                //}

                //if (ContainsIgnoreCase(TextDump, "Edit number"))
                //{
                //    TapDynamic(serial, "text=\"Edit\"");
                //    continue;
                //}


                if (ContainsIgnoreCase(TextDump, "abcxyz")){}
            }
        }

        private void OpenApp(string serial)
        {
            OpenApp(serial, "telegram");
        }

        private void CloseAllApp(string serial)
        {
            CloseApp(serial, "telegram");
            CloseApp(serial, "getcodeapi");
        }

        private string GetRandomNumberPhone(string net)
        {
            try
            {
                string[] info = new string[1];
                switch (net)
                {
                    case "net1":
                        break;
                    case "net2":
                        info = MyFile.GetLine(filePath: "Data\\12-TE2IN.txt", index: 1, remove: true).Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                        break;
                }
                return info[0];
            }
            catch { return null; }
        }

        private void SavePhoneSuccess(string phone, string net)
        {
            switch (net)
            {
                case "net1":
                    break;
                case "net2":
                    File.AppendAllText("Data\\13-TE2Sucess.txt", phone + "\n");
                    break;
            }
        }

        private void SaveBackNumberPhone(string phone, string net)
        {
            switch (net)
            {
                case "net1":
                    break;
                case "net2":
                    File.AppendAllText("Data\\12-TE2IN.txt", phone);
                    break;
            }
        }

    }
}
