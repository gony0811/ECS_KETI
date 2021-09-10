
using SharedLibrary;

namespace DEV.PowerMeter.Library
{
    [API(APICategory.Measurement, "Specify Data fields included in measurement data records during data acquisition.")]
    [System.Flags]
    public enum DataFieldFlags
    {
        [API("Include Primary (Measurement) field in data stream")] Primary = 1,
        [API("Include X,Y Offsets (Quad) fields in data stream")] Quad = 2,
        [API("Include Flags field in data stream")] Flags = 4,
        [API("Include Sequence ID field in data stream")] Sequence = 8,
        [API("Include Period field in data stream")] Period = 16, // 0x00000010
        Unused = 96, // 0x00000060
        [API("Include Timestamp field in data stream")] Timestamp = 128, // 0x00000080
    }
}
