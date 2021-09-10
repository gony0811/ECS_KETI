using DEV.PowerMeter.Library.DeviceModels;
using SharedLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;


namespace DEV.PowerMeter.Library
{
    public abstract class Device : IDevice, IDecodeMeasurement, IRangeSelection, IAutoRange, IDaqDevice
    {
        public const string SCPI_Identification = "*IDN";

        public Communicator Communicator { get; protected set; }

        public bool IsOpen => this.Communicator != null && this.Communicator.IsOpen;

        public Channel Channel => this.Communicator?.Channel;

        public string PortName => this.Communicator?.Channel.PortName;

        public void Open(Channel channel)
        {
            this.Communicator = new Communicator(channel);
            this.EnableHandshaking();
        }

        public void OpenRaw(Channel channel) => this.Communicator = new Communicator(channel);

        public void Close() => this.Communicator.Close();

        public override string ToString() => string.Format("{0} on {1}", (object)this.GetType().Name, (object)this.Communicator.Channel.PortName);

        protected abstract void EnableHandshaking();

        protected abstract void DisableHandshaking();

        public abstract string Identification { get; }

        public Library.SystemType SystemType_AsEnum { get; protected set; }

        public void LoadSystemType()
        {
            string systemType = this.SystemType;
            this.SystemType_AsEnum = SCPI.SystemTypeConverter.FromString(systemType);
        }

        public abstract string SystemType { get; }

        public string BaudRate
        {
            get
            {
                try
                {
                    if (this is IConfigureBaudRate configureBaudRate2)
                        return configureBaudRate2.ConfigureBaudRate;
                }
                catch (Exception ex)
                {
                    this.Trace("ConfigureBaudRate Exception: " + ex.Message);
                }
                return "NA";
            }
        }

        public abstract SystemStatusBits SystemStatus_AsEnum { get; }

        public abstract SystemFaultBits SystemFaults_AsEnum { get; }

        public abstract OperatingMode OperatingMode_AsEnum { get; set; }

        public virtual MeasurementChannelFlags MeasurementSpeed_AsEnum
        {
            get => MeasurementChannelFlags.Slow;
            set
            {
                if (value != MeasurementChannelFlags.Slow)
                    throw new NotImplementedException("non-slow MeasurementChannelFlags");
            }
        }

        public virtual MeasurementChannelFlags QuerySpeedList_AsEnum => MeasurementChannelFlags.Slow;

        public abstract bool EnableSpeedup_AsBool { get; set; }

        public abstract bool EnableWavelengthCorrection_AsBool { get; set; }

        public abstract uint WavelengthCorrectionValue_AsUint { get; set; }

        public abstract uint WavelengthCorrectionMin_AsUint { get; }

        public abstract uint WavelengthCorrectionMax_AsUint { get; }

        public abstract List<uint> WavelengthTable_AsList { get; }

        public abstract string SerialNumber { get; set; }

        public abstract string PartNumber { get; set; }

        public abstract string ModelName { get; set; }

        public abstract string CalibrationDate { get; set; }

        public abstract string ManufactureDate { get; set; }

        public abstract string ProbeTypeAndQualifier { get; }

        public virtual string ProbeModel => "[ProbeModel]";

        public virtual string ProbeSerialNumber => string.Empty;

        public virtual double ProbeResponsivity_AsReal => 0.0;

        public virtual string ProbeCalDate => string.Empty;

        public virtual string ProbeManDate => string.Empty;

        public virtual string SensorTemperature => "NA";

        public virtual string ProbeDiameter => "NA";

        public virtual double ProbeDiameter_AsReal => 0.0;

        public virtual bool ProbeDiameter_IsAvailable => this.ProbeDiameter != "NA";

        public virtual bool EnableAreaCorrection_AsBool
        {
            get => false;
            set
            {
            }
        }

        public virtual string AreaCorrectionValue
        {
            get => (string)null;
            set
            {
            }
        }

        public virtual double AreaCorrectionValue_AsReal
        {
            get => 0.0;
            set
            {
            }
        }

        public virtual string AnalogOutputLevel
        {
            get => (string)null;
            set
            {
            }
        }

        public virtual Library.AnalogOutputLevel AnalogOutputLevel_AsEnum
        {
            get => Library.AnalogOutputLevel.One;
            set
            {
            }
        }

        public virtual bool EnableSmoothing_AsBool
        {
            get => false;
            set
            {
            }
        }

        public virtual bool EnableAutoRange_AsBool
        {
            get => false;
            set
            {
            }
        }

        public abstract double SelectedRange_AsReal { get; set; }

        public abstract List<double> QueryRangeList_AsList { get; }

        public abstract double RangeMax_AsReal { get; }

        public abstract double RangeMin_AsReal { get; }

        public virtual bool EnableGainCompensation_AsBool
        {
            get => false;
            set
            {
            }
        }

        public virtual double GainCompensationFactor_AsReal
        {
            get => 0.0;
            set
            {
            }
        }

        public virtual string DecimationRate
        {
            get => (string)null;
            set
            {
            }
        }

        public virtual uint DecimationRate_AsUint
        {
            get => 0;
            set
            {
            }
        }

