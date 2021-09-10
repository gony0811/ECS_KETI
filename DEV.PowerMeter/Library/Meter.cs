using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using SharedLibrary;
using DEV.PowerMeter.Library.DeviceModels;
using System.Diagnostics;
using System.Threading;
using System.Reflection;

namespace DEV.PowerMeter.Library
{
    [API(APICategory.Measurement, "The Meter object coordinates all commands and responses to/from the hardware. It Caches many settings. It translates the low-level \"SCPI\" command protocol to C# objects. ")]
    [DataContract]
    public abstract class Meter :
      IMeter,
      IIsQuadOrPyro,
      ISamplePeriod,
      IImportExport,
      ISequenceIds,
      IDaqMeter,
      ILicensedMeter,
      ILicensedItem,
      ISlowEnergyMode
    {
        private uint capacity = 50000;
        [API(APICategory.Unclassified)]
        public const bool DefaultContinuousMode = true;
        [API(APICategory.Unclassified)]
        public const bool DefaultSnapshotModeEnabled = false;
        [API(APICategory.Unclassified)]
        public const uint DefaultPreTrigger = 0;
        [API(APICategory.Unclassified)]
        public const bool DefaultWaitForTriggerEnabled = false;
        protected DataEncoding selectedDataEncoding;
        [API(APICategory.Unclassified)]
        public const uint SSIM_SnapshotMax = 240000;
        [API(APICategory.Unclassified)]
        public const uint PMPRO_SnapshotMax = 25000;
        public const uint NonSnapshotDefaultCapacity = 50000;
        public const uint NonSnapshotMaximumCapacity = 1000000;
        public const double MostCommonDetectorRadius = 9.5;
        protected OperatingMode operatingMode;
        [API(APICategory.Unclassified)]
        protected MeasurementChannelFlags selectedChannel;
        protected double selectedRange;
        [API(APICategory.Unclassified)]
        public const double AutoRangeValue = 0.0;
        protected bool autoRangeSelected = true;
        [API(APICategory.Unclassified)]
        public const uint CorrectionDisabled_Wavelength = 0;
        protected uint wavelengthCorrection;
        public const double FPGA_BaseClockRate = 40000000.0;
        public const double FPGA_BaseSampleRate = 40000000.0;
        public uint SnapshotFpgaDecimation = 64;
        public uint StandardFpgaDecimation = 2048;
        public uint StandardFpgaAvreagingId = 11;
        protected double triggerLevel;
        private double triggerLevelPercent;
        private TriggerSource triggerSource;
        private TriggerSlope triggerSlope;
        public const uint DefaultTriggerDelay = 0;
        [API(APICategory.Unclassified)]
        public const uint MaximumTriggerDelay = 1000000;
        private uint triggerDelay;
        private TriggerLevel_LPEM triggerLevel_LPEM;
        private uint measurementWindow = 100;
        [API(APICategory.Unclassified)]
        public const uint DefaultMeasurementWindow = 100;
        [API(APICategory.Unclassified)]
        public const uint MeasurementWindow_Minimum = 25;
        [API(APICategory.Unclassified)]
        public const uint MeasurementWindow_Maximum = 10000000;
        public const double DefaultAreaCorrectionValue = 1.0;
        [API(APICategory.Unclassified)]
        public const double MinAreaCorrectionArea = 0.01;
        [API(APICategory.Unclassified)]
        public const double MaxAreaCorrectionArea = 999.0;
        public readonly double MinAreaCorrectionDiameter = Meter.AreaToDiameter(0.01);
        public readonly double MaxAreaCorrectionDiameter = Meter.AreaToDiameter(999.0);
        private bool areaCorrection_Enabled;
        private double areaCorrectionValue = 1.0;
        private bool areaCorrectionIsDiameter;
        [API(APICategory.Unclassified)]
        public const double MinGainCompensationValue = 0.001;
        [API(APICategory.Unclassified)]
        public const double MaxGainCompensationValue = 100000.0;
        [API(APICategory.Unclassified)]
        public const double UnityGainCompensationFactor = 1.0;
        protected bool gainCompensation_Enabled;
        protected double gainCompensationFactor = 1.0;
        protected bool decimation_Enabled;
        [API(APICategory.Unclassified)]
        public const uint DecimationRate_Disabled = 1;
        protected uint decimationRate = 1;
        private bool speedup_Enabled;
        [API(APICategory.Unclassified)]
        public const bool DefaultSpeedup_Enabled = false;
        private AnalogOutputLevel analogOutputLevel;

        [API(APICategory.Unclassified)]
        public Meter.DataAcquisitionMode DAQ_Mode
        {
            get
            {
                if (this.SnapshotMode_IsSelected && this.SnapshotSettingsValid)
                    return Meter.DataAcquisitionMode.Snapshot;
                if (this.TriggerWaitMode_IsSelected && this.WaitTriggerSettingsValid)
                    return Meter.DataAcquisitionMode.TriggerWait;
                if (this.HighSpeedChannel_IsSelected)
                    return Meter.DataAcquisitionMode.HighSpeed;
                return this.QuadMode_IsSelected ? Meter.DataAcquisitionMode.Quad : Meter.DataAcquisitionMode.Power;
            }
        }

        public bool SnapshotContinuousMode_IsSelected => this.SnapshotMode_IsSelected && this.ContinuousMode;

        [API(APICategory.Unclassified)]
        public bool SnapshotMode_IsSelected => this.HasSensor && this.SnapshotSettingsValid && this.SnapshotModeEnabled;

        [API(APICategory.Unclassified)]
        public bool TriggerWaitMode_IsSelected => this.HasSensor && this.OperatingMode_IsPower && this.HighSpeedChannel_IsSelected && this.WaitForTriggerModeEnabled && !this.SnapshotModeEnabled;

        [API(APICategory.Unclassified)]
        public bool QuadMode_IsSelected => this.HasSensor && this.Sensor_IsQuad && !this.HighSpeedChannel_IsSelected;

        protected DAQ_Thread DAQ_ThreadCreate()
        {
            if (this.DAQ_Mode == Meter.DataAcquisitionMode.Snapshot && !this.SnapshotSettingsValid)
                throw new NotSupportedException("snapshot pre-requisites");
            return (DAQ_Thread)Activator.CreateInstance(Meter.GetDAQ_ThreadType(this.DAQ_Mode, this is ILegacyMeterless), (object)this.DAQ_StateChanged, (object)this, (object)this.Device, (object)this.CaptureBuffer);
        }

        public static Type GetDAQ_ThreadType(
          Meter.DataAcquisitionMode DAQ_Mode,
          bool isLegacyMeterless)
        {
            switch (DAQ_Mode)
            {
                case Meter.DataAcquisitionMode.Snapshot:
                    return typeof(DAQ_Snapshot);
                case Meter.DataAcquisitionMode.HighSpeed:
                    return typeof(DAQ_HighSpeed);
                case Meter.DataAcquisitionMode.TriggerWait:
                    return typeof(DAQ_TriggerWait);
                default:
                    return isLegacyMeterless ? typeof(DAQ_SlowAscii) : typeof(DAQ_SlowBinary);
            }
        }

        [API(APICategory.Unclassified)]
        [DataMember]
        public uint Capacity
        {
            get => this.capacity;
            set
            {
                if (value <= 0U)
                {
                    this.TraceValidationError("Capacity: validation error {0}", (object)value);
                    value = this.DefaultCapacity;
                }
                this.capacity = value;
            }
        }

        [API(APICategory.Unclassified)]
        [DataMember]
        [PreviouslyNamed("AutoRestartEnabled")]
        public bool ContinuousMode { get; set; }

        [API(APICategory.Unclassified)]
        [DataMember]
        public bool SnapshotModeEnabled { get; set; }

        [API(APICategory.Unclassified)]
        [DataMember]
        public uint PreTrigger { get; set; }

        [API(APICategory.Unclassified)]
        public bool SnapshotSettingsValid => this.Sensor_IsSnapshotCapable && this.OperatingMode_IsPower && !this.SmoothingEnabled && this.HighSpeedChannel_IsSelected && !this.AutoRangeSelected && !this.WaitForTriggerModeEnabled && this.Capacity <= this.SnapshotMaxCapacity;

        [API(APICategory.Unclassified)]
        [DataMember]
        [PreviouslyNamed("WaitForTriggerEnabled")]
        public bool WaitForTriggerModeEnabled { get; set; }

        [API(APICategory.Unclassified)]
        public bool WaitTriggerSettingsValid => this.HighSpeedAllowed && this.HighSpeedChannel_IsSelected && !this.SmoothingEnabled && this.OperatingMode_IsPower && !this.SnapshotMode_IsSelected;

        public bool WaitTriggerMode_IsAllowed => this.HighSpeedAllowed;

        [API(APICategory.Unclassified)]
        public DataFieldFlags SelectedDataFields
        {
            get => DataRecordBase.SelectedDataFields;
            set
            {
                this.Device.ConfigureDataFields_AsEnum = value;
                DataRecordBase.SelectedDataFields = value;
            }
        }

        [API(APICategory.Unclassified)]
        public bool SelectedDataFields_HasPeriod => (uint)(this.SelectedDataFields & DataFieldFlags.Period) > 0U;

        [API(APICategory.Unclassified)]
        public DataEncoding SelectedDataEncoding
        {
            get => this.selectedDataEncoding;
            set => this.Device.DataEncoding_AsEnum = this.selectedDataEncoding = value;
        }

        public abstract bool UploadSequenceIDs { get; set; }

        public abstract bool CanChangeUploadSequenceIDs { get; }

        public abstract void SetAcquisitionMode();

        public abstract void SetPollingMode();

        [API(APICategory.Unclassified)]
        public DAQ_Thread DataAcquisitionThread { get; protected set; }

        [API(APICategory.Unclassified)]
        public bool IsRunning => this.Device != null && this.DataAcquisitionThread != null && this.DataAcquisitionThread.IsRunning;

        [API(APICategory.Unclassified)]
        public bool IsWaiting => this.IsRunning && this.DataAcquisitionThread.IsWaiting;

        [API(APICategory.Unclassified)]
        public bool DAQ_Thread_WasTerminated => this.DataAcquisitionThread != null && this.DataAcquisitionThread.WasTerminated;

        [API(APICategory.Unclassified)]
        public double Progress => this.Device == null || this.DataAcquisitionThread == null || !this.DataAcquisitionThread.IsRunning ? 0.0 : this.DataAcquisitionThread.Progress;

        [API(APICategory.Unclassified)]
        public DAQ_State DAQ_State => this.Device == null || this.DataAcquisitionThread == null || !this.DataAcquisitionThread.IsRunning ? DAQ_State.Progress : this.DataAcquisitionThread.DAQ_State;

        public virtual void Start()
        {
            this.TraceStartStop("Meter.Start {0}{1}{2}, DAQ_Mode: {3}", (object)this.Capacity, this.SnapshotMode_IsSelected ? (object)" /Snap" : (object)"", this.ContinuousMode ? (object)" /Cont" : (object)"", (object)this.DAQ_Mode);
            this.SetAcquisitionMode();
            this.DataAcquisitionThread = this.DAQ_ThreadCreate();
            Timestamper.SetSamplePeriod((IDaqMeter)this);
            Timestamper.Clear();
            RecorderConfig singleton = RecorderConfig.Singleton;
            if ((singleton != null ? (singleton.RecordingEnabled ? 1 : 0) : 0) != 0)
                RecorderConfig.Singleton.PrepareToStart(this);
            this.DataAcquisitionThread.Start();
        }

        public void Stop()
        {
            this.TraceStartStop(string.Format("Meter.Stop {0} - - - - - - - - - - ", (object)this.DataAcquisitionThread?.ThreadState));
            if (this.DataAcquisitionThread == null)
                return;
            this.DataAcquisitionThread.Stop();
            this.DataAcquisitionThread.WaitThreadExits();
        }

        [API(APICategory.Unclassified)]
        public void ForceTrigger() => this.DataAcquisitionThread.RequestForceTrigger();

        [API(APICategory.Unclassified)]
        public uint SnapshotMaxCapacity => !this.IsPmPro_SystemType ? 240000U : 25000U;

        [API(APICategory.Unclassified)]
        public uint MaximumCapacity => !this.SnapshotModeEnabled ? 1000000U : this.SnapshotMaxCapacity;

        [API(APICategory.Unclassified)]
        public uint DefaultCapacity => Math.Min(50000U, this.MaximumCapacity);

        public Device Device { get; protected set; }

        [API(APICategory.Unclassified)]
        public CaptureBuffer CaptureBuffer { get; protected set; }

        public void SetCaptureBuffer(CaptureBuffer CaptureBuffer) => this.CaptureBuffer = CaptureBuffer;

        [API(APICategory.Unclassified)]
        public bool IsInitialized { get; protected set; }

        [API(APICategory.Unclassified)]
        public bool IsDeviceOpen => this.Device != null && this.Device.IsOpen;

        [API(APICategory.Unclassified)]
        public bool IsOpen => this.IsDeviceOpen && this.IsInitialized;

        public string BaudRate { get; protected set; }

        public static IErrorReporter ErrorReporter;// => ServiceLocator.Resolve<IErrorReporter>();

        public void ReportError(string format, params object[] args)
        {
            if (Meter.ErrorReporter == null)
                throw new MeterException(format, args);
            Meter.ErrorReporter.ReportError(format, args);
        }

        [API(APICategory.Unclassified)]
        public event Library.DAQ_StateChanged DAQ_StateChanged;

        protected Meter(Device Device)
        {
            this.Device = Device;
            this.WavelengthsTable = new WavelengthsTable();
            this.SelectedSensor = new Sensor();
        }

        protected Meter()
        {
        }

        public void Open(Channel channel)
        {
            try
            {
                this.Device.Open(channel);
            }
            catch (Exception ex)
            {
                this.Device.Close();
                throw new DeviceOpenException(ex);
            }
            try
            {
                this.ReloadProperties();
                this.IsInitialized = true;
            }
            catch (Exception ex)
            {
                this.Device.Close();
                throw new LoadPropertiesException(ex);
            }
        }

        public void OpenRaw(Channel channel)
        {
            this.Device.OpenRaw(channel);
            this.IsInitialized = true;
        }

        public void Close()
        {
            this.IsInitialized = false;
            if (this.Device == null || !this.Device.IsOpen)
                return;
            this.Device.Close();
        }

        public void SensorDisconnected() => this.SelectedSensor.IsInitialized = false;

        public virtual void ReloadProperties()
        {
            this.ReloadMeterProperties();
            if (this.SystemStatus_SensorValid)
                this.ReloadSensorProperties();
            else
                this.Trace("Meter.ReloadProperties INVALID Sensor");
        }

        [Conditional("WAIT_SENSOR_VALID")]
        protected void WaitSensorValid()
        {
            int num = 0;
            while (!this.SystemStatus_SensorValid && num++ < 16)
            {
                Thread.Sleep(25);
                this.UpdateSystemStatusAndFaults();
            }
        }

        protected virtual void ReloadMeterProperties()
        {
            this.SetPollingMode();
            this.Identification = this.Device.Identification;
            this.Device.LoadSystemType();
            this.UpdateSystemStatusAndFaults();
            this.ModelName = this.Device.ModelName;
            this.MeterType = this.Device.MeterType;
            this.MeterHasPyro = this.Device.MeterHasPyro;
            this.PartNumber = this.Device.PartNumber;
            this.SerialNumber = this.Device.SerialNumber;
            this.CalibrationDate = this.Device.CalibrationDate;
            this.ManufactureDate = this.Device.ManufactureDate;
            this.FirmwareVersion = this.Device.FirmwareVersion;
            this.FpgaFirmwareVersion = this.Device.FpgaFirmwareVersion;
            this.FpgaHardwareVersion = this.Device.FpgaHardwareVersion;
            this.BaudRate = this.Device.BaudRate;
            if (this.ObsoleteHardwareOrFirmware())
                throw new ObsoleteHardwareOrFirmwareException("");
            this.areaCorrection_Enabled = false;
            this.areaCorrectionIsDiameter = false;
            this.areaCorrectionValue = 1.0;
            this.gainCompensationFactor = 1.0;
            this.gainCompensation_Enabled = false;
            this.decimationRate = 1U;
            this.decimation_Enabled = false;
            this.SmoothingEnabled = false;
            this.speedup_Enabled = false;
            this.WaitForTriggerModeEnabled = false;
            this.SnapshotModeEnabled = false;
            this.ContinuousMode = true;
            this.Capacity = this.DefaultCapacity;
            this.PreTrigger = 0U;
            this.TriggerLevelMinimum = this.Device.TriggerLevelMinimum_AsReal;
            this.TriggerLevelMaximum = this.Device.TriggerLevelMaximum_AsReal;
            this.measurementWindow = 100U;
            this.operatingMode = OperatingMode.PowerWatts;
            this.selectedRange = 0.0;
            this.wavelengthCorrection = 0U;
            this.selectedRange = this.Device.SelectedRange_AsReal;
            this.triggerLevel = this.Device.TriggerLevel_AsReal;
            this.ReloadSelectedChannelAndRange();
            this.ClearPendingChanges();
        }

        public virtual void ReloadSensorProperties()
        {
            this.SelectedSensor.Reload(this.Device);
            this.operatingMode = this.Device.OperatingMode_AsEnum;
            this.ReloadSelectedChannelAndRange();
            List<uint> wavelengthTableAsList = this.Device.WavelengthTable_AsList;
            if (wavelengthTableAsList == null)
                this.ReportError("Meter: Empty WavelengthTable from device");
            this.WavelengthsTable.LoadOriginals((IEnumerable<uint>)wavelengthTableAsList);
            this.wavelengthCorrection = this.Device.WavelengthCorrectionValue_AsUint;
            this.TriggerSource = TriggerSource.Internal;
            this.TriggerSlope = TriggerSlope.Positive;
            this.TriggerDelay = 0U;
            this.TriggerLevel_LPEM = TriggerLevel_LPEM.Low;
            this.ReloadTriggerLevels();
            this.ReloadSensorHasTemperature();
            this.ClearPendingChanges();
            this.SelectedSensor.IsInitialized = true;
        }

        public void ReloadSelectedChannelAndRange()
        {
            this.SensorSpeeds = this.Device.QuerySpeedList_AsEnum;
            this.selectedChannel = this.Device.MeasurementSpeed_AsEnum;
            if (this.selectedChannel == MeasurementChannelFlags.None)
            {
                this.Device.MeasurementSpeed_AsEnum = MeasurementChannelFlags.Slow;
                this.selectedChannel = this.Device.MeasurementSpeed_AsEnum;
            }
            if (this.SystemStatus_SensorValid)
            {
                this.RangeTable = new RangeTable(this.Device.QueryRangeList_AsList);
                this.selectedRange = this.Device.SelectedRange_AsReal;
                this.autoRangeSelected = this.selectedChannel == MeasurementChannelFlags.Fast && this.Device.EnableAutoRange_AsBool;
            }
            else
            {
                this.RangeTable = new RangeTable();
                this.SelectedRange = this.RangeTable[0];
            }
            this.MaximumRange = this.Device.RangeMax_AsReal;
            this.MinimumRange = this.Device.RangeMin_AsReal;
        }

        [Conditional("TRACE_BACK")]
        public void TraceBack()
        {
            StackTrace stackTrace = new StackTrace(true);
            for (int index = 1; index < stackTrace.FrameCount; ++index)
            {
                MethodBase method = stackTrace.GetFrame(index).GetMethod();
                if (method.Name == "OpenMeterPort" || method.Name == "ConnectNewSensor")
                    break;
            }
        }

        [API(APICategory.Unclassified)]
        public virtual PropertyChangedType PendingChanges { get; protected set; }

        [API(APICategory.Unclassified)]
        public void ClearPendingChanges() => this.PendingChanges = PropertyChangedType.None;

        [API(APICategory.Unclassified)]
        public virtual PropertyChangedType GetAndClearPendingChanges()
        {
            int pendingChanges = (int)this.PendingChanges;
            this.ClearPendingChanges();
            return (PropertyChangedType)pendingChanges;
        }

        protected void DetectAutomaticChanges(PropertyChangedType extra = PropertyChangedType.None)
        {
            if (!this.HasSensor)
                return;
            List<double> queryRangeListAsList = this.Device.QueryRangeList_AsList;
            double selectedRangeAsReal = this.Device.SelectedRange_AsReal;
            if (this.RangeTable != null && queryRangeListAsList != null && !this.RangeTable.Equals(queryRangeListAsList))
            {
                this.RangeTable = new RangeTable(queryRangeListAsList);
                this.selectedRange = selectedRangeAsReal;
                this.PendingChanges |= PropertyChangedType.RangeTable | PropertyChangedType.SelectedRange;
            }
            else if (selectedRangeAsReal != this.SelectedRange)
            {
                this.selectedRange = selectedRangeAsReal;
                this.PendingChanges |= PropertyChangedType.SelectedRange;
            }
            this.DetectTriggerLevelChanges(this.PendingChanges | extra);
        }

        protected void DetectTriggerLevelChanges(PropertyChangedType extra = PropertyChangedType.None)
        {
            if (this.Device == null)
                return;
            this.PendingChanges |= extra;
            double triggerLevelAsReal = this.Device.TriggerLevel_AsReal;
            if (triggerLevelAsReal == this.TriggerLevel)
                return;
            this.ReloadTriggerMinMax();
            this.triggerLevel = triggerLevelAsReal;
            this.PendingChanges |= PropertyChangedType.TriggerLevel;
        }

        [API(APICategory.Unclassified)]
        public SystemStatusBits SystemStatus { get; protected set; }

        [API(APICategory.Unclassified)]
        public SystemFaultBits SystemFaults { get; protected set; }

        public void UpdateSystemStatusAndFaults()
        {
            this.SystemStatus = this.Device.SystemStatus_AsEnum;
            this.SystemFaults = this.Device.SystemFaults_AsEnum;
        }

        public void UpdateSystemStatusAndFaults(PollingData pollingData)
        {
            this.Device.Communicator.LockAndDelegate((Action)(() => pollingData.FetchData(this)));
            this.SystemStatus = pollingData.SystemStatus;
            this.SystemFaults = pollingData.SystemFaults;
        }

        [API(APICategory.Unclassified)]
        public bool SystemFault_IsBadZero => (this.Device.SystemStatus_AsEnum & SystemStatusBits.SystemFault) != (SystemStatusBits)0 && (uint)(this.Device.SystemFaults_AsEnum & SystemFaultBits.BadZero) > 0U;

        [API(APICategory.Unclassified)]
        public SensorState SensorState
        {
            get
            {
                if (!this.SystemStatus_SensorIsAttached)
                    return SensorState.Missing;
                return !this.HasSensor ? SensorState.Identifying : SensorState.Ready;
            }
        }

        [API(APICategory.Unclassified)]
        public virtual bool HasSensor => this.IsOpen && this.SystemStatus_SensorValid && this.SelectedSensor.IsInitialized;

        [API(APICategory.Unclassified)]
        public SystemStatusBits SystemStatus_SensorFlags => this.SystemStatus & SystemStatusBits.SensorFlags;

        [API(APICategory.Unclassified)]
        protected bool SystemStatus_SensorIsAttached => (this.SystemStatus & SystemStatusBits.SensorIsAttached) > (SystemStatusBits)0;

        [API(APICategory.Unclassified)]
        protected bool SystemStatus_BusyIdentifying => (this.SystemStatus & SystemStatusBits.BusyIdentifying) > (SystemStatusBits)0;

        [API(APICategory.Unclassified)]
        protected bool SystemStatus_SensorValid => this.SystemStatus_SensorIsAttached && !this.SystemStatus_BusyIdentifying;

        [API(APICategory.Unclassified)]
        public Sensor SelectedSensor { get; protected set; }

        [API(APICategory.Unclassified)]
        public bool Sensor_IsThermo => this.HasSensor && this.SelectedSensor.SensorType == SensorType.Thermo;

        [API(APICategory.Unclassified)]
        public bool Sensor_IsOptical => this.HasSensor && this.SelectedSensor.SensorType == SensorType.Optical;

        [API(APICategory.Unclassified)]
        public bool Sensor_IsPyro => this.HasSensor && this.SelectedSensor.SensorType == SensorType.Pyro;

        [API(APICategory.Unclassified)]
        public bool Sensor_IsQuad
        {
            get
            {
                if (!this.HasSensor)
                    return false;
                return this.SelectedSensor.SensorTypeQualifier == SensorTypeQualifier.Quad || this.SelectedSensor.SensorTypeQualifier == SensorTypeQualifier.EnhQuad;
            }
        }

        [API(APICategory.Unclassified)]
        public bool Sensor_IsPM_LM => this.Sensor_IsThermo && !this.Sensor_IsPmPro;

        [API(APICategory.Unclassified)]
        public bool Sensor_IsPmPro
        {
            get
            {
                if (!this.HasSensor)
                    return false;
                return this.IsDualSpeed || this.IsPmPro_SystemType;
            }
        }

        [API(APICategory.Unclassified)]
        public bool Sensor_IsSnapshotCapable => this.Sensor_IsPmPro;

        public double SelectedSensor_DetectorRadius => !this.HasSensor ? 9.5 : this.SelectedSensor.Diameter / 2.0;

        [API(APICategory.Unclassified)]
        public string FirmwareVersion { get; protected set; }

        [API(APICategory.Unclassified)]
        public string FpgaFirmwareVersion { get; protected set; }

        [API(APICategory.Unclassified)]
        public string FpgaHardwareVersion { get; protected set; }

        public bool ObsoleteHardwareOrFirmware()
        {
            if (!this.IsOpen || !(this.Device.GetType() == typeof(Device_SSIM)))
                return false;
            return this.ObsoleteFirmware() || this.ObsoleteHardware();
        }

        protected bool ObsoleteFirmware() => !this.IsPmPro_SystemType ? this.FirmwareVersion.OlderThan("01.00.18") : this.FirmwareVersion.OlderThan("03.00.06");

        protected bool ObsoleteHardware() => this.FpgaFirmwareVersion.OlderThan("20140303");

        [API(APICategory.Unclassified)]
        public bool HasFastEnergy => this.Device != null && this.IsOpen && this.Device.FirmwareVersion_HasFastEnergy;

        [API(APICategory.Unclassified)]
        public SystemType SystemType => this.Device.SystemType_AsEnum;

        [API(APICategory.Unclassified)]
        public bool IsSSIM_SystemType => this.SystemType == SystemType.SSIM;

        [API(APICategory.Unclassified)]
        public bool IsPmPro_SystemType => this.SystemType == SystemType.PmPro;

        public bool IsLegacyMeterlessPower => this.GetType() == typeof(Meter_MP);

        public bool IsLegacyMeterlessEnergy => this.GetType() == typeof(Meter_ME);

        [API(APICategory.Unclassified)]
        public string Identification { get; protected set; }

        [API(APICategory.Unclassified)]
        public string ModelName { get; protected set; }

        [API(APICategory.Unclassified)]
        public string PartNumber { get; protected set; }

        [API(APICategory.Unclassified)]
        public string SerialNumber { get; protected set; }

        [API(APICategory.Unclassified)]
        public string CalibrationDate { get; protected set; }

        [API(APICategory.Unclassified)]
        public string ManufactureDate { get; protected set; }

        [API(APICategory.Unclassified)]
        public string MeterType { get; protected set; }

        [API(APICategory.Unclassified)]
        public bool MeterHasPyro { get; protected set; }

        [API(APICategory.Unclassified)]
        [DataMember]
        public OperatingMode OperatingMode
        {
            get => this.operatingMode;
            set
            {
                this.operatingMode = value;
                this.Device.OperatingMode_AsEnum = value;
                this.DetectAutomaticChanges();
            }
        }

        [API(APICategory.Unclassified)]
        public bool OperatingMode_IsEnergy => this.OperatingMode == OperatingMode.EnergyJoules;

        [API(APICategory.Unclassified)]
        public bool OperatingMode_IsTrueEnergy => this.OperatingMode_IsEnergy && !this.LPEM_Mode_IsSelected;

        [API(APICategory.Unclassified)]
        public bool OperatingMode_IsPower => this.OperatingMode == OperatingMode.PowerWatts;

        [API(APICategory.Unclassified)]
        public bool HighSpeedEnergyPro => this.Sensor_IsPmPro && this.HighSpeedChannel_IsSelected && this.OperatingMode_IsEnergy;

        [API(APICategory.Unclassified)]
        public bool LPEM_Mode_IsSelected => this.LPEM_Mode_IsAllowed && this.OperatingMode_IsEnergy;

        public bool LPEM_Mode_IsAllowed => this.HasSensor && this.Sensor_IsPM_LM && !this.HighSpeedChannel_IsSelected && !(this is Meter_MP);

        [API(APICategory.Unclassified)]
        public virtual bool SlowEnergyMode_IsSelected => false;

        [API(APICategory.Unclassified)]
        [DataMember]
        public MeasurementChannelFlags SelectedChannel
        {
            get => this.selectedChannel;
            set
            {
                if ((value & this.SensorSpeeds) == (MeasurementChannelFlags)0)
                    return;
                this.selectedChannel = value;
                this.Device.MeasurementSpeed_AsEnum = value;
                this.DetectAutomaticChanges();
            }
        }

        [API(APICategory.Unclassified)]
        public bool HighSpeedChannel_IsSelected
        {
            get => (uint)(this.SelectedChannel & MeasurementChannelFlags.Fast) > 0U;
            set => this.SelectedChannel = value ? MeasurementChannelFlags.Fast : MeasurementChannelFlags.Slow;
        }

        [API(APICategory.Unclassified)]
        public MeasurementChannelFlags SensorSpeeds { get; protected set; }

        [API(APICategory.Unclassified)]
        public bool HighSpeedAllowed => (uint)(this.SensorSpeeds & MeasurementChannelFlags.Fast) > 0U;

        [API(APICategory.Unclassified)]
        public bool SlowSpeedAllowed => (uint)(this.SensorSpeeds & MeasurementChannelFlags.Slow) > 0U;

        [API(APICategory.Unclassified)]
        public bool OnlyHighSpeedAllowed => (this.SensorSpeeds & MeasurementChannelFlags.FastAndSlow) == MeasurementChannelFlags.Fast;

        [API(APICategory.Unclassified)]
        public bool IsDualSpeed => (this.SensorSpeeds & MeasurementChannelFlags.FastAndSlow) == MeasurementChannelFlags.FastAndSlow;

        [API(APICategory.Unclassified)]
        public RangeTable RangeTable { get; protected set; }

        public double MaximumRange { get; protected set; }

        public virtual double MinimumRange { get; protected set; }

        [API(APICategory.Unclassified)]
        public bool SelectedRange_IsAuto => Meter.IsAutoValue(this.SelectedRange);

        [API(APICategory.Unclassified)]
        [DataMember]
        public double SelectedRange
        {
            get => !this.AutoRangeSelected ? this.selectedRange : 0.0;
            set
            {
                this.selectedRange = value;
                if (this.HighSpeedChannel_IsSelected)
                    this.SelectedRange_HighSpeed = value;
                else
                    this.SelectedRange_LowSpeed = value;
                this.autoRangeSelected = Meter.IsAutoValue(value);
                this.FlushRangeSettings();
            }
        }

        public static bool IsAutoValue(double value) => value == 0.0;

        public bool AutoRangeSelected
        {
            get => this.autoRangeSelected;
            protected set
            {
                this.autoRangeSelected = value;
                this.FlushRangeSettings();
            }
        }

        protected void FlushRangeSettings()
        {
            if (!this.HasSensor)
                return;
            this.Device.EnableAutoRange_AsBool = this.AutoRangeSelected;
            if (!this.AutoRangeSelected)
                this.Device.SelectedRange_AsReal = this.SelectedRange;
            this.DetectTriggerLevelChanges();
        }

        public double SelectedRange_HighSpeed { get; protected set; }

        public double SelectedRange_LowSpeed { get; protected set; }

        public PhysicalUnits RangeTablePhysicalUnits => this.HasSensor && !this.Sensor_IsPmPro && this.Sensor_IsPyro ? PhysicalUnits.Joules : PhysicalUnits.Watts;

        [API(APICategory.Unclassified)]
        [DataMember]
        public WavelengthsTable WavelengthsTable { get; set; }

        public void ResetWavelengthCorrectionAndTable()
        {
            this.wavelengthCorrection = 0U;
            this.WavelengthCorrection_Enabled = false;
            this.WavelengthsTable.ClearAll();
        }

        [API(APICategory.Unclassified)]
        [DataMember]
        [PreviouslyNamed("WavelengthCorrectionValue")]
        public uint WavelengthCorrection
        {
            get => !this.WavelengthCorrection_Enabled ? 0U : this.wavelengthCorrection;
            set
            {
                this.wavelengthCorrection = value;
                this.WavelengthCorrection_Enabled = this.wavelengthCorrection > 0U;
                this.FlushWavelengthSettings();
            }
        }

        [API(APICategory.Unclassified)]
        public bool WavelengthCorrection_Enabled { get; protected set; }

        protected void FlushWavelengthSettings()
        {
            PropertyChangedType extra = PropertyChangedType.None;
            if (this.WavelengthCorrection_Enabled)
            {
                this.Device.WavelengthCorrectionValue_AsUint = this.wavelengthCorrection;
                this.Device.EnableWavelengthCorrection_AsBool = true;
                uint correctionValueAsUint = this.Device.WavelengthCorrectionValue_AsUint;
                if ((int)correctionValueAsUint != (int)this.wavelengthCorrection)
                {
                    this.wavelengthCorrection = correctionValueAsUint;
                    extra = PropertyChangedType.Wavelength;
                }
                this.DetectAutomaticChanges(extra);
            }
            else
                this.Device.EnableWavelengthCorrection_AsBool = false;
        }

        [API(APICategory.Unclassified)]
        public long SamplePeriod => this.EstimatedSamplePeriod_Ticks;

        public long EstimatedSamplePeriod_Ticks
        {
            get
            {
                double estimatedSampleRateHz = this.EstimatedSampleRate_Hz;
                return 10000000L / (long)this.EstimatedSampleRate_Hz;
            }
        }

        [API(APICategory.Unclassified)]
        public double EstimatedSampleRate_Hz => this.HighSpeedChannel_IsSelected ? this.EstimateSampleRate(this.Decimation_Enabled ? this.DecimationRate : 1U, this.FpgaDecimation) : 10.0;

        public double EstimateSampleRate(Meter meter)
        {
            uint fpgaDecimation = meter.FpgaDecimation;
            return this.EstimateSampleRate(meter.Decimation_Enabled ? meter.DecimationRate : 1U, fpgaDecimation);
        }

        public double EstimateSampleRate(uint firmwareDecimation, uint fpgaDecimation) => 40000000.0 / (double)Math.Max(1U, firmwareDecimation) / (double)Math.Max(1U, fpgaDecimation);

        [DataMember]
        public uint FpgaDecimation
        {
            get => !this.SnapshotMode_IsSelected ? this.StandardFpgaDecimation : this.SnapshotFpgaDecimation;
            set
            {
            }
        }

        [DataMember]
        public uint FpgaAveragingId
        {
            get => this.StandardFpgaAvreagingId;
            set
            {
            }
        }

        public virtual bool HasTriggering => false;

        [API(APICategory.Unclassified)]
        public double TriggerLevelMinimum { get; protected set; }

        [API(APICategory.Unclassified)]
        public double TriggerLevelMaximum { get; protected set; }

        [API(APICategory.Unclassified)]
        public double TriggerLevelPercentMinimum { get; protected set; }

        [API(APICategory.Unclassified)]
        public double TriggerLevelPercentMaximum { get; protected set; }

        public void ReloadTriggerMinMax()
        {
            this.TriggerLevelMinimum = this.Device.TriggerLevelMinimum_AsReal;
            this.TriggerLevelMaximum = this.Device.TriggerLevelMaximum_AsReal;
            this.TriggerLevelPercentMinimum = this.Device.TriggerLevelPercentMinimum_AsReal;
            this.TriggerLevelPercentMaximum = this.Device.TriggerLevePercentMaximum_AsReal;
        }

        public void ReloadTriggerLevels()
        {
            if (!this.IsDeviceOpen)
                return;
            this.ReloadTriggerMinMax();
            this.triggerLevel = this.Device.TriggerLevel_AsReal;
            this.SyncTriggerLevelPercent();
        }

        public double TriggerPercentToLevel(double percent) => percent / this.TriggerLevelPercentMaximum * this.TriggerLevelMaximum;

        public double TriggerLevelToPercent(double level) => this.TriggerLevelMaximum == 0.0 ? 0.0 : level / this.TriggerLevelMaximum * this.TriggerLevelPercentMaximum;

        protected void SyncTriggerLevel() => this.SetTriggerLevel(this.TriggerPercentToLevel(this.TriggerLevelPercent));

        protected void SyncTriggerLevelPercent() => this.triggerLevelPercent = this.TriggerLevelToPercent(this.TriggerLevel);

        [API(APICategory.Unclassified)]
        [DataMember]
        [Validator("TriggerLevel_Validator")]
        public virtual double TriggerLevel
        {
            get => this.triggerLevel;
            set
            {
                if (this.TriggerLevel_IsValid(value))
                {
                    this.SetTriggerLevel(value);
                    this.SyncTriggerLevelPercent();
                }
                else
                    this.TraceValidationError("TriggerLevel: validation error {0} <= {1} <= {2}", (object)this.TriggerLevelMinimum, (object)value, (object)this.TriggerLevelMaximum);
            }
        }

        protected void SetTriggerLevel(double value)
        {
            this.triggerLevel = value;
            if (this.IsRunning)
                this.Device.SetTriggerLevel(value);
            else
                this.Device.TriggerLevel_AsReal = value;
        }

        public bool TriggerLevel_Validator(string level)
        {
            double result;
            return double.TryParse(level, out result) && this.TriggerLevel_IsValid(result);
        }

        public bool TriggerLevel_IsValid(double level) => this.TriggerLevelMinimum <= level && level <= this.TriggerLevelMaximum;

        public string TriggerLevel_Validate(string level)
        {
            double num;
            string str = Validate.Real(level, out num);
            if (str != null)
                return str;
            if (num < this.TriggerLevelMinimum)
                return string.Format("Must be ≥ Minimum Level ({0})", (object)this.TriggerLevelMinimum);
            return num > this.TriggerLevelMaximum ? string.Format("Must be ≤ Maximum Level ({0})", (object)this.TriggerLevelMaximum) : (string)null;
        }

        [API(APICategory.Unclassified)]
        public double TriggerLevelPercent
        {
            get => this.triggerLevelPercent;
            set
            {
                if (!this.TriggerLevelPercent_Validator(value))
                    return;
                this.triggerLevelPercent = value;
                this.SyncTriggerLevel();
            }
        }

        public bool TriggerLevelPercent_Validator(double value) => value >= this.TriggerLevelPercentMinimum && value <= this.TriggerLevelPercentMaximum;

        public string TriggerLevelPercent_Validate(string percent)
        {
            double num;
            string str = Validate.Real(percent, out num);
            if (str != null)
                return str;
            if (num < this.TriggerLevelPercentMinimum)
                return string.Format("Must be ≥ Minimum % ({0})", (object)this.TriggerLevelPercentMinimum);
            return num > this.TriggerLevelPercentMaximum ? string.Format("Must be ≤ Maximum % ({0})", (object)this.TriggerLevelPercentMaximum) : (string)null;
        }

        [API(APICategory.Unclassified)]
        [DataMember]
        public TriggerSource TriggerSource
        {
            get => this.triggerSource;
            set => this.Device.TriggerSource_AsEnum = this.triggerSource = value;
        }

        [API(APICategory.Unclassified)]
        public bool TriggerSource_IsExternal => this.triggerSource == TriggerSource.External;

        [API(APICategory.Unclassified)]
        public bool TriggerSource_IsInternal => this.triggerSource == TriggerSource.Internal;

        [API(APICategory.Unclassified)]
        [DataMember]
        public TriggerSlope TriggerSlope
        {
            get => this.triggerSlope;
            set => this.Device.TriggerSlope_AsEnum = this.triggerSlope = value;
        }

        [API(APICategory.Unclassified)]
        [DataMember]
        public uint TriggerDelay
        {
            get => this.triggerDelay;
            set => this.Device.TriggerDelay_AsUint = this.triggerDelay = value;
        }

        [API(APICategory.Unclassified)]
        [DataMember]
        [PreviouslyNamed("LPEM_TriggerLevel")]
        public TriggerLevel_LPEM TriggerLevel_LPEM
        {
            get => this.triggerLevel_LPEM;
            set
            {
                if (this.IsSSIM_SystemType)
                    this.Device.TriggerLevel_LPEM_AsEnum = value;
                this.triggerLevel_LPEM = value;
            }
        }

        [API(APICategory.Unclassified)]
        [DataMember]
        public uint MeasurementWindow
        {
            get => this.measurementWindow;
            set
            {
                this.Device.MeasurementWindow_AsUint = this.measurementWindow = value < 25U ? 25U : (value > 10000000U ? 10000000U : value);
                this.DetectAutomaticChanges();
            }
        }

        public double MinAreaCorrectionValue(bool areaCorrectionIsDiameter) => !areaCorrectionIsDiameter ? 0.01 : this.MinAreaCorrectionDiameter;

        public double MaxAreaCorrectionValue(bool areaCorrectionIsDiameter) => !areaCorrectionIsDiameter ? 999.0 : this.MaxAreaCorrectionDiameter;

        public bool AreaCorrectionValue_IsValid(double value) => Validate.Real(value, new double?(this.MinAreaCorrectionValue(this.areaCorrectionIsDiameter)), new double?(this.MaxAreaCorrectionValue(this.areaCorrectionIsDiameter))) == null;

        public string AreaCorrectionValue_Validate(string text) => Validate.Real(text, new double?(this.MinAreaCorrectionValue(this.areaCorrectionIsDiameter)), new double?(this.MaxAreaCorrectionValue(this.areaCorrectionIsDiameter)));

        public void FlushAreaCorrectionSettings()
        {
            if (!this.IsOpen)
                return;
            this.Device.EnableAreaCorrection_AsBool = this.AreaCorrection_Enabled;
            if (this.AreaCorrection_Enabled)
                this.Device.AreaCorrectionValue_AsReal = this.ActualAreaCorrectionValue;
            this.DetectAutomaticChanges();
        }

        [API(APICategory.Unclassified)]
        [DataMember]
        public bool AreaCorrection_Enabled
        {
            get => this.areaCorrection_Enabled;
            set
            {
                this.areaCorrection_Enabled = value;
                this.FlushAreaCorrectionSettings();
            }
        }

        [API(APICategory.Unclassified)]
        [DataMember]
        public double AreaCorrectionValue
        {
            get => this.areaCorrectionValue;
            set
            {
                if (!Validate.Real_GZ(value))
                {
                    this.TraceValidationError("AreaCorrectionValue: validation error {0}", (object)value);
                    value = 1.0;
                }
                this.areaCorrectionValue = value;
                this.FlushAreaCorrectionSettings();
            }
        }

        [API(APICategory.Unclassified)]
        [DataMember]
        [PreviouslyNamed("AreaCorrectionIsPerCm")]
        public bool AreaCorrectionIsDiameter
        {
            get => this.areaCorrectionIsDiameter;
            set
            {
                this.areaCorrectionIsDiameter = value;
                this.FlushAreaCorrectionSettings();
            }
        }

        private double ActualAreaCorrectionValue => !this.areaCorrectionIsDiameter ? this.AreaCorrectionValue : this.DiameterToArea(this.AreaCorrectionValue);

        private double DiameterToArea(double diameter) => this.RadiusToArea(diameter / 2.0);

        private double RadiusToArea(double radius) => Math.PI * radius * radius;

        private double SqareToArea(double side) => side * side;

        private double RectangleToArea(double side1, double side2) => side1 * side2;

        private static double AreaToDiameter(double area) => 2.0 * Math.Sqrt(area / Math.PI);

        public bool GainCompensationFactor_Validate(double value) => Validate.Real(value, new double?(0.001), new double?(100000.0)) == null;

        public bool GainCompensationFactor_Validator(string text) => Validate.Real(text, new double?(0.001), new double?(100000.0)) == null;

        public string GainCompensationFactor_Validate(string text) => Validate.Real(text, new double?(0.001), new double?(100000.0));

        [API(APICategory.Unclassified)]
        [DataMember]
        public bool GainCompensation_Enabled
        {
            get => this.gainCompensation_Enabled;
            set
            {
                this.Device.EnableGainCompensation_AsBool = this.gainCompensation_Enabled = value;
                this.DetectAutomaticChanges();
            }
        }

        [API(APICategory.Unclassified)]
        [DataMember]
        [Validator("GainCompensationFactor_Validator")]
        public double GainCompensationFactor
        {
            get => this.gainCompensationFactor;
            set
            {
                if (!this.GainCompensationFactor_Validate(value))
                {
                    this.TraceValidationError("GainCompensationFactor: validation error {0}", (object)value);
                }
                else
                {
                    this.Device.GainCompensationFactor_AsReal = this.gainCompensationFactor = value;
                    this.DetectAutomaticChanges();
                }
            }
        }

        [API(APICategory.Unclassified)]
        [DataMember]
        public bool Decimation_Enabled
        {
            get => this.decimation_Enabled;
            set
            {
                this.decimation_Enabled = value;
                if (this.decimation_Enabled)
                    this.Device.DecimationRate_AsUint = this.decimationRate;
                else
                    this.Device.DecimationRate_AsUint = 1U;
            }
        }

        [API(APICategory.Unclassified)]
        [DataMember]
        [Validator("DecimationRate_Validator")]
        public uint DecimationRate
        {
            get => this.decimationRate;
            set
            {
                if (!this.DecimationRate_Validate(value))
                {
                    this.TraceValidationError("DecimationRate: validation error {0}", (object)value);
                }
                else
                {
                    this.decimationRate = value;
                    if (!this.Decimation_Enabled)
                        return;
                    this.Device.DecimationRate_AsUint = value;
                }
            }
        }

        public bool DecimationRate_Validate(uint value) => value > 0U;

        public bool DecimationRate_Validator(string text) => Validate.UInt_GZ(text) == null;

        public string DecimationRate_Validate(string text) => Validate.UInt_GZ(text);

        [API(APICategory.Unclassified)]
        [DataMember]
        public virtual bool SmoothingEnabled
        {
            get => false;
            set
            {
            }
        }

        [API(APICategory.Unclassified)]
        public virtual bool SmoothingAllowed => false;

        [API(APICategory.Unclassified)]
        [DataMember]
        public bool Speedup_Enabled
        {
            get => this.SpeedupAllowed && this.speedup_Enabled;
            set
            {
                this.speedup_Enabled = value;
                if (!this.SpeedupAllowed)
                    return;
                this.Device.EnableSpeedup_AsBool = this.speedup_Enabled;
            }
        }

        [API(APICategory.Unclassified)]
        public virtual bool SpeedupAllowed => this.HasSensor && !this.Sensor_IsPmPro && !this.Sensor_IsPyro && !this.OperatingMode_IsEnergy;

        [API(APICategory.Unclassified)]
        [DataMember]
        public AnalogOutputLevel AnalogOutputLevel
        {
            get => !this.IsPmPro_SystemType ? this.analogOutputLevel : AnalogOutputLevel.Two;
            set
            {
                this.analogOutputLevel = value;
                if (this.IsPmPro_SystemType)
                    return;
                this.Device.AnalogOutputLevel_AsEnum = value;
            }
        }

        [API(APICategory.Unclassified)]
        public virtual bool AnalogOutputAllowed
        {
            get
            {
                if (this.IsPmPro_SystemType)
                    return false;
                if (!this.Sensor_IsPmPro)
                    return !this.Sensor_IsPyro;
                return this.OperatingMode_IsPower && !this.SnapshotMode_IsSelected;
            }
        }

        [API(APICategory.Unclassified)]
        public string SensorTemperature => !this.SystemStatus_SensorValid || !this.SensorHasTemperature ? "NA" : this.Device.SensorTemperature;

        [API(APICategory.Unclassified)]
        public bool SensorHasTemperature { get; set; }

        protected void ReloadSensorHasTemperature() => this.SensorHasTemperature = this.SystemStatus_SensorValid && !this.Sensor_IsPyro && !this.Sensor_IsOptical && !this.Device.SensorTemperature.IsNA();

        [API(APICategory.Unclassified)]
        public virtual void ZeroMeter()
        {
            if (this.Sensor_IsPyro)
                throw new NotSupportedException("Zeroing not allowed for Pyro sensors");
            this.Device.ConfigureZero();
        }

        [API(APICategory.Unclassified)]
        public virtual void WaitZeroMeter()
        {
        }

        public virtual double QueryZeroBaseline() => 0.0;

        public virtual bool HasZeroBaseline => this.IsSSIM_SystemType && this.Sensor_IsOptical;

        public void SystemRestore() => this.Device.SystemRestore();

        public void SendCommand(string command)
        {
            if (!this.IsOpen)
                return;
            try
            {
                this.Device.SendCommand(command);
            }
            catch
            {
            }
        }

        public DataRecordSingle DecodeMeasurement(string line) => this.Device.DecodeMeasurement(line);

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(this.GetType().Name);
            stringBuilder.Append(this.Device.Channel.IsOpen ? " [Open on " + this.Device.Channel.PortName + "]" : " [Closed]");
            return stringBuilder.ToString();
        }

