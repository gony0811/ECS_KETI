using System;
using SharedLibrary;

namespace DEV.PowerMeter.Library
{
    [Flags]
    [API(APICategory.Measurement)]
    public enum SystemStatusBits : uint
    {
        SensorOvertemp = 2,
        SensorIsAttached = 4,
        BusyIdentifying = 8,
        BusyZeroing = 262144, // 0x00040000
        BusyCalculating = 524288, // 0x00080000
        FpgaUpdating = 1048576, // 0x00100000
        SystemFault = 2147483648, // 0x80000000
        BUSY = FpgaUpdating | BusyCalculating | BusyZeroing | BusyIdentifying, // 0x001C0008
        SensorFlags = BusyIdentifying | SensorIsAttached, // 0x0000000C
        ALL = SensorFlags | SystemFault | FpgaUpdating | BusyCalculating | BusyZeroing, // 0x801C000C
    }
}
