
using Faker;
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
    class TiktokLogin : BaseActivity
    {
        public bool isStopAuto = false;

        public TaskResult TiktokLoginAuto(string serial)
        {
            string numberphone = GetRandomNumberPhone();
            if (numberphone == null)
            {
                Common.SetStatus(serial, "Call API fail. Out of number");
                return TaskResult.StopAuto;
            }
            Adb.SendKey(serial, "KEYCODE_HOME");
            FillInfoGetCodeAPI(serial, numberphone, EnumBrandApp.tiktok);
            OpenApp(serial);
            Common.Sleep(2000);
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

                //Vào màn chính
                if (TextDump == "" || ContainsIgnoreCase(TextDump, "com.zhiliaoapp.musically.go:id/t0"))
                {
                    TapPosition(serial, new Point(x: 1400, y: 2500));
                    Common.SetStatus(serial, "Tap Tôi");
                    continue;
                }

                //Chính sách quyền riêng tư
                if (ContainsIgnoreCase(TextDump, "com.zhiliaoapp.musically.go:id/a2z")) {
                    TapDynamic(serial, "com.zhiliaoapp.musically.go:id/a2z");
                    Common.SetStatus(serial, "Tap chinh sach");
                    continue;
                }

                //Nhấn đăng nhập và nhấn đăng nhập qua sđt
                if (ContainsIgnoreCase(TextDump, "com.zhiliaoapp.musically.go:id/abm")) {
                    if(!ContainsIgnoreCase(TextDump, "TikTok ID"))
                    {
                        //Nhấn đăng nhập
                        TapDynamic(serial, "com.zhiliaoapp.musically.go:id/abm");
                        Common.SetStatus(serial, "Tap login");
                    }
                    else
                    {
                        //Đăng nhập bằng sđt
                        TapDynamic(serial, "com.zhiliaoapp.musically.go:id/ab9");
                        Common.SetStatus(serial, "Tap login by phone");
                    }
                    continue;
                }

                //Nhập sđt
                if (ContainsIgnoreCase(TextDump, "com.zhiliaoapp.musically.go:id/uy"))
                {
                    InputDynamic(serial, "com.zhiliaoapp.musically.go:id/uy", numberphone);
                    Common.SetStatus(serial, "Input numberphone");
                    TapDynamic(serial, "com.zhiliaoapp.musically.go:id/y2");
                    Common.SetStatus(serial, "Send OTP");
                    Common.Sleep(3000);
                    DumpUi(serial);

                    if (ContainsIgnoreCase(TextDump, "com.zhiliaoapp.musically.go:id/ul"))
                    {
                        return TaskResult.Success;
                    }
                    else if (ContainsIgnoreCase(TextDump, "com.zhiliaoapp.musically.go:id/uy"))
                    {
                        return TaskResult.Failure;
                    }
                    continue;
                }

                //if (ContainsIgnoreCase(TextDump, "abcxyz")) { }

            }
        }

        private void OpenApp(string serial)
        {
            OpenApp(serial, "tiktoklite");
        }

        private void CloseAllApp(string serial)
        {
            CloseApp(serial, "tiktoklite");
            CloseApp(serial, "getcodeapi");
        }

        private string GetRandomNumberPhone()
        {
            try
            {
                string[] info = new string[1];
                info = MyFile.GetLine(filePath: "Data\\39-T1IN.txt", index: 1, remove: true).Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                return info[0];
            }
            catch { return null; }
        }

        //Lưu lại số
        private void SavePhoneSuccess(string numberphone)
        {
            MyFile.WriteAllText("Data\\.txt", numberphone, true);
        }
    }
}
