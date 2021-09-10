using SharedLibrary;

namespace DEV.PowerMeter.Library
{
    [API(APICategory.Measurement)]
    public enum SensorTypeQualifier
    {
        [API("Sensor Type Qualifier missing or unknown")] None,
        [API("Sensor Type Qualifier missing, unknown, or not relevant")] Nospec,
        [API("Sensor lacks Quadrents")] Single,
        [API("Sensor has Quadrents, capable of showing x,y beam position")] Quad,
        [API("Sensor has ENHANCED Quadrents, capable of showing x,y beam position")] EnhQuad,
    }
}
