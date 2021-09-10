using DEV.PowerMeter.Library.DeviceModels;
using DEV.PowerMeter.Library.ImportExport;
using DEV.PowerMeter.Library.Interfaces;
using DEV.PowerMeter.Library.ViewModels;
using SharedLibrary;
using System;
using System.Diagnostics;

namespace DEV.PowerMeter.Library
{
    [API(APICategory.Measurement, "PhoenixMeter object collects the Meter, CaptureBuffer, PollingThread, PreviewBufferController,\r\n\t\tand various other objects and integrates them into a functioning meter subsystem.")]
    public class PhoenixMeter : IPhoenixMeter
    {
        public AlarmsAndLimits.AlarmsAndLimits mAlarmsAndLimits;
        [API(APICategory.SeeAlso, "<h1>The two primary use cases for opening a meter</h1>:\r\n<OL>\r\n<LI>Meter has attached sensor which is ready to go. \r\nThis is the most common use case.\r\n\r\nIn this case we initialize all meter and sensor properties so that\r\nPhoenixMeter is ready to accept commands. \r\n\r\nThis is all we need for unit-testing purposes. \r\n\r\nIn the context of a UI: \r\n\r\nA big change is that we no longer fire a SensorConnected event \r\nwhen opened in this state.\r\n\r\nInstead the UI's Open method (which presumably called this.Open \r\nin the first place) optionally applies additional settings \r\nto the meter, and then updates the UI based on the result. \r\n</LI>\r\n<LI>Meter has no sensor attached. This is a common use case. However\r\nthere is hardly anything the meter can do without a sensor. So this\r\ninitialization is just a place-holder until a sensor is attached.\r\n\r\nThis use case includes the case where the Meter has sensor attached \r\nbut the sensor is busy Identifying (during which time the sensor is just \r\nas useless as if it wasn't there).\r\n\r\nIn this case we initialize meter properties best we can, substituting \r\ngeneric defaults where necessary, so the UI can be updated without crashing.\r\nHowever, MOST operations are disallowed without an attached Sensor.\r\n\r\nLater, if/when a valid sensor is attached, we'll update the sensor-specific\r\nproperties, and send a SensorDetected event to the UI, which then will load\r\nthe sensor-specific settings and update the UI.\r\n</LI>\r\n</OL>\r\n")]
        public object OpenMeterUseCase;
        protected bool CancelAutoRestart;
        protected bool FirstSnapshotStart;
        protected readonly DAQ_StateEventArgs StateEventStopping = new DAQ_StateEventArgs(DAQ_State.Stopping);
        protected readonly DAQ_StateEventArgs DAQ_StateEventRestart = new DAQ_StateEventArgs(DAQ_State.Restart);

        public PortManager PortManager => ServiceLocator.Resolve<PortManager>();

        [API("The PhoenixMeter object's Meter object.")]
        public Meter Meter { get; protected set; }

        [API("The PhoenixMeter object's PollingThread.")]
        public PollingThread PollingThread { get; protected set; }

        [API("The PhoenixMeter object's Sensor. ")]
        public Sensor SelectedSensor => this.Meter == null ? (Sensor)null : this.Meter.SelectedSensor;

        [API("The PhoenixMeter object's CaptureBuffer. CaptureBuffer is a Singleton,\r\nin that there is only one unique instance used for DataAcquisition\r\nfor any given PhoenixMeter instance. \r\nHowever this instance may be replaced from time to time, \r\nand there may be other CaptureBuffers elsewhere in the app, used for other purposes.")]
        public CaptureBuffer CaptureBuffer => this.Meter == null ? (CaptureBuffer)null : this.Meter.CaptureBuffer;

        [API("Manually change to a new CaptureBuffer")]
        protected void SetCaptureBuffer(CaptureBuffer CaptureBuffer)
        {
            this.Meter.SetCaptureBuffer(CaptureBuffer);
            if (CaptureBuffer.Header == null || CaptureBuffer.Header.Units == null)
                return;
            MagnitudeConverter.Units = CaptureBuffer.Units;
        }

