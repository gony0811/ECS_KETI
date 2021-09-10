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
    public class S6F11_CellProcessEnd2 : SFMessage
    {
        /*S6F11(Cell Process End), CEID:406*/
        public S6F11_CellProcessEnd2(SECSDriver driver) 
            : base(driver)
        {
            
        }
        public override void DoWork(string driverName, object obj)
        {
            int stream = 6, function = 11;
            bool bResult;

            #region Variable
            string sDataID = "0";
            string sCRST = ((int)CommonData.Instance.HOST_MODE).ToString();
            string sEQPID = CommonData.Instance.EQP_SETTINGS.EQPID;
            string sCEID = "406";

            string sAVAILABILITYSTATE = DataManager.Instance.GET_STRING_DATA("iPLC2_EtoC_EQPAvailability", out bResult);
            string sINTERLOCKSTATE = DataManager.Instance.GET_STRING_DATA("iPLC2_EtoC_EQPInterlock", out bResult);
            string sMOVESTATE = DataManager.Instance.GET_STRING_DATA("iPLC2_EtoC_EQPMove", out bResult);
            string sRUNSTATE = DataManager.Instance.GET_STRING_DATA("iPLC2_EtoC_EQPRun", out bResult);
            string sFRONTSTATE = DataManager.Instance.GET_STRING_DATA("iPLC2_EtoC_EQPFront", out bResult);
            string sREARSTATE = DataManager.Instance.GET_STRING_DATA("iPLC2_EtoC_EQPRear", out bResult);
            string sPP_SPLSTATE = DataManager.Instance.GET_STRING_DATA("iPLC2_EtoC_EQPState_PP", out bResult);
            string sREASONCODE = DataManager.Instance.GET_STRING_DATA("iPLC2_EtoC_AvailabilityReasonCode", out bResult);
            string sDESCRIPTION = DataManager.Instance.GET_STRING_DATA("iPLC2_EtoC_AvailabilityDescription", out bResult);

            // 150214 HYUNG-JUN Modify
            // RPTID "300"
            string sCELLID = string.Empty;
            string sPPID = RMSManager.Instance.GetCurrentPPID();
            string sPRODUCTID = string.Empty;
            string sSTEPID = string.Empty;
            // RPTID "301"
            string sPROCESS_JOB = string.Empty;
            string sPLANQTY = string.Empty;
            string sPROCESSD_QTY = string.Empty;
            // RPTID "400"
            string sREADER_ID = string.Empty;
            string sREADER_RESULTCODE = string.Empty;
            // RTPID "501"
            string sOPERATOR_ID_1 = string.Empty;
            string sOPERATOR_ID_2 = string.Empty;
            string sOPERATOR_ID_3 = string.Empty;
            string sJUDGE = string.Empty;
            string sREASONCODE2 = string.Empty;
            string sDESCRIPTION2 = string.Empty;

            int nMAT_NO = DataManager.Instance.GET_INT_DATA("vSys_PLC1_MaterialCount", out bResult) + DataManager.Instance.GET_INT_DATA("vSys_PLC2_MaterialCount", out bResult) + DataManager.Instance.GET_INT_DATA("vSys_PLC3_MaterialCount", out bResult) + DataManager.Instance.GET_INT_DATA("vSys_PLC4_MaterialCount", out bResult) + DataManager.Instance.GET_INT_DATA("vSys_PLC5_MaterialCount", out bResult) + DataManager.Instance.GET_INT_DATA("vSys_PLC6_MaterialCount", out bResult) + DataManager.Instance.GET_INT_DATA("vSys_PLC7_MaterialCount", out bResult) + DataManager.Instance.GET_INT_DATA("vSys_PLC8_MaterialCount", out bResult);

            string[] aEQ_MAT_BAT_ID = new string[nMAT_NO];
            string[] aEQ_MAT_BAT_NAME = new string[nMAT_NO];
            string[] aEQ_MAT_ID = new string[nMAT_NO];
            string[] aEQ_MAT_TYPE = new string[nMAT_NO];
            string[] aEQ_MAT_ST = new string[nMAT_NO];
            string[] aEQ_MAT_PORT_ID = new string[nMAT_NO];
            string[] aEQ_MAT_STATE = new string[nMAT_NO];
            string[] aEQ_MAT_TOTAL_QTY = new string[nMAT_NO];
            string[] aEQ_MAT_USE_QTY = new string[nMAT_NO];
            string[] aEQ_MAT_ASSEM_QTY = new string[nMAT_NO];
            string[] aEQ_MAT_NG_QTY = new string[nMAT_NO];
            string[] aEQ_MAT_REMAINQTY_QTY = new string[nMAT_NO];
            string[] aEQ_MAT_PRODUCT_QTY = new string[nMAT_NO];
            string[] aEQ_MAT_PROCE_USE_QTY = new string[nMAT_NO];
            string[] aEQ_MAT_PROCE_ASSEM_QTY = new string[nMAT_NO];
            string[] aEQ_MAT_PROC_NG_QTY = new string[nMAT_NO];
            string[] aEQ_MAT_SUPPLY_REQUEST_QTY = new string[nMAT_NO];

            // RPTID "300"
            sCELLID = DataManager.Instance.GET_STRING_DATA("iPLC2_EtoC_TrackOutCellID2", out bResult);
            sPRODUCTID = DataManager.Instance.GET_STRING_DATA("iPLC2_EtoC_TrackOutProductID2", out bResult);
            sSTEPID = DataManager.Instance.GET_STRING_DATA("iPLC2_EtoC_TrackOutStepID2", out bResult);
            // RPTID "301"
            sPROCESS_JOB = DataManager.Instance.GET_STRING_DATA("iPLC2_EtoC_TrackOutProcessJobID2", out bResult);
            sPLANQTY = DataManager.Instance.GET_INT_DATA("iPLC2_EtoC_TrackOutPlanQuantity2", out bResult).ToString();
            sPROCESSD_QTY = DataManager.Instance.GET_INT_DATA("iPLC2_EtoC_TrackOutProcessQuantity2", out bResult).ToString();
            // RPTID "400"
            sREADER_ID = DataManager.Instance.GET_STRING_DATA("iPLC2_EtoC_TrackOutReaderID2", out bResult);
            sREADER_RESULTCODE = DataManager.Instance.GET_STRING_DATA("iPLC2_EtoC_TrackOutRRC2", out bResult);
            // RTPID "501"
            sOPERATOR_ID_1 = DataManager.Instance.GET_STRING_DATA("iPLC2_EtoC_TrackOutOperatorID2_1", out bResult);
            sOPERATOR_ID_2 = DataManager.Instance.GET_STRING_DATA("iPLC2_EtoC_TrackOutOperatorID2_1", out bResult);
            sOPERATOR_ID_3 = DataManager.Instance.GET_STRING_DATA("iPLC2_EtoC_TrackOutOperatorID2_1", out bResult);
            sJUDGE = DataManager.Instance.GET_STRING_DATA("iPLC2_EtoC_TrackOutJudge2", out bResult);
            sREASONCODE2 = DataManager.Instance.GET_STRING_DATA("iPLC2_EtoC_TrackOutReasonCode2", out bResult);
            sDESCRIPTION2 = DataManager.Instance.GET_STRING_DATA("iPLC2_EtoC_TrackOutDescription2", out bResult);
            // RPTID "201"
            if (nMAT_NO != 0)
            {
                aEQ_MAT_BAT_ID[0] = DataManager.Instance.GET_STRING_DATA("iPLC2_EtoC_TROUT2_1_BatchID", out bResult);
                aEQ_MAT_BAT_NAME[0] = DataManager.Instance.GET_STRING_DATA("iPLC2_EtoC_TROUT2_1_BatchName", out bResult);
                aEQ_MAT_ID[0] = DataManager.Instance.GET_STRING_DATA("iPLC2_EtoC_TROUT2_1_ID", out bResult);
                aEQ_MAT_TYPE[0] = DataManager.Instance.GET_STRING_DATA("iPLC2_EtoC_TROUT2_1_Type", out bResult);
                aEQ_MAT_ST[0] = DataManager.Instance.GET_STRING_DATA("iPLC2_EtoC_TROUT2_1_ST", out bResult);
                aEQ_MAT_PORT_ID[0] = DataManager.Instance.GET_STRING_DATA("iPLC2_EtoC_TROUT2_1_PortID", out bResult);
                aEQ_MAT_STATE[0] = DataManager.Instance.GET_STRING_DATA("iPLC2_EtoC_TROUT2_1_State", out bResult);
                aEQ_MAT_TOTAL_QTY[0] = DataManager.Instance.GET_INT_DATA("iPLC2_EtoC_TROUT2_1_TotalQty", out bResult).ToString();
                aEQ_MAT_USE_QTY[0] = DataManager.Instance.GET_INT_DATA("iPLC2_EtoC_TROUT2_1_UseQty", out bResult).ToString();
                aEQ_MAT_ASSEM_QTY[0] = DataManager.Instance.GET_INT_DATA("iPLC2_EtoC_TROUT2_1_AssemQty", out bResult).ToString();
                aEQ_MAT_NG_QTY[0] = DataManager.Instance.GET_INT_DATA("iPLC2_EtoC_TROUT2_1_NGQty", out bResult).ToString();
                aEQ_MAT_REMAINQTY_QTY[0] = DataManager.Instance.GET_INT_DATA("iPLC2_EtoC_TROUT2_1_RemainQty", out bResult).ToString();
                aEQ_MAT_PRODUCT_QTY[0] = DataManager.Instance.GET_INT_DATA("iPLC2_EtoC_TROUT2_1_ProductQty", out bResult).ToString();
                aEQ_MAT_PROCE_USE_QTY[0] = DataManager.Instance.GET_INT_DATA("iPLC2_EtoC_TROUT2_1_ProcessUseQty", out bResult).ToString();
                aEQ_MAT_PROCE_ASSEM_QTY[0] = DataManager.Instance.GET_INT_DATA("iPLC2_EtoC_TROUT2_1_ProcessAssemQty", out bResult).ToString();
                aEQ_MAT_PROC_NG_QTY[0] = DataManager.Instance.GET_INT_DATA("iPLC2_EtoC_TROUT2_1_ProcessNGQty", out bResult).ToString();
                aEQ_MAT_SUPPLY_REQUEST_QTY[0] = DataManager.Instance.GET_INT_DATA("iPLC2_EtoC_TROUT2_1_SupplyReqQty", out bResult).ToString();

                if (nMAT_NO >= 2)
                {
                    aEQ_MAT_BAT_ID[1] = DataManager.Instance.GET_STRING_DATA("iPLC2_EtoC_TROUT2_2_BatchID", out bResult);
                    aEQ_MAT_BAT_NAME[1] = DataManager.Instance.GET_STRING_DATA("iPLC2_EtoC_TROUT2_2_BatchName", out bResult);
                    aEQ_MAT_ID[1] = DataManager.Instance.GET_STRING_DATA("iPLC2_EtoC_TROUT2_2_ID", out bResult);
                    aEQ_MAT_TYPE[1] = DataManager.Instance.GET_STRING_DATA("iPLC2_EtoC_TROUT2_2_Type", out bResult);
                    aEQ_MAT_ST[1] = DataManager.Instance.GET_STRING_DATA("iPLC2_EtoC_TROUT2_2_ST", out bResult);
                    aEQ_MAT_PORT_ID[1] = DataManager.Instance.GET_STRING_DATA("iPLC2_EtoC_TROUT2_2_PortID", out bResult);
                    aEQ_MAT_STATE[1] = DataManager.Instance.GET_STRING_DATA("iPLC2_EtoC_TROUT2_2_State", out bResult);
                    aEQ_MAT_TOTAL_QTY[1] = DataManager.Instance.GET_INT_DATA("iPLC2_EtoC_TROUT2_2_TotalQty", out bResult).ToString();
                    aEQ_MAT_USE_QTY[1] = DataManager.Instance.GET_INT_DATA("iPLC2_EtoC_TROUT2_2_UseQty", out bResult).ToString();
                    aEQ_MAT_ASSEM_QTY[1] = DataManager.Instance.GET_INT_DATA("iPLC2_EtoC_TROUT2_2_AssemQty", out bResult).ToString();
                    aEQ_MAT_NG_QTY[1] = DataManager.Instance.GET_INT_DATA("iPLC2_EtoC_TROUT2_2_NGQty", out bResult).ToString();
                    aEQ_MAT_REMAINQTY_QTY[1] = DataManager.Instance.GET_INT_DATA("iPLC2_EtoC_TROUT2_2_RemainQty", out bResult).ToString();
                    aEQ_MAT_PRODUCT_QTY[1] = DataManager.Instance.GET_INT_DATA("iPLC2_EtoC_TROUT2_2_ProductQty", out bResult).ToString();
                    aEQ_MAT_PROCE_USE_QTY[1] = DataManager.Instance.GET_INT_DATA("iPLC2_EtoC_TROUT2_2_ProcessUseQty", out bResult).ToString();
                    aEQ_MAT_PROCE_ASSEM_QTY[1] = DataManager.Instance.GET_INT_DATA("iPLC2_EtoC_TROUT2_2_ProcessAssemQty", out bResult).ToString();
                    aEQ_MAT_PROC_NG_QTY[1] = DataManager.Instance.GET_INT_DATA("iPLC2_EtoC_TROUT2_2_ProcessNGQty", out bResult).ToString();
                    aEQ_MAT_SUPPLY_REQUEST_QTY[1] = DataManager.Instance.GET_INT_DATA("iPLC2_EtoC_TROUT2_2_SupplyReqQty", out bResult).ToString();
                }
                if (nMAT_NO >= 3)
                {
                    aEQ_MAT_BAT_ID[2] = DataManager.Instance.GET_STRING_DATA("iPLC2_EtoC_TROUT2_3_BatchID", out bResult);
                    aEQ_MAT_BAT_NAME[2] = DataManager.Instance.GET_STRING_DATA("iPLC2_EtoC_TROUT2_3_BatchName", out bResult);
                    aEQ_MAT_ID[2] = DataManager.Instance.GET_STRING_DATA("iPLC2_EtoC_TROUT2_3_ID", out bResult);
                    aEQ_MAT_TYPE[2] = DataManager.Instance.GET_STRING_DATA("iPLC2_EtoC_TROUT2_3_Type", out bResult);
                    aEQ_MAT_ST[2] = DataManager.Instance.GET_STRING_DATA("iPLC2_EtoC_TROUT2_3_ST", out bResult);
                    aEQ_MAT_PORT_ID[2] = DataManager.Instance.GET_STRING_DATA("iPLC2_EtoC_TROUT2_3_PortID", out bResult);
                    aEQ_MAT_STATE[2] = DataManager.Instance.GET_STRING_DATA("iPLC2_EtoC_TROUT2_3_State", out bResult);
                    aEQ_MAT_TOTAL_QTY[2] = DataManager.Instance.GET_INT_DATA("iPLC2_EtoC_TROUT2_3_TotalQty", out bResult).ToString();
                    aEQ_MAT_USE_QTY[2] = DataManager.Instance.GET_INT_DATA("iPLC2_EtoC_TROUT2_3_UseQty", out bResult).ToString();
                    aEQ_MAT_ASSEM_QTY[2] = DataManager.Instance.GET_INT_DATA("iPLC2_EtoC_TROUT2_3_AssemQty", out bResult).ToString();
                    aEQ_MAT_NG_QTY[2] = DataManager.Instance.GET_INT_DATA("iPLC2_EtoC_TROUT2_3_NGQty", out bResult).ToString();
                    aEQ_MAT_REMAINQTY_QTY[2] = DataManager.Instance.GET_INT_DATA("iPLC2_EtoC_TROUT2_3_RemainQty", out bResult).ToString();
                    aEQ_MAT_PRODUCT_QTY[2] = DataManager.Instance.GET_INT_DATA("iPLC2_EtoC_TROUT2_3_ProductQty", out bResult).ToString();
                    aEQ_MAT_PROCE_USE_QTY[2] = DataManager.Instance.GET_INT_DATA("iPLC2_EtoC_TROUT2_3_ProcessUseQty", out bResult).ToString();
                    aEQ_MAT_PROCE_ASSEM_QTY[2] = DataManager.Instance.GET_INT_DATA("iPLC2_EtoC_TROUT2_3_ProcessAssemQty", out bResult).ToString();
                    aEQ_MAT_PROC_NG_QTY[2] = DataManager.Instance.GET_INT_DATA("iPLC2_EtoC_TROUT2_3_ProcessNGQty", out bResult).ToString();
                    aEQ_MAT_SUPPLY_REQUEST_QTY[2] = DataManager.Instance.GET_INT_DATA("iPLC2_EtoC_TROUT2_3_SupplyReqQty", out bResult).ToString();
                }
            }
            // RTPID "600"
            int nDV_NO = 0;

            Dictionary<string, string> dvNameValueList = DVManager.Instance.GetDVNameValueList("PLC2", "PORT2");
            nDV_NO = dvNameValueList.Count;
            List<string> aDV_NAME = new List<string>(nDV_NO);
            List<string> aDV = new List<string>(nDV_NO);
            #endregion

            #region S6F11(Cell Process End), CEID:406
            SecsMessage msg = new SecsMessage(stream, function, true, Convert.ToInt32(SecsDriver.DeviceID));

            msg.AddList(3);                                                                                     //L3  Cell Complete Event Info
            {
                msg.AddAscii(AppUtil.ToAscii(sDataID, gDefine.DEF_DATAID_SIZE));                                    //A4  Data ID
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
                                        msg.AddAscii(AppUtil.ToAscii(aEQ_MAT_TOTAL_QTY[i], 10));                                        //A10 Material Batch Quantity
                                        msg.AddAscii(AppUtil.ToAscii(aEQ_MAT_USE_QTY[i], 10));                                          //A10 Material Batch Cumulative Usage Quantity
                                        msg.AddAscii(AppUtil.ToAscii(aEQ_MAT_ASSEM_QTY[i], 10));                                        //A10 Material Batch Cumulative Attachment Quantity
                                        msg.AddAscii(AppUtil.ToAscii(aEQ_MAT_NG_QTY[i], 10));                                           //A10 Material Batch Cumulative NG Quantity
                                        msg.AddAscii(AppUtil.ToAscii(aEQ_MAT_REMAINQTY_QTY[i], 10));                                    //A10 Remaining Material Quantity 
                                        msg.AddAscii(AppUtil.ToAscii(aEQ_MAT_PRODUCT_QTY[i], 10));                                      //A10 Material Process Standard Quantity
                                        msg.AddAscii(AppUtil.ToAscii(aEQ_MAT_PROCE_USE_QTY[i], 10));                                    //A10 Material Process Usage Quantity
                                        msg.AddAscii(AppUtil.ToAscii(aEQ_MAT_PROCE_ASSEM_QTY[i], 10));                                  //A10 Material Process Attachment Quantity
                                        msg.AddAscii(AppUtil.ToAscii(aEQ_MAT_PROC_NG_QTY[i], 10));                                      //A10 Material Process NG Quantity
                                        msg.AddAscii(AppUtil.ToAscii(aEQ_MAT_SUPPLY_REQUEST_QTY[i], 10));                               //A10 Material Supply Quest Quantity
                                    }
                                }
                            }
                        }
                    }
                    msg.AddList(2);                                                                                     //L2 RPTID 600 Set
                    {
                        msg.AddAscii(AppUtil.ToAscii("600", gDefine.DEF_RPTID_SIZE));                                       //A3  RPTID="600"
                        msg.AddList(nDV_NO);                                                                                //Lm  DV
                        if (nDV_NO != 0)
                        {
                            foreach (KeyValuePair<string, string> list in dvNameValueList)
                            {

                                msg.AddList(2);                                                                                 //L2  DV Set
                                {
                                    msg.AddAscii(AppUtil.ToAscii(list.Key, 40));                        //A20 Data Name 
                                    msg.AddAscii(AppUtil.ToAscii(list.Value, 40));                  //A40 Data Value                                    
                                }
                                aDV_NAME.Add(list.Key);
                                aDV.Add(list.Value);
                            }
                        }
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

            this.SecsDriver.Send(msg);
            #endregion

            #region MATERIAL TRACK OUT WRITE LOG DATA
            //if (nMAT_NO != 0)
            //{
            //    for (int i = 0; i < nMAT_NO; i++)
            //    {
            //        MaterialLogData logMsg = new MaterialLogData();

            //        logMsg.sCEID = sCEID;
            //        logMsg.sCELL_ID = sCELLID;
            //        logMsg.sMT_BATCH_ID = aEQ_MAT_BAT_ID[i];
            //        logMsg.sMT_BATCH_NAME = aEQ_MAT_BAT_NAME[i];
            //        logMsg.sMT_ID = aEQ_MAT_ID[i];
            //        logMsg.sMT_PORT = aEQ_MAT_PORT_ID[i];
            //        logMsg.sMT_ST = aEQ_MAT_ST[i];
            //        logMsg.sMT_STATE = aEQ_MAT_STATE[i];
            //        logMsg.sMT_TOTAL = aEQ_MAT_TOTAL_QTY[i];
            //        logMsg.sMT_TYPE = aEQ_MAT_TYPE[i];
            //        logMsg.sUSE = aEQ_MAT_USE_QTY[i];
            //        logMsg.sPRODUCT = aEQ_MAT_PRODUCT_QTY[i];
            //        logMsg.sASSEMBLE = aEQ_MAT_ASSEM_QTY[i];
            //        logMsg.sNG = aEQ_MAT_NG_QTY[i];
            //        logMsg.sP_SUPPLY = aEQ_MAT_SUPPLY_REQUEST_QTY[i];
            //        logMsg.sP_USE = aEQ_MAT_PROCE_USE_QTY[i];
            //        logMsg.sP_NG = aEQ_MAT_PROC_NG_QTY[i];
            //        logMsg.sP_ASSEMBLE = aEQ_MAT_PROCE_ASSEM_QTY[i];
            //        logMsg.sREMAIN = aEQ_MAT_REMAINQTY_QTY[i];
            //        logMsg.sMT_NO = (i + 1).ToString();

            //        SetMaterialLog(logMsg);
            //    }
            //}
            #endregion

            #region CELL PROCESS END WRITE LOG DATA

            //CTrackOUTLogData logData = new CTrackOUTLogData();

            //logData.sEQP_ID = sEQPID;
            //logData.sCRST = sCRST;
            //logData.sCELLID = sCELLID;
            //logData.sAVAILABILITY = sAVAILABILITYSTATE;
            //logData.sINTERLOCK = sINTERLOCKSTATE;
            //logData.sMOVESTATE = sMOVESTATE;
            //logData.sRUNSTATE = sRUNSTATE;
            //logData.sPPID = sPPID;
            //logData.sJUDGE_CODE = sJUDGE; //0525
            //logData.sPRODUCT_ID = sPRODUCTID;
            //logData.sSTEP_ID = sSTEPID;
            //logData.sPROCESS_JOB = sPROCESS_JOB;
            //logData.sPROCESSED_QTY = sPROCESSD_QTY;
            //logData.sREADER_ID = sREADER_ID;
            //logData.sREAD_RESULT_CODE = sREADER_RESULTCODE;
            //logData.sDESCRIPTION = sDESCRIPTION2;
            //this.SetTrackOutLog(logData);

            #endregion

            #region DV DATA WRITE LOG DATA

            //DVLog logDVmsg = new DVLog();

            //logDVmsg.aDVNameHeader = aDV_NAME;
            //logDVmsg.aDVValue = aDV;
            //SetDVLog(logDVmsg);

            #endregion
        }
    }
}
