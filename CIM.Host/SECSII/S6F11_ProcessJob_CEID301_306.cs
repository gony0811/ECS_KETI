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
    class S6F11_ProcessJob_CEID301_306 : SFMessage
    {
        /*S6F11(ProcessJob), CEID :301, 302, 303, 304, 305, 306*/

        private string _processPortNo;
        private string _portName;

        public S6F11_ProcessJob_CEID301_306(SECSDriverBase driver, string ProcessPortNo, string PortName)
            : base(driver)
        {
            Stream = 6; Function = 11;
            _processPortNo = ProcessPortNo; _portName = PortName;
        }

        public override void DoWork(string driverName, object obj)
        {
            LogHelper.Instance.BizLog.DebugFormat("[{0}]DoWork : {1}, {2}", this.GetType().Name, driverName, obj);
            //Loader, Unloader에서만 발생
            //MODULE module = CommonData.Instance.MODULE_SETTINGS.Where(m => m.TYPE == eModuleType.LOADER).FirstOrDefault();
            Data data = obj as Data;
            bool bResult;
            INNO6.IO.Interface.eDevMode connectStatus = DataManager.Instance.IsDeviceMode("DEV1");

            //1. Parsing Data 
            string sDATAID = "0";
            string ceidTagName = string.Format("i{0}.ProcessJobEvent{1}.CEID", data.Module, _processPortNo);
            string sCEID = DataManager.Instance.GET_INT_DATA(ceidTagName, out bResult).ToString();

            //RTPID "100"
            string sCRST = ((int)CommonData.Instance.HOST_MODE).ToString();
            string sEQPID = CommonData.Instance.EQP_SETTINGS.EQPID;

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

            //RTPID "200"
            int nMaterial_List = CommonData.Instance.EQP_SETTINGS.MATERIALPORTCOUNT;//MaterialPort와 Material수량은 구분 되어야 함
            string sMATERIALID = "";
            string sMATARIALTYPE = "";
            string sMATERIALST = "";
            string sMATERIALPORTID = "";
            string sMATERIALUSAGE = "";

            //RTPID "305"
            string sEQPNAME = CommonData.Instance.EQP_SETTINGS.EQPNAME;
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

            //RTPID "306"
            string parentlotTagName = string.Format("i{0}.ProcessJobEvent{1}.ParentLot", data.Module, _processPortNo);
            string rfidTagName = string.Format("i{0}.ProcessJobEvent{1}.RFID", data.Module, _processPortNo);
            string portNo1TagName = string.Format("i{0}.ProcessJobEvent{1}.PortNo", data.Module, _processPortNo);
            string planQTYTagName = string.Format("i{0}.ProcessJobEvent{1}.PlanQTY", data.Module, _processPortNo);
            string processQTYTagName = string.Format("i{0}.ProcessJobEvent{1}.ProcessQTY", data.Module, _processPortNo);

            string sPARENT_LOT = DataManager.Instance.GET_STRING_DATA(parentlotTagName, out bResult);
            string s_RFID = DataManager.Instance.GET_STRING_DATA(rfidTagName, out bResult);
            string s_PORTNO1 = DataManager.Instance.GET_STRING_DATA(portNo1TagName, out bResult);
            string sPLAN_QTY = DataManager.Instance.GET_INT_DATA(planQTYTagName, out bResult).ToString();
            string sPROCESS_DQTY = DataManager.Instance.GET_INT_DATA(processQTYTagName, out bResult).ToString();

            #region S6F11(Process Job), CEID:301, 302, 303, 304, 305, 306

            SecsMessage msg = new SecsMessage(Stream, Function, true, Convert.ToInt32(SecsDriver.DeviceID));

            msg.AddList(3);                                                                                         //L3  EQP Status Event Info
            {
                msg.AddAscii(AppUtil.ToAscii(sDATAID, gDefine.DEF_DATAID_SIZE));                                        //A4  Data ID
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
                        msg.AddList(nMaterial_List);                                                                                   //Ln Material 개수
                        {
                            if (nMaterial_List != 0)
                            {
                                foreach (MODULE m in CommonData.Instance.MODULE_SETTINGS)
                                {
                                for (int i = 1; i <= /*nMaterial_List*/m.MATERIAL_PORT_COUNT; i++)
                                    {
                                        sMATERIALID = DataManager.Instance.GET_STRING_DATA(string.Format("i{0}.MaterialPortState.MaterialID{1}", data.Module, i), out bResult);
                                        sMATARIALTYPE = DataManager.Instance.GET_STRING_DATA(string.Format("i{0}.MaterialPortState.MaterialType{1}", data.Module, i), out bResult);
                                        sMATERIALST = DataManager.Instance.GET_STRING_DATA(string.Format("i{0}.MaterialPortState.MaterialST{1}", data.Module, i), out bResult);
                                        sMATERIALPORTID = DataManager.Instance.GET_INT_DATA(string.Format("i{0}.MaterialPortState.MaterialMLN{1}", data.Module, i), out bResult).ToString();
                                        sMATERIALUSAGE = DataManager.Instance.GET_INT_DATA(string.Format("i{0}.MaterialPortState.MaterialUsage{1}", data.Module, i), out bResult).ToString();

                                        msg.AddList(5);                                                                                 //L5  Material Set
                                        {
                                            msg.AddAscii(AppUtil.ToAscii(sMATERIALID, gDefine.DEF_MATERIALID_SIZE));                        //A40 Material ID
                                            msg.AddAscii(AppUtil.ToAscii(sMATARIALTYPE, gDefine.DEF_MATERIALTYPE_SIZE));                   //A20 Material Type
                                            msg.AddAscii(sMATERIALST);                                                                      //A1  Material State Info
                                            msg.AddAscii(sMATERIALPORTID);                                                                  //A1  Material Port Info
                                            msg.AddAscii(AppUtil.ToAscii(sMATERIALUSAGE, 20));                                              //A20 Remaining Material Quantity Info
                                        }
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
                            msg.AddAscii(sPORT_NO);                                                                                //A1  Port No
                            msg.AddAscii(sPORT_AVAILABLE_STATE);                                                                   //A1  Port Avilability State Info
                            msg.AddAscii(sPORT_ACCESS_MODE);                                                                       //A1  Port Access State Info
                            msg.AddAscii(sPORT_TRANSFER_STATE);                                                                    //A1  Port Transfer State Info
                            msg.AddAscii(sPORT_PROCESSING_STATE);                                                                  //A1  Port Processing State Info
                        }
                    }
                    msg.AddList(2);                                                                                         //L2  RPTID 306 Set 
                    {
                        msg.AddAscii(AppUtil.ToAscii("306", gDefine.DEF_RPTID_SIZE));                                           //A3  RPTID   
                        msg.AddList(5);                                                                                         //L5  Process Job Set
                        {
                            msg.AddAscii(AppUtil.ToAscii(sPARENT_LOT, gDefine.DEF_PARENT_LOT_SIZE));                                //A40 Batch ID 
                            msg.AddAscii(AppUtil.ToAscii(s_RFID, gDefine.DEF_RFID_SIZE));                                            //A40 RF Tag ID
                            msg.AddAscii(s_PORTNO1);                                                                               //A1  Port#1 Position Info
                            msg.AddAscii(AppUtil.ToAscii(sPLAN_QTY, gDefine.DEF_PLAN_QTY_SIZE));                                    //A10 Plan Quantity Info
                            msg.AddAscii(AppUtil.ToAscii(sPROCESS_DQTY, gDefine.DEF_PROCESSED_QTY_SIZE));                          //A10 Processed Quantity Info
                        }
                    }
                }
            }
            this.SecsDriver.WriteLogAndSendMessage(msg, sCEID);

            CommonData.Instance.OnStreamFunctionAdd("MAIN", "E->H", "CARRIER", "S6F11", sCEID, "Process Job", null);
            #endregion
        }
    }
}
