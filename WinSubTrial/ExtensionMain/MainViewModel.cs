using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SharpAdbClient;
using Utils;
using WinSubTrial.Functions;
using WinSubTrial.Utilities;

namespace WinSubTrial
{
    public class MainViewModel
    {
        public Dictionary<string, Task> deviceTasks = new Dictionary<string, Task>();
        public Dictionary<string, Thread> deviceThreads = new Dictionary<string, Thread>();
        public Dictionary<string, bool> deviceWaitForStop = new Dictionary<string, bool>();
        public Thread threadLoadBackup;
        public Thread threadMonitor;
        public List<Network> networkList = new List<Network>();
        public List<Device> devicesModel = new List<Device>();
        public List<String> listCountry = new List<String>();
        public List<String> operatorNetWork = new List<String>();
        public string[] listSdk = { "11", "10", "9", "8.1", "8.0" };

        public string countryChosen = "VN";
        public string operatorChosen = "Viettel Mobile";
        public string brand = "Random";
        public string[] listBrand = { "Random", "google", "LGE", "samsung", "Sony" };
        public string[] listOperator = { "EVNTelecom", "Gmobile", "I-Telecom", "MobiFone", "S-Fone", "Vietnammobile", "Vinaphone", "Viettel Telecom" };

        public DeviceType deviceType = DeviceType.miA1;

        public bool isGetDevice = false;
        public bool isLoaded = false;
        private readonly Random _rand = new Random();

        //số lần reboot
        private const int constTimesChanged = 3;

        public void fetch()
        {
            networkList = JsonConvert.DeserializeObject<List<Network>>(MyFile.ReadAllText("Data\\networks.json"));
            listCountry = (from Network network in networkList select network.code).ToList();
            operatorNetWork = (from Carrier carrier in networkList.First(x => x.code.Equals(countryChosen)).operators select carrier.name).ToList();
            Utils.Debug.Log("Country count: " + Common.countryList.Count + "");
        }

        public void LoginGmail(string serial)
        {
            if (IsDeviceInTask(serial))
            {
                deviceThreads[serial].Abort();
            }
            Thread thread = new Thread(new ThreadStart(() =>
            {
                Device device = devicesModel.FirstOrDefault(x => x.Serial.Equals(serial));
                int indexDevice = devicesModel.FindIndex(element => element.Serial == serial);
                int times = 10000000;
                while (times > 0)
                {
                    #region Change Info
                    if (device == null)
                    {
                        Common.SetStatus(serial, "Not found this device, stop auto");
                        return;
                    }
                    if (device.RequestOption == null) device.RequestOption = new RequestInfo();
                    device.RequestOption.Brand = brand;
                    device.RequestOption.SDK = listSdk[Rand.Next(0, listSdk.Count() - 1)];
                    device.RequestOption.Country = countryChosen;
                    device.RequestOption.Network = operatorChosen;
                    device.GetInfo();
                    #endregion Change Info

                    #region RebootFast
                    if (device.RequestOption == null)
                    {
                        Common.SetStatus(serial, "Info không thay đổi");
                        return;
                    }
                    Common.SetStatus(serial, "Saving info...");
                    new ChangeA1 { device = device }.SaveInfo();
                    Common.SetStatus(serial, "Saving info done");
                    device.RequestOption = null;
                    #endregion RebootFast

                    #region LoginMail
                    Common.SetStatus(serial, $"In queue get gmail after {indexDevice} seconds");
                    Common.Sleep(Rand.Next(1000 * indexDevice, (1000 * indexDevice) + 1000));
                    Common.Sleep(Rand.Next(1000, 9999));
                    Gmail gmail = GetRandomMail();
                    if (gmail == null)
                    {
                        Common.SetStatus(serial, "Login fail. Out of gmail");
                        return;
                    }
                    Common.SetStatus(serial, $"Login {gmail.Mail} gmail...");
                    TaskResult loginGmailResponse = new GmailTask { gmail = gmail }.Login(serial);
                    if (loginGmailResponse == TaskResult.Success)
                    {
                        Common.SetStatus(serial, $"Login {gmail.Mail} done");
                    }
                    else if (loginGmailResponse == TaskResult.StopAuto)
                    {
                        return;
                    }
                    else
                    {
                        Common.SetStatus(serial, $"Login {gmail.Mail} fail, run other gmail!");
                        continue;
                    }
                    #endregion LoginMail

                    #region AddPaymentMethod
                    TaskResult addingPaymentOk = new AddingPaymentTask { gmail = gmail }.AddPaymentMethod(serial);
                    if (addingPaymentOk == TaskResult.Success)
                    {
                        Common.SetStatus(serial, $"Adding payment {gmail.Mail} done");
                    }
                    else if (addingPaymentOk == TaskResult.StopAuto)
                    {
                        return;
                    }
                    else
                    {
                        Common.SetStatus(serial, $"Adding payment fail, run other gmail!");
                        continue;
                    }
                    #endregion AddPaymentMethod

                    #region RunScriptTask
                    RunScript runScript = new RunScript();
                    string[] scripts = (from string file in Directory.EnumerateFiles(@"C:\WINALL\winscript") select (Path.GetFileName(file))).ToArray();
                    runScript.ScriptFile = scripts[Rand.Next(0, scripts.Length)];
                    runScript.RunTimes = 1;
                    runScript.IsRun = true;
                    Script script = new Script(serial, runScript.ScriptFile);
                    script.gmail = gmail;
                    bool resultRunScript = script.Run();
                    if (resultRunScript == false)
                    {
                        Common.SetStatus(serial, "Run Script failed, run other gmail");
                        continue;
                    }
                    Common.SetStatus(serial, "Run Script done");
                    Common.Sleep(2000);
                    #endregion RunScriptTask

                    #region RemoveAccountTask
                    bool removeAccountOk = new RemoveAccountTask { gmail = gmail, mailTask = MailTask.loginMail, pastePasswordOk = script.pastePasswordOk }.RemoveAccount(serial);
                    if (!removeAccountOk)
                    {
                        Common.SetStatus(serial, $"Remove Account {gmail.Mail} Task fail");
                        return;
                    }
                    Common.SetStatus(serial, $"Remove Account {gmail.Mail} Task done");
                    Common.Sleep(5000);
                    times -= 1;
                    if (deviceWaitForStop[serial] == true)
                    {
                        return;
                    }
                    #endregion RemoveAccountTask
                }

            }))
            { IsBackground = true };
            deviceThreads[serial] = thread;
            thread.Start();
        }

