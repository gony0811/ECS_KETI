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
        private bool visionPositionMoveResult = false;
        private bool ModeEgyNgrSetResult = false;
        private bool laserStandByResult = false;

        public override string Execute()
        {
            string result = F_RESULT_SUCCESS;
           
            IsProcessing = true;
            ProgressRate = CalcurateProgressRate(StopWatch.ElapsedMilliseconds);

            FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC("F_MOVE_VISION_POSITION", this.ExecuteFunctionResult);
            FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC("F_SET_MODE_EGYNGR", this.ExecuteFunctionResult);
            FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC("F_LASER_STANDBY", this.ExecuteFunctionResult);

            StopWatch.Restart();

            while (true)
            {
                if (IsAbort)
                {
                    FunctionManager.Instance.ABORT_FUNCTION("F_MOVE_VISION_POSITION");
                    FunctionManager.Instance.ABORT_FUNCTION("F_SET_MODE_EGYNGR");
                    FunctionManager.Instance.ABORT_FUNCTION("F_LASER_STANDBY");

                    return F_RESULT_ABORT;
                }
                else if (!FunctionManager.Instance.CHECK_EXECUTING_FUNCTION_EXSIST("F_MOVE_VISION_POSITION")
                    && !FunctionManager.Instance.CHECK_EXECUTING_FUNCTION_EXSIST("F_SET_MODE_EGYNGR")
                    && !FunctionManager.Instance.CHECK_EXECUTING_FUNCTION_EXSIST("F_LASER_STANDBY")
                    )
                {
                    if (visionPositionMoveResult && ModeEgyNgrSetResult && laserStandByResult)
                        return F_RESULT_SUCCESS;
                    else
                        return F_RESULT_FAIL;
                }
                else if (StopWatch.ElapsedMilliseconds > TimeoutMiliseconds)
                {
                    return F_RESULT_TIMEOUT;
                }
                else
                {
                    Thread.Sleep(100);
                    ProgressRate = CalcurateProgressRate(StopWatch.ElapsedMilliseconds);
                }
            }
        }

        public override void PostExecute()
        {
            //throw new NotImplementedException();
        }


        private void ExecuteFunctionResult(string executeName, object state)
        {
            string result = state as string;

            switch(executeName)
            {
                case "F_MOVE_VISION_POSITION":
                    {
                        if (result == F_RESULT_SUCCESS)
                            visionPositionMoveResult = true;
                        else
                            visionPositionMoveResult = false;
                    }
                    break;
                case "F_SET_MODE_EGYNGR":
                    {
                        if (result == F_RESULT_SUCCESS)
                            ModeEgyNgrSetResult = true;
                        else
                            ModeEgyNgrSetResult = false;
                    }
                    break;
                case "F_LASER_STANDBY":
                    {
                        if (result == F_RESULT_SUCCESS)
                            laserStandByResult = true;
                        else
                            laserStandByResult = false;
                    }
                    break;
            }
        }
    }
}
