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

    public class F_Y_AXIS_JOG_MINUS : AbstractFunction
    {
        private const string IO_Y_JOG_MINUS = "oPMAC.iAxisY.JogBwd";
        private const string ALARM_Y_AXIS_JOG_MINUS_FAIL = "E2035";

        private const string IO_Y_JOG_PLUS = "oPMAC.iAxisY.JogFwd";

        private const string VIO_Y_JOG_SPEED_MODE = "vSet.sAxisY.JogVelMode";
        private const string VIO_Y_JOG_SPEED_HIGH = "vSet.dAxisY.JogVelHigh";
        private const string VIO_Y_JOG_SPEED_LOW = "vSet.dAxisY.JogVelLow";

        private const string IO_Y_JOG_VELOCITY_SET = "oPMAC.dAxisY.JogVel";

        public override bool CanExecute()
        {
            return this.EquipmentStatusCheck();
        }

        public override string Execute()
        {
            if (DataManager.Instance.GET_STRING_DATA(IoNameHelper.V_STR_SET_Y_JOGVEL_MODE, out bool _).StartsWith("H"))
            {
                double velocity = DataManager.Instance.GET_DOUBLE_DATA(IoNameHelper.V_DBL_SET_Y_JOGVEL_HIGH, out bool _);
                DataManager.Instance.SET_DOUBLE_DATA(IO_Y_JOG_VELOCITY_SET, velocity);
            }
            else if (DataManager.Instance.GET_STRING_DATA(IoNameHelper.V_STR_SET_Y_JOGVEL_MODE, out _).StartsWith("M"))
            {
                double velocity = DataManager.Instance.GET_DOUBLE_DATA(IoNameHelper.V_DBL_SET_Y_JOGVEL_MID, out bool _);
                DataManager.Instance.SET_DOUBLE_DATA(IO_Y_JOG_VELOCITY_SET, velocity);
            }
            else if (DataManager.Instance.GET_STRING_DATA(IoNameHelper.V_STR_SET_Y_JOGVEL_MODE, out _).StartsWith("L"))
            {
                double velocity = DataManager.Instance.GET_DOUBLE_DATA(IoNameHelper.V_DBL_SET_Y_JOGVEL_LOW, out bool _);
                DataManager.Instance.SET_DOUBLE_DATA(IO_Y_JOG_VELOCITY_SET, velocity);
            }
            else
            {
                double velocity = DataManager.Instance.GET_DOUBLE_DATA(IoNameHelper.V_DBL_SET_Y_JOGVEL_VERYLOW, out bool _);
                DataManager.Instance.SET_DOUBLE_DATA(IO_Y_JOG_VELOCITY_SET, velocity);
            }

            if (DataManager.Instance.SET_INT_DATA(IO_Y_JOG_MINUS, 1))
            {
                return this.F_RESULT_SUCCESS;
            }
            else
            {
                AlarmManager.Instance.SetAlarm(ALARM_Y_AXIS_JOG_MINUS_FAIL);
                return this.F_RESULT_FAIL;
            }
        }

        public override void PostExecute()
        {
            DataManager.Instance.SET_INT_DATA(IO_Y_JOG_MINUS, 0);
        }
    }
}
