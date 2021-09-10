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
    public class S2F30 : SFMessage
    {
        public S2F30(SECSDriverBase driver)
            : base(driver)
        {
            Stream = 2; Function = 30;
        }

        public override void DoWork(string driverName, object obj)
        {
            LogHelper.Instance.BizLog.DebugFormat("[{0}]DoWork : {1}, {2}", this.GetType().Name, driverName, obj);
            CommonData.Instance.OnStreamFunctionAdd("MAIN", "H->E", "EC", "S2F29", null, "EC List Request", null);
            SecsMessage primaryMessage = obj as SecsMessage;

            string EQPID = CommonData.Instance.EQP_SETTINGS.EQPID;
            string receivedEQPID;
            int requestECIDListCount;
            List<string> ecidList = new List<string>();
            uint systemByte = primaryMessage.SystemByte;
            Dictionary<EC, ECValue> ecList = new Dictionary<EC,ECValue>();
            bool ack = true;

            int nList = primaryMessage.GetItem().GetList();                             //L2
            {
                receivedEQPID = primaryMessage.GetItem().GetAscii().Trim();
                requestECIDListCount = primaryMessage.GetItem().GetList();                         //Ln

                if (requestECIDListCount == 0)
                {
                    ecList = ECManager.Instance.GetECValueList(null);
                }
                else
                {
                    for (int i = 0; i < requestECIDListCount; i++)
                    {
                        string ecid = primaryMessage.GetItem().GetAscii().Trim();

                        if(ECManager.Instance.IsExistECID(ecid))
                        {
                            if(!ecidList.Contains(ecid))
                                ecidList.Add(ecid);                 //A8  ECID
                            else
                            {
                                ack = false;
                                break;
                            }
                        } 
                        else
                        {
                            ack = false;
                        }
                    }

                    ecList = ECManager.Instance.GetECValueList(ecidList.ToArray());
                }

                 

                if (EQPID != receivedEQPID)
                {
                    ack = false;
                }

                SecsMessage reply = new SecsMessage(Stream, Function, systemByte)
                {
                    WaitBit = false
                };
                reply.AddList(2);                                                                       //L2
                {
                    reply.AddAscii(AppUtil.ToAscii(EQPID, gDefine.DEF_EQPID_SIZE));                        //A40 HOST REQ EQPID

                    if (!ack)
                    {
                        reply.AddList(0);                                                                   //Ln=0 ERROR (0221 SJP)
                    }
                    else
                    {
                        reply.AddList(ecList.Count);                                                           //Ln  n = HOST REQ ECID No.
                        {
                            for (int i = 0; i < ecList.Count; i++)
                            {
                                reply.AddList(7);                                                               //L7  ECID Set
                                {
                                    reply.AddAscii(AppUtil.ToAscii(ecList.Keys.ToArray()[i].ECID, 8));                                   //A8  Equipment Constant ID 
                                    reply.AddAscii(AppUtil.ToAscii(ecList.Keys.ToArray()[i].ECNAME, 40));                                //A40 Equipment Constant name
                                    reply.AddAscii(AppUtil.ToAscii(ecList.Values.ToArray()[i].ECDEF, 20));                                 //A20 Equipment Constant Set value
                                    reply.AddAscii(AppUtil.ToAscii(ecList.Values.ToArray()[i].ECSLL, 20));                                 //A20 Equipment Direction Stop Low Limit
                                    reply.AddAscii(AppUtil.ToAscii(ecList.Values.ToArray()[i].ECSUL, 20));                                 //A20 Equipment Direction Stop Upper Limit
                                    reply.AddAscii(AppUtil.ToAscii(ecList.Values.ToArray()[i].ECWLL, 20));                                 //A20 Equipment Warning Low Limit
                                    reply.AddAscii(AppUtil.ToAscii(ecList.Values.ToArray()[i].ECSUL, 20));                                 //A20 Equipment Warning Upper Limit
                                }
                            }
                        }
                    }
                }
                SecsDriver.WriteLogAndSendMessage(reply, ack);
                CommonData.Instance.OnStreamFunctionAdd("MAIN", "E->H", "EC", "S2F30", null, "EC List Data Send", null);
            }
        }
    }
}
