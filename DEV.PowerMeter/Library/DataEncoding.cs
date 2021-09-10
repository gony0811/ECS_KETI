using SharedLibrary;

namespace DEV.PowerMeter.Library
{
    [API(APICategory.Measurement, "Binary vs. ascii encoding of measurement data.")]
    public enum DataEncoding
    {
        Binary,
        Ascii,
    }
}
