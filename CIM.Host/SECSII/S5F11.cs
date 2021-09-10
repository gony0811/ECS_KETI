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
    public class S5F11 : SFMessage
    {
        string _EQPID;
        string _MOUDLEID;
        ALARM.eALST _ALST;
        ALARM.eALCD _ALCD;
        string _ALID;
        string _ALTX;

        public S5F11(SECSDriverBase driver)
            : base(driver)
        {
            Stream = 5; Function = 11;
        }

        public override void DoWork(string driverName, object obj)
        {
            //bool bResult;
            ALARM alarm = obj as ALARM;

            _EQPID = CommonData.Instance.EQP_SETTINGS.EQPID;
            //string moduleIDTagName = string.Format("i{0}.EQStatus.ModuleID", alarm.MODULE);
            //_MOUDLEID = DataManager.Instance.GET_STRING_DATA(moduleIDTagName, out bResult);
            MODULE module = CommonData.Instance.MODULE_SETTINGS.Where(m => m.MODULE_NAME == alarm.MODULE).FirstOrDefault(); //KTW 18.04.11
            _MOUDLEID = module.UNIT_ID;
            _ALST = alarm.STATUS;
            _ALCD = alarm.LEVEL;
            _ALID = alarm.ID;
            _ALTX = alarm.TEXT;

            LogHelper.Instance.BizLog.DebugFormat("[{0}]DoWork : {1}, {2}", this.GetType().Name, _MOUDLEID, obj);

            CommonData.Instance.OnStreamFunctionAdd(_MOUDLEID, "E->H", "ALARM", "S5F11", null, "Alarm Report Send", null);

            SecsMessage secsMessage = new SecsMessage(Stream, Function, true, Convert.ToInt32(SecsDriver.DeviceID));
            secsMessage.AddList(6);
            secsMessage.AddAscii(_EQPID);
            secsMessage.AddAscii(_MOUDLEID);
            secsMessage.AddAscii(((int)_ALST).ToString());
            secsMessage.AddAscii(((int)_ALCD).ToString());
            secsMessage.AddAscii(_ALID);
            secsMessage.AddAscii(_ALTX);

            SecsDriver.WriteLogAndSendMessage(secsMessage, "");

            //AlarmManager.Instance.UpdateAlarmHistory(alarm);
        }
    }
}
