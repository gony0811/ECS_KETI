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
    public class S6F1 : SFMessage
    {
        string _TRID;
        string _SMPLN;
        string _STIME;
        string _EQPID;

        public S6F1(SECSDriverBase driver)
            : base(driver) 
        {
            Stream = 6; Function = 1;
        }

        public override void DoWork(string driverName, object obj)
        {
            LogHelper.Instance.BizLog.DebugFormat("[{0}]DoWork : {1}, {2}", this.GetType().Name, driverName, obj);
            //CommonData.Instance.OnStreamFunctionAdd("MAIN", "H->E", "FDC", "S6F1", null, "Trace Data Send", null);
            TRACE trace = obj as TRACE;
            _EQPID = CommonData.Instance.EQP_SETTINGS.EQPID;
            _TRID = trace.TRID;
            _SMPLN = trace.SamplingCount.ToString();
            _STIME = DateTime.Now.ToString("yyyyMMddHHmmss");

            SecsMessage secsMessage = new SecsMessage(Stream, Function, false, Convert.ToInt32(SecsDriver.DeviceID));

            secsMessage.AddList(5);
            {
                secsMessage.AddAscii(AppUtil.ToAscii(_EQPID, gDefine.DEF_EQPID_SIZE));
                secsMessage.AddAscii(_TRID.Length, _TRID);
                secsMessage.AddAscii(AppUtil.ToAscii(_SMPLN, 5));
                secsMessage.AddAscii(AppUtil.ToAscii(_STIME, 14));
                secsMessage.AddList(trace.SVs.Count);
                {
                    foreach (SV sv in trace.SVs)
                    {
                        secsMessage.AddList(2);
                        {
                            secsMessage.AddAscii(sv.SVID);
                            secsMessage.AddAscii(FDCManager.Instance.GetValueBySvid(sv.SVID));
                        }
                    }
                }
            }

            this.SecsDriver.WriteLogAndSendMessage(secsMessage, "");

        }
    }
}
