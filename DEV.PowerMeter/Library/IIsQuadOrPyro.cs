using SharedLibrary;

namespace DEV.PowerMeter.Library
{
    [API(APICategory.Measurement, "Measurement properties of interest to some components of the system. Meter, CaptureBuffer, and Import/Export subsystems all utilize this info.")]
    public interface IIsQuadOrPyro
    {
        [API("True iff the Sensor associated with the object implementing this interface is a Quad sensor")]
        bool Sensor_IsQuad { get; }

        [API("True iff the Sensor associated with the object implementing this interface is a Pyro sensor")]
        bool Sensor_IsPyro { get; }
    }
}
