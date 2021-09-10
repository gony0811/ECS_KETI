using SharedLibrary;
using System;

namespace DEV.PowerMeter.Library
{
    [Flags]
    [API(APICategory.Measurement)]
    public enum MeasurementFlags : ushort
    {
        TriggerDetected = 1,
        BaselineClip = 2,
        Calculating = 4,
        FinalEnergyRecord = 8,
        OverRange = 16, // 0x0010
        UnderRange = 32, // 0x0020
        SpedUp = 64, // 0x0040
        OverTemp = 128, // 0x0080
        MissingSamples = 256, // 0x0100
        MissingPulse = 512, // 0x0200
        DirtyBatch = 1024, // 0x0400
        Terminated = 32768, // 0x8000
        AlarmDetected = 16384, // 0x4000
        Impossible = 4096, // 0x1000
        StopSent = 8192, // 0x2000
        ALL = StopSent | Impossible | AlarmDetected | Terminated | DirtyBatch | MissingPulse | MissingSamples | OverTemp | SpedUp | UnderRange | OverRange | FinalEnergyRecord | Calculating | BaselineClip | TriggerDetected, // 0xF7FF
        DebugFlags = StopSent | Impossible, // 0x3000
        OverrangeOrUnderrange = UnderRange | OverRange, // 0x0030
        EnergyFlags = FinalEnergyRecord | Calculating, // 0x000C
        TriggerFlags = FinalEnergyRecord | TriggerDetected, // 0x0009
        HistogramIgnoreBits = Impossible | Terminated | MissingSamples | FinalEnergyRecord | BaselineClip, // 0x910A
    }
}
