using ECS.Common.Helper;
using INNO6.Core.Manager;
using INNO6.Core.Manager.Model;
using INNO6.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ECS.Function.Operation
{
    public class OP_INIT_PROCESS : AbstractFunction
    {
        public override string Execute()
        {
            string result = F_RESULT_SUCCESS;
           
            IsProcessing = true;
            ProgressRate = CalcurateProgressRate(StopWatch.ElapsedMilliseconds);

            FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC("F_MOVE_VISION_POSITION");
            FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC("F_SET_MODE_EGYNGR");
            FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC("F_LASER_STANDBY");

            StopWatch.Restart();

            while(true)
            {
                if(Abort)
                {
                    FunctionManager.Instance.ABORT_FUNCTION("F_MOVE_VISION_POSITION");
                    FunctionManager.Instance.ABORT_FUNCTION("F_SET_MODE_EGYNGR");
                    FunctionManager.Instance.ABORT_FUNCTION("F_LASER_STANDBY");

                    return F_RESULT_ABORT;
                }
                else if(!FunctionManager.Instance.CHECK_EXECUTING_FUNCTION_EXSIST("F_MOVE_VISION_POSITION") 
                    && !FunctionManager.Instance.CHECK_EXECUTING_FUNCTION_EXSIST("F_SET_MODE_EGYNGR")
                    && !FunctionManager.Instance.CHECK_EXECUTING_FUNCTION_EXSIST("F_LASER_STANDBY")
                    )
                {
                    return F_RESULT_SUCCESS;
                }
                else
                {
                    Thread.Sleep(100);
                    ProgressRate = CalcurateProgressRate(StopWatch.ElapsedMilliseconds);
                }
            }

            return F_RESULT_FAIL;


            //if ((result = FunctionManager.Instance.EXECUTE_FUNCTION_SYNC("F_MOVE_VISION_POSITION")) != F_RESULT_SUCCESS) return result;

            //ProgressRate = CalcurateProgressRate(StopWatch.ElapsedMilliseconds);

            //if ((result = FunctionManager.Instance.EXECUTE_FUNCTION_SYNC("F_SET_MODE_EGYNGR")) != F_RESULT_SUCCESS) return result;

            //ProgressRate = CalcurateProgressRate(StopWatch.ElapsedMilliseconds);

            //if ((result = FunctionManager.Instance.EXECUTE_FUNCTION_SYNC("F_LASER_STANDBY")) != F_RESULT_SUCCESS) return result;

            //ProgressRate = CalcurateProgressRate(StopWatch.ElapsedMilliseconds, true);

            //if(result == F_RESULT_SUCCESS)
            //{
            //    DataManager.Instance.SET_STRING_DATA(IoNameHelper.V_STR_SYS_OPERATION_MODE, "AUTO");
            //}

            //return result;
        }

        public override void PostExecute()
        {
            //throw new NotImplementedException();
        }
    }
}
