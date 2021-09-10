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
    class S6F11_CellProcessEnd_CEID406 : SFMessage
    {
        /*S6F11(CellProcessEnd), CEID :406*/

        private string _cellPortNo;

        public S6F11_CellProcessEnd_CEID406(SECSDriverBase driver, string CellPortNo)
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
            string sCEID = "406";

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
            string cellIDTagName = string.Format("i{0}.TrackOut.CellID{1}", data.Module, _cellPortNo);
            string productIDTagName = string.Format("i{0}.TrackOut.ProductID{1}", data.Module, _cellPortNo);
            string stepIDTagName = string.Format("i{0}.TrackOut.StepID{1}", data.Module, _cellPortNo);

            string sCELLID = DataManager.Instance.GET_STRING_DATA(cellIDTagName, out bResult);
            string sPPID = RMSManager.Instance.CurrentPPID;
            string sPRODUCTID = DataManager.Instance.GET_STRING_DATA(productIDTagName, out bResult);
            string sSTEPID = DataManager.Instance.GET_STRING_DATA(stepIDTagName, out bResult);

            //RTPID "301"
            string processjobIDTagName = string.Format("i{0}.TrackOut.ProcessJobID{1}", data.Module, _cellPortNo);
            string planquantityTagName = string.Format("i{0}.TrackOut.PlanQuantity{1}", data.Module, _cellPortNo);
            string processquantityTagName = string.Format("i{0}.TrackOut.ProcessQuantity{1}", data.Module, _cellPortNo);

            string sPROCESS_JOB = DataManager.Instance.GET_STRING_DATA(processjobIDTagName, out bResult);
            string sPLANQTY = DataManager.Instance.GET_INT_DATA(planquantityTagName, out bResult).ToString();
            string sPROCESSD_QTY = DataManager.Instance.GET_INT_DATA(processquantityTagName, out bResult).ToString();

            //RTPID "400"
            string readerIDIDTagName = string.Format("i{0}.TrackOut.ReaderID{1}", data.Module, _cellPortNo);
            string readerresultcodeTagName = string.Format("i{0}.TrackOut.RRC{1}", data.Module, _cellPortNo);

            string sREADER_ID = DataManager.Instance.GET_STRING_DATA(readerIDIDTagName, out bResult);
            string sREADER_RESULTCODE = DataManager.Instance.GET_STRING_DATA(readerresultcodeTagName, out bResult);

            //RTPID "201"
            short nMAT_NO = 2;

            string[] EQmatbatIDTagName = new string[nMAT_NO];
            string[] EQmatbatnameTagName = new string[nMAT_NO];
            string[] EQmatIDTagName = new string[nMAT_NO];
            string[] EQmattypeTagName = new string[nMAT_NO];
            string[] EQmatSTTagName = new string[nMAT_NO];
            string[] EQmatportIDTagName = new string[nMAT_NO];
            string[] EQmatstateTagName = new string[nMAT_NO];
            string[] EQmattotalQTYTagName = new string[nMAT_NO];
            string[] EQmatuseQTYTagName = new string[nMAT_NO];
            string[] EQmatassemQTYTagName = new string[nMAT_NO];
            string[] EQmatngQTYTagName = new string[nMAT_NO];
            string[] EQmatremainQTYTagName = new string[nMAT_NO];
            string[] EQmatproductQTYTagName = new string[nMAT_NO];
            string[] EQmatproceuseQTYTagName = new string[nMAT_NO];
            string[] EQmatproceassemQTYTagName = new string[nMAT_NO];
            string[] EQmatprocngQTYTagName = new string[nMAT_NO];
            string[] EQmatsupplyrequestQTYTagName = new string[nMAT_NO];

            for (int i = 0; i < nMAT_NO; i++)
            {
                EQmatbatIDTagName[i] = string.Format("i{0}.MaterialTrackOut.MaterialBatchID{1}_{2}", data.Module, _cellPortNo, i+1);
                EQmatbatnameTagName[i] = string.Format("i{0}.MaterialTrackOut.MaterialBatchName{1}_{2}", data.Module, _cellPortNo, i+1);
                EQmatIDTagName[i] = string.Format("i{0}.MaterialTrackOut.MaterialID{1}_{2}", data.Module, _cellPortNo, i+1);
                EQmattypeTagName[i] = string.Format("i{0}.MaterialTrackOut.MaterialType{1}_{2}", data.Module, _cellPortNo, i+1);
                EQmatSTTagName[i] = string.Format("i{0}.MaterialTrackOut.MaterialST{1}_{2}", data.Module, _cellPortNo, i+1);
                EQmatportIDTagName[i] = string.Format("i{0}.MaterialTrackOut.MaterialPortID{1}_{2}", data.Module, _cellPortNo, i+1);
                EQmatstateTagName[i] = string.Format("i{0}.MaterialTrackOut.MaterialState{1}_{2}", data.Module, _cellPortNo, i+1);
                EQmattotalQTYTagName[i] = string.Format("i{0}.MaterialTrackOut.MaterialTotalQTY{1}_{2}", data.Module, _cellPortNo, i+1);
                EQmatuseQTYTagName[i] = string.Format("i{0}.MaterialTrackOut.MaterialUseQTY{1}_{2}", data.Module, _cellPortNo, i+1);
                EQmatassemQTYTagName[i] = string.Format("i{0}.MaterialTrackOut.MaterialAssemQTY{1}_{2}", data.Module, _cellPortNo, i+1);
                EQmatngQTYTagName[i] = string.Format("i{0}.MaterialTrackOut.MaterialNGQTY{1}_{2}", data.Module, _cellPortNo, i+1);
                EQmatremainQTYTagName[i] = string.Format("i{0}.MaterialTrackOut.MaterialRemainQTY{1}_{2}", data.Module, _cellPortNo, i+1);
                EQmatproductQTYTagName[i] = string.Format("i{0}.MaterialTrackOut.MaterialProductQTY{1}_{2}", data.Module, _cellPortNo, i+1);
                EQmatproceuseQTYTagName[i] = string.Format("i{0}.MaterialTrackOut.MaterialProcessUseQTY{1}_{2}", data.Module, _cellPortNo, i+1);
                EQmatproceassemQTYTagName[i] = string.Format("i{0}.MaterialTrackOut.MaterialProcessAssemQTY{1}_{2}", data.Module, _cellPortNo, i+1);
                EQmatprocngQTYTagName[i] = string.Format("i{0}.MaterialTrackOut.MaterialProcessNGQTY{1}_{2}", data.Module, _cellPortNo, i+1);
                EQmatsupplyrequestQTYTagName[i] = string.Format("i{0}.MaterialTrackOut.MaterialSupplyRequestQTY{1}_{2}", data.Module, _cellPortNo, i+1);
            }

            string[] aEQ_MAT_BAT_ID = new string[nMAT_NO];
            string[] aEQ_MAT_BAT_NAME = new string[nMAT_NO];
            string[] aEQ_MAT_ID = new string[nMAT_NO];
            string[] aEQ_MAT_TYPE = new string[nMAT_NO];
            string[] aEQ_MAT_ST = new string[nMAT_NO];
            string[] aEQ_MAT_PORT_ID = new string[nMAT_NO];
            string[] aEQ_MAT_STATE = new string[nMAT_NO];
            int[] aEQ_MAT_TOTAL_QTY = new int[nMAT_NO];
            int[] aEQ_MAT_USE_QTY = new int[nMAT_NO];
            int[] aEQ_MAT_ASSEM_QTY = new int[nMAT_NO];
            int[] aEQ_MAT_NG_QTY = new int[nMAT_NO];
            int[] aEQ_MAT_REMAINQTY_QTY = new int[nMAT_NO];
            int[] aEQ_MAT_PRODUCT_QTY = new int[nMAT_NO];
            int[] aEQ_MAT_PROCE_USE_QTY = new int[nMAT_NO];
            int[] aEQ_MAT_PROCE_ASSEM_QTY = new int[nMAT_NO];
            int[] aEQ_MAT_PROC_NG_QTY = new int[nMAT_NO];
            int[] aEQ_MAT_SUPPLY_REQUEST_QTY = new int[nMAT_NO];

            for (int i = 0; i < nMAT_NO; i++)
            {
                aEQ_MAT_BAT_ID[i] = "";
                aEQ_MAT_BAT_NAME[i] = "";
                aEQ_MAT_ID[i] = "";
                aEQ_MAT_TYPE[i] = "";
                aEQ_MAT_ST[i] = "";
                aEQ_MAT_PORT_ID[i] = "";
                aEQ_MAT_STATE[i] = "";
                aEQ_MAT_TOTAL_QTY[i] = 0;
                aEQ_MAT_USE_QTY[i] = 0;
                aEQ_MAT_ASSEM_QTY[i] = 0;
                aEQ_MAT_NG_QTY[i] = 0;
                aEQ_MAT_REMAINQTY_QTY[i] = 0;
                aEQ_MAT_PRODUCT_QTY[i] = 0;
                aEQ_MAT_PROCE_USE_QTY[i] = 0;
                aEQ_MAT_PROCE_ASSEM_QTY[i] = 0;
                aEQ_MAT_PROC_NG_QTY[i] = 0;
                aEQ_MAT_SUPPLY_REQUEST_QTY[i] = 0;
            }

            for (int i = 0; i < nMAT_NO; i++)
            {
                aEQ_MAT_BAT_ID[i] = DataManager.Instance.GET_STRING_DATA(EQmatbatIDTagName[i], out bResult);
                aEQ_MAT_BAT_NAME[i] = DataManager.Instance.GET_STRING_DATA(EQmatbatnameTagName[i], out bResult);
                aEQ_MAT_ID[i] = DataManager.Instance.GET_STRING_DATA(EQmatIDTagName[i], out bResult);
                aEQ_MAT_TYPE[i] = DataManager.Instance.GET_STRING_DATA(EQmattypeTagName[i], out bResult);
                aEQ_MAT_ST[i] = DataManager.Instance.GET_STRING_DATA(EQmatSTTagName[i], out bResult);
                aEQ_MAT_PORT_ID[i] = DataManager.Instance.GET_STRING_DATA(EQmatportIDTagName[i], out bResult);
                aEQ_MAT_STATE[i] = DataManager.Instance.GET_STRING_DATA(EQmatstateTagName[i], out bResult);
                aEQ_MAT_TOTAL_QTY[i] = DataManager.Instance.GET_INT_DATA(EQmattotalQTYTagName[i], out bResult);
                aEQ_MAT_USE_QTY[i] = DataManager.Instance.GET_INT_DATA(EQmatuseQTYTagName[i], out bResult);
                aEQ_MAT_ASSEM_QTY[i] = DataManager.Instance.GET_INT_DATA(EQmatassemQTYTagName[i], out bResult);
                aEQ_MAT_NG_QTY[i] = DataManager.Instance.GET_INT_DATA(EQmatngQTYTagName[i], out bResult);
                aEQ_MAT_REMAINQTY_QTY[i] = DataManager.Instance.GET_INT_DATA(EQmatremainQTYTagName[i], out bResult);
                aEQ_MAT_PRODUCT_QTY[i] = DataManager.Instance.GET_INT_DATA(EQmatproductQTYTagName[i], out bResult);
                aEQ_MAT_PROCE_USE_QTY[i] = DataManager.Instance.GET_INT_DATA(EQmatproceuseQTYTagName[i], out bResult);
                aEQ_MAT_PROCE_ASSEM_QTY[i] = DataManager.Instance.GET_INT_DATA(EQmatproceassemQTYTagName[i], out bResult);
                aEQ_MAT_PROC_NG_QTY[i] = DataManager.Instance.GET_INT_DATA(EQmatprocngQTYTagName[i], out bResult);
                aEQ_MAT_SUPPLY_REQUEST_QTY[i] = DataManager.Instance.GET_INT_DATA(EQmatsupplyrequestQTYTagName[i], out bResult);
            }

            //RTPID "501"
            string operatorIDTagName = string.Format("i{0}.TrackOut.OperatorID{1}", data.Module, _cellPortNo);
            string judgeTagName = string.Format("i{0}.TrackOut.Judge{1}", data.Module, _cellPortNo);
            string reasoncodeTagName = string.Format("i{0}.TrackOut.ReasonCode{1}", data.Module, _cellPortNo);
            string description2TagName = string.Format("i{0}.TrackOut.Description{1}", data.Module, _cellPortNo);

            string sOPERATOR_ID_1 = DataManager.Instance.GET_STRING_DATA(operatorIDTagName, out bResult);
            string sOPERATOR_ID_2 = "";
            string sOPERATOR_ID_3 = "";
            string sJUDGE = DataManager.Instance.GET_STRING_DATA(judgeTagName, out bResult);
            string sREASONCODE2 = DataManager.Instance.GET_STRING_DATA(reasoncodeTagName, out bResult);
            string sDESCRIPTION2 = DataManager.Instance.GET_STRING_DATA(description2TagName, out bResult);

            #region S6F11(Cell Process End), CEID:406

            SecsMessage msg = new SecsMessage(Stream, Function, true, Convert.ToInt32(SecsDriver.DeviceID));

            msg.AddList(3);                                                                                     //L3  Cell Complete Event Info
            {
                msg.AddAscii(AppUtil.ToAscii(sDATAID, gDefine.DEF_DATAID_SIZE));                                    //A4  Data ID
                msg.AddAscii(AppUtil.ToAscii(sCEID, gDefine.DEF_CEID_SIZE));                                        //A3  Collection Event ID
                msg.AddList(8);                                                                                     //L8  RPTID Set
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
                        msg.AddList(4);                                                                                     //L4  Cell Info Set
                        {
                            msg.AddAscii(AppUtil.ToAscii(sCELLID, 40));                                                         //A40 Cell Unique ID
                            msg.AddAscii(AppUtil.ToAscii(sPPID, gDefine.DEF_PPID_SIZE));                                        //A40 LOT 지PPID
                            msg.AddAscii(AppUtil.ToAscii(sPRODUCTID, gDefine.DEF_PRODUCTID_SIZE));                              //A40 CELL Product Info
                            msg.AddAscii(AppUtil.ToAscii(sSTEPID, 40));                                                         //A40 CELL STEP ID Info
                        }
                    }
                    msg.AddList(2);                                                                                     //L2  RPTID 301 Set
                    {
                        msg.AddAscii(AppUtil.ToAscii("301", gDefine.DEF_RPTID_SIZE));                                       //A3  RPTID="301"
                        msg.AddList(3);                                                                                     //L3  Process JOB Set
                        {
                            msg.AddAscii(AppUtil.ToAscii(sPROCESS_JOB, gDefine.DEF_PROCESS_JOB_SIZE));                          //A40 Process JOB Info
                            msg.AddAscii(AppUtil.ToAscii(sPLANQTY, gDefine.DEF_PLAN_QTY_SIZE));                                 //A10 Plan Quantity Info
                            msg.AddAscii(AppUtil.ToAscii(sPROCESSD_QTY, gDefine.DEF_PROCESSED_QTY_SIZE));                       //A10 Processed Quantity Info
                        }
                    }
                    msg.AddList(2);                                                                                     //L2  RPTID 400 Set
                    {
                        msg.AddAscii(AppUtil.ToAscii("400", gDefine.DEF_RPTID_SIZE));                                       //A3  RPTID="400"
                        msg.AddList(2);                                                                                     //L2  Reader Set
                        {
                            msg.AddAscii(AppUtil.ToAscii(sREADER_ID, gDefine.DEF_READER_ID_SIZE));                              //A10 MCR Reader Position/Order Info
                            msg.AddAscii(sREADER_RESULTCODE);                                                                   //A1  Reading Result Value
                        }
                    }
                    msg.AddList(2);                                                                                     //L2 RPTID 201 Set
                    {
                        msg.AddAscii(AppUtil.ToAscii("201", gDefine.DEF_RPTID_SIZE));                                       //A3 RPTID="201"
                        msg.AddList(nMAT_NO);                                                                               //Ln Material No
                        {
                            if (nMAT_NO != 0)
                            {
                                for (int i = 0; i < nMAT_NO; i++)
                                {
                                    msg.AddList(17);                                                                                //L17 Material Info Set
                                    {
                                        msg.AddAscii(AppUtil.ToAscii(aEQ_MAT_BAT_ID[i], 40));                                           //A40 Batch MaterialID 
                                        msg.AddAscii(AppUtil.ToAscii(aEQ_MAT_BAT_NAME[i], 40));                                         //A40 Batch Material Code
                                        msg.AddAscii(AppUtil.ToAscii(aEQ_MAT_ID[i], 40));                                               //A40 Material ID
                                        msg.AddAscii(AppUtil.ToAscii(aEQ_MAT_TYPE[i], 20));                                             //A20 Material Type
                                        msg.AddAscii(AppUtil.ToAscii(aEQ_MAT_ST[i], 1));                                                //A1  Material State Info
                                        msg.AddAscii(AppUtil.ToAscii(aEQ_MAT_PORT_ID[i], 1));                                           //A1  Material Port ID Info
                                        msg.AddAscii(AppUtil.ToAscii(aEQ_MAT_STATE[i], 1));                                             //A1  Material Avilability State Info
                                        msg.AddAscii(AppUtil.ToAscii(aEQ_MAT_TOTAL_QTY[i].ToString(), 10));                                        //A10 Material Batch Quantity
                                        msg.AddAscii(AppUtil.ToAscii(aEQ_MAT_USE_QTY[i].ToString(), 10));                                          //A10 Material Batch Cumulative Usage Quantity
                                        msg.AddAscii(AppUtil.ToAscii(aEQ_MAT_ASSEM_QTY[i].ToString(), 10));                                        //A10 Material Batch Cumulative Attachment Quantity
                                        msg.AddAscii(AppUtil.ToAscii(aEQ_MAT_NG_QTY[i].ToString(), 10));                                           //A10 Material Batch Cumulative NG Quantity
                                        msg.AddAscii(AppUtil.ToAscii(aEQ_MAT_REMAINQTY_QTY[i].ToString(), 10));                                    //A10 Remaining Material Quantity 
                                        msg.AddAscii(AppUtil.ToAscii(aEQ_MAT_PRODUCT_QTY[i].ToString(), 10));                                      //A10 Material Process Standard Quantity
                                        msg.AddAscii(AppUtil.ToAscii(aEQ_MAT_PROCE_USE_QTY[i].ToString(), 10));                                    //A10 Material Process Usage Quantity
                                        msg.AddAscii(AppUtil.ToAscii(aEQ_MAT_PROCE_ASSEM_QTY[i].ToString(), 10));                                  //A10 Material Process Attachment Quantity
                                        msg.AddAscii(AppUtil.ToAscii(aEQ_MAT_PROC_NG_QTY[i].ToString(), 10));                                      //A10 Material Process NG Quantity
                                        msg.AddAscii(AppUtil.ToAscii(aEQ_MAT_SUPPLY_REQUEST_QTY[i].ToString(), 10));                               //A10 Material Supply Quest Quantity
                                    }
                                }
                            }
                        }
                    }
                    msg.AddList(2);                                                                                     //L2 RPTID 600 Set
                    {
                        msg.AddAscii(AppUtil.ToAscii("600", gDefine.DEF_RPTID_SIZE));                                       //A3  RPTID="600"
                        msg.AddList(0);                                                                                //Lm  DV
                    }
                    msg.AddList(2);                                                                                     //L2 RPTID 501 Set
                    {
                        msg.AddAscii(AppUtil.ToAscii("501", gDefine.DEF_RPTID_SIZE));                                       //A3  RPTID="501"
                        msg.AddList(6);                                                                                     //L6  Judge Set
                        {
                            msg.AddAscii(AppUtil.ToAscii(sOPERATOR_ID_1, 20));                                                  //A20 1st OPERATOR ID
                            msg.AddAscii(AppUtil.ToAscii(sOPERATOR_ID_2, 20));                                                  //A20 2nd OPERATOR ID
                            msg.AddAscii(AppUtil.ToAscii(sOPERATOR_ID_3, 20));                                                  //A20 3rd OPERATOR ID
                            msg.AddAscii(AppUtil.ToAscii(sJUDGE, 1));                                                           //A1  JUDGE
                            msg.AddAscii(AppUtil.ToAscii(sREASONCODE2, gDefine.DEF_REASONCODE_SIZE));                           //A20 CODE Info
                            msg.AddAscii(AppUtil.ToAscii(sDESCRIPTION2, gDefine.DEF_DESCRIPTION_SIZE));                         //A40 DESCRIPTION
                        }
                    }
                }
            }
            this.SecsDriver.WriteLogAndSendMessage(msg, sCEID);

            CommonData.Instance.OnStreamFunctionAdd("MAIN", "E->H", "TRACK", "S6F11", "406", string.Format("Cell Process Complete #{0}", _cellPortNo), sCELLID);
            LogHelper.Instance.Tracking.DebugFormat("TrackOut,{0},{1},{2}, {3}, {4}", sCELLID, sREADER_ID, sREADER_RESULTCODE, sJUDGE, sDESCRIPTION2);
            #endregion
        }
    }
}
