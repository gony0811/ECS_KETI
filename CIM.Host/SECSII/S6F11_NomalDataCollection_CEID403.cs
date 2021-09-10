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
    class S6F11_NomalDataCollection_CEID403 : SFMessage
    {
        /*S6F11(NomalDataCollection), CEID :403*/

        private string _cellPortNo;

        public S6F11_NomalDataCollection_CEID403(SECSDriverBase driver, string CellPortNo)
            : base(driver)
        {
            Stream = 6; Function = 11;
            _cellPortNo = CellPortNo;
        }

        public override void DoWork(string driverName, object obj)
        {
            LogHelper.Instance.BizLog.DebugFormat("[{0}]DoWork : {1}, {2}", this.GetType().Name, driverName, obj);
            Data data = obj as Data;
            bool bResult;
            INNO6.IO.Interface.eDevMode connectStatus = DataManager.Instance.IsDeviceMode("DEV1");

            //1. Parsing Data 
            string sDATAID = "0";
            string sCEID = "403";

            //RTPID "100"
            string sEQPID = CommonData.Instance.EQP_SETTINGS.EQPID;
            string sCRST = ((int)CommonData.Instance.HOST_MODE).ToString();

            //string moduleIDTagName = string.Format("i{0}.EQStatus.ModuleID", data.Module);
            //string sMODULEID = DataManager.Instance.GET_STRING_DATA(moduleIDTagName, out bResult);
            MODULE module = CommonData.Instance.MODULE_SETTINGS.Where(m => m.MODULE_NAME == data.Module).FirstOrDefault(); //KTW 18.04.11
            string sMODULEID = module.UNIT_ID; //KTW 18.04.11

            //RTPID "300"
            string cellIDTagName = string.Format("i{0}.NormalDataCollection.CellID{1}", data.Module, _cellPortNo);
            string productIDTagName = string.Format("i{0}.NormalDataCollection.ProductID{1}", data.Module, _cellPortNo);
            string stepIDTagName = string.Format("i{0}.NormalDataCollection.StepID{1}", data.Module, _cellPortNo);

            string sCELLID = DataManager.Instance.GET_STRING_DATA(cellIDTagName, out bResult);
            string sPPID = RMSManager.Instance.CurrentPPID;
            string sPRODUCTID = DataManager.Instance.GET_STRING_DATA(productIDTagName, out bResult);
            string sSTEPID = DataManager.Instance.GET_STRING_DATA(stepIDTagName, out bResult);

            //RTPID "600"
            int nDV_NO = 0;

            Dictionary<string, string> dvNameValueList = DVManager.Instance.GetDVNameValueList(data.Module, string.Format("{0}", _cellPortNo));
            nDV_NO = dvNameValueList.Count;
            List<string> aDV_NAME = new List<string>(nDV_NO);
            List<string> aDV = new List<string>(nDV_NO);

            #region S6F11(Cell Process End), CEID:403

            SecsMessage msg = new SecsMessage(Stream, Function, true, Convert.ToInt32(SecsDriver.DeviceID));

            msg.AddList(3);                                                                                     //L3  Cell Complete Event Info
            {
                msg.AddAscii(AppUtil.ToAscii(sDATAID, gDefine.DEF_DATAID_SIZE));                                    //A4  Data ID
                msg.AddAscii(AppUtil.ToAscii(sCEID, gDefine.DEF_CEID_SIZE));                                        //A3  Collection Event ID
                msg.AddList(3);
                {
                    msg.AddList(2);                                                                                     //L2  RPTID 100 Set 
                    {
                        msg.AddAscii(AppUtil.ToAscii("100", gDefine.DEF_RPTID_SIZE));                                       //A3  RPTID="100"
                        msg.AddList(2);                                                                                     //L2  EQP Control State Set
                        {
                            msg.AddAscii(AppUtil.ToAscii(sEQPID, gDefine.DEF_UNITID_SIZE));                                      //A40 HOST REQ EQPID 
                            msg.AddAscii(sCRST);                                                                                //A1  Online Control State 
                        }
                    }
                    msg.AddList(2);                                                                                     //L2  RPTID 300 Set
                    {
                        msg.AddAscii(AppUtil.ToAscii("300", gDefine.DEF_RPTID_SIZE));                                       //A3  RPTID="300"
                        msg.AddList(4);                                                                                     //L4  CELL Info Set
                        {
                            msg.AddAscii(AppUtil.ToAscii(sCELLID, 40));                                                        //A40 CELL Unique ID 
                            msg.AddAscii(AppUtil.ToAscii(sPPID, gDefine.DEF_PPID_SIZE));                                        //A40 LOT 지PPID
                            msg.AddAscii(AppUtil.ToAscii(sPRODUCTID, gDefine.DEF_PRODUCTID_SIZE));                             //A40 CELL Prodecut ID
                            msg.AddAscii(AppUtil.ToAscii(sSTEPID, 40));                                                        //A40 CELL Step ID
                        }
                    }
                    msg.AddList(2);                                                                                     //L2  RPTID 300 Set
                    {
                        msg.AddAscii(AppUtil.ToAscii("600", gDefine.DEF_RPTID_SIZE));
                        msg.AddList(nDV_NO);
                        if (nDV_NO != 0)
                        {
                            foreach (KeyValuePair<string, string> list in dvNameValueList)
                            {

                                msg.AddList(2);                                                                                 //L2  DV Set
                                {
                                    msg.AddAscii(AppUtil.ToAscii(list.Key, 40));                        //A20 Data Name 
                                    msg.AddAscii(AppUtil.ToAscii(list.Value, 40));                  //A40 Data Value                                    
                                }
                                aDV_NAME.Add(list.Key);
                                aDV.Add(list.Value);
                            }
                        }
                        //else
                        //    msg.AddList(0);
                    }
                }
            }
            this.SecsDriver.WriteLogAndSendMessage(msg, sCEID);

            CommonData.Instance.OnStreamFunctionAdd(data.Module, "E->H", "DV", "S6F11", "403", string.Format("Normal Data Collection #{0}", _cellPortNo), null);
            LogHelper.Instance.Collection.DebugFormat("Collection, {0}, {1}", sCELLID, sMODULEID);
            #endregion
        }
    }
}                    
