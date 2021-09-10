using SharedLibrary;

namespace DEV.PowerMeter.Library
{
    [API(APICategory.Measurement, "Essential members of DataRecordQuad objects")]
    public interface IDataRecordQuad : IDataRecordSingle, IEncodable
    {
        [API("X position of center of beam")]
        double X { get; }

        [API("Y position of center of beam")]
        double Y { get; }
    }
}
