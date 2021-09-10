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
    class S6F11_MaterialIDReaderResult_CEID615 : SFMessage
    {
        /*S6F11(MaterialIDReaderResult), CEID :615*/

        public S6F11_MaterialIDReaderResult_CEID615(SECSDriverBase driver)
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

            MODULE module = CommonData.Instance.MODULE_SETTINGS.Where(m => m.MODULE_NAME == data.Module).FirstOrDefault(); //KTW 18.04.11

            //1. Parsing Data 
            string sDATAID = "0";
            string sCEID = "615";

            ////RTPID "811"
            //string moduleIDTagName = string.Format("i{0}.EQStatus.ModuleID", data.Module);
            string matIDTagName = string.Format("i{0}.MaterialIDReaderResult.MaterialID", data.Module);
            string matportIDTagName = string.Format("i{0}.MaterialIDReaderResult.PortID", data.Module);

            string sEQPID = CommonData.Instance.EQP_SETTINGS.EQPID;
            string sMODULEID = module.UNIT_ID; //KTW 18.04.11
            string MAT_ID = DataManager.Instance.GET_STRING_DATA(matIDTagName, out bResult);
            string MAT_PORT_ID = DataManager.Instance.GET_STRING_DATA(matportIDTagName, out bResult);

            #region S6F11(MaterialID Reader Result), CEID:615

            SecsMessage msg = new SecsMessage(Stream, Function, true, Convert.ToInt32(SecsDriver.DeviceID));

            msg.AddList(3);                                                                                  //L3  Reader Result Event Info
            {
                msg.AddAscii(AppUtil.ToAscii(sDATAID, gDefine.DEF_DATAID_SIZE));                                //A4  Data ID
                msg.AddAscii(AppUtil.ToAscii(sCEID, gDefine.DEF_CEID_SIZE));                                    //A3  Collection Event ID
                msg.AddList(1);                                                                                 //L1
                {
                    msg.AddList(5);                                                                                 //L5  RPTID 810Set
                    {
                        msg.AddAscii(AppUtil.ToAscii("811", gDefine.DEF_RPTID_SIZE));                                   //A3  RPTID="810"
                        msg.AddAscii(AppUtil.ToAscii(sEQPID, gDefine.DEF_EQPID_SIZE));                                  //A40 HOST REQ EQPID
                        msg.AddAscii(AppUtil.ToAscii(sMODULEID, gDefine.DEF_UNITID_SIZE));                                  //A40 HOST REQ MODULEID
                        msg.AddAscii(AppUtil.ToAscii(MAT_ID, 40));                                                //A40 EQP Material ID
                        msg.AddAscii(MAT_PORT_ID);                                                                //A1  Material Port ID
                    }
                }
            }
            this.SecsDriver.WriteLogAndSendMessage(msg, sCEID);

            CommonData.Instance.OnStreamFunctionAdd("MAIN", "E->H", "MATERIAL", "S6F11", "615", "Material ID Reader Result", null);
            #endregion
        }
    }
}