        public void CreateMail(string serial)
        {
            if (IsDeviceInTask(serial))
            {
                deviceThreads[serial].Abort();
            }
            Thread thread = new Thread(new ThreadStart(() =>
            {
                Device device = devicesModel.FirstOrDefault(x => x.Serial.Equals(serial));
                int times = 10000000;

                while (times > 0)
                {
                    #region Change Info
                    if (device == null)
                    {
                        Common.SetStatus(serial, "Not found this device, stop auto");
                        return;
                    }
                    if (device.RequestOption == null) device.RequestOption = new RequestInfo();
                    device.RequestOption.Brand = brand;
                    device.RequestOption.SDK = listSdk[Rand.Next(0, listSdk.Count() - 1)];
                    device.RequestOption.Country = countryChosen;
                    device.RequestOption.Network = operatorChosen;
                    device.GetInfo();
                    #endregion Change Info

                    #region RebootFast
                    if (device.RequestOption == null)
                    {
                        Common.SetStatus(device.Serial, "Info không thay đổi");
                        return;
                    }
                    Common.SetStatus(device.Serial, "Saving info...");
                    new ChangeA1 { device = device }.SaveInfo();

                    Common.SetStatus(device.Serial, "Saving info done");
                    //Remove changed info
                    device.RequestOption = null;
                    #endregion RebootFast

                    #region CreateMailRegion
                    CreateMailTask createMailTask = new CreateMailTask { };
                    TaskResult resultOK = createMailTask.CreateMail(serial);
                    if (resultOK == TaskResult.Success)
                    {
                        Common.SetStatus(serial, $"Create Mail {createMailTask.gmail.Mail} done");
                    }
                    else if (resultOK == TaskResult.StopAuto)
                    {
                        Common.SetStatus(serial, $"Create Mail {createMailTask.gmail.Mail} Stopped");
                        return;
                    }
                    else
                    {
                        Common.SetStatus(serial, $"Create Mail Failure, restart device");
                        continue;
                    }
                    Gmail gmail = createMailTask.gmail;
                    #endregion CreateMailRegion


                    #region RemoveAccountTask
                    bool removeAccountOk = new RemoveAccountTask { gmail = gmail, mailTask = MailTask.createMail }.RemoveAccount(serial);
                    if (!removeAccountOk)
                    {
                        Common.SetStatus(serial, $"Remove Account {gmail.Mail} Task fail");
                        return;
                    }
                    Common.SetStatus(serial, $"Remove Account {gmail.Mail} Task done");
                    #endregion RemoveAccountTask

                    times -= 1;
                    if (deviceWaitForStop[serial] == true)
                    {
                        return;
                    }
                    #region AddPaymentMethod
                    /*
                    #region AddPaymentMethod
                    TaskResult addingPaymentOk = new AddingPaymentTask { gmail = gmail }.AddPaymentMethod(serial);
                    if (addingPaymentOk == TaskResult.Success)
                    {
                        Common.SetStatus(serial, $"Adding payment {gmail.Mail} done");
                    }
                    else if (addingPaymentOk == TaskResult.StopAuto)
                    {
                        return;
                    }
                    else
                    {
                        Common.SetStatus(serial, $"Adding payment fail, create other gmail!");
                        continue;
                    }
                    #endregion AddPaymentMethod

                    #region RunScriptTask
                    RunScript runScript = new RunScript();
                    string[] scripts = (from string file in Directory.EnumerateFiles(@"C:\WINALL\winscript") select (Path.GetFileName(file))).ToArray();
                    runScript.ScriptFile = scripts[Rand.Next(0, scripts.Length)];
                    runScript.RunTimes = 1;
                    runScript.IsRun = true;
                    Script script = new Script(serial, runScript.ScriptFile);
                    script.gmail = gmail;
                    script.Run();
                    Common.SetStatus(serial, "Run Script done");
                    Common.Sleep(2000);
                    #endregion RunScriptTask
                    */
                    #endregion AddPaymentMethod
                }
            }))
            { IsBackground = true };
            deviceThreads[serial] = thread;
            thread.Start();
        }

