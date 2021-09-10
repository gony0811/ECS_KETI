using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CIM.Common;
using INNO6.Core;
using SYSWIN.Secl;

namespace CIM.Host.SECSII
{
    /* S1F1(Are You There Request) */
    public class S1F1 : SFMessage
    {
        public S1F1(SECSDriverBase driver)
            :base(driver)
        {

        }
        public override void DoWork(string driverName, object obj)
        {
            LogHelper.Instance.BizLog.DebugFormat("[{0}]DoWork : {1}, {2}", this.GetType().Name, driverName, obj);
            CommonData.Instance.OnStreamFunctionAdd("MAIN", "E->H", "INIT", "S1F1", null, "Are you there?", null);
            Stream = 1; Function = 1;
            SecsMessage msg = new SecsMessage(Stream, Function, true, Convert.ToInt32(SecsDriver.DeviceID));
            SecsDriver.WriteLogAndSendMessage(msg, "");
        }
    }
} 