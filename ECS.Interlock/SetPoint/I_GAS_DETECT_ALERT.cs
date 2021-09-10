using INNO6.Core.Interlock.Interface;
using INNO6.Core.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS.Interlock.SetPoint
{
    public class I_GAS_DETECT_ALERT : IExecuteInterlock
    {
        public void Execute()
        {
            //GAS DETECTOR ALARM 발생시 할일 정의
            AlarmManager.Instance.SetAlarm("E1005");
        }
    }
}
