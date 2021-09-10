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
    class S6F11_InterlockConfirm_CEID502 : SFMessage
    {
        /*S6F11(InterlockConfirm), CEID :502*/

        public S6F11_InterlockConfirm_CEID502(SECSDriverBase driver)
            : base(driver)
        {
            Stream = 6; Function = 11;
        }

        public override void DoWork(string driverName, object obj)
        {
            LogHelper.Instance.BizLog.DebugFormat("[{0}]DoWork : {1}, {2}", this.GetType().Name, driverName, obj);
            bool bResult;
            INNO6.IO.Interface.eDevMode connectStatus = DataManager.Instance.IsDeviceMode("DEV1");

            //1. Parsing Data 
            string sDATAID = "0";
            string sCEID = "502";

            //RTPID "100"
            string sEQPID = CommonData.Instance.EQP_SETTINGS.EQPID;
            string sCRST = ((int)CommonData.Instance.HOST_MODE).ToString();
            MODULE module = CommonData.Instance.MODULE_SETTINGS.Where(m => m.TYPE == eModuleType.MASTER).FirstOrDefault();

            //RTPID "102"
            string availabilityTagName = string.Format("i{0}.EQStatus.Availability", module.MODULE_NAME);
            string interlockTagName = string.Format("i{0}.EQStatus.Interlock", module.MODULE_NAME);
            string moveStateTagName = string.Format("i{0}.EQStatus.Move", module.MODULE_NAME);
            string runStateTagName = string.Format("i{0}.EQStatus.Run", module.MODULE_NAME);
            string frontStateTagName = string.Format("i{0}.EQStatus.Front", module.MODULE_NAME);
            string rearStateTagName = string.Format("i{0}.EQStatus.Rear", module.MODULE_NAME);
            string PPStateTagName = string.Format("i{0}.EQStatus.PP_SPL", module.MODULE_NAME);
            string reasonCodeTagName = string.Format("i{0}.Availability.ReasonCode", module.MODULE_NAME);
            string descriptionTagName = string.Format("i{0}.Availability.Description", module.MODULE_NAME);

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
            string cellIDTagName = string.Format("i{0}.InterlockConfirm.CellID", module.MODULE_NAME);
            string productIDTagName = string.Format("i{0}.InterlockConfirm.ProductID", module.MODULE_NAME);
            string stepIDTagName = string.Format("i{0}.InterlockConfirm.StepID", module.MODULE_NAME);

            string sCELLID = DataManager.Instance.GET_STRING_DATA(cellIDTagName, out bResult);
            string sPPID = RMSManager.Instance.CurrentPPID;
            string sPRODUCTID = DataManager.Instance.GET_STRING_DATA(productIDTagName, out bResult);
            string sSTEPID = DataManager.Instance.GET_STRING_DATA(stepIDTagName, out bResult);

            short nINTERLOCK_NO = 1;

            string[] interlcokIDTagName = new string[nINTERLOCK_NO];
            string[] messageTagName = new string[nINTERLOCK_NO];
            for (int i = 0; i < nINTERLOCK_NO; i++)
            {
                interlcokIDTagName[i] = string.Format("i{0}.InterlockConfirm.InterlockIDID",module.MODULE_NAME);
                messageTagName[i] = string.Format("i{0}.InterlockConfirm.Message", module.MODULE_NAME);
            }

            string[] aINTERLOCK_ID = new string[nINTERLOCK_NO];
            string[] aMESSAGE = new string[nINTERLOCK_NO];
            for (int i = 0; i < nINTERLOCK_NO; i++)
            {
                aINTERLOCK_ID[i] = DataManager.Instance.GET_STRING_DATA(interlcokIDTagName[i], out bResult);
                aMESSAGE[i] = DataManager.Instance.GET_STRING_DATA(messageTagName[i], out bResult);
            }

            #region S6F11(Unit Interlock Confirm), CEID:514

            SecsMessage msg = new SecsMessage(Stream, Function, true, Convert.ToInt32(SecsDriver.DeviceID));

            msg.AddList(3);                                                                                     //L3  Opcall Event Info
            {
                msg.AddAscii(AppUtil.ToAscii(sDATAID, gDefine.DEF_DATAID_SIZE));                                    //A4  Data ID
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
                    msg.AddList(2);                                                                                     //L2  RPTID 700 Set
                    {
                        msg.AddAscii(AppUtil.ToAscii("701", gDefine.DEF_RPTID_SIZE));                                       //A3  RPTID="700"
                        msg.AddList(nINTERLOCK_NO);                                                                            //Ln  OPCALL
                        {
                            for (int i = 0; i < nINTERLOCK_NO; i++)
                            {
                                msg.AddList(2);                                                                                 //L2  OPCALL Set
                                {
                                    msg.AddAscii(AppUtil.ToAscii(aINTERLOCK_ID[i], 20));                                               //A20 OPCALL Name  
                                    msg.AddAscii(AppUtil.ToAscii(aMESSAGE[i], 120));                                                //A120 OPCALL Message
                                }
                            }
                        }
                    }
                }
            }
            this.SecsDriver.WriteLogAndSendMessage(msg, sCEID);

            CommonData.Instance.OnStreamFunctionAdd("MAIN", "E->H", "INTERLOCK", "S6F11", "502", "Interlock Confirm Send", null);
            #endregion
        }
    }
}