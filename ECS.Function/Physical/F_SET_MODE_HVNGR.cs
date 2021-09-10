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
    public class F_SET_MODE_HVNGR : AbstractFunction
    {
        public override bool CanExecute()
        {
            Abort = false;
            IsProcessing = false;

            return this.EquipmentStatusCheck();
        }

        public override string Execute()
        {

            if (DataManager.Instance.SET_INT_DATA(IoNameHelper.OUT_INT_LASER_EGYMODE_HVNGR, 1))
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
                        AlarmManager.Instance.SetAlarm(AlarmCodeHelper.ALARM_MODE_SET_HVNGR_TIMEOUT);
                        return this.F_RESULT_TIMEOUT;
                    }
                    else if (IsCompleted())
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
            else
            {
                AlarmManager.Instance.SetAlarm(AlarmCodeHelper.ALARM_MODE_SET_HVNGR_FAIL);
                return this.F_RESULT_FAIL;
            }
        }

        private bool IsCompleted()
        {
            string mode = DataManager.Instance.GET_STRING_DATA(IoNameHelper.IN_STR_LASER_EGYMODE_STATUS, out bool _);

            if (!string.IsNullOrEmpty(mode) && mode.Contains("HV NGR")) return true;
            else return false;
        }

        public override void ExecuteWhenSimulate()
        {
            DataManager.Instance.SET_STRING_DATA(IoNameHelper.IN_STR_LASER_EGYMODE_STATUS, "HV NGR");
        }

        public override void PostExecute()
        {
            DataManager.Instance.SET_INT_DATA(IoNameHelper.OUT_INT_LASER_EGYMODE_HVNGR, 0);
        }
    }
}
