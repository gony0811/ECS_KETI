using ECS.Common.Helper;
using INNO6.Core.Interlock.Interface;
using INNO6.Core.Manager;
using INNO6.IO;

namespace ECS.Interlock.SetPoint
{
    public class I_DOOR_OPEN_FRONT : IExecuteInterlock
    {
        public void Execute()
        {
            FunctionManager.Instance.ABORT_FUNCTION_ALL();
            //FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(FuncNameHelper.LASER_SHUTDOWN);
            AlarmManager.Instance.SetAlarm("E9004");
        }
    }
}
