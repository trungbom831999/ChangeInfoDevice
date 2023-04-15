
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
    class GlobalSmartTask : BaseActivity
    {
        public bool isStopAuto = false;

        public TaskResult GlobalSmartRegister(string serial)
        {
            string numberphone = GetRandomNumberPhone();
            if (numberphone == null)
            {
                Common.SetStatus(serial, "Call API fail. Out of number");
                return TaskResult.StopAuto;
            }
            Adb.SendKey(serial, "KEYCODE_HOME");
            FillInfoGetCodeAPI(serial, numberphone, EnumBrandApp.globalsmart);
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

                //Đồng ý chính sách
                if (ContainsIgnoreCase(TextDump, "positiveButton"))
                {
                    TapDynamic(serial, "positiveButton");
                    Common.SetStatus(serial, "Tap positive button");
                    continue;
                }

                //Tiếp tục sử dụng
                if (ContainsIgnoreCase(TextDump, "tv_confirm"))
                {
                    TapDynamic(serial, "tv_confirm");
                    Common.SetStatus(serial, "Tap confirm continue use");
                    continue;
                }

                //Ấn đăng ký
                if (ContainsIgnoreCase(TextDump, "btn_register"))
                {
                    TapDynamic(serial, "btn_register");
                    Common.SetStatus(serial, "Tap register");
                    continue;
                }

                //Chọn khu vực
                //Điền sđt
                //Chung 1 màn hình nên phải xử lý chung 1 cụm
                if (ContainsIgnoreCase(TextDump, "tuya_register_page_chooseRegionItem"))
                {
                    Common.SetStatus(serial, "Choose location");
                    TapDynamic(serial, "tuya_register_page_chooseRegionItem");
                    Common.Sleep(1000);
                    DumpUi(serial);
                    InputDynamic(serial, "tv_search_box_collapsed_hint", "vie");
                    Common.Sleep(1000);
                    DumpUi(serial);
                    TapDynamic(serial, "Vietnam");
                    DumpUi(serial);
                    Common.SetStatus(serial, "Fill numberphone");
                    InputDynamic(serial, "tuya_register_page_accountItem", numberphone);
                    TapPosition(serial, new Point(x: 250, y: 1250));
                    TapDynamic(serial, "tuya_register_page_loadingButton");
                    Common.Sleep(300);
                    TapDynamic(serial, "tuya_register_page_loadingButton");
                    continue;
                }

                //Nhập mã xác minh
                if (ContainsIgnoreCase(TextDump, "tuya_verify_account_page_veriftyTextFeild"))
                {
                    GetOTP(serial);
                    OpenApp(serial);
                    DumpUi(serial);
                    //TapDynamic(serial, "tuya_verify_account_page_veriftyTextFeild");
                    //InputClipboard(serial);
                    InputDynamic(serial, "tuya_verify_account_page_veriftyTextFeild", "123456");
                    Common.SetStatus(serial, "Input OTP");
                    Common.Sleep(2000);
                    DumpUi(serial);
                    //if (ContainsIgnoreCase(TextDump, "tuya_verify_account_page_veriftyTextFeild"))
                    //{
                    //    //OTP thất bại
                    //    CloseAllApp(serial);
                    //    return TaskResult.OtpError;
                    //}
                    continue;
                }

                if (ContainsIgnoreCase(TextDump, "abcxyz")){}

            }
        }

        private string GetRandomNumberPhone()
        {
            try
            {
                string[] info = new string[1];
                info = MyFile.GetLine(filePath: "Data\\19-GS1IN.txt", index: 1, remove: true).Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                return info[0];
            }
            catch { return null; }
        }
        private void SavePhoneSuccess(string phone)
        {
            File.AppendAllText("Data\\20-GS1Sucess.txt", phone + "\n");
        }
        private void OpenApp(string serial)
        {
            OpenApp(serial, "globalsmart");
        }
        private void CloseAllApp(string serial)
        {
            CloseApp(serial, "globalsmart");
            CloseApp(serial, "getcodeapi");
        }
    }
}