        [API("The PhoenixMeter object's PreviewBufferController. \r\n\t\t\tPreviewBufferController periodically updates the PreviewBuffer,\r\n\t\t\twith the most recent data from the CaptureBuffer, when the Meter is Running.\r\n\t\t\tThis PreviewBufferController is a Singleton, \r\n\t\t\tin that there is just one unique instance for any given PhoenixMeter instance.")]
        public PreviewBufferController PreviewBufferController { get; protected set; }

        [API("The PhoenixMeter object's DataLogger object. \r\n\t\t\tThis DataLogger is a Singleton for any given PhoenixMeter instance.")]
        public DataLogger DataLogger { get; protected set; }

        [API("The PhoenixMeter object's Statistics object. \r\n\t\t\tThis Statistics object is a Singleton for any given PhoenixMeter instance.")]
        public Statistics Statistics { get; protected set; }

        [API("The PhoenixMeter object's Computations object. \r\n\t\t\tThis Computations object is a Singleton for any given PhoenixMeter instance. ")]
        public Computations Computations { get; protected set; }

        [API("The PhoenixMeter object's instance of PulseAnalyzer. \r\n\t\t\tThis Statistics object is a Singleton for any given PhoenixMeter instance.")]
        public PulseAnalyzer PulseAnalyzer => this.Computations.PulseAnalyzer;

        [API("The PhoenixMeter object's instance of PulseAnalysisOptions. \r\n\t\t\tThis PulseAnalysisOptions object is a Singleton for any given PhoenixMeter instance. ")]
        public PulseAnalysisOptions PulseAnalysisOptions => this.Computations.PulseAnalysisOptions;

        public IErrorReporter ErrorLogger => ServiceLocator.Resolve<IErrorReporter>();

        public void ReportError(string format, params object[] args)
        {
            if (this.ErrorLogger == null)
                throw new MeterException(format, args);
            this.ErrorLogger.ReportError(format, args);
        }

        [API("True iff Meter is open on a port. Changes trigger a MeterStatusChanged event: MeterConnected or MeterDisconnected.")]
        public bool IsOpen => this.Meter != null && this.Meter.IsOpen;

        [API("True iff Meter is Running -- acquiring data. \r\nChanges trigger a MeterStatusChanged event: MeterStarted or MeterStopped.")]
        public bool IsRunning => this.Meter != null && this.Meter.IsRunning;

        [API("True iff Meter has a Sensor attached that has completed initialization. \r\nChanges trigger a MeterStatusChanged event: SensorDisconnected, SensorIdentifying or SensorConnected.")]
        public bool HasSensor => this.Meter != null && this.Meter.HasSensor;

        [API("The PhoenixMeter object's Sensor Type")]
        public SensorType SelectedSensorType => this.SelectedSensor == null ? SensorType.None : this.SelectedSensor.SensorType;

        [API("The PhoenixMeter object's current Operating Mode")]
        public OperatingMode OperatingMode
        {
            get => this.Meter == null ? OperatingMode.PowerWatts : this.Meter.OperatingMode;
            set
            {
                if (this.Meter == null || !this.Meter.IsOpen)
                    return;
                this.Meter.OperatingMode = value;
            }
        }

        [API("Set DataAcquisition mode: Continuous (run until explicitly Stopped) \r\n\t\t\tvs. Non-Continuous (stop soon as single buffer's worth of data has been acquired).")]
        public bool Continuous
        {
            get => this.Meter != null && this.Meter.ContinuousMode;
            set => this.Meter.ContinuousMode = value;
        }

        [API("Enable or disable Polling")]
        public bool Polling_Enabled
        {
            get => this.PollingThread.Polling_Enabled;
            set => this.PollingThread.Polling_Enabled = value;
        }

