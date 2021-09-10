using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using INNO6.Core;
using CIM.Manager;
using CIM.Common;
using SYSWIN.Secl;

namespace CIM.Host.SECSII
{
    public class S1F4 : SFMessage
    {
        public S1F4(SECSDriverBase driver)
            : base(driver)
        {
            Stream = 1; Function = 4;
        }
        public override void DoWork(string driverName, object obj)
        {
            LogHelper.Instance.BizLog.DebugFormat("[{0}]DoWork : {1}, {2}", this.GetType().Name, driverName, obj);
            CommonData.Instance.OnStreamFunctionAdd("MAIN", "H->E", "FDC", "S1F3", null, "FDC Value List Request", null);
            SecsMessage primaryMessage = obj as SecsMessage;

            uint systemByte = primaryMessage.SystemByte;
            int nListA;
            int nList;
            string data;
            string eqpid;
            int svidListCount = 0;
            List<string> svids = new List<string>();

            nList = primaryMessage.GetItem().GetList();
            {
                data = primaryMessage.GetItem().GetAscii();
                eqpid = data.Trim();
                nListA = primaryMessage.GetItem().GetList();

                if (nListA != 0)
                {
                    for (int i = 0; i < nListA; i++)
                    {
                        svids.Add(primaryMessage.GetItem().GetAscii());
                    }
                }
            }

            if(svids.Count == 0)
            {
                svidListCount = FDCManager.Instance.TotalSVListCount;
            }
            else
            {
                svidListCount = svids.Count;
            }

            bool error = false;
            Dictionary<string, string> svidValueListAll = FDCManager.Instance.GetNameListBySvids(svids.ToArray(), out error);

            string[] aSV = new string[svidListCount];
            string[] aSVID = new string[svidListCount];

            if(!error)
            {
                if (svids.Count == 0)
                {
                    for (int i = 0; i < svidListCount; i++)
                    {
                        aSVID[i] = svidValueListAll.Keys.ToList()[i];
                        aSV[i] = FDCManager.Instance.GetValueBySvid(aSVID[i]);
                    }
                }
                else
                {
                    for (int i = 0; i < svidListCount; i++)
                    {
                        if (svidValueListAll.ContainsKey(svids[i]))
                        {
                            aSVID[i] = svids[i];
                            aSV[i] = FDCManager.Instance.GetValueBySvid(aSVID[i]);

                        }
                    }
                }
            }

            if (eqpid != CommonData.Instance.EQP_SETTINGS.EQPID) error = true;

            SecsMessage reply = new SecsMessage(Stream, Function, systemByte)
            {
                WaitBit = false
            };
            reply.AddList(2);
            {
                reply.AddAscii(AppUtil.ToAscii(eqpid, gDefine.DEF_EQPID_SIZE)); 

                if(error)
                {
                    reply.AddList(0);
                }
                else
                {
                    reply.AddList(svidListCount);
                    {
                        for(int i = 0; i < svidListCount; i++)
                        {
                            reply.AddList(2);
                            {
                                reply.AddAscii(AppUtil.ToAscii(aSVID[i], 20));
                                reply.AddAscii(AppUtil.ToAscii(aSV[i], 40));
                            }
                        }
                    }
                }
            }

            SecsDriver.WriteLogAndSendMessage(reply, "");

            CommonData.Instance.OnStreamFunctionAdd("MAIN", "E->H", "FDC", "S1F4", null, "FDC Value List Data Send", null);
        }
    }
}
