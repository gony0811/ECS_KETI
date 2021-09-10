using SharedLibrary;

namespace DEV.PowerMeter.Library
{
    [API(APICategory.Measurement, "Meter operating mode. Only PowerWatts and EnergyJoules are presently used.")]
    public enum OperatingMode
    {
        [API("Measure Power in Watts")] PowerWatts,
        [API("Measure Power in dBm [NOT USED]")] PowerDbm,
        [API("Measure Energy in Joules")] EnergyJoules,
        [API("Measure Pressure in PSI [NOT USED]")] PressurePsi,
    }
}