        [API("Enable or disable periodic measurements updates")]
        public bool Updates_Enabled
        {
            get => this.PollingThread.Update_Enabled;
            set => this.PollingThread.Update_Enabled = value;
        }

        [API("Temporarily suspend Polling")]
        public void Suspend_Polling() => this.PollingThread.Suspend();

        [API("Resume suspend Polling")]
        public void Resume_Polling() => this.PollingThread.Resume();

        [API("The PhoenixMeter object's present MeterStatus")]
        public MeterStatus MeterStatus { get; protected set; }

        [API("Event fired when this object's MeterStatus changes")]
        public event Library.MeterStatusChanged MeterStatusChanged;

        [API("Event fired when this object's Data Acquisition State changes")]
        public event Library.DAQ_StateChanged DAQ_StateChanged;

        [API("Event fired when to effect periodic Polling Updates, only when Meter is not running (and only if Update_Enabled=True)")]
        public event Action<PollingData> MeasurementUpdate;

        [API("Normal constructor. Creates a PhoenixMeter object, \r\nall the necessary component objects, and maps their events to PhoenixMeter events.\r\nResulting instance must be Opened before it can do anything.")]
        public PhoenixMeter()
        {
            this.Statistics = new Statistics();
            this.Computations = new Computations();
            this.PreviewBufferController = new PreviewBufferController();
            this.DataLogger = new DataLogger();
            this.PollingThread = new PollingThread();
            this.PollingThread.SensorFlagsChanged += new Action<SystemStatusBits, SystemStatusBits>(this.OnPollingThread_SensorFlagsChanged);
            this.PollingThread.MeasurementUpdate += new Action<PollingData>(this.PollingThread_MeasurementUpdate);
            this.PollingThread.SystemFaultDetected += new Action<SystemFaultBits>(this.OnSystemFaultDetected);
            this.PollingThread.MeterDisconnected += new Action(this.OnMeterDisconnected);
            this.AttachMeter((Meter)new Meter_SSIM());
            this.mAlarmsAndLimits = new Library.AlarmsAndLimits.AlarmsAndLimits(NonvolatileUserSettings.getInstance(), this);
        }

        protected void AttachMeter(Meter meter)
        {
            if (this.Meter != null)
                this.Meter.DAQ_StateChanged -= new Library.DAQ_StateChanged(this.Meter_DAQ_StateChanged);
            this.Meter = meter;
            this.PollingThread.AttachMeter(meter);
            if (this.Meter == null)
                return;
            this.InitializeBuffers();
            this.Meter.DAQ_StateChanged += new Library.DAQ_StateChanged(this.Meter_DAQ_StateChanged);
        }

        [API("Always called before closing, generally in the Window.Closing event. \r\n\t\t\tIt gracefully shuts down meter subsystems in the proper order and optionally saves meter settings.")]
        public void Shutdown(Action saveSettings = null)
        {
            if (this.Meter.IsRunning)
                this.Stop();
            this.DataLogger.Shutdown();
            this.PollingThread.Terminate();
            if (saveSettings != null)
                saveSettings();
            this.Meter.Close();
        }

        [API("Open the meter on a specific com port. \r\n\t\t\tGenerally stays open until Shutdown is called, which does the close for you.")]
        public void Open(string portName) => this.Open(this.PortManager.CreateOpenChannel(portName));

        [API("Open the meter on a specific Channel. \r\n\t\t\tGenerally stays open until Shutdown is called, which calls close.")]
        public void Open(Channel channel)
        {
            this.OpenMeter(channel);
            this.OnMeterConnected();
            if (!this.Meter.HasSensor)
                return;
            this.OnSensorConnected();
        }

        protected void OpenMeter(Channel channel)
        {
            try
            {
                this.Meter = (Meter)Activator.CreateInstance(PortManager.ValidateDevice(channel).MeterClassType);
                this.Meter.Open(channel);
                CMC_CLA.Current.GetAuthorization((ILicensedMeter)this.Meter);
                this.AttachMeter(this.Meter);
            }
            catch (System.Exception ex)
            {
                this.Trace("PM.OpenMeter({0}) Exception: {1}", (object)channel.PortName, (object)ex.Message);
                channel.Close();
                throw;
            }
        }

