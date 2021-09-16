using ECS.Common.Helper;
using INNO6.Core.Interlock.Interface;
using INNO6.Core.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS.Interlock.SetPoint
{
    public class I_EMO_SWITCH_BACK : IExecuteInterlock
    {
        public void Execute()
        {
            FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(FuncNameHelper.LASER_SHUTDOWN);
            FunctionManager.Instance.ABORT_FUNCTION_ALL();
            FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(FuncNameHelper.X_AXIS_SERVO_STOP);
            FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(FuncNameHelper.Y_AXIS_SERVO_STOP);
            AlarmManager.Instance.SetAlarm(AlarmCodeHelper.EMO_SWITCH_BACK_INTERLOCK);
        }
    }
}
