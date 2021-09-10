using SharedLibrary;

namespace DEV.PowerMeter.Library
{
    [API(APICategory.Measurement, "Measurement properties of interest to some components of the system. Meter, CaptureBuffer, and Timestamper objects all utilize this info.")]
    public interface ISamplePeriod
    {
        [API("True iff the Sensor associated with the object implementing this interface is a Pyro sensor")]
        bool Sensor_IsPyro { get; }

        [API("The current data acquisition sample period (in Ticks) according to the object implementing this interface")]
        long SamplePeriod { get; }
    }
}
