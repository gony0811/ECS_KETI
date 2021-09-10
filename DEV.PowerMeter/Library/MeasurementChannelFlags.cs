using SharedLibrary;
using System;

namespace DEV.PowerMeter.Library
{
    [API(APICategory.Measurement, "Measurement Channel Flags.")]
    [Flags]
    public enum MeasurementChannelFlags
    {
        Slow = 1,
        Fast = 2,
        None = 4,
        FastAndSlow = Fast | Slow, // 0x00000003
    }
}
