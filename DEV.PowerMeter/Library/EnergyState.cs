using SharedLibrary;

namespace DEV.PowerMeter.Library
{
    [API(APICategory.Unclassified)]
    public enum EnergyState
    {
        Searching,
        Calculating,
        Finished,
    }
}
