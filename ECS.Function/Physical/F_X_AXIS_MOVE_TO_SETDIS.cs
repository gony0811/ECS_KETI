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
    public class F_X_AXIS_MOVE_TO_SETDIS : AbstractFunction
    {
        private const string ALARM_X_AXIS_MOVE_TIMEOUT = "E2028";
        private const string ALARM_X_AXIS_MOVE_FAIL = "E2029";

        private double _TargetPosition;

        public override bool CanExecute()
        {
            Abort = false;
            IsProcessing = false;
            return this.EquipmentStatusCheck();
        }

        public override string Execute()
        {
            bool result = false;

            double startPostion = DataManager.Instance.GET_DOUBLE_DATA(IoNameHelper.IN_DBL_PMAC_X_POSITION, out bool _);
            double setDistance = DataManager.Instance.GET_DOUBLE_DATA(IoNameHelper.V_DBL_SET_X_REL_DISTANCE, out bool _);
            double setVelocity = DataManager.Instance.GET_DOUBLE_DATA(IoNameHelper.V_DBL_SET_X_REL_VELOCITY, out bool _);

            _TargetPosition = startPostion + setDistance;

            DataManager.Instance.SET_DOUBLE_DATA(IoNameHelper.OUT_DBL_PMAC_X_SETDISTANCE, setDistance);
            DataManager.Instance.SET_DOUBLE_DATA(IoNameHelper.OUT_DBL_PMAC_X_SETVELOCITY, setVelocity);

            Thread.SpinWait(500);

            if (DataManager.Instance.SET_INT_DATA(IoNameHelper.OUT_INT_PMAC_X_MOVETOSETDIS, 1))
            {
                //Thread.Sleep(1000);
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                IsProcessing = true;

                while (true)
                {
                    Thread.Sleep(100);

                    if (Abort)
                    {
                        return F_RESULT_ABORT;
                    }
                    else if (stopwatch.ElapsedMilliseconds > TimeoutMiliseconds)
                    {
                        AlarmManager.Instance.SetAlarm(ALARM_X_AXIS_MOVE_TIMEOUT);
                        return this.F_RESULT_TIMEOUT;
                    }
                    else if (InPosition(_TargetPosition))
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
                AlarmManager.Instance.SetAlarm(ALARM_X_AXIS_MOVE_FAIL);
                return this.F_RESULT_FAIL;
            }
        }

        private bool InPosition(double targetPos)
        {
            double curPos = DataManager.Instance.GET_DOUBLE_DATA(IoNameHelper.IN_DBL_PMAC_X_POSITION, out bool _);
            double inPosRange = DataManager.Instance.GET_DOUBLE_DATA(IoNameHelper.V_DBL_SET_X_INPOS_RANGE, out bool _);

            double highLimit = targetPos + inPosRange;
            double lowLimit = targetPos - inPosRange;

            if (highLimit >= curPos && lowLimit <= curPos) return true;
            else return false;
        }

        public override void ExecuteWhenSimulate()
        {
            DataManager.Instance.SET_DOUBLE_DATA(IoNameHelper.IN_DBL_PMAC_X_POSITION, _TargetPosition);
        }
        public override void PostExecute()
        {
            Abort = false;
            IsProcessing = false;
            DataManager.Instance.SET_INT_DATA(IoNameHelper.OUT_INT_PMAC_X_MOVETOSETDIS, 0);
        }
    }
}
