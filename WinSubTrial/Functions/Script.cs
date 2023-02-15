using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Utils;
using WinSubTrial.Functions;
using WinSubTrial.Utilities;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;

namespace WinSubTrial
{
    public class Script
    {
        public string Name { get; set; }
        public string Content { get; set; }

        private string serial;
        public Gmail gmail { get; set; }
        public string appPackage { get; set; }

        public TaskResult pastePasswordOk { get; set; }


    public Script(string serial, string name)
        {
            this.serial = serial;
            Name = name;
            Read();
        }
        public void Read()
        {
            Content = MyFile.ReadAllText($@"C:\WINALL\winscript\{Name}");
        }

        public bool Run()
        {
            bool result = true;
            if (Content.Trim().Equals(string.Empty))
            {
                Common.SetStatus(serial, $"[Script {Name}]Lỗi không có lệnh nào để chạy");
                return false;
            }

            string[] commands = Content.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            int total = commands.Length;
            dynamic cmdResult;
            for (int i = 0; i < total; i++)
            {
                cmdResult = RunCommand(i + 1, total, commands[i]);
                if (cmdResult is bool)
                {
                    if (!cmdResult)
                    {
                        Common.SetStatus(serial, $"[Script {Name}]Thất bại");
                        result = false;
                        break;
                    }
                }
                else if (((string)cmdResult).Contains("replaced"))
                {
                    result = ((string)cmdResult).Contains("True") ? true : false;
                    break;
                } else if (cmdResult is string)
                {
                    return false;
                }
            }
            Common.SetStatus(serial, $"[Script {Name}]" + (result? "Hoàn thành!":"Thất bại"));
            return result;
        }

