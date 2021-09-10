using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using INNO6.Core;
using INNO6.Core.Threading;
using INNO6.IO;
using CIM.Manager;
using CIM.Common;
using SYSWIN.Secl;

namespace CIM.Host.SECSII
{
    public class S7F26 : SFMessage
    {
        uint _systemByte;
        string _EQPID;
        string _PPID;
        string _PPIDTYPE;
        int _ppidIndex;
        int ACKC3;

        public S7F26(SECSDriverBase driver) : base (driver)
        {
            Stream = 7; Function = 26;
        }

        public override void DoWork(string driverName, object obj)
        {
            LogHelper.Instance.BizLog.DebugFormat("[{0}]DoWork : {1}, {2}", this.GetType().Name, driverName, obj);
            CommonData.Instance.OnStreamFunctionAdd("MAIN", "H->E", "RMS", "S7F25", null, "PPID Paramter Search Request", null);

            SecsMessage primaryMessage = obj as SecsMessage;

            _systemByte = primaryMessage.SystemByte;

            int list0 = primaryMessage.GetItem().GetList();
            {
                _EQPID = primaryMessage.GetItem().GetAscii();
                _PPID = primaryMessage.GetItem().GetAscii();
                _PPIDTYPE = primaryMessage.GetItem().GetAscii();
            }

            List<string> ppidList = RMSManager.Instance.GetPPIDListAsList();

            Dictionary<string, string> rmsNameValue = RMSManager.Instance.GetParameterNameValueByPPID(_PPID);


            if (_PPID == "")
            {
                SecsMessage reply = new SecsMessage(Stream, Function, _systemByte);
                reply.WaitBit = false;
                ACKC3 = (int)Common.TC.ACKC3.ERROR;
                reply.AddAscii(ACKC3.ToString());
                SecsDriver.WriteLogAndSendMessage(reply, "");
                CommonData.Instance.OnStreamFunctionAdd("MAIN", "E->H", "PPID", "S7F26", null, "PPID NULL", null);
            }
            else if (!ppidList.Contains(_PPID) || _EQPID != CommonData.Instance.EQP_SETTINGS.EQPID || _PPIDTYPE != "1")
            {

                SecsMessage reply = new SecsMessage(Stream, Function, _systemByte);
                reply.WaitBit = false;
                reply.AddList(7);
                {
                    reply.AddAscii(gDefine.DEF_EQPID_SIZE, _EQPID);
                    reply.AddAscii(gDefine.DEF_PPID_SIZE, _PPID);
                    reply.AddAscii(1, _PPIDTYPE);
                    reply.AddAscii(20, CommonData.Instance.EQP_SETTINGS.CIM_SW_VERSION);
                    reply.AddAscii(6, CommonData.Instance.EQP_SETTINGS.EQP_SW_VERSION);
                    reply.AddAscii(14, DateTime.Now.ToString("yyyyMMddHHmmss"));

                    reply.AddList(1);
                    {
                        reply.AddList(2);
                        {
                            reply.AddAscii(3, " ");
                            reply.AddList(0);
                        }
                    }
                }

                SecsDriver.WriteLogAndSendMessage(reply, "");
                CommonData.Instance.OnStreamFunctionAdd("MAIN", "E->H", "RMS", "S7F26", null, "PPID Paramter Search Data Send ", null);
            }
            else if (rmsNameValue.Count != 0)
            {
                SecsMessage reply = new SecsMessage(Stream, Function, _systemByte);
                reply.WaitBit = false;
                reply.AddList(7);
                {
                    reply.AddAscii(gDefine.DEF_EQPID_SIZE, _EQPID);
                    reply.AddAscii(gDefine.DEF_PPID_SIZE, _PPID);
                    reply.AddAscii(1, _PPIDTYPE);
                    reply.AddAscii(20, CommonData.Instance.EQP_SETTINGS.CIM_SW_VERSION);
                    reply.AddAscii(6, CommonData.Instance.EQP_SETTINGS.EQP_SW_VERSION);
                    reply.AddAscii(14, DateTime.Now.ToString("yyyyMMddHHmmss"));

                    for (int i = 0; i < 1; i++)
                    {
                        reply.AddList(1);
                        {
                            reply.AddList(2);
                            {
                                reply.AddAscii(3, " ");

                                reply.AddList(rmsNameValue.Count);

                                foreach (KeyValuePair<string, string> r in rmsNameValue)
                                {
                                    reply.AddList(2);
                                    {
                                        reply.AddAscii(r.Key);
                                        reply.AddAscii(r.Value);
                                    }
                                }

                            }
                        }
                    }
                }

                SecsDriver.WriteLogAndSendMessage(reply, "");
                CommonData.Instance.OnStreamFunctionAdd("MAIN", "E->H", "RMS", "S7F26", null, "PPID Paramter Search Data Send ", null);
            }
            else
            {
                foreach (MODULE m in CommonData.Instance.MODULE_SETTINGS)
                {
                    DataManager.Instance.SET_STRING_DATA(string.Format("o{0}.FormattedProcessProgramReq.PPID", m.MODULE_NAME), _PPID);
                }

                DataTable ppidListDatatable = RMSManager.Instance.GetPPIDListAsDataTable();

                var result = (from m in ppidListDatatable.AsEnumerable()
                              where m.Field<string>("PPIDNAME") == _PPID
                              select m).FirstOrDefault();

                if (result == null)
                {
                    SecsMessage reply = new SecsMessage(Stream, Function, _systemByte);
                    reply.WaitBit = false;
                    reply.AddList(7);
                    {
                        reply.AddAscii(gDefine.DEF_EQPID_SIZE, _EQPID);
                        reply.AddAscii(gDefine.DEF_PPID_SIZE, _PPID);
                        reply.AddAscii(1, _PPIDTYPE);
                        reply.AddAscii(20, CommonData.Instance.EQP_SETTINGS.CIM_SW_VERSION);
                        reply.AddAscii(6, CommonData.Instance.EQP_SETTINGS.EQP_SW_VERSION);
                        reply.AddAscii(14, DateTime.Now.ToString("yyyyMMddHHmmss"));

                        for (int i = 0; i < 1; i++)
                        {
                            reply.AddList(1);
                            {
                                reply.AddList(2);
                                {
                                    reply.AddAscii(3, " ");

                                    reply.AddList(0);
                                }
                            }
                        }
                    }

                    SecsDriver.WriteLogAndSendMessage(reply, "");
                    CommonData.Instance.OnStreamFunctionAdd("MAIN", "E->H", "RMS", "S7F26", null, "PPID Paramter Search Data Send ", null);
                }
                else
                {
                    DataRow dr = result as DataRow;
                    _ppidIndex = dr.Field<int>("NO");

                    foreach (MODULE m in CommonData.Instance.MODULE_SETTINGS)
                    {
                        DataManager.Instance.SET_INT_DATA(string.Format("o{0}.FormattedProcessProgramReq.Index", m.MODULE_NAME), _ppidIndex);
                        DataManager.Instance.SET_INT_DATA(string.Format("o{0}.Send.FormattedProcessProgramRequest", m.MODULE_NAME), 1);
                    }

                    WorkQueue.Instance.Add(new HandlerWorkerTask(this, null, new WorkEventHandler((sender, param) =>
                    {
                        DateTime workQStartTime = DateTime.Now;
                        TimeSpan workqQTimeout = DateTime.Now - workQStartTime;

                        S7F26 msg = sender as S7F26;

                        while (workqQTimeout.Seconds < 10000)
                        {
                            bool dataReadResult = true;
                            bool replyComplete = true;
                            foreach (MODULE m in CommonData.Instance.MODULE_SETTINGS)
                            {
                                int ret = DataManager.Instance.GET_INT_DATA(string.Format("i{0}.Reply.FormattedProcessProgramRequest", m.MODULE_NAME), out dataReadResult);
                                if (ret <= 0 || !dataReadResult)
                                {
                                    replyComplete = false;
                                    break;
                                }
                            }

                            if (replyComplete)
                            {
                                InitializeProcess.Instance.InitializeRMSData();

                                Dictionary<string, string> paramNameValue = RMSManager.Instance.GetParameterNameValueByPPID(_PPID);

                                SecsMessage reply = new SecsMessage(msg.Stream, msg.Function, msg._systemByte);
                                reply.WaitBit = false;
                                reply.AddList(7);
                                {
                                    reply.AddAscii(gDefine.DEF_EQPID_SIZE, msg._EQPID);
                                    reply.AddAscii(gDefine.DEF_PPID_SIZE, msg._PPID);
                                    reply.AddAscii(1, msg._PPIDTYPE);
                                    reply.AddAscii(20, CommonData.Instance.EQP_SETTINGS.CIM_SW_VERSION);
                                    reply.AddAscii(6, CommonData.Instance.EQP_SETTINGS.EQP_SW_VERSION);
                                    reply.AddAscii(14, DateTime.Now.ToString("yyyyMMddHHmmss"));

                                    for (int i = 0; i < 1; i++)
                                    {
                                        reply.AddList(1);
                                        {
                                            reply.AddList(2);
                                            {
                                                reply.AddAscii(3, " ");

                                                reply.AddList(paramNameValue.Count);

                                                foreach (KeyValuePair<string, string> p in paramNameValue)
                                                {
                                                    reply.AddList(2);
                                                    {
                                                        reply.AddAscii(p.Key);
                                                        reply.AddAscii(p.Value);
                                                    }
                                                }

                                            }
                                        }
                                    }
                                }

                                SecsDriver.WriteLogAndSendMessage(reply, "");
                                CommonData.Instance.OnStreamFunctionAdd("MAIN", "E->H", "RMS", "S7F26", null, "PPID Paramter Search Data Send ", null);
                                return;
                            }

                            Thread.Sleep(50);
                        }

                        //Console.WriteLine("[Error] PPID Parameter inquiry Failed : PLC reply timeout");
                        LogHelper.Instance.SECSMessageLog.DebugFormat("[ERROR] PPID Parameter inquiry Failed : PLC reply timeout");
                        CommonData.Instance.OnStreamFunctionAdd("MAIN", "E->H", "RMS", "S7F26", null, "PPID Paramter Search Time Out ", "TIMEOUT");

                    })));
                }
            }
        }
    }
}