        [API("Close the meter. When exiting an app, it's important that you call Shutdown, \r\n\t\t\tbut Close is optional. Generally only need Close if your app is \r\n\t\t\tclosing a meter in preparation of opening one on another channel.")]
        public void Close()
        {
            if (!this.IsOpen)
                return;
            this.PollingThread.CloseMeter();
            this.Meter.Close();
            this.OnSensorDisconnected();
        }

        [API("Initialize various Meter settings based on external .PROCON file.")]
        public void LoadSettings(string filename, IErrorReporter errorReporter) => new PropertySerializer("CMC_App", new object[2]
        {
      (object) this.Meter,
      (object) this.PreviewBufferController
        }, errorReporter).LoadFromFile(filename);

        [API("Save most Meter settings to external .PROCON file.")]
        public void SaveSettings(string filename, IErrorReporter errorReporter) => new PropertySerializer("CMC_App", new object[2]
        {
      (object) this.Meter,
      (object) this.PreviewBufferController
        }, errorReporter).SaveToFile(filename);

        [API("Load Meter's CaptureBuffer with contents of an external file.")]
        public void Import(string filename) => this.SetCaptureBuffer(CaptureBuffer.Import(filename));

        [API("Save Meter's CaptureBuffer contents to an external file.")]
        public void Export(string filename, bool sliceToBounds = false) => this.CaptureBuffer.Export(filename, sliceToBounds);

        [API("Indicate whether or not DataLogging is in effect.")]
        public bool DataLogging_IsEnabled => this.DataLogger.LoggingEnabled;

        [API("Turn DataLogging on or off.")]
        public void DataLogging_Enabled(bool enabled = true)
        {
            this.DataLogger.LoggingEnabled = enabled;
            try
            {
                if (!this.IsRunning)
                    return;
                if (enabled)
                    this.DataLogger_Startup();
                else
                    this.DataLogging_Shutdown();
            }
            catch (System.Exception ex)
            {
                this.ReportError("Error starting or stopping Logging: Filename: {0}\nException: {1}", (object)this.DataLogger.Destination, (object)ex.Message);
                this.DataLogger.LoggingEnabled = false;
            }
        }

        protected void DataLogger_Startup() => this.DataLogger.Startup(this.CaptureBuffer, this.Meter);

        protected void DataLogging_Shutdown() => this.DataLogger.Shutdown();

        [API("Re-create Capture buffer when size, sensor type, or operating mode changes.")]
        public void InitializeBuffers()
        {
            this.Meter.SetCaptureBuffer((CaptureBuffer)new CaptureBufferCircular(new Header(this.Meter, Units.SelectedUnits), this.Meter.Capacity));
            this.PreviewBufferController.UpdateCaptureBuffer(this.Meter);
        }

        public void UpdateBufferUnits() => this.CaptureBuffer.Header = new Header(this.Meter, Units.SelectedUnits);

        public bool CaptureBuffer_IsObsolete() => !(this.CaptureBuffer is CaptureBufferCircular captureBuffer) || (int)captureBuffer.Capacity != (int)this.Meter.Capacity || this.CaptureBuffer.Sensor_IsQuad != this.Meter.Sensor_IsQuad || this.CaptureBuffer.Sensor_IsPyro != this.Meter.Sensor_IsPyro;

        public void PrepareToStart()
        {
            if (this.CaptureBuffer_IsObsolete())
                this.InitializeBuffers();
            MagnitudeConverter.Units = Units.SelectedUnits;
        }

