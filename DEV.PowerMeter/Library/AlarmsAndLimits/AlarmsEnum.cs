using System;

namespace DEV.PowerMeter.Library.AlarmsAndLimits
{
    [Flags]
    public enum AlarmsEnum
    {
        noAlarm = 0,
        StopAcquisition = 1,
        SoundAlarm = 2,
        ReverseBackgroundColor = 4,
        ChangeBackgroundColor = 8,
    }
}
