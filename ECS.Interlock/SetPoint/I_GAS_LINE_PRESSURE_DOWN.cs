using INNO6.Core.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS.Interlock.SetPoint
{
    public class I_GAS_LINE_PRESSURE_DOWN : AbstractExecuteInterlock
    {
        public override bool Execute(object setvalue)
        {
            //GAS CABINET LEAK DETECTION ALARM 발생시 할일 정의

            AlarmManager.Instance.SetAlarm("E1008");

            return true;
        }
    }
}
