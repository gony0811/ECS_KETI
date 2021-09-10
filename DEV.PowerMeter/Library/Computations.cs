using SharedLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DEV.PowerMeter.Library
{
    [API(APICategory.Analysis, "The Computations class accumulates various statistics about a range of data samples, \r\n\t\tmost typically and notably the contents of a selected subset of the current CaptureBuffer.\r\n\t\tThe Computations class includes accurate Pulse Detection, using the PulseAnalyzer class.")]
    public class Computations : IComputations
    {
        [API("Literal Zero = 0")]
        public const int Zero = 0;
        protected Units _Units;

        [API("CaptureBuffer for these Statistics.")]
        public CaptureBuffer CaptureBuffer { get; protected set; }

        [API("BufferBounds for these Statistics. \r\n\t\t\tAnalysis is confined within these bounds, and they appear in the Statistics panel.")]
        public BufferBounds BufferBounds { get; protected set; }

        [API("Number of samples between cursors")]
        public int Count { get; protected set; }

        [API("Units in effect for these Statistics")]
        public Units Units
        {
            get => this._Units ?? Units.Watts;
            set => this._Units = value;
        }

        [API("Overall width between left/right cursors (as a TimeSpan)")]
        public TimeSpan Width => this.BufferBounds.Width;

        [API("Overall width between left/right cursors (as Seconds)")]
        public double Width_Seconds => this.BufferBounds.Width.TotalSeconds;

        [API("Overall height between upper/lower cursors")]
        public double Height => this.BufferBounds.Height;

        [API("Minimum value of samples between left/right cursors")]
        public double Minimum { get; protected set; }

        [API("Maximum value of samples between left/right cursors")]
        public double Maximum { get; protected set; }

        [API("Number of samples with values greater than Top cursor")]
        public int Above { get; protected set; }

        [API("Number of samples with values less than Bottom cursor")]
        public int Below { get; protected set; }

        [API("Number of samples with TriggerDetected flag set between left/right cursors")]
        public int TriggerCount { get; protected set; }

        [API("A approximate count of Pulses between left/right cursors. (This was an early approximation, assuming a pulse always starts with a Trigger marker. The Proper way to count Pulses is with the PulseAnalyzer.)")]
        public int PulseCount => this.TriggerCount <= 1 || this.LastSample != this.LastTrigger ? this.TriggerCount : this.TriggerCount - 1;

        [API("First DataRecord between the cursors")]
        public DataRecordSingle FirstSample { get; protected set; }

        [API("Last DataRecord between the cursors")]
        public DataRecordSingle LastSample { get; protected set; }

        [API("First DataSample in the selection with a Trigger marker")]
        public DataRecordSingle FirstTrigger { get; protected set; }

        [API("Last DataSample in the selection with a Trigger marker")]
        public DataRecordSingle LastTrigger { get; protected set; }

        [API("Measurement Value at the Left Cursor (if there is one)")]
        public double LeftValue => this.OptionalMeasurement(this.FirstSample);

        [API("Measurement Value at the Right Cursor (if there is one)")]
        public double RightValue => this.OptionalMeasurement(this.LastSample);

        public double OptionalMeasurement(DataRecordSingle sample) => sample == null ? double.NaN : sample.Measurement;

        [API("PulseAnalyzer and analysis results; singleton instance for whole system.")]
        public PulseAnalyzer PulseAnalyzer { get; protected set; }

        [API("Pulse analyzer options; singleton instance for whole system.")]
        public PulseAnalysisOptions PulseAnalysisOptions { get; protected set; }

        [API("Pulse analyzer results; singleton instance for whole system.")]
        public PulseAnalysisResults PulseAnalysisResults => this.PulseAnalyzer.Results;

        [API("Baseline power level used for energy calculations")]
        public double EnergyBaselineLevel => this.PulseAnalysisOptions.EnergyBaselineLevel;

        [API("Sum of measurements between cursors (for computing energy)")]
        public double MeasurementSum { get; protected set; }

        [API("Width of each sample between left/right cursors (as Seconds)")]
        public double SampleWidth => this.Width_Seconds / (double)this.Count;

        [API("Energy calculation for all samples between left/right cursors")]
        public double TotalEnergy => this.Count <= 0 ? 0.0 : this.MeasurementSum * this.SampleWidth;

        [API("Mean Energy for each Trigger-delenated Pulse in the Selection")]
        public double PulseEnergy => this.PulseCount <= 0 ? 0.0 : this.TotalEnergy / (double)this.PulseCount;

        [API("Mean Pulse Width for each Trigger-delenated Pulse in the Selection")]
        public double PulseWidth => this.FirstTrigger != null && this.LastTrigger != null && this.LastTrigger.Timestamp > this.FirstTrigger.Timestamp ? (this.LastTrigger.Timestamp - this.FirstTrigger.Timestamp).TotalSeconds / (double)this.PulseCount : 0.0;

        [API("Mean Pulse Period for each Trigger-delenated Pulse in the Selection")]
        public double PulsePeriod => this.PulseCount <= 0 ? 0.0 : this.Width_Seconds / (double)this.PulseCount;

        [API("1 / PulsePeriod (if PulsePeriod != 0; else 0)")]
        public double PulseRate => this.PulsePeriod == 0.0 ? 0.0 : 1.0 / this.PulsePeriod;

        [API("Create Computations, Cleared")]
        public Computations()
        {
            this.PulseAnalysisOptions = new PulseAnalysisOptions();
            this.PulseAnalyzer = new PulseAnalyzer(this.PulseAnalysisOptions);
            this.BufferBounds = new BufferBounds();
            this.Clear();
        }

        [API("Clear all Computations")]
        public void Clear()
        {
            this.Count = this.Above = this.Below = this.TriggerCount = 0;
            this.MeasurementSum = 0.0;
            this.Minimum = double.MaxValue;
            this.Maximum = double.MinValue;
            this.FirstTrigger = this.LastTrigger = this.FirstSample = this.LastSample = (DataRecordSingle)null;
        }

        [API("Attach CaptureBuffer and recompute statistics.")]
        public void AttachCaptureBuffer(CaptureBuffer captureBuffer)
        {
            this.CaptureBuffer = captureBuffer;
            if (!this.BufferBounds.IsInitialized)
                return;
            this.Reload();
        }

        [API("Clear Statistics and AddRange the current CaptureBuffer")]
        protected void Reload() => this.Reload(this.CaptureBuffer.Slice(this.BufferBounds));

        [API("Clear Statistics and AddRange a sequence of DataRecords")]
        public void Reload(IEnumerable<DataRecordSingle> data)
        {
            Stopwatch.StartNew();
            this.Clear();
            this.AddRange(data);
            if (this.Count <= 0 || !this.PulseAnalysisOptions.PulseAnalysisEnabled || this.CaptureBuffer == null || !this.BufferBounds.IsInitialized || this.Minimum == double.MaxValue || this.Maximum == double.MinValue)
                return;
            this.PulseAnalyzer.Options.SetBounds(this.BufferBounds);
            this.PulseAnalyzer.SetThresholds(this.Minimum, this.Maximum);
            this.PulseAnalyzer.Reload(data);
        }

        [API("Add a sequence of DataRecords (without Clear)")]
        protected void AddRange(IEnumerable<DataRecordSingle> data)
        {
            foreach (DataRecordSingle record in data)
                this.Add(record);
        }

        [API("Add a single DataRecord")]
        public void Add(DataRecordSingle record)
        {
            if (this.Count++ == 0)
                this.FirstSample = record;
            this.LastSample = record;
            if ((record.Flags & MeasurementFlags.TriggerDetected) != (MeasurementFlags)0)
            {
                ++this.TriggerCount;
                if (this.FirstTrigger == null)
                    this.FirstTrigger = record;
                this.LastTrigger = record;
            }
            double measurement = record.Measurement;
            if (this.Minimum > measurement)
                this.Minimum = measurement;
            if (this.Maximum < measurement)
                this.Maximum = measurement;
            if (measurement > this.BufferBounds.Upper)
                ++this.Above;
            if (measurement < this.BufferBounds.Lower)
                ++this.Below;
            double num = measurement - this.PulseAnalysisOptions.EnergyBaselineLevel;
            if (num <= 0.0)
                return;
            this.MeasurementSum += num;
        }

        [Conditional("TRACE")]
        protected void Trace(string format, params object[] args)
        {
        }

        [Conditional("TRACE_ELAPSED")]
        public void TraceElapsed(string fmt, params object[] args)
        {
        }
    }
}