        [API("Start Data Acquisition.")]
        public void Start()
        {
            if (this.IsRunning)
                return;
            this.CancelAutoRestart = false;
            this.FirstSnapshotStart = true;
            try
            {
                this.PrepareToStart();
                this.DataLogger_Startup();
                this.Suspend_Polling();
                this.Meter.SetAcquisitionMode();
                this.PreviewBufferController.Start();
                this.OnMeterStarted();
                this.Restart();
            }
            catch (System.Exception ex)
            {
                this.ErrorLogger.ReportError("Error Starting PhoenixMeter: {0}", (object)ex.Message);
                this.Stop();
            }
        }

        protected void Restart()
        {
            int num = this.Meter.SnapshotMode_IsSelected ? 1 : 0;
            this.Meter.Start();
        }

        [API("Stop Data Acquisition.")]
        public void Stop()
        {
            if (this.Meter.SnapshotMode_IsSelected && !this.CancelAutoRestart)
            {
                this.CancelAutoRestart = true;
                this.OnDAQ_StateChanged_Stopping();
                if (!this.Meter.IsWaiting)
                    return;
            }
            this.Meter.Stop();
        }

        protected void ActuallyStopped()
        {
            this.DataLogging_Shutdown();
            this.PreviewBufferController.Stop();
            try
            {
                if (this.Meter.IsOpen)
                    this.Meter.SetPollingMode();
            }
            catch (System.Exception ex)
            {
                this.Trace("ActuallyStopped ignoring exception from SetPollingMode: " + ex.Message);
            }
            try
            {
                if (this.PollingThread != null)
                    this.Resume_Polling();
            }
            catch (System.Exception ex)
            {
                this.Trace("ActuallyStopped ignoring exception from Resume_Polling: " + ex.Message);
            }
            this.OnMeterStopped();
        }

        private void Meter_DAQ_StateChanged(DAQ_Thread sender, DAQ_StateEventArgs e)
        {
            switch (e.DAQ_State)
            {
                case DAQ_State.Start:
                    if (this.Meter.SnapshotContinuousMode_IsSelected)
                    {
                        if (!this.FirstSnapshotStart)
                            return;
                        this.FirstSnapshotStart = false;
                        break;
                    }
                    break;
                case DAQ_State.Stop:
                    if (this.Meter.SnapshotContinuousMode_IsSelected && !this.CancelAutoRestart && !this.Meter.DAQ_Thread_WasTerminated)
                    {
                        this.Restart();
                        this.OnDAQ_StateChanged_Restarted();
                        return;
                    }
                    this.ActuallyStopped();
                    break;
            }
            this.OnDAQ_StateChanged(sender, e);
        }

        private void OnDAQ_StateChanged(DAQ_Thread sender, DAQ_StateEventArgs e)
        {
            Library.DAQ_StateChanged daqStateChanged = this.DAQ_StateChanged;
            if (daqStateChanged == null)
                return;
            daqStateChanged(sender, e);
        }

        private void OnDAQ_StateChanged_Stopping() => this.OnDAQ_StateChanged(this.Meter.DataAcquisitionThread, this.StateEventStopping);

        private void OnDAQ_StateChanged_Restarted() => this.OnDAQ_StateChanged(this.Meter.DataAcquisitionThread, this.DAQ_StateEventRestart);

        protected void OnMeterStatusChanged(MeterStatus status)
        {
            this.MeterStatus = status;
            if (this.MeterStatusChanged == null)
                return;
            this.MeterStatusChanged((object)this, status);
        }

        protected void OnPollingThread_SensorFlagsChanged(
          SystemStatusBits Previous,
          SystemStatusBits Current)
        {
            if ((Current & SystemStatusBits.BusyIdentifying) != (SystemStatusBits)0)
            {
                if ((Previous & SystemStatusBits.BusyIdentifying) != (SystemStatusBits)0)
                    return;
                this.OnMeterStatusChanged(MeterStatus.SensorIdentifying);
            }
            else
            {
                if ((Previous & SystemStatusBits.SensorIsAttached) == (SystemStatusBits)0 && (Current & SystemStatusBits.SensorIsAttached) != (SystemStatusBits)0)
                    this.OnSensorConnected();
                if ((Previous & SystemStatusBits.SensorIsAttached) == (SystemStatusBits)0 || (Current & SystemStatusBits.SensorIsAttached) != (SystemStatusBits)0)
                    return;
                this.OnSensorDisconnected();
            }
        }

