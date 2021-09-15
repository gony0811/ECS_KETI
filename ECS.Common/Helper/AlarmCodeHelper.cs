using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS.Common.Helper
{
    public class AlarmCodeHelper
    {
        public const string CH1_LED_OFF_TIMEOUT = "E1001";
        public const string CH1_LED_OFF_FAIL = "E1002";
        public const string CH1_LED_ON_TIMEOUT = "E1003";
        public const string CH1_LED_ON_FAIL = "E1004";

        public const string GAS_LEAK_DETECTION = "E1005";
        public const string LASER_INTERLOCK_DETECTION = "E1006";

        public const string X_AXIS_HOME_STOP_TIMEOUT = "E2021";
        public const string X_AXIS_HOME_STOP_FAIL = "E2022";
        public const string X_AXIS_HOMMING_TIMEOUT = "E2023";
        public const string X_AXIS_HOMMING_FAIL = "E2024";
        public const string X_AXIS_JOG_BWD_MOTION_FAIL = "E2025";
        public const string X_AXIS_JOG_FWD_MOTION_FAIL = "E2026";
        public const string X_AXIS_JOG_STOP_MOTION_FAIL = "E2027";
        public const string X_AXIS_MOVE_MOTION_TIMEOUT = "E2028";
        public const string X_AXIS_MOVE_MOTION_FAIL = "E2029";


        public const string Y_AXIS_HOME_STOP_TIMEOUT = "E2031";
        public const string Y_AXIS_HOME_STOP_FAIL = "E2032";
        public const string Y_AXIS_HOMMING_TIMEOUT = "E2033";
        public const string Y_AXIS_HOMMING_FAIL = "E2034";
        public const string Y_AXIS_JOG_BWD_MOTION_FAIL = "E2035";
        public const string Y_AXIS_JOG_FWD_MOTION_FAIL = "E2036";
        public const string Y_AXIS_JOG_STOP_MOTION_FAIL = "E2037";
        public const string Y_AXIS_MOVE_MOTION_TIMEOUT = "E2038";
        public const string Y_AXIS_MOVE_MOTION_FAIL = "E2039";

        public const string X_AXIS_SERVO_STOP_TIMEOUT = "E2040";
        public const string X_AXIS_SERVO_STOP_FAIL = "E2041";
        public const string Y_AXIS_SERVO_STOP_TIMEOUT = "E2042";
        public const string Y_AXIS_SERVO_STOP_FAIL = "E2043";

        public const string LASER_STARTUP_TIMEOUT = "E3001";

        public const string LASER_ON_TIMEOUT = "E3002";
        public const string LASER_ON_FAIL = "E3003";

        public const string LASER_OFF_TIMEOUT = "E3004";
        public const string LASER_OFF_FAIL = "E3005";

        public const string LASER_STANDBY_TIMEOUT = "E3006";
        public const string LASER_STANDBY_FAIL = "E3007";

        public const string LASER_SHUTDOWN_TIMEOUT = "E3008";
        public const string LASER_SHUTDOWN_FAIL = "E3009";

        public const string AUTO_PROCESS_FAIL = "E5001";
        public const string EQUIPMENT_DOWN = "E9001";
        public const string EMERGENCY_STOP_INTERLOCK = "E9999";

        public const string MODE_SET_EGYNGR_TIMEOUT = "E3010";
        public const string MODE_SET_EGYNGR_FAIL = "E3011";

        public const string MODE_SET_EGYBURSTNGR_TIMEOUT = "E3012";
        public const string MODE_SET_EGYBURSTNGR_FAIL = "E3013";

        public const string MODE_SET_HVNGR_TIMEOUT = "E3014";
        public const string MODE_SET_HVNGR_FAIL = "E3015";

        public const string POWER_METER_SHUTTER_BACKWARD_TIMEOUT = "E3016";
        public const string POWER_METER_SHUTTER_BACKWARD_FAIL = "E3017";

        public const string POWER_METER_SHUTTER_FORWARD_TIMEOUT = "E3018";
        public const string POWER_METER_SHUTTER_FORWARD_FAIL = "E3019";

        public const string DOOR_OPEN_INTERLOCK = "E9002";
        public const string CPBOX_OPEN_INTERLOCK = "E9003";

    }
}
