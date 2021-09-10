using DEV.PowerMeter.Library.ImportExport;
using SharedLibrary;
using System;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace DEV.PowerMeter.Library
{
    [DataContract]
    [API(APICategory.Utility)]
    public class DataLogger
    {
        protected DataRecordAccumulator Accumulator = new DataRecordAccumulator();
        protected CaptureBuffer CaptureBuffer;
        protected Exporter Exporter;
        public AutoFlusher AutoFlusher = new AutoFlusher();
        protected double Interval;
        protected DateTime NextTime;
        protected int RecordsWritten;
        protected DataAdded CurrentDataAddedFunction;
        public readonly string[] TimeScaleNames = new string[6]
        {
      "Micro-Seconds",
      "Milli-Seconds",
      "Seconds",
      "Minutes",
      "Hours",
      "Days"
        };
        public readonly double[] TimeScaleToSeconds = new double[6]
        {
      1E-06,
      0.001,
      1.0,
      60.0,
      3600.0,
      86400.0
        };

        [DataMember]
        [API(APICategory.Utility)]
        public bool LoggingEnabled { get; set; }

        [DataMember]
        [API(APICategory.Utility)]
        public string Destination { get; set; }

        [API(APICategory.Utility)]
        [DataMember]
        public DestinationCollisionRemedy FileNameCollisionRemedy { get; set; }

        [API(APICategory.Utility)]
        [DataMember]
        public FileFormat FileFormat { get; set; }

        [API(APICategory.Utility)]
        [DataMember]
        public ReductionType ReductionType { get; set; }

        [API(APICategory.Utility)]
        [DataMember]
        [PreviouslyNamed("SampleInterval")]
        public double ReductionTimeFactor { get; set; }

        [API(APICategory.Utility)]
        [DataMember]
        public TimeScale TimeScale { get; set; }

        [API(APICategory.Utility)]
        [DataMember]
        [PreviouslyNamed("SampleCount")]
        public uint ReductionSkipCount { get; set; }

        [API(APICategory.Utility)]
        [DataMember]
        public SkippedSampleAction SkippedSampleAction { get; set; }

        public bool LoggingIsActive => this.Exporter != null;

        public DataLogger()
        {
            this.LoggingEnabled = false;
            this.Destination = (string)null;
            this.ReductionTimeFactor = 1.0;
            this.ReductionSkipCount = 1000U;
            this.ReductionType = ReductionType.Time;
            this.SkippedSampleAction = SkippedSampleAction.Skip;
            this.TimeScale = TimeScale.Seconds;
            this.FileFormat = FileFormat.CSV;
            this.FileNameCollisionRemedy = DestinationCollisionRemedy.Overwrite;
        }

        public bool ValidateReductionType(Meter meter)
        {
            int num = meter.OperatingMode_IsTrueEnergy ? 1 : (meter.Sensor_IsPyro ? 1 : 0);
            if (num == 0)
                return num != 0;
            if (this.ReductionType != ReductionType.Time)
                return num != 0;
            this.ReductionType = ReductionType.None;
            return num != 0;
        }

        public void Startup(CaptureBuffer captureBuffer, Meter meter)
        {
            this.ValidateReductionType(meter);
            this.Interval = this.ReductionTimeFactor * this.TimeScaleToSeconds[(int)this.TimeScale];
            this.NextTime = DateTimeExtensions.FromSeconds(this.Interval);
            this.CaptureBuffer = captureBuffer;
            if (!this.LoggingEnabled || this.CaptureBuffer == null)
                return;
            this.Accumulator = !this.CaptureBuffer.Sensor_IsQuad ? new DataRecordAccumulator() : (DataRecordAccumulator)new DataRecordAccumulatorQuad();
            this.Accumulator.Clear();
            if (this.AutoFlusher != null)
                this.AutoFlusher.Restart();
            this.Exporter = new Exporter(this.Destination, this.CaptureBuffer);
            this.Exporter.Create(this.FileNameCollisionRemedy);
            this.Destination = this.Exporter.Filename;
            this.RecordsWritten = 0;
            this.CurrentDataAddedFunction = this.DataAddedFunction;
            this.CaptureBuffer.DataAdded += this.CurrentDataAddedFunction;
        }

        public void Shutdown()
        {
            if (!this.LoggingIsActive)
                return;
            this.CaptureBuffer.DataAdded -= this.CurrentDataAddedFunction;
            this.Exporter.Close();
            this.Exporter = (Exporter)null;
        }

        public void ExportDatum(IDataRecordSingle data)
        {
            this.Exporter.Export(data);
            ++this.RecordsWritten;
            if (this.AutoFlusher == null || !this.AutoFlusher.ShouldDoFlush(data.Timestamp))
                return;
            this.Exporter.Flush();
        }

        protected void DataAddedTimeSkip(IDataRecordSingle data)
        {
            this.Accumulator.Tally(data);
            if (!(this.Accumulator.Timestamp >= this.NextTime))
                return;
            this.NextTime = this.Accumulator.Timestamp.AddSecondsAccurately(this.Interval);
            this.ExportDatum(data);
            this.Accumulator.Clear();
        }

        protected void DataAddedTimeMean(IDataRecordSingle data)
        {
            this.Accumulator.Add(data);
            if (!(this.Accumulator.Timestamp >= this.NextTime))
                return;
            this.NextTime = this.Accumulator.Timestamp.AddSecondsAccurately(this.Interval);
            this.ExportDatum((IDataRecordSingle)this.Accumulator);
            this.Accumulator.Clear();
        }

        protected void DataAddedCountMean(IDataRecordSingle data)
        {
            this.Accumulator.Add(data);
            if (this.Accumulator.Count < this.ReductionSkipCount)
                return;
            this.ExportDatum((IDataRecordSingle)this.Accumulator);
            this.Accumulator.Clear();
        }

        protected void DataAddedCountSkip(IDataRecordSingle data)
        {
            this.Accumulator.Tally(data);
            if (this.Accumulator.Count < this.ReductionSkipCount)
                return;
            this.ExportDatum(data);
            this.Accumulator.Clear();
        }

        protected DataAdded DataAddedFunction
        {
            get
            {
                switch (this.ReductionType)
                {
                    case ReductionType.Time:
                        switch (this.SkippedSampleAction)
                        {
                            case SkippedSampleAction.Skip:
                                return new DataAdded(this.DataAddedTimeSkip);
                            case SkippedSampleAction.Mean:
                                return new DataAdded(this.DataAddedTimeMean);
                        }
                        break;
                    case ReductionType.Count:
                        switch (this.SkippedSampleAction)
                        {
                            case SkippedSampleAction.Skip:
                                return new DataAdded(this.DataAddedCountSkip);
                            case SkippedSampleAction.Mean:
                                return new DataAdded(this.DataAddedCountMean);
                        }
                        break;
                }
                return new DataAdded(this.ExportDatum);
            }
        }

        [API(APICategory.Utility)]
        public string Settings
        {
            get => SharedLibrary.Serialization.Serialize((object)this);
            set
            {
                try
                {
                    this.DeSerialize(value);
                }
                catch (System.Exception ex)
                {
                    this.Trace("DeSerialize Exception: " + ex.Message);
                }
            }
        }

        [Conditional("TRACE")]
        public void Trace(string message)
        {
        }

        [Conditional("TRACE_START_STOP")]
        public void TraceStartStop(string message) => this.Trace(message);

        [Conditional("TRACE_DATA_ADDED")]
        public void TraceDataAdded(string message) => this.Trace(message);

        [Conditional("TRACE_FLUSHING")]
        public void TraceFlushing(string message) => this.Trace(message);
    }
}
