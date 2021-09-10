
using System;

namespace DEV.PowerMeter.Library
{
    public class MeterException : Exception
    {
        public MeterException(string format, params object[] args)
          : base(string.Format(format, args))
        {
        }
    }
}