        public void LoginAll(string serial)
        {
            if (IsDeviceInTask(serial))
            {
                deviceThreads[serial].Abort();
            }
            Thread thread = new Thread(new ThreadStart(() =>
            {
                Device device = devicesModel.FirstOrDefault(x => x.Serial.Equals(serial));
                int indexDevice = devicesModel.FindIndex(element => element.Serial == serial);
                int times = 10000000;

                while (times > 0)
                {
                    #region Change Info
                    if (device == null)
                    {
                        Common.SetStatus(serial, "Not found this device, stop auto");
                        return;
                    }
                    if (device.RequestOption == null) device.RequestOption = new RequestInfo();
                    device.RequestOption.Brand = brand;
                    device.RequestOption.SDK = listSdk[Rand.Next(0, listSdk.Count() - 1)];
                    device.RequestOption.Country = countryChosen;
                    device.RequestOption.Network = operatorChosen;

                    device.GetInfo();
                    #endregion Change Info

                    #region RebootFast
                    if (device.RequestOption == null)
                    {
                        Common.SetStatus(device.Serial, "Info không thay đổi");
                        return;
                    }
                    Common.SetStatus(device.Serial, "Saving info...");
                    new ChangeA1 { device = device }.SaveInfo();

                    Common.SetStatus(device.Serial, "Saving info done");
                    //Remove changed info
                    device.RequestOption = null;
                    #endregion RebootFast

                    #region LoginMail
                    Common.SetStatus(serial, $"In queue get gmail after {indexDevice} seconds");

                    Common.Sleep(Rand.Next(1000 * indexDevice, (1000 * indexDevice) + 1000));
                    Common.Sleep(Rand.Next(1000, 9999));

                    Gmail gmail = GetRandomMail();
                    Common.SetStatus(serial, "Get gmail from file done");
                    if (gmail == null)
                    {
                        Common.SetStatus(serial, "Login fail. Out of gmail");
                        return;
                    }
                    Common.SetStatus(serial, $"Login {gmail.Mail} gmail...");
                    TaskResult loginGmailResponse = new GmailTask { gmail = gmail }.Login(serial);
                    if (loginGmailResponse == TaskResult.Success)
                    {
                        Common.SetStatus(serial, $"Login {gmail.Mail} done");
                    }
                    else if (loginGmailResponse == TaskResult.StopAuto)
                    {
                        return;
                    }
                    else
                    {
                        Common.SetStatus(serial, $"Login {gmail.Mail} fail, restart device!");
                        continue;
                    }
                    #endregion LoginMail

                    Funcs.OpenPaymentMethods(serial);
                    string gmailCompleted = $"{gmail.Mail}|{gmail.Password}|{gmail.Recovery}    Login only";

                    MyFile.WriteAllText("Data\\gmailCompletelySubscribed.txt", gmailCompleted, true, "\r\n");
                    Common.SetStatus(serial, "Saved to Completely Subscribed done");
                    Common.Sleep(Rand.Next(2000, 19000));
                    times -= 1;
                    if (deviceWaitForStop[serial] == true)
                    {
                        return;
                    }
                }

            }))
            { IsBackground = true };
            deviceThreads[serial] = thread;
            thread.Start();
        }

        public void SaveInfoAll(string serial)
        {
            if (IsDeviceInTask(serial))
            {
                deviceThreads[serial].Abort();
            }
            Thread thread = new Thread(new ThreadStart(() =>
            {
                #region Change Info
                Device device = devicesModel.FirstOrDefault(x => x.Serial.Equals(serial));
                if (device == null)
                {
                    Common.SetStatus(serial, "Not found this device, stop auto");
                    return;
                }
                if (device.RequestOption == null) device.RequestOption = new RequestInfo();
                device.RequestOption.Brand = brand;
                device.RequestOption.SDK = listSdk[Rand.Next(0, listSdk.Count() - 1)];
                device.RequestOption.Country = countryChosen;
                device.RequestOption.Network = operatorChosen;

                device.GetInfo();
                #endregion Change Info

                #region RebootFast
                if (device.RequestOption == null)
                {
                    Common.SetStatus(device.Serial, "Info không thay đổi");
                    return;
                }
                Common.SetStatus(device.Serial, "Saving info...");
                new ChangeA1 { device = device }.SaveInfo();

                Common.SetStatus(device.Serial, "Saving info done");
                //Remove changed info
                device.RequestOption = null;
                #endregion RebootFast

            }))
            { IsBackground = true };
            deviceThreads[serial] = thread;
            thread.Start();
        }

