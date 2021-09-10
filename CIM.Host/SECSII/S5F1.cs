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
    public class S5F1 : SFMessage
    {
        string _EQPID;
        ALARM.eALST _ALST;
        ALARM.eALCD _ALCD;
        string _ALID;
        string _ALTX;

        public S5F1(SECSDriverBase driver)
            : base(driver)
        {
            Stream = 5; Function = 1;
        }

        public override void DoWork(string driverName, object obj)
        {
            LogHelper.Instance.BizLog.DebugFormat("[{0}]DoWork : {1}, {2}", this.GetType().Name, driverName, obj);
            CommonData.Instance.OnStreamFunctionAdd("MAIN", "E->H", "RMS", "S5F1", null, "Alarm Report Send", null);
            ALARM alarm = obj as ALARM;

            _EQPID = CommonData.Instance.EQP_SETTINGS.EQPID;
            _ALST = alarm.STATUS;
            _ALCD = alarm.LEVEL;
            _ALID = alarm.ID;
            _ALTX = alarm.TEXT;

            SecsMessage secsMessage = new SecsMessage(Stream, Function, true, Convert.ToInt32(SecsDriver.DeviceID));
            secsMessage.AddList(5);
            secsMessage.AddAscii(_EQPID);
            secsMessage.AddAscii(((int)_ALST).ToString());
            secsMessage.AddAscii(((int)_ALCD).ToString());
            secsMessage.AddAscii(_ALID);
            secsMessage.AddAscii(_ALTX);

            SecsDriver.WriteLogAndSendMessage(secsMessage, "");

            AlarmManager.Instance.UpdateAlarmHistory(alarm);
        }
    }
}
