using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using INNO6.Core;
using INNO6.IO;
using CIM.Manager;
using CIM.Common;
using SYSWIN.Secl;

namespace CIM.Host.SECSII
{
    public class S2F17 : SFMessage
    {
        public S2F17(SECSDriverBase driver)
            : base(driver)
        {
            Stream = 2; Function = 17;
        }

        public override void DoWork(string driverName, object obj)
        {
            LogHelper.Instance.BizLog.DebugFormat("[{0}]DoWork : {1}, {2}", this.GetType().Name, driverName, obj);
            SecsMessage secsMessage = new SecsMessage(Stream, Function, true, Convert.ToInt32(SecsDriver.DeviceID))
            {
                Stream = 2,
                Function = 17
            };

            SecsDriver.WriteLogAndSendMessage(secsMessage, "");

            CommonData.Instance.OnStreamFunctionAdd("MAIN", "E->H", "TIMESET", "S2F17", null, "Data and Time Request", null);
        }
    }
}
