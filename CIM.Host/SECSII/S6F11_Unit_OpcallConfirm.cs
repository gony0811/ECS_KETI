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
    public class S6F11_Unit_OpcallConfirm : SFMessage
    {
        /*S6F11(TRS OPCALL Confirm), CEID:511*/
        public S6F11_Unit_OpcallConfirm(SECSDriver driver) 
            : base(driver)
        {
            
        }
        public override void DoWork(string driverName, object obj)
        {
            #region DEFINE
            int stream = 6, function = 11;
            bool bResult;

            string sDataID = "0";
            string sCRST = ((int)DataManager.Instance.GET_INT_DATA("vSys_HostData_Mode", out bResult)).ToString();
            string sEQPID = CommonData.Instance.EQP_SETTINGS.EQPID;
            string sUNITID = string.Empty; // 어디에서 가져올지 정해지면 코딩.
            string sCEID = "511";

            int nUnit_List = DataManager.Instance.GET_INT_DATA("vSys_UnitInfos_Count", out bResult); // m_lnkHostData.UnitNo;
            string[] aUNITID = new string[nUnit_List];

            string[] sAVAILABILITYSTATE = new string[nUnit_List];
            string[] sINTERLOCKSTATE = new string[nUnit_List];
            string[] sMOVESTATE = new string[nUnit_List];
            string[] sRUNSTATE = new string[nUnit_List];
            string[] sFRONTSTATE = new string[nUnit_List];
            string[] sREARSTATE = new string[nUnit_List];
            string[] sPP_SPLSTATE = new string[nUnit_List];
            string[] sREASONCODE = new string[nUnit_List];
            string[] sDESCRIPTION = new string[nUnit_List];

            string[] sCELLID = new string[nUnit_List];
            string sPPID = RMSManager.Instance.GetCurrentPPID();
            string[] sPRODUCTID = new string[nUnit_List];
            string[] sSTEPID = new string[nUnit_List];

            if (nUnit_List != 0)
            {
                sAVAILABILITYSTATE[0] = DataManager.Instance.GET_STRING_DATA("iPLC1_EtoC_EQPAvailability", out bResult);
                sINTERLOCKSTATE[0] = DataManager.Instance.GET_STRING_DATA("iPLC1_EtoC_EQPInterlock", out bResult);
                sMOVESTATE[0] = DataManager.Instance.GET_STRING_DATA("iPLC1_EtoC_EQPMove", out bResult);
                sRUNSTATE[0] = DataManager.Instance.GET_STRING_DATA("iPLC1_EtoC_EQPRun", out bResult);
                sFRONTSTATE[0] = DataManager.Instance.GET_STRING_DATA("iPLC1_EtoC_EQPFront", out bResult);
                sREARSTATE[0] = DataManager.Instance.GET_STRING_DATA("iPLC1_EtoC_EQPRear", out bResult);
                sPP_SPLSTATE[0] = DataManager.Instance.GET_STRING_DATA("iPLC1_EtoC_EQPState_PP", out bResult);
                sREASONCODE[0] = DataManager.Instance.GET_STRING_DATA("iPLC1_EtoC_AvailabilityReasonCode", out bResult);
                sDESCRIPTION[0] = DataManager.Instance.GET_STRING_DATA("iPLC1_EtoC_AvailabilityDescription", out bResult);

                sCELLID[0] = DataManager.Instance.GET_STRING_DATA("iPLC1_EtoC_InterlockCellIDConfirm", out bResult);
                sPRODUCTID[0] = DataManager.Instance.GET_STRING_DATA("iPLC1_EtoC_InterlockProductIDConfirm", out bResult);
                sSTEPID[0] = DataManager.Instance.GET_STRING_DATA("iPLC1_EtoC_InterlockStepIDConfirm", out bResult);

                if (nUnit_List >= 2)
                {
                    sAVAILABILITYSTATE[1] = DataManager.Instance.GET_STRING_DATA("iPLC2_EtoC_EQPAvailability", out bResult);
                    sINTERLOCKSTATE[1] = DataManager.Instance.GET_STRING_DATA("iPLC2_EtoC_EQPInterlock", out bResult);
                    sMOVESTATE[1] = DataManager.Instance.GET_STRING_DATA("iPLC2_EtoC_EQPMove", out bResult);
                    sRUNSTATE[1] = DataManager.Instance.GET_STRING_DATA("iPLC2_EtoC_EQPRun", out bResult);
                    sFRONTSTATE[1] = DataManager.Instance.GET_STRING_DATA("iPLC2_EtoC_EQPFront", out bResult);
                    sREARSTATE[1] = DataManager.Instance.GET_STRING_DATA("iPLC2_EtoC_EQPRear", out bResult);
                    sPP_SPLSTATE[1] = DataManager.Instance.GET_STRING_DATA("iPLC2_EtoC_EQPState_PP", out bResult);
                    sREASONCODE[1] = DataManager.Instance.GET_STRING_DATA("iPLC2_EtoC_AvailabilityReasonCode", out bResult);
                    sDESCRIPTION[1] = DataManager.Instance.GET_STRING_DATA("iPLC2_EtoC_AvailabilityDescription", out bResult);

                    sCELLID[1] = DataManager.Instance.GET_STRING_DATA("iPLC2_EtoC_InterlockCellIDConfirm", out bResult);
                    sPRODUCTID[1] = DataManager.Instance.GET_STRING_DATA("iPLC2_EtoC_InterlockProductIDConfirm", out bResult);
                    sSTEPID[1] = DataManager.Instance.GET_STRING_DATA("iPLC2_EtoC_InterlockStepIDConfirm", out bResult);
                }
                if (nUnit_List >= 3)
                {
                    sAVAILABILITYSTATE[2] = DataManager.Instance.GET_STRING_DATA("iPLC3_EtoC_EQPAvailability", out bResult);
                    sINTERLOCKSTATE[2] = DataManager.Instance.GET_STRING_DATA("iPLC3_EtoC_EQPInterlock", out bResult);
                    sMOVESTATE[2] = DataManager.Instance.GET_STRING_DATA("iPLC3_EtoC_EQPMove", out bResult);
                    sRUNSTATE[2] = DataManager.Instance.GET_STRING_DATA("iPLC3_EtoC_EQPRun", out bResult);
                    sFRONTSTATE[2] = DataManager.Instance.GET_STRING_DATA("iPLC3_EtoC_EQPFront", out bResult);
                    sREARSTATE[2] = DataManager.Instance.GET_STRING_DATA("iPLC3_EtoC_EQPRear", out bResult);
                    sPP_SPLSTATE[2] = DataManager.Instance.GET_STRING_DATA("iPLC3_EtoC_EQPState_PP", out bResult);
                    sREASONCODE[2] = DataManager.Instance.GET_STRING_DATA("iPLC3_EtoC_AvailabilityReasonCode", out bResult);
                    sDESCRIPTION[2] = DataManager.Instance.GET_STRING_DATA("iPLC3_EtoC_AvailabilityDescription", out bResult);

                    sCELLID[2] = DataManager.Instance.GET_STRING_DATA("iPLC3_EtoC_InterlockCellIDConfirm", out bResult);
                    sPRODUCTID[2] = DataManager.Instance.GET_STRING_DATA("iPLC3_EtoC_InterlockProductIDConfirm", out bResult);
                    sSTEPID[2] = DataManager.Instance.GET_STRING_DATA("iPLC3_EtoC_InterlockStepIDConfirm", out bResult);
                }
                if (nUnit_List >= 4)
                {
                    sAVAILABILITYSTATE[3] = DataManager.Instance.GET_STRING_DATA("iPLC4_EtoC_EQPAvailability", out bResult);
                    sINTERLOCKSTATE[3] = DataManager.Instance.GET_STRING_DATA("iPLC4_EtoC_EQPInterlock", out bResult);
                    sMOVESTATE[3] = DataManager.Instance.GET_STRING_DATA("iPLC4_EtoC_EQPMove", out bResult);
                    sRUNSTATE[3] = DataManager.Instance.GET_STRING_DATA("iPLC4_EtoC_EQPRun", out bResult);
                    sFRONTSTATE[3] = DataManager.Instance.GET_STRING_DATA("iPLC4_EtoC_EQPFront", out bResult);
                    sREARSTATE[3] = DataManager.Instance.GET_STRING_DATA("iPLC4_EtoC_EQPRear", out bResult);
                    sPP_SPLSTATE[3] = DataManager.Instance.GET_STRING_DATA("iPLC4_EtoC_EQPState_PP", out bResult);
                    sREASONCODE[3] = DataManager.Instance.GET_STRING_DATA("iPLC4_EtoC_AvailabilityReasonCode", out bResult);
                    sDESCRIPTION[3] = DataManager.Instance.GET_STRING_DATA("iPLC4_EtoC_AvailabilityDescription", out bResult);

                    sCELLID[3] = DataManager.Instance.GET_STRING_DATA("iPLC4_EtoC_InterlockCellIDConfirm", out bResult);
                    sPRODUCTID[3] = DataManager.Instance.GET_STRING_DATA("iPLC4_EtoC_InterlockProductIDConfirm", out bResult);
                    sSTEPID[3] = DataManager.Instance.GET_STRING_DATA("iPLC4_EtoC_InterlockStepIDConfirm", out bResult);
                }
                if (nUnit_List >= 5)
                {
                    sAVAILABILITYSTATE[4] = DataManager.Instance.GET_STRING_DATA("iPLC5_EtoC_EQPAvailability", out bResult);
                    sINTERLOCKSTATE[4] = DataManager.Instance.GET_STRING_DATA("iPLC5_EtoC_EQPInterlock", out bResult);
                    sMOVESTATE[4] = DataManager.Instance.GET_STRING_DATA("iPLC5_EtoC_EQPMove", out bResult);
                    sRUNSTATE[4] = DataManager.Instance.GET_STRING_DATA("iPLC5_EtoC_EQPRun", out bResult);
                    sFRONTSTATE[4] = DataManager.Instance.GET_STRING_DATA("iPLC5_EtoC_EQPFront", out bResult);
                    sREARSTATE[4] = DataManager.Instance.GET_STRING_DATA("iPLC5_EtoC_EQPRear", out bResult);
                    sPP_SPLSTATE[4] = DataManager.Instance.GET_STRING_DATA("iPLC5_EtoC_EQPState_PP", out bResult);
                    sREASONCODE[4] = DataManager.Instance.GET_STRING_DATA("iPLC5_EtoC_AvailabilityReasonCode", out bResult);
                    sDESCRIPTION[4] = DataManager.Instance.GET_STRING_DATA("iPLC5_EtoC_AvailabilityDescription", out bResult);

                    sCELLID[4] = DataManager.Instance.GET_STRING_DATA("iPLC5_EtoC_InterlockCellIDConfirm", out bResult);
                    sPRODUCTID[4] = DataManager.Instance.GET_STRING_DATA("iPLC5_EtoC_InterlockProductIDConfirm", out bResult);
                    sSTEPID[4] = DataManager.Instance.GET_STRING_DATA("iPLC5_EtoC_InterlockStepIDConfirm", out bResult);
                }
                if (nUnit_List >= 6)
                {
                    sAVAILABILITYSTATE[5] = DataManager.Instance.GET_STRING_DATA("iPLC6_EtoC_EQPAvailability", out bResult);
                    sINTERLOCKSTATE[5] = DataManager.Instance.GET_STRING_DATA("iPLC6_EtoC_EQPInterlock", out bResult);
                    sMOVESTATE[5] = DataManager.Instance.GET_STRING_DATA("iPLC6_EtoC_EQPMove", out bResult);
                    sRUNSTATE[5] = DataManager.Instance.GET_STRING_DATA("iPLC6_EtoC_EQPRun", out bResult);
                    sFRONTSTATE[5] = DataManager.Instance.GET_STRING_DATA("iPLC6_EtoC_EQPFront", out bResult);
                    sREARSTATE[5] = DataManager.Instance.GET_STRING_DATA("iPLC6_EtoC_EQPRear", out bResult);
                    sPP_SPLSTATE[5] = DataManager.Instance.GET_STRING_DATA("iPLC6_EtoC_EQPState_PP", out bResult);
                    sREASONCODE[5] = DataManager.Instance.GET_STRING_DATA("iPLC6_EtoC_AvailabilityReasonCode", out bResult);
                    sDESCRIPTION[5] = DataManager.Instance.GET_STRING_DATA("iPLC6_EtoC_AvailabilityDescription", out bResult);

                    sCELLID[5] = DataManager.Instance.GET_STRING_DATA("iPLC6_EtoC_InterlockCellIDConfirm", out bResult);
                    sPRODUCTID[5] = DataManager.Instance.GET_STRING_DATA("iPLC6_EtoC_InterlockProductIDConfirm", out bResult);
                    sSTEPID[5] = DataManager.Instance.GET_STRING_DATA("iPLC6_EtoC_InterlockStepIDConfirm", out bResult);
                }
                if (nUnit_List >= 7)
                {
                    sAVAILABILITYSTATE[6] = DataManager.Instance.GET_STRING_DATA("iPLC7_EtoC_EQPAvailability", out bResult);
                    sINTERLOCKSTATE[6] = DataManager.Instance.GET_STRING_DATA("iPLC7_EtoC_EQPInterlock", out bResult);
                    sMOVESTATE[6] = DataManager.Instance.GET_STRING_DATA("iPLC7_EtoC_EQPMove", out bResult);
                    sRUNSTATE[6] = DataManager.Instance.GET_STRING_DATA("iPLC7_EtoC_EQPRun", out bResult);
                    sFRONTSTATE[6] = DataManager.Instance.GET_STRING_DATA("iPLC7_EtoC_EQPFront", out bResult);
                    sREARSTATE[6] = DataManager.Instance.GET_STRING_DATA("iPLC7_EtoC_EQPRear", out bResult);
                    sPP_SPLSTATE[6] = DataManager.Instance.GET_STRING_DATA("iPLC7_EtoC_EQPState_PP", out bResult);
                    sREASONCODE[6] = DataManager.Instance.GET_STRING_DATA("iPLC7_EtoC_AvailabilityReasonCode", out bResult);
                    sDESCRIPTION[6] = DataManager.Instance.GET_STRING_DATA("iPLC7_EtoC_AvailabilityDescription", out bResult);

                    sCELLID[6] = DataManager.Instance.GET_STRING_DATA("iPLC7_EtoC_InterlockCellIDConfirm", out bResult);
                    sPRODUCTID[6] = DataManager.Instance.GET_STRING_DATA("iPLC7_EtoC_InterlockProductIDConfirm", out bResult);
                    sSTEPID[6] = DataManager.Instance.GET_STRING_DATA("iPLC7_EtoC_InterlockStepIDConfirm", out bResult);
                }
                if (nUnit_List >= 8)
                {
                    sAVAILABILITYSTATE[7] = DataManager.Instance.GET_STRING_DATA("iPLC8_EtoC_EQPAvailability", out bResult);
                    sINTERLOCKSTATE[7] = DataManager.Instance.GET_STRING_DATA("iPLC8_EtoC_EQPInterlock", out bResult);
                    sMOVESTATE[7] = DataManager.Instance.GET_STRING_DATA("iPLC8_EtoC_EQPMove", out bResult);
                    sRUNSTATE[7] = DataManager.Instance.GET_STRING_DATA("iPLC8_EtoC_EQPRun", out bResult);
                    sFRONTSTATE[7] = DataManager.Instance.GET_STRING_DATA("iPLC8_EtoC_EQPFront", out bResult);
                    sREARSTATE[7] = DataManager.Instance.GET_STRING_DATA("iPLC8_EtoC_EQPRear", out bResult);
                    sPP_SPLSTATE[7] = DataManager.Instance.GET_STRING_DATA("iPLC8_EtoC_EQPState_PP", out bResult);
                    sREASONCODE[7] = DataManager.Instance.GET_STRING_DATA("iPLC8_EtoC_AvailabilityReasonCode", out bResult);
                    sDESCRIPTION[7] = DataManager.Instance.GET_STRING_DATA("iPLC8_EtoC_AvailabilityDescription", out bResult);

                    sCELLID[7] = DataManager.Instance.GET_STRING_DATA("iPLC8_EtoC_InterlockCellIDConfirm", out bResult);
                    sPRODUCTID[7] = DataManager.Instance.GET_STRING_DATA("iPLC8_EtoC_InterlockProductIDConfirm", out bResult);
                    sSTEPID[7] = DataManager.Instance.GET_STRING_DATA("iPLC8_EtoC_InterlockStepIDConfirm", out bResult);
                }
            }

            short Unit1_nOPCALL_NO = 1;
            short Unit2_nOPCALL_NO = 1;
            short Unit3_nOPCALL_NO = 1;
            short Unit4_nOPCALL_NO = 1;
            short Unit5_nOPCALL_NO = 1;
            short Unit6_nOPCALL_NO = 1;
            short Unit7_nOPCALL_NO = 1;
            short Unit8_nOPCALL_NO = 1;

            DataManager.Instance.SET_INT_DATA("vSys_Send_Count", 0);
            #endregion

            if (aUNITID[0] == sUNITID)
            {
                #region S6F11(OPCALL Confirm), CEID:501

                SecsMessage msg = new SecsMessage(stream, function, true, Convert.ToInt32(SecsDriver.DeviceID));

                msg.AddList(3);                                                                                     //L3  Opcall Event Info
                {
                    msg.AddAscii(AppUtil.ToAscii(sDataID, gDefine.DEF_DATAID_SIZE));                                    //A4  Data ID
                    msg.AddAscii(AppUtil.ToAscii(sCEID, gDefine.DEF_CEID_SIZE));                                        //A3  Collection Event ID
                    msg.AddList(4);                                                                                     //L4  RPTID Set
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
                            msg.AddAscii(AppUtil.ToAscii("102", gDefine.DEF_RPTID_SIZE));                                       //A3  RPTID="101"
                            msg.AddList(2);                                                                                     //L2  EQP State Set
                            {
                                msg.AddAscii(AppUtil.ToAscii(sUNITID, gDefine.DEF_UNITID_SIZE));                                      //A40 HOST REQ UNITID 
                                msg.AddList(9);                                                                                     //L9  EQP State Set
                                {
                                    msg.AddAscii(sAVAILABILITYSTATE[0]);                                                                   //A1  EQ Avilability State Info
                                    msg.AddAscii(sINTERLOCKSTATE[0]);                                                                      //A1  Interlock Avilability State Info
                                    msg.AddAscii(sMOVESTATE[0]);                                                                           //A1  EQ Move State Info
                                    msg.AddAscii(sRUNSTATE[0]);                                                                            //A1  Cell existence/nonexistence Check
                                    msg.AddAscii(sFRONTSTATE[0]);                                                                          //A1  Upper EQ Processing State
                                    msg.AddAscii(sREARSTATE[0]);                                                                           //A1  Lower EQ Processing State
                                    msg.AddAscii(sPP_SPLSTATE[0]);                                                                         //A1  Sample Run-Normal Run State
                                    msg.AddAscii(AppUtil.ToAscii(sREASONCODE[0], gDefine.DEF_REASONCODE_SIZE));                            //A20 Code Info
                                    msg.AddAscii(AppUtil.ToAscii(sDESCRIPTION[0], gDefine.DEF_DESCRIPTION_SIZE));                          //A40 EQ Description
                                }
                            }
                        }
                        msg.AddList(2);                                                                                     //L2  RPTID 300 Set
                        {
                            msg.AddAscii(AppUtil.ToAscii("300", gDefine.DEF_RPTID_SIZE));                                       //A3  RPTID="300"
                            msg.AddList(4);                                                                                     //L4  Cell Info Set
                            {
                                msg.AddAscii(AppUtil.ToAscii(sCELLID[0], 40));                                                         //A40 Cell Unique ID
                                msg.AddAscii(AppUtil.ToAscii(sPPID, gDefine.DEF_PPID_SIZE));                                        //A40 LOT 지PPID
                                msg.AddAscii(AppUtil.ToAscii(sPRODUCTID[0], gDefine.DEF_PRODUCTID_SIZE));                              //A40 CELL Product Info
                                msg.AddAscii(AppUtil.ToAscii(sSTEPID[0], 40));                                                         //A40 CELL STEP ID Info
                            }
                        }
                        msg.AddList(2);                                                                                     //L2  RPTID 700 Set
                        {
                            msg.AddAscii(AppUtil.ToAscii("700", gDefine.DEF_RPTID_SIZE));                                       //A3  RPTID="700"
                            msg.AddList(Unit1_nOPCALL_NO);                                                                            //Ln  OPCALL
                            {
                                string[] Unit1_aOPCALL_ID = new string[Unit1_nOPCALL_NO];
                                string[] Unit1_aMESSAGE = new string[Unit1_nOPCALL_NO];

                                for (int i = 0; i < Unit1_nOPCALL_NO; i++)
                                {
                                    Unit1_aOPCALL_ID[i] = DataManager.Instance.GET_STRING_DATA("iPLC1_EtoC_OPCallIDComfirm", out bResult);
                                    Unit1_aMESSAGE[i] = DataManager.Instance.GET_STRING_DATA("iPLC1_EtoC_OPCallMessageConfirm", out bResult);


                                    msg.AddList(2);                                                                                 //L2  OPCALL Set
                                    {
                                        msg.AddAscii(AppUtil.ToAscii(Unit1_aOPCALL_ID[i], 20));                                               //A20 OPCALL Name  
                                        msg.AddAscii(AppUtil.ToAscii(Unit1_aMESSAGE[i], 120));                                                //A120 OPCALL Message
                                    }
                                }
                            }
                        }
                    }
                }

                this.SecsDriver.Send(msg);
                #endregion
            }

            else if (aUNITID[1] == sUNITID)
            {
                #region S6F11(OPCALL Confirm), CEID:501

                SecsMessage msg = new SecsMessage(stream, function, true, Convert.ToInt32(SecsDriver.DeviceID));

                msg.AddList(3);                                                                                     //L3  Opcall Event Info
                {
                    msg.AddAscii(AppUtil.ToAscii(sDataID, gDefine.DEF_DATAID_SIZE));                                    //A4  Data ID
                    msg.AddAscii(AppUtil.ToAscii(sCEID, gDefine.DEF_CEID_SIZE));                                        //A3  Collection Event ID
                    msg.AddList(4);                                                                                     //L4  RPTID Set
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
                            msg.AddAscii(AppUtil.ToAscii("102", gDefine.DEF_RPTID_SIZE));                                       //A3  RPTID="101"
                            msg.AddList(2);                                                                                     //L2  EQP State Set
                            {
                                msg.AddAscii(AppUtil.ToAscii(sUNITID, gDefine.DEF_UNITID_SIZE));                                      //A40 HOST REQ UNITID 
                                msg.AddList(9);                                                                                     //L9  EQP State Set
                                {
                                    msg.AddAscii(sAVAILABILITYSTATE[0]);                                                                   //A1  EQ Avilability State Info
                                    msg.AddAscii(sINTERLOCKSTATE[0]);                                                                      //A1  Interlock Avilability State Info
                                    msg.AddAscii(sMOVESTATE[0]);                                                                           //A1  EQ Move State Info
                                    msg.AddAscii(sRUNSTATE[0]);                                                                            //A1  Cell existence/nonexistence Check
                                    msg.AddAscii(sFRONTSTATE[0]);                                                                          //A1  Upper EQ Processing State
                                    msg.AddAscii(sREARSTATE[0]);                                                                           //A1  Lower EQ Processing State
                                    msg.AddAscii(sPP_SPLSTATE[0]);                                                                         //A1  Sample Run-Normal Run State
                                    msg.AddAscii(AppUtil.ToAscii(sREASONCODE[0], gDefine.DEF_REASONCODE_SIZE));                            //A20 Code Info
                                    msg.AddAscii(AppUtil.ToAscii(sDESCRIPTION[0], gDefine.DEF_DESCRIPTION_SIZE));                          //A40 EQ Description
                                }
                            }
                        }
                        msg.AddList(2);                                                                                     //L2  RPTID 300 Set
                        {
                            msg.AddAscii(AppUtil.ToAscii("300", gDefine.DEF_RPTID_SIZE));                                       //A3  RPTID="300"
                            msg.AddList(4);                                                                                     //L4  Cell Info Set
                            {
                                msg.AddAscii(AppUtil.ToAscii(sCELLID[1], 40));                                                         //A40 Cell Unique ID
                                msg.AddAscii(AppUtil.ToAscii(sPPID, gDefine.DEF_PPID_SIZE));                                        //A40 LOT 지PPID
                                msg.AddAscii(AppUtil.ToAscii(sPRODUCTID[1], gDefine.DEF_PRODUCTID_SIZE));                              //A40 CELL Product Info
                                msg.AddAscii(AppUtil.ToAscii(sSTEPID[1], 40));                                                         //A40 CELL STEP ID Info
                            }
                        }
                        msg.AddList(2);                                                                                     //L2  RPTID 700 Set
                        {
                            msg.AddAscii(AppUtil.ToAscii("700", gDefine.DEF_RPTID_SIZE));                                       //A3  RPTID="700"
                            msg.AddList(Unit2_nOPCALL_NO);                                                                            //Ln  OPCALL
                            {
                                string[] Unit2_aOPCALL_ID = new string[Unit2_nOPCALL_NO];
                                string[] Unit2_aMESSAGE = new string[Unit2_nOPCALL_NO];

                                for (int i = 0; i < Unit2_nOPCALL_NO; i++)
                                {
                                    Unit2_aOPCALL_ID[i] = DataManager.Instance.GET_STRING_DATA("iPLC2_EtoC_OPCallIDComfirm", out bResult);
                                    Unit2_aMESSAGE[i] = DataManager.Instance.GET_STRING_DATA("iPLC2_EtoC_OPCallMessageConfirm", out bResult);

                                    msg.AddList(2);                                                                                 //L2  OPCALL Set
                                    {
                                        msg.AddAscii(AppUtil.ToAscii(Unit2_aOPCALL_ID[i], 20));                                               //A20 OPCALL Name  
                                        msg.AddAscii(AppUtil.ToAscii(Unit2_aMESSAGE[i], 120));                                                //A120 OPCALL Message
                                    }
                                }
                            }
                        }
                    }
                }

                this.SecsDriver.Send(msg);
                #endregion
            }

            else if (aUNITID[2] == sUNITID)
            {
                #region S6F11(OPCALL Confirm), CEID:501

                SecsMessage msg = new SecsMessage(stream, function, true, Convert.ToInt32(SecsDriver.DeviceID));

                msg.AddList(3);                                                                                     //L3  Opcall Event Info
                {
                    msg.AddAscii(AppUtil.ToAscii(sDataID, gDefine.DEF_DATAID_SIZE));                                    //A4  Data ID
                    msg.AddAscii(AppUtil.ToAscii(sCEID, gDefine.DEF_CEID_SIZE));                                        //A3  Collection Event ID
                    msg.AddList(4);                                                                                     //L4  RPTID Set
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
                            msg.AddAscii(AppUtil.ToAscii("102", gDefine.DEF_RPTID_SIZE));                                       //A3  RPTID="101"
                            msg.AddList(2);                                                                                     //L2  EQP State Set
                            {
                                msg.AddAscii(AppUtil.ToAscii(sUNITID, gDefine.DEF_UNITID_SIZE));                                      //A40 HOST REQ UNITID 
                                msg.AddList(9);                                                                                     //L9  EQP State Set
                                {
                                    msg.AddAscii(sAVAILABILITYSTATE[0]);                                                                   //A1  EQ Avilability State Info
                                    msg.AddAscii(sINTERLOCKSTATE[0]);                                                                      //A1  Interlock Avilability State Info
                                    msg.AddAscii(sMOVESTATE[0]);                                                                           //A1  EQ Move State Info
                                    msg.AddAscii(sRUNSTATE[0]);                                                                            //A1  Cell existence/nonexistence Check
                                    msg.AddAscii(sFRONTSTATE[0]);                                                                          //A1  Upper EQ Processing State
                                    msg.AddAscii(sREARSTATE[0]);                                                                           //A1  Lower EQ Processing State
                                    msg.AddAscii(sPP_SPLSTATE[0]);                                                                         //A1  Sample Run-Normal Run State
                                    msg.AddAscii(AppUtil.ToAscii(sREASONCODE[0], gDefine.DEF_REASONCODE_SIZE));                            //A20 Code Info
                                    msg.AddAscii(AppUtil.ToAscii(sDESCRIPTION[0], gDefine.DEF_DESCRIPTION_SIZE));                          //A40 EQ Description
                                }
                            }
                        }
                        msg.AddList(2);                                                                                     //L2  RPTID 300 Set
                        {
                            msg.AddAscii(AppUtil.ToAscii("300", gDefine.DEF_RPTID_SIZE));                                       //A3  RPTID="300"
                            msg.AddList(4);                                                                                     //L4  Cell Info Set
                            {
                                msg.AddAscii(AppUtil.ToAscii(sCELLID[2], 40));                                                         //A40 Cell Unique ID
                                msg.AddAscii(AppUtil.ToAscii(sPPID, gDefine.DEF_PPID_SIZE));                                        //A40 LOT 지PPID
                                msg.AddAscii(AppUtil.ToAscii(sPRODUCTID[2], gDefine.DEF_PRODUCTID_SIZE));                              //A40 CELL Product Info
                                msg.AddAscii(AppUtil.ToAscii(sSTEPID[2], 40));                                                         //A40 CELL STEP ID Info
                            }
                        }
                        msg.AddList(2);                                                                                     //L2  RPTID 700 Set
                        {
                            msg.AddAscii(AppUtil.ToAscii("700", gDefine.DEF_RPTID_SIZE));                                       //A3  RPTID="700"
                            msg.AddList(Unit3_nOPCALL_NO);                                                                            //Ln  OPCALL
                            {
                                string[] Unit3_aOPCALL_ID = new string[Unit3_nOPCALL_NO];
                                string[] Unit3_aMESSAGE = new string[Unit3_nOPCALL_NO];

                                for (int i = 0; i < Unit3_nOPCALL_NO; i++)
                                {
                                    Unit3_aOPCALL_ID[i] = DataManager.Instance.GET_STRING_DATA("iPLC3_EtoC_OPCallIDComfirm", out bResult);
                                    Unit3_aMESSAGE[i] = DataManager.Instance.GET_STRING_DATA("iPLC3_EtoC_OPCallMessageConfirm", out bResult);

                                    msg.AddList(2);                                                                                 //L2  OPCALL Set
                                    {
                                        msg.AddAscii(AppUtil.ToAscii(Unit3_aOPCALL_ID[i], 20));                                               //A20 OPCALL Name  
                                        msg.AddAscii(AppUtil.ToAscii(Unit3_aMESSAGE[i], 120));                                                //A120 OPCALL Message
                                    }
                                }
                            }
                        }
                    }
                }

                this.SecsDriver.Send(msg);
                #endregion
            }

            else if (aUNITID[3] == sUNITID)
            {
                #region S6F11(OPCALL Confirm), CEID:501

                SecsMessage msg = new SecsMessage(stream, function, true, Convert.ToInt32(SecsDriver.DeviceID));

                msg.AddList(3);                                                                                     //L3  Opcall Event Info
                {
                    msg.AddAscii(AppUtil.ToAscii(sDataID, gDefine.DEF_DATAID_SIZE));                                    //A4  Data ID
                    msg.AddAscii(AppUtil.ToAscii(sCEID, gDefine.DEF_CEID_SIZE));                                        //A3  Collection Event ID
                    msg.AddList(4);                                                                                     //L4  RPTID Set
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
                            msg.AddAscii(AppUtil.ToAscii("102", gDefine.DEF_RPTID_SIZE));                                       //A3  RPTID="101"
                            msg.AddList(2);                                                                                     //L2  EQP State Set
                            {
                                msg.AddAscii(AppUtil.ToAscii(sUNITID, gDefine.DEF_UNITID_SIZE));                                      //A40 HOST REQ UNITID 
                                msg.AddList(9);                                                                                     //L9  EQP State Set
                                {
                                    msg.AddAscii(sAVAILABILITYSTATE[0]);                                                                   //A1  EQ Avilability State Info
                                    msg.AddAscii(sINTERLOCKSTATE[0]);                                                                      //A1  Interlock Avilability State Info
                                    msg.AddAscii(sMOVESTATE[0]);                                                                           //A1  EQ Move State Info
                                    msg.AddAscii(sRUNSTATE[0]);                                                                            //A1  Cell existence/nonexistence Check
                                    msg.AddAscii(sFRONTSTATE[0]);                                                                          //A1  Upper EQ Processing State
                                    msg.AddAscii(sREARSTATE[0]);                                                                           //A1  Lower EQ Processing State
                                    msg.AddAscii(sPP_SPLSTATE[0]);                                                                         //A1  Sample Run-Normal Run State
                                    msg.AddAscii(AppUtil.ToAscii(sREASONCODE[0], gDefine.DEF_REASONCODE_SIZE));                            //A20 Code Info
                                    msg.AddAscii(AppUtil.ToAscii(sDESCRIPTION[0], gDefine.DEF_DESCRIPTION_SIZE));                          //A40 EQ Description
                                }
                            }
                        }
                        msg.AddList(2);                                                                                     //L2  RPTID 300 Set
                        {
                            msg.AddAscii(AppUtil.ToAscii("300", gDefine.DEF_RPTID_SIZE));                                       //A3  RPTID="300"
                            msg.AddList(4);                                                                                     //L4  Cell Info Set
                            {
                                msg.AddAscii(AppUtil.ToAscii(sCELLID[3], 40));                                                         //A40 Cell Unique ID
                                msg.AddAscii(AppUtil.ToAscii(sPPID, gDefine.DEF_PPID_SIZE));                                        //A40 LOT 지PPID
                                msg.AddAscii(AppUtil.ToAscii(sPRODUCTID[3], gDefine.DEF_PRODUCTID_SIZE));                              //A40 CELL Product Info
                                msg.AddAscii(AppUtil.ToAscii(sSTEPID[3], 40));                                                         //A40 CELL STEP ID Info
                            }
                        }
                        msg.AddList(2);                                                                                     //L2  RPTID 700 Set
                        {
                            msg.AddAscii(AppUtil.ToAscii("700", gDefine.DEF_RPTID_SIZE));                                       //A3  RPTID="700"
                            msg.AddList(Unit4_nOPCALL_NO);                                                                            //Ln  OPCALL
                            {
                                string[] Unit4_aOPCALL_ID = new string[Unit4_nOPCALL_NO];
                                string[] Unit4_aMESSAGE = new string[Unit4_nOPCALL_NO];

                                for (int i = 0; i < Unit4_nOPCALL_NO; i++)
                                {
                                    Unit4_aOPCALL_ID[i] = DataManager.Instance.GET_STRING_DATA("iPLC4_EtoC_OPCallIDComfirm", out bResult);
                                    Unit4_aMESSAGE[i] = DataManager.Instance.GET_STRING_DATA("iPLC4_EtoC_OPCallMessageConfirm", out bResult);

                                    msg.AddList(2);                                                                                 //L2  OPCALL Set
                                    {
                                        msg.AddAscii(AppUtil.ToAscii(Unit4_aOPCALL_ID[i], 20));                                               //A20 OPCALL Name  
                                        msg.AddAscii(AppUtil.ToAscii(Unit4_aMESSAGE[i], 120));                                                //A120 OPCALL Message
                                    }
                                }
                            }
                        }
                    }
                }

                this.SecsDriver.Send(msg);
                #endregion
            }

            else if (aUNITID[4] == sUNITID)
            {
                #region S6F11(OPCALL Confirm), CEID:501

                SecsMessage msg = new SecsMessage(stream, function, true, Convert.ToInt32(SecsDriver.DeviceID));

                msg.AddList(3);                                                                                     //L3  Opcall Event Info
                {
                    msg.AddAscii(AppUtil.ToAscii(sDataID, gDefine.DEF_DATAID_SIZE));                                    //A4  Data ID
                    msg.AddAscii(AppUtil.ToAscii(sCEID, gDefine.DEF_CEID_SIZE));                                        //A3  Collection Event ID
                    msg.AddList(4);                                                                                     //L4  RPTID Set
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
                            msg.AddAscii(AppUtil.ToAscii("102", gDefine.DEF_RPTID_SIZE));                                       //A3  RPTID="101"
                            msg.AddList(2);                                                                                     //L2  EQP State Set
                            {
                                msg.AddAscii(AppUtil.ToAscii(sUNITID, gDefine.DEF_UNITID_SIZE));                                      //A40 HOST REQ UNITID 
                                msg.AddList(9);                                                                                     //L9  EQP State Set
                                {
                                    msg.AddAscii(sAVAILABILITYSTATE[0]);                                                                   //A1  EQ Avilability State Info
                                    msg.AddAscii(sINTERLOCKSTATE[0]);                                                                      //A1  Interlock Avilability State Info
                                    msg.AddAscii(sMOVESTATE[0]);                                                                           //A1  EQ Move State Info
                                    msg.AddAscii(sRUNSTATE[0]);                                                                            //A1  Cell existence/nonexistence Check
                                    msg.AddAscii(sFRONTSTATE[0]);                                                                          //A1  Upper EQ Processing State
                                    msg.AddAscii(sREARSTATE[0]);                                                                           //A1  Lower EQ Processing State
                                    msg.AddAscii(sPP_SPLSTATE[0]);                                                                         //A1  Sample Run-Normal Run State
                                    msg.AddAscii(AppUtil.ToAscii(sREASONCODE[0], gDefine.DEF_REASONCODE_SIZE));                            //A20 Code Info
                                    msg.AddAscii(AppUtil.ToAscii(sDESCRIPTION[0], gDefine.DEF_DESCRIPTION_SIZE));                          //A40 EQ Description
                                }
                            }
                        }
                        msg.AddList(2);                                                                                     //L2  RPTID 300 Set
                        {
                            msg.AddAscii(AppUtil.ToAscii("300", gDefine.DEF_RPTID_SIZE));                                       //A3  RPTID="300"
                            msg.AddList(4);                                                                                     //L4  Cell Info Set
                            {
                                msg.AddAscii(AppUtil.ToAscii(sCELLID[4], 40));                                                         //A40 Cell Unique ID
                                msg.AddAscii(AppUtil.ToAscii(sPPID, gDefine.DEF_PPID_SIZE));                                        //A40 LOT 지PPID
                                msg.AddAscii(AppUtil.ToAscii(sPRODUCTID[4], gDefine.DEF_PRODUCTID_SIZE));                              //A40 CELL Product Info
                                msg.AddAscii(AppUtil.ToAscii(sSTEPID[4], 40));                                                         //A40 CELL STEP ID Info
                            }
                        }
                        msg.AddList(2);                                                                                     //L2  RPTID 700 Set
                        {
                            msg.AddAscii(AppUtil.ToAscii("700", gDefine.DEF_RPTID_SIZE));                                       //A3  RPTID="700"
                            msg.AddList(Unit5_nOPCALL_NO);                                                                            //Ln  OPCALL
                            {
                                string[] Unit5_aOPCALL_ID = new string[Unit5_nOPCALL_NO];
                                string[] Unit5_aMESSAGE = new string[Unit5_nOPCALL_NO];

                                for (int i = 0; i < Unit5_nOPCALL_NO; i++)
                                {
                                    Unit5_aOPCALL_ID[i] = DataManager.Instance.GET_STRING_DATA("iPLC5_EtoC_OPCallIDComfirm", out bResult);
                                    Unit5_aMESSAGE[i] = DataManager.Instance.GET_STRING_DATA("iPLC5_EtoC_OPCallMessageConfirm", out bResult);

                                    msg.AddList(2);                                                                                 //L2  OPCALL Set
                                    {
                                        msg.AddAscii(AppUtil.ToAscii(Unit5_aOPCALL_ID[i], 20));                                               //A20 OPCALL Name  
                                        msg.AddAscii(AppUtil.ToAscii(Unit5_aMESSAGE[i], 120));                                                //A120 OPCALL Message
                                    }
                                }
                            }
                        }
                    }
                }

                this.SecsDriver.Send(msg);
                #endregion
            }

            else if (aUNITID[5] == sUNITID)
            {
                #region S6F11(OPCALL Confirm), CEID:501

                SecsMessage msg = new SecsMessage(stream, function, true, Convert.ToInt32(SecsDriver.DeviceID));

                msg.AddList(3);                                                                                     //L3  Opcall Event Info
                {
                    msg.AddAscii(AppUtil.ToAscii(sDataID, gDefine.DEF_DATAID_SIZE));                                    //A4  Data ID
                    msg.AddAscii(AppUtil.ToAscii(sCEID, gDefine.DEF_CEID_SIZE));                                        //A3  Collection Event ID
                    msg.AddList(4);                                                                                     //L4  RPTID Set
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
                            msg.AddAscii(AppUtil.ToAscii("102", gDefine.DEF_RPTID_SIZE));                                       //A3  RPTID="101"
                            msg.AddList(2);                                                                                     //L2  EQP State Set
                            {
                                msg.AddAscii(AppUtil.ToAscii(sUNITID, gDefine.DEF_UNITID_SIZE));                                      //A40 HOST REQ UNITID 
                                msg.AddList(9);                                                                                     //L9  EQP State Set
                                {
                                    msg.AddAscii(sAVAILABILITYSTATE[0]);                                                                   //A1  EQ Avilability State Info
                                    msg.AddAscii(sINTERLOCKSTATE[0]);                                                                      //A1  Interlock Avilability State Info
                                    msg.AddAscii(sMOVESTATE[0]);                                                                           //A1  EQ Move State Info
                                    msg.AddAscii(sRUNSTATE[0]);                                                                            //A1  Cell existence/nonexistence Check
                                    msg.AddAscii(sFRONTSTATE[0]);                                                                          //A1  Upper EQ Processing State
                                    msg.AddAscii(sREARSTATE[0]);                                                                           //A1  Lower EQ Processing State
                                    msg.AddAscii(sPP_SPLSTATE[0]);                                                                         //A1  Sample Run-Normal Run State
                                    msg.AddAscii(AppUtil.ToAscii(sREASONCODE[0], gDefine.DEF_REASONCODE_SIZE));                            //A20 Code Info
                                    msg.AddAscii(AppUtil.ToAscii(sDESCRIPTION[0], gDefine.DEF_DESCRIPTION_SIZE));                          //A40 EQ Description
                                }
                            }
                        }
                        msg.AddList(2);                                                                                     //L2  RPTID 300 Set
                        {
                            msg.AddAscii(AppUtil.ToAscii("300", gDefine.DEF_RPTID_SIZE));                                       //A3  RPTID="300"
                            msg.AddList(4);                                                                                     //L4  Cell Info Set
                            {
                                msg.AddAscii(AppUtil.ToAscii(sCELLID[5], 40));                                                         //A40 Cell Unique ID
                                msg.AddAscii(AppUtil.ToAscii(sPPID, gDefine.DEF_PPID_SIZE));                                        //A40 LOT 지PPID
                                msg.AddAscii(AppUtil.ToAscii(sPRODUCTID[5], gDefine.DEF_PRODUCTID_SIZE));                              //A40 CELL Product Info
                                msg.AddAscii(AppUtil.ToAscii(sSTEPID[5], 40));                                                         //A40 CELL STEP ID Info
                            }
                        }
                        msg.AddList(2);                                                                                     //L2  RPTID 700 Set
                        {
                            msg.AddAscii(AppUtil.ToAscii("700", gDefine.DEF_RPTID_SIZE));                                       //A3  RPTID="700"
                            msg.AddList(Unit6_nOPCALL_NO);                                                                            //Ln  OPCALL
                            {
                                string[] Unit6_aOPCALL_ID = new string[Unit6_nOPCALL_NO];
                                string[] Unit6_aMESSAGE = new string[Unit6_nOPCALL_NO];

                                for (int i = 0; i < Unit6_nOPCALL_NO; i++)
                                {
                                    Unit6_aOPCALL_ID[i] = DataManager.Instance.GET_STRING_DATA("iPLC6_EtoC_OPCallIDComfirm", out bResult);
                                    Unit6_aMESSAGE[i] = DataManager.Instance.GET_STRING_DATA("iPLC6_EtoC_OPCallMessageConfirm", out bResult);

                                    msg.AddList(2);                                                                                 //L2  OPCALL Set
                                    {
                                        msg.AddAscii(AppUtil.ToAscii(Unit6_aOPCALL_ID[i], 20));                                               //A20 OPCALL Name  
                                        msg.AddAscii(AppUtil.ToAscii(Unit6_aMESSAGE[i], 120));                                                //A120 OPCALL Message
                                    }
                                }
                            }
                        }
                    }
                }

                this.SecsDriver.Send(msg);
                #endregion
            }

            else if (aUNITID[6] == sUNITID)
            {
                #region S6F11(OPCALL Confirm), CEID:501

                SecsMessage msg = new SecsMessage(stream, function, true, Convert.ToInt32(SecsDriver.DeviceID));

                msg.AddList(3);                                                                                     //L3  Opcall Event Info
                {
                    msg.AddAscii(AppUtil.ToAscii(sDataID, gDefine.DEF_DATAID_SIZE));                                    //A4  Data ID
                    msg.AddAscii(AppUtil.ToAscii(sCEID, gDefine.DEF_CEID_SIZE));                                        //A3  Collection Event ID
                    msg.AddList(4);                                                                                     //L4  RPTID Set
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
                            msg.AddAscii(AppUtil.ToAscii("102", gDefine.DEF_RPTID_SIZE));                                       //A3  RPTID="101"
                            msg.AddList(2);                                                                                     //L2  EQP State Set
                            {
                                msg.AddAscii(AppUtil.ToAscii(sUNITID, gDefine.DEF_UNITID_SIZE));                                      //A40 HOST REQ UNITID 
                                msg.AddList(9);                                                                                     //L9  EQP State Set
                                {
                                    msg.AddAscii(sAVAILABILITYSTATE[0]);                                                                   //A1  EQ Avilability State Info
                                    msg.AddAscii(sINTERLOCKSTATE[0]);                                                                      //A1  Interlock Avilability State Info
                                    msg.AddAscii(sMOVESTATE[0]);                                                                           //A1  EQ Move State Info
                                    msg.AddAscii(sRUNSTATE[0]);                                                                            //A1  Cell existence/nonexistence Check
                                    msg.AddAscii(sFRONTSTATE[0]);                                                                          //A1  Upper EQ Processing State
                                    msg.AddAscii(sREARSTATE[0]);                                                                           //A1  Lower EQ Processing State
                                    msg.AddAscii(sPP_SPLSTATE[0]);                                                                         //A1  Sample Run-Normal Run State
                                    msg.AddAscii(AppUtil.ToAscii(sREASONCODE[0], gDefine.DEF_REASONCODE_SIZE));                            //A20 Code Info
                                    msg.AddAscii(AppUtil.ToAscii(sDESCRIPTION[0], gDefine.DEF_DESCRIPTION_SIZE));                          //A40 EQ Description
                                }
                            }
                        }
                        msg.AddList(2);                                                                                     //L2  RPTID 300 Set
                        {
                            msg.AddAscii(AppUtil.ToAscii("300", gDefine.DEF_RPTID_SIZE));                                       //A3  RPTID="300"
                            msg.AddList(4);                                                                                     //L4  Cell Info Set
                            {
                                msg.AddAscii(AppUtil.ToAscii(sCELLID[6], 40));                                                         //A40 Cell Unique ID
                                msg.AddAscii(AppUtil.ToAscii(sPPID, gDefine.DEF_PPID_SIZE));                                        //A40 LOT 지PPID
                                msg.AddAscii(AppUtil.ToAscii(sPRODUCTID[6], gDefine.DEF_PRODUCTID_SIZE));                              //A40 CELL Product Info
                                msg.AddAscii(AppUtil.ToAscii(sSTEPID[6], 40));                                                         //A40 CELL STEP ID Info
                            }
                        }
                        msg.AddList(2);                                                                                     //L2  RPTID 700 Set
                        {
                            msg.AddAscii(AppUtil.ToAscii("700", gDefine.DEF_RPTID_SIZE));                                       //A3  RPTID="700"
                            msg.AddList(Unit7_nOPCALL_NO);                                                                            //Ln  OPCALL
                            {
                                string[] Unit7_aOPCALL_ID = new string[Unit7_nOPCALL_NO];
                                string[] Unit7_aMESSAGE = new string[Unit7_nOPCALL_NO];

                                for (int i = 0; i < Unit7_nOPCALL_NO; i++)
                                {
                                    Unit7_aOPCALL_ID[i] = DataManager.Instance.GET_STRING_DATA("iPLC7_EtoC_OPCallIDComfirm", out bResult);
                                    Unit7_aMESSAGE[i] = DataManager.Instance.GET_STRING_DATA("iPLC7_EtoC_OPCallMessageConfirm", out bResult);

                                    msg.AddList(2);                                                                                 //L2  OPCALL Set
                                    {
                                        msg.AddAscii(AppUtil.ToAscii(Unit7_aOPCALL_ID[i], 20));                                               //A20 OPCALL Name  
                                        msg.AddAscii(AppUtil.ToAscii(Unit7_aMESSAGE[i], 120));                                                //A120 OPCALL Message
                                    }
                                }
                            }
                        }
                    }
                }

                this.SecsDriver.Send(msg);
                #endregion
            }

            else if (aUNITID[7] == sUNITID)
            {
                #region S6F11(OPCALL Confirm), CEID:501

                SecsMessage msg = new SecsMessage(stream, function, true, Convert.ToInt32(SecsDriver.DeviceID));

                msg.AddList(3);                                                                                     //L3  Opcall Event Info
                {
                    msg.AddAscii(AppUtil.ToAscii(sDataID, gDefine.DEF_DATAID_SIZE));                                    //A4  Data ID
                    msg.AddAscii(AppUtil.ToAscii(sCEID, gDefine.DEF_CEID_SIZE));                                        //A3  Collection Event ID
                    msg.AddList(4);                                                                                     //L4  RPTID Set
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
                            msg.AddAscii(AppUtil.ToAscii("102", gDefine.DEF_RPTID_SIZE));                                       //A3  RPTID="101"
                            msg.AddList(2);                                                                                     //L2  EQP State Set
                            {
                                msg.AddAscii(AppUtil.ToAscii(sUNITID, gDefine.DEF_UNITID_SIZE));                                      //A40 HOST REQ UNITID 
                                msg.AddList(9);                                                                                     //L9  EQP State Set
                                {
                                    msg.AddAscii(sAVAILABILITYSTATE[0]);                                                                   //A1  EQ Avilability State Info
                                    msg.AddAscii(sINTERLOCKSTATE[0]);                                                                      //A1  Interlock Avilability State Info
                                    msg.AddAscii(sMOVESTATE[0]);                                                                           //A1  EQ Move State Info
                                    msg.AddAscii(sRUNSTATE[0]);                                                                            //A1  Cell existence/nonexistence Check
                                    msg.AddAscii(sFRONTSTATE[0]);                                                                          //A1  Upper EQ Processing State
                                    msg.AddAscii(sREARSTATE[0]);                                                                           //A1  Lower EQ Processing State
                                    msg.AddAscii(sPP_SPLSTATE[0]);                                                                         //A1  Sample Run-Normal Run State
                                    msg.AddAscii(AppUtil.ToAscii(sREASONCODE[0], gDefine.DEF_REASONCODE_SIZE));                            //A20 Code Info
                                    msg.AddAscii(AppUtil.ToAscii(sDESCRIPTION[0], gDefine.DEF_DESCRIPTION_SIZE));                          //A40 EQ Description
                                }
                            }
                        }
                        msg.AddList(2);                                                                                     //L2  RPTID 300 Set
                        {
                            msg.AddAscii(AppUtil.ToAscii("300", gDefine.DEF_RPTID_SIZE));                                       //A3  RPTID="300"
                            msg.AddList(4);                                                                                     //L4  Cell Info Set
                            {
                                msg.AddAscii(AppUtil.ToAscii(sCELLID[7], 40));                                                         //A40 Cell Unique ID
                                msg.AddAscii(AppUtil.ToAscii(sPPID, gDefine.DEF_PPID_SIZE));                                        //A40 LOT 지PPID
                                msg.AddAscii(AppUtil.ToAscii(sPRODUCTID[7], gDefine.DEF_PRODUCTID_SIZE));                              //A40 CELL Product Info
                                msg.AddAscii(AppUtil.ToAscii(sSTEPID[7], 40));                                                         //A40 CELL STEP ID Info
                            }
                        }
                        msg.AddList(2);                                                                                     //L2  RPTID 700 Set
                        {
                            msg.AddAscii(AppUtil.ToAscii("700", gDefine.DEF_RPTID_SIZE));                                       //A3  RPTID="700"
                            msg.AddList(Unit8_nOPCALL_NO);                                                                            //Ln  OPCALL
                            {
                                string[] Unit8_aOPCALL_ID = new string[Unit8_nOPCALL_NO];
                                string[] Unit8_aMESSAGE = new string[Unit8_nOPCALL_NO];

                                for (int i = 0; i < Unit8_nOPCALL_NO; i++)
                                {
                                    Unit8_aOPCALL_ID[i] = DataManager.Instance.GET_STRING_DATA("iPLC8_EtoC_OPCallIDComfirm", out bResult);
                                    Unit8_aMESSAGE[i] = DataManager.Instance.GET_STRING_DATA("iPLC8_EtoC_OPCallMessageConfirm", out bResult);

                                    msg.AddList(2);                                                                                 //L2  OPCALL Set
                                    {
                                        msg.AddAscii(AppUtil.ToAscii(Unit8_aOPCALL_ID[i], 20));                                               //A20 OPCALL Name  
                                        msg.AddAscii(AppUtil.ToAscii(Unit8_aMESSAGE[i], 120));                                                //A120 OPCALL Message
                                    }
                                }
                            }
                        }
                    }
                }

                this.SecsDriver.Send(msg);
                #endregion
            }
        }
    }
}
