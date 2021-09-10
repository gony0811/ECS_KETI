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
    public class F_LED_CH1_ON : AbstractFunction
    {
        private const string ALARM_LED_CH1_ON_TIMEOUT = "E1003";
        private const string ALARM_LED_CH1_ON_FAIL = "E1004";

        private const string IO_NAME_CH1_LED_ONOFF = "oLed.iOnOff.Ch1";
        private const string IO_NAME_CH1_LED_ONOFF_STATUS = "iLed.iOnOff.Ch1";

        public override bool CanExecute()
        {
            Abort = false;
            IsProcessing = false;

            return this.EquipmentStatusCheck();
        }

        public override string Execute()
        {
            bool result = false;

            if (DataManager.Instance.SET_INT_DATA(IO_NAME_CH1_LED_ONOFF, 1))
            {

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
                        AlarmManager.Instance.SetAlarm(ALARM_LED_CH1_ON_TIMEOUT);
                        return this.F_RESULT_TIMEOUT;
                    }
                    else if (DataManager.Instance.GET_INT_DATA(IO_NAME_CH1_LED_ONOFF_STATUS, out result) == 1)
                    {
                        Abort = false;
                        IsProcessing = false;
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
            else
            {
                AlarmManager.Instance.SetAlarm(ALARM_LED_CH1_ON_FAIL);
                return this.F_RESULT_FAIL;
            }
        }

        public override void ExecuteWhenSimulate()
        {
            DataManager.Instance.SET_INT_DATA(IO_NAME_CH1_LED_ONOFF_STATUS, 1);
        }

        public override void PostExecute()
        {
            if (EquipmentSimulation == OperationMode.SIMULATION.ToString())
            {
                DataManager.Instance.SET_INT_DATA(IO_NAME_CH1_LED_ONOFF, 0);
            }
        }
    }
}
