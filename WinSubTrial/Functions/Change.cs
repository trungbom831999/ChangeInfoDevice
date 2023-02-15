using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinSubTrial.Utilities;
using System.Text.RegularExpressions;

namespace WinSubTrial.Functions
{
    class Change
    {
        public Device device { get; set; }
        
        public bool SaveInfo()
        {
            //Buoc 1: pm clear
            WipeApps();

            //Bo sung
            // Buoc 2: Boot recovery
            Common.SetStatus(device.Serial, "Boot Recovery");
            Adb.Reboot(device.Serial, "recovery");
            Common.SetStatus(device.Serial, "Wait Recovery");
            Adb.WaitRecovery(device.Serial);

            // Buoc 3: Mount system
            Remount();
            // Buoc 4: Remove all old info
            CleanUtils();

            // Buoc 5: Change... 
            PutInfo();

            // Buoc 6: WipeMail
            if (Common.Settings.ChangeLogoutMail)
                CustomWipeMail(device.Serial);
            // Buoc 7: WipeApp
            if (Common.Settings.AppBackup.Length > 0)
            {
                Adb.UninstallApp(device.Serial, Common.GlobalSettings["backup-app"].ToString());
            }
            // Buoc 8: Reboot
            Adb.Reboot(device.Serial);
            Adb.Run(device.Serial, "wait-for-device");

            return true;
        }

        public void WipeApps()
        {
            foreach (string pack in Common.FullWipePackages)
            {
                Adb.UninstallApp(device.Serial, pack);
            }
            string wipePackages = "pm clear " + string.Join(";pm clear ", Common.DefaultWipePackages);
            Adb.Shell(device.Serial, wipePackages.TrimStart(';'));
        }

        public void WipeAppsData()
        {

            string wipePackages2 = "pm clear " + string.Join(";pm clear ", Common.FullWipePackages);
            Adb.Shell(device.Serial, wipePackages2.TrimStart(';'));

            string wipePackages = "pm clear " + string.Join(";pm clear ", Common.DefaultWipePackages);
            Adb.Shell(device.Serial, wipePackages.TrimStart(';'));
        }

        public static void CustomWipeMail(string serial)
        {
            Adb.Shell(serial, "rm -rf /data/system/graphicsstats/* /data/system/dropbox/* /data/system/netstats/* /data/system/procstats/* /data/system/usagestats/* /data/system/syncmanager-log/* /data/system/0/* /data/system_ce/0/* /data/system_de/0/* /data/data/com.android.vending /data/user_de/0/com.android.vending /sdcard/Android/data/com.android.vending /data/data/com.google.android.gms/* /data/user_de/0/com.google.android.gms /sdcard/Android/data/com.google.android.gms /data/data/com.google.android.gsf /data/user_de/0/com.google.android.gsf /sdcard/Android/data/com.google.android.gsf /data/data/com.google.android.gsf.login /data/user_de/0/com.google.android.gsf.login /sdcard/Android/data/com.google.android.gsf.login /data/data/com.android.chrome /data/user_de/0/com.android.chrome /sdcard/Android/data/com.android.chrome");
        }

        public void Remount()
        {
            // Phan vung mount
            for (int i = 0; i < 3; i++)
            {
                string result = Adb.Shell(device.Serial, "twrp mount /system");
                if (ContainsIgnoreCase(result, "Mounted"))
                {
                    break;
                }
                Common.Sleep(2000);
            }
            // Untick readonly
            Adb.Shell(device.Serial, "mount -o rw,remount /");
            Adb.Shell(device.Serial, "mount -o rw,remount /system");
        }
        
        public void CleanUtils()
        {
            Adb.Shell(device.Serial, "rm -rf /system/vendor/Utils /system/system/vendor/Utils /system/system/etc/Utils /system/etc/Utils");
            Adb.Shell(device.Serial, "mkdir /system/vendor/Utils");
        }
        
        public void PutInfo()
        {
            Common.SetStatus(device.Serial, "Save Info...");
            Adb.Shell(device.Serial, device.CmdInfo);
        }
        
        public void ChangeTimezone()
        {
            Common.SetStatus(device.Serial, "Change timezone...");
            string timezone = Adb.Shell(device.Serial, "curl http://ip-api.com/json/?fields=timezone");
            if (timezone.Contains('/'))
            {
                timezone = timezone.Substring(13).Replace("\"}", "");
                Adb.ShellSu(device.Serial, $"setprop persist.sys.timezone '{timezone}' && settings put global time_zone {timezone}");
            }
        }
        
        public void ResetProps()
        {
            Common.SetStatus(device.Serial, "Reset props...");
            Adb.ShellSu(device.Serial, "rm -rf /data/local/tmp/win_props /data/adb/modules/wingisk/system.prop");
            Adb.ShellSu(device.Serial, device.CmdProps);
            Adb.ShellSu(device.Serial, "resetprop -v -p --file /data/local/tmp/win_props");
        }
    
        public void ChangeGsm()
        {
            Common.SetStatus(device.Serial, "Change carrier...");
            Adb.Shell(device.Serial, device.CmdGsm);
        }


        public bool ContainsIgnoreCase(string source, string subString)
        {
            return Regex.IsMatch(source, subString, RegexOptions.IgnoreCase);
        }
    }
}
