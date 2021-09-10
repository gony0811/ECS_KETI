using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SharedLibrary;

namespace DEV.PowerMeter.Library
{

    public delegate void DAQ_StateChanged(DAQ_Thread sender, DAQ_StateEventArgs e);

    public class DAQ_StateEventArgs
    {
        public DAQ_State DAQ_State;
        public double Progress;

        public DAQ_StateEventArgs()
          : this(DAQ_State.Progress)
        {
        }

        public DAQ_StateEventArgs(DAQ_State state) => this.DAQ_State = state;

        public override string ToString() => "DAQ_State." + this.DAQ_State.ToString();
    }

    public abstract class DAQ_Thread
    {
        protected Thread Thread;
        protected DAQ_StateChanged DAQ_StateChanged;
        private object Lock = new object();
        private ManualResetEventSlim ThreadBusy = new ManualResetEventSlim(true);
        private bool isStopping;
        private bool isWaiting;
        protected byte[] Data;
        protected DataRecordSingle Record;
        protected static int NextNumber = 1;
        public const int Infinite = -1;
        public Stopwatch SampleTime;
        public Stopwatch StopWatch;

        public IDaqMeter Meter { get; set; }

        protected IDaqDevice Device { get; set; }

        protected Channel Channel => this.Device.Channel;

        public CaptureBuffer CaptureBuffer { get; set; }

        public event Action<DAQ_Thread> OnExit;

        protected uint Capacity { get; set; }

        public uint PreTrigger { get; set; }

        protected uint StartingCount { get; set; }

        protected bool StopOnCount { get; set; }

        protected bool SnapshotMode_IsSelected { get; set; }

        protected bool OperatingMode_IsTrueEnergy { get; set; }

        protected virtual int RecordSize => !this.QuadMode_IsSelected ? DataRecordSingle.BinaryRecordLength() : DataRecordQuad.BinaryRecordLength();

        public bool QuadMode_IsSelected { get; set; }

        public int PowerModeMaxElapsed_ms { get; set; } = 1500;

        public bool IsRunning
        {
            get => !this.ThreadBusy.IsSet;
            set
            {
                if (value)
                    this.ThreadBusy.Reset();
                else
                    this.ThreadBusy.Set();
            }
        }

        public bool IsStopping
        {
            get
            {
                lock (this.Lock)
                    return this.isStopping;
            }
            protected set
            {
                lock (this.Lock)
                {
                    int num1 = this.isStopping ? 1 : 0;
                    int num2 = value ? 1 : 0;
                    this.isStopping = value;
                }
            }
        }

        public bool IsWaiting
        {
            get
            {
                lock (this.Lock)
                    return this.isWaiting;
            }
            set
            {
                lock (this.Lock)
                    this.isWaiting = value;
            }
        }

        public System.Threading.ThreadState ThreadState => this.Thread.ThreadState;

        public int Count { get; protected set; }

        protected bool ForceTriggerRequested { get; set; }

        public void RequestForceTrigger() => this.ForceTriggerRequested = true;

        protected void ForceTrigger()
        {
            this.ForceTriggerRequested = false;
            this.Device.ForceTrigger();
        }

        [Conditional("BOOST_THREAD_PRIORITY")]
        protected void BoostThreadPriority() => Thread.CurrentThread.Priority = ThreadPriority.AboveNormal;

        public int Number { get; protected set; }

