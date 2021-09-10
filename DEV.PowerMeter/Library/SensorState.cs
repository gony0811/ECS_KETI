
using SharedLibrary;

namespace DEV.PowerMeter.Library
{
    [API(APICategory.Measurement, "Present sensor state. not much can happen unless it's Ready. For a newly-opened meter, usually the sensor simply is Ready (present and initialized). With hot swapping, the usual sequence is: Missing -> Identifying -> Ready. Most sensors complete the Identifying state almost immediately. A few spend a noticeable amount of time there. ")]
    public enum SensorState
    {
        [API("Sensor is not connected to meter")] Missing,
        [API("Sensor is connected to meter but initialization is in progress by the Firmware or this Software")] Identifying,
        [API("Sensor is Ready -- connected and initialized")] Ready,
    }
}
