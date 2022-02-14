using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEV.LaserControl
{
    public class InputDeviceData
    {
        public string OPMODE_STATUS { get; set; }
        public string OPMODE_ERRORCODE { get; set; }
        public string STATUS_INTERLOCK { get; set; }
        public string STATUS_MAINTREQUIRED { get; set; }
        public string TRIGGER_MODE { get; set; }
        public string ENERGY_MODE { get; set; }
        public int STATUS_TUBEPRESSURE { get; set; }
        public int STATUS_MANPRESSURE { get; set; }
        public int STATUS_COUNTER { get; set; }
        public int STATUS_COUNTERTOTAL { get; set; }
        public int STATUS_COUNTERNEWFILL { get; set; }
        public int STATUS_COUNTERMAINT { get; set; }
        public int PULSE_SEQPAUSE { get; set; }
        public int PULSE_SEQBST { get; set; }
        public int PULSE_REPRATE { get; set; }
        public int PULSE_COUNTS { get; set; }
        public int PULSE_BSTPULSE { get; set; }
        public int PULSE_BSTPAUSE { get; set; }
        public int OPMODE_ISWAIT { get; set; }
        public double STATUS_TUBETEMP { get; set; }
        public double ENERGY_HV { get; set; }
        public double ENERGY_EGYSET { get; set; }
        public double ENERGY_EGY { get; set; }

        public InputDeviceData()
        {
            OPMODE_STATUS = "0";
            OPMODE_ERRORCODE = "0";
            STATUS_INTERLOCK = "NONE";
            STATUS_MAINTREQUIRED = "NONE";
            TRIGGER_MODE = "INT";
            ENERGY_MODE = "EGY NGR";
        }
    }
}