        public DAQ_Thread(
          DAQ_StateChanged daq_StateChanged,
          IDaqMeter meter,
          IDaqDevice device,
          CaptureBuffer captureBuffer)
        {
            this.Number = DAQ_Thread.NextNumber++;
            this.Meter = meter;
            this.Device = device;
            this.CaptureBuffer = captureBuffer;
            this.DAQ_StateChanged = daq_StateChanged;
            this.SnapshotMode_IsSelected = this.Meter.SnapshotMode_IsSelected;
            this.PreTrigger = this.Meter.PreTrigger;
            this.OperatingMode_IsTrueEnergy = this.Meter.OperatingMode_IsTrueEnergy;
            this.QuadMode_IsSelected = this.Meter.QuadMode_IsSelected;
            if (this.SnapshotMode_IsSelected)
            {
                this.Capacity = Math.Min(this.Meter.Capacity, this.Meter.SnapshotMaxCapacity);
                this.StartingCount = this.Capacity;
                this.StopOnCount = true;
            }
            else
            {
                this.Capacity = this.Meter.Capacity;
                this.StartingCount = 0U;
                this.StopOnCount = !this.Meter.ContinuousMode;
            }
            this.TraceRecordSize("Fields={0}; QuadMode={1}, RecordSize={2}", (object)this.Meter.SelectedDataFields, (object)this.QuadMode_IsSelected, (object)this.RecordSize);
            this.TraceRecordSize("DRB={0}; Meter={1}, Units={2}", (object)DataRecordBase.SelectedDataFields, (object)this.Meter.OperatingMode, (object)Units.SelectedUnits.Abbreviation);
            this.Data = new byte[this.RecordSize];
            this.Record = this.QuadMode_IsSelected ? (DataRecordSingle)new DataRecordQuad() : new DataRecordSingle();
            this.StopWatch = new Stopwatch();
            this.SampleTime = new Stopwatch();
        }

        protected void OnDAQ_StateChanged(DAQ_State type) => this.OnDAQ_StateChanged(new DAQ_StateEventArgs(type));

        protected void OnDAQ_StateChanged(DAQ_StateEventArgs args)
        {
            this.DAQ_State = args.DAQ_State;
            DAQ_StateChanged daqStateChanged = this.DAQ_StateChanged;
            if (daqStateChanged == null)
                return;
            daqStateChanged(this, args);
        }

        public DAQ_State DAQ_State { get; protected set; }

        protected abstract void ThreadBody();

        public void Start()
        {
            this.Thread = new Thread((ThreadStart)(() => this.ThreadBody()));
            this.Thread.Name = string.Format("DAQ.{0}", (object)this.Number);
            this.Thread.IsBackground = true;
            this.IsRunning = true;
            this.IsStopping = false;
            this.IsWaiting = true;
            this.BytesRead = this.RecordsRead = this.Count = 0;
            this.StopWatch.Start();
            this.Device.Start(this.StartingCount);
            this.Thread.Start();
        }

        public void Stop()
        {
            this.IsStopping = true;
            if ((this.ThreadState & System.Threading.ThreadState.Unstarted) == System.Threading.ThreadState.Running)
                return;
            this.OnThreadExits();
        }

        protected void OnThreadExitsBody()
        {
            this.StopWatch.Stop();
            try
            {
                this.Device.Stop();
            }
            catch (Exception ex1)
            {
                this.Trace("Device.Stop exception: " + ex1.Message);
                try
                {
                    this.Device.Close();
                }
                catch (Exception ex2)
                {
                    this.Trace("Device.Stop.Close exception: " + ex2.Message);
                }
            }
            this.CaptureBuffer.SkipBuffer = Communicator.CurrentSkipBuffer;
            try
            {
                Action<DAQ_Thread> onExit1 = this.OnExit;
                Action<DAQ_Thread> onExit2 = this.OnExit;
                if (onExit2 != null)
                    onExit2(this);
            }
            catch (Exception ex)
            {
            }
            this.IsRunning = false;
            this.OnDAQ_StateChanged(DAQ_State.Stop);
        }

        protected void OnThreadExits()
        {
            try
            {
                this.OnThreadExitsBody();
            }
            catch (Exception ex)
            {
                this.ReportException(ex);
            }
        }

        public bool WaitThreadExits(int timeout = 500)
        {
            int num = this.ThreadBusy.Wait(timeout) ? 1 : 0;
            if (num != 0)
                return num != 0;
            this.Trace("WaitThreadExits: ThreadBusy.Wait timed out");
            return num != 0;
        }

        public int BytesRead { get; protected set; }

        public int RecordsRead { get; protected set; }

        public void ReportRecordRead(int bytes)
        {
            this.BytesRead += bytes;
            ++this.RecordsRead;
        }

        public TimeSpan TotalReadTime => this.SampleTime.Elapsed;

        public double MeanSampleRate => this.Count <= 0 ? 0.0 : this.SampleTime.Elapsed.TotalSeconds / (double)this.Count;

