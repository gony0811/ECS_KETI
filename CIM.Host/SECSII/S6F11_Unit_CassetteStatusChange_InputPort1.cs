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
    public class S6F11_Unit_CassetteStatusChange_InputPort1 : SFMessage
    {
         /*S6F11(Cassette Status Change), CEID: 390, 391, 392, 393, 394 */
        // CEID : 390, 391
        public S6F11_Unit_CassetteStatusChange_InputPort1(SECSDriver driver)
            : base(driver)
        {

        }
        public override void DoWork(string driverName, object obj)
        {

            #region Varialbe
            int stream = 6, function = 11;
            bool bResult;

            int nUnit_List = DataManager.Instance.GET_INT_DATA("vSys_UnitInfos_Count", out bResult); // m_lnkHostData.UnitNo;
            string[] aUNITID = new string[nUnit_List];
            string sEQPID = CommonData.Instance.EQP_SETTINGS.EQPID;
            string sCEID = DataManager.Instance.GET_INT_DATA("iPLC1_EtoC_Cst_Load_CEID1", out bResult).ToString();
            string sUNITID = string.Empty; // 어디서 가져올지 정해지면 코딩

            int nUnit1MaterialList = DataManager.Instance.GET_INT_DATA("vSys_PLC1_MaterialCount", out bResult);
            int nUnit2MaterialList = DataManager.Instance.GET_INT_DATA("vSys_PLC2_MaterialCount", out bResult);
            int nUnit3MaterialList = DataManager.Instance.GET_INT_DATA("vSys_PLC3_MaterialCount", out bResult);
            int nUnit4MaterialList = DataManager.Instance.GET_INT_DATA("vSys_PLC4_MaterialCount", out bResult);
            int nUnit5MaterialList = DataManager.Instance.GET_INT_DATA("vSys_PLC5_MaterialCount", out bResult);
            int nUnit6MaterialList = DataManager.Instance.GET_INT_DATA("vSys_PLC6_MaterialCount", out bResult);
            int nUnit7MaterialList = DataManager.Instance.GET_INT_DATA("vSys_PLC7_MaterialCount", out bResult);
            int nUnit8MaterialList = DataManager.Instance.GET_INT_DATA("vSys_PLC8_MaterialCount", out bResult);

            string sDataID = "0"; string sCRST = ((int)DataManager.Instance.GET_INT_DATA("vSys_HostData_Mode", out bResult)).ToString();

            string Unit1_sPORT_ID = DataManager.Instance.GET_STRING_DATA("iPLC1_EtoC_Cst_Load_PortID1", out bResult);
            string Unit1_sPORT_AVAILABLE_STATE = DataManager.Instance.GET_STRING_DATA("iPLC1_EtoC_Cst_Load_PortAvailstate1", out bResult);
            string Unit1_sPORT_ACCESS_MODE = DataManager.Instance.GET_STRING_DATA("iPLC1_EtoC_Cst_Load_PortAccessMode1", out bResult);
            string Unit1_sPORT_TRANSFER_STATE = DataManager.Instance.GET_STRING_DATA("iPLC1_EtoC_Cst_Load_PortTransferState1", out bResult); ;
            string Unit1_sPORT_PROCESSING_STATE = DataManager.Instance.GET_STRING_DATA("iPLC1_EtoC_Cst_Load_PortProcessingState1", out bResult);
            string Unit1_sTRSID = DataManager.Instance.GET_STRING_DATA("iPLC1_EtoC_Cst_Load_TRSID1", out bResult);
            string Unit1_sOBJECTTYPE = DataManager.Instance.GET_STRING_DATA("iPLC1_EtoC_Cst_Load_ObjectType1", out bResult);
            string Unit1_sPRODUCTID = DataManager.Instance.GET_STRING_DATA("iPLC1_EtoC_Cst_Load_ProductID1", out bResult);
            string Unit1_sTRAYTYPE = DataManager.Instance.GET_STRING_DATA("iPLC1_EtoC_Cst_Load_TrayType1", out bResult);

            string Unit2_sPORT_ID = DataManager.Instance.GET_STRING_DATA("iPLC2_EtoC_Cst_Load_PortID1", out bResult);
            string Unit2_sPORT_AVAILABLE_STATE = DataManager.Instance.GET_STRING_DATA("iPLC2_EtoC_Cst_Load_PortAvailstate1", out bResult);
            string Unit2_sPORT_ACCESS_MODE = DataManager.Instance.GET_STRING_DATA("iPLC2_EtoC_Cst_Load_PortAccessMode1", out bResult);
            string Unit2_sPORT_TRANSFER_STATE = DataManager.Instance.GET_STRING_DATA("iPLC2_EtoC_Cst_Load_PortTransferState1", out bResult); ;
            string Unit2_sPORT_PROCESSING_STATE = DataManager.Instance.GET_STRING_DATA("iPLC2_EtoC_Cst_Load_PortProcessingState1", out bResult);
            string Unit2_sTRSID = DataManager.Instance.GET_STRING_DATA("iPLC2_EtoC_Cst_Load_TRSID1", out bResult);
            string Unit2_sOBJECTTYPE = DataManager.Instance.GET_STRING_DATA("iPLC2_EtoC_Cst_Load_ObjectType1", out bResult);
            string Unit2_sPRODUCTID = DataManager.Instance.GET_STRING_DATA("iPLC2_EtoC_Cst_Load_ProductID1", out bResult);
            string Unit2_sTRAYTYPE = DataManager.Instance.GET_STRING_DATA("iPLC2_EtoC_Cst_Load_TrayType1", out bResult);

            string Unit3_sPORT_ID = DataManager.Instance.GET_STRING_DATA("iPLC3_EtoC_Cst_Load_PortID1", out bResult);
            string Unit3_sPORT_AVAILABLE_STATE = DataManager.Instance.GET_STRING_DATA("iPLC3_EtoC_Cst_Load_PortAvailstate1", out bResult);
            string Unit3_sPORT_ACCESS_MODE = DataManager.Instance.GET_STRING_DATA("iPLC3_EtoC_Cst_Load_PortAccessMode1", out bResult);
            string Unit3_sPORT_TRANSFER_STATE = DataManager.Instance.GET_STRING_DATA("iPLC3_EtoC_Cst_Load_PortTransferState1", out bResult); ;
            string Unit3_sPORT_PROCESSING_STATE = DataManager.Instance.GET_STRING_DATA("iPLC3_EtoC_Cst_Load_PortProcessingState1", out bResult);
            string Unit3_sTRSID = DataManager.Instance.GET_STRING_DATA("iPLC3_EtoC_Cst_Load_TRSID1", out bResult);
            string Unit3_sOBJECTTYPE = DataManager.Instance.GET_STRING_DATA("iPLC3_EtoC_Cst_Load_ObjectType1", out bResult);
            string Unit3_sPRODUCTID = DataManager.Instance.GET_STRING_DATA("iPLC3_EtoC_Cst_Load_ProductID1", out bResult);
            string Unit3_sTRAYTYPE = DataManager.Instance.GET_STRING_DATA("iPLC3_EtoC_Cst_Load_TrayType1", out bResult);

            string Unit4_sPORT_ID = DataManager.Instance.GET_STRING_DATA("iPLC4_EtoC_Cst_Load_PortID1", out bResult);
            string Unit4_sPORT_AVAILABLE_STATE = DataManager.Instance.GET_STRING_DATA("iPLC4_EtoC_Cst_Load_PortAvailstate1", out bResult);
            string Unit4_sPORT_ACCESS_MODE = DataManager.Instance.GET_STRING_DATA("iPLC4_EtoC_Cst_Load_PortAccessMode1", out bResult);
            string Unit4_sPORT_TRANSFER_STATE = DataManager.Instance.GET_STRING_DATA("iPLC4_EtoC_Cst_Load_PortTransferState1", out bResult); ;
            string Unit4_sPORT_PROCESSING_STATE = DataManager.Instance.GET_STRING_DATA("iPLC4_EtoC_Cst_Load_PortProcessingState1", out bResult);
            string Unit4_sTRSID = DataManager.Instance.GET_STRING_DATA("iPLC4_EtoC_Cst_Load_TRSID1", out bResult);
            string Unit4_sOBJECTTYPE = DataManager.Instance.GET_STRING_DATA("iPLC4_EtoC_Cst_Load_ObjectType1", out bResult);
            string Unit4_sPRODUCTID = DataManager.Instance.GET_STRING_DATA("iPLC4_EtoC_Cst_Load_ProductID1", out bResult);
            string Unit4_sTRAYTYPE = DataManager.Instance.GET_STRING_DATA("iPLC4_EtoC_Cst_Load_TrayType1", out bResult);

            string Unit5_sPORT_ID = DataManager.Instance.GET_STRING_DATA("iPLC5_EtoC_Cst_Load_PortID1", out bResult);
            string Unit5_sPORT_AVAILABLE_STATE = DataManager.Instance.GET_STRING_DATA("iPLC5_EtoC_Cst_Load_PortAvailstate1", out bResult);
            string Unit5_sPORT_ACCESS_MODE = DataManager.Instance.GET_STRING_DATA("iPLC5_EtoC_Cst_Load_PortAccessMode1", out bResult);
            string Unit5_sPORT_TRANSFER_STATE = DataManager.Instance.GET_STRING_DATA("iPLC5_EtoC_Cst_Load_PortTransferState1", out bResult); ;
            string Unit5_sPORT_PROCESSING_STATE = DataManager.Instance.GET_STRING_DATA("iPLC5_EtoC_Cst_Load_PortProcessingState1", out bResult);
            string Unit5_sTRSID = DataManager.Instance.GET_STRING_DATA("iPLC5_EtoC_Cst_Load_TRSID1", out bResult);
            string Unit5_sOBJECTTYPE = DataManager.Instance.GET_STRING_DATA("iPLC5_EtoC_Cst_Load_ObjectType1", out bResult);
            string Unit5_sPRODUCTID = DataManager.Instance.GET_STRING_DATA("iPLC5_EtoC_Cst_Load_ProductID1", out bResult);
            string Unit5_sTRAYTYPE = DataManager.Instance.GET_STRING_DATA("iPLC5_EtoC_Cst_Load_TrayType1", out bResult);

            string Unit6_sPORT_ID = DataManager.Instance.GET_STRING_DATA("iPLC6_EtoC_Cst_Load_PortID1", out bResult);
            string Unit6_sPORT_AVAILABLE_STATE = DataManager.Instance.GET_STRING_DATA("iPLC6_EtoC_Cst_Load_PortAvailstate1", out bResult);
            string Unit6_sPORT_ACCESS_MODE = DataManager.Instance.GET_STRING_DATA("iPLC6_EtoC_Cst_Load_PortAccessMode1", out bResult);
            string Unit6_sPORT_TRANSFER_STATE = DataManager.Instance.GET_STRING_DATA("iPLC6_EtoC_Cst_Load_PortTransferState1", out bResult); ;
            string Unit6_sPORT_PROCESSING_STATE = DataManager.Instance.GET_STRING_DATA("iPLC6_EtoC_Cst_Load_PortProcessingState1", out bResult);
            string Unit6_sTRSID = DataManager.Instance.GET_STRING_DATA("iPLC6_EtoC_Cst_Load_TRSID1", out bResult);
            string Unit6_sOBJECTTYPE = DataManager.Instance.GET_STRING_DATA("iPLC6_EtoC_Cst_Load_ObjectType1", out bResult);
            string Unit6_sPRODUCTID = DataManager.Instance.GET_STRING_DATA("iPLC6_EtoC_Cst_Load_ProductID1", out bResult);
            string Unit6_sTRAYTYPE = DataManager.Instance.GET_STRING_DATA("iPLC6_EtoC_Cst_Load_TrayType1", out bResult);

            string Unit7_sPORT_ID = DataManager.Instance.GET_STRING_DATA("iPLC7_EtoC_Cst_Load_PortID1", out bResult);
            string Unit7_sPORT_AVAILABLE_STATE = DataManager.Instance.GET_STRING_DATA("iPLC7_EtoC_Cst_Load_PortAvailstate1", out bResult);
            string Unit7_sPORT_ACCESS_MODE = DataManager.Instance.GET_STRING_DATA("iPLC7_EtoC_Cst_Load_PortAccessMode1", out bResult);
            string Unit7_sPORT_TRANSFER_STATE = DataManager.Instance.GET_STRING_DATA("iPLC7_EtoC_Cst_Load_PortTransferState1", out bResult); ;
            string Unit7_sPORT_PROCESSING_STATE = DataManager.Instance.GET_STRING_DATA("iPLC7_EtoC_Cst_Load_PortProcessingState1", out bResult);
            string Unit7_sTRSID = DataManager.Instance.GET_STRING_DATA("iPLC7_EtoC_Cst_Load_TRSID1", out bResult);
            string Unit7_sOBJECTTYPE = DataManager.Instance.GET_STRING_DATA("iPLC7_EtoC_Cst_Load_ObjectType1", out bResult);
            string Unit7_sPRODUCTID = DataManager.Instance.GET_STRING_DATA("iPLC7_EtoC_Cst_Load_ProductID1", out bResult);
            string Unit7_sTRAYTYPE = DataManager.Instance.GET_STRING_DATA("iPLC7_EtoC_Cst_Load_TrayType1", out bResult);

            string Unit8_sPORT_ID = DataManager.Instance.GET_STRING_DATA("iPLC8_EtoC_Cst_Load_PortID1", out bResult);
            string Unit8_sPORT_AVAILABLE_STATE = DataManager.Instance.GET_STRING_DATA("iPLC8_EtoC_Cst_Load_PortAvailstate1", out bResult);
            string Unit8_sPORT_ACCESS_MODE = DataManager.Instance.GET_STRING_DATA("iPLC8_EtoC_Cst_Load_PortAccessMode1", out bResult);
            string Unit8_sPORT_TRANSFER_STATE = DataManager.Instance.GET_STRING_DATA("iPLC8_EtoC_Cst_Load_PortTransferState1", out bResult); ;
            string Unit8_sPORT_PROCESSING_STATE = DataManager.Instance.GET_STRING_DATA("iPLC8_EtoC_Cst_Load_PortProcessingState1", out bResult);
            string Unit8_sTRSID = DataManager.Instance.GET_STRING_DATA("iPLC8_EtoC_Cst_Load_TRSID1", out bResult);
            string Unit8_sOBJECTTYPE = DataManager.Instance.GET_STRING_DATA("iPLC8_EtoC_Cst_Load_ObjectType1", out bResult);
            string Unit8_sPRODUCTID = DataManager.Instance.GET_STRING_DATA("iPLC8_EtoC_Cst_Load_ProductID1", out bResult);
            string Unit8_sTRAYTYPE = DataManager.Instance.GET_STRING_DATA("iPLC8_EtoC_Cst_Load_TrayType1", out bResult);
            #endregion

            if (aUNITID[0] == sUNITID)
            {
                #region S6F11(Cassette Status Change), CEID:350, 351

                SecsMessage msg = new SecsMessage(stream, function, true, Convert.ToInt32(SecsDriver.DeviceID));

                msg.AddList(3);                                                                                              //L3  Cassette Process Event Info
                {
                    msg.AddAscii(AppUtil.ToAscii(sDataID, gDefine.DEF_DATAID_SIZE));                                         //A4  Data ID
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
                            msg.AddList(2);                                                                                         //L2  RPTID 307 Set
                            {
                                msg.AddAscii(AppUtil.ToAscii(sUNITID, gDefine.DEF_UNITID_SIZE));                                           //A40 HOST REQ EQPID
                                msg.AddList(5);                                                                                         //L5  Port Set
                                {
                                    msg.AddAscii(AppUtil.ToAscii(Unit1_sPORT_ID, 4));                                                            //A4  Port No
                                    msg.AddAscii(Unit1_sPORT_AVAILABLE_STATE);                                                                   //A1  Port Avilability State Info
                                    msg.AddAscii(Unit1_sPORT_ACCESS_MODE);                                                                       //A1  Port Access State Info
                                    msg.AddAscii(Unit1_sPORT_TRANSFER_STATE);                                                                    //A1  Port Transfer State Info
                                    msg.AddAscii(Unit1_sPORT_PROCESSING_STATE);                                                                  //A1  Port Processing State Info
                                }
                            }
                        }
                        msg.AddList(2);                                                                                          //L2  RPTID 251 Set
                        {
                            msg.AddAscii(AppUtil.ToAscii("251", gDefine.DEF_RPTID_SIZE));                                           //A3  RPTID="251"   
                            msg.AddList(4);                                                                                         //L2  Port Set
                            {
                                msg.AddAscii(AppUtil.ToAscii(Unit1_sTRSID, 40));                                                             //A40 TRS ID
                                msg.AddAscii(AppUtil.ToAscii(Unit1_sOBJECTTYPE, 20));                                                           //A20 OBJECTTYPE
                                msg.AddAscii(AppUtil.ToAscii(Unit1_sPRODUCTID, 40));                                                             //A40 PRODUCTID
                                msg.AddAscii(AppUtil.ToAscii(Unit1_sTRAYTYPE, 20));                                                           //A20 TRAYTYPE
                            }
                        }
                    }
                }

                this.SecsDriver.Send(msg);
                #endregion
            }

            else if (aUNITID[1] == sUNITID)
            {
                #region S6F11(Cassette Status Change), CEID:350, 351

                SecsMessage msg = new SecsMessage(stream, function, true, Convert.ToInt32(SecsDriver.DeviceID));

                msg.AddList(3);                                                                                              //L3  Cassette Process Event Info
                {
                    msg.AddAscii(AppUtil.ToAscii(sDataID, gDefine.DEF_DATAID_SIZE));                                         //A4  Data ID
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
                            msg.AddList(2);                                                                                         //L2  RPTID 307 Set
                            {
                                msg.AddAscii(AppUtil.ToAscii(sUNITID, gDefine.DEF_UNITID_SIZE));                                           //A40 HOST REQ EQPID
                                msg.AddList(5);                                                                                         //L5  Port Set
                                {
                                    msg.AddAscii(AppUtil.ToAscii(Unit2_sPORT_ID, 4));                                                            //A4  Port No
                                    msg.AddAscii(Unit2_sPORT_AVAILABLE_STATE);                                                                   //A1  Port Avilability State Info
                                    msg.AddAscii(Unit2_sPORT_ACCESS_MODE);                                                                       //A1  Port Access State Info
                                    msg.AddAscii(Unit2_sPORT_TRANSFER_STATE);                                                                    //A1  Port Transfer State Info
                                    msg.AddAscii(Unit2_sPORT_PROCESSING_STATE);                                                                  //A1  Port Processing State Info
                                }
                            }
                        }
                        msg.AddList(2);                                                                                          //L2  RPTID 251 Set
                        {
                            msg.AddAscii(AppUtil.ToAscii("251", gDefine.DEF_RPTID_SIZE));                                           //A3  RPTID="251"   
                            msg.AddList(2);                                                                                         //L2  Port Set
                            {
                                msg.AddAscii(AppUtil.ToAscii(Unit2_sTRSID, 40));                                                             //A40 TRS ID
                                msg.AddAscii(AppUtil.ToAscii(Unit2_sOBJECTTYPE, 20));                                                           //A20 OBJECTTYPE
                                msg.AddAscii(AppUtil.ToAscii(Unit2_sPRODUCTID, 40));                                                             //A40 PRODUCTID
                                msg.AddAscii(AppUtil.ToAscii(Unit2_sTRAYTYPE, 20));                                                           //A20 TRAYTYPE
                            }
                        }
                    }
                }

                this.SecsDriver.Send(msg);
                #endregion
            }

            else if (aUNITID[2] == sUNITID)
            {
                #region S6F11(Cassette Status Change), CEID:350, 351

                SecsMessage msg = new SecsMessage(stream, function, true, Convert.ToInt32(SecsDriver.DeviceID));

                msg.AddList(3);                                                                                              //L3  Cassette Process Event Info
                {
                    msg.AddAscii(AppUtil.ToAscii(sDataID, gDefine.DEF_DATAID_SIZE));                                         //A4  Data ID
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
                            msg.AddList(2);                                                                                         //L2  RPTID 307 Set
                            {
                                msg.AddAscii(AppUtil.ToAscii(sUNITID, gDefine.DEF_UNITID_SIZE));                                           //A40 HOST REQ EQPID
                                msg.AddList(5);                                                                                         //L5  Port Set
                                {
                                    msg.AddAscii(AppUtil.ToAscii(Unit3_sPORT_ID, 4));                                                            //A4  Port No
                                    msg.AddAscii(Unit3_sPORT_AVAILABLE_STATE);                                                                   //A1  Port Avilability State Info
                                    msg.AddAscii(Unit3_sPORT_ACCESS_MODE);                                                                       //A1  Port Access State Info
                                    msg.AddAscii(Unit3_sPORT_TRANSFER_STATE);                                                                    //A1  Port Transfer State Info
                                    msg.AddAscii(Unit3_sPORT_PROCESSING_STATE);                                                                  //A1  Port Processing State Info
                                }
                            }
                        }
                        msg.AddList(2);                                                                                          //L2  RPTID 251 Set
                        {
                            msg.AddAscii(AppUtil.ToAscii("251", gDefine.DEF_RPTID_SIZE));                                           //A3  RPTID="251"   
                            msg.AddList(2);                                                                                         //L2  Port Set
                            {
                                msg.AddAscii(AppUtil.ToAscii(Unit3_sTRSID, 40));                                                             //A40 TRS ID
                                msg.AddAscii(AppUtil.ToAscii(Unit3_sOBJECTTYPE, 20));                                                           //A20 OBJECTTYPE
                                msg.AddAscii(AppUtil.ToAscii(Unit3_sPRODUCTID, 40));                                                             //A40 PRODUCTID
                                msg.AddAscii(AppUtil.ToAscii(Unit3_sTRAYTYPE, 20));                                                           //A20 TRAYTYPE
                            }
                        }
                    }
                }

                this.SecsDriver.Send(msg);
                #endregion
            }

            else if (aUNITID[3] == sUNITID)
            {
                #region S6F11(Cassette Status Change), CEID:350, 351

                SecsMessage msg = new SecsMessage(stream, function, true, Convert.ToInt32(SecsDriver.DeviceID));

                msg.AddList(3);                                                                                              //L3  Cassette Process Event Info
                {
                    msg.AddAscii(AppUtil.ToAscii(sDataID, gDefine.DEF_DATAID_SIZE));                                         //A4  Data ID
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
                            msg.AddList(2);                                                                                         //L2  RPTID 307 Set
                            {
                                msg.AddAscii(AppUtil.ToAscii(sUNITID, gDefine.DEF_UNITID_SIZE));                                           //A40 HOST REQ EQPID
                                msg.AddList(5);                                                                                         //L5  Port Set
                                {
                                    msg.AddAscii(AppUtil.ToAscii(Unit4_sPORT_ID, 4));                                                            //A4  Port No
                                    msg.AddAscii(Unit4_sPORT_AVAILABLE_STATE);                                                                   //A1  Port Avilability State Info
                                    msg.AddAscii(Unit4_sPORT_ACCESS_MODE);                                                                       //A1  Port Access State Info
                                    msg.AddAscii(Unit4_sPORT_TRANSFER_STATE);                                                                    //A1  Port Transfer State Info
                                    msg.AddAscii(Unit4_sPORT_PROCESSING_STATE);                                                                  //A1  Port Processing State Info
                                }
                            }
                        }
                        msg.AddList(2);                                                                                          //L2  RPTID 251 Set
                        {
                            msg.AddAscii(AppUtil.ToAscii("251", gDefine.DEF_RPTID_SIZE));                                           //A3  RPTID="251"   
                            msg.AddList(2);                                                                                         //L2  Port Set
                            {
                                msg.AddAscii(AppUtil.ToAscii(Unit4_sTRSID, 40));                                                             //A40 TRS ID
                                msg.AddAscii(AppUtil.ToAscii(Unit4_sOBJECTTYPE, 20));                                                           //A20 OBJECTTYPE
                                msg.AddAscii(AppUtil.ToAscii(Unit4_sPRODUCTID, 40));                                                             //A40 PRODUCTID
                                msg.AddAscii(AppUtil.ToAscii(Unit4_sTRAYTYPE, 20));                                                           //A20 TRAYTYPE
                            }
                        }
                    }
                }

                this.SecsDriver.Send(msg);
                #endregion
            }

            else if (aUNITID[4] == sUNITID)
            {
                #region S6F11(Cassette Status Change), CEID:350, 351

                SecsMessage msg = new SecsMessage(stream, function, true, Convert.ToInt32(SecsDriver.DeviceID));

                msg.AddList(3);                                                                                              //L3  Cassette Process Event Info
                {
                    msg.AddAscii(AppUtil.ToAscii(sDataID, gDefine.DEF_DATAID_SIZE));                                         //A4  Data ID
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
                            msg.AddList(2);                                                                                         //L2  RPTID 307 Set
                            {
                                msg.AddAscii(AppUtil.ToAscii(sUNITID, gDefine.DEF_UNITID_SIZE));                                           //A40 HOST REQ EQPID
                                msg.AddList(5);                                                                                         //L5  Port Set
                                {
                                    msg.AddAscii(AppUtil.ToAscii(Unit5_sPORT_ID, 4));                                                            //A4  Port No
                                    msg.AddAscii(Unit5_sPORT_AVAILABLE_STATE);                                                                   //A1  Port Avilability State Info
                                    msg.AddAscii(Unit5_sPORT_ACCESS_MODE);                                                                       //A1  Port Access State Info
                                    msg.AddAscii(Unit5_sPORT_TRANSFER_STATE);                                                                    //A1  Port Transfer State Info
                                    msg.AddAscii(Unit5_sPORT_PROCESSING_STATE);                                                                  //A1  Port Processing State Info
                                }
                            }
                        }
                        msg.AddList(2);                                                                                          //L2  RPTID 251 Set
                        {
                            msg.AddAscii(AppUtil.ToAscii("251", gDefine.DEF_RPTID_SIZE));                                           //A3  RPTID="251"   
                            msg.AddList(2);                                                                                         //L2  Port Set
                            {
                                msg.AddAscii(AppUtil.ToAscii(Unit5_sTRSID, 40));                                                             //A40 TRS ID
                                msg.AddAscii(AppUtil.ToAscii(Unit5_sOBJECTTYPE, 20));                                                           //A20 OBJECTTYPE
                                msg.AddAscii(AppUtil.ToAscii(Unit5_sPRODUCTID, 40));                                                             //A40 PRODUCTID
                                msg.AddAscii(AppUtil.ToAscii(Unit5_sTRAYTYPE, 20));                                                           //A20 TRAYTYPE
                            }
                        }
                    }
                }

                this.SecsDriver.Send(msg);
                #endregion
            }

            else if (aUNITID[5] == sUNITID)
            {
                #region S6F11(Cassette Status Change), CEID:350, 351

                SecsMessage msg = new SecsMessage(stream, function, true, Convert.ToInt32(SecsDriver.DeviceID));

                msg.AddList(3);                                                                                              //L3  Cassette Process Event Info
                {
                    msg.AddAscii(AppUtil.ToAscii(sDataID, gDefine.DEF_DATAID_SIZE));                                         //A4  Data ID
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
                            msg.AddList(2);                                                                                         //L2  RPTID 307 Set
                            {
                                msg.AddAscii(AppUtil.ToAscii(sUNITID, gDefine.DEF_UNITID_SIZE));                                           //A40 HOST REQ EQPID
                                msg.AddList(5);                                                                                         //L5  Port Set
                                {
                                    msg.AddAscii(AppUtil.ToAscii(Unit6_sPORT_ID, 4));                                                            //A4  Port No
                                    msg.AddAscii(Unit6_sPORT_AVAILABLE_STATE);                                                                   //A1  Port Avilability State Info
                                    msg.AddAscii(Unit6_sPORT_ACCESS_MODE);                                                                       //A1  Port Access State Info
                                    msg.AddAscii(Unit6_sPORT_TRANSFER_STATE);                                                                    //A1  Port Transfer State Info
                                    msg.AddAscii(Unit6_sPORT_PROCESSING_STATE);                                                                  //A1  Port Processing State Info
                                }
                            }
                        }
                        msg.AddList(2);                                                                                          //L2  RPTID 251 Set
                        {
                            msg.AddAscii(AppUtil.ToAscii("251", gDefine.DEF_RPTID_SIZE));                                           //A3  RPTID="251"   
                            msg.AddList(2);                                                                                         //L2  Port Set
                            {
                                msg.AddAscii(AppUtil.ToAscii(Unit6_sTRSID, 40));                                                             //A40 TRS ID
                                msg.AddAscii(AppUtil.ToAscii(Unit6_sOBJECTTYPE, 20));                                                           //A20 OBJECTTYPE
                                msg.AddAscii(AppUtil.ToAscii(Unit6_sPRODUCTID, 40));                                                             //A40 PRODUCTID
                                msg.AddAscii(AppUtil.ToAscii(Unit6_sTRAYTYPE, 20));                                                           //A20 TRAYTYPE
                            }
                        }
                    }
                }

                this.SecsDriver.Send(msg);
                #endregion
            }

            else if (aUNITID[6] == sUNITID)
            {
                #region S6F11(Cassette Status Change), CEID:350, 351

                SecsMessage msg = new SecsMessage(stream, function, true, Convert.ToInt32(SecsDriver.DeviceID));

                msg.AddList(3);                                                                                              //L3  Cassette Process Event Info
                {
                    msg.AddAscii(AppUtil.ToAscii(sDataID, gDefine.DEF_DATAID_SIZE));                                         //A4  Data ID
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
                            msg.AddList(2);                                                                                         //L2  RPTID 307 Set
                            {
                                msg.AddAscii(AppUtil.ToAscii(sUNITID, gDefine.DEF_UNITID_SIZE));                                           //A40 HOST REQ EQPID
                                msg.AddList(5);                                                                                         //L5  Port Set
                                {
                                    msg.AddAscii(AppUtil.ToAscii(Unit7_sPORT_ID, 4));                                                            //A4  Port No
                                    msg.AddAscii(Unit7_sPORT_AVAILABLE_STATE);                                                                   //A1  Port Avilability State Info
                                    msg.AddAscii(Unit7_sPORT_ACCESS_MODE);                                                                       //A1  Port Access State Info
                                    msg.AddAscii(Unit7_sPORT_TRANSFER_STATE);                                                                    //A1  Port Transfer State Info
                                    msg.AddAscii(Unit7_sPORT_PROCESSING_STATE);                                                                  //A1  Port Processing State Info
                                }
                            }
                        }
                        msg.AddList(2);                                                                                          //L2  RPTID 251 Set
                        {
                            msg.AddAscii(AppUtil.ToAscii("251", gDefine.DEF_RPTID_SIZE));                                           //A3  RPTID="251"   
                            msg.AddList(2);                                                                                         //L2  Port Set
                            {
                                msg.AddAscii(AppUtil.ToAscii(Unit7_sTRSID, 40));                                                             //A40 TRS ID
                                msg.AddAscii(AppUtil.ToAscii(Unit7_sOBJECTTYPE, 20));                                                           //A20 OBJECTTYPE
                                msg.AddAscii(AppUtil.ToAscii(Unit7_sPRODUCTID, 40));                                                             //A40 PRODUCTID
                                msg.AddAscii(AppUtil.ToAscii(Unit7_sTRAYTYPE, 20));                                                           //A20 TRAYTYPE
                            }
                        }
                    }
                }

                this.SecsDriver.Send(msg);
                #endregion
            }

            else if (aUNITID[7] == sUNITID)
            {
                #region S6F11(Cassette Status Change), CEID:350, 351

                SecsMessage msg = new SecsMessage(stream, function, true, Convert.ToInt32(SecsDriver.DeviceID));

                msg.AddList(3);                                                                                              //L3  Cassette Process Event Info
                {
                    msg.AddAscii(AppUtil.ToAscii(sDataID, gDefine.DEF_DATAID_SIZE));                                         //A4  Data ID
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
                            msg.AddList(2);                                                                                         //L2  RPTID 307 Set
                            {
                                msg.AddAscii(AppUtil.ToAscii(sUNITID, gDefine.DEF_UNITID_SIZE));                                           //A40 HOST REQ EQPID
                                msg.AddList(5);                                                                                         //L5  Port Set
                                {
                                    msg.AddAscii(AppUtil.ToAscii(Unit8_sPORT_ID, 4));                                                            //A4  Port No
                                    msg.AddAscii(Unit8_sPORT_AVAILABLE_STATE);                                                                   //A1  Port Avilability State Info
                                    msg.AddAscii(Unit8_sPORT_ACCESS_MODE);                                                                       //A1  Port Access State Info
                                    msg.AddAscii(Unit8_sPORT_TRANSFER_STATE);                                                                    //A1  Port Transfer State Info
                                    msg.AddAscii(Unit8_sPORT_PROCESSING_STATE);                                                                  //A1  Port Processing State Info
                                }
                            }
                        }
                        msg.AddList(2);                                                                                          //L2  RPTID 251 Set
                        {
                            msg.AddAscii(AppUtil.ToAscii("251", gDefine.DEF_RPTID_SIZE));                                           //A3  RPTID="251"   
                            msg.AddList(2);                                                                                         //L2  Port Set
                            {
                                msg.AddAscii(AppUtil.ToAscii(Unit8_sTRSID, 40));                                                             //A40 TRS ID
                                msg.AddAscii(AppUtil.ToAscii(Unit8_sOBJECTTYPE, 20));                                                           //A20 OBJECTTYPE
                                msg.AddAscii(AppUtil.ToAscii(Unit8_sPRODUCTID, 40));                                                             //A40 PRODUCTID
                                msg.AddAscii(AppUtil.ToAscii(Unit8_sTRAYTYPE, 20));                                                           //A20 TRAYTYPE
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
