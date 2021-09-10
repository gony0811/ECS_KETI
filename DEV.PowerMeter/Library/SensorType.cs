using SharedLibrary;

namespace DEV.PowerMeter.Library
{
    [API(APICategory.Measurement)]
    public enum SensorType
    {
        [API("Sensor missing or type unknown")] None,
        [API("Sensor is Thermopile type")] Thermo,
        [API("Sensor is Pyro type")] Pyro,
        [API("Sensor is Optical type")] Optical,
    }
}