        public TimeSpan Elapsed => this.StopWatch.Elapsed;

        public double Elapsed_ms
        {
            get
            {
                Stopwatch stopWatch = this.StopWatch;
                return stopWatch == null ? 0.0 : stopWatch.Elapsed.TotalMilliseconds;
            }
        }

        public double SecondsEach => this.RecordsRead <= 0 ? 0.0 : this.Elapsed.TotalSeconds / (double)this.RecordsRead;

        public double SamplesPerSec => this.Elapsed.TotalSeconds <= 0.0 ? 0.0 : (double)this.RecordsRead / this.Elapsed.TotalSeconds;

        public double BytesPerSec => this.Elapsed.TotalSeconds <= 0.0 ? 0.0 : (double)this.BytesRead / this.Elapsed.TotalSeconds;

        public double Progress => this.Capacity <= 0U ? 0.0 : 100.0 * (double)this.Count / (double)this.Capacity;

        public bool WasTerminated { get; protected set; }

        public DataRecordSingle TerminationRecord { get; protected set; }

        public MeasurementFlags TerminationFlags => this.TerminationRecord == null ? (MeasurementFlags)0 : this.TerminationRecord.Flags;

        protected bool TerminatedByMeter(DataRecordSingle record)
        {
            this.WasTerminated = (uint)(record.Flags & MeasurementFlags.Terminated) > 0U;
            if (this.WasTerminated)
            {
                this.Trace("Termination Record Received: {0}", (object)record.ToString_AsHex());
                this.TerminationRecord = record;
            }
            return this.WasTerminated;
        }

        public string SummaryLine1 => string.Format("{0:f3} sec, {1} bytes, {2} records, {3} bytes each ", (object)this.Elapsed.TotalSeconds, (object)this.BytesRead, (object)this.RecordsRead, (object)this.RecordSize);

        public string SummaryLine2 => string.Format("{0:f3} ms ea, {1:f3} kHz, {2:f3} kbps", (object)(this.SecondsEach * 1000.0), (object)(this.SamplesPerSec / 1000.0), (object)(this.BytesPerSec / 1000.0));

        public static IErrorReporter ErrorReporter;// => ServiceLocator.Resolve<IErrorReporter>();

        public void ReportError(string message)
        {
            this.Trace(message);
            if (DAQ_Thread.ErrorReporter == null)
                return;
            DAQ_Thread.ErrorReporter.ReportError("{0}: {1}", (object)this.InstanceTypeName, (object)message);
        }

        protected void ReportUnexpectedEOF() => this.ReportError("Unexpected EOF reading data");

        protected void ReportUnexpectedTimeout() => this.ReportError("Unexpected timeout");

        protected void ReportException(Exception ex) => this.ReportError("Unexpected Exception: " + ex.Message);

        [Conditional("TRACE")]
        public void Trace(string message)
        {
        }

        public string InstanceTypeName => this.GetType().Name;

        [Conditional("TRACE")]
        public void Trace(string fmt, params object[] args) => this.Trace(string.Format(fmt, args));

        [Conditional("TRACE_START_STOP")]
        public void TraceStartStop(string fmt, params object[] args) => this.Trace(fmt, args);

        [Conditional("TRACE_RECORD_SIZE")]
        public void TraceRecordSize(string fmt, params object[] args) => this.Trace(fmt, args);

        [Conditional("TRACE_DATA")]
        public void TraceData(string fmt, params object[] args) => this.Trace(fmt, args);

        [Conditional("TRACE_DATA_COUNT")]
        public void TraceDataCount(string fmt, params object[] args) => this.Trace(fmt, args);

        [Conditional("TRACE_READ_DATA")]
        public void TraceReadData(byte[] data, int index)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int index1 = 0; index1 < index; ++index1)
                stringBuilder.AppendFormat(" {0:x2}", (object)data[index1]);
        }

        [Conditional("TRACE_READ_DATA")]
        public void TraceReadData(string data)
        {
        }

        [Conditional("TRACE_SHOW_SUMMARY")]
        public void ShowSummary()
        {
            this.Trace("Summary: {0}", (object)this.SummaryLine1);
            this.Trace("Summary: {0}", (object)this.SummaryLine2);
        }
    }
}
