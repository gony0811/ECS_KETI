using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using INNO6.Core;
using INNO6.IO;
using CIM.Manager;
using CIM.Common;
using SYSWIN.Secl;

namespace CIM.Host.SECSII
{
    class S6F203 : SFMessage
    {
        /*S6F203(SpecificValidationRequest)*/
        private string _cellPortNo;

        public S6F203(SECSDriverBase driver, string CellPortNo)
        : base(driver)
        {
            Stream = 6; Function = 203;
            _cellPortNo = CellPortNo;
        }

        public override void DoWork(string driverName, object obj)
        {
            Data data = obj as Data;
            LogHelper.Instance.BizLog.DebugFormat("[{0}]DoWork : {1}, {2}", this.GetType().Name, driverName, obj);
            bool bResult;
            INNO6.IO.Interface.eDevMode connectStatus = DataManager.Instance.IsDeviceMode("DEV1");

            MODULE module = CommonData.Instance.MODULE_SETTINGS.Where(m => m.TYPE == eModuleType.UNLOADER).FirstOrDefault();

            string optioncodeTagName = string.Format("i{0}.SpecificValidationRequest.OptionCode{1}", module.MODULE_NAME, _cellPortNo);
            string cellIDTagName = string.Format("i{0}.SpecificValidationRequest.CellID{1}", module.MODULE_NAME, _cellPortNo);
            string optioninfoTagName = string.Format("i{0}.SpecificValidationRequest.OptionInfo{1}", module.MODULE_NAME, _cellPortNo);

            string sEQPID = CommonData.Instance.EQP_SETTINGS.EQPID;
            string sOPTIONCODE = DataManager.Instance.GET_STRING_DATA(optioncodeTagName, out bResult);
            string sCELLID = DataManager.Instance.GET_STRING_DATA(cellIDTagName, out bResult);
            string sOPTIONINFO = DataManager.Instance.GET_STRING_DATA(optioninfoTagName, out bResult);

            #region S6F203(SpecificValidationRequest)

            SecsMessage msg = new SecsMessage(Stream, Function, true, Convert.ToInt32(SecsDriver.DeviceID));

            msg.AddList(3);
            {
                msg.AddAscii(AppUtil.ToAscii(sEQPID, gDefine.DEF_EQPID_SIZE));               //A40 EQPID
                msg.AddAscii(AppUtil.ToAscii(sOPTIONCODE, gDefine.DEF_LABEL_OPTIONCODE_SIZE));         //A10 OptionCode
                msg.AddList(2);
                {
                    msg.AddAscii(AppUtil.ToAscii(sCELLID, gDefine.DEF_LABEL_CELLID_SIZE));         //A40 CellID
                    msg.AddAscii(AppUtil.ToAscii(sOPTIONINFO, gDefine.DEF_LABEL_PRODUCTID_SIZE));         //A40 OptionInfo
                }
            }
            this.SecsDriver.WriteLogAndSendMessage(msg, "");
            if (!string.IsNullOrEmpty(sCELLID))
            {
                CommonData.Instance.VALIDATION_DATA.SaveValidationData(KindOfValidation.SPECIFIC_VALIDATION, sCELLID, Convert.ToInt32(_cellPortNo), module.MODULE_NAME);
            }
                CommonData.Instance.OnStreamFunctionAdd("MAIN", "E->H", "SPECIFIC", "S6F203", null, string.Format("Specific Validation Request #{0}", _cellPortNo), null);
            #endregion
        }
    }
}