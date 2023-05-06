using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinSubTrial.Functions
{
    class Backup
    {
        public Device device { get; set; }

        public bool Save(string saveDir, string bk_packages)
        {
            Adb.ShellSu(device.Serial, "mount -o rw,remount /");
            Adb.ShellSu(device.Serial, "mount -o rw,remount /system");

            string fileName = "";
            string pathAppbk = "";

            if (bk_packages != "")
            {
                if (bk_packages.Contains(","))
                {
                    string[] bkAppName = bk_packages.Split(new string[1] { "," }, StringSplitOptions.None);
                    int i = 0;
                    foreach (string app in bkAppName)
                    {
                        i++;
                        pathAppbk += $"/data/data/{app} /data/user_de/0/{app} /sdcard/Android/data/{app} /data/misc/profiles/cur/0/{app} /data/misc/profiles/ref/{app}";
                        if (i < bkAppName.Count())
                        {
                            pathAppbk += " ";
                        }
                    }
                }
                else
                {
                    pathAppbk = $"/data/data/{bk_packages} /data/user_de/0/{bk_packages} /sdcard/Android/data/{bk_packages} /data/misc/profiles/cur/0/{bk_packages} /data/misc/profiles/ref/{bk_packages}";
                }

                fileName = $"{bk_packages}_{DateTime.Now:ddMMyyyy_HHmmss}.wbk";
            }
            else
            {
                string account = "";
                string accountInfo = Adb.ShellSu(device.Serial, "grep -w account /data/data/com.android.vending/shared_prefs/finsky.xml");
                if (!accountInfo.Contains("@gmail.com"))
                {
                    Common.Info("No account to backup!");
                    return false;
                }
                account = accountInfo.Substring(accountInfo.IndexOf("account\">") + 20).Split('@')[0];

                fileName = $"{account}_{DateTime.Now:ddMMyyyy_HHmmss}.wbk";
            }

            pathAppbk += " /system/vendor/Utils /data/local/tmp/win_props /data/system_de/0/accounts_de.db /data/system_ce/0/accounts_ce.db";
            pathAppbk += " /data/data/com.android.vending/ /data/data/com.google.android.gms/ /data/data/com.google.android.gsf/";
            pathAppbk += " /data/user_de/0/com.android.vending/ /data/user_de/0/com.google.android.gms/ /data/user_de/0/com.google.android.gsf/ /data/user_de/0/com.google.android.gsf.login";
            pathAppbk += " /data/misc/profiles/cur/0/com.android.vending /data/misc/profiles/cur/0/com.google.android.gms /data/misc/profiles/cur/0/com.google.android.gsf";
            pathAppbk += " /data/system/ /sdcard/Android/data/com.android.vending /sdcard/Android/data/com.google.android.gms";

            Console.WriteLine("list Path: " + pathAppbk);

            Adb.ShellCmd(device.Serial, $"su -c tar -czvf /sdcard/{fileName} {pathAppbk}", 9);
            bool isSuccess = Adb.GetFile(device.Serial, $"/sdcard/{fileName}", saveDir);
            if (isSuccess)
            {
                Adb.Shell(device.Serial, $"rm -rf /sdcard/{fileName}");
                return true;
            }

            return false;
        }

        public bool Restore(string filePath, string bk_packages)
        {
            string appRestorepath = "";
            if (bk_packages != "")
            {
                if (bk_packages.Contains(","))
                {
                    string[] bkAppName = bk_packages.Split(new string[1] { "," }, StringSplitOptions.None);
                    int i = 0;
                    foreach (string app in bkAppName)
                    {
                        i++;
                        appRestorepath += $"/data/data/{app}/* /data/user_de/0/{app}/* /sdcard/Android/data/{app}/* /data/misc/profiles/cur/0/{app}/* /data/misc/profiles/ref/{app}/*";
                        if (i < bkAppName.Count())
                        {
                            appRestorepath += " ";
                        }
                    }
                }
                else
                {
                    appRestorepath += $"/data/data/{bk_packages}/* /data/user_de/0/{bk_packages}/* /sdcard/Android/data/{bk_packages}/* /data/misc/profiles/cur/0/{bk_packages}/* /data/misc/profiles/ref/{bk_packages}/*";
                }
            }

            string fileName = Path.GetFileName(filePath);
            Adb.SendFile(device.Serial, filePath, $"/sdcard/{fileName}");
            if (!Adb.FileExist(device.Serial, $"/sdcard/{fileName}"))
            {
                Common.SetStatus(device.Serial, "Restore fail. Error push backup file");
                return false;
            }

            new Change { device = device }.WipeApps();

            Adb.ShellSu(device.Serial, "mount -o rw,remount /; mount -o rw,remount /system");

            Adb.ShellSu(device.Serial, "rm -rf /system/vendor/Utils/* /system/system/vendor/Utils/* /system/system/etc/Utils/* /system/etc/Utils/*");

            Adb.ShellSu(device.Serial, "rm -rf " +
                "/data/system/graphicsstats/* /data/system/dropbox/* /data/system/netstats/* /data/system/procstats/* " +
                "/data/system/usagestats/* /data/system/syncmanager-log/* /data/system/0/* " +
                "/data/system_ce/0/* /data/system_de/0/* " +
                "/data/user_de/0/com.android.vending/* /data/user_de/0/com.google.android.gms/* /data/user_de/0/com.google.android.gsf/* /data/user_de/0/com.google.android.gsf.login /data/user_de/0/com.android.chrome/* " +
                "/sdcard/Android/data/com.google.android.gms/* /sdcard/Android/data/com.android.vending/* /sdcard/Android/data/com.google.android.gsf/* " +
                "/sdcard/Android/data/com.google.android.gsf.login /sdcard/Android/data/com.android.chrome/* " +
                "/data/data/com.android.vending/* /data/data/com.google.android.gms/* /data/data/com.google.android.gsf/* /data/data/com.google.android.gsf.login /data/data/com.android.chrome/* " +
                "/data/misc/profiles/cur/0/com.android.vending/* /data/misc/profiles/cur/0/com.google.android.gms/* /data/misc/profiles/cur/0/com.google.android.gsf/*");

            Adb.ShellSu(device.Serial, $"rm -rf {appRestorepath}");

            string pathAPK = AppDomain.CurrentDomain.BaseDirectory + "APKFile";

            Adb.ShellSu(device.Serial, $"tar -xf /sdcard/{fileName}");

            DateTime now0 = DateTime.Now;
            while (!Adb.FileExist(device.Serial, "/data/local/tmp/win_props"))
            {
                Application.DoEvents();
                Thread.Sleep(50);

                if ((DateTime.Now - now0).TotalSeconds > 9.0)
                {
                    break;
                }
            }

            if (!Adb.FileExist(device.Serial, "/data/local/tmp/win_props"))
            {
                Common.SetStatus(device.Serial, "Restore fail. No props info");
                return false;
            }

            Adb.ShellSu(device.Serial, "resetprop -v -p --file /data/local/tmp/win_props");

            if (bk_packages != "")
            {
                if (bk_packages.Contains(","))
                {
                    string[] bkAppName = bk_packages.Split(new string[1] { "," }, StringSplitOptions.None);

                    foreach (string app in bkAppName)
                    {
                        Common.SetStatus(device.Serial, $"Restoring: {app}");

                        string output = Adb.RunCmd(device.Serial, $"install {pathAPK}\\{app}.apk");
                        DateTime now = DateTime.Now;
                        while (!output.Contains("Success"))
                        {
                            Application.DoEvents();
                            Thread.Sleep(50);

                            if ((DateTime.Now - now).TotalSeconds > 30.0)
                            {
                                break;
                            }
                        }

                        if (output.Contains("Success"))
                        {
                            Adb.ShellSu(device.Serial, $"chown -R {app}:{app} /data/data/{app}");
                            Adb.ShellSu(device.Serial, $"chmod -R 0777 /data/data/{app}");

                            Adb.ShellSu(device.Serial, $"chown -R {app}:{app} /data/user_de/0/{app}");
                            Adb.ShellSu(device.Serial, $"chmod -R 0777 /data/user_de/0/{app}/");

                            Adb.ShellSu(device.Serial, $"chown -R {app}:{app} /sdcard/Android/data/{app}");
                            Adb.ShellSu(device.Serial, $"chmod -R 0777 /sdcard/Android/data/{app}/");

                            Adb.ShellSu(device.Serial, $"chown -R {app}:{app} /data/misc/profiles/cur/0/{app}");
                            Adb.ShellSu(device.Serial, $"chmod -R 0777 /data/misc/profiles/cur/0/{app}/");

                            Adb.ShellSu(device.Serial, $"chown -R {app}:{app} /data/misc/profiles/ref/{app}");
                            Adb.ShellSu(device.Serial, $"chmod -R 0777 /data/misc/profiles/ref/{app}/");

                            Common.SetStatus(device.Serial, $"The {app} Restored.");
                        }
                        else
                        {
                            // Installation failed
                            Common.SetStatus(device.Serial, $"The {app} Restored failed.");
                            return false;
                        }
                    }
                }
                else
                {
                    Common.SetStatus(device.Serial, $"Restoring: {bk_packages}");

                    string output = Adb.RunCmd(device.Serial, $"install {pathAPK}\\{bk_packages}.apk");
                    DateTime now = DateTime.Now;
                    while (!output.Contains("Success"))
                    {
                        Application.DoEvents();
                        Thread.Sleep(50);

                        if ((DateTime.Now - now).TotalSeconds > 30.0)
                        {
                            break;
                        }
                    }

                    if (output.Contains("Success"))
                    {
                        Adb.ShellSu(device.Serial, $"chown -R {bk_packages}:{bk_packages} /data/data/{bk_packages}");
                        Adb.ShellSu(device.Serial, $"chmod -R 0777 /data/data/{bk_packages}");

                        Adb.ShellSu(device.Serial, $"chown -R {bk_packages}:{bk_packages} /data/user_de/0/{bk_packages}");
                        Adb.ShellSu(device.Serial, $"chmod -R 0777 /data/user_de/0/{bk_packages}/");

                        Adb.ShellSu(device.Serial, $"chown -R {bk_packages}:{bk_packages} /sdcard/Android/data/{bk_packages}");
                        Adb.ShellSu(device.Serial, $"chmod -R 0777 /sdcard/Android/data/{bk_packages}/");

                        Adb.ShellSu(device.Serial, $"chown -R {bk_packages}:{bk_packages} /data/misc/profiles/cur/0/{bk_packages}");
                        Adb.ShellSu(device.Serial, $"chmod -R 0777 /data/misc/profiles/cur/0/{bk_packages}/");

                        Adb.ShellSu(device.Serial, $"chown -R {bk_packages}:{bk_packages} /data/misc/profiles/ref/{bk_packages}");
                        Adb.ShellSu(device.Serial, $"chmod -R 0777 /data/misc/profiles/ref/{bk_packages}/");

                        Common.SetStatus(device.Serial, $"The {bk_packages} Restored.");
                    }
                    else
                    {
                        // Installation failed
                        Common.SetStatus(device.Serial, $"The {bk_packages} Restored failed.");
                        return false;
                    }
                }
            }

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
