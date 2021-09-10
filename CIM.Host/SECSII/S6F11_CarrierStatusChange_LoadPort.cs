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
    public class S6F11_CarrierStatusChange_LoadPort : SFMessage
    {
        /*S6F11(Carrier Status Change), CEID:250, 251, 252, 253*/
        // 사양이 정확히 정해지면 Loader/Unloader로 나누어서 메세지 작성해야 함.

        public S6F11_CarrierStatusChange_LoadPort(SECSDriver driver)
            : base(driver)
        {

        }
        public override void DoWork(string driverName, object obj)
        {

            short stream = 6, function = 11;
            bool bResult;
            string sEQPID = CommonData.Instance.EQP_SETTINGS.EQPID;
            string sCEID = DataManager.Instance.GET_INT_DATA("iPLC7_EtoC_CarrierLoadReqEventType", out bResult).ToString();
            string sDataID = "0"; string sCRST = ((int)DataManager.Instance.GET_INT_DATA("vSys_HostData_Mode", out bResult)).ToString();
            string sAVAILABILITYSTATE = DataManager.Instance.GET_STRING_DATA("iPLC7_EtoC_EQPAvailability", out bResult);
            string sINTERLOCKSTATE = DataManager.Instance.GET_STRING_DATA("iPLC7_EtoC_EQPInterlock", out bResult);
            string sMOVESTATE = DataManager.Instance.GET_STRING_DATA("iPLC7_EtoC_EQPMove", out bResult);
            string sRUNSTATE = DataManager.Instance.GET_STRING_DATA("iPLC7_EtoC_EQPRun", out bResult);
            string sFRONTSTATE = DataManager.Instance.GET_STRING_DATA("iPLC7_EtoC_EQPFront", out bResult);
            string sREARSTATE = DataManager.Instance.GET_STRING_DATA("iPLC7_EtoC_EQPRear", out bResult);
            string sPP_SPLSTATE = DataManager.Instance.GET_STRING_DATA("iPLC7_EtoC_EQPState_PP", out bResult);
            string sREASONCODE = DataManager.Instance.GET_STRING_DATA("iPLC7_EtoC_AvailabilityReasonCode", out bResult);
            string sDESCRIPTION = DataManager.Instance.GET_STRING_DATA("iPLC7_EtoC_AvailabilityDescription", out bResult);

            string sPORT_NO1 = DataManager.Instance.GET_STRING_DATA("iPLC7_EtoC_PortNo1", out bResult);
            string sPORT_AVAILABLE_STATE1 = DataManager.Instance.GET_STRING_DATA("iPLC7_EtoC_PortAvailstate1", out bResult);
            string sPORT_ACCESS_MODE1 = DataManager.Instance.GET_STRING_DATA("iPLC7_EtoC_PortAccessMode1", out bResult);
            string sPORT_TRANSFER_STATE1 = DataManager.Instance.GET_STRING_DATA("iPLC7_EtoC_PortTransferState1", out bResult);
            string sPORT_PROCESSING_STATE1 = DataManager.Instance.GET_STRING_DATA("iPLC7_EtoC_PortProcessingState1", out bResult);
            string sPARENT_LOT = DataManager.Instance.GET_STRING_DATA("iPLC7_EtoC_CarrierLoadReqParentLot", out bResult);
            string sRFID = DataManager.Instance.GET_STRING_DATA("iPLC7_EtoC_CarrierLoadReqRFID", out bResult);
            string sPORTNO_1 = DataManager.Instance.GET_STRING_DATA("iPLC7_EtoC_CarrierLoadReqPortNo", out bResult);
            string sPPID = RMSManager.Instance.GetCurrentPPID();

            #region S6F11(CarrierStatusChange), CEID:250, 251, 252, 253
            SecsMessage msg = new SecsMessage(stream, function, true, Convert.ToInt32(SecsDriver.DeviceID));

            msg.AddList(3);                                                                             //L3  EQP Status Event Info
            {
                msg.AddAscii(AppUtil.ToAscii(sDataID, gDefine.DEF_DATAID_SIZE));                            //A4  Data ID
                msg.AddAscii(AppUtil.ToAscii(sCEID, gDefine.DEF_CEID_SIZE));                                //A3  Collection Event ID
                msg.AddList(4);                                                                             //L4  RPTID Set
                {
                    msg.AddList(2);                                                                             //L2  RPTID 100 Set
                    {
                        msg.AddAscii(AppUtil.ToAscii("100", gDefine.DEF_RPTID_SIZE));                               //A3  RPTID="100"
                        msg.AddList(2);                                                                             //L2  EQP Control State Set     
                        {
                            msg.AddAscii(AppUtil.ToAscii(sEQPID, gDefine.DEF_EQPID_SIZE));                              //A40 HOST REQ EQPID 
                            msg.AddAscii(sCRST);                                                                        //A1  Online Control State
                        }
                    }
                    msg.AddList(2);                                                                             //L2  RPTID 101 Set
                    {
                        msg.AddAscii(AppUtil.ToAscii("101", gDefine.DEF_RPTID_SIZE));                               //A3  RPTID="101"
                        msg.AddList(9);                                                                             //L9  EQP State Set   
                        {
                            msg.AddAscii(sAVAILABILITYSTATE);                                                           //A1  EQ Avilability State Info
                            msg.AddAscii(sINTERLOCKSTATE);                                                              //A1  Interlock Avilability State Info
                            msg.AddAscii(sMOVESTATE);                                                                   //A1  EQ Move State Info
                            msg.AddAscii(sRUNSTATE);                                                                    //A1  Cell existence/nonexistence Check
                            msg.AddAscii(sFRONTSTATE);                                                                  //A1  Upper EQ Processing State
                            msg.AddAscii(sREARSTATE);                                                                   //A1  Lower EQ Processing State
                            msg.AddAscii(sPP_SPLSTATE);                                                                 //A1  Sample Run-Normal Run State
                            msg.AddAscii(AppUtil.ToAscii(sREASONCODE, gDefine.DEF_REASONCODE_SIZE));                    //A20 Code Info
                            msg.AddAscii(AppUtil.ToAscii(sDESCRIPTION, gDefine.DEF_DESCRIPTION_SIZE));                  //A40 EQ Description
                        }
                    }
                    msg.AddList(2);                                                                             //L2  RPTID 300 Set
                    {
                        msg.AddAscii(AppUtil.ToAscii("305", gDefine.DEF_RPTID_SIZE));                               //A3  RPTID="300"
                        msg.AddList(5);                                                                             //L5  Port Set
                        {
                            msg.AddAscii(sPORT_NO1);                                                                    //A1  Port No
                            msg.AddAscii(sPORT_AVAILABLE_STATE1);                                                       //A1  Port Avilability State Info
                            msg.AddAscii(sPORT_ACCESS_MODE1);                                                           //A1  Port Access State Info
                            msg.AddAscii(sPORT_TRANSFER_STATE1);                                                        //A1  Port Transfer State Info
                            msg.AddAscii(sPORT_PROCESSING_STATE1);                                                      //A1  Port Processing State Info
                        }
                    }
                    msg.AddList(2);                                                                             //L2  RPTID 250 Set
                    {
                        msg.AddAscii(AppUtil.ToAscii("250", gDefine.DEF_RPTID_SIZE));                               //A3 RPTID="250"
                        msg.AddList(4);                                                                             //L4  Batch Set
                        {
                            msg.AddAscii(AppUtil.ToAscii(sPARENT_LOT, gDefine.DEF_PARENT_LOT_SIZE));                    //A40 Batch ID
                            msg.AddAscii(AppUtil.ToAscii(sRFID, gDefine.DEF_RFID_SIZE));                                //A40 RF Tag ID
                            msg.AddAscii(sPORTNO_1);                                                                    //A1  Port#1 Position Info
                            msg.AddAscii(AppUtil.ToAscii(sPPID, gDefine.DEF_PPID_SIZE));                                //A40 PPID
                        }
                    }
                }
            }

            this.SecsDriver.Send(msg);
            #endregion
        }


    }
}
