using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinSubTrial
{
    public class Settings
    {
        [JsonProperty("random-carrier")]
        public bool RandomCarrier = true;

        [JsonProperty("random-phone")]
        public bool RandomPhone = true;

        [JsonProperty("random-sdk")]
        public bool RandomSDK = true;

        [JsonProperty("bypass-safetynet")]
        public bool BypassSafetyNet = true;

        [JsonProperty("move-to-restore")]
        public bool MoveToRestore = false;

        [JsonProperty("check-ip-vysor")]
        public bool CheckIpVysor = true;

        [JsonProperty("fix-pad-fgo")]
        public bool FixPadFgo = false;

        [JsonProperty("change-logout-mail")]
        public bool ChangeLogoutMail = false;

        [JsonProperty("change-reboot")]
        public bool ChangeReboot = false;

        [JsonProperty("app-backup")]
        public string AppBackup = "";

        [JsonProperty("change-remove-app")]
        public bool ChangeRemoveApp = false;
    }
}