        public void AutoClickTac(string serial)
        {
            if (IsDeviceInTask(serial))
            {
                deviceThreads[serial].Abort();
            }
            Thread thread = new Thread(new ThreadStart(() =>
            {
                Device device = devicesModel.FirstOrDefault(x => x.Serial.Equals(serial));
                int indexDevice = devicesModel.FindIndex(element => element.Serial == serial);
                int times = 10000000;
                while (times > 0)
                {
                    #region Change Info
                    if (device == null)
                    {
                        Common.SetStatus(serial, "Not found this device, stop auto");
                        return;
                    }
                    if (device.RequestOption == null) device.RequestOption = new RequestInfo();
                    device.RequestOption.Brand = brand;
                    device.RequestOption.SDK = listSdk[Rand.Next(0, listSdk.Count() - 1)];
                    device.RequestOption.Country = countryChosen;
                    device.RequestOption.Network = operatorChosen;
                    device.GetInfo();
                    #endregion Change Info

                    #region RebootFast
                    if (device.RequestOption == null)
                    {
                        Common.SetStatus(device.Serial, "Info không thay đổi");
                        return;
                    }
                    Common.SetStatus(device.Serial, "Saving info...");
                    new ChangeA1 { device = device }.SaveInfo();

                    Common.SetStatus(device.Serial, "Saving info done");
                    //Remove changed info
                    device.RequestOption = null;
                    #endregion RebootFast

                    #region Autoclicktac
                    TaskResult autoClickTacResponse = new AutoClickTacTask { }.ClickLink(serial);
                    if (autoClickTacResponse == TaskResult.Success)
                    {
                        Common.SetStatus(serial, "click tac done");
                    }
                    else if (autoClickTacResponse == TaskResult.StopAuto)
                    {
                        return;
                    }
                    else
                    {
                        Common.SetStatus(serial, "Click tac fail, tiep tuc mo lai chrome!");
                        continue;
                    }
                    #endregion Autoclicktac
                }

            }))
            { IsBackground = true };
            deviceThreads[serial] = thread;
            thread.Start();
        }

        public void SubscribeYoutube(string serial)
        {
            if (IsDeviceInTask(serial))
            {
                deviceThreads[serial].Abort();
            }
            Thread thread = new Thread(new ThreadStart(() =>
            {
                Device device = devicesModel.FirstOrDefault(x => x.Serial.Equals(serial));
                int indexDevice = devicesModel.FindIndex(element => element.Serial == serial);
                int times = 10000000;
                while (times > 0)
                {
                    #region SubscribeYoutube
                    TaskResult subscribeYoutubeResponse = new SubscribeYoutubeTask { }.LikeAndSubscribe(serial);
                    if (subscribeYoutubeResponse == TaskResult.Success)
                    {
                        Common.SetStatus(serial, "Subscribe done");
                    }
                    else if (subscribeYoutubeResponse == TaskResult.StopAuto)
                    {
                        return;
                    }
                    else
                    {
                        Common.SetStatus(serial, "Subscribe fail, tiep tuc mo lai youtube!");
                        continue;
                    }
                    #endregion SubscribeYoutube
                }
            }))
            { IsBackground = true };
            deviceThreads[serial] = thread;
            thread.Start();
        }

        public void TiktokApp(string serial)
        {
            if (IsDeviceInTask(serial))
            {
                deviceThreads[serial].Abort();
            }
            Thread thread = new Thread(new ThreadStart(() =>
            {
                Device device = devicesModel.FirstOrDefault(x => x.Serial.Equals(serial));
                int indexDevice = devicesModel.FindIndex(element => element.Serial == serial);
                int times = 10000000;
                while (times > 0)
                {

                    #region Change Info
                    if (device == null)
                    {
                        Common.SetStatus(serial, "Not found this device, stop auto");
                        return;
                    }
                    if (device.RequestOption == null) device.RequestOption = new RequestInfo();
                    device.RequestOption.Brand = brand;
                    device.RequestOption.SDK = listSdk[Rand.Next(0, listSdk.Count() - 1)];
                    device.RequestOption.Country = countryChosen;
                    device.RequestOption.Network = operatorChosen;
                    device.GetInfo();
                    #endregion Change Info

                    #region RebootFast
                    if (device.RequestOption == null)
                    {
                        Common.SetStatus(serial, "Info không thay đổi");
                        return;
                    }
                    Common.SetStatus(serial, "Saving info...");
                    new ChangeA1 { device = device }.SaveInfo();
                    Common.SetStatus(serial, "Saving info done");
                    device.RequestOption = null;
                    #endregion RebootFast



                    //Adb.Shell(serial, "pm dump com.ss.android.ugc.trill | grep -A 1 MAIN");

                    #region GetTikTok
                    Common.SetStatus(serial, $"In queue get tiktok after {indexDevice} seconds");
                    Common.Sleep(Rand.Next(1000 * indexDevice, (1000 * indexDevice) + 1000));
                    Common.Sleep(Rand.Next(1000, 9999));
                    string numberTiktok = GetRandomTiktokNumber();
                    Common.SetStatus(serial, $"Get Tiktok from file done: {numberTiktok}");
                    Common.Sleep(4000);
                    if (numberTiktok == null)
                    {
                        Common.SetStatus(serial, "Call API fail. Out of number");
                        Common.Sleep(4000);
                        return;
                    }
                    TaskResult enableAPI = new TikTokTask { phonenumber = numberTiktok }.LoginTiktok(serial);
                    if (enableAPI == TaskResult.Success)
                    {
                        Common.SetStatus(serial, $"Login tiktok {numberTiktok} done");
                    }
                    else if (enableAPI == TaskResult.StopAuto)
                    {
                        return;
                    }
                    else
                    {
                        Common.SetStatus(serial, $"Login tiktok {numberTiktok} fail, run other phone!");
                        continue;
                    }
                    #endregion GetTikTok
                }

            }))
            { IsBackground = true };
            deviceThreads[serial] = thread;
            thread.Start();
        }

