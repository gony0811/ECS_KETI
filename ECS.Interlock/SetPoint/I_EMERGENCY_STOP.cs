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
    public class I_EMERGENCY_STOP : IExecuteInterlock
    {
        public void Execute()
        {
            FunctionManager.Instance.ABORT_FUNCTION_ALL();

            FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(FuncNameHelper.LASER_SHUTDOWN);

            DataManager.Instance.SET_INT_DATA(IoNameHelper.V_INT_SYS_EQP_INTERLOCK, 1);         
            AlarmManager.Instance.SetAlarm("E9999");

            
        }
    }
}
