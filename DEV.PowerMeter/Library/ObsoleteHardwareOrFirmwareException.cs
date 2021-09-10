using System;


namespace DEV.PowerMeter.Library
{
    public class ObsoleteHardwareOrFirmwareException : Exception
    {
        public ObsoleteHardwareOrFirmwareException(Exception inner)
          : base("Obsolete Hardware Or Firmware versions", inner)
        {
        }

        public ObsoleteHardwareOrFirmwareException(string message)
          : base(message)
        {
        }
    }
}
