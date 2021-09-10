
using SharedLibrary;
using System;
using System.Diagnostics;

namespace DEV.PowerMeter.Library
{
    [API(APICategory.Analysis)]
    public class EnergyCalculator
    {
        [API(APICategory.Unclassified)]
        public int Count { get; protected set; }

        [API(APICategory.Unclassified)]
        public DateTime StartTime { get; set; }

        [API(APICategory.Unclassified)]
        public DateTime StopTime { get; set; }

        [API(APICategory.Unclassified)]
        public double Width => (this.StopTime - this.StartTime).TotalSeconds;

        [API(APICategory.Unclassified)]
        public double PowerSum { get; protected set; }

        [API(APICategory.Unclassified)]
        public double AveragePower => this.PowerSum / (double)this.Count;

        [API(APICategory.Unclassified)]
        public double SamplePeriod { get; protected set; }

        [API(APICategory.Unclassified)]
        public double Total => EnergyCalculator.Energy(this.PowerSum, this.Width, this.Count);

        [API(APICategory.Unclassified)]
        public double PeakPower { get; protected set; }

        [API(APICategory.Unclassified)]
        public DateTime PeakPowerTime { get; protected set; }

        [API(APICategory.Unclassified)]
        public double Height => this.PeakPower - this.Baseline;

        protected double Baseline { get; set; }

        public static double Energy(double powerSum, double intervalWidth, int count) => count <= 0 ? 0.0 : EnergyCalculator.Energy(powerSum, intervalWidth / (double)count);

        public static double Energy(double powerSum, double pulseWidth) => powerSum * pulseWidth;

        [API(APICategory.Unclassified)]
        public EnergyCalculator(double energyBaseline)
        {
            this.Baseline = energyBaseline;
            this.SamplePeriod = 0.0;
            this.Clear();
        }

        [API(APICategory.Unclassified)]
        public void Clear()
        {
            this.CurrentState = EnergyCalculator.State.Searching;
            this.PeakPower = double.MinValue;
            this.PowerSum = 0.0;
            this.Count = 0;
            DateTime dateTime = new DateTime();
            this.StopTime = dateTime;
            this.StartTime = dateTime;
        }

        public EnergyCalculator.State CurrentState { get; protected set; }

        public void Add(DataRecordSingle datum)
        {
            DateTime timestamp = datum.Timestamp;
            double measurement = datum.Measurement;
            if (this.Count == 0)
                this.StartTime = timestamp;
            this.StopTime = timestamp;
            if (this.PeakPower < measurement)
            {
                this.PeakPowerTime = timestamp;
                this.PeakPower = measurement;
            }
            this.Integrate(measurement);
        }

        protected void Integrate(double powerSample)
        {
            ++this.Count;
            powerSample -= this.Baseline;
            if (powerSample <= 0.0)
                return;
            this.PowerSum += powerSample;
        }

        protected void Trace(string format, params object[] args)
        {
        }

        [Conditional("TRACE_SAMPLES")]
        protected void TraceSamples(string format, params object[] args) => this.Trace(format, args);

        public enum State
        {
            Searching,
            Calculating,
            Finished,
        }
    }
}
