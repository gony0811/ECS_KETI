

namespace DEV.PowerMeter.Library
{
    public interface IDecodeMeasurement
    {
        DataRecordSingle DecodeMeasurement(string line);
    }
}
