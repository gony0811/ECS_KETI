using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using INNO6.Core;
using CIM.Common;
using SYSWIN.Secl;

namespace CIM.Host.SECSII
{
    public class S1F2 : SFMessage
    {
        public S1F2(SECSDriverBase driver)
            :base(driver)
        {
            Stream = 1; Function = 2;
        }

        public override void DoWork(string driverName, object obj)
        {
            LogHelper.Instance.BizLog.DebugFormat("[{0}]DoWork : {1}, {2}", this.GetType().Name, driverName, obj);
            SecsMessage primaryMessage = obj as SecsMessage;

            uint systemByte = primaryMessage.SystemByte;

            SecsMessage secsMessage = new SecsMessage(Stream, Function, systemByte);

            string MDLN = CommonData.Instance.EQP_SETTINGS.EQPID;
            string VERSION = CommonData.Instance.EQP_SETTINGS.CIM_SW_VERSION;

            secsMessage.AddList(2);                                                                            //L2
            {
                secsMessage.AddAscii(AppUtil.ToAscii(MDLN, 20));                                                  //A20 EQPID  
                secsMessage.AddAscii(AppUtil.ToAscii(VERSION, 6));                                                //A6 VERSION  
            }

            SecsDriver.WriteLogAndSendMessage(secsMessage, "");

            CommonData.Instance.OnStreamFunctionAdd("MAIN", "E->H", "INIT", "S1F2", null, "I'm Here", null);
        }
    }
}
