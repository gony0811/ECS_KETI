using SharedLibrary;

namespace DEV.PowerMeter.Library
{
    [API(APICategory.Measurement, "Common features of DataRecordSingle and DataRecordQuad classes.")]
    public abstract class DataRecordBase
    {
        [API("when creating data records, whether or not retain a copy of the original binary data (global setting). This option is primarily for debugging.It consumes more memory, slows down the software and may make it impossible to run the meter at full speed without seeing MissingData flags. ")]
        public static bool RetainBinary = true;
        [API("Specify whether timestamps shall be assigned by the UI or by the instrument, for Meter hardware transmission and for Software decoding of same.")]
        public static bool TimestampsAreSeqId = false;
        public static MeasurementFlags FakeFlags = (MeasurementFlags)0;
        [API("Some graphics libraries fault when measurement data exceed the range of a Decimal, and in any case, abnormally large samples are problematic. The software marks any samples with suspiciously large values with MeasurementFlags.Impossible flag, and truncates the measurement to lie within range. Note that the comparision is made to the final measurement value reported by the meter, and that Gain and other meter settings may result in a measurement value that exceedes the power rating of the sensor (else we'd just make that the limit).")]
        public static double ImpossiblyLargeMeasurement = 1000000.0;

        [API("Specify which data fields are included in binary data. for Meter hardware transmission and for Software decoding of same.")]
        public static DataFieldFlags SelectedDataFields { get; set; }

        public static IErrorReporter ErrorReporter;// => ServiceLocator.Resolve<IErrorReporter>();
    }
}