        public void SnapchatApp(string serial)
        {
            if (IsDeviceInTask(serial))
            {
                deviceThreads[serial].Abort();
            }
            Thread thread = new Thread(new ThreadStart(() =>
            {
                Device device = devicesModel.FirstOrDefault(x => x.Serial.Equals(serial));
                int times = 10000000;
                int timesChanged = constTimesChanged;
                //int timesChanged = 1;

                while (times > 0)
                {
                    if (timesChanged > (constTimesChanged - 1)) // reboot
                    {
                        RebootDevice(serial, device);
                        timesChanged = 0;
                    }
                    else
                    {
                        // wipe app /
                        new ChangeA1 { device = device }.WipeAppsData();
                        timesChanged += 1;
                    }

                    //Adb.Shell(serial, "pm dump com.snapchat.android | grep -A 1 MAIN");

                    string numberSnapchat = GetRandomSnapchatNumber();
                    Common.SetStatus(serial, $"Get Snapchat from file done: {numberSnapchat}");
                    //Common.Sleep(1000);
                    if (numberSnapchat == null)
                    {
                        Common.SetStatus(serial, "Call API fail. Out of number");
                        Common.Sleep(4000);
                        return;
                    }
                    TaskResult snapchatEnable = new SnapchatTask { phonenumber = numberSnapchat }.LoginSnapchat(serial);
                    switch (snapchatEnable)
                    {
                        case TaskResult.Success:
                            {
                                Common.SetStatus(serial, $"Login snapchat {numberSnapchat} done");
                                break;
                            }
                        case TaskResult.StopAuto:
                            {
                                return;
                            }
                        case TaskResult.ProxyError:
                        case TaskResult.OtpError:
                            {
                                timesChanged = constTimesChanged;
                                Common.SetStatus(serial, $"Login snapchat {numberSnapchat} fail, run other phone!");
                                continue;
                            }
                        case TaskResult.Failure:
                            {
                                Common.SetStatus(serial, $"Login snapchat {numberSnapchat} fail, run other phone!");
                                continue;
                            }
                        default:
                            return;
                    }
                }
            }))
            { IsBackground = true };
            deviceThreads[serial] = thread;
            thread.Start();
        }

        public void TinderAutomation(string serial)
        {
            int constTimeChangeTinder = 2;
            if (IsDeviceInTask(serial))
            {
                deviceThreads[serial].Abort();
            }
            Thread thread = new Thread(new ThreadStart(() =>
            {
                Device device = devicesModel.FirstOrDefault(x => x.Serial.Equals(serial));
                int times = 10000000;
                int timesChanged = constTimeChangeTinder;
                //int timesChanged = 1;

                while (times > 0)
                {
                    if (timesChanged >= constTimeChangeTinder) // reboot
                    {
                        //reboot khi quá số lần auto
                        RebootDevice(serial, device);
                        timesChanged = 1;
                    }
                    else
                    {
                        // wipe app /
                        new ChangeA1 { device = device }.WipeAppsData();
                        timesChanged += 1;
                    }

                    string numberTinder = GetRandomTinderNumber();
                    Common.SetStatus(serial, $"Get Tinder from file done: {numberTinder}");
                    //Common.Sleep(1000);
                    if (numberTinder == null)
                    {
                        Common.SetStatus(serial, "Call API fail. Out of number");
                        Common.Sleep(4000);
                        return;
                    }
                    TaskResult tinderResult = new TinderTask { phonenumber = numberTinder }.TinderAutomationRegister(serial);
                    switch (tinderResult)
                    {
                        case TaskResult.Success:
                            {
                                Common.SetStatus(serial, $"Login Tinder {numberTinder} done");
                                break;
                            }
                        case TaskResult.StopAuto:
                            {
                                return;
                            }
                        case TaskResult.ProxyError:
                        case TaskResult.OtpError:
                            {
                                timesChanged = constTimeChangeTinder;
                                Common.SetStatus(serial, $"Login tinder {numberTinder} fail, run other phone!");
                                continue;
                            }
                        case TaskResult.Failure:
                            {
                                Common.SetStatus(serial, $"Login tinder {numberTinder} fail, run other phone!");
                                continue;
                            }
                        default:
                            return;
                    }
                }
            }))
            { IsBackground = true };
            deviceThreads[serial] = thread;
            thread.Start();
        }

        public void BigoAutomation(string serial, bool isSMS = true)
        {
            if (IsDeviceInTask(serial))
            {
                deviceThreads[serial].Abort();
            }
            Thread thread = new Thread(new ThreadStart(() =>
            {
                Device device = devicesModel.FirstOrDefault(x => x.Serial.Equals(serial));
                int times = 10000000;
                int timesChanged = constTimesChanged;

                while (times > 0)
                {
                    if (timesChanged >= constTimesChanged) // reboot
                    {
                        //reboot khi quá số lần auto
                        RebootDevice(serial, device);
                        timesChanged = 1;
                    }
                    else
                    {
                        // wipe app /
                        new ChangeA1 { device = device }.WipeAppsData();
                        timesChanged += 1;
                    }

                    Common.SetStatus(serial, $"Get bigo phonenumber from file done");
                    TaskResult result = new TaskResult();
                    if (isSMS)
                    {
                        result = new BigoSMSTask { }.BigoSMSAuto(serial);
                    }
                    else
                    {
                        result = new BigoRegisterTask { }.BigoAutoRegister(serial);
                    }
                    switch (result)
                    {
                        case TaskResult.Success:
                            {
                                Common.SetStatus(serial, $"Bigo register done");
                                break;
                            }
                        case TaskResult.OtpError:
                            {
                                timesChanged = timesChanged == 1 ? timesChanged : (timesChanged - 1);
                                Common.SetStatus(serial, $"Bigo OTP fail, run other phone!");

                                continue;
                            }
                        case TaskResult.Failure:
                            {
                                Common.SetStatus(serial, $"Bigo register fail, run other phone!");
                                continue;
                            }
                        default:
                            return;
                    }
                }
            }))
            { IsBackground = true };
            deviceThreads[serial] = thread;
            thread.Start();
        }

