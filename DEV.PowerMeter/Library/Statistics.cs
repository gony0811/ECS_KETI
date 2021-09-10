using SharedLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DEV.PowerMeter.Library
{
    [API(APICategory.Analysis, "The Statistics class accumulates various statistics about a range \r\n\t\tof data samples,  most typically and notably the contents of the \r\n\t\tcurrent CaptureBuffer.")]
    public class Statistics
    {
        public double DefaultMaximum = double.MinValue;
        public double DefaultMinimum = double.MaxValue;
        [API("Literal Zero = 0")]
        public const int Zero = 0;
        protected double? filtered;
        protected double Variance;

        public Units Units { get; set; }

        [API("Number of samples for these Statistics")]
        public int Count { get; protected set; }

        [API("Most recent sample for these Statistics (if there is one)")]
        public double? Current { get; protected set; }

        [API("Most recent sample for these Statistics (if there is one; else Zero)")]
        public double Live => !this.Current.HasValue ? 0.0 : this.Current.Value;

        [API("A simple filter applied to recent samples (if there are any)")]
        public double Filtered => !this.filtered.HasValue ? 0.0 : this.filtered.Value;

        [API("Minimum value seen thus far")]
        public double Minimum { get; protected set; }

        [API("Maximum value seen thus far")]
        public double Maximum { get; protected set; }

        [API("Count of samples with OverTemp Flag set")]
        public int OverTemp { get; protected set; }

        [API("Count of samples with OverRange Flag set")]
        public int OverRange { get; protected set; }

        [API("Count of samples with UnderRange Flag set")]
        public int UnderRange { get; protected set; }

        [API("Count of samples with Trigger Flag set")]
        public int Triggers { get; protected set; }

        [API("Count of samples with MissedPulse Flag set")]
        public int MissedPulses { get; protected set; }

        [API("Dose is sum of Energy samples (if any; else 0)")]
        public double Dose { get; protected set; }

        [API("Count of samples with MissingSamples Flag set")]
        public int MissingSamples { get; protected set; }

        [API("Count of samples where meter returned Impossibly Large values")]
        public int Impossible { get; protected set; }

        [API("MissingSamples + Impossible")]
        public int SevereErrors => this.MissingSamples + this.Impossible;

        [API("Sum of current samples")]
        public double Sum { get; protected set; }

        [API("Sum of squares of current samples")]
        public double SumSq { get; protected set; }

        [API("Mean value (µ) of current samples ")]
        public double Mean => this.Count <= 0 ? 0.0 : this.Sum / (double)this.Count;

        [API("Standard Deviation (σ or Sigma) of current samples ")]
        public double Stddev => this.StddevS;

        [API("Standard Deviation (σ or Sigma) of current samples (Alternative method 2)")]
        public double Stddev2
        {
            get
            {
                if (this.Count <= 1)
                    return 0.0;
                this.Variance = this.SumSq / (double)this.Count - this.Mean * this.Mean;
                return Math.Sqrt(Math.Max(0.0, this.Variance));
            }
        }

        [API("Standard Deviation (σ or Sigma) of current samples (Alternative method S)")]
        public double StddevS
        {
            get
            {
                if (this.Count <= 1)
                    return 0.0;
                this.Variance = (this.SumSq - this.Sum * this.Sum / (double)this.Count) / (double)(this.Count - 1);
                return Math.Sqrt(Math.Max(0.0, this.Variance));
            }
        }

        [API("Standard Deviation (σ or Sigma) of current samples (Alternative method P)")]
        public double StddevP
        {
            get
            {
                if (this.Count <= 1)
                    return 0.0;
                this.Variance = (this.SumSq - this.Sum * this.Sum / (double)this.Count) / (double)this.Count;
                return Math.Sqrt(this.Variance);
            }
        }

        [API("Standard Deviation (σ or Sigma) of current samples (Alternative Legacy LabMax, same as method P)")]
        public double Stddev_LabMax
        {
            get
            {
                if (this.Count <= 1)
                    return 0.0;
                this.Variance = ((double)this.Count * this.SumSq - this.Sum * this.Sum) / (double)(this.Count * (this.Count - 1));
                return Math.Sqrt(Math.Max(0.0, this.Variance));
            }
        }

        [Conditional("TRACE_STDDEV")]
        public void Stddev_Data(IEnumerable<DataRecordSingle> data)
        {
            int num1 = data.Count<DataRecordSingle>();
            double mean = data.Average<DataRecordSingle>((Func<DataRecordSingle, double>)(drs => drs.Measurement));
            this.Variance = data.Sum<DataRecordSingle>((Func<DataRecordSingle, double>)(drs => (drs.Measurement - mean) * (drs.Measurement - mean))) / (double)num1;
            double num2 = Math.Sqrt(this.Variance);
            Statistics.Trace("Stddev_Load Count={0}, Mean={1}, var={2}, stddev={3}", (object)num1, (object)mean, (object)this.Variance, (object)num2);
        }

        [API("Stddev/Mean (σ/µ) of current samples (or 0 if µ is zero)")]
        public double SigMean => this.Mean == 0.0 ? 0.0 : this.Stddev / this.Mean;

        [API("2*SigMean (2σ/µ) of current samples")]
        public double Sig2Mean => 2.0 * this.SigMean;

        [API("3*SigMean (3σ/µ) of current samples")]
        public double Sig3Mean => 3.0 * this.SigMean;

        [API("Create Statistics, Cleared")]
        public Statistics() => this.Clear();

        [API("Clear all statistics to Zero")]
        public void Clear()
        {
            this.Count = this.UnderRange = this.OverRange = this.OverTemp = this.Triggers = this.MissedPulses = this.MissingSamples = this.Impossible = 0;
            this.Sum = this.SumSq = this.Dose = 0.0;
            this.Current = this.filtered = new double?();
            this.Minimum = this.DefaultMinimum;
            this.Maximum = this.DefaultMaximum;
        }

        [API("Clear and load the entire contents of a CaptureBuffer")]
        public void Load(IEnumerable<DataRecordSingle> data)
        {
            this.Clear();
            this.AddRange(data);
        }

        [API("Load an IEnumerable<DataRecordSingle> (without Clearing)")]
        public void AddRange(IEnumerable<DataRecordSingle> data)
        {
            foreach (DataRecordSingle record in data)
                this.Add(record);
        }

        [API("Add a single DataRecord (without Clearing)")]
        public void Add(DataRecordSingle record)
        {
            double measurement = record.Measurement;
            this.Current = new double?(measurement);
            if ((record.Flags & MeasurementFlags.TriggerFlags) != (MeasurementFlags)0)
                ++this.Triggers;
            if ((record.Flags & MeasurementFlags.OverRange) != (MeasurementFlags)0)
                ++this.OverRange;
            if ((record.Flags & MeasurementFlags.UnderRange) != (MeasurementFlags)0)
                ++this.UnderRange;
            if ((record.Flags & MeasurementFlags.OverTemp) != (MeasurementFlags)0)
                ++this.OverTemp;
            if ((record.Flags & MeasurementFlags.MissingSamples) != (MeasurementFlags)0)
                ++this.MissingSamples;
            if ((record.Flags & MeasurementFlags.MissingPulse) != (MeasurementFlags)0)
                ++this.MissedPulses;
            if ((record.Flags & MeasurementFlags.Impossible) != (MeasurementFlags)0)
                ++this.Impossible;
            if (this.Units != null && this.Units.IsEnergy)
                this.Dose += measurement;
            this.filtered = !this.filtered.HasValue ? new double?(measurement) : new double?((this.filtered.Value + measurement) / 2.0);
            this.Sum += measurement;
            this.SumSq += measurement * measurement;
            if (this.Minimum > measurement)
                this.Minimum = measurement;
            if (this.Maximum < measurement)
                this.Maximum = measurement;
            ++this.Count;
        }

        public static void Trace(string format, params object[] args)
        {
        }

        [Conditional("TRACE_STDDEV")]
        public void TraceStddev()
        {
            Statistics.Trace("Statistics.Count, Mean {0}, {1}", (object)this.Count, (object)this.Mean);
            Statistics.Trace("Statistics.Min, Max, Diff {0}, {1}, {2}", (object)this.Minimum, (object)this.Maximum, (object)(this.Maximum - this.Minimum));
            Statistics.Trace("Statistics.Sum, SumSq {0}, {1}", (object)this.Sum, (object)this.SumSq);
            Statistics.Trace("Statistics.Stddev2, Var {0}, {1}", (object)this.Stddev2, (object)this.Variance);
            Statistics.Trace("Statistics.StddevS, Var {0}, {1}", (object)this.StddevS, (object)this.Variance);
            Statistics.Trace("Statistics.StddevP, Var {0}, {1}", (object)this.StddevP, (object)this.Variance);
            Statistics.Trace("Statistics.Legacy, Var {0}, {1}", (object)this.Stddev_LabMax, (object)this.Variance);
        }
    }
}