        public void Trace(string message)
        {
            string name = this.GetType().Name;
        }

        public void Trace(string format, params object[] args) => this.Trace(string.Format(format, args));

        [Conditional("TRACE_OPEN")]
        public void TraceOpen(string format, params object[] args) => this.Trace(format, args);

        [Conditional("TRACE_BACK")]
        public void TraceBack(string format, params object[] args) => this.Trace("Stack: " + format, args);

        [Conditional("TRACE_START_STOP")]
        public void TraceStartStop(string format, params object[] args) => this.Trace(format, args);

        [Conditional("TRACE_START_STOP_EXITS")]
        public void TraceStartStopExits(string fmt, params object[] args) => this.Trace(fmt, args);

        [Conditional("TRACE_OPMODE")]
        public void TraceOpMode(string format, params object[] args) => this.Trace(format, args);

        [Conditional("TRACE_RANGE")]
        public void TraceRange(string format, params object[] args) => this.Trace(format, args);

        [Conditional("TRACE_TRIGGERS")]
        public void TraceTriggers(string format, params object[] args) => this.Trace(format, args);

        [Conditional("TRACE_NOTE")]
        public void TraceNote(string format, params object[] args) => this.Trace(format, args);

        [Conditional("TRACE_DATA")]
        public void TraceData(string format, params object[] args) => this.Trace(format, args);

        public void TraceValidationError(string format, params object[] args) => this.Trace(format, args);

        [Conditional("TRACE_LOAD_CACHE")]
        public void TraceLoadCache(string format, params object[] args) => this.Trace(format, args);

        [Conditional("TRACE_SAMPLE_RATE")]
        public void TraceSampleRate(string format, params object[] args) => this.Trace(format, args);

        [Conditional("TRACE_AUTO_UPDATE")]
        public void TraceAutoUpdate(string format, params object[] args) => this.Trace(format, args);

        [Conditional("TRACE_WAVE")]
        public void TraceWave(string format, params object[] args)
        {
        }

        [API(APICategory.Unclassified)]
        public enum DataAcquisitionMode
        {
            Snapshot,
            HighSpeed,
            TriggerWait,
            Quad,
            Power,
        }
    }
}
