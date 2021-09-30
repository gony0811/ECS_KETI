using ECS.Common.Helper;
using INNO6.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using ECS.Recipe;
using ECS.Recipe.Model;
using ECS.Recipe.Comparer;
using INNO6.Core.Manager;

namespace ECS.Function.Operation
{
    public class OP_AUTO_PROCESS : AbstractFunction
    {
        public override bool CanExecute()
        {
            bool result = true;
            result &= base.CanExecute();
            result &= (DataManager.Instance.GET_STRING_DATA(IoNameHelper.V_STR_SYS_OPERATION_MODE, out bool _) == "AUTO");

            return result;
        }

        public override string Execute()
        {
            int TotalProcessTime = RecipeManager.Instance.GET_RECIPE_TOTALWORKTIME();
            ProgressRate = 0;
            long lastElapsed = 0;
            bool result = true;
            List<RECIPE_STEP> stepList = RecipeManager.Instance.GET_RECIPE_STEP_LIST();

            stepList.Sort(new RecipeStepIdComparer());

            foreach(RECIPE_STEP step in stepList)
            {
                ProcessingMessage = string.Format("Processing... STEP ID={0}", step.STEP_ID);

                switch (step.EGY_MODE)
                {
                    case "EGY NGR":
                        result &= FunctionManager.Instance.EXECUTE_FUNCTION_SYNC(FuncNameHelper.SET_MODE_EGYNGR) == F_RESULT_SUCCESS;                       
                        break;
                    case "HV NGR":
                        result &= FunctionManager.Instance.EXECUTE_FUNCTION_SYNC(FuncNameHelper.SET_MODE_HVNGR) == F_RESULT_SUCCESS;
                        break;
                    case "EGYBURST NGR":
                        result &= FunctionManager.Instance.EXECUTE_FUNCTION_SYNC(FuncNameHelper.SET_MODE_HVNGR) == F_RESULT_SUCCESS;
                        break;
                    default:
                        result = false;
                        break;
                }

                if (!result) return F_RESULT_FAIL;

                result &= DataManager.Instance.SET_DOUBLE_DATA(IoNameHelper.OUT_DBL_PMAC_X_SETPOSITION, step.X_POSITION);
                result &= DataManager.Instance.SET_DOUBLE_DATA(IoNameHelper.OUT_DBL_PMAC_Y_SETPOSITION, step.Y_POSITION);

                if (!result) return F_RESULT_FAIL;

                result &= FunctionManager.Instance.EXECUTE_FUNCTION_SYNC(FuncNameHelper.X_AXIS_MOVE_TO_SETPOS) == F_RESULT_SUCCESS;
                if (!result) return F_RESULT_FAIL;
                result &= FunctionManager.Instance.EXECUTE_FUNCTION_SYNC(FuncNameHelper.LASER_ON) == F_RESULT_SUCCESS;
                if (!result) return F_RESULT_FAIL;

                StopWatch.Restart();
                lastElapsed = StopWatch.ElapsedMilliseconds;

                while (result)
                {                  
                    if(IsAbort)
                    {

                        return F_RESULT_ABORT;
                    }
                    else if(StopWatch.Elapsed.TotalMilliseconds >= (step.PROCESS_TIME*1000))
                    {           
                        
                        result &= FunctionManager.Instance.EXECUTE_FUNCTION_SYNC(FuncNameHelper.LASER_STANDBY) == F_RESULT_SUCCESS;
                        if (!result) return F_RESULT_FAIL;
                        break;
                    }
                    else
                    {
                        Thread.Sleep(10);

                        double one = (TotalProcessTime*1000) / 100;

                        if((StopWatch.ElapsedMilliseconds - lastElapsed) >= (one))
                        {
                            lastElapsed = StopWatch.ElapsedMilliseconds;
                            ProgressRate += 1;
                        }                        

                        continue;
                    }    
                }

                if (!result)
                {
                    AlarmManager.Instance.SetAlarm("E5001");
                    return F_RESULT_FAIL;
                }
            }

            return F_RESULT_SUCCESS;
        }

        public override void PostExecute()
        {
            ProgressRate = 100;
        }
    }
}