        protected void OnMeterConnected() => this.OnMeterStatusChanged(MeterStatus.MeterConnected);

        protected void OnMeterDisconnected() => this.OnMeterStatusChanged(MeterStatus.MeterDisconnected);

        protected void OnSensorConnected()
        {
            this.Meter.ReloadSensorProperties();
            this.InitializeBuffers();
            this.OnMeterStatusChanged(MeterStatus.SensorConnected);
        }

        protected void OnSensorDisconnected()
        {
            this.Meter.SensorDisconnected();
            this.OnMeterStatusChanged(MeterStatus.SensorDisconnected);
        }

        protected void OnSystemFaultDetected(SystemFaultBits obj) => this.OnMeterStatusChanged(MeterStatus.SystemFault);

        protected void OnMeterStarted() => this.OnMeterStatusChanged(MeterStatus.MeterStarted);

        protected void OnMeterStopped() => this.OnMeterStatusChanged(MeterStatus.MeterStopped);

        private void PollingThread_MeasurementUpdate(PollingData pollingData)
        {
            if (this.MeasurementUpdate == null)
                return;
            this.MeasurementUpdate(pollingData);
        }

        [API(APICategory.Unclassified)]
        public bool ZeroMeter()
        {
            bool snapshotModeEnabled = this.Meter.SnapshotModeEnabled;
            bool channelIsSelected = this.Meter.HighSpeedChannel_IsSelected;
            bool suspended = this.PollingThread.Suspended;
            this.Meter.SnapshotModeEnabled = false;
            if (!suspended)
                this.Suspend_Polling();
            try
            {
                this.Meter.ZeroMeter();
                if (!this.Meter.SystemFault_IsBadZero)
                {
                    if (this.Meter.IsDualSpeed)
                    {
                        if (this.Meter.Device.FirmwareVersion_HasWorkingZero)
                        {
                            this.Meter.HighSpeedChannel_IsSelected = !channelIsSelected;
                            this.Meter.ZeroMeter();
                        }
                    }
                }
            }
            finally
            {
                this.Meter.HighSpeedChannel_IsSelected = channelIsSelected;
                this.Meter.SnapshotModeEnabled = snapshotModeEnabled;
                if (!suspended)
                    this.Resume_Polling();
            }
            return this.Meter.SystemFault_IsBadZero;
        }

        [API(APICategory.Unclassified)]
        public bool WaitZeroMeter()
        {
            this.Meter.WaitZeroMeter();
            return this.Meter.SystemFault_IsBadZero;
        }

        public void Trace(string format, params object[] args)
        {
        }

        [Conditional("TRACE_OPEN")]
        public void TraceOpen(string format, params object[] args)
        {
        }

        [Conditional("TRACE_INIT_BUFFERS")]
        public void TraceInitBuffers(string format, params object[] args)
        {
        }

        [Conditional("TRACE_ACQUISITION_EVENTS")]
        public void TraceAcquisitionEvents(string format, params object[] args)
        {
        }

        [Conditional("TRACE_LOGGING")]
        public void TraceLogging(string format, params object[] args)
        {
        }

        [Conditional("TRACE_ZEROING")]
        public void TraceZeroing(string format, params object[] args)
        {
        }

        [Conditional("TRACE_GC")]
        public void TraceAndGC() => GC.Collect();

        [Conditional("TRACE_GC")]
        public void TraceGC(string format, params object[] args) => this.Trace("GC {0}:{1}, {2}, {3}, {4}", (object)string.Format(format, args), (object)GC.CollectionCount(0), (object)GC.CollectionCount(1), (object)GC.CollectionCount(2), (object)GC.GetTotalMemory(false));
    }
}
