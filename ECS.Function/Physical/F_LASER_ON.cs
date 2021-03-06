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
    public class F_LASER_ON : AbstractFunction
    {
        public override bool CanExecute()
        {
            IsAbort = false;
            IsProcessing = false;

            return this.EquipmentStatusCheck();
        }

        public override string Execute()
        {

            if (DataManager.Instance.SET_INT_DATA(IoNameHelper.OUT_INT_LASER_OPMODE_ON, 1))
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                while (true)
                {
                    Thread.Sleep(10);

                    if (IsAbort)
                    {
                        return F_RESULT_ABORT;
                    }
                    else if (stopwatch.ElapsedMilliseconds > TimeoutMiliseconds)
                    {
                        AlarmManager.Instance.SetAlarm(AlarmCodeHelper.LASER_ON_TIMEOUT);
                        return this.F_RESULT_TIMEOUT;
                    }
                    else if (/*IsCompleted()*/true)
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
                AlarmManager.Instance.SetAlarm(AlarmCodeHelper.LASER_ON_FAIL);
                return this.F_RESULT_FAIL;
            }
        }

        private bool IsCompleted()
        {
            string opModeStatus = DataManager.Instance.GET_STRING_DATA(IoNameHelper.IN_STR_LASER_OPMODE_STATUS, out bool _);

            string[] arrStatus = opModeStatus.Split(',');

            if (arrStatus != null && arrStatus.Contains("ON")) return true;
            else return false;
        }

        public override void ExecuteWhenSimulate()
        {
            DataManager.Instance.SET_STRING_DATA(IoNameHelper.IN_STR_LASER_OPMODE_STATUS, "ON");
        }

        public override void PostExecute()
        {
            DataManager.Instance.SET_INT_DATA(IoNameHelper.OUT_INT_LASER_OPMODE_ON, 0);
        }
    }
}
