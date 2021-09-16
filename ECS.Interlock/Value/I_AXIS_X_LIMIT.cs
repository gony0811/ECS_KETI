using INNO6.Core.Interlock.Interface;
using INNO6.Core.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS.Interlock.Value
{
    public class I_AXIS_X_LIMIT : AbstractExecuteInterlock
    {
        public override bool Execute(object setvalue)
        {
            FunctionManager.Instance.ABORT_FUNCTION_ALL();
            AlarmManager.Instance.SetAlarm("E9901");

            return true;
        }
    }
}
