using INNO6.Core.Manager;
using INNO6.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ECS.Common.Helper;

namespace ECS.Function.Physical
{
    public class F_LASER_STARTUP : AbstractFunction
    {
        public override bool CanExecute()
        {
            Abort = false;
            IsProcessing = false;

            return this.EquipmentStatusCheck();
        }

        public override string Execute()
        {

            if (true)
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
                    else if (stopwatch.ElapsedMilliseconds > TimeoutMiliseconds)
                    {
                        AlarmManager.Instance.SetAlarm(AlarmCodeHelper.LASER_STARTUP_TIMEOUT);
                        return this.F_RESULT_TIMEOUT;
                    }
                    else if (IsCompletedDeviceWarmUp())
                    {
                        Abort = false;
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
        }

        private bool IsCompletedDeviceWarmUp()
        {
            string opModeStatus = DataManager.Instance.GET_STRING_DATA(IoNameHelper.IN_STR_LASER_OPMODE_STATUS, out bool _);
            DataManager.Instance.GET_DOUBLE_DATA(IoNameHelper.IN_DBL_LASER_STATUS_TUBETEMP, out bool _);

            string[] arrStatus = opModeStatus.Split(',');

            if (arrStatus != null && arrStatus.Contains("WAIT")) return false;
            else return true;
        }

        public override void ExecuteWhenSimulate()
        {
            DataManager.Instance.SET_STRING_DATA(IoNameHelper.IN_STR_LASER_OPMODE_STATUS, "OFF");
            DataManager.Instance.SET_DOUBLE_DATA(IoNameHelper.IN_DBL_LASER_STATUS_TUBETEMP, 38.2);
        }

        public override void PostExecute()
        {

        }
    }
}
