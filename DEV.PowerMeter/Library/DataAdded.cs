
using SharedLibrary;

namespace DEV.PowerMeter.Library
{
    [API(APICategory.Measurement, "Delegate for DataAdded events, primarily for linking CaptureBuffer with DataLogger")]
    public delegate void DataAdded(IDataRecordSingle data);
}
