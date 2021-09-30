using ECS.Common.Helper;
using INNO6.Core.Manager;
using INNO6.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ECS.Function.Physical
{
    public class F_MOVE_PROCESS_POSITION : AbstractFunction
    {
        public override bool CanExecute()
        {
            IsAbort = false;
            IsProcessing = false;

            return this.EquipmentStatusCheck();
        }
        public override string Execute()
        {
            double processPosX = DataManager.Instance.GET_DOUBLE_DATA(IoNameHelper.V_DBL_SET_X_PROCESS_POSITION, out _);
            double processPosY = DataManager.Instance.GET_DOUBLE_DATA(IoNameHelper.V_DBL_SET_Y_PROCESS_POSITION, out _);

            DataManager.Instance.SET_DOUBLE_DATA(IoNameHelper.V_DBL_SET_X_ABS_POSITION, processPosX);
            DataManager.Instance.SET_DOUBLE_DATA(IoNameHelper.V_DBL_SET_Y_ABS_POSITION, processPosY);

          
            FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(FuncNameHelper.X_AXIS_MOVE_TO_SETPOS);
            FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(FuncNameHelper.Y_AXIS_MOVE_TO_SETPOS);

            Stopwatch stopwatch = Stopwatch.StartNew();

            while (true)
            {
                Thread.Sleep(100);

                if (IsAbort)
                {
                    FunctionManager.Instance.ABORT_FUNCTION(FuncNameHelper.X_AXIS_MOVE_TO_SETPOS);
                    FunctionManager.Instance.ABORT_FUNCTION(FuncNameHelper.Y_AXIS_MOVE_TO_SETPOS);

                    return F_RESULT_ABORT;
                }
                else if (stopwatch.ElapsedMilliseconds > TimeoutMiliseconds)
                {
                    return this.F_RESULT_TIMEOUT;
                }
                else if (!FunctionManager.Instance.CHECK_EXECUTING_FUNCTION_EXSIST(FuncNameHelper.X_AXIS_MOVE_TO_SETPOS)
                         && !FunctionManager.Instance.CHECK_EXECUTING_FUNCTION_EXSIST(FuncNameHelper.Y_AXIS_MOVE_TO_SETPOS)
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
    }
}
