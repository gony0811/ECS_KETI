using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEV.PowerMeter.Library
{
    public enum DAQ_State
    {
        Progress,
        Start,
        Stop,
        Restart,
        Loading,
        Stopping,
        Triggered,
        TriggerWait,
    }
}
