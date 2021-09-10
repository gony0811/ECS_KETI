using CIM.Common;
using CIM.Manager;
using SDC.Core;
using SDC.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SYSWIN.Secl;

namespace CIM.Host.Swin.SECSII
{
    public class S6F11_CellProcessStart1 : SFMessage
    {
        /*S6F11(Cell Process Start), CEID:401*/
        public S6F11_CellProcessStart1(SECSDriver driver) 
            : base(driver)
        {
            
        }
        public override void DoWork(string driverName, object obj)
        {
            int stream = 6, function = 11;
            bool bResult;

            string sDataID = "0";
            string sCRST = ((int)DataManager.Instance.GET_INT_DATA("vSys_HostData_Mode", out bResult)).ToString();
            string sEQPID = CommonData.Instance.EQP_SETTINGS.EQPID;
            string sCEID = "401";

            string sAVAILABILITYSTATE = DataManager.Instance.GET_STRING_DATA("iPLC1_EtoC_EQPAvailability", out bResult);
            string sINTERLOCKSTATE = DataManager.Instance.GET_STRING_DATA("iPLC1_EtoC_EQPInterlock", out bResult);
            string sMOVESTATE = DataManager.Instance.GET_STRING_DATA("iPLC1_EtoC_EQPMove", out bResult);
            string sRUNSTATE = DataManager.Instance.GET_STRING_DATA("iPLC1_EtoC_EQPRun", out bResult);
            string sFRONTSTATE = DataManager.Instance.GET_STRING_DATA("iPLC1_EtoC_EQPFront", out bResult);
            string sREARSTATE = DataManager.Instance.GET_STRING_DATA("iPLC1_EtoC_EQPRear", out bResult);
            string sPP_SPLSTATE = DataManager.Instance.GET_STRING_DATA("iPLC1_EtoC_EQPState_PP", out bResult);
            string sREASONCODE = DataManager.Instance.GET_STRING_DATA("iPLC1_EtoC_AvailabilityReasonCode", out bResult);
            string sDESCRIPTION = DataManager.Instance.GET_STRING_DATA("iPLC1_EtoC_AvailabilityDescription", out bResult);

            string sCELL_ID = DataManager.Instance.GET_STRING_DATA("iPLC1_EtoC_TrackInCellID1", out bResult);
            string sPPID = RMSManager.Instance.GetCurrentPPID();
            string sPRODUCT_ID = DataManager.Instance.GET_STRING_DATA("iPLC1_EtoC_TrackInProductID1", out bResult);
            string sSTEP_ID = DataManager.Instance.GET_STRING_DATA("iPLC1_EtoC_TrackInStepID1", out bResult);
            string sPROCESS_JOB = DataManager.Instance.GET_STRING_DATA("iPLC1_EtoC_TrackInProcessJobID1", out bResult);
            string sPLAN_QTY = DataManager.Instance.GET_INT_DATA("iPLC1_EtoC_TrackInPlanQuantity1", out bResult).ToString();
            string sPROCESSED_QTY = DataManager.Instance.GET_INT_DATA("iPLC1_EtoC_TrackInProcessQuantity1", out bResult).ToString();
            string sREADER_ID = DataManager.Instance.GET_STRING_DATA("iPLC1_EtoC_TrackInReaderID1", out bResult);
            string sREAD_RESULT_CODE = DataManager.Instance.GET_STRING_DATA("iPLC1_EtoC_TrackInRRC1", out bResult);

            #region S6F11(Cell Process Start), CEID:401

            SecsMessage msg = new SecsMessage(stream, function, true, Convert.ToInt32(SecsDriver.DeviceID));

            msg.AddList(3);                                                                                     //L3  Cell Start Event Info
            {
                msg.AddAscii(AppUtil.ToAscii(sDataID, gDefine.DEF_DATAID_SIZE));                                    //A4  Data ID
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
                            msg.AddAscii(AppUtil.ToAscii(sCELL_ID, 40));                                                        //A40 CELL Unique ID 
                            msg.AddAscii(AppUtil.ToAscii(sPPID, gDefine.DEF_PPID_SIZE));                                        //A40 LOT 지PPID
                            msg.AddAscii(AppUtil.ToAscii(sPRODUCT_ID, gDefine.DEF_PRODUCTID_SIZE));                             //A40 CELL Prodecut ID
                            msg.AddAscii(AppUtil.ToAscii(sSTEP_ID, 40));                                                        //A40 CELL Step ID
                        }
                    }
                    msg.AddList(2);                                                                                     //L2  RPTID 301 Set
                    {
                        msg.AddAscii(AppUtil.ToAscii("301", gDefine.DEF_RPTID_SIZE));                                       //A3  RPTID="301"
                        msg.AddList(3);                                                                                     //L3  Process JOB Set
                        {
                            msg.AddAscii(AppUtil.ToAscii(sPROCESS_JOB, gDefine.DEF_PROCESS_JOB_SIZE));                          //A40 Process JOB Info
                            msg.AddAscii(AppUtil.ToAscii(sPLAN_QTY, gDefine.DEF_PLAN_QTY_SIZE));                                //A10 Plan Quantity Info
                            msg.AddAscii(AppUtil.ToAscii(sPROCESSED_QTY, gDefine.DEF_PROCESSED_QTY_SIZE));                      //A10 Processed Quantity Info
                        }
                    }
                    msg.AddList(2);                                                                                     //L2  RPTID 400 Set
                    {
                        msg.AddAscii(AppUtil.ToAscii("400", gDefine.DEF_RPTID_SIZE));                                       //A3  RPTID="400"
                        msg.AddList(2);                                                                                     //L2  Reader Set
                        {
                            msg.AddAscii(AppUtil.ToAscii(sREADER_ID, gDefine.DEF_READER_ID_SIZE));                              //A10 MCR Reader Position/Order Info
                            msg.AddAscii(sREAD_RESULT_CODE);                                                                    //A1  Reading Result Value
                        }
                    }
                }
            }
            this.SecsDriver.Send(msg);
            #endregion

            #region CELL PROCESS START WRITE LOG DATA

            //CTrackINLogData logData = new CTrackINLogData();

            //logData.sCRST = sCRST;
            //logData.sCELLID = sCELL_ID;
            //logData.sAVAILABILITY = sAVAILABILITYSTATE;
            //logData.sINTERLOCK = sINTERLOCKSTATE;
            //logData.sMOVESTATE = sMOVESTATE;
            //logData.sRUNSTATE = sRUNSTATE;
            //logData.sPPID = sPPID;
            //logData.sPRODUCT_ID = sPRODUCT_ID;
            //logData.sSTEP_ID = sSTEP_ID;
            //logData.sREADER_ID = sREADER_ID;
            //logData.sREAD_RESULT_CODE = sREAD_RESULT_CODE;

            //this.SetTrackInLog(logData);

            #endregion
        } //401
    }
}
