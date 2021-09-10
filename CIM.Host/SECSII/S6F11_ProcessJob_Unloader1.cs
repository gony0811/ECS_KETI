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
    public class S6F11_ProcessJob_Unloader1 : SFMessage
    {
        /*S6F11(Process Job), CEID:301, 302, 303, 304, 305, 306*/
        public S6F11_ProcessJob_Unloader1(SECSDriver driver)
            : base(driver)
        {

        }
        public override void DoWork(string driverName, object obj)
        {
            int stream = 6, function = 11;
            bool bResult;

            string sDataID = "0"; string sCRST = ((int)DataManager.Instance.GET_INT_DATA("vSys_HostData_Mode", out bResult)).ToString();
            string sEQPID = CommonData.Instance.EQP_SETTINGS.EQPID;
            string sCEID = DataManager.Instance.GET_INT_DATA("iPLC8_EtoC_ProcessJobEventCEID1", out bResult).ToString();
            string sAVAILABILITYSTATE = DataManager.Instance.GET_STRING_DATA("iPLC8_EtoC_EQPAvailability", out bResult);
            string sINTERLOCKSTATE = DataManager.Instance.GET_STRING_DATA("iPLC8_EtoC_EQPInterlock", out bResult);
            string sMOVESTATE = DataManager.Instance.GET_STRING_DATA("iPLC8_EtoC_EQPMove", out bResult);
            string sRUNSTATE = DataManager.Instance.GET_STRING_DATA("iPLC8_EtoC_EQPRun", out bResult);
            string sFRONTSTATE = DataManager.Instance.GET_STRING_DATA("iPLC8_EtoC_EQPFront", out bResult);
            string sREARSTATE = DataManager.Instance.GET_STRING_DATA("iPLC8_EtoC_EQPRear", out bResult);
            string sPP_SPLSTATE = DataManager.Instance.GET_STRING_DATA("iPLC8_EtoC_EQPState_PP", out bResult);
            string sREASONCODE = DataManager.Instance.GET_STRING_DATA("iPLC8_EtoC_AvailabilityReasonCode", out bResult);
            string sDESCRIPTION = DataManager.Instance.GET_STRING_DATA("iPLC8_EtoC_AvailabilityDescription", out bResult);

            string sPORT_NO1 = DataManager.Instance.GET_STRING_DATA("iPLC8_EtoC_PortNo1", out bResult);
            string sPORT_AVAILABLE_STATE1 = DataManager.Instance.GET_STRING_DATA("iPLC8_EtoC_PortAvailstate1", out bResult);
            string sPORT_ACCESS_MODE1 = DataManager.Instance.GET_STRING_DATA("iPLC8_EtoC_PortAccessMode1", out bResult);
            string sPORT_TRANSFER_STATE1 = DataManager.Instance.GET_STRING_DATA("iPLC8_EtoC_PortTransferState1", out bResult);
            string sPORT_PROCESSING_STATE1 = DataManager.Instance.GET_STRING_DATA("iPLC8_EtoC_PortProcessingState1", out bResult);

            string sPARENT_LOT = DataManager.Instance.GET_STRING_DATA("iPLC8_EtoC_ProcessJobEvent1", out bResult);
            string sRFID = DataManager.Instance.GET_STRING_DATA("iPLC8_EtoC_ProcessJobEventRFID1", out bResult);
            string sPORT_NO_1 = DataManager.Instance.GET_INT_DATA("iPLC8_EtoC_ProcessJobEventPortNo1", out bResult).ToString();
            string sPLAN_QTY = DataManager.Instance.GET_INT_DATA("iPLC8_EtoC_ProcessJobEventPlanQTY1", out bResult).ToString();
            string sPROCESSED_QTY = DataManager.Instance.GET_INT_DATA("iPLC8_EtoC_ProcessJobEventDQTY1", out bResult).ToString();


            int nMAT_NO = 0;

            foreach (MODULE m in CommonData.Instance.MODULE_SETTINGS)
            {
                nMAT_NO += m.MATERIAL_PORT_COUNT;
            }

            string aMATERIALID = string.Empty;
            string aMATERIAL_TYPE = string.Empty;
            string aMATERIALST = string.Empty;
            string aMATERIALPORTID = string.Empty;
            string aMATERIALUSAGE = string.Empty;

            #region S6F11(Process Job), CEID:301, 302, 303, 304, 305, 306

            SecsMessage msg = new SecsMessage(stream, function, true, Convert.ToInt32(SecsDriver.DeviceID));

            msg.AddList(3);                                                                                         //L3  EQP Status Event Info
            {
                msg.AddAscii(AppUtil.ToAscii(sDataID, gDefine.DEF_DATAID_SIZE));                                        //A4  Data ID
                msg.AddAscii(AppUtil.ToAscii(sCEID, gDefine.DEF_CEID_SIZE));                                            //A3  Collection Event ID
                msg.AddList(5);                                                                                         //L5  RPTID Set
                {
                    msg.AddList(2);                                                                                         //L2  RPTID 100 Set 
                    {
                        msg.AddAscii(AppUtil.ToAscii("100", gDefine.DEF_RPTID_SIZE));                                           //A3  RPTID="100"
                        msg.AddList(2);                                                                                         //L2  EQP Control State Set
                        {
                            msg.AddAscii(AppUtil.ToAscii(sEQPID, gDefine.DEF_EQPID_SIZE));                                          //A40 HOST REQ EQPID 
                            msg.AddAscii(sCRST);                                                                                    //A1  Online Control State 
                        }
                    }
                    msg.AddList(2);                                                                                         //L2  RPTID 101 Set 
                    {
                        msg.AddAscii(AppUtil.ToAscii("101", gDefine.DEF_RPTID_SIZE));                                           //A3  RPTID="101"
                        msg.AddList(9);                                                                                         //L9  EQP State Set
                        {
                            msg.AddAscii(sAVAILABILITYSTATE);                                                                       //A1  EQ Avilability State Info
                            msg.AddAscii(sINTERLOCKSTATE);                                                                          //A1  Interlock Avilability State Info
                            msg.AddAscii(sMOVESTATE);                                                                               //A1  EQ Move State Info
                            msg.AddAscii(sRUNSTATE);                                                                                //A1  Cell existence/nonexistence Check
                            msg.AddAscii(sFRONTSTATE);                                                                              //A1  Upper EQ Processing State
                            msg.AddAscii(sREARSTATE);                                                                               //A1  Lower EQ Processing State
                            msg.AddAscii(sPP_SPLSTATE);                                                                             //A1  Sample Run-Normal Run State
                            msg.AddAscii(AppUtil.ToAscii(sREASONCODE, gDefine.DEF_REASONCODE_SIZE));                                //A20 Code Info
                            msg.AddAscii(AppUtil.ToAscii(sDESCRIPTION, gDefine.DEF_DESCRIPTION_SIZE));                              //A40 EQ Description
                        }
                    }
                    msg.AddList(2);                                                                                         //L2  RPTID 200 Set
                    {
                        msg.AddAscii(AppUtil.ToAscii("200", gDefine.DEF_RPTID_SIZE));                                           //A3 RPTID="200"
                        msg.AddList(nMAT_NO);                                                                                   //Ln Material 개수
                        {
                            if (nMAT_NO != 0)
                            {
                                foreach (MODULE m in CommonData.Instance.MODULE_SETTINGS)
                                {
                                    for (int i = 1; i <= m.MATERIAL_PORT_COUNT; i++)
                                    {
                                        aMATERIALID = DataManager.Instance.GET_STRING_DATA(string.Format("i{0}_EtoC_MaterialPortStsID{1}", m.MODULE_NAME, i), out bResult);
                                        aMATERIAL_TYPE = DataManager.Instance.GET_STRING_DATA(string.Format("i{0}_EtoC_MaterialPortStsType{1}", m.MODULE_NAME, i), out bResult);
                                        aMATERIALST = DataManager.Instance.GET_STRING_DATA(string.Format("i{0}_EtoC_MaterialPortStsLST{1}", m.MODULE_NAME, i), out bResult);
                                        aMATERIALPORTID = DataManager.Instance.GET_STRING_DATA(string.Format("i{0}_EtoC_MaterialPortStsLoaderNo{1}", m.MODULE_NAME, i), out bResult);
                                        aMATERIALUSAGE = DataManager.Instance.GET_STRING_DATA(string.Format("i{0}_EtoC_MaterialPortStsUsage{1}", m.MODULE_NAME, i), out bResult);
                                    }

                                    msg.AddList(5);                                                                                 //L5  Material Set
                                    {
                                        msg.AddAscii(AppUtil.ToAscii(aMATERIALID, gDefine.DEF_MATERIALID_SIZE));                        //A40 Material ID
                                        msg.AddAscii(AppUtil.ToAscii(aMATERIAL_TYPE, gDefine.DEF_MATERIALTYPE_SIZE));                   //A20 Material Type
                                        msg.AddAscii(aMATERIALST);                                                                      //A1  Material State Info
                                        msg.AddAscii(aMATERIALPORTID);                                                                  //A1  Material Port Info
                                        msg.AddAscii(AppUtil.ToAscii(aMATERIALUSAGE, 20));                                              //A20 Remaining Material Quantity Info
                                    }
                                }
                            }
                        }
                    }
                    msg.AddList(2);                                                                                         //L2  RPTID 305 Set 
                    {
                        msg.AddAscii(AppUtil.ToAscii("305", gDefine.DEF_RPTID_SIZE));                                           //A3  RPTID   
                        msg.AddList(5);                                                                                         //L5  Port Set
                        {
                            msg.AddAscii(sPORT_NO1);                                                                                //A1  Port No
                            msg.AddAscii(sPORT_AVAILABLE_STATE1);                                                                   //A1  Port Avilability State Info
                            msg.AddAscii(sPORT_ACCESS_MODE1);                                                                       //A1  Port Access State Info
                            msg.AddAscii(sPORT_TRANSFER_STATE1);                                                                    //A1  Port Transfer State Info
                            msg.AddAscii(sPORT_PROCESSING_STATE1);                                                                  //A1  Port Processing State Info
                        }
                    }
                    msg.AddList(2);                                                                                         //L2  RPTID 306 Set 
                    {
                        msg.AddAscii(AppUtil.ToAscii("306", gDefine.DEF_RPTID_SIZE));                                           //A3  RPTID   
                        msg.AddList(5);                                                                                         //L5  Process Job Set
                        {
                            msg.AddAscii(AppUtil.ToAscii(sPARENT_LOT, gDefine.DEF_PARENT_LOT_SIZE));                                //A40 Batch ID 
                            msg.AddAscii(AppUtil.ToAscii(sRFID, gDefine.DEF_RFID_SIZE));                                            //A40 RF Tag ID
                            msg.AddAscii(sPORT_NO_1);                                                                               //A1  Port#1 Position Info
                            msg.AddAscii(AppUtil.ToAscii(sPLAN_QTY, gDefine.DEF_PLAN_QTY_SIZE));                                    //A10 Plan Quantity Info
                            msg.AddAscii(AppUtil.ToAscii(sPROCESSED_QTY, gDefine.DEF_PROCESSED_QTY_SIZE));                          //A10 Processed Quantity Info
                        }
                    }
                }
            }

            this.SecsDriver.Send(msg);
            #endregion
        }
    }
    
}
