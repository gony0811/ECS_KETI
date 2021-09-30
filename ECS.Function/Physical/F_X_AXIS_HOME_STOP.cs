using INNO6.IO;
using INNO6.IO.Service;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using INNO6.Core.Manager;

namespace ECS.Function.Physical
{
    public class F_X_AXIS_HOME_STOP : AbstractFunction
    {
        private const string IO_X_HOME_STOP = "oPMAC.iAxisX.HomeStop";
        private const string IO_X_IS_HOMMING = "iPMAC.iAxisX.IsHomming";

        private const string ALARM_X_AXIS_HOMESTOP_TIMEOUT = "E2021";
        private const string ALARM_X_AXIS_HOMESTOP_FAIL = "E2022";

        public override bool CanExecute()
        {
            IsAbort = false;
            IsProcessing = false;

            return this.EquipmentStatusCheck();
        }

        public override string Execute()
        {
            bool result = false;

            if (DataManager.Instance.SET_INT_DATA(IO_X_HOME_STOP, 1))
            {
                Thread.Sleep(1000);
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
                        AlarmManager.Instance.SetAlarm(ALARM_X_AXIS_HOMESTOP_TIMEOUT);
                        return this.F_RESULT_TIMEOUT;
                    }
                    else if (DataManager.Instance.GET_INT_DATA(IO_X_IS_HOMMING, out result) == 0)
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
            else
            {
                AlarmManager.Instance.SetAlarm(ALARM_X_AXIS_HOMESTOP_FAIL);
                return this.F_RESULT_FAIL;
            }
        }

        public override void ExecuteWhenSimulate()
        {
            DataManager.Instance.SET_INT_DATA(IO_X_IS_HOMMING, 0);
        }

        public override void PostExecute()
        {
            IsAbort = false;
            IsProcessing = false;
            if (EquipmentSimulation == OperationMode.SIMULATION.ToString())
            {
                DataManager.Instance.SET_INT_DATA(IO_X_HOME_STOP, 0);
            }
        }
    }
}
