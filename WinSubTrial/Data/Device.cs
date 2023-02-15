using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace WinSubTrial
{
    public class Device : IEquatable<Device>
    {
        public int autoIncrementId { get; set; }
        public string Serial { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }
        public string Model { get; set; }

        public RequestInfo RequestOption { get; set; }
        public FullInfo Info { get; set; }
        public string CmdInfo { get; set; }
        public string Timezone { get; set; }
        public string CmdProps { get; set; }
        public string CmdGsm { get; set; }

        public bool isSelected { get; set; }

        public bool Equals(Device other)
        {
            return this.Serial == other.Serial;
        }

        public string GetWinName()
        {
            string winName = Adb.Shell(Serial, "cat /sdcard/win_name");
            return !winName.Contains("No such file") ? winName : string.Empty;
        }

        public void SetWinName()
        {
            if (string.IsNullOrEmpty(Name)) return;
            Adb.Shell(Serial, $"echo '{Name}' > /sdcard/win_name");
        }

        #region Faking Info
        public void GetInfo()
        {
            string query = "/change?" +
                            $"brand={RequestOption.Brand}" +
                            $"&country={RequestOption.Country}" +
                            $"&sdk={RequestOption.SDK}" +
                            $"&operator={RequestOption.Network}";
            Debug.WriteLine($"Query: {query}");

            JObject fakeInfo;
            while (true)
            {
                fakeInfo = JObject.Parse(new xNet.HttpRequest().GetString(Common.ServerApi + query));
                if (fakeInfo.ContainsKey("props")) break;

                Common.Sleep(3000);
            }
            
            GetFullInfo(fakeInfo);
            GetCmdInfo();
            GetCmdProp(fakeInfo);
            GetCmdGsm();
        }

        private void GetFullInfo(JObject fakeInfo)
        {
            Info = JsonConvert.DeserializeObject<FullInfo>(fakeInfo["fullInfo"].ToString());
            Debug.WriteLine(fakeInfo["fullInfo"].ToString());
        }

        private void GetCmdInfo()
        {
            CmdInfo = 
                $"echo '{Cryption.Encrypt(Info.Ssid)}' > /system/vendor/Utils/Ssid " +
                $"&& echo '{Cryption.Encrypt(Info.Bssid)}' > /system/vendor/Utils/Bssid " +
                $"&& echo '{Cryption.Encrypt(Info.MacAddress)}' > /system/vendor/Utils/MacAddress " +
                $"&& echo '{Cryption.Encrypt(Info.SimOperator)}' > /system/vendor/Utils/SimOperator " +
                $"&& echo '{Cryption.Encrypt(Info.SimOperatorName)}' > /system/vendor/Utils/SimOperatorName " +
                $"&& echo '{Cryption.Encrypt(Info.Mcc)}' > /system/vendor/Utils/Mcc " +
                $"&& echo '{Cryption.Encrypt(Info.Mnc)}' > /system/vendor/Utils/Mnc " +
                $"&& echo '{Cryption.Encrypt(Info.CountryCode)}' > /system/vendor/Utils/CountryCode " +
                $"&& echo '{Cryption.Encrypt(Info.IMEI)}' > /system/vendor/Utils/IMEI " +
                $"&& echo '{Cryption.Encrypt(Info.SimSerial)}' > /system/vendor/Utils/SimSerial " +
                $"&& echo '{Cryption.Encrypt(Info.SubscriberId)}' > /system/vendor/Utils/SubscriberId " +
                $"&& echo '{Cryption.Encrypt(Info.PhoneNumber)}' > /system/vendor/Utils/PhoneNumber " +
                $"&& echo '{Cryption.Encrypt(Info.androidID)}' > /system/vendor/Utils/androidID " +
                $"&& echo '{Cryption.Encrypt(Info.AndroidSeri)}' > /system/vendor/Utils/AndroidSeri " +
                $"&& echo '{Cryption.Encrypt(Info.GLVendor)}' > /system/vendor/Utils/GLVendor " +
                $"&& echo '{Cryption.Encrypt(Info.GLRenderer)}' > /system/vendor/Utils/GLRenderer " +
                $"&& echo '{Cryption.Encrypt(Info.wifiMac)}' > /system/vendor/Utils/wifiMac " +
                $"&& echo '{Cryption.Encrypt(Info.wifiName)}' > /system/vendor/Utils/wifiName " +
                $"&& echo '{Cryption.Encrypt(Info.BSSID)}' > /system/vendor/Utils/BSSID " +
                $"&& echo '{Cryption.Encrypt(Info.UserAgent)}' > /system/vendor/Utils/UserAgent";
        }
        
        private void GetCmdProp(JObject fakeInfo)
        {
            /*StringBuilder @string = new StringBuilder();
            Utils.MyFile.WriteAllText("win_props", fakeInfo["props"].ToString());
            foreach(string prop in fakeInfo["props"].ToString().Split(new string[] { "\\n" }, StringSplitOptions.RemoveEmptyEntries))
            {
                @string.Append($"echo '{prop}' >> /data/local/tmp/win_props");
            }

            CmdProps = @string.ToString();*/

            if ((bool)Common.Settings.FixPadFgo) fakeInfo["props"] += "\nro.debuggable=0";
            
            CmdProps = $"echo '{fakeInfo["props"]}' >> /data/local/tmp/win_props";
        }

        private void GetCmdGsm()
        {
            List<string> setprops = new List<string>
            {
                $"su -c setprop gsm.version.baseband \"{Info.BaseBand}\"",
                $"su -c setprop gsm.sim.operator.iso-country \"{Info.CountryCode.ToLower()}\"",
                $"su -c setprop gsm.operator.iso-country \"{Info.CountryCode.ToLower()}\"",
                $"su -c setprop gsm.sim.operator.alpha \"{Info.SimOperatorName}\"",
                $"su -c setprop gsm.operator.alpha \"{Info.SimOperatorName}\"",
                $"su -c setprop gsm.sim.operator.numeric \"{Info.SimOperator}\"",
                $"su -c setprop gsm.operator.numeric \"{Info.SimOperator}\""
            };

            CmdGsm = string.Join(";", setprops);
        }
        #endregion Faking Info
    }
}