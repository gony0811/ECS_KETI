using SharedLibrary;
using System;

namespace DEV.PowerMeter.Library
{
    [API(APICategory.Measurement, "Essential members of DataRecordSingle objects")]
    public interface IDataRecordSingle : IEncodable
    {
        [API("Timestamp of this sample")]
        DateTime Timestamp { get; }

        [API("Sequence ID of this sample")]
        ulong Sequence { get; }

        [API("Measurement value for this sample")]
        double Measurement { get; }

        [API("MeasurementFlags for this sample")]
        MeasurementFlags Flags { get; set; }

        [API("Pulse Period (microseconds) for this Energy Measurement")]
        uint Period { get; }

        [API("Binary data bytes, if any")]
        byte[] DataBytes { get; }
    }
}