        private dynamic RunCommand(int current, int total, string command)
        {
            Random random = new Random();
            string[] aloneCommands = "Back,Enter,Tab,Home,UnRoot,OpenMonitor,OpenOfferLink,SendOfferLink,InstallPackage,FillCaptcha,Paste,ProxyOn,ProxyOff,Reboot,RestoreData,Tab,RandomRealEmail,ChangeSock,ChangeIP,ChangeDevice,StarBuck,Target,FillAccount,FillPassword,FillRecovery,Home,GmailLogOut,GmailLogin,GmailChange".Split(',');
            string cmd;
            string[] param = new string[] { };
            Common.SetStatus(serial, $"[Script {Name}][{current}/{total}]{command}");
            if (!Regex.Match(command, @"\(.*\)").Success)
            {
                if (!Array.Exists(aloneCommands, x => x.Equals(command, StringComparison.OrdinalIgnoreCase)))
                {
                    Common.SetStatus(serial, $"[Script {Name}][{current}/{total}]{command} => lỗi định dạng lệnh! Hãy đảm bảo lệnh đúng dạng: Command=>tham số");
                    return false;
                }
                else
                {
                    cmd = command;
                }
            }
            else
            {
                cmd = command.Substring(0, command.IndexOf("("));
                param = command.Replace($"{cmd}(", string.Empty).TrimEnd(';').TrimEnd(')').Split('|');
            }

            switch (cmd.ToLower())
            {
                #region Touch
                case "find_touch":
                    if (!ClickText(param)) return false;
                    break;
                case "find_exactly_touch":
                    if (!ClickText(param, findExactly: true)) return false;
                    break;
                case "touch":
                    Adb.Shell(serial, $"input tap {string.Join(" ", param)}");
                    break;
                case "random_touch":
                    TouchRandom(param);
                    break;
                case "swipe":
                    Adb.Shell(serial, $"input swipe {string.Join(" ", param)}");
                    break;
                #endregion Touch

                #region Text
                case "send_text":
                    Adb.TextSend(serial, param[0]);
                    break;
                case "send_random_text":
                    Adb.TextSend(serial, RandomText(param));
                    break;
                case "send_random_text_from_file":
                    Adb.TextSend(serial, RandomTextFromFile(param));
                    break;
                case "send_loop_text_from_file":
                    Adb.TextSend(serial, LoopTextFromFile(param));
                    break;
                #endregion Text

                #region Key
                case "jump_to_end":
                    Adb.SendKey(serial, "KEYCODE_MOVE_END", int.Parse(param[0]));
                    break;
                case "back_space":
                    Adb.Shell(serial, "KEYCODE_DEL", int.Parse(param[0]));
                    break;
                case "tab":
                    Adb.SendKey(serial, "KEYCODE_TAB", int.Parse(param[0]));
                    break;
                case "space":
                    Adb.SendKey(serial, "KEYCODE_SPACE", int.Parse(param[0]));
                    break;
                case "up":
                    Adb.SendKey(serial, "KEYCODE_DPAD_UP", int.Parse(param[0]));
                    break;
                case "left":
                    Adb.SendKey(serial, "KEYCODE_DPAD_LEFT", int.Parse(param[0]));
                    break;
                case "right":
                    Adb.SendKey(serial, "KEYCODE_DPAD_RIGHT", int.Parse(param[0]));
                    break;
                case "down":
                    Adb.SendKey(serial, "KEYCODE_DPAD_DOWN", int.Parse(param[0]));
                    break;
                case "recent":
                    Adb.SendKey(serial, "KEYCODE_APP_SWITCH");
                    break;
                case "home":
                    Adb.SendKey(serial, "KEYCODE_HOME");
                    break;
                case "back":
                    Adb.SendKey(serial, "KEYCODE_BACK");
                    break;
                #endregion Key

                #region Package
                case "open_package":
                    Adb.OpenApp(serial, param[0]);
                    break;
                case "close_package":
                    Adb.CloseApp(serial, param[0]);
                    break;
                case "wipe_package":
                    Adb.WipeApp(serial, param[0]);
                    break;
                case "install_package_from_pc":
                    Adb.AppInstall(serial, param[0]);
                    break;
                case "install_package_from_device":
                    Adb.AppInstallRemote(serial, param[0]);
                    break;
                case "unistall_package":
                    Adb.UninstallApp(serial, param[0]);
                    break;
                case "disable_package":
                    Adb.DisableApp(serial, param[0]);
                    break;
                case "enable_package":
                    Adb.EnableApp(serial, param[0]);
                    break;
                #endregion Package

                #region Other
                case "sleep":
                    Common.Sleep((int)Math.Round(double.Parse(param[0]) * 1000, 1));
                    break;
                case "random_sleep":
                    Common.Sleep(random.Next(int.Parse(param[0]), int.Parse(param[1])) * 1000);
                    break;
                case "off_proxy":
                    Funcs.ProxyOff(serial);
                    break;
                case "on_proxy_from_file":
                    OnProxy(serial, param);
                    break;
                case "open_link":
                    Adb.OpenUrl(serial, param[0]);
                    break;
                case "open_link_random_from_file":
                    Adb.OpenUrl(serial, RandomTextFromFile(param));
                    break;
                case "find":
                    return RunScriptByText(serial, param);
                case "find_exactly":
                    return RunScriptByText(serial, param, findExactly: true);
                case "clear_mail":
                    new Change { device = new Device { Serial = serial } }.WipeApps();
                    Funcs.WipeMail(serial);
                    Adb.RebootFast(serial);
                    break;
                case "login_mail_random_from_file":
                    LoginMail(serial, param);
                    break;
                case "paste_password":
                    TaskResult resultPaste = new PastePasswordTask { gmail = gmail, appPackage = appPackage }.PasteTask(serial);
                    switch (resultPaste)
                    {
                        case TaskResult.StopAuto:
                            Common.SetStatus(serial, "Stop Paste password");
                            break;
                        case TaskResult.Failure:
                            Common.SetStatus(serial, $"Paste Password {gmail.Password} fail");
                            break;
                        case TaskResult.Success:
                            Common.SetStatus(serial, $"Paste Password {gmail.Password} done");
                            break;
                    }
                    pastePasswordOk = resultPaste;
                    break;
                case "find_apion_touch":
                    bool exactlyContentTouch = new FindExactlyContentAndTouch { content = param[0], content2 = param[1], appPackage = appPackage }.TouchExactly(serial);
                    if (!exactlyContentTouch)
                    {
                        Common.SetStatus(serial, $"Touch Exactly  fail");
                    }
                    Common.SetStatus(serial, $"Touch Exactly done");
                    break;
                case "install_apion_app":
                    appPackage = param[0];
                    TaskResult installAppOk = new InstallAppTask { appPackage = param[0], gmail = gmail }.InstallApp(serial);
                    if (installAppOk == TaskResult.Success)
                    {
                        Common.SetStatus(serial, "Install App Task done");
                        break;
                    }
                    else
                    {
                        Common.SetStatus(serial, "Install App Task fail");
                        return "failed";
                    }
                case "touch_position_apion":
                    bool touchPosition = new TouchPositionTask { point1 = new Point(int.Parse(param[0]), int.Parse(param[1])),
                        point2 = new Point(int.Parse(param[2]), int.Parse(param[3])),
                        point3 = new Point(int.Parse(param[4]), int.Parse(param[5])), 
                        appPackage = appPackage }.TouchPosition(serial);

                    if (!touchPosition)
                    {
                        Common.SetStatus(serial, $"Touch Exactly fail");
                    }
                    Common.SetStatus(serial, $"Touch Exactly done");
                    break;
                #endregion Other

                default:
                    Common.SetStatus(serial, $"[Script {Name}][{current}/{total}]{command} => chưa hỗ trợ lệnh {cmd}");
                    Common.Info($"[Script {Name}][{current}/{total}]{command} => chưa hỗ trợ lệnh {cmd}");
                    break;
            }
            return true;
        }

