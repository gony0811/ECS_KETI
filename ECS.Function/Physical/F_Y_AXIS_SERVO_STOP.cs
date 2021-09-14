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
    public class F_Y_AXIS_SERVO_STOP : AbstractFunction
    {
        public override bool CanExecute()
        {
            Abort = false;
            IsProcessing = false;

            return this.EquipmentStatusCheck();
        }

        public override string Execute()
        {
            bool result = false;

            if (DataManager.Instance.SET_INT_DATA(IoNameHelper.OUT_INT_PMAC_Y_SERVO_STOP, 1))
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                while (true)
                {
                    Thread.Sleep(100);

                    if (Abort)
                    {
                        return F_RESULT_ABORT;
                    }
                    else if (true/*DataManager.Instance.GET_INT_DATA(IoNameHelper.IN_INT_PMAC_Y_MOTOR_ACTIVE, out result) == 0*/)
                    {
                        return this.F_RESULT_SUCCESS;
                    }
                    else if (stopwatch.ElapsedMilliseconds > TimeoutMiliseconds)
                    {
                        AlarmManager.Instance.SetAlarm("E2042");
                        return this.F_RESULT_TIMEOUT;
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
            else
            {
                AlarmManager.Instance.SetAlarm("E2043");
                return this.F_RESULT_FAIL;
            }
        }

        public override void ExecuteWhenSimulate()
        {
            DataManager.Instance.SET_INT_DATA(IoNameHelper.IN_INT_PMAC_Y_MOTOR_ACTIVE, 0);
        }

        public override void PostExecute()
        {
            Abort = false;
            IsProcessing = false;

            DataManager.Instance.SET_INT_DATA(IoNameHelper.OUT_INT_PMAC_Y_SERVO_STOP, 0);

        }
    }
}
