using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEV.LaserControl
{
    public struct ReplyCodes
    {
        public static string ACCEPT_COMMAND = "0";
        public static string NOT_ACCEPT_COMMAND = "1";
        public static string UNKNOWN_COMMAND = "2";
        public static string PARAMETER_VALUE_OUT_OF_RANGE = "3";
    }
}