        private string LoopTextFromFile(string[] param)
        {
            string filePath = param[0];
            bool save = bool.Parse(param[2]);
            string text = MyFile.GetRandomLine(filePath, remove: true);
            if (save) MyFile.WriteAllText(filePath + "used", text, true);
            return text;
        }

        private void OnProxy(string serial, string[] param)
        {
            string filePath = param[0];
            bool remove = bool.Parse(param[1]);
            bool save = bool.Parse(param[2]);
            string[] proxyInfo = MyFile.GetLine(filePath, 1, remove).Split('|');
            if (save) MyFile.WriteAllText(@"C:\WINALL\winsave\proxyUsed.txt", string.Join("|", proxyInfo));
            Funcs.ProxyOn(serial, proxyInfo[0], proxyInfo[1], "socks5", "192.168.1.1", "8.8.8.8");
        }

        private void LoginMail(string serial, string[] param)
        {
            string filePath = param[0];
            bool remove = bool.Parse(param[1]);
            bool save = bool.Parse(param[2]);
            string[] gmailInfo = MyFile.GetLine(filePath, 1, remove).Split('|');
            if (save) MyFile.WriteAllText(filePath + "used", string.Join("|", gmailInfo), true);
            Gmail gmail = new Gmail { Mail = gmailInfo[0], Password = gmailInfo[1], Recovery = gmailInfo[2] };
            new GmailTask { gmail = gmail }.Login(serial);
        }

        private void TouchRandom(string[] param)
        {
            if (param.Length < 8)
            {
                Common.SetStatus(serial, "Thiếu tham số!");
                return;
            }
            try
            {
                int.TryParse(param[0], out int x1);
                int.TryParse(param[1], out int y1);
                int.TryParse(param[2], out int x2);
                int.TryParse(param[3], out int y2);
                int.TryParse(param[4], out int timesMin);
                int.TryParse(param[5], out int timesMax);
                int.TryParse(param[6], out int delayMin);
                int.TryParse(param[7], out int delayMax);
                Random random = new Random();
                int times = random.Next(timesMin, timesMax);
                int delay;
                while (times > 0)
                {
                    Common.SetStatus(serial, $"Times {times}...");
                    delay = random.Next(delayMin, delayMax);
                    Adb.Shell(serial, "input tap " + random.Next(x1, x2) + " " + random.Next(y1, y2));
                    Common.Sleep(delay * 1000);
                    times -= delay;
                }
            }
            catch (Exception ex)
            {
                Common.SetStatus(serial, $"TouchRandom error: {ex.Message}");
            }
        }

        private dynamic RunScriptByText(string serial, string[] param, bool findExactly = false)
        {
            int timeout = int.Parse(param[1]);
            while (timeout > 0)
            {
                if (Adb.GetPointFromUi(param[0], Adb.DumpUi(serial), findExactly) != null) break;
            }

            if (timeout <= 0) return false;

            return RunScript(serial, new string[] { param[2] });
        }

        private dynamic RunScript(string serial, string[] param)
        {
            return new Script(serial, param[0]).Run().ToString();
        }

        private string RandomText(string[] param)
        {
            int max = int.MaxValue;
            bool lower = false;
            int.TryParse(param[0], out int min);
            if (param.Length > 1) int.TryParse(param[1], out max);
            if (param.Length > 2) bool.TryParse(param[2], out lower);
            string text = MyString.RandomLetter(min, max, lower);
            return text;
        }

        private string RandomTextFromFile(string[] param)
        {
            string filePath = param[0];
            bool remove = bool.Parse(param[1]);
            bool save = bool.Parse(param[2]);
            string text = MyFile.GetRandomLine(filePath, remove);
            if (save) MyFile.WriteAllText(filePath + "saved", text, true);
            return text;
        }

        private bool ClickText(string[] param, bool findExactly = false)
        {
            int timeout = int.Parse(param[1]);
            while (timeout > 0)
            {
                Debug.WriteLine($"Time out: {timeout}");
                Debug.WriteLine($"Content touch : {param[0]}");
                timeout--;
                if (Adb.TapDynamic(serial, param[0], findExactly)) return true;
                Common.Sleep(1000);
            }
            return false;
        }

    }
}
