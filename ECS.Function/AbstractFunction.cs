using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using ECS.Common.Helper;
using INNO6.Core.Function.Interface;
using INNO6.Core.Manager;
using INNO6.IO;

namespace ECS.Function
{
    public abstract class AbstractFunction : IFunction
    {
        public enum OperationMode
        {
            UNKNOWN,
            NORMAL,
            SIMULATION
        }

        public enum Alarm
        {
            OFF,
            ON
        }

        public enum Interlock
        {
            UNKNOWN,
            ON,
            OFF,
        }

        public delegate bool ExecuteFunction();
        public delegate bool SuccessCondition();

        private const string ALARM_EQSTATUS_NOT_READY = "E9001";

        public readonly string F_RESULT_SUCCESS = "SUCCESS";
        public readonly string F_RESULT_FAIL = "FAIL";
        public readonly string F_RESULT_TIMEOUT = "TIMEOUT";
        public readonly string F_RESULT_ABORT = "ABORT";

        public Stopwatch StopWatch;

        public int ProgressRate { get; protected set; }
        public string ProcessingMessage { get; protected set; }
        public bool IsProcessing { get; protected set; }
        public bool Abort { get; set; }
        public int TimeoutMiliseconds { get; set; }

        public string EquipmentSimulation { get; set; }

        public string FunctionResult { get; set; }

        public AbstractFunction()
        {
            Abort = false;
            IsProcessing = false;
            TimeoutMiliseconds = 10000;
            StopWatch = new Stopwatch();

            EquipmentSimulation = DataManager.Instance.GET_STRING_DATA(IoNameHelper.V_STR_SYS_SIMULATION_MODE, out bool result);

            DataManager.Instance.DataAccess.DataChangedEvent += DataAccess_DataChanged;

            if (!result) EquipmentSimulation = "UNKNOWN";
        }

        public virtual void ProgressUpdate(int progress, string message)
        {
            int rate = ProgressRate + progress;
            ProcessingMessage = message;

            if (rate >= 100) ProgressRate = 100;
            else ProgressRate = rate;
        }

        public virtual void DataAccess_DataChanged(object sender, DataChangedEventHandlerArgs args)
        {
            Data data = args.Data;

            if (data.Name.Equals(IoNameHelper.V_STR_SYS_SIMULATION_MODE))
            {
                EquipmentSimulation = (string)data.Value;
            }
        }

        public virtual int CalcurateProgressRate(int currentElapsedTime, int totalTime, bool complited = false)
        {
            if (complited) return 100;

            double cal = ((double)currentElapsedTime / (double)TimeoutMiliseconds) * 100;

            int progress = Convert.ToInt32(Math.Round(cal, 1));

            return progress;
        }

        public virtual int CalcurateProgressRate(long currentElapsedMilliseconds, bool complited = false)
        {
            if (complited) return 100;

            double cal = ((double)currentElapsedMilliseconds / (double)TimeoutMiliseconds) *100;

            int progress = Convert.ToInt32(Math.Round(cal,1));

            return progress;
        }

        public virtual bool CanExecute()
        {
            Abort = false;
            IsProcessing = false;
            EquipmentSimulation = DataManager.Instance.GET_STRING_DATA(IoNameHelper.V_STR_SYS_SIMULATION_MODE, out bool _);
            return this.EquipmentStatusCheck();
        }

        public virtual string Sequence(ExecuteFunction execute, SuccessCondition condition, int timeoutMilliseconds)
        {
            Stopwatch.StartNew();
      
            if (execute())
            {
                while(true)
                {
                    if(Abort)
                    {
                        return F_RESULT_ABORT;
                    }
                    else if(condition())
                    {
                        ProgressRate = CalcurateProgressRate(StopWatch.ElapsedMilliseconds, true);
                        return F_RESULT_SUCCESS;
                    }
                    else if (StopWatch.ElapsedMilliseconds > TimeoutMiliseconds)
                    {
                        ProgressRate = CalcurateProgressRate(StopWatch.ElapsedMilliseconds, true);
                        return this.F_RESULT_TIMEOUT;
                    }
                    else if (EquipmentSimulation == OperationMode.SIMULATION.ToString())
                    {
                        ExecuteWhenSimulate();
                    }
                    else
                    {
                        IsProcessing = true;
                        ProgressRate = CalcurateProgressRate(StopWatch.ElapsedMilliseconds);
                        continue;
                    }
                }
            }
            else
            {
                return F_RESULT_FAIL;
            }
        }

        public abstract string Execute();

        public abstract void PostExecute();

        protected bool EquipmentStatusCheck()
        {
            bool result = true;
            bool check = false;

            EquipmentSimulation = DataManager.Instance.GET_STRING_DATA(IoNameHelper.V_STR_SYS_SIMULATION_MODE, out result);

            int alarmStatus = DataManager.Instance.GET_INT_DATA(IoNameHelper.V_INT_SYS_EQP_ALARM, out check);
            result &= check;

            int interlockStatus = DataManager.Instance.GET_INT_DATA(IoNameHelper.V_INT_SYS_EQP_INTERLOCK, out check);
            result &= check;

            if (result == true && alarmStatus == (int)Alarm.OFF && interlockStatus == (int)Interlock.OFF)
            {
                return true;
            }
            else
            {
                AlarmManager.Instance.SetAlarm(ALARM_EQSTATUS_NOT_READY);
                return false;
            }
        }

        public virtual void ExecuteWhenSimulate()
        {

        }
    }
}
