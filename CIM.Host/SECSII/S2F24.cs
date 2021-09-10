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
    public class S2F24 : SFMessage
    {
        public S2F24(SECSDriverBase driver)
            : base(driver)
        {
            Stream = 2; Function = 24;
        }

        public override void DoWork(string driverName, object obj)
        {
            LogHelper.Instance.BizLog.DebugFormat("[{0}]DoWork : {1}, {2}", this.GetType().Name, driverName, obj);
            CommonData.Instance.OnStreamFunctionAdd("MAIN", "H->E", "TRACE", "S2F23", null, "FDC Trace Initialize Send", null);
            SecsMessage primaryMessage = obj as SecsMessage;

            string EQPID = CommonData.Instance.EQP_SETTINGS.EQPID;
            string TRID;
            int TOTSMP = 0;
            int REPGSZ = 0;
            int svidListCount = 0;
            int DSPER;
            int traceValidationResult = 0;
            uint systemByte = primaryMessage.SystemByte;
            List<SV> svids = new List<SV>();
            TRACE trace = new TRACE();

            int nList = primaryMessage.GetItem().GetList();                             //L6
            {
                EQPID = primaryMessage.GetItem().GetAscii().Trim();
                TRID = primaryMessage.GetItem().GetAscii();                            //A5  TRID : "0" -> TRID Delete
                int.TryParse(primaryMessage.GetItem().GetAscii(), out DSPER);                           //A6  DSPER : "hhmmss.mmm" Trace Period
                int.TryParse(primaryMessage.GetItem().GetAscii(), out TOTSMP);                          //A5  TOTSMP : "0" -> Untill Init or Disconnect 보고
                int.TryParse(primaryMessage.GetItem().GetAscii(), out REPGSZ);                          //A3  REPGSZ
                
                
                if(CommonData.Instance.HOST_MODE != eHostMode.HostOnlineRemote)
                {
                    traceValidationResult = (int)CIM.Common.TC.TIAACK.EquipmentOffline;
                }
                else if(CommonData.Instance.EQP_SETTINGS.EQPID != EQPID)
                {
                    traceValidationResult = (int)CIM.Common.TC.TIAACK.EquipmentSpecifiedError;
                }
                else
                {
                    if(TRID == "0")
                    {
                        FDCManager.Instance.AllTraceJobStop();
                    }
                    else
                    {
                        svidListCount = primaryMessage.GetItem().GetList();                        //Ln : n=0, Trace OFF

                        if(svidListCount == 0)
                        {
                            if(FDCManager.Instance.TraceJobStop(TRID))
                            {
                                traceValidationResult = (int)CIM.Common.TC.TIAACK.ACCEPTED;
                            }
                            else
                            {
                                traceValidationResult = (int)CIM.Common.TC.TIAACK.EquipmentSpecifiedError;
                            }
                        }
                        else
                        {
                            TimeSpan tsDSPER = TimeSpan.FromMilliseconds(Convert.ToDouble(DSPER));

                            if(tsDSPER.TotalSeconds < 1)
                            {
                                traceValidationResult = (int)CIM.Common.TC.TIAACK.InvalidPeriod;
                            }
                            else
                            {
                                bool NoMoretracesAllowed_NACK_Flag = false;

                                for(int i = 0; i < svidListCount; i++)
                                {
                                    string svid = primaryMessage.GetItem().GetAscii();

                                    if (string.IsNullOrEmpty(svid) || !FDCManager.Instance.IsExistSVID(svid))
                                    {
                                        NoMoretracesAllowed_NACK_Flag = true;
                                        break;
                                    }
                                    else
                                    {
                                        svids.Add(FDCManager.Instance.GetSVInfoBySvid(svid));
                                    }
                                }

                                if (NoMoretracesAllowed_NACK_Flag)
                                {
                                    // NACK 일 경우는 처리 하지 않는다.
                                    //traceValidationResult = (int)CIM.Common.TC.TIAACK.EquipmentSpecifiedError;
                                    traceValidationResult = (int)CIM.Common.TC.TIAACK.NoMoretracesAllowed;
                                }
                                else
                                {
                                    trace.TRID = TRID; trace.TotalSamplingCount = TOTSMP; trace.SamplingPeriod = tsDSPER.TotalMilliseconds; trace.SVs = svids;
                                    trace.SamplingCount = 0;

                                    if (FDCManager.Instance.TraceJobAddOrUpdate(trace))
                                    {
                                        traceValidationResult = 0;
                                    }
                                    else
                                    {
                                        traceValidationResult = (int)CIM.Common.TC.TIAACK.EquipmentSpecifiedError;
                                    }
                                }
                            }
                        }
                    }
                }

                SecsMessage reply = new SecsMessage(Stream, Function, systemByte);
                reply.WaitBit = false;
                try
                {
                    reply.AddAscii(traceValidationResult.ToString());

                    SecsDriver.WriteLogAndSendMessage(reply, "");
                }
                catch (Exception ex)
                {
                    LogHelper.Instance.ErrorLog.DebugFormat(ex.Message + "\n\n\n S2F24(Trace Initialize Acknowledge(TIA))");
                }
            }
            CommonData.Instance.OnStreamFunctionAdd("MAIN", "E->H", "TRACE", "S2F24", null, "FDC Trace Initialize Ack", null);
        }
    }
}
