using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS.Common.Helper
{
    public class AlarmCodeHelper
    {
        public const string ALARM_LASER_STARTUP_TIMEOUT = "E3001";

        public const string ALARM_LASER_ON_TIMEOUT = "E3002";
        public const string ALARM_LASER_ON_FAIL = "E3003";

        public const string ALARM_LASER_OFF_TIMEOUT = "E3004";
        public const string ALARM_LASER_OFF_FAIL = "E3005";

        public const string ALARM_LASER_STANDBY_TIMEOUT = "E3006";
        public const string ALARM_LASER_STANDBY_FAIL = "E3007";

        public const string ALARM_LASER_SHUTDOWN_TIMEOUT = "E3008";
        public const string ALARM_LASER_SHUTDOWN_FAIL = "E3009";

        public const string ALARM_MODE_SET_EGYNGR_TIMEOUT = "E3010";
        public const string ALARM_MODE_SET_EGYNGR_FAIL = "E3011";

        public const string ALARM_MODE_SET_EGYBURSTNGR_TIMEOUT = "E3012";
        public const string ALARM_MODE_SET_EGYBURSTNGR_FAIL = "E3013";

        public const string ALARM_MODE_SET_HVNGR_TIMEOUT = "E3014";
        public const string ALARM_MODE_SET_HVNGR_FAIL = "E3015";


    }
}
