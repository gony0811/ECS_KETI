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

namespace CIM.Host.SECSII
{
    public class S7F102 : SFMessage
    {
        uint _systemByte;
        string _EQPID;
        string _PPIDTYPE;
        List<string> _PPIDList = new List<string>();

        public S7F102(SECSDriverBase driver)
            :base(driver)
        {
            Stream = 7; Function = 102;
        }

        public override void DoWork(string driverName, object obj)
        {
            LogHelper.Instance.BizLog.DebugFormat("[{0}]DoWork : {1}, {2}", this.GetType().Name, driverName, obj);
            CommonData.Instance.OnStreamFunctionAdd("MAIN", "H->E", "RMS", "S7F101", null, "PPID List Search Request", null);
            SecsMessage primaryMessage = obj as SecsMessage;

            _systemByte = primaryMessage.SystemByte;

            int list0 = primaryMessage.GetItem().GetList();
            _EQPID = primaryMessage.GetItem().GetAscii();
            _PPIDTYPE = primaryMessage.GetItem().GetAscii();

            _PPIDList = RMSManager.Instance.GetPPIDListAsList();

            SecsMessage reply = new SecsMessage(Stream, Function, _systemByte);

            reply.AddList(3);
            {
                reply.AddAscii(gDefine.DEF_EQPID_SIZE, CommonData.Instance.EQP_SETTINGS.EQPID);
                reply.AddAscii(1, _PPIDTYPE);

                if (_EQPID != CommonData.Instance.EQP_SETTINGS.EQPID
                    || CommonData.Instance.CURRENT_EQP_STATUS.AVAILABILITY == "1"
                    || DataManager.Instance.IsDeviceMode("DEV1") == INNO6.IO.Interface.eDevMode.DISCONNECT 
                    || _PPIDTYPE != "1")
                {
                    reply.AddList(0);
                }
                else
                {
                    reply.AddList(_PPIDList.Count);
                    {
                        foreach (string id in _PPIDList)
                        {
                            reply.AddAscii(gDefine.DEF_EQPID_SIZE, id);
                        }
                    }
                }
            }

            SecsDriver.WriteLogAndSendMessage(reply, "");
            CommonData.Instance.OnStreamFunctionAdd("MAIN", "E->H", "RMS", "S7F102", null, "PPID List Search Data Send", null);
        }
    }
}
