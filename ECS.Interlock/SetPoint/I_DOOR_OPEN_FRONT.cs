using ECS.Common.Helper;
using INNO6.Core.Interlock.Interface;
using INNO6.Core.Manager;
using INNO6.IO;

namespace ECS.Interlock.SetPoint
{
    public class I_DOOR_OPEN_FRONT : AbstractExecuteInterlock
    {
        public override bool Execute(object setvalue)
        {
            FunctionManager.Instance.ABORT_FUNCTION_ALL();
            //FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(FuncNameHelper.LASER_SHUTDOWN);
            AlarmManager.Instance.SetAlarm("E9004");

            return true;
        }
    }
}
