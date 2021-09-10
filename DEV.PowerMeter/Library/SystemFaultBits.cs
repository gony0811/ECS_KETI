using System;
using SharedLibrary;

namespace DEV.PowerMeter.Library
{
    [Flags]
    [API(APICategory.Measurement)]
    public enum SystemFaultBits
    {
        SensorMissing = 1,
        SensorOvertemp = 2,
        SensorCommFailure = 4,
        SensorChecksum = 8,
        SensorFirmware = 16, // 0x00000010
        SensorEEPROM = 32, // 0x00000020
        SensorUnrecognized = 64, // 0x00000040
        BadInitialization = 128, // 0x00000080
        BadZero = 256, // 0x00000100
        IPCFailure = 512, // 0x00000200
        IgnoreDuringPolling = BadZero | SensorMissing, // 0x00000101
        ALL = IgnoreDuringPolling | IPCFailure | BadInitialization | SensorUnrecognized | SensorEEPROM | SensorFirmware | SensorChecksum | SensorCommFailure | SensorOvertemp, // 0x000003FF
    }
}
