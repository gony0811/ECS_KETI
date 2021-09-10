
using SharedLibrary;

namespace DEV.PowerMeter.Library
{
    [API(APICategory.Measurement, "Sensor represents the present state of a sensor. All of its properties are read-only.")]
    public class Sensor : ISensor
    {
        protected string sensorTypeAndQualifier;

        [API("True once this Sensor object itself is properly initialized. ")]
        public bool IsInitialized { get; set; }

        [API("Sensor object are automatically created as needed by the Meter. There is no reason to explicitly create one.")]
        public Sensor() => this.IsInitialized = false;

        [API("Reload is automatically called by the Meter at the appropriate point during Sensor Discovery")]
        public void Reload(Device device)
        {
            this.SensorTypeAndQualifier = device.ProbeTypeAndQualifier;
            this.ModelName = device.ProbeModel;
            this.SerialNumber = device.ProbeSerialNumber;
            this.Responsivity = device.ProbeResponsivity_AsReal;
            this.CalibrationDate = device.ProbeCalDate;
            this.MfgDate = device.ProbeManDate;
            this.Diameter = device.ProbeDiameter_AsReal;
            this.ProbeRomVersion = device.ProbeRomVersion;
        }

        [API("Sensor Type And Qualifier")]
        public string SensorTypeAndQualifier
        {
            get => this.sensorTypeAndQualifier;
            protected set
            {
                this.sensorTypeAndQualifier = value;
                if (this.sensorTypeAndQualifier == null)
                    return;
                Library.SensorTypeAndQualifier typeAndQualifier = new Library.SensorTypeAndQualifier(value, SCPI.CommaChar);
                this.SensorType = typeAndQualifier.SensorType;
                this.SensorTypeQualifier = typeAndQualifier.SensorTypeQualifier;
            }
        }

        [API("Sensor Type")]
        public SensorType SensorType { get; protected set; }

        [API("Sensor Type Qualifier")]
        public SensorTypeQualifier SensorTypeQualifier { get; protected set; }

        [API("Sensor Model Name")]
        public string ModelName { get; protected set; }

        [API("Sensor Serial Number")]
        public string SerialNumber { get; protected set; }

        [API("Sensor Diameter (mm if > 0)")]
        public double Diameter { get; protected set; }

        [API("Sensor Aperture Area (cm squared, if > 0)")]
        public double ApertureArea { get; protected set; }

        [API("Sensor Responsivity")]
        public double Responsivity { get; protected set; }

        [API("Sensor Calibration Date")]
        public string CalibrationDate { get; protected set; }

        [API("Sensor Manufacturing Date")]
        public string MfgDate { get; protected set; }

        [API("Sensor Probe ROM Version")]
        public string ProbeRomVersion { get; protected set; }

        public string UniqueName => this.SensorType.ToString() + this.ModelName + " #" + this.SerialNumber;

        public override string ToString() => string.Format("{0} {1} #{2}", (object)this.SensorType, (object)this.ModelName, (object)this.SerialNumber);
    }
}
