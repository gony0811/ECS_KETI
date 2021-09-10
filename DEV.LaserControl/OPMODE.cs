using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEV.LaserControl
{
    public struct OPMODE
    {
        public static string OFF = "OFF";
        public static string ON = "ON";
        public static string STANDBY = "STANDBY";
        public static string SHUTDOWN = "SHUTDOWN";
        public static string NEWFILL = "NEW FILL";
        public static string PURGE_RESERVOIR = "PURGE RESERVOIR";
        public static string SAFETY_FILL = "STFETY FILL";
        public static string TRANSPORT_FILL = "TRANSPORT FILL";
        public static string FLUSHING = "FLUSHING";
        public static string CONT = "CONT";
        public static string FLUSH_HALOGEN_LINE = "FLUSH HALOGEN LINE";
        public static string FLUSH_INERT_LINE = "FLUSH INERT LINE";
        public static string PURGE_HALOGEN_LINE = "PURGE HALOGEN LINE";
        public static string LL_OFF = "LL OFF";
        public static string ENERGY_CAL = "ENERGY CAL";
    }
}
