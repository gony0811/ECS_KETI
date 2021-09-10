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
    public class S6F11_Unit_EquipmentLossCodeReport : SFMessage
    {
        /*S6F11(Equipment Loss Code Report), CEID:616*/
        public S6F11_Unit_EquipmentLossCodeReport(SECSDriver driver) 
            : base(driver)
        {
            
        }
        public override void DoWork(string driverName, object obj)
        {
            #region DEFINE
            int stream = 6, function = 11;
            bool bResult;
            string sEQPID = CommonData.Instance.EQP_SETTINGS.EQPID;
            string sCEID = "616";
            string sUNITID = string.Empty; // 어디에서 가져올지 정해지면 코딩.

            string sDataID = "0";
            string sCRST = ((int)DataManager.Instance.GET_INT_DATA("vSys_HostData_Mode", out bResult)).ToString();

            //int UNITInfo_List = 1;                             //Undefine
            int nUnit_List = DataManager.Instance.GET_INT_DATA("vSys_UnitInfos_Count", out bResult); // m_lnkHostData.UnitNo;

            string[] sAVAILABILITYSTATE = new string[nUnit_List];
            string[] sINTERLOCKSTATE = new string[nUnit_List];
            string[] sMOVESTATE = new string[nUnit_List];
            string[] sRUNSTATE = new string[nUnit_List];
            string[] sFRONTSTATE = new string[nUnit_List];
            string[] sREARSTATE = new string[nUnit_List];
            string[] sPP_SPLSTATE = new string[nUnit_List];
            string[] sREASONCODE = new string[nUnit_List];
            string[] sDESCRIPTION = new string[nUnit_List];
            string[] sLOSS_CODE = new string[nUnit_List];
            string[] sLOSS_DESCRIPTION = new string[nUnit_List];

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
                sLOSS_CODE[0] = DataManager.Instance.GET_INT_DATA("iPLC1_EtoC_TMPLossCode", out bResult).ToString();
                sLOSS_DESCRIPTION[0] = DataManager.Instance.GET_STRING_DATA("iPLC1_EtoC_TMPLossDescp", out bResult);

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
                    sLOSS_CODE[1] = DataManager.Instance.GET_INT_DATA("iPLC2_EtoC_TMPLossCode", out bResult).ToString();
                    sLOSS_DESCRIPTION[1] = DataManager.Instance.GET_STRING_DATA("iPLC2_EtoC_TMPLossDescp", out bResult);
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
                    sLOSS_CODE[2] = DataManager.Instance.GET_INT_DATA("iPLC3_EtoC_TMPLossCode", out bResult).ToString();
                    sLOSS_DESCRIPTION[2] = DataManager.Instance.GET_STRING_DATA("iPLC3_EtoC_TMPLossDescp", out bResult);
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
                    sLOSS_CODE[3] = DataManager.Instance.GET_INT_DATA("iPLC4_EtoC_TMPLossCode", out bResult).ToString();
                    sLOSS_DESCRIPTION[3] = DataManager.Instance.GET_STRING_DATA("iPLC4_EtoC_TMPLossDescp", out bResult);
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
                    sLOSS_CODE[4] = DataManager.Instance.GET_INT_DATA("iPLC5_EtoC_TMPLossCode", out bResult).ToString();
                    sLOSS_DESCRIPTION[4] = DataManager.Instance.GET_STRING_DATA("iPLC5_EtoC_TMPLossDescp", out bResult);
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
                    sLOSS_CODE[5] = DataManager.Instance.GET_INT_DATA("iPLC6_EtoC_TMPLossCode", out bResult).ToString();
                    sLOSS_DESCRIPTION[5] = DataManager.Instance.GET_STRING_DATA("iPLC6_EtoC_TMPLossDescp", out bResult);
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
                    sLOSS_CODE[6] = DataManager.Instance.GET_INT_DATA("iPLC7_EtoC_TMPLossCode", out bResult).ToString();
                    sLOSS_DESCRIPTION[6] = DataManager.Instance.GET_STRING_DATA("iPLC7_EtoC_TMPLossDescp", out bResult);
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
                    sLOSS_CODE[7] = DataManager.Instance.GET_INT_DATA("iPLC8_EtoC_TMPLossCode", out bResult).ToString();
                    sLOSS_DESCRIPTION[7] = DataManager.Instance.GET_STRING_DATA("iPLC8_EtoC_TMPLossDescp", out bResult);
                }
            }

            string[] aUNITID = new string[nUnit_List];

            //170401 HJKIM : LOSS CODE 3000일 경우, 03000으로 변경
            sLOSS_CODE[0] = sLOSS_CODE[0].Trim();
            sLOSS_CODE[1] = sLOSS_CODE[1].Trim();
            sLOSS_CODE[2] = sLOSS_CODE[2].Trim();
            sLOSS_CODE[3] = sLOSS_CODE[3].Trim();
            sLOSS_CODE[4] = sLOSS_CODE[4].Trim();
            sLOSS_CODE[5] = sLOSS_CODE[5].Trim();
            sLOSS_CODE[6] = sLOSS_CODE[6].Trim();
            sLOSS_CODE[7] = sLOSS_CODE[7].Trim();

            if (sLOSS_CODE[0].Length < 5) sLOSS_CODE[0] = "0" + sLOSS_CODE[0];
            if (sLOSS_CODE[1].Length < 5) sLOSS_CODE[1] = "0" + sLOSS_CODE[1];
            if (sLOSS_CODE[2].Length < 5) sLOSS_CODE[2] = "0" + sLOSS_CODE[2];
            if (sLOSS_CODE[3].Length < 5) sLOSS_CODE[3] = "0" + sLOSS_CODE[3];
            if (sLOSS_CODE[4].Length < 5) sLOSS_CODE[4] = "0" + sLOSS_CODE[4];
            if (sLOSS_CODE[5].Length < 5) sLOSS_CODE[5] = "0" + sLOSS_CODE[5];
            if (sLOSS_CODE[6].Length < 5) sLOSS_CODE[6] = "0" + sLOSS_CODE[6];
            if (sLOSS_CODE[7].Length < 5) sLOSS_CODE[7] = "0" + sLOSS_CODE[7];
            #endregion

            if (aUNITID[0] == sUNITID)
            {
                #region S6F11(Equipment Loss Code Report), CEID:616

                SecsMessage msg = new SecsMessage(stream, function, true, Convert.ToInt32(SecsDriver.DeviceID));

                msg.AddList(3);                                                                                  //L3  EQP Status Event Info
                {
                    msg.AddAscii(AppUtil.ToAscii(sDataID, gDefine.DEF_DATAID_SIZE));                                //A4  Data ID
                    msg.AddAscii(AppUtil.ToAscii(sCEID, gDefine.DEF_CEID_SIZE));                                    //A3  Collection Event ID
                    msg.AddList(3);                                                                                 //L3  RPTID Set
                    {
                        msg.AddList(2);                                                                                 //L2  RPTID 100 Set 
                        {
                            msg.AddAscii(AppUtil.ToAscii("100", gDefine.DEF_RPTID_SIZE));                                   //A3  RPTID="100" 
                            msg.AddList(2);                                                                                 //L2  EQP Control State Set 
                            {
                                msg.AddAscii(AppUtil.ToAscii(sEQPID, gDefine.DEF_EQPID_SIZE));                                  //A40 HOST REQ EQPID 
                                msg.AddAscii(sCRST);                                                                            //A1  Online Control State 
                            }
                        }
                        msg.AddList(2);                                                                                 //L2  RPTID 101 Set
                        {
                            msg.AddAscii(AppUtil.ToAscii("102", gDefine.DEF_RPTID_SIZE));                                   //A3  RPTID="102"
                            msg.AddList(2);                                                                                 //L2  RPTID 101 Set
                            {
                                msg.AddAscii(AppUtil.ToAscii(sUNITID, gDefine.DEF_UNITID_SIZE));                                  //A40 HOST REQ UNITID 
                                msg.AddList(9);                                                                                 //L9  EQP State Set
                                {
                                    msg.AddAscii(sAVAILABILITYSTATE[0]);                                                               //A1  EQ Avilability State Info
                                    msg.AddAscii(sINTERLOCKSTATE[0]);                                                                  //A1  Interlock Avilability State Info
                                    msg.AddAscii(sMOVESTATE[0]);                                                                       //A1  EQ Move State Info
                                    msg.AddAscii(sRUNSTATE[0]);                                                                        //A1  Cell existence/nonexistence Check
                                    msg.AddAscii(sFRONTSTATE[0]);                                                                      //A1  Upper EQ Processing State
                                    msg.AddAscii(sREARSTATE[0]);                                                                       //A1  Lower EQ Processing State
                                    msg.AddAscii(sPP_SPLSTATE[0]);                                                                     //A1  Sample Run-Normal Run State
                                    msg.AddAscii(AppUtil.ToAscii(sREASONCODE[0], gDefine.DEF_REASONCODE_SIZE));                        //A20 Code Info
                                    msg.AddAscii(AppUtil.ToAscii(sDESCRIPTION[0], gDefine.DEF_DESCRIPTION_SIZE));                      //A40 EQ Description
                                }
                            }
                        }
                        msg.AddList(2);                                                                                 //L2  RPTID 805 Set
                        {
                            msg.AddAscii(AppUtil.ToAscii("806", gDefine.DEF_RPTID_SIZE));                                   //A3  RPTID="806"
                            msg.AddList(2);                                                                                 //L2  Loss Code List
                            {
                                msg.AddAscii(AppUtil.ToAscii(sLOSS_CODE[0], 20));                                                  //A20 Loss Code 
                                msg.AddAscii(AppUtil.ToAscii(sLOSS_DESCRIPTION[0], 40));                                           //A40 Loss Code DESCRIPTION
                            }
                        }
                    }
                }

                m_lnkMachineInfo.Unit1_AvailabilityState = sAVAILABILITYSTATE[0];
                m_lnkMachineInfo.Unit1_InterlockState = sINTERLOCKSTATE[0];
                m_lnkMachineInfo.Unit1_MoveState = sMOVESTATE[0];
                m_lnkMachineInfo.Unit1_RunState = sRUNSTATE[0];
                m_lnkMachineInfo.Unit1_FrontState = sFRONTSTATE[0];
                m_lnkMachineInfo.Unit1_RearState = sREARSTATE[0];
                m_lnkMachineInfo.Unit1_PPSPLState = sPP_SPLSTATE[0];
                m_lnkMachineInfo.Unit1_ReasonCode = sREASONCODE[0];
                m_lnkMachineInfo.Unit1_Description = sDESCRIPTION[0];

                this.SecsDriver.Send(msg);
                #endregion
            }

            else if (aUNITID[1] == sUNITID)
            {
                #region S6F11(Equipment Loss Code Report), CEID:616

                SecsMessage msg = new SecsMessage(stream, function, true, Convert.ToInt32(SecsDriver.DeviceID));

                msg.AddList(3);                                                                                  //L3  EQP Status Event Info
                {
                    msg.AddAscii(AppUtil.ToAscii(sDataID, gDefine.DEF_DATAID_SIZE));                                //A4  Data ID
                    msg.AddAscii(AppUtil.ToAscii(sCEID, gDefine.DEF_CEID_SIZE));                                    //A3  Collection Event ID
                    msg.AddList(3);                                                                                 //L3  RPTID Set
                    {
                        msg.AddList(2);                                                                                 //L2  RPTID 100 Set 
                        {
                            msg.AddAscii(AppUtil.ToAscii("100", gDefine.DEF_RPTID_SIZE));                                   //A3  RPTID="100" 
                            msg.AddList(2);                                                                                 //L2  EQP Control State Set 
                            {
                                msg.AddAscii(AppUtil.ToAscii(sEQPID, gDefine.DEF_EQPID_SIZE));                                  //A40 HOST REQ EQPID 
                                msg.AddAscii(sCRST);                                                                            //A1  Online Control State 
                            }
                        }
                        msg.AddList(2);                                                                                 //L2  RPTID 101 Set
                        {
                            msg.AddAscii(AppUtil.ToAscii("102", gDefine.DEF_RPTID_SIZE));                                   //A3  RPTID="102"
                            msg.AddList(2);                                                                                 //L2  RPTID 101 Set
                            {
                                msg.AddAscii(AppUtil.ToAscii(sUNITID, gDefine.DEF_UNITID_SIZE));                                  //A40 HOST REQ UNITID 
                                msg.AddList(9);                                                                                 //L9  EQP State Set
                                {
                                    msg.AddAscii(sAVAILABILITYSTATE[1]);                                                               //A1  EQ Avilability State Info
                                    msg.AddAscii(sINTERLOCKSTATE[1]);                                                                  //A1  Interlock Avilability State Info
                                    msg.AddAscii(sMOVESTATE[1]);                                                                       //A1  EQ Move State Info
                                    msg.AddAscii(sRUNSTATE[1]);                                                                        //A1  Cell existence/nonexistence Check
                                    msg.AddAscii(sFRONTSTATE[1]);                                                                      //A1  Upper EQ Processing State
                                    msg.AddAscii(sREARSTATE[1]);                                                                       //A1  Lower EQ Processing State
                                    msg.AddAscii(sPP_SPLSTATE[1]);                                                                     //A1  Sample Run-Normal Run State
                                    msg.AddAscii(AppUtil.ToAscii(sREASONCODE[1], gDefine.DEF_REASONCODE_SIZE));                        //A20 Code Info
                                    msg.AddAscii(AppUtil.ToAscii(sDESCRIPTION[1], gDefine.DEF_DESCRIPTION_SIZE));                      //A40 EQ Description
                                }
                            }
                        }
                        msg.AddList(2);                                                                                 //L2  RPTID 805 Set
                        {
                            msg.AddAscii(AppUtil.ToAscii("806", gDefine.DEF_RPTID_SIZE));                                   //A3  RPTID="806"
                            msg.AddList(2);                                                                                 //L2  Loss Code List
                            {
                                msg.AddAscii(AppUtil.ToAscii(sLOSS_CODE[1], 20));                                                  //A20 Loss Code 
                                msg.AddAscii(AppUtil.ToAscii(sLOSS_DESCRIPTION[1], 40));                                           //A40 Loss Code DESCRIPTION
                            }
                        }
                    }
                }

                m_lnkMachineInfo.Unit2_AvailabilityState = sAVAILABILITYSTATE[1];
                m_lnkMachineInfo.Unit2_InterlockState = sINTERLOCKSTATE[1];
                m_lnkMachineInfo.Unit2_MoveState = sMOVESTATE[1];
                m_lnkMachineInfo.Unit2_RunState = sRUNSTATE[1];
                m_lnkMachineInfo.Unit2_FrontState = sFRONTSTATE[1];
                m_lnkMachineInfo.Unit2_RearState = sREARSTATE[1];
                m_lnkMachineInfo.Unit2_PPSPLState = sPP_SPLSTATE[1];
                m_lnkMachineInfo.Unit2_ReasonCode = sREASONCODE[1];
                m_lnkMachineInfo.Unit2_Description = sDESCRIPTION[1];

                this.SecsDriver.Send(msg);
                #endregion
            }

            else if (aUNITID[2] == sUNITID)
            {
                #region S6F11(Equipment Loss Code Report), CEID:616

                SecsMessage msg = new SecsMessage(stream, function, true, Convert.ToInt32(SecsDriver.DeviceID));

                msg.AddList(3);                                                                                  //L3  EQP Status Event Info
                {
                    msg.AddAscii(AppUtil.ToAscii(sDataID, gDefine.DEF_DATAID_SIZE));                                //A4  Data ID
                    msg.AddAscii(AppUtil.ToAscii(sCEID, gDefine.DEF_CEID_SIZE));                                    //A3  Collection Event ID
                    msg.AddList(3);                                                                                 //L3  RPTID Set
                    {
                        msg.AddList(2);                                                                                 //L2  RPTID 100 Set 
                        {
                            msg.AddAscii(AppUtil.ToAscii("100", gDefine.DEF_RPTID_SIZE));                                   //A3  RPTID="100" 
                            msg.AddList(2);                                                                                 //L2  EQP Control State Set 
                            {
                                msg.AddAscii(AppUtil.ToAscii(sEQPID, gDefine.DEF_EQPID_SIZE));                                  //A40 HOST REQ EQPID 
                                msg.AddAscii(sCRST);                                                                            //A1  Online Control State 
                            }
                        }
                        msg.AddList(2);                                                                                 //L2  RPTID 101 Set
                        {
                            msg.AddAscii(AppUtil.ToAscii("102", gDefine.DEF_RPTID_SIZE));                                   //A3  RPTID="102"
                            msg.AddList(2);                                                                                 //L2  RPTID 101 Set
                            {
                                msg.AddAscii(AppUtil.ToAscii(sUNITID, gDefine.DEF_UNITID_SIZE));                                  //A40 HOST REQ UNITID 
                                msg.AddList(9);                                                                                 //L9  EQP State Set
                                {
                                    msg.AddAscii(sAVAILABILITYSTATE[2]);                                                               //A1  EQ Avilability State Info
                                    msg.AddAscii(sINTERLOCKSTATE[2]);                                                                  //A1  Interlock Avilability State Info
                                    msg.AddAscii(sMOVESTATE[2]);                                                                       //A1  EQ Move State Info
                                    msg.AddAscii(sRUNSTATE[2]);                                                                        //A1  Cell existence/nonexistence Check
                                    msg.AddAscii(sFRONTSTATE[2]);                                                                      //A1  Upper EQ Processing State
                                    msg.AddAscii(sREARSTATE[2]);                                                                       //A1  Lower EQ Processing State
                                    msg.AddAscii(sPP_SPLSTATE[2]);                                                                     //A1  Sample Run-Normal Run State
                                    msg.AddAscii(AppUtil.ToAscii(sREASONCODE[2], gDefine.DEF_REASONCODE_SIZE));                        //A20 Code Info
                                    msg.AddAscii(AppUtil.ToAscii(sDESCRIPTION[2], gDefine.DEF_DESCRIPTION_SIZE));                      //A40 EQ Description
                                }
                            }
                        }
                        msg.AddList(2);                                                                                 //L2  RPTID 805 Set
                        {
                            msg.AddAscii(AppUtil.ToAscii("806", gDefine.DEF_RPTID_SIZE));                                   //A3  RPTID="806"
                            msg.AddList(2);                                                                                 //L2  Loss Code List
                            {
                                msg.AddAscii(AppUtil.ToAscii(sLOSS_CODE[2], 20));                                                  //A20 Loss Code 
                                msg.AddAscii(AppUtil.ToAscii(sLOSS_DESCRIPTION[2], 40));                                           //A40 Loss Code DESCRIPTION
                            }
                        }
                    }
                }

                m_lnkMachineInfo.Unit3_AvailabilityState = sAVAILABILITYSTATE[2];
                m_lnkMachineInfo.Unit3_InterlockState = sINTERLOCKSTATE[2];
                m_lnkMachineInfo.Unit3_MoveState = sMOVESTATE[2];
                m_lnkMachineInfo.Unit3_RunState = sRUNSTATE[2];
                m_lnkMachineInfo.Unit3_FrontState = sFRONTSTATE[2];
                m_lnkMachineInfo.Unit3_RearState = sREARSTATE[2];
                m_lnkMachineInfo.Unit3_PPSPLState = sPP_SPLSTATE[2];
                m_lnkMachineInfo.Unit3_ReasonCode = sREASONCODE[2];
                m_lnkMachineInfo.Unit3_Description = sDESCRIPTION[2];

                this.SecsDriver.Send(msg);
                #endregion
            }

            else if (aUNITID[3] == sUNITID)
            {
                #region S6F11(Equipment Loss Code Report), CEID:616

                SecsMessage msg = new SecsMessage(stream, function, true, Convert.ToInt32(SecsDriver.DeviceID));

                msg.AddList(3);                                                                                  //L3  EQP Status Event Info
                {
                    msg.AddAscii(AppUtil.ToAscii(sDataID, gDefine.DEF_DATAID_SIZE));                                //A4  Data ID
                    msg.AddAscii(AppUtil.ToAscii(sCEID, gDefine.DEF_CEID_SIZE));                                    //A3  Collection Event ID
                    msg.AddList(3);                                                                                 //L3  RPTID Set
                    {
                        msg.AddList(2);                                                                                 //L2  RPTID 100 Set 
                        {
                            msg.AddAscii(AppUtil.ToAscii("100", gDefine.DEF_RPTID_SIZE));                                   //A3  RPTID="100" 
                            msg.AddList(2);                                                                                 //L2  EQP Control State Set 
                            {
                                msg.AddAscii(AppUtil.ToAscii(sEQPID, gDefine.DEF_EQPID_SIZE));                                  //A40 HOST REQ EQPID 
                                msg.AddAscii(sCRST);                                                                            //A1  Online Control State 
                            }
                        }
                        msg.AddList(2);                                                                                 //L2  RPTID 101 Set
                        {
                            msg.AddAscii(AppUtil.ToAscii("102", gDefine.DEF_RPTID_SIZE));                                   //A3  RPTID="102"
                            msg.AddList(2);                                                                                 //L2  RPTID 101 Set
                            {
                                msg.AddAscii(AppUtil.ToAscii(sUNITID, gDefine.DEF_UNITID_SIZE));                                  //A40 HOST REQ UNITID 
                                msg.AddList(9);                                                                                 //L9  EQP State Set
                                {
                                    msg.AddAscii(sAVAILABILITYSTATE[3]);                                                               //A1  EQ Avilability State Info
                                    msg.AddAscii(sINTERLOCKSTATE[3]);                                                                  //A1  Interlock Avilability State Info
                                    msg.AddAscii(sMOVESTATE[3]);                                                                       //A1  EQ Move State Info
                                    msg.AddAscii(sRUNSTATE[3]);                                                                        //A1  Cell existence/nonexistence Check
                                    msg.AddAscii(sFRONTSTATE[3]);                                                                      //A1  Upper EQ Processing State
                                    msg.AddAscii(sREARSTATE[3]);                                                                       //A1  Lower EQ Processing State
                                    msg.AddAscii(sPP_SPLSTATE[3]);                                                                     //A1  Sample Run-Normal Run State
                                    msg.AddAscii(AppUtil.ToAscii(sREASONCODE[3], gDefine.DEF_REASONCODE_SIZE));                        //A20 Code Info
                                    msg.AddAscii(AppUtil.ToAscii(sDESCRIPTION[3], gDefine.DEF_DESCRIPTION_SIZE));                      //A40 EQ Description
                                }
                            }
                        }
                        msg.AddList(2);                                                                                 //L2  RPTID 805 Set
                        {
                            msg.AddAscii(AppUtil.ToAscii("806", gDefine.DEF_RPTID_SIZE));                                   //A3  RPTID="806"
                            msg.AddList(2);                                                                                 //L2  Loss Code List
                            {
                                msg.AddAscii(AppUtil.ToAscii(sLOSS_CODE[3], 20));                                                  //A20 Loss Code 
                                msg.AddAscii(AppUtil.ToAscii(sLOSS_DESCRIPTION[3], 40));                                           //A40 Loss Code DESCRIPTION
                            }
                        }
                    }
                }

                m_lnkMachineInfo.Unit4_AvailabilityState = sAVAILABILITYSTATE[3];
                m_lnkMachineInfo.Unit4_InterlockState = sINTERLOCKSTATE[3];
                m_lnkMachineInfo.Unit4_MoveState = sMOVESTATE[3];
                m_lnkMachineInfo.Unit4_RunState = sRUNSTATE[3];
                m_lnkMachineInfo.Unit4_FrontState = sFRONTSTATE[3];
                m_lnkMachineInfo.Unit4_RearState = sREARSTATE[3];
                m_lnkMachineInfo.Unit4_PPSPLState = sPP_SPLSTATE[3];
                m_lnkMachineInfo.Unit4_ReasonCode = sREASONCODE[3];
                m_lnkMachineInfo.Unit4_Description = sDESCRIPTION[3];

                this.SecsDriver.Send(msg);
                #endregion
            }

            else if (aUNITID[4] == sUNITID)
            {
                #region S6F11(Equipment Loss Code Report), CEID:616

                SecsMessage msg = new SecsMessage(stream, function, true, Convert.ToInt32(SecsDriver.DeviceID));

                msg.AddList(3);                                                                                  //L3  EQP Status Event Info
                {
                    msg.AddAscii(AppUtil.ToAscii(sDataID, gDefine.DEF_DATAID_SIZE));                                //A4  Data ID
                    msg.AddAscii(AppUtil.ToAscii(sCEID, gDefine.DEF_CEID_SIZE));                                    //A3  Collection Event ID
                    msg.AddList(3);                                                                                 //L3  RPTID Set
                    {
                        msg.AddList(2);                                                                                 //L2  RPTID 100 Set 
                        {
                            msg.AddAscii(AppUtil.ToAscii("100", gDefine.DEF_RPTID_SIZE));                                   //A3  RPTID="100" 
                            msg.AddList(2);                                                                                 //L2  EQP Control State Set 
                            {
                                msg.AddAscii(AppUtil.ToAscii(sEQPID, gDefine.DEF_EQPID_SIZE));                                  //A40 HOST REQ EQPID 
                                msg.AddAscii(sCRST);                                                                            //A1  Online Control State 
                            }
                        }
                        msg.AddList(2);                                                                                 //L2  RPTID 101 Set
                        {
                            msg.AddAscii(AppUtil.ToAscii("102", gDefine.DEF_RPTID_SIZE));                                   //A3  RPTID="102"
                            msg.AddList(2);                                                                                 //L2  RPTID 101 Set
                            {
                                msg.AddAscii(AppUtil.ToAscii(sUNITID, gDefine.DEF_UNITID_SIZE));                                  //A40 HOST REQ UNITID 
                                msg.AddList(9);                                                                                 //L9  EQP State Set
                                {
                                    msg.AddAscii(sAVAILABILITYSTATE[4]);                                                               //A1  EQ Avilability State Info
                                    msg.AddAscii(sINTERLOCKSTATE[4]);                                                                  //A1  Interlock Avilability State Info
                                    msg.AddAscii(sMOVESTATE[4]);                                                                       //A1  EQ Move State Info
                                    msg.AddAscii(sRUNSTATE[4]);                                                                        //A1  Cell existence/nonexistence Check
                                    msg.AddAscii(sFRONTSTATE[4]);                                                                      //A1  Upper EQ Processing State
                                    msg.AddAscii(sREARSTATE[4]);                                                                       //A1  Lower EQ Processing State
                                    msg.AddAscii(sPP_SPLSTATE[4]);                                                                     //A1  Sample Run-Normal Run State
                                    msg.AddAscii(AppUtil.ToAscii(sREASONCODE[4], gDefine.DEF_REASONCODE_SIZE));                        //A20 Code Info
                                    msg.AddAscii(AppUtil.ToAscii(sDESCRIPTION[4], gDefine.DEF_DESCRIPTION_SIZE));                      //A40 EQ Description
                                }
                            }
                        }
                        msg.AddList(2);                                                                                 //L2  RPTID 805 Set
                        {
                            msg.AddAscii(AppUtil.ToAscii("806", gDefine.DEF_RPTID_SIZE));                                   //A3  RPTID="806"
                            msg.AddList(2);                                                                                 //L2  Loss Code List
                            {
                                msg.AddAscii(AppUtil.ToAscii(sLOSS_CODE[4], 20));                                                  //A20 Loss Code 
                                msg.AddAscii(AppUtil.ToAscii(sLOSS_DESCRIPTION[4], 40));                                           //A40 Loss Code DESCRIPTION
                            }
                        }
                    }
                }

                m_lnkMachineInfo.Unit5_AvailabilityState = sAVAILABILITYSTATE[4];
                m_lnkMachineInfo.Unit5_InterlockState = sINTERLOCKSTATE[4];
                m_lnkMachineInfo.Unit5_MoveState = sMOVESTATE[4];
                m_lnkMachineInfo.Unit5_RunState = sRUNSTATE[4];
                m_lnkMachineInfo.Unit5_FrontState = sFRONTSTATE[4];
                m_lnkMachineInfo.Unit5_RearState = sREARSTATE[4];
                m_lnkMachineInfo.Unit5_PPSPLState = sPP_SPLSTATE[4];
                m_lnkMachineInfo.Unit5_ReasonCode = sREASONCODE[4];
                m_lnkMachineInfo.Unit5_Description = sDESCRIPTION[4];

                this.SecsDriver.Send(msg);
                #endregion
            }

            else if (aUNITID[5] == sUNITID)
            {
                #region S6F11(Equipment Loss Code Report), CEID:616

                SecsMessage msg = new SecsMessage(stream, function, true, Convert.ToInt32(SecsDriver.DeviceID));

                msg.AddList(3);                                                                                  //L3  EQP Status Event Info
                {
                    msg.AddAscii(AppUtil.ToAscii(sDataID, gDefine.DEF_DATAID_SIZE));                                //A4  Data ID
                    msg.AddAscii(AppUtil.ToAscii(sCEID, gDefine.DEF_CEID_SIZE));                                    //A3  Collection Event ID
                    msg.AddList(3);                                                                                 //L3  RPTID Set
                    {
                        msg.AddList(2);                                                                                 //L2  RPTID 100 Set 
                        {
                            msg.AddAscii(AppUtil.ToAscii("100", gDefine.DEF_RPTID_SIZE));                                   //A3  RPTID="100" 
                            msg.AddList(2);                                                                                 //L2  EQP Control State Set 
                            {
                                msg.AddAscii(AppUtil.ToAscii(sEQPID, gDefine.DEF_EQPID_SIZE));                                  //A40 HOST REQ EQPID 
                                msg.AddAscii(sCRST);                                                                            //A1  Online Control State 
                            }
                        }
                        msg.AddList(2);                                                                                 //L2  RPTID 101 Set
                        {
                            msg.AddAscii(AppUtil.ToAscii("102", gDefine.DEF_RPTID_SIZE));                                   //A3  RPTID="102"
                            msg.AddList(2);                                                                                 //L2  RPTID 101 Set
                            {
                                msg.AddAscii(AppUtil.ToAscii(sUNITID, gDefine.DEF_UNITID_SIZE));                                  //A40 HOST REQ UNITID 
                                msg.AddList(9);                                                                                 //L9  EQP State Set
                                {
                                    msg.AddAscii(sAVAILABILITYSTATE[5]);                                                               //A1  EQ Avilability State Info
                                    msg.AddAscii(sINTERLOCKSTATE[5]);                                                                  //A1  Interlock Avilability State Info
                                    msg.AddAscii(sMOVESTATE[5]);                                                                       //A1  EQ Move State Info
                                    msg.AddAscii(sRUNSTATE[5]);                                                                        //A1  Cell existence/nonexistence Check
                                    msg.AddAscii(sFRONTSTATE[5]);                                                                      //A1  Upper EQ Processing State
                                    msg.AddAscii(sREARSTATE[5]);                                                                       //A1  Lower EQ Processing State
                                    msg.AddAscii(sPP_SPLSTATE[5]);                                                                     //A1  Sample Run-Normal Run State
                                    msg.AddAscii(AppUtil.ToAscii(sREASONCODE[5], gDefine.DEF_REASONCODE_SIZE));                        //A20 Code Info
                                    msg.AddAscii(AppUtil.ToAscii(sDESCRIPTION[5], gDefine.DEF_DESCRIPTION_SIZE));                      //A40 EQ Description
                                }
                            }
                        }
                        msg.AddList(2);                                                                                 //L2  RPTID 805 Set
                        {
                            msg.AddAscii(AppUtil.ToAscii("806", gDefine.DEF_RPTID_SIZE));                                   //A3  RPTID="806"
                            msg.AddList(2);                                                                                 //L2  Loss Code List
                            {
                                msg.AddAscii(AppUtil.ToAscii(sLOSS_CODE[5], 20));                                                  //A20 Loss Code 
                                msg.AddAscii(AppUtil.ToAscii(sLOSS_DESCRIPTION[5], 40));                                           //A40 Loss Code DESCRIPTION
                            }
                        }
                    }
                }

                m_lnkMachineInfo.Unit6_AvailabilityState = sAVAILABILITYSTATE[5];
                m_lnkMachineInfo.Unit6_InterlockState = sINTERLOCKSTATE[5];
                m_lnkMachineInfo.Unit6_MoveState = sMOVESTATE[5];
                m_lnkMachineInfo.Unit6_RunState = sRUNSTATE[5];
                m_lnkMachineInfo.Unit6_FrontState = sFRONTSTATE[5];
                m_lnkMachineInfo.Unit6_RearState = sREARSTATE[5];
                m_lnkMachineInfo.Unit6_PPSPLState = sPP_SPLSTATE[5];
                m_lnkMachineInfo.Unit6_ReasonCode = sREASONCODE[5];
                m_lnkMachineInfo.Unit6_Description = sDESCRIPTION[5];

                this.SecsDriver.Send(msg);
                #endregion
            }

            else if (aUNITID[6] == sUNITID)
            {
                #region S6F11(Equipment Loss Code Report), CEID:616

                SecsMessage msg = new SecsMessage(stream, function, true, Convert.ToInt32(SecsDriver.DeviceID));

                msg.AddList(3);                                                                                  //L3  EQP Status Event Info
                {
                    msg.AddAscii(AppUtil.ToAscii(sDataID, gDefine.DEF_DATAID_SIZE));                                //A4  Data ID
                    msg.AddAscii(AppUtil.ToAscii(sCEID, gDefine.DEF_CEID_SIZE));                                    //A3  Collection Event ID
                    msg.AddList(3);                                                                                 //L3  RPTID Set
                    {
                        msg.AddList(2);                                                                                 //L2  RPTID 100 Set 
                        {
                            msg.AddAscii(AppUtil.ToAscii("100", gDefine.DEF_RPTID_SIZE));                                   //A3  RPTID="100" 
                            msg.AddList(2);                                                                                 //L2  EQP Control State Set 
                            {
                                msg.AddAscii(AppUtil.ToAscii(sEQPID, gDefine.DEF_EQPID_SIZE));                                  //A40 HOST REQ EQPID 
                                msg.AddAscii(sCRST);                                                                            //A1  Online Control State 
                            }
                        }
                        msg.AddList(2);                                                                                 //L2  RPTID 101 Set
                        {
                            msg.AddAscii(AppUtil.ToAscii("102", gDefine.DEF_RPTID_SIZE));                                   //A3  RPTID="102"
                            msg.AddList(2);                                                                                 //L2  RPTID 101 Set
                            {
                                msg.AddAscii(AppUtil.ToAscii(sUNITID, gDefine.DEF_UNITID_SIZE));                                  //A40 HOST REQ UNITID 
                                msg.AddList(9);                                                                                 //L9  EQP State Set
                                {
                                    msg.AddAscii(sAVAILABILITYSTATE[6]);                                                               //A1  EQ Avilability State Info
                                    msg.AddAscii(sINTERLOCKSTATE[6]);                                                                  //A1  Interlock Avilability State Info
                                    msg.AddAscii(sMOVESTATE[6]);                                                                       //A1  EQ Move State Info
                                    msg.AddAscii(sRUNSTATE[6]);                                                                        //A1  Cell existence/nonexistence Check
                                    msg.AddAscii(sFRONTSTATE[6]);                                                                      //A1  Upper EQ Processing State
                                    msg.AddAscii(sREARSTATE[6]);                                                                       //A1  Lower EQ Processing State
                                    msg.AddAscii(sPP_SPLSTATE[6]);                                                                     //A1  Sample Run-Normal Run State
                                    msg.AddAscii(AppUtil.ToAscii(sREASONCODE[6], gDefine.DEF_REASONCODE_SIZE));                        //A20 Code Info
                                    msg.AddAscii(AppUtil.ToAscii(sDESCRIPTION[6], gDefine.DEF_DESCRIPTION_SIZE));                      //A40 EQ Description
                                }
                            }
                        }
                        msg.AddList(2);                                                                                 //L2  RPTID 805 Set
                        {
                            msg.AddAscii(AppUtil.ToAscii("806", gDefine.DEF_RPTID_SIZE));                                   //A3  RPTID="806"
                            msg.AddList(2);                                                                                 //L2  Loss Code List
                            {
                                msg.AddAscii(AppUtil.ToAscii(sLOSS_CODE[6], 20));                                                  //A20 Loss Code 
                                msg.AddAscii(AppUtil.ToAscii(sLOSS_DESCRIPTION[6], 40));                                           //A40 Loss Code DESCRIPTION
                            }
                        }
                    }
                }

                m_lnkMachineInfo.Unit7_AvailabilityState = sAVAILABILITYSTATE[6];
                m_lnkMachineInfo.Unit7_InterlockState = sINTERLOCKSTATE[6];
                m_lnkMachineInfo.Unit7_MoveState = sMOVESTATE[6];
                m_lnkMachineInfo.Unit7_RunState = sRUNSTATE[6];
                m_lnkMachineInfo.Unit7_FrontState = sFRONTSTATE[6];
                m_lnkMachineInfo.Unit7_RearState = sREARSTATE[6];
                m_lnkMachineInfo.Unit7_PPSPLState = sPP_SPLSTATE[6];
                m_lnkMachineInfo.Unit7_ReasonCode = sREASONCODE[6];
                m_lnkMachineInfo.Unit7_Description = sDESCRIPTION[6];

                this.SecsDriver.Send(msg);
                #endregion
            }

            else if (aUNITID[7] == sUNITID)
            {
                #region S6F11(Equipment Loss Code Report), CEID:616

                SecsMessage msg = new SecsMessage(stream, function, true, Convert.ToInt32(SecsDriver.DeviceID));

                msg.AddList(3);                                                                                  //L3  EQP Status Event Info
                {
                    msg.AddAscii(AppUtil.ToAscii(sDataID, gDefine.DEF_DATAID_SIZE));                                //A4  Data ID
                    msg.AddAscii(AppUtil.ToAscii(sCEID, gDefine.DEF_CEID_SIZE));                                    //A3  Collection Event ID
                    msg.AddList(3);                                                                                 //L3  RPTID Set
                    {
                        msg.AddList(2);                                                                                 //L2  RPTID 100 Set 
                        {
                            msg.AddAscii(AppUtil.ToAscii("100", gDefine.DEF_RPTID_SIZE));                                   //A3  RPTID="100" 
                            msg.AddList(2);                                                                                 //L2  EQP Control State Set 
                            {
                                msg.AddAscii(AppUtil.ToAscii(sEQPID, gDefine.DEF_EQPID_SIZE));                                  //A40 HOST REQ EQPID 
                                msg.AddAscii(sCRST);                                                                            //A1  Online Control State 
                            }
                        }
                        msg.AddList(2);                                                                                 //L2  RPTID 101 Set
                        {
                            msg.AddAscii(AppUtil.ToAscii("102", gDefine.DEF_RPTID_SIZE));                                   //A3  RPTID="102"
                            msg.AddList(2);                                                                                 //L2  RPTID 101 Set
                            {
                                msg.AddAscii(AppUtil.ToAscii(sUNITID, gDefine.DEF_UNITID_SIZE));                                  //A40 HOST REQ UNITID 
                                msg.AddList(9);                                                                                 //L9  EQP State Set
                                {
                                    msg.AddAscii(sAVAILABILITYSTATE[7]);                                                               //A1  EQ Avilability State Info
                                    msg.AddAscii(sINTERLOCKSTATE[7]);                                                                  //A1  Interlock Avilability State Info
                                    msg.AddAscii(sMOVESTATE[7]);                                                                       //A1  EQ Move State Info
                                    msg.AddAscii(sRUNSTATE[7]);                                                                        //A1  Cell existence/nonexistence Check
                                    msg.AddAscii(sFRONTSTATE[7]);                                                                      //A1  Upper EQ Processing State
                                    msg.AddAscii(sREARSTATE[7]);                                                                       //A1  Lower EQ Processing State
                                    msg.AddAscii(sPP_SPLSTATE[7]);                                                                     //A1  Sample Run-Normal Run State
                                    msg.AddAscii(AppUtil.ToAscii(sREASONCODE[7], gDefine.DEF_REASONCODE_SIZE));                        //A20 Code Info
                                    msg.AddAscii(AppUtil.ToAscii(sDESCRIPTION[7], gDefine.DEF_DESCRIPTION_SIZE));                      //A40 EQ Description
                                }
                            }
                        }
                        msg.AddList(2);                                                                                 //L2  RPTID 805 Set
                        {
                            msg.AddAscii(AppUtil.ToAscii("806", gDefine.DEF_RPTID_SIZE));                                   //A3  RPTID="806"
                            msg.AddList(2);                                                                                 //L2  Loss Code List
                            {
                                msg.AddAscii(AppUtil.ToAscii(sLOSS_CODE[7], 20));                                                  //A20 Loss Code 
                                msg.AddAscii(AppUtil.ToAscii(sLOSS_DESCRIPTION[7], 40));                                           //A40 Loss Code DESCRIPTION
                            }
                        }
                    }
                }

                m_lnkMachineInfo.Unit8_AvailabilityState = sAVAILABILITYSTATE[7];
                m_lnkMachineInfo.Unit8_InterlockState = sINTERLOCKSTATE[7];
                m_lnkMachineInfo.Unit8_MoveState = sMOVESTATE[7];
                m_lnkMachineInfo.Unit8_RunState = sRUNSTATE[7];
                m_lnkMachineInfo.Unit8_FrontState = sFRONTSTATE[7];
                m_lnkMachineInfo.Unit8_RearState = sREARSTATE[7];
                m_lnkMachineInfo.Unit8_PPSPLState = sPP_SPLSTATE[7];
                m_lnkMachineInfo.Unit8_ReasonCode = sREASONCODE[7];
                m_lnkMachineInfo.Unit8_Description = sDESCRIPTION[7];

                this.SecsDriver.Send(msg);
                #endregion
            }
        }

    }
}
