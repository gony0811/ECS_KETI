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
    public class F_MOVE_MOTION_STOP : AbstractFunction
    {
        public override bool CanExecute()
        {
            Abort = false;
            IsProcessing = false;

            return this.EquipmentStatusCheck();
        }

        public override string Execute()
        {
            DataManager.Instance.SET_INT_DATA(IoNameHelper.OUT_INT_PMAC_ALL_MOVE_STOP, 1);

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
                else if (DataManager.Instance.GET_DOUBLE_DATA(IoNameHelper.IN_DBL_PMAC_X_VELOCITY, out _) == 0.0
                    && DataManager.Instance.GET_DOUBLE_DATA(IoNameHelper.IN_DBL_PMAC_Y_VELOCITY, out _) == 0.0)
                {
                    return this.F_RESULT_SUCCESS;
                }
                else if (EquipmentSimulation == OperationMode.SIMULATION.ToString())
                {
                    ExecuteWhenSimulate();
                }
                else
                {
                    IsProcessing = true;
                    continue;
                }
            }
        }

        public override void ExecuteWhenSimulate()
        {
            DataManager.Instance.SET_DOUBLE_DATA(IoNameHelper.IN_DBL_PMAC_X_VELOCITY, 0.0);
            DataManager.Instance.SET_DOUBLE_DATA(IoNameHelper.IN_DBL_PMAC_Y_VELOCITY, 0.0);
        }

        public override void PostExecute()
        {
            DataManager.Instance.SET_INT_DATA(IoNameHelper.OUT_INT_PMAC_ALL_MOVE_STOP, 0);
        }
    }
}
