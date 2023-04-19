
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
    class BigoLiteTask : BaseActivity
    {
        public bool isStopAuto = false;
        public string phonenumber { get; set; }

        public TaskResult BigoLiteFilterPhone(string serial)
        {
            phonenumber = GetRandomBigoLiteNumber();
            Adb.SendKey(serial, "KEYCODE_HOME");
            OpenBigoLiteApp(serial);
            Common.Sleep(3000);
            while (true)
            {
                if (phonenumber == null)
                {
                    return TaskResult.StopAuto;
                }
                if (isStopAuto)
                {
                    Common.SetStatus(serial, "Stopped Auto");

                    return TaskResult.StopAuto;
                }

                DumpUi(serial);

                //Nhập sđt
                if (ContainsIgnoreCase(TextDump, "sg.bigo.live.lite:id/lm")){
                    InputDynamic(serial, "sg.bigo.live.lite:id/lm", phonenumber);
                    Common.SetStatus(serial, "Input phone number");
                    TapDynamic(serial, "sg.bigo.live.lite:id/a_8");
                    continue;
                }

                //Bấm nút xác nhận
                if(ContainsIgnoreCase(TextDump, "sg.bigo.live.lite:id/h_"))
                {
                    TapDynamic(serial, "sg.bigo.live.lite:id/h_");
                    phonenumber = GetRandomBigoLiteNumber();
                    Common.SetStatus(serial, "Tap confirm btn");
                    continue;
                }

                //Nhập mã thì lùi lại
                if (ContainsIgnoreCase(TextDump, "sg.bigo.live.lite:id/aal"))
                {
                    TapDynamic(serial, "sg.bigo.live.lite:id/qz");
                    DumpUi(serial);
                    if (ContainsIgnoreCase(TextDump, "sg.bigo.live.lite:id/h9"))
                    {
                        TapDynamic(serial, "sg.bigo.live.lite:id/h9");
                    }
                    Common.SetStatus(serial, "Back to phonenumber");
                    phonenumber = GetRandomBigoLiteNumber();
                    continue;
                }

                //Khi sang màn hình thành công
                if (ContainsIgnoreCase(TextDump, "sg.bigo.live.lite:id/vx"))
                {
                    SavePhoneSuccess(phonenumber);
                    CloseAllApp(serial);
                    return TaskResult.Success;
                }

            }
        }

        public string GetRandomBigoLiteNumber()
        {
            try
            {
                string[] info = MyFile.GetLine(filePath: "Data\\06-B1IN.txt", index: 1, remove: true).Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                return info[0];
                //var r = new Random();
                //var randomLineNumber = r.Next(0, 200);
                //string line = File.ReadLines("Data\\06-BigoIN.txt").Skip(randomLineNumber).Take(1).First();
                //return line.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries)[0];

                //var lines = File.ReadAllLines("Data\\06-BigoIN.txt");
                //var r = new Random();
                //var randomLineNumber = r.Next(0, lines.Length - 1);
                //var line = lines[randomLineNumber].Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                //return line[0];

                //string line = MyFile.GetRandomLine("Data\\06-BigoIN.txt", true);
                //return line.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries)[0];
            }
            catch { return null; }
        }

        private void SavePhoneSuccess(string phone)
        {
            MyFile.WriteAllText("Data\\07-B1Sucess.txt", phone, true);
        }

        private void CloseAllApp(string serial)
        {
            CloseApp(serial, "bigolite");
        }
    }
}
