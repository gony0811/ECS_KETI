using SharedLibrary;

namespace DEV.PowerMeter.Library
{
    [API(APICategory.Measurement, "TriggerSource settings.")]
    public enum TriggerSource
    {
        Default = 0,
        Internal = 0,
        External = 1,
    }
}
