using ECS.Common.Helper;
using INNO6.Core.Interlock.Interface;
using INNO6.Core.Manager;
using INNO6.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS.Interlock.SetPoint
{
    public class I_GAS_DETECT_ALERT : AbstractExecuteInterlock
    {
        public override bool Execute(object setvalue)
        {
            //GAS DETECTOR ALARM 발생시 할일 정의
            
            AlarmManager.Instance.SetAlarm("E1005");

            return true;
        }
    }
}