        public void BigoLiteAutomation(string serial)
        {
            if (IsDeviceInTask(serial))
            {
                deviceThreads[serial].Abort();
            }
            Thread thread = new Thread(new ThreadStart(() =>
            {
                Device device = devicesModel.FirstOrDefault(x => x.Serial.Equals(serial));
                int times = 10000000;
                int timesChanged = constTimesChanged;

                while (times > 0)
                {
                    if (timesChanged >= constTimesChanged) // reboot
                    {
                        //reboot
                        RebootDevice(serial, device);
                        timesChanged = 1;
                    }
                    else
                    {
                        // wipe app /
                        new ChangeA1 { device = device }.WipeAppsData();
                        timesChanged += 1;
                    }

                    TaskResult result = new BigoLiteTask { }.BigoLiteFilterPhone(serial);
                    switch (result)
                    {
                        case TaskResult.Success:
                            {
                                Common.SetStatus(serial, $"Bigo filter phone done");
                                break;
                            }
                        case TaskResult.Failure:
                            {
                                Common.SetStatus(serial, $"Bigo lite fail, run away!");
                                continue;
                            }
                        default:
                            return;
                    }
                }
            }))
            { IsBackground = true };
            deviceThreads[serial] = thread;
            thread.Start();
        }

        public void SnapchatPasswordRetrieval(string serial, string net)
        {
            if (IsDeviceInTask(serial))
            {
                deviceThreads[serial].Abort();
            }
            Thread thread = new Thread(new ThreadStart(() =>
            {
                Device device = devicesModel.FirstOrDefault(x => x.Serial.Equals(serial));
                int times = 10000000;
                int timesChanged = constTimesChanged;
                //int timesChanged = 1;

                while (times > 0)
                {
                    if (timesChanged > (constTimesChanged - 1)) // reboot
                    {
                        //reboot khi quá số lần auto
                        RebootDevice(serial, device);
                        timesChanged = 0;
                    }
                    else
                    {
                        // wipe app /
                        new ChangeA1 { device = device }.WipeAppsData();
                        timesChanged += 1;
                    }

                    TaskResult result = new SnapchatPasswordTask { }.SnapchatPasswordRetrieval(serial, net);
                    switch (result)
                    {
                        case TaskResult.Success:
                            {
                                Common.SetStatus(serial, $"Password snapchat done");
                                break;
                            }
                        case TaskResult.StopAuto:
                            {
                                return;
                            }
                        case TaskResult.OtpError:
                            {
                                timesChanged = constTimesChanged;
                                Common.SetStatus(serial, $"Password snapchat fail, run other phone!");
                                continue;
                            }
                        case TaskResult.Failure:
                            {
                                Common.SetStatus(serial, $"Password snapchat fail, run other phone!");
                                continue;
                            }
                        default:
                            return;
                    }
                }
            }))
            { IsBackground = true };
            deviceThreads[serial] = thread;
            thread.Start();
        }
        public void ChametAutomation(string serial)
        {
            if (IsDeviceInTask(serial))
            {
                deviceThreads[serial].Abort();
            }
            Thread thread = new Thread(new ThreadStart(() =>
            {
                Device device = devicesModel.FirstOrDefault(x => x.Serial.Equals(serial));
                int times = 10000000;
                int timesChanged = constTimesChanged;

                while (times > 0)
                {
                    if (timesChanged > (constTimesChanged - 1)) // reboot
                    {
                        //reboot khi quá số lần auto
                        RebootDevice(serial, device);
                        timesChanged = 0;
                    }
                    else
                    {
                        // wipe app /
                        new ChangeA1 { device = device }.WipeAppsData();
                        timesChanged += 1;
                    }

                    TaskResult result = new ChametTask { }.ChametRegister(serial);
                    switch (result)
                    {
                        case TaskResult.Success:
                            {
                                Common.SetStatus(serial, $"Register chamet succeess");
                                break;
                            }
                        case TaskResult.StopAuto:
                            {
                                return;
                            }
                        case TaskResult.OtpError:
                        case TaskResult.Failure:
                            {
                                Common.SetStatus(serial, $"Register chamet fail, run other phone!");
                                continue;
                            }
                        default:
                            return;
                    }
                }
            }))
            { IsBackground = true };
            deviceThreads[serial] = thread;
            thread.Start();
        }
        public void CamScannerAutomation(string serial)
        {
            if (IsDeviceInTask(serial))
            {
                deviceThreads[serial].Abort();
            }
            Thread thread = new Thread(new ThreadStart(() =>
            {
                Device device = devicesModel.FirstOrDefault(x => x.Serial.Equals(serial));
                int times = 10000000;
                int timesChanged = constTimesChanged;

                while (times > 0)
                {
                    if (timesChanged > (constTimesChanged - 1)) // reboot
                    {
                        //reboot khi quá số lần auto
                        RebootDevice(serial, device);
                        timesChanged = 0;
                    }
                    else
                    {
                        // wipe app /
                        new ChangeA1 { device = device }.WipeAppsData();
                        timesChanged += 1;
                    }

                    TaskResult result = new CamScannerTask { }.CamScannerRegister(serial);
                    switch (result)
                    {
                        case TaskResult.Success:
                            {
                                Common.SetStatus(serial, $"Register CamScanner succeess");
                                break;
                            }
                        case TaskResult.StopAuto:
                            {
                                return;
                            }
                        case TaskResult.OtpError:
                        case TaskResult.Failure:
                            {
                                Common.SetStatus(serial, $"Register CamScanner fail, run other phone!");
                                continue;
                            }
                        default:
                            return;
                    }
                }
            }))
            { IsBackground = true };
            deviceThreads[serial] = thread;
            thread.Start();
        }
        public void TelegramRegister(string serial, string net)
        {
            if (IsDeviceInTask(serial))
            {
                deviceThreads[serial].Abort();
            }
            Thread thread = new Thread(new ThreadStart(() =>
            {
                Device device = devicesModel.FirstOrDefault(x => x.Serial.Equals(serial));
                int times = 10000000;
                int timesChanged = constTimesChanged;
                //int timesChanged = 1;

                while (times > 0)
                {
                    if (timesChanged > (constTimesChanged - 1)) // reboot
                    {
                        //reboot khi quá số lần auto
                        //RebootDevice(serial, device);
                        timesChanged = 0;
                    }
                    else
                    {
                        // wipe app /
                        new ChangeA1 { device = device }.WipeAppsData();
                        timesChanged += 1;
                    }

                    TaskResult result = new TelegramTask { }.TelegramRegister(serial, net);
                    switch (result)
                    {
                        case TaskResult.Success:
                            {
                                Common.SetStatus(serial, $"Password snapchat done");
                                break;
                            }
                        case TaskResult.StopAuto:
                            {
                                return;
                            }
                        case TaskResult.OtpError:
                        case TaskResult.Failure:
                            {
                                Common.SetStatus(serial, $"Password snapchat fail, run other phone!");
                                continue;
                            }
                        default:
                            return;
                    }
                }
            }))
            { IsBackground = true };
            deviceThreads[serial] = thread;
            thread.Start();
        }
        public void GlobalSmartRegister(string serial)
        {
            if (IsDeviceInTask(serial))
            {
                deviceThreads[serial].Abort();
            }
            Thread thread = new Thread(new ThreadStart(() =>
            {
                Device device = devicesModel.FirstOrDefault(x => x.Serial.Equals(serial));
                int times = 10000000;
                int timesChanged = constTimesChanged;

                while (times > 0)
                {
                    if (timesChanged > (constTimesChanged - 1)) // reboot
                    {
                        //reboot khi quá số lần auto
                        //RebootDevice(serial, device);
                        timesChanged = 0;
                    }
                    else
                    {
                        // wipe app /
                        new ChangeA1 { device = device }.WipeAppsData();
                        timesChanged += 1;
                    }

                    TaskResult result = new GlobalSmartTask { }.GlobalSmartRegister(serial);
                    switch (result)
                    {
                        case TaskResult.Success:
                            {
                                Common.SetStatus(serial, $"Register global smart done");
                                break;
                            }
                        case TaskResult.StopAuto:
                            {
                                return;
                            }
                        case TaskResult.OtpError:
                        case TaskResult.Failure:
                            {
                                Common.SetStatus(serial, $"Register global smart fail!");
                                continue;
                            }
                        default:
                            return;
                    }
                }
            }))
            { IsBackground = true };
            deviceThreads[serial] = thread;
            thread.Start();
        }
        public void XbankRegister(string serial)
        {
            if (IsDeviceInTask(serial))
            {
                deviceThreads[serial].Abort();
            }
            Thread thread = new Thread(new ThreadStart(() =>
            {
                Device device = devicesModel.FirstOrDefault(x => x.Serial.Equals(serial));
                int times = 10000000;
                int timesChanged = constTimesChanged;

                while (times > 0)
                {
                    if (timesChanged > (constTimesChanged - 1)) // reboot
                    {
                        //reboot khi quá số lần auto
                        RebootDevice(serial, device);
                        timesChanged = 0;
                    }
                    else
                    {
                        // wipe app /
                        new ChangeA1 { device = device }.WipeAppsData();
                        timesChanged += 1;
                    }

                    TaskResult result = new Xbank { }.XbankRegister(serial);
                    switch (result)
                    {
                        case TaskResult.Success:
                            {
                                Common.SetStatus(serial, $"Register xBank done");
                                break;
                            }
                        case TaskResult.StopAuto:
                            {
                                return;
                            }
                        case TaskResult.OtpError:
                        case TaskResult.Failure:
                            {
                                Common.SetStatus(serial, $"Register xBank fail!");
                                continue;
                            }
                        default:
                            return;
                    }
                }
            }))
            { IsBackground = true };
            deviceThreads[serial] = thread;
            thread.Start();
        }
        public void RebootDevice(string serial, Device device)
        {
            #region Change Info
            if (device == null)
            {
                Common.SetStatus(serial, "Not found this device, stop auto");
                return;
            }
            if (device.RequestOption == null) device.RequestOption = new RequestInfo();
            device.RequestOption.Brand = listBrand[Rand.Next(0, listBrand.Length)];
            device.RequestOption.SDK = listSdk[Rand.Next(0, listSdk.Length)];
            device.RequestOption.Country = countryChosen;
            device.RequestOption.Network = listOperator[Rand.Next(0, listOperator.Length)];

            device.GetInfo();
            #endregion Change Info

            #region RebootFast
            if (device.RequestOption == null)
            {
                Common.SetStatus(device.Serial, "Info không thay đổi");
                return;
            }
            Common.SetStatus(device.Serial, "Saving info...");
            new ChangeA1 { device = device }.SaveInfo();

            Common.SetStatus(device.Serial, "Saving info done");
            //Remove changed info
            device.RequestOption = null;
            #endregion RebootFast
        }

