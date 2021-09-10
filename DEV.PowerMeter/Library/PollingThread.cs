using DEV.PowerMeter.Library.ViewModels;
using SharedLibrary;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace DEV.PowerMeter.Library
{
    [API(APICategory.Undocumented, "This PollingThread class contains a thread that starts and runs continuously when the PhoenixMeter object is Opened, and continues until it is Shutdown. PollingThread periodically queries the hardware for status. This polling detects errors and hot swap events (sensor or meter being connected or disconnected). This is translated to various properties and notifications presented to the rest of the system. \nIn normal usage, all of these details are actually handled by the PhoenixMeter object. There is very little need for direct access to the PollingThread. This PollingThread class is a singleton object (for each PhoenixMeter instance). ")]
    public class PollingThread
    {
        protected Thread Thread;
        protected PollingData PollingData;
        private object PollingLock = new object();
        public const int PollingShortInterval = 200;
        public const int PollingLongInterval = 1500;
        public const int PollingCycleInterval = 50;
        public const int JoinTimeoutInterval = 3000;
        private Meter _Meter;
        protected ActionList TriggeredEvents = new ActionList();

        public int PollingInterval => !this.MeterIsPresent || !this.SensorIsValid ? 200 : 1500;

        [API("The PollingThread has started (and possibly completed) \r\nthe process of shutting down. \r\nSet by the Terminate method.")]
        public bool Terminated { get; protected set; }

        [API("The PollingThread is Suspended, \r\nwhich temporarily prevents all polling activity, \r\nsay when meter is Running or Zeroing.\r\nControlled by the Suspend and Resume methods.")]
        public bool Suspended { get; protected set; }

        [API("Enable MeasurementUpdate events when Meter is not running and PollingThread is not suspended.")]
        public bool Update_Enabled { get; set; }

        [API("Master switch to control all polling activity. Updates don't happen if !Polling_Enabled.")]
        public bool Polling_Enabled { get; set; }

        protected bool MeterIsPresent { get; set; }

        protected bool MeterWasPresent { get; set; }

        protected SystemStatusBits SensorFlags { get; set; }

        protected SystemStatusBits PreviousSensorFlags { get; set; }

        protected SystemFaultBits PreviousSystemFaults { get; set; }

        public void ClearFlags()
        {
            this.MeterIsPresent = this.MeterWasPresent = false;
            this.PreviousSystemFaults = (SystemFaultBits)0;
            this.PreviousSensorFlags = this.SensorFlags = (SystemStatusBits)0;
        }

        [API("Local test if SensorIsAttached flag was set during the most recent polling event.")]
        protected bool SensorIsValid => (this.SensorFlags & SystemStatusBits.SensorIsAttached) > (SystemStatusBits)0;

        protected Meter Meter
        {
            get => this._Meter;
            set => this._Meter = value;
        }

        public void AttachMeter(Meter meter)
        {
            using (new SafeLock(nameof(AttachMeter), this.PollingLock))
                this.Meter = meter;
        }

        protected bool Meter_IsOpen
        {
            get
            {
                using (new SafeLock(nameof(Meter_IsOpen), this.PollingLock))
                    return this.Meter != null && this.Meter.IsOpen;
            }
        }

        public void CloseMeter()
        {
            using (new SafeLock(nameof(CloseMeter), this.PollingLock))
            {
                this.AttachMeter((Meter)null);
                this.ClearFlags();
            }
        }

        [API("Event fires when meter is disconnected.")]
        public event Action MeterDisconnected;

        [API("Event fires when Sensor Status flags change.")]
        public event Action<SystemStatusBits, SystemStatusBits> SensorFlagsChanged;

        [API("Event fires periodically to trigger a Measurement Update.")]
        public event Action<PollingData> MeasurementUpdate;

        [API("Event fires when a SystemFault is Detected.")]
        public event Action<SystemFaultBits> SystemFaultDetected;

        protected void FetchPollingData()
        {
            if (!this.Meter_IsOpen)
                return;
            try
            {
                this.Meter.UpdateSystemStatusAndFaults(this.PollingData);
            }
            catch (Exception ex)
            {
                this.ReportError("FetchPollingData exception: {0}", (object)ex.Message);
            }
        }

        protected void OnMeterDisconnected()
        {
            this.TraceEvents(nameof(OnMeterDisconnected));
            try
            {
                if (this.MeterDisconnected != null)
                    this.MeterDisconnected();
                this.ClearFlags();
            }
            catch (Exception ex)
            {
                this.ReportError("MeterDisconnected exception: {0}", (object)ex.Message);
            }
            this.TraceEvents("OnMeterDisconnected exits");
        }

        protected void OnSensorFlagsChange(
          SystemStatusBits SensorFlagsPrevious,
          SystemStatusBits SensorFlags)
        {
            this.TraceEvents("SensorFlagsChange {0:x} -> {1:x}", (object)SensorFlagsPrevious, (object)SensorFlags);
            try
            {
                if (this.SensorFlagsChanged != null)
                    this.SensorFlagsChanged(SensorFlagsPrevious, SensorFlags);
            }
            catch (Exception ex)
            {
                this.ReportError("SensorFlagsChange exception: {0}", (object)ex.Message);
            }
            this.TraceEvents("SensorFlagsChange exits");
        }

        protected void OnMeasurementUpdate(PollingData pollingData)
        {
            try
            {
                if (this.MeasurementUpdate == null)
                    return;
                this.MeasurementUpdate(pollingData);
            }
            catch (Exception ex)
            {
            }
        }

        protected void OnSystemFaultDetected(SystemFaultBits faults)
        {
            this.TraceEvents(nameof(OnSystemFaultDetected));
            try
            {
                if (this.SystemFaultDetected != null)
                    this.SystemFaultDetected(faults);
            }
            catch (Exception ex)
            {
                this.ReportError("SystemFaultDetected exception: {0}", (object)ex.Message);
            }
            this.TraceEvents("OnSystemFaultDetected exits");
        }

        public PollingThread()
        {
            this.PollingData = new PollingData();
            this.Terminated = false;
            this.Suspended = false;
            this.Thread = new Thread((ThreadStart)(() => this.PollingThreadBody()));
            this.Thread.Name = "Polling";
            this.Thread.IsBackground = true;
            this.Thread.Start();
        }

        protected void PollingThreadBody()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            while (!this.Terminated)
            {
                this.TriggeredEvents.Clear();
                try
                {
                    using (new SafeLock("Polling.FallingEdge", this.PollingLock))
                    {
                        if (this.MeterWasPresent)
                        {
                            if (!this.Meter_IsOpen)
                            {
                                this.TraceEvents(string.Format("Falling Edge {0} && {1}", (object)this.Meter.IsDeviceOpen, (object)this.Meter.IsInitialized));
                                this.MeterIsPresent = this.MeterWasPresent = false;
                                this.TriggeredEvents.Add((Action)(() => this.OnMeterDisconnected()));
                            }
                        }
                    }
                    if (!this.Suspended && this.Polling_Enabled && this.Meter_IsOpen && !this.Terminated && stopwatch.ElapsedMilliseconds > (long)this.PollingInterval)
                    {
                        stopwatch.Restart();
                        this.CollectData();
                    }
                    this.TriggeredEvents.InvokeAll();
                    Thread.Sleep(50);
                }
                catch (ThreadAbortException ex)
                {
                    break;
                }
                catch (IOException ex)
                {
                    if (this.Meter != null)
                        this.Meter.Close();
                    this.OnMeterDisconnected();
                }
                catch (Exception ex)
                {
                }
            }
            this.TraceExit("Body Exits");
        }

        protected void CollectData()
        {
            using (new SafeLock("Polling.Main", this.PollingLock))
            {
                this.FetchPollingData();
                this.MeterIsPresent = this.PollingData.HasMeter;
                if (this.MeterWasPresent && !this.MeterIsPresent)
                    this.TriggeredEvents.Add((Action)(() => this.OnMeterDisconnected()));
                if (!this.MeterWasPresent && this.MeterIsPresent)
                    this.PreviousSensorFlags = this.PollingData.SystemStatus_SensorFlags;
                this.MeterWasPresent = this.MeterIsPresent;
                if (!this.MeterIsPresent)
                    return;
                SystemFaultBits faults = this.PollingData.SystemFaults & ~SystemFaultBits.IgnoreDuringPolling;
                if (faults != (SystemFaultBits)0 && this.PreviousSystemFaults != faults || ((faults ^ this.PreviousSystemFaults) & SystemFaultBits.SensorOvertemp) != (SystemFaultBits)0)
                    this.TriggeredEvents.Add((Action)(() => this.OnSystemFaultDetected(faults)));
                this.PreviousSystemFaults = faults;
                this.SensorFlags = this.PollingData.SystemStatus_SensorFlags;
                if (this.PreviousSensorFlags != this.SensorFlags)
                {
                    SystemStatusBits previous = this.PreviousSensorFlags;
                    SystemStatusBits current = this.SensorFlags;
                    this.TriggeredEvents.Add((Action)(() => this.OnSensorFlagsChange(previous, current)));
                }
                this.PreviousSensorFlags = this.SensorFlags;
                if (!this.SensorIsValid || !this.Update_Enabled || this.Terminated)
                    return;
                this.TriggeredEvents.Add((Action)(() => this.OnMeasurementUpdate(this.PollingData)));
            }
        }

        [API("Temporarily suspend polling.")]
        public void Suspend()
        {
            this.TraceSuspend("Suspending...");
            using (new SafeLock(nameof(Suspend), this.PollingLock))
                this.Suspended = true;
            this.TraceSuspend("Suspended");
        }

        [API("Resume temporarily suspended polling.")]
        public void Resume()
        {
            this.TraceSuspend("Resuming...");
            using (new SafeLock(nameof(Resume), this.PollingLock))
                this.Suspended = false;
            this.TraceSuspend("Resumed");
        }

        [API("Terminate this PollingThread.")]
        public void Terminate()
        {
            this.Terminated = true;
            this.TraceExit("TerminateWait...");
            if (!this.Thread.Join(3000))
                this.TraceExit("Join failed");
            this.TraceExit("Exited");
        }

        public IErrorReporter ErrorReporter => ServiceLocator.Resolve<IErrorReporter>();

        public void ReportError(string format, params object[] args)
        {
            if (this.ErrorReporter == null)
                return;
            this.ErrorReporter.ReportError(format, args);
        }

        [Conditional("TRACE_POLLING_THREAD")]
        public void Trace(string format, params object[] args)
        {
        }

        [Conditional("TRACE_EVENTS")]
        public void TraceEvents(string format, params object[] args)
        {
        }

        [Conditional("TRACE_EXITS")]
        public void TraceExit(string format, params object[] args)
        {
        }

        [Conditional("TRACE_SUSPENDRESUME")]
        public void TraceSuspend(string format, params object[] args)
        {
        }

        [Conditional("TRACE_FETCHPOLLINGDATA")]
        public void TraceFetchPollingData(string format, params object[] args)
        {
        }
    }
}
