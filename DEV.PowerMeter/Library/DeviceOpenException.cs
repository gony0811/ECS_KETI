using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEV.PowerMeter.Library
{
    public class DeviceOpenException : Exception
    {
        public DeviceOpenException(Exception inner)
          : base("Unable to open device", inner)
        {
        }
    }
}
