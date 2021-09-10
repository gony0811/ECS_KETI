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
    class S6F11_EQPStatusChange_CEID101 : SFMessage
    {
        /*S6F11(EQPStatusChange_CEID102), CEID :101*/

        public S6F11_EQPStatusChange_CEID101(SECSDriverBase driver)
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

            if (data.Module != "PLC1") return;

            //2. Variable
            string sDATAID = "0";
            string sCRST = ((int)CommonData.Instance.HOST_MODE).ToString();
            string sCEID = "101";
            string sEQPID = CommonData.Instance.EQP_SETTINGS.EQPID;
            // RTPID "101"
            //string moduleIDTagName = string.Format("i{0}.EQStatus.ModuleID", data.Module);
            MODULE module = CommonData.Instance.MODULE_SETTINGS.Where(m => m.MODULE_NAME == data.Module).FirstOrDefault(); //KTW 18.04.11

            string availabilityTagName = string.Format("i{0}.EQStatus.Availability", data.Module);
            string interlockTagName = string.Format("i{0}.EQStatus.Interlock", data.Module);
            string moveStateTagName = string.Format("i{0}.EQStatus.Move", data.Module);
            string runStateTagName = string.Format("i{0}.EQStatus.Run", data.Module);
            string frontStateTagName = string.Format("i{0}.EQStatus.Front", data.Module);
            string rearStateTagName = string.Format("i{0}.EQStatus.Rear", data.Module);
            string PPStateTagName = string.Format("i{0}.EQStatus.PP_SPL", data.Module);
            string reasonCodeTagName = string.Format("i{0}.Availability.ReasonCode", data.Module);
            string descriptionTagName = string.Format("i{0}.Availability.Description", data.Module);

            //string sMODULEID = DataManager.Instance.GET_STRING_DATA(moduleIDTagName, out bResult);
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

            // RTPID "103"
            string availabilityTagName2 = string.Format("vSys{0}.EQStatus.Availability", data.Module.Substring(3, 1));
            string interlockTagName2 = string.Format("vSys{0}.EQStatus.Interlock", data.Module.Substring(3, 1));
            string moveStateTagName2 = string.Format("vSys{0}.EQStatus.Move", data.Module.Substring(3, 1));
            string runStateTagName2 = string.Format("vSys{0}.EQStatus.Run", data.Module.Substring(3, 1));
            string frontStateTagName2 = string.Format("vSys{0}.EQStatus.Front", data.Module.Substring(3, 1));
            string rearStateTagName2 = string.Format("vSys{0}.EQStatus.Rear", data.Module.Substring(3, 1));
            string PPStateTagName2 = string.Format("vSys{0}.EQStatus.PP_SPL", data.Module.Substring(3, 1));
            string reasonCodeTagName2 = string.Format("vSys{0}.Availability.ReasonCode", data.Module.Substring(3, 1));
            string descriptionTagName2 = string.Format("vSys{0}.Availability.Description", data.Module.Substring(3, 1));

            string sAVAILABILITYSTATE2 = DataManager.Instance.GET_STRING_DATA(availabilityTagName2, out bResult);
            string sINTERLOCKSTATE2 = DataManager.Instance.GET_STRING_DATA(interlockTagName2, out bResult);
            string sMOVESTATE2 = DataManager.Instance.GET_STRING_DATA(moveStateTagName2, out bResult);
            string sRUNSTATE2 = DataManager.Instance.GET_STRING_DATA(runStateTagName2, out bResult);
            string sFRONTSTATE2 = DataManager.Instance.GET_STRING_DATA(frontStateTagName2, out bResult);
            string sREARSTATE2 = DataManager.Instance.GET_STRING_DATA(rearStateTagName2, out bResult);
            string sPP_SPLSTATE2 = DataManager.Instance.GET_STRING_DATA(PPStateTagName2, out bResult);
            string sREASONCODE2 = DataManager.Instance.GET_STRING_DATA(reasonCodeTagName2, out bResult);
            string sDESCRIPTION2 = DataManager.Instance.GET_STRING_DATA(descriptionTagName2, out bResult);

            if ((sAVAILABILITYSTATE == sAVAILABILITYSTATE2) && (sINTERLOCKSTATE == sINTERLOCKSTATE2) && (sMOVESTATE == sMOVESTATE2) && (sRUNSTATE == sRUNSTATE2)) return;

            

            string sALST = string.Empty;
            string sALCD = string.Empty;
            string sALID = string.Empty;
            string sALTX = string.Empty;

            string interlockid = string.Format("o{0}.Interlock.ID", data.Module);
            string interlockmsg = string.Format("o{0}.Interlock.Text", data.Module);

            string sINTERLOCKID = DataManager.Instance.GET_STRING_DATA(interlockid, out bResult);
            string sINTERLOCKMSG = DataManager.Instance.GET_STRING_DATA(interlockmsg, out bResult);

            int nAlarmList;

            if (AlarmManager.Instance.GetCurrentOccurredAlarms().Count > 0)
                nAlarmList = 1;
            else
                nAlarmList = 0;

            #region S6F11(Equipment Status Change), CEID: 101

            string tpmlossreadyTagName = string.Format("i{0}.Send.TPMLossReady", data.Module);
            string sTPMLOSSREADY = DataManager.Instance.GET_INT_DATA(tpmlossreadyTagName, out bResult).ToString();

            if (sTPMLOSSREADY != "1")
            {
                SecsMessage msg = new SecsMessage(Stream, Function, true, Convert.ToInt32(SecsDriver.DeviceID));

                msg.AddList(3);                                                                                     //L3  EQP Status Event Info
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
                        msg.AddList(2);                                                                                     //L2  RPTID 111 Set
                        {
                            msg.AddAscii(AppUtil.ToAscii("101", gDefine.DEF_RPTID_SIZE));                                       //A3  RPTID="111"
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
                        msg.AddList(2);                                                                                     //L2  RPTID 112 Set
                        {
                            msg.AddAscii(AppUtil.ToAscii("103", gDefine.DEF_RPTID_SIZE));                                       //A3  RPTID="112"
                            msg.AddList(9);                                                                                     //L9  EQP State Set
                            {
                                msg.AddAscii(sAVAILABILITYSTATE2);                                                                  //A1  EQ Avilability State Info
                                msg.AddAscii(sINTERLOCKSTATE2);                                                                     //A1  Interlock Avilability State Info
                                msg.AddAscii(sMOVESTATE2);                                                                          //A1  EQ Move State Info
                                msg.AddAscii(sRUNSTATE2);                                                                           //A1  Cell existence/nonexistence Check
                                msg.AddAscii(sFRONTSTATE2);                                                                         //A1  Upper EQ Processing State
                                msg.AddAscii(sREARSTATE2);                                                                          //A1  Lower EQ Processing State
                                msg.AddAscii(sPP_SPLSTATE2);                                                                        //A1  Sample Run-Normal Run State
                                msg.AddAscii(AppUtil.ToAscii(sREASONCODE2, gDefine.DEF_REASONCODE_SIZE));                           //A20 Code Info
                                msg.AddAscii(AppUtil.ToAscii(sDESCRIPTION2, gDefine.DEF_DESCRIPTION_SIZE));                         //A40 EQ Description
                            }
                        }
                        msg.AddList(2);                                                                                     //L2  RPTID 104 Set
                        {
                            msg.AddAscii(AppUtil.ToAscii("104", gDefine.DEF_RPTID_SIZE));                                       //A3  RPTID="104"
                                                                                                                                //Equipment equipment = Common.CommonData.GetInstance().EQUIPMENT;
                            ALARM reasonAlarm = null;
                            if (sAVAILABILITYSTATE != sAVAILABILITYSTATE2)
                            {
                                if (sAVAILABILITYSTATE == "1") //Down
                                {
                                    if (AlarmManager.Instance.GetCurrentOccurredAlarms().Where(a => a.LEVEL == ALARM.eALCD.Serious).ToList().Count > 0)
                                    {
                                        List<ALARM> reportAlarm = AlarmManager.Instance.GetCurrentOccurredAlarms().Where(a => a.LEVEL == ALARM.eALCD.Serious).ToList();
                                        //if (reportAlarm.Value.requireReport == true)
                                        //{
                                        reasonAlarm = reportAlarm.FirstOrDefault();
                                        //equipment.OccurredHeavyAlarmHist[reportAlarm.Value.NO].requireReport = false;
                                        //}
                                    }
                                    //equipment.OccurredHeavyAlarmHist.Clear();
                                }
                            }

                            if (sINTERLOCKSTATE == "1" && sINTERLOCKSTATE != sINTERLOCKSTATE2)
                            {
                                sALST = string.Empty;
                                sALCD = string.Empty;

                                msg.AddList(1);                                                      //Ln  Alarm List                             
                                {
                                    msg.AddList(4);
                                    {
                                        msg.AddAscii(sALST);                                                                            //A1  Alarm Occur/Clear
                                        msg.AddAscii(sALCD);                                                                            //A1  Alarm Level Code
                                        msg.AddAscii(AppUtil.ToAscii(sINTERLOCKID, gDefine.DEF_ALID_SIZE));                                    //A10 Alarm ID
                                        msg.AddAscii(AppUtil.ToAscii(sINTERLOCKMSG, gDefine.DEF_ALTX_SIZE));
                                    }
                                }
                            }
                            else
                            {
                                //인터락이 아니고 
                                if (reasonAlarm == null)
                                {
                                    //L0
                                    msg.AddList(0);
                                }
                                else
                                {
                                    sALST = Convert.ToInt16(reasonAlarm.STATUS).ToString();
                                    sALCD = Convert.ToInt16(reasonAlarm.LEVEL).ToString();
                                    sALID = reasonAlarm.ID;
                                    sALTX = reasonAlarm.TEXT;

                                    msg.AddList(Convert.ToInt16(nAlarmList));                                                           //Ln  Alarm List
                                    {
                                        if (nAlarmList != 0)
                                        {
                                            msg.AddList(4);                                                                                 //L4  Alarm Set
                                            {
                                                msg.AddAscii(sALST);                                                                            //A1  Alarm Occur/Clear
                                                msg.AddAscii(sALCD);                                                                            //A1  Alarm Level Code
                                                msg.AddAscii(AppUtil.ToAscii(sALID, gDefine.DEF_ALID_SIZE));                                    //A10 Alarm ID
                                                msg.AddAscii(AppUtil.ToAscii(sALTX, gDefine.DEF_ALTX_SIZE));
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                this.SecsDriver.WriteLogAndSendMessage(msg, sCEID);

                CommonData.Instance.OnStreamFunctionAdd(sMODULEID, "E->H", "STATE", "S6F11", sCEID, "EQP Status Change", null);
            }
            #endregion
        }
    }
}