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
    class S6F11_CellProcessStart_CEID401 : SFMessage
    {
        /*S6F11(CellProcessStart), CEID :401*/

        private string _cellPortNo;

        public S6F11_CellProcessStart_CEID401(SECSDriverBase driver, string CellPortNo)
            : base(driver)
        {
            Stream = 6; Function = 11;
            _cellPortNo = CellPortNo;
        }

        public override void DoWork(string driverName, object obj)
        {
            LogHelper.Instance.BizLog.DebugFormat("[{0}]DoWork : {1}, {2}", this.GetType().Name, driverName, obj);
            Data data = obj as Data;
            bool bResult;
            INNO6.IO.Interface.eDevMode connectStatus = DataManager.Instance.IsDeviceMode("DEV1");

            //1. Parsing Data 
            string sDATAID = "0";
            string sCEID = "401";

            //RTPID "100"
            string sEQPID = CommonData.Instance.EQP_SETTINGS.EQPID;
            string sCRST = ((int)CommonData.Instance.HOST_MODE).ToString();

            //RTPID "101"
            string availabilityTagName = string.Format("i{0}.EQStatus.Availability", data.Module);
            string interlockTagName = string.Format("i{0}.EQStatus.Interlock", data.Module);
            string moveStateTagName = string.Format("i{0}.EQStatus.Move", data.Module);
            string runStateTagName = string.Format("i{0}.EQStatus.Run", data.Module);
            string frontStateTagName = string.Format("i{0}.EQStatus.Front", data.Module);
            string rearStateTagName = string.Format("i{0}.EQStatus.Rear", data.Module);
            string PPStateTagName = string.Format("i{0}.EQStatus.PP_SPL", data.Module);
            string reasonCodeTagName = string.Format("i{0}.Availability.ReasonCode", data.Module);
            string descriptionTagName = string.Format("i{0}.Availability.Description", data.Module);

            string sAVAILABILITYSTATE = DataManager.Instance.GET_STRING_DATA(availabilityTagName, out bResult);
            string sINTERLOCKSTATE = DataManager.Instance.GET_STRING_DATA(interlockTagName, out bResult);
            string sMOVESTATE = DataManager.Instance.GET_STRING_DATA(moveStateTagName, out bResult);
            string sRUNSTATE = DataManager.Instance.GET_STRING_DATA(runStateTagName, out bResult);
            string sFRONTSTATE = DataManager.Instance.GET_STRING_DATA(frontStateTagName, out bResult);
            string sREARSTATE = DataManager.Instance.GET_STRING_DATA(rearStateTagName, out bResult);
            string sPP_SPLSTATE = DataManager.Instance.GET_STRING_DATA(PPStateTagName, out bResult);
            string sREASONCODE = DataManager.Instance.GET_STRING_DATA(reasonCodeTagName, out bResult);
            string sDESCRIPTION = DataManager.Instance.GET_STRING_DATA(descriptionTagName, out bResult);

            //RTPID "300"
            string cellIDTagName = string.Format("i{0}.TrackIn.CellID{1}", data.Module, _cellPortNo);
            string productIDTagName = string.Format("i{0}.TrackIn.ProductID{1}", data.Module, _cellPortNo);
            string stepIDTagName = string.Format("i{0}.TrackIn.StepID{1}", data.Module, _cellPortNo);

            string sCELLID = DataManager.Instance.GET_STRING_DATA(cellIDTagName, out bResult);
            string sPPID = RMSManager.Instance.GetCurrentPPID();
            string sPRODUCTID = DataManager.Instance.GET_STRING_DATA(productIDTagName, out bResult);
            string sSTEPID = DataManager.Instance.GET_STRING_DATA(stepIDTagName, out bResult);

            //RTPID "301"
            string processjobIDTagName = string.Format("i{0}.TrackIn.ProcessJobID{1}", data.Module, _cellPortNo);
            string planquantityTagName = string.Format("i{0}.TrackIn.PlanQuantity{1}", data.Module, _cellPortNo);
            string processquantityTagName = string.Format("i{0}.TrackIn.ProcessQuantity{1}", data.Module, _cellPortNo);

            string sPROCESS_JOB = DataManager.Instance.GET_STRING_DATA(processjobIDTagName, out bResult);
            string sPLANQTY = DataManager.Instance.GET_INT_DATA(planquantityTagName, out bResult).ToString();
            string sPROCESSD_QTY = DataManager.Instance.GET_INT_DATA(processquantityTagName, out bResult).ToString();

            //RTPID "400"
            string readerIDIDTagName = string.Format("i{0}.TrackIn.ReaderID{1}", data.Module, _cellPortNo);
            string readerresultcodeTagName = string.Format("i{0}.TrackIn.RRC{1}", data.Module, _cellPortNo);

            string sREADER_ID = DataManager.Instance.GET_STRING_DATA(readerIDIDTagName, out bResult);
            string sREADER_RESULTCODE = DataManager.Instance.GET_STRING_DATA(readerresultcodeTagName, out bResult);

            CommonData.Instance.VALIDATION_DATA.SaveValidationData(KindOfValidation.TRACK_IN_VALIDATION, sCELLID, Convert.ToInt32(_cellPortNo), data.Module);

            #region S6F11(Cell Process Start), CEID:401

            SecsMessage msg = new SecsMessage(Stream, Function, true, Convert.ToInt32(SecsDriver.DeviceID));

            msg.AddList(3);                                                                                     //L3  Cell Start Event Info
            {
                msg.AddAscii(AppUtil.ToAscii(sDATAID, gDefine.DEF_DATAID_SIZE));                                    //A4  Data ID
                msg.AddAscii(AppUtil.ToAscii(sCEID, gDefine.DEF_CEID_SIZE));                                        //A3  Collection Event ID
                msg.AddList(5);                                                                                     //L5  RPTID Set
                {
                    msg.AddList(2);                                                                                     //L2  RPTID 100 Set 
                    {
                        msg.AddAscii(AppUtil.ToAscii("100", gDefine.DEF_RPTID_SIZE));                                       //A3  RPTID="100"
                        msg.AddList(2);                                                                                     //L2  EQP Control State Set
                        {
                            msg.AddAscii(AppUtil.ToAscii(sEQPID, gDefine.DEF_EQPID_SIZE));                                      //A40 HOST REQ EQPID 
                            msg.AddAscii(sCRST);                                                                                //A1  Online Control State 
                        }
                    }
                    msg.AddList(2);                                                                                     //L2  RPTID 101 Set 
                    {
                        msg.AddAscii(AppUtil.ToAscii("101", gDefine.DEF_RPTID_SIZE));                                       //A3  RPTID="101"
                        msg.AddList(9);                                                                                     //L9  EQP State Set
                        {
                            msg.AddAscii(sAVAILABILITYSTATE);                                                                   //A1  EQ Avilability State Info
                            msg.AddAscii(sINTERLOCKSTATE);                                                                      //A1  Interlock Avilability State Info
                            msg.AddAscii(sMOVESTATE);                                                                           //A1  EQ Move State Info
                            msg.AddAscii(sRUNSTATE);                                                                            //A1  Cell existence/nonexistence Check
                            msg.AddAscii(sFRONTSTATE);                                                                          //A1  Upper EQ Processing State
                            msg.AddAscii(sREARSTATE);                                                                           //A1  Lower EQ Processing State
                            msg.AddAscii(sPP_SPLSTATE);                                                                         //A1  Sample Run-Normal Run State
                            msg.AddAscii(AppUtil.ToAscii(sREASONCODE, gDefine.DEF_REASONCODE_SIZE));                            //A20 Code Info
                            msg.AddAscii(AppUtil.ToAscii(sDESCRIPTION, gDefine.DEF_DESCRIPTION_SIZE));                          //A40 EQ Description
                        }
                    }
                    msg.AddList(2);                                                                                     //L2  RPTID 300 Set
                    {
                        msg.AddAscii(AppUtil.ToAscii("300", gDefine.DEF_RPTID_SIZE));                                       //A3  RPTID="300"
                        msg.AddList(4);                                                                                     //L4  CELL Info Set
                        {
                            msg.AddAscii(AppUtil.ToAscii(sCELLID, 40));                                                        //A40 CELL Unique ID 
                            msg.AddAscii(AppUtil.ToAscii(sPPID, gDefine.DEF_PPID_SIZE));                                        //A40 LOT 지PPID
                            msg.AddAscii(AppUtil.ToAscii(sPRODUCTID, gDefine.DEF_PRODUCTID_SIZE));                             //A40 CELL Prodecut ID
                            msg.AddAscii(AppUtil.ToAscii(sSTEPID, 40));                                                        //A40 CELL Step ID
                        }
                    }
                    msg.AddList(2);                                                                                     //L2  RPTID 301 Set
                    {
                        msg.AddAscii(AppUtil.ToAscii("301", gDefine.DEF_RPTID_SIZE));                                       //A3  RPTID="301"
                        msg.AddList(3);                                                                                     //L3  Process JOB Set
                        {
                            msg.AddAscii(AppUtil.ToAscii(sPROCESS_JOB, gDefine.DEF_PROCESS_JOB_SIZE));                          //A40 Process JOB Info
                            msg.AddAscii(AppUtil.ToAscii(sPLANQTY, gDefine.DEF_PLAN_QTY_SIZE));                                //A10 Plan Quantity Info
                            msg.AddAscii(AppUtil.ToAscii(sPROCESSD_QTY, gDefine.DEF_PROCESSED_QTY_SIZE));                      //A10 Processed Quantity Info
                        }
                    }
                    msg.AddList(2);                                                                                     //L2  RPTID 400 Set
                    {
                        msg.AddAscii(AppUtil.ToAscii("400", gDefine.DEF_RPTID_SIZE));                                       //A3  RPTID="400"
                        msg.AddList(2);                                                                                     //L2  Reader Set
                        {
                            msg.AddAscii(AppUtil.ToAscii(sREADER_ID, gDefine.DEF_READER_ID_SIZE));                              //A10 MCR Reader Position/Order Info
                            msg.AddAscii(sREADER_RESULTCODE);                                                                    //A1  Reading Result Value
                        }
                    }
                }
            }
            this.SecsDriver.WriteLogAndSendMessage(msg, sCEID);

            CommonData.Instance.OnStreamFunctionAdd("MAIN", "E->H", "TRACK", "S6F11", "401", string.Format("Cell Start Port #{0}", _cellPortNo), sCELLID);
            LogHelper.Instance.Tracking.DebugFormat("TrackIn,{0},{1},{2}", sCELLID, sREADER_ID, sREADER_RESULTCODE);
            #endregion
        }
    }
}
