using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CIM.Common;
using INNO6.Core;
using CIM.Manager;
using SYSWIN.Secl;

namespace CIM.Host.SECSII
{
    public class S1F12 : SFMessage
    {
        private string _eqpid;
        private uint _systemByte;
        private List<string> _svidList;

        public S1F12(SECSDriverBase driver)
        :base(driver)
        {
            Stream = 1;
            Function = 12;
            _svidList = new List<string>();
        }

        private void ReceiveMessage(object obj)
        {
            SecsMessage primaryMessage = obj as SecsMessage;

            _systemByte = primaryMessage.SystemByte;


            int list1 = primaryMessage.GetItem().GetList();
            {
                _eqpid = primaryMessage.GetItem().GetAscii().Trim();
                int list2 = primaryMessage.GetItem().GetList();              

                if (list2 != 0)
                {                 
                    for (int i = 0; i < list2; i++)
                    {
                        _svidList.Add(primaryMessage.GetItem().GetAscii());
                    }
                }
            }
        }

        public override void DoWork(string driverName, object obj)
        {
            LogHelper.Instance.BizLog.DebugFormat("[{0}]DoWork : {1}, {2}", this.GetType().Name, driverName, obj);
            CommonData.Instance.OnStreamFunctionAdd("MAIN", "H->E", "FDC", "S1F11", null, "FDC Name List Request", null);
            bool error;
            
            ReceiveMessage(obj);

            SecsMessage reply = new SecsMessage(Stream, Function, _systemByte)
            {
                WaitBit = false
            };
            reply.AddList(2);
            {
                reply.AddAscii(AppUtil.ToAscii(CommonData.Instance.EQP_SETTINGS.EQPID, gDefine.DEF_EQPID_SIZE));                               //A40 EQP ID
                if (_eqpid != CommonData.Instance.EQP_SETTINGS.EQPID)
                {
                    reply.AddList(0);
                    SecsDriver.Send(reply);
                    return;
                }
                else
                {
                    Dictionary<string, string> svNameList; 

                    if(_svidList.Count == 0)
                    {
                        svNameList = FDCManager.Instance.GetNameListBySvids(null, out error);
                    }
                    else
                    {
                        svNameList = FDCManager.Instance.GetNameListBySvids(_svidList.ToArray(), out error);
                    }

                    List<string> svids = svNameList.Keys.ToList();
                    List<string> svNames = svNameList.Values.ToList();

                    reply.AddList(svNameList.Count);
                    {
                        if(error)
                        {
                            reply.AddList(0);
                        }
                        else
                        {
                            for (int i = 0; i < svNameList.Count; i++)
                            {
                                reply.AddList(2);
                                {
                                    reply.AddAscii(AppUtil.ToAscii(svids[i], 20));
                                    reply.AddAscii(AppUtil.ToAscii(svNames[i], 40));
                                }
                            }
                        }
                    }
                }
            }

            SecsDriver.WriteLogAndSendMessage(reply, "");

            CommonData.Instance.OnStreamFunctionAdd("MAIN", "E->H", "FDC", "S1F12", null, "FDC Name List Request", null);
        }
    }
}
