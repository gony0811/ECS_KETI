using System;

namespace DEV.PowerMeter.Library
{
    [Flags]
    public enum PropertyChangedType
    {
        RangeTable = 1,
        SelectedRange = 2,
        TriggerLevel = 4,
        Wavelength = 8,
        All = Wavelength | TriggerLevel | SelectedRange | RangeTable, // 0x0000000F
        None = 0,
    }
}
