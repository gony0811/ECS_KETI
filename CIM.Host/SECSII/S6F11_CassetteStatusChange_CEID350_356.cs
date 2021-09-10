using CIM.Common;
using CIM.Manager;
using INNO6.Core;
using INNO6.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SYSWIN.Secl;

namespace CIM.Host.SECSII
{
    class S6F11_CassetteStatusChange_CEID350_356 : SFMessage
    {
        /*S6F11(ProcessJob), CEID :350, 351, 352, 353, 354, 355, 356*/

        private string _processPortNo;
        private string _portName;

        public S6F11_CassetteStatusChange_CEID350_356(SECSDriverBase driver, string ProcessPortNo, string PortName)
            : base(driver)
        {
            Stream = 6; Function = 11;
            _processPortNo = ProcessPortNo; _portName = PortName;
        }

        public override void DoWork(string driverName, object obj)
        {
            LogHelper.Instance.BizLog.DebugFormat("[{0}]DoWork : {1}, {2}", this.GetType().Name, driverName, obj);
            Data data = obj as Data;
            bool bResult;
            INNO6.IO.Interface.eDevMode connectStatus = DataManager.Instance.IsDeviceMode("DEV1");

            //1. Parsing Data 
            string sDATAID = "0";
            string ceidTagName = string.Format("i{0}.CassetteStateChange{1}_{2}.CEID", data.Module, _processPortNo, _portName);
            string sCEID = DataManager.Instance.GET_INT_DATA(ceidTagName, out bResult).ToString();

            //RTPID "100"
            string sCRST = ((int)CommonData.Instance.HOST_MODE).ToString();
            string sEQPID = CommonData.Instance.EQP_SETTINGS.EQPID;

            //RTPID "307"
            string portNoTagName = string.Format("i{0}.CassetteStateChange{1}_{2}.PortID", data.Module, _processPortNo, _portName);
            string portavailableTagName = string.Format("i{0}.CassetteStateChange{1}_{2}.PortAvailableState", data.Module, _processPortNo, _portName);
            string portaccessTagName = string.Format("i{0}.CassetteStateChange{1}_{2}.PortAccessMode", data.Module, _processPortNo, _portName);
            string porttransferTagName = string.Format("i{0}.CassetteStateChange{1}_{2}.PortTransferState", data.Module, _processPortNo, _portName);
            string portprocessingTagName = string.Format("i{0}.CassetteStateChange{1}_{2}.PortProcessingState", data.Module, _processPortNo, _portName);

            string sPORT_NO = DataManager.Instance.GET_STRING_DATA(portNoTagName, out bResult);
            string sPORT_AVAILABLE_STATE = DataManager.Instance.GET_STRING_DATA(portavailableTagName, out bResult);
            string sPORT_ACCESS_MODE = DataManager.Instance.GET_STRING_DATA(portaccessTagName, out bResult);
            string sPORT_TRANSFER_STATE = DataManager.Instance.GET_STRING_DATA(porttransferTagName, out bResult);
            string sPORT_PROCESSING_STATE = DataManager.Instance.GET_STRING_DATA(portprocessingTagName, out bResult);

            //RTPID "251"
            string jobIDTagName = string.Format("i{0}.CassetteStateChange{1}_{2}.JobID", data.Module, _processPortNo, _portName);
            string JobtypeTagName = string.Format("i{0}.CassetteStateChange{1}_{2}.JobType", data.Module, _processPortNo, _portName);

            string sJOBID = DataManager.Instance.GET_STRING_DATA(jobIDTagName, out bResult);
            string sJOBTYPE = DataManager.Instance.GET_STRING_DATA(JobtypeTagName, out bResult);

            if (sCEID == "354")
            {
                CommonData.Instance.VALIDATION_DATA.SaveValidationData(KindOfValidation.CARRIER_INFO_SEND, sJOBID + ".", Convert.ToInt32(_processPortNo), data.Module);
                CommonData.Instance.LOADER_CASSETTEID = sJOBID;
            }
            
            

            #region S6F11(CassetteStatusChange), CEID:350, 351, 352, 353, 354, 355, 356
            SecsMessage msg = new SecsMessage(Stream, Function, true, Convert.ToInt32(SecsDriver.DeviceID));

            msg.AddList(3);                                                                                              //L3  Cassette Process Event Info
            {
                msg.AddAscii(AppUtil.ToAscii(sDATAID, gDefine.DEF_DATAID_SIZE));                                         //A4  Data ID
                msg.AddAscii(AppUtil.ToAscii(sCEID, gDefine.DEF_CEID_SIZE));                                             //A3  Collection Event ID
                msg.AddList(3);                                                                                          //L3  RPTID Set
                {
                    msg.AddList(2);                                                                                          //L2  RPTID 100 Set 
                    {
                        msg.AddAscii(AppUtil.ToAscii("100", gDefine.DEF_RPTID_SIZE));                                            //A3  RPTID="100"
                        msg.AddList(2);                                                                                          //L2  EQP Control State Set
                        {
                            msg.AddAscii(AppUtil.ToAscii(sEQPID, gDefine.DEF_EQPID_SIZE));                                           //A40 HOST REQ EQPID 
                            msg.AddAscii(sCRST);                                                                                     //A1  Online Control State 
                        }
                    }
                    msg.AddList(2);                                                                                         //L2  RPTID 307 Set 
                    {
                        msg.AddAscii(AppUtil.ToAscii("307", gDefine.DEF_RPTID_SIZE));                                           //A3  RPTID="307"   
                        {
                            msg.AddList(5);                                                                                         //L5  Port Set
                            {
                                msg.AddAscii(sPORT_NO);                                                            //A4  Port No
                                msg.AddAscii(sPORT_AVAILABLE_STATE);                                                                   //A1  Port Avilability State Info
                                msg.AddAscii(sPORT_ACCESS_MODE);                                                                       //A1  Port Access State Info
                                msg.AddAscii(sPORT_TRANSFER_STATE);                                                                    //A1  Port Transfer State Info
                                msg.AddAscii(sPORT_PROCESSING_STATE);                                                                  //A1  Port Processing State Info
                            }
                        }
                    }
                    msg.AddList(2);                                                                                          //L2  RPTID 251 Set
                    {
                        msg.AddAscii(AppUtil.ToAscii("251", gDefine.DEF_RPTID_SIZE));                                           //A3  RPTID="251"   
                        msg.AddList(2);                                                                                         //L2  Port Set
                        {
                            msg.AddAscii(AppUtil.ToAscii(sJOBID, 40));                                                             //A40 JOB ID
                            msg.AddAscii(AppUtil.ToAscii(sJOBTYPE, 20));                                                           //A20 JOBTYPE
                        }
                    }
                }
            }
            this.SecsDriver.WriteLogAndSendMessage(msg, sCEID);

            #region for UI
            string Materiallog_name;
            if (sCEID == "350") Materiallog_name = string.Format("Cassette Status Change Load Request #{0}_{1}", _processPortNo, _portName);
            else if (sCEID == "351") Materiallog_name = string.Format("Cassette Status Change Load Complete #{0}_{1}", _processPortNo, _portName);
            else if (sCEID == "352") Materiallog_name = string.Format("Cassette Status Change Unload Request #{0}_{1}", _processPortNo, _portName);
            else if (sCEID == "353") Materiallog_name = string.Format("Cassette Status Change Unload Complete #{0}_{1}", _processPortNo, _portName);
            else if (sCEID == "354") Materiallog_name = string.Format("Cassette Status Change Information Request #{0}_{1}", _processPortNo, _portName);
            else if (sCEID == "355") Materiallog_name = string.Format("Cassette Status Change Move In #{0}_{1}", _processPortNo, _portName);
            else Materiallog_name = "";

            CommonData.Instance.OnStreamFunctionAdd("MAIN", "E->H", "MARTERIAL", "S6F11", sCEID, Materiallog_name, null);
            #endregion
            #endregion
        }
    }
}