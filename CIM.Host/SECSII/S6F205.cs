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
    class S6F205 : SFMessage
    {
        /*S6F205(CellLotInformationRequest)*/
        private string _cellPortNo;

        public S6F205(SECSDriverBase driver, string CellPortNo)
        : base(driver)
        {
            Stream = 6; Function = 205;
            _cellPortNo = CellPortNo;
        }

        public override void DoWork(string driverName, object obj)
        {
            LogHelper.Instance.BizLog.DebugFormat("[{0}]DoWork : {1}, {2}", this.GetType().Name, driverName, obj);
            
            bool bResult;
            INNO6.IO.Interface.eDevMode connectStatus = DataManager.Instance.IsDeviceMode("DEV1");

            MODULE module = CommonData.Instance.MODULE_SETTINGS.Where(m => m.TYPE == eModuleType.UNLOADER).FirstOrDefault();

            string cellIDTagName = string.Format("i{0}.CellLotInformationRequest.CellID{1}", module.MODULE_NAME, _cellPortNo);

            string sEQPID = CommonData.Instance.EQP_SETTINGS.EQPID;
            string sOPTIONCODE = "LOTINFO";
            string sCELLID = DataManager.Instance.GET_STRING_DATA(cellIDTagName, out bResult);

            CommonData.Instance.VALIDATION_DATA.SaveValidationData(KindOfValidation.CELL_LOT_INFO_SEND, sCELLID, Convert.ToInt32(_cellPortNo), module.MODULE_NAME);

            #region S6F205(CellLotInformationRequest)

            SecsMessage msg = new SecsMessage(Stream, Function, true, Convert.ToInt32(SecsDriver.DeviceID));

            msg.AddList(3);              //L3  Cell Lot Info Set
            {
                msg.AddAscii(sEQPID);                                  //A40 EQPID
                msg.AddAscii(sOPTIONCODE);                             //A10 OptionCode
                msg.AddList(1);
                {
                    msg.AddAscii(sCELLID);                                 //A40 CellID
                }
            }
            this.SecsDriver.WriteLogAndSendMessage(msg, "");

            CommonData.Instance.OnStreamFunctionAdd("MAIN", "E->H", "CARRIER", "S6F205", null, string.Format("Cell Lot Information Request #{0}", _cellPortNo), null);
            #endregion
        }
    }
}
