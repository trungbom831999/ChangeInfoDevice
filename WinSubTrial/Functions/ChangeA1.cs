using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using WinSubTrial.Utilities;

namespace WinSubTrial.Functions
{
    class ChangeA1
    {
        public Device device { get; set; }
        private Timer wipingTimer = new Timer();
        private int secondsRemain = 12;

        public bool SaveInfo()
        {
            wipingTimer = new Timer();
            wipingTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            wipingTimer.Interval = 10000;
            wipingTimer.Enabled = true;
            WipeAppsData();

            if (Common.Settings.ChangeRemoveApp && Common.Settings.AppBackup.Length > 0)
            {
                Adb.UninstallApp(device.Serial, Common.GlobalSettings["backup-app"].ToString());
            }
            if (Common.Settings.ChangeLogoutMail)
            {
                Funcs.WipeMail(device.Serial);
            }
            Remount();
            CleanUtils();
            PutInfo();
            ChangeTimezone();
            ResetProps();

            Common.SetStatus(device.Serial, "Rebooting...");
            Adb.RebootFast(device.Serial);

            ChangeGsm();

            Common.SetStatus(device.Serial, "ChangeA1: Save info done");
            wipingTimer.Dispose();
            return true;
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            secondsRemain -= 1;
            Common.SetStatus(device.Serial, $"Wiping again after {secondsRemain * 10} seconds");

            if (secondsRemain <= 0)
            {
                secondsRemain = 12;
                wipingTimer.Dispose();
                Adb.Reboot(device.Serial);
                while (!Adb.IsDeviceOnine(device.Serial))
                {
                    Common.Sleep(2000);
                }
                Adb.Run(device.Serial, "wait-for-device");
                SaveInfo();
            }
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

        public void Remount()
        {
            Adb.ShellSu(device.Serial, "mount -o rw,remount /");
            Adb.ShellSu(device.Serial, "mount -o rw,remount /system");
        }

        public void CleanUtils()
        {
            Adb.ShellSu(device.Serial, "rm -rf /system/vendor/Utils /system/system/vendor/Utils /system/system/etc/Utils /system/etc/Utils");
            Adb.ShellSu(device.Serial, "mkdir /system/vendor/Utils");
        }

        public void PutInfo()
        {
            Adb.ShellSu(device.Serial, device.CmdInfo);
        }

        public void ChangeTimezone()
        {
            string timezone = Adb.Shell(device.Serial, "curl http://ip-api.com/json/?fields=timezone");
            if (timezone.Contains('/'))
            {
                timezone = timezone.Substring(13).Replace("\"}", "");
                Adb.ShellSu(device.Serial, $"setprop persist.sys.timezone '{timezone}' && settings put global time_zone {timezone}");
            }
        }

        public void ResetProps()
        {
            Adb.ShellSu(device.Serial, "rm -rf /data/local/tmp/win_props /data/adb/modules/wingisk/system.prop");
            Adb.ShellSu(device.Serial, device.CmdProps);
            Adb.ShellSu(device.Serial, "resetprop -v -p --file /data/local/tmp/win_props");
        }

        public void ChangeGsm()
        {
            Adb.Shell(device.Serial, device.CmdGsm);
        }
    }
}
