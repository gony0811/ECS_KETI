using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ECS.Common.Helper;
using INNO6.Core.Manager;
using INNO6.IO;

namespace ECS.Function.Physical
{
    public class F_MOVE_VISION_POSITION : AbstractFunction
    {
        private string F_X_AXIS_MOVE_TO_SETPOS = "F_X_AXIS_MOVE_TO_SETPOS";
        private string F_Y_AXIS_MOVE_TO_SETPOS = "F_Y_AXIS_MOVE_TO_SETPOS";


        public override bool CanExecute()
        {
            Abort = false;
            IsProcessing = false;

            return this.EquipmentStatusCheck();
        }

        public override string Execute()
        {
            bool result = true;

            double visionPosX = DataManager.Instance.GET_DOUBLE_DATA(IoNameHelper.V_DBL_SET_X_VISION_POSITION, out result);

            if (!result) return F_RESULT_FAIL;

            double visionPosY = DataManager.Instance.GET_DOUBLE_DATA(IoNameHelper.V_DBL_SET_Y_VISION_POSITION, out result);

            if (!result) return F_RESULT_FAIL;

            DataManager.Instance.SET_DOUBLE_DATA(IoNameHelper.V_DBL_SET_X_ABS_POSITION, visionPosX);
            DataManager.Instance.SET_DOUBLE_DATA(IoNameHelper.V_DBL_SET_Y_ABS_POSITION, visionPosY);

            FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(F_X_AXIS_MOVE_TO_SETPOS);
            FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(F_Y_AXIS_MOVE_TO_SETPOS);

            Stopwatch stopwatch = Stopwatch.StartNew();

            while (true)
            {

                Thread.Sleep(100);

                if (Abort)
                {
                    return F_RESULT_ABORT;
                }
                else if (stopwatch.ElapsedMilliseconds > TimeoutMiliseconds)
                {
                    return this.F_RESULT_TIMEOUT;
                }
                else if (!FunctionManager.Instance.CHECK_EXECUTING_FUNCTION_EXSIST(F_X_AXIS_MOVE_TO_SETPOS) &&
                    !FunctionManager.Instance.CHECK_EXECUTING_FUNCTION_EXSIST(F_Y_AXIS_MOVE_TO_SETPOS)
                    )
                {
                    return this.F_RESULT_SUCCESS;
                }
                else if (EquipmentSimulation == OperationMode.SIMULATION.ToString())
                {

                }
                else
                {
                    IsProcessing = true;
                    continue;
                }
            }
        }

        public override void PostExecute()
        {

        }

        public override void ExecuteWhenSimulate()
        {

        }
    }
}

