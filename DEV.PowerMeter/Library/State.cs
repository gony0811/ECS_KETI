using SharedLibrary;

namespace DEV.PowerMeter.Library
{
    [API(APICategory.Unclassified)]
    public enum State
    {
        Waiting,
        RisingLo,
        RisingHi,
        Running,
        FallingHi,
        FallingLo,
        Accept,
    }
}
