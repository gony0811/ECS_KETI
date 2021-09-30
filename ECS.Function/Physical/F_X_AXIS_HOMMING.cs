using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using INNO6.Core.Function;
using INNO6.IO;
using ECS.Function;
using System.Diagnostics;
using System.Threading;
using INNO6.Core.Manager;
using ECS.Common.Helper;

namespace ECS.Function.Physical
{
    public class F_X_AXIS_HOMMING : AbstractFunction
    {
        private const string ALARM_X_AXIS_HOMMING_TIMEOUT = "E2023";
        private const string ALARM_X_AXIS_HOMMING_FAIL = "E2024";



        public override bool CanExecute()
        {
            IsAbort = false;
            IsProcessing = false;
            EquipmentSimulation = DataManager.Instance.GET_STRING_DATA(IoNameHelper.V_STR_SYS_SIMULATION_MODE, out bool _);
            return this.EquipmentStatusCheck();
        }

        public override string Execute()
        {
            bool result = false;

            if (DataManager.Instance.SET_INT_DATA(IoNameHelper.OUT_INT_PMAC_X_HOMMING, 1))
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
                        AlarmManager.Instance.SetAlarm(ALARM_X_AXIS_HOMMING_TIMEOUT);
                        return this.F_RESULT_TIMEOUT;
                    }
                    else if (DataManager.Instance.GET_INT_DATA(IoNameHelper.IN_INT_PMAC_X_ISHOME, out result) == 1)
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
                AlarmManager.Instance.SetAlarm(ALARM_X_AXIS_HOMMING_FAIL);
                return this.F_RESULT_FAIL;
            }
        }

        public override void ExecuteWhenSimulate()
        {
            DataManager.Instance.SET_INT_DATA(IoNameHelper.IN_INT_PMAC_X_ISHOME, 1);
        }

        public override void PostExecute()
        {
            IsAbort = false;
            IsProcessing = false;
            DataManager.Instance.SET_INT_DATA(IoNameHelper.OUT_INT_PMAC_X_HOMMING, 0);
            DataManager.Instance.SET_INT_DATA(IoNameHelper.IN_INT_PMAC_X_ISHOME, 0);
        }
    }
}
