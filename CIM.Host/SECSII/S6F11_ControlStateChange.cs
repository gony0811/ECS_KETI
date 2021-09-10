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
    public class S6F11_ControlStateChange : SFMessage
    {
        /*S6F11(Control State Change), CEID:104, 105, 106*/

        public S6F11_ControlStateChange(SECSDriverBase driver) 
            : base(driver)
        {
            Stream = 6; Function = 11;
        }
        public override void DoWork(string driverName, object obj)
        {
            LogHelper.Instance.BizLog.DebugFormat("[{0}]DoWork : {1}, {2}", this.GetType().Name, driverName, obj);
            bool bResult;
            INNO6.IO.Interface.eDevMode connectStatus = DataManager.Instance.IsDeviceMode("DEV1");
            string sDATAID = "0";
            string sCRST = "1";
            string sEQPID = CommonData.Instance.EQP_SETTINGS.EQPID;
            string sCEID = "106";

            MODULE module = CommonData.Instance.MODULE_SETTINGS.Where(m => m.TYPE == eModuleType.MASTER).FirstOrDefault();

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

            CommonData.Instance.HOST_MODE = eHostMode.HostOnlineRemote;

            if (sCEID == "104") // OFFLINE
            {
                FDCManager.Instance.AllTraceJobStop();
            }

            #region S6F11(Control State Change), CEID: 104,105,106

            SecsMessage msg = new SecsMessage(Stream, Function, true, Convert.ToInt32(SecsDriver.DeviceID));

            msg.AddList(3);                                                                                      //L3  Unit Status Event Info
            {
                msg.AddAscii(AppUtil.ToAscii(sDATAID, gDefine.DEF_DATAID_SIZE));                                    //A4  Data ID
                msg.AddAscii(AppUtil.ToAscii(sCEID, gDefine.DEF_CEID_SIZE));                                        //A3  Collection Event ID
                msg.AddList(2);                                                                                     //L2  RPTID Set
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
                }
            }

            this.SecsDriver.WriteLogAndSendMessage(msg, sCEID);
            
            CommonData.Instance.OnEqpStatusChanged(sAVAILABILITYSTATE, sINTERLOCKSTATE, sRUNSTATE, sPP_SPLSTATE, sMOVESTATE);
            

            CommonData.Instance.OnStreamFunctionAdd("MAIN", "E->H", "REMOTE", "S6F11", "106", "On-Line Remote", null);

            foreach (MODULE module1 in CommonData.Instance.MODULE_SETTINGS)
            {
                string availabilityTagName1 = string.Format("i{0}.EQStatus.Availability", module1.MODULE_NAME);
                string interlockTagName1 = string.Format("i{0}.EQStatus.Interlock", module1.MODULE_NAME);
                string moveStateTagName1 = string.Format("i{0}.EQStatus.Move", module1.MODULE_NAME);
                string runStateTagName1 = string.Format("i{0}.EQStatus.Run", module1.MODULE_NAME);
                string frontStateTagName1 = string.Format("i{0}.EQStatus.Front", module1.MODULE_NAME);
                string rearStateTagName1 = string.Format("i{0}.EQStatus.Rear", module1.MODULE_NAME);
                string PPStateTagName1 = string.Format("i{0}.EQStatus.PP_SPL", module1.MODULE_NAME);
                string reasonCodeTagName1 = string.Format("i{0}.Availability.ReasonCode", module1.MODULE_NAME);
                string descriptionTagName1 = string.Format("i{0}.Availability.Description", module1.MODULE_NAME);
                
                DataManager.Instance.SET_STRING_DATA(string.Format("vSys{0}.EQStatus.Availability", module1.MODULE_NAME.Substring(3,1)), DataManager.Instance.GET_STRING_DATA(availabilityTagName1, out bResult));
                DataManager.Instance.SET_STRING_DATA(string.Format("vSys{0}.EQStatus.Interlock", module1.MODULE_NAME.Substring(3,1)), DataManager.Instance.GET_STRING_DATA(interlockTagName1, out bResult));
                DataManager.Instance.SET_STRING_DATA(string.Format("vSys{0}.EQStatus.Move", module1.MODULE_NAME.Substring(3,1)), DataManager.Instance.GET_STRING_DATA(moveStateTagName1, out bResult));
                DataManager.Instance.SET_STRING_DATA(string.Format("vSys{0}.EQStatus.Run", module1.MODULE_NAME.Substring(3,1)), DataManager.Instance.GET_STRING_DATA(runStateTagName1, out bResult));
                DataManager.Instance.SET_STRING_DATA(string.Format("vSys{0}.EQStatus.Front", module1.MODULE_NAME.Substring(3,1)), DataManager.Instance.GET_STRING_DATA(frontStateTagName1, out bResult));
                DataManager.Instance.SET_STRING_DATA(string.Format("vSys{0}.EQStatus.Rear", module1.MODULE_NAME.Substring(3,1)), DataManager.Instance.GET_STRING_DATA(rearStateTagName1, out bResult));
                DataManager.Instance.SET_STRING_DATA(string.Format("vSys{0}.EQStatus.PP_SPL", module1.MODULE_NAME.Substring(3,1)), DataManager.Instance.GET_STRING_DATA(PPStateTagName1, out bResult));
                DataManager.Instance.SET_STRING_DATA(string.Format("vSys{0}.Availability.ReasonCode", module1.MODULE_NAME.Substring(3,1)), DataManager.Instance.GET_STRING_DATA(reasonCodeTagName1, out bResult));
                DataManager.Instance.SET_STRING_DATA(string.Format("vSys{0}.Availability.Description", module1.MODULE_NAME.Substring(3,1)), DataManager.Instance.GET_STRING_DATA(descriptionTagName1, out bResult));

                string sAVAILABILITYSTATE2 = DataManager.Instance.GET_STRING_DATA(availabilityTagName1, out bResult);
                string sINTERLOCKSTATE2 = DataManager.Instance.GET_STRING_DATA(interlockTagName1, out bResult);
                string sMOVESTATE2 = DataManager.Instance.GET_STRING_DATA(moveStateTagName1, out bResult);
                string sRUNSTATE2 = DataManager.Instance.GET_STRING_DATA(runStateTagName1, out bResult);
                string sPP_SPLSTATE2 = DataManager.Instance.GET_STRING_DATA(PPStateTagName1, out bResult);

                string[] strtemp = module1.UNIT_ID.Split('_');  //KTW 18.04.11

                CommonData.Instance.OnUniteEqpStatusChanged(sAVAILABILITYSTATE2, sINTERLOCKSTATE2, sRUNSTATE2, sMOVESTATE2, sPP_SPLSTATE2, module1.MODULE_NAME, strtemp[2]);

            }

            #endregion
        }
    }
}
