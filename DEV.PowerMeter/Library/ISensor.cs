

namespace DEV.PowerMeter.Library
{
    public interface ISensor
    {
        SensorType SensorType { get; }

        SensorTypeQualifier SensorTypeQualifier { get; }

        string SensorTypeAndQualifier { get; }

        string ModelName { get; }

        string SerialNumber { get; }
    }
}
