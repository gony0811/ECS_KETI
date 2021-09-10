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
    class S6F11_EquipmentLossCodeReport_CEID616 : SFMessage
    {
        /*S6F11(EquipmentLossCodeReport), CEID :616*/

        public S6F11_EquipmentLossCodeReport_CEID616(SECSDriverBase driver)
            : base(driver)
        {
            Stream = 6; Function = 11;
        }

        public override void DoWork(string driverName, object obj)
        {
            LogHelper.Instance.BizLog.DebugFormat("[{0}]DoWork : {1}, {2}", this.GetType().Name, driverName, obj);
            Data data = obj as Data;
            bool bResult;
            INNO6.IO.Interface.eDevMode connectStatus = DataManager.Instance.IsDeviceMode("DEV1");

            //1. Parsing Data 
            string sDATAID = "0";
            string sCEID = "616";

            //RTPID "100"
            string sEQPID = CommonData.Instance.EQP_SETTINGS.EQPID;
            string sCRST = ((int)CommonData.Instance.HOST_MODE).ToString();

            //RTPID "102"
            MODULE module = CommonData.Instance.MODULE_SETTINGS.Where(m => m.MODULE_NAME == data.Module).FirstOrDefault(); //KTW 18.04.11
            
            //string moduleIDTagName = string.Format("i{0}.EQStatus.ModuleID", data.Module);
            string availabilityTagName = string.Format("i{0}.EQStatus.Availability", data.Module);
            string interlockTagName = string.Format("i{0}.EQStatus.Interlock", data.Module);
            string moveStateTagName = string.Format("i{0}.EQStatus.Move", data.Module);
            string runStateTagName = string.Format("i{0}.EQStatus.Run", data.Module);
            string frontStateTagName = string.Format("i{0}.EQStatus.Front", data.Module);
            string rearStateTagName = string.Format("i{0}.EQStatus.Rear", data.Module);
            string PPStateTagName = string.Format("i{0}.EQStatus.PP_SPL", data.Module);
            string reasonCodeTagName = string.Format("i{0}.Availability.ReasonCode", data.Module);
            string descriptionTagName = string.Format("i{0}.Availability.Description", data.Module);



            string sMODULEID = module.UNIT_ID; //KTW 18.04.11
            string sAVAILABILITYSTATE = DataManager.Instance.GET_STRING_DATA(availabilityTagName, out bResult);
            string sINTERLOCKSTATE = DataManager.Instance.GET_STRING_DATA(interlockTagName, out bResult);
            string sMOVESTATE = DataManager.Instance.GET_STRING_DATA(moveStateTagName, out bResult);
            string sRUNSTATE = DataManager.Instance.GET_STRING_DATA(runStateTagName, out bResult);
            string sFRONTSTATE = DataManager.Instance.GET_STRING_DATA(frontStateTagName, out bResult);
            string sREARSTATE = DataManager.Instance.GET_STRING_DATA(rearStateTagName, out bResult);
            string sPP_SPLSTATE = DataManager.Instance.GET_STRING_DATA(PPStateTagName, out bResult);
            string sREASONCODE = DataManager.Instance.GET_STRING_DATA(reasonCodeTagName, out bResult);
            string sDESCRIPTION = DataManager.Instance.GET_STRING_DATA(descriptionTagName, out bResult);

            //RTPID "806"
            string losscodeTagName = string.Format("i{0}.TPMLoss.Code", data.Module);
            string lossdescriptionTagName = string.Format("i{0}.TPMLoss.Description", data.Module);

            string sLOSS_CODE = DataManager.Instance.GET_INT_DATA(losscodeTagName, out bResult).ToString();
            string sLOSS_DESCRIPTION = DataManager.Instance.GET_STRING_DATA(lossdescriptionTagName, out bResult);

            sLOSS_CODE = sLOSS_CODE.Trim();
            if (sLOSS_CODE.Length < 5)
                sLOSS_CODE = "0" + sLOSS_CODE;

            #region S6F11(Equipment Loss Code Report), CEID:616

            SecsMessage msg = new SecsMessage(Stream, Function, true, Convert.ToInt32(SecsDriver.DeviceID));

            msg.AddList(3);                                                                                  //L3  EQP Status Event Info
            {
                msg.AddAscii(AppUtil.ToAscii(sDATAID, gDefine.DEF_DATAID_SIZE));                                //A4  Data ID
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
                            msg.AddAscii(AppUtil.ToAscii(sMODULEID, gDefine.DEF_UNITID_SIZE));                                  //A40 HOST REQ UNITID 
                            msg.AddList(9);                                                                                 //L9  EQP State Set
                            {
                                msg.AddAscii(sAVAILABILITYSTATE);                                                               //A1  EQ Avilability State Info
                                msg.AddAscii(sINTERLOCKSTATE);                                                                  //A1  Interlock Avilability State Info
                                msg.AddAscii(sMOVESTATE);                                                                       //A1  EQ Move State Info
                                msg.AddAscii(sRUNSTATE);                                                                        //A1  Cell existence/nonexistence Check
                                msg.AddAscii(sFRONTSTATE);                                                                      //A1  Upper EQ Processing State
                                msg.AddAscii(sREARSTATE);                                                                       //A1  Lower EQ Processing State
                                msg.AddAscii(sPP_SPLSTATE);                                                                     //A1  Sample Run-Normal Run State
                                msg.AddAscii(AppUtil.ToAscii(sREASONCODE, gDefine.DEF_REASONCODE_SIZE));                        //A20 Code Info
                                msg.AddAscii(AppUtil.ToAscii(sDESCRIPTION, gDefine.DEF_DESCRIPTION_SIZE));                      //A40 EQ Description
                            }
                        }
                    }
                    msg.AddList(2);                                                                                 //L2  RPTID 805 Set
                    {
                        msg.AddAscii(AppUtil.ToAscii("806", gDefine.DEF_RPTID_SIZE));                                   //A3  RPTID="806"
                        msg.AddList(2);                                                                                 //L2  Loss Code List
                        {
                            msg.AddAscii(AppUtil.ToAscii(sLOSS_CODE, 20));                                                  //A20 Loss Code 
                            msg.AddAscii(AppUtil.ToAscii(sLOSS_DESCRIPTION, 40));                                           //A40 Loss Code DESCRIPTION
                        }
                    }
                }
            }
            this.SecsDriver.WriteLogAndSendMessage(msg, sCEID);

            CommonData.Instance.OnStreamFunctionAdd(sMODULEID, "E->H", "TPM", "S6F11", sCEID, "Unit TPM Loss", null);
            #endregion
        }
    }
}
