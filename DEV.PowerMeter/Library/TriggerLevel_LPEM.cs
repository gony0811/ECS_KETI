
using SharedLibrary;

namespace DEV.PowerMeter.Library
{
    [API(APICategory.Measurement, "TriggerLevel_LPEM settings.")]
    public enum TriggerLevel_LPEM
    {
        Default = 0,
        Low = 0,
        Medium = 1,
        High = 2,
    }
}