        public virtual TriggerSource TriggerSource_AsEnum
        {
            get => TriggerSource.Internal;
            set
            {
            }
        }

        public virtual double TriggerLevel_AsReal
        {
            get => 0.0;
            set => this.SetTriggerLevel(value);
        }

        public virtual void SetTriggerLevel(double level)
        {
        }

        public virtual double TriggerLevelMinimum_AsReal => 0.0;

        public virtual double TriggerLevelMaximum_AsReal => 0.0;

        public virtual double TriggerLevelPercent_AsReal
        {
            get => 0.0;
            set
            {
            }
        }

        public virtual double TriggerLevelPercentMinimum_AsReal => 0.0;

        public virtual double TriggerLevePercentMaximum_AsReal => 0.0;

        public virtual TriggerSlope TriggerSlope_AsEnum
        {
            get => TriggerSlope.Positive;
            set
            {
            }
        }

        public virtual uint TriggerDelay_AsUint
        {
            get => 0;
            set
            {
            }
        }

        public virtual void ClearSequenceId()
        {
        }

        public virtual TriggerLevel_LPEM TriggerLevel_LPEM_AsEnum
        {
            get => TriggerLevel_LPEM.Low;
            set
            {
            }
        }

        public virtual void SetCalibrationPassword(string password)
        {
        }

        public virtual void CalibrationCommit()
        {
        }

        public virtual void PersistentMemoryWipe()
        {
        }

        protected virtual void ProbeTypeOverride(string probe)
        {
        }

        public virtual void ImpersonateHighSpeedProbe()
        {
        }

        public virtual string FpgaAveragingId
        {
            get => (string)null;
            set
            {
            }
        }

        public virtual uint FpgaAveragingId_AsUint
        {
            get => 0;
            set
            {
            }
        }

        public virtual string FpgaDecimation
        {
            get => (string)null;
            set
            {
            }
        }

        public virtual uint FpgaDecimation_AsUint
        {
            get => 0;
            set
            {
            }
        }

        public virtual string FirmwareVersion => (string)null;

        public virtual string FpgaFirmwareVersion => (string)null;

        public virtual string FpgaHardwareVersion => (string)null;

        public virtual bool FirmwareVersion_HasProbeRomVersion => false;

        public virtual string ProbeRomVersion => (string)null;

        public virtual bool FirmwareVersion_HasForceCommand => false;

        public virtual bool FirmwareVersion_HasWorkingZero => false;

        public virtual bool FirmwareVersion_EEPEOM_UpgradeAllowed => false;

        public virtual bool FirmwareVersion_V2_Release => false;

        public virtual bool FirmwareVersion_HasHeisenbergSensorType => false;

        public virtual bool FirmwareVersion_HasWorkingQuadBinary => false;

        public virtual bool FirmwareVersion_HasFastEnergy => false;

        public virtual bool FirmwareVersion_HasMeterType => false;

        public virtual bool FirmwareVersion_HasDynamicTriggerLevels => false;

        public virtual bool FirmwareVersion_HasConfigBaudRate => false;

        public virtual void SystemReset()
        {
        }

        public virtual void SystemRestore()
        {
        }

        public abstract void SendCommand(string command);

        public abstract void ConfigureZero();

        public virtual DataEncoding DataEncoding_AsEnum
        {
            get => DataEncoding.Binary;
            set
            {
            }
        }

        public abstract DataFieldFlags ConfigureDataFields_AsEnum { get; set; }

        public virtual string EnableSnapshotMode
        {
            get => (string)null;
            set
            {
            }
        }

        public virtual bool EnableSnapshotMode_AsBool
        {
            get => false;
            set
            {
            }
        }

        public virtual string PreTriggerDelay
        {
            get => (string)null;
            set
            {
            }
        }

        public virtual uint PreTriggerDelay_AsUint
        {
            get => 0;
            set
            {
            }
        }

        public virtual void ForceTrigger()
        {
        }

        public virtual uint MeasurementWindow_AsUint
        {
            get => 0;
            set
            {
            }
        }

        public virtual string MeterType => (string)null;

        public virtual bool MeterHasPyro => false;

        public abstract string ReadNewDataRecord();

        public abstract void Start(uint count = 0);

        public abstract void Stop();

        public abstract DataRecordSingle DecodeMeasurement(string line);

        public double DecodeReal(string value) => !string.IsNullOrWhiteSpace(value) ? double.Parse(value, NumberStyles.Float, (IFormatProvider)CultureInfo.InvariantCulture) : throw new FormatException("Null or empty real number");

        public void Trace(string format, params object[] args)
        {
        }

        [Conditional("TRACE_DECODE")]
        public void TraceDecode(string message) => this.Trace(message);

        [Conditional("TRACE_VERSIONS")]
        public void TraceVersions(string format, params object[] args)
        {
        }

        [Conditional("TRACE_START_STOP")]
        public void TraceStartStop(string format, params object[] args)
        {
        }

        [Conditional("TRACE_ZEROING")]
        public void TraceZeroing(string format, params object[] args) => DateTime.Now.ToHhMmSs();

        [Conditional("TRACE_TRIGGERS")]
        public void TraceTriggers(string message) => this.Trace(message);
    }
}
