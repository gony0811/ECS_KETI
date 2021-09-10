using SharedLibrary;

namespace DEV.PowerMeter.Library
{
    [API(APICategory.Measurement, "TriggerSlope settings.")]
    public enum TriggerSlope
    {
        Default = 0,
        Positive = 0,
        Negative = 1,
    }
}