        public string UploadImage(string url, Bitmap bmp)
        {
            using (WebClient client = new WebClient())
            using (var ms = new MemoryStream())
            {
                bmp.Save(ms, ImageFormat.Png);
                byte[] result = client.UploadData(url, ms.ToArray());
                return Encoding.UTF8.GetString(result);
            }

        }

        public string RandomPasswordString()
        {
            int length = _rand.Next(12, 14);
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[_rand.Next(s.Length)]).ToArray());
        }

        public bool selectedOneDevice()
        {
            List<Device> array = devicesModel.Where(x => x.isSelected == true).ToList();
            if (array.Count == 0)
            {
                Common.Info("Please select a device");
                return false;
            }
            else if (array.Count > 1)
            {
                Common.Info("Please select only one device");
                return false;
            }
            return true;
        }

        public bool someDevicesSelected()
        {
            List<Device> array = devicesModel.Where(x => x.isSelected == true).ToList();
            if (array.Count == 0)
            {
                Common.Info("Please select a device");
                return false;
            }
            return true;
        }

        public bool IsDeviceInTask(string serial)
        {
            return deviceTasks[serial]?.Status == TaskStatus.Running || deviceThreads[serial]?.IsAlive == true;
        }

        public void RunLastEmailAndStop(string serial)
        {
            if (IsDeviceInTask(serial))
            {
                try
                {
                    deviceWaitForStop[serial] = true;
                    Common.SetStatus(serial, "Waiting for stop");
                }
                catch { }
            }
        }

        public void StopTask(string serial)
        {
            if (IsDeviceInTask(serial))
            {
                try
                {
                    deviceThreads[serial].Abort();
                    Common.SetStatus(serial, "Stopped");
                }
                catch { }
            }
        }

        public Gmail GetRandomMail()
        {
            try
            {
                string[] info = MyFile.GetLine(filePath: "Data\\gmail.txt", index: 1, remove: true).Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                if (info.Length < 3)
                {
                    return new Gmail { Mail = info[0], Password = info[1], Recovery = "norecoveryemail@gmail.com" };
                }
                else
                {
                    //if (info.Length >= 3)
                    return new Gmail { Mail = info[0], Password = info[1], Recovery = info[2] };
                }
            }
            catch { return null; }
        }


        public string GetRandomTiktokNumber()
        {
            try
            {
                string[] info = MyFile.GetLine(filePath: "Data\\tiktok.txt", index: 1, remove: true).Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                return info[0];
            }
            catch { return null; }
        }

        public string GetRandomTinderNumber()
        {
            try
            {
                string[] info = MyFile.GetLine(filePath: "Data\\04-TI1.txt", index: 1, remove: true).Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                return info[0];
            }
            catch { return null; }
        }

        public string GetRandomSnapchatNumber()
        {
            try
            {
                string[] info = MyFile.GetLine(filePath: "Data\\01-SN1IN.txt", index: 1, remove: true).Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                return info[0];
            }
            catch { return null; }
        }


        public Action OpenLinkForm(string serial)
        {
            return () =>
            {
                using (OpenLink ol = new OpenLink(serial))
                {
                    ol.ShowDialog();
                    switch (ol.commands[0].ToString())
                    {
                        case "openurl":
                            Common.SetStatus(serial, "Opening link...");
                            Adb.OpenUrl(serial, ol.commands[1], ol.commands[2]);
                            Common.SetStatus(serial, "Open link done");
                            break;
                        case "checkipapi":
                            Common.SetStatus(serial, "Checking api...");
                            string proxy = default;
                            string resultIpApi = Adb.Shell(serial, $"curl {proxy} http://ip-api.com/line");
                            if (Common.ProxyHost.Length > 1)
                                proxy = $"--socks5 {Common.ProxyHost}:{Common.ProxyPort}";
                            Common.SetStatus(serial, "Check ip api done. See popup window");
                            break;
                        case "openapp":
                            Common.SetStatus(serial, "Opening app...");
                            Adb.OpenApp(serial, "com.android.vending");
                            Common.SetStatus(serial, "Open app done");
                            break;
                        case "wipeapp":
                            Common.SetStatus(serial, "Wiping app...");
                            Adb.WipeApp(serial, ol.commands[1]);
                            Common.SetStatus(serial, "Wiping app done");
                            break;
                        case "proxyon":
                            Common.SetStatus(serial, "Settings proxy...");
                            Common.ProxyHost = ol.commands[1].Split(':')[0];
                            Common.ProxyPort = ol.commands[1].Split(':')[1];
                            Funcs.ProxyOn(serial, Common.ProxyHost, Common.ProxyPort, "socks5", "192.168.1.1", "8.8.8.8");
                            Common.SetStatus(serial, "Set proxy done: " + Common.ProxyHost + ":" + Common.ProxyPort);
                            break;
                    }
                }
            };
        }
    }
}
