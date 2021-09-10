
using SharedLibrary;

namespace DEV.PowerMeter.Library
{
    [API(APICategory.Measurement, "AnalogOutputLevel settings (output voltage corresponding to max measurement range).")]
    public enum AnalogOutputLevel
    {
        [API("1.0 Volts at full scale.")] One = 0,
        [API("2.0 Volts at full scale.")] Default = 1,
        [API("2.0 Volts at full scale.")] Two = 1,
        [API("4.0 Volts at full scale.")] Four = 2,
    }
}
