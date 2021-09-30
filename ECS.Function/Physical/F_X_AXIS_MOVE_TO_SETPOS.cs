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

namespace ECS.Function.Physical
{
    public class F_X_AXIS_MOVE_TO_SETPOS : AbstractFunction
    {
        private const string IO_X_MOVE_TO_SETPOS = "oPMAC.iAxisX.MoveToSetPos";
        private const string IO_X_IS_MOVING = "iPMAC.iAxisX.IsMoving";

        private const string IO_DBL_X_SET_POSITION = "oPMAC.dAxisX.SetPosition";
        private const string IO_DBL_X_SET_VELOCITY = "oPMAC.dAxisX.SetVelocity";

        private const string VIO_DBL_X_ABS_POSITION = "vSet.dAxisX.AbsPosition";
        private const string VIO_DBL_X_ABS_VELOCITY = "vSet.dAxisX.AbsVelocity";

        private const string ALARM_X_AXIS_MOVE_TIMEOUT = "E2028";
        private const string ALARM_X_AXIS_MOVE_FAIL = "E2029";

        private const string IO_GET_X_POSITION = "iPMAC.dAxisX.Position";
        private const string VIO_DBL_X_INPOS_RANGE = "vSet.dAxisX.InPosRange";


        public override bool CanExecute()
        {
            IsAbort = false;
            IsProcessing = false;
            return this.EquipmentStatusCheck();
        }

        public override string Execute()
        {
            bool result = false;

            double setPosition = DataManager.Instance.GET_DOUBLE_DATA(VIO_DBL_X_ABS_POSITION, out bool _);
            double setVelocity = DataManager.Instance.GET_DOUBLE_DATA(VIO_DBL_X_ABS_VELOCITY, out bool _);

            DataManager.Instance.SET_DOUBLE_DATA(IO_DBL_X_SET_POSITION, setPosition);
            DataManager.Instance.SET_DOUBLE_DATA(IO_DBL_X_SET_VELOCITY, setVelocity);

            Thread.SpinWait(500);

            if (DataManager.Instance.SET_INT_DATA(IO_X_MOVE_TO_SETPOS, 1))
            {
                Thread.Sleep(1000);
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                IsProcessing = true;

                while (true)
                {
                    Thread.Sleep(100);

                    if (IsAbort)
                    {
                        return F_RESULT_ABORT;
                    }
                    else if (stopwatch.ElapsedMilliseconds > TimeoutMiliseconds)
                    {
                        AlarmManager.Instance.SetAlarm(ALARM_X_AXIS_MOVE_TIMEOUT);
                        return this.F_RESULT_TIMEOUT;
                    }
                    else if (InPosition(setPosition))
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

        public override void ExecuteWhenSimulate()
        {
            double setPosition = DataManager.Instance.GET_DOUBLE_DATA(VIO_DBL_X_ABS_POSITION, out bool _);
            DataManager.Instance.SET_DOUBLE_DATA(IO_GET_X_POSITION, setPosition);
        }
        public override void PostExecute()
        {
            IsAbort = false;
            IsProcessing = false;
            DataManager.Instance.SET_INT_DATA(IO_X_MOVE_TO_SETPOS, 0);
        }

        private bool InPosition(double targetPos)
        {
            double curPos = DataManager.Instance.GET_DOUBLE_DATA(IO_GET_X_POSITION, out bool _);
            double inPosRange = DataManager.Instance.GET_DOUBLE_DATA(VIO_DBL_X_INPOS_RANGE, out bool _);

            double highLimit = targetPos + inPosRange;
            double lowLimit = targetPos - inPosRange;

            if (highLimit >= curPos && lowLimit <= curPos) return true;
            else return false;
        }
    }
}
