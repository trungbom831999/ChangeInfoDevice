using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinSubTrial.Functions
{
    class Backup
    {
        public Device device { get; set; }
        
        public bool Save(string saveDir)
        {
            string account;
            Adb.ShellSu(device.Serial, "mount -o rw,remount /");
            Adb.ShellSu(device.Serial, "mount -o rw,remount /system");
            
            if ((Adb.ShellSu(device.Serial, "ls /system/vendor/Utils").Contains("No such file") 
                    && Adb.ShellSu(device.Serial, "ls /system/etc/Utils").Contains("No such file"))
                || (Adb.ShellSu(device.Serial, "ls /data/system_de/0/accounts_de.db").Contains("No such file")
                    && Adb.ShellSu(device.Serial, "ls /data/system_de/0/accounts_ce.db").Contains("No such file")))
            {
                Common.Info("No info/account to backup!");
                return false;
            }
            
            string accountInfo = Adb.ShellSu(device.Serial, "grep -w account /data/data/com.android.vending/shared_prefs/finsky.xml");
            if (!accountInfo.Contains("@gmail.com"))
            {
                Common.Info("No account to backup!");
                return false;
            }

            account = accountInfo.Substring(accountInfo.IndexOf("account\">") + 9).Split('@')[0];

            string fileName = $"{account}_{DateTime.Now:ddMMyyyy_HHmmss}.wbk";

            Adb.ShellCmd(device.Serial, $"su -c tar -czvf /sdcard/{fileName} system/vendor/Utils data/local/tmp/win_props data/system_de/0/accounts_de.db data/system_ce/0/accounts_ce.db");
            //Adb.Shell(device.Serial, $"su -c tar -cf /sdcard/{fileName} system/vendor/Utils data/local/tmp/win_props data/system_de/0/accounts_de.db data/system_ce/0/accounts_ce.db");

            bool ok = Adb.GetFile(device.Serial, $"/sdcard/{fileName}", saveDir);
            Adb.Shell(device.Serial, $"rm -rf /sdcard/{fileName}");

            return ok;
        }

        public bool Restore(string filePath)
        {
            string fileName = Path.GetFileName(filePath);
            Adb.SendFile(device.Serial, filePath, $"/sdcard/{fileName}");
            if (!Adb.FileExist(device.Serial, $"/sdcard/{fileName}"))
            {
                Common.SetStatus(device.Serial, "Restore fail. Error push backup file");
                return false;
            }

            new Change { device = device }.WipeApps();
            
            Adb.ShellSu(device.Serial, "mount -o rw,remount /; mount -o rw,remount /system");
            
            Adb.ShellSu(device.Serial, "rm -rf /system/vendor/Utils /system/system/vendor/Utils /system/system/etc/Utils /system/etc/Utils");
            Adb.ShellSu(device.Serial, "rm -rf /data/system/graphicsstats/* /data/system/dropbox/* /data/system/netstats/* /data/system/procstats/* /data/system/usagestats/* /data/system/syncmanager-log/* /data/system/0/* /data/system_ce/0/* /data/system_de/0/* /data/data/com.android.vending /data/user_de/0/com.android.vending /sdcard/Android/data/com.android.vending /data/data/com.google.android.gms/* /data/user_de/0/com.google.android.gms /sdcard/Android/data/com.google.android.gms /data/data/com.google.android.gsf /data/user_de/0/com.google.android.gsf /sdcard/Android/data/com.google.android.gsf /data/data/com.google.android.gsf.login /data/user_de/0/com.google.android.gsf.login /sdcard/Android/data/com.google.android.gsf.login /data/data/com.android.chrome /data/user_de/0/com.android.chrome /sdcard/Android/data/com.android.chrome");

            Common.SetStatus(device.Serial, "Restoring info...");
            Adb.ShellSu(device.Serial, $"tar -xf /sdcard/{fileName}");
            Adb.ShellSu(device.Serial, "resetprop -v -p --file /data/local/tmp/win_props");

            Adb.ShellSu(device.Serial, $"rm -rf /sdcard/{fileName}");

            if (!Adb.FileExist(device.Serial, "/data/local/tmp/win_props"))
            {
                Common.SetStatus(device.Serial, "Restore fail. No props info");
                return false;
            }

            Common.SetStatus(device.Serial, "Restored, rebooting...");
            Adb.RebootFast(device.Serial);

            Adb.Shell(device.Serial, "pm enable com.google.android.gms");

            Common.SetStatus(device.Serial, "Restore success");

            return true;
        }
    }
}
