using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using INNO6.Core;
using INNO6.IO;
using CIM.Manager;
using CIM.Common;
using CIM.Common.TC;
using SYSWIN.Secl;

namespace CIM.Host.SECSII
{
    class S14F1 : SFMessage
    {
        /*S14F1(Get Attribute Request)*/

        public S14F1(SECSDriverBase driver)
            : base(driver)
        {
            Stream = 14; Function = 1;
        }
        public override void DoWork(string driverName, object obj)
        {
            LogHelper.Instance.BizLog.DebugFormat("[{0}]DoWork : {1}, {2}", this.GetType().Name, driverName, obj);
            Data data = obj as Data;
            bool bResult;
            INNO6.IO.Interface.eDevMode connectStatus = DataManager.Instance.IsDeviceMode("DEV1");

            string sEQPID = CommonData.Instance.EQP_SETTINGS.EQPID;
            string sOBJTYPE = "";

            string objidTagName = string.Format("i{0}.GetAttributeRequest.OBJID", data.Module);
            string commentTagName = string.Format("i{0}.GetAttributeRequest.Comment", data.Module);

            string sOBJID = DataManager.Instance.GET_STRING_DATA(objidTagName, out bResult);
            string sCOMMENT = DataManager.Instance.GET_STRING_DATA(commentTagName, out bResult);

            CommonData.Instance.VALIDATION_DATA.SaveValidationData(KindOfValidation.ATTRIBUTE_INFO_SEND, sOBJID, 0, data.Module);

            #region S14F1(Get Attribute Request)
            SecsMessage msg = new SecsMessage(Stream, Function, true, Convert.ToInt32(SecsDriver.DeviceID));

            msg.AddList(4);
            {
                msg.AddAscii(sEQPID);
                msg.AddAscii(sOBJTYPE);
                msg.AddAscii(sOBJID);
                msg.AddAscii(sCOMMENT);
            }
            this.SecsDriver.WriteLogAndSendMessage(msg, "");

            CommonData.Instance.OnStreamFunctionAdd("LOADER", "E->H", "GET ATTRIBUTE REQUEST", "S14F1", null, null, null);
            #endregion
        }
    }
}
