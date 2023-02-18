using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinSubTrial
{
    public enum TaskResult
    {
        Success,
        Failure,
        StopAuto,
        ProxyError
    }

    public enum MailTask
    {
        createMail,
        loginMail,
        saveInfo,
        onlyLogin
    }

    public enum DeviceType
    {
        miA1,
        samsung
    }
}
