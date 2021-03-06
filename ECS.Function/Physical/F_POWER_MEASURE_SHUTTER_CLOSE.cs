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
    public class F_POWER_MEASURE_SHUTTER_CLOSE : AbstractFunction
    {
        public override bool CanExecute()
        {
            IsAbort = false;
            IsProcessing = false;

            return this.EquipmentStatusCheck();
        }

        public override string Execute()
        {
            if (DataManager.Instance.SET_INT_DATA(IoNameHelper.OUT_INT_PMAC_SHUTTER_FORWARD, 1)
                && DataManager.Instance.SET_INT_DATA(IoNameHelper.OUT_INT_PMAC_SHUTTER_BACKWARD, 0))
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                while (true)
                {
                    Thread.Sleep(100);

                    if (IsAbort)
                    {
                        return F_RESULT_ABORT;
                    }
                    else if (stopwatch.ElapsedMilliseconds > TimeoutMiliseconds)
                    {
                        AlarmManager.Instance.SetAlarm(AlarmCodeHelper.POWER_METER_SHUTTER_FORWARD_TIMEOUT);
                        return this.F_RESULT_TIMEOUT;
                    }
                    else if (true/*DataManager.Instance.GET_INT_DATA(IoNameHelper.IN_INT_PMAC_SHUTTER_FORWARD_STATUS, out _) == 1*/)
                    {
                        IsAbort = false;
                        IsProcessing = false;
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
            else
            {
                AlarmManager.Instance.SetAlarm(AlarmCodeHelper.POWER_METER_SHUTTER_FORWARD_FAIL);
                return this.F_RESULT_FAIL;
            }
        }

        public override void ExecuteWhenSimulate()
        {
            DataManager.Instance.SET_INT_DATA(IoNameHelper.IN_INT_PMAC_SHUTTER_FORWARD_STATUS, 1);
        }

        public override void PostExecute()
        {
            DataManager.Instance.SET_INT_DATA(IoNameHelper.IN_INT_PMAC_SHUTTER_FORWARD_STATUS, 0);
            DataManager.Instance.SET_INT_DATA(IoNameHelper.OUT_INT_PMAC_SHUTTER_FORWARD, 0);
        }
    }
}
