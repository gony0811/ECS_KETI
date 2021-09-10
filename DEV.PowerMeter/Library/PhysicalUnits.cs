
using SharedLibrary;

namespace DEV.PowerMeter.Library
{
    [API(APICategory.Formatting, "Physical dimensions of measurement Units. Sometimes used as a shorthand for Units, \r\nwhen Area Correction does not apply, and for PhysicalUnits \r\nthat aren't supported by the Data Acquisition hardware.")]
    public enum PhysicalUnits
    {
        [API("Joules from Energy measurements")] Joules,
        [API("Watts from Power measurements")] Watts,
        [API("dBm (decibel-milliwatts https://en.wikipedia.org/wiki/DBm) from Power measurements [NOT USED]. ")] Dbm,
        [API("Raw ADC Counts [NOT USED]")] ADC_Counts,
        [API("Seconds for measurement Timestamps (X axis)")] Seconds,
        [API("Hertz for frequency (1/Period)")] Hertz,
        [API("Raw ADC Counts [NOT USED]")] Timestamp,
        [API("Pulse ID index for Energy measurements (X axis)")] PulseId,
        [API("Pounds per Square Inch for Pressure measurements [NOT USED]")] PressurePSI,
    }
}
