using System;

namespace DEV.PowerMeter.Library
{
    public class DeviceDisableHandshakingException : Exception
    {
        public DeviceDisableHandshakingException()
          : base("Unable to disable handshaking")
        {
        }
    }
}
