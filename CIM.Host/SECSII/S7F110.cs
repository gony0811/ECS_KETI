using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using INNO6.Core;
using INNO6.Core.Threading;
using INNO6.IO;
using CIM.Manager;
using CIM.Common;
using SYSWIN.Secl;
using CIM.Common.TC;

namespace CIM.Host.SECSII
{
    public class S7F110 : SFMessage
    {
        string _PPID;
        string _EQPID;
        string _PPIDTYPE;

        public S7F110(SECSDriverBase driver)
            : base(driver)
        {
            Stream = 7; Function = 110;
        }

        public override void DoWork(string driverName, object obj)
        {
            LogHelper.Instance.BizLog.DebugFormat("[{0}]DoWork : {1}, {2}", this.GetType().Name, driverName, obj);
            SecsMessage primaryMessage = obj as SecsMessage;

            int list = primaryMessage.GetItem().GetList();
            string rcvEqpID = primaryMessage.GetItem().GetAscii().Trim();
            string rcvPPIDType = primaryMessage.GetItem().GetAscii().Trim();

            uint systemByte = primaryMessage.SystemByte;
            bool dataReadResult;

            HCACK HCACK = HCACK.ACCEPTED;

            MODULE module = CommonData.Instance.MODULE_SETTINGS.Where(m => m.TYPE == eModuleType.MASTER).FirstOrDefault();

            _EQPID = CommonData.Instance.EQP_SETTINGS.EQPID.Trim();
            _PPID = DataManager.Instance.GET_STRING_DATA(string.Format("i{0}.EQStatus.PPID", module.MODULE_NAME), out dataReadResult).Trim();
            _PPIDTYPE = rcvPPIDType;

            if (rcvEqpID != _EQPID)
                HCACK = Common.TC.HCACK.UNKNOWN_EQPID;

            if (_PPIDTYPE != "1")
                HCACK = Common.TC.HCACK.UNKNOWN_PPIDTYPE;

            SecsMessage reply = new SecsMessage(Stream, Function, systemByte);
            reply.WaitBit = false;
            reply.AddList(2);
            {
                reply.AddAscii(((int)HCACK).ToString());
                reply.AddList(3);
                {
                    reply.AddAscii(_EQPID);
                    reply.AddAscii(_PPID);
                    reply.AddAscii(_PPIDTYPE);
                }
            }

            SecsDriver.WriteLogAndSendMessage(reply, "");

            CommonData.Instance.OnStreamFunctionAdd("MAIN", "E->H", "RMS", "S7F109", null, "Current Running Equipment PPID data", null);
        }
    }
}