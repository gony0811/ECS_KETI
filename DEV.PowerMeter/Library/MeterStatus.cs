using SharedLibrary;


namespace DEV.PowerMeter.Library
{
    [API(APICategory.Measurement, "Principal Status states for PhoenixMeter. ")]
    public enum MeterStatus
    {
        [API("Meter has been connected (port is open)")] MeterConnected,
        [API("Meter has become disconnected (port is closed)")] MeterDisconnected,
        [API("Same as SensorStatus.Missing")] SensorDisconnected,
        [API("Same as SensorStatus.Identifying")] SensorIdentifying,
        [API("Sensor and thus Meter are Ready for normal operations")] SensorConnected,
        [API("Meter has started Data Acquisition.")] MeterStarted,
        [API("Meter has stopped Data Acquisition, and is Ready for normal operations")] MeterStopped,
        [API("A System Fault has been detected")] SystemFault,
    }
}
