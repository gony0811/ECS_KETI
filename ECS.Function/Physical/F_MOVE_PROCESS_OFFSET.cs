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
    public class F_MOVE_PROCESS_OFFSET : AbstractFunction
    {
        private string F_X_AXIS_MOVE_TO_SETDIS = "F_X_AXIS_MOVE_TO_SETDIS";
        private string F_Y_AXIS_MOVE_TO_SETDIS = "F_Y_AXIS_MOVE_TO_SETDIS";

        public override string Execute()
        {
            double visionPosX = DataManager.Instance.GET_DOUBLE_DATA(IoNameHelper.V_DBL_SET_X_VISION_POSITION, out bool _);
            double visionPosY = DataManager.Instance.GET_DOUBLE_DATA(IoNameHelper.V_DBL_SET_Y_VISION_POSITION, out bool _);

            double processPosX = DataManager.Instance.GET_DOUBLE_DATA(IoNameHelper.V_DBL_SET_X_PROCESS_POSITION, out bool _);
            double processPosY = DataManager.Instance.GET_DOUBLE_DATA(IoNameHelper.V_DBL_SET_Y_PROCESS_POSITION, out bool _);

            double dffsetX = processPosX - visionPosX;
            double offsetY = processPosY - visionPosY;

            DataManager.Instance.SET_DOUBLE_DATA(IoNameHelper.V_DBL_SET_X_REL_DISTANCE, dffsetX);
            DataManager.Instance.SET_DOUBLE_DATA(IoNameHelper.V_DBL_SET_Y_REL_DISTANCE, offsetY);

            FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(F_X_AXIS_MOVE_TO_SETDIS);
            FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(F_Y_AXIS_MOVE_TO_SETDIS);

            Stopwatch stopwatch = Stopwatch.StartNew();

            while (true)
            {
                Thread.Sleep(100);

                if (IsAbort)
                {
                    FunctionManager.Instance.ABORT_FUNCTION(F_X_AXIS_MOVE_TO_SETDIS);
                    FunctionManager.Instance.ABORT_FUNCTION(F_Y_AXIS_MOVE_TO_SETDIS);
                    return F_RESULT_ABORT;
                }
                else if (stopwatch.ElapsedMilliseconds > TimeoutMiliseconds)
                {
                    return this.F_RESULT_TIMEOUT;
                }
                else if (!FunctionManager.Instance.CHECK_EXECUTING_FUNCTION_EXSIST(F_X_AXIS_MOVE_TO_SETDIS) &&
                    !FunctionManager.Instance.CHECK_EXECUTING_FUNCTION_EXSIST(F_Y_AXIS_MOVE_TO_SETDIS))
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
    }
}

