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
    public class S2F14 : SFMessage
    {
        private int _ecidCount;
        private List<string> _ecids;

        public S2F14(SECSDriverBase driver)
            : base(driver)
        {
            Stream = 2; Function = 14;
        }

        public override void DoWork(string driverName, object obj)
        {
            LogHelper.Instance.BizLog.DebugFormat("[{0}]DoWork : {1}, {2}", this.GetType().Name, driverName, obj);
            CommonData.Instance.OnStreamFunctionAdd("MAIN", "H->E", "EC", "S2F13", null, "Equipment Constant Request", null);
            SecsMessage primaryMessage = obj as SecsMessage;

            string myEqpId = CommonData.Instance.EQP_SETTINGS.EQPID;
            string rcvEqpId = string.Empty;
            uint systemByte = primaryMessage.SystemByte;
            Dictionary<EC, ECValue> ecList = new Dictionary<EC,ECValue>();

            int nList = primaryMessage.GetItem().GetList();                                     //L2
            {
                rcvEqpId = primaryMessage.GetItem().GetAscii();                                // A40 EQPID

                _ecidCount = primaryMessage.GetItem().GetList();                                 //Ln  Number of EQ Constant List Count
                {
                    if (_ecidCount != 0)                   
                    {
                        _ecids = new List<string>(_ecidCount);
                        {
                            for (int i = 0; i < _ecidCount; i++)
                            {
                                _ecids.Add(primaryMessage.GetItem().GetAscii());                         //A8 ECID
                            }
                        }

                        ecList = ECManager.Instance.GetECValueList(_ecids.ToArray());
                    }
                    else
                    {
                        ecList = ECManager.Instance.GetECValueList(null);
                    }
                }
            }

            SecsMessage reply = new SecsMessage(Stream, Function, systemByte)
            {
                WaitBit = false
            };
            if (myEqpId != rcvEqpId)
            {
                reply.AddList(0);                                                                     //Ln=0 ERROR (0221 SJP)
            }
            else if(ecList == null)
            {
                reply.AddList(0);          
            }
            else
            {
                reply.AddList(ecList.Count);                                                             //Ln  n = HOST REQ ECID No.
                {
                    for (int i = 0; i < ecList.Count; i++)
                    {
                        reply.AddAscii(AppUtil.ToAscii(ecList.Keys.ToArray()[i].ECNAME, 40));                                 //A40 Equipment Constant name
                    }
                }
            }

            SecsDriver.WriteLogAndSendMessage(reply, "");

            CommonData.Instance.OnStreamFunctionAdd("MAIN", "E->H", "EC", "S2F14", null, "Equipment Constant Data Send", null);
        }
    }
}
