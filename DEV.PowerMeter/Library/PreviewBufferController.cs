using SharedLibrary;
using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Windows.Threading;

namespace DEV.PowerMeter.Library
{
    [API(APICategory.Utility)]
    [DataContract]
    public class PreviewBufferController
    {
        [API(APICategory.Unclassified)]
        public const double DefaultPeriod = 0.5;
        [API(APICategory.Unclassified)]
        public const double MinimumPeriod = 0.2;
        [API(APICategory.Unclassified)]
        public const double MaximumPeriod = 5.0;
        [API(APICategory.Unclassified)]
        public const uint DefaultCapacity = 1500;
        [API(APICategory.Unclassified)]
        public const uint MaximumCapacity = 3000;
        protected double period;
        protected uint capacity;
        private object PreviewBufferLock = new object();
        protected Meter Meter;
        protected Stopwatch Stopwatch = new Stopwatch();
        protected DispatcherTimer PreviewTimer;
        public const int SuspendCount = 3;

        [API(APICategory.Unclassified)]
        [DataMember]
        public double Period
        {
            get => this.period;
            set
            {
                this.period = value;
                this.UpdatePeriod();
            }
        }

        [API(APICategory.Unclassified)]
        [DataMember]
        public uint Capacity
        {
            get => this.capacity;
            set
            {
                this.capacity = value;
                this.UpdateCapacity();
            }
        }

        [API(APICategory.Unclassified)]
        public CaptureBufferCircular PreviewBuffer { get; protected set; }

        protected CaptureBufferCircular CaptureBuffer => this.Meter == null ? (CaptureBufferCircular)null : this.Meter.CaptureBuffer as CaptureBufferCircular;

        [API(APICategory.Unclassified)]
        public bool WaitForTriggerMode_IsActive
        {
            get
            {
                if (this.Meter == null)
                    return false;
                if (this.Meter.SnapshotMode_IsSelected)
                    return true;
                return this.Meter.TriggerWaitMode_IsSelected && this.Meter.IsWaiting;
            }
        }

        [API(APICategory.Unclassified)]
        public bool LPEM_Mode_IsSelected => this.Meter != null && this.Meter.LPEM_Mode_IsSelected;

        [API(APICategory.Unclassified)]
        public double Progress => this.Meter == null ? 0.0 : this.Meter.Progress;

        [API(APICategory.Unclassified)]
        public DAQ_State DAQ_State => this.Meter == null ? DAQ_State.Progress : this.Meter.DAQ_State;

        [API(APICategory.Unclassified)]
        public int EnergyRecordsProcessed { get; set; }

        [API(APICategory.Unclassified)]
        public event Action<PreviewBufferController> PreviewUpdate;

        public PreviewBufferController()
        {
            this.PreviewTimer = new DispatcherTimer();
            this.period = 0.5;
            this.capacity = 1500U;
            this.UpdatePeriod();
            this.UpdateCapacity();
            this.PreviewTimer.Tick += new EventHandler(this.PreviewTimer_Tick);
        }

        public void UpdatePeriod() => this.PreviewTimer.Interval = TimeSpan.FromSeconds(this.Period);

        public void UpdateCapacity()
        {
            lock (this.PreviewBufferLock)
            {
                CaptureBufferCircular captureBufferCircular;
                if (this.CaptureBuffer == null)
                {
                    captureBufferCircular = (CaptureBufferCircular)null;
                }
                else
                {
                    captureBufferCircular = new CaptureBufferCircular(this.CaptureBuffer.Header, this.Capacity);
                    captureBufferCircular.ParentBuffer = (Library.CaptureBuffer)this.CaptureBuffer;
                }
                this.PreviewBuffer = captureBufferCircular;
            }
        }

        public void UpdateCaptureBuffer(Meter meter)
        {
            this.Meter = meter;
            this.UpdateCapacity();
        }

        public void Start()
        {
            this.EnergyRecordsProcessed = 0;
            this.UpdatePeriod();
            this.PreviewTimer.Start();
        }

        public void Stop() => this.PreviewTimer.Stop();

        public int Suspended { get; protected set; }

        [API(APICategory.Unclassified)]
        public void Suspend() => this.Suspended = 3;

        private void PreviewTimer_Tick(object sender, EventArgs e)
        {
            if (this.Suspended > 0)
            {
                --this.Suspended;
            }
            else
            {
                this.Stopwatch.Restart();
                this.OnPreviewUpdate();
                this.Stopwatch.Stop();
                int num = this.PreviewTimer.Interval < this.Stopwatch.Elapsed ? 1 : 0;
            }
        }

        protected void OnPreviewUpdate()
        {
            if (!this.WaitForTriggerMode_IsActive)
            {
                if (this.PreviewBuffer == null || this.CaptureBuffer == null)
                    return;
                lock (this.PreviewBufferLock)
                    this.PreviewBuffer.Update(this.CaptureBuffer);
                if (this.PreviewBuffer.Count <= 0U)
                    return;
            }
            if (this.PreviewUpdate == null)
                return;
            this.PreviewUpdate(this);
        }
    }
}
