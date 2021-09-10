using SharedLibrary;

namespace DEV.PowerMeter.Library
{
    [API(APICategory.Utility)]
    public enum TimeScale
    {
        uS,
        mS,
        Seconds,
        Minutes,
        Hours,
        Days,
    }
}
