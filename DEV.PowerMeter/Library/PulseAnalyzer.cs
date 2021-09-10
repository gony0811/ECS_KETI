
using SharedLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace DEV.PowerMeter.Library
{
    public class PulseAnalyzer
    {
        protected State? CurrentState;
        protected Zone PreviousZone;
        [API(APICategory.Unclassified)]
        public EnergyState EnergyState;
        protected PulseProperties Pending;
        private DateTime PreviousTimestamp;
        public readonly State[] LeadingEdge = new State[4]
        {
      State.Waiting,
      State.RisingLo,
      State.RisingHi,
      State.Running
        };
        public readonly State[] TrailingEdge = new State[4]
        {
      State.Accept,
      State.FallingLo,
      State.FallingHi,
      State.Running
        };

        public PulseAnalysisOptions Options { get; protected set; }

        public PulseAnalysisResults Results { get; protected set; }

        public event Action<PulseAnalyzer> AnalysisComplete;

        protected void OnAnalysisComplete()
        {
            Action<PulseAnalyzer> analysisComplete = this.AnalysisComplete;
            if (analysisComplete == null)
                return;
            analysisComplete(this);
        }

        [API(APICategory.Unclassified)]
        protected bool IsCalculating => this.EnergyState == EnergyState.Calculating;

        [API(APICategory.Unclassified)]
        public PulseAnalyzer(PulseAnalysisOptions options)
        {
            this.Results = new PulseAnalysisResults();
            this.Options = options;
            this.Clear();
        }

        [API(APICategory.Unclassified)]
        public void Clear()
        {
            this.CurrentState = new State?();
            this.PreviousZone = Zone.None;
            this.NewPendingPulse();
            this.Results.Clear();
        }

        protected void AddPendingPulse()
        {
            this.Results.Add(this.Pending);
            this.NewPendingPulse();
        }

        protected void NewPendingPulse()
        {
            this.Pending = new PulseProperties(this.Options);
            this.EnergyState = EnergyState.Searching;
        }

        public void SetThresholds(double minimum, double maximum)
        {
            this.Options.SetMinAndMax(minimum, maximum);
            this.CalculateStartStopSates(this.Options.WidthStartThreshold, this.Options.WidthStopThreshold);
        }

        [API(APICategory.Unclassified)]
        public void Reload(IEnumerable<DataRecordSingle> data)
        {
            Stopwatch.StartNew();
            this.Clear();
            this.AddRange(data);
            this.OnAnalysisComplete();
        }

        [API(APICategory.Unclassified)]
        public void AddRange(IEnumerable<DataRecordSingle> data)
        {
            foreach (DataRecordSingle datum in data)
                this.Add(datum);
        }

        private double SamplePeriod_Seconds { get; set; }

        [API(APICategory.Unclassified)]
        public void Add(DataRecordSingle datum)
        {
            Zone zone = this.Options.Classify(datum.Measurement);
            this.SamplePeriod_Seconds = (datum.Timestamp - this.PreviousTimestamp).TotalSeconds;
            this.PreviousTimestamp = datum.Timestamp;
            if (this.PreviousZone != zone)
            {
                this.PreviousZone = zone;
                this.Analyze(zone, datum);
                if (this.CurrentState.HasValue && this.CurrentState.Value == State.Accept)
                {
                    this.AddPendingPulse();
                    this.CurrentState = new State?(State.Waiting);
                    this.EnergyState = EnergyState.Searching;
                    return;
                }
            }
            this.UpdateEnergy(datum);
        }

        public void Analyze(Zone zone, DataRecordSingle datum)
        {
            if (!this.CurrentState.HasValue)
            {
                if (zone != Zone.Bottom)
                    return;
                this.CurrentState = new State?(State.Waiting);
            }
            State? nullable = new State?(this.NextState(zone));
            State state1 = this.CurrentState.Value;
            if (!nullable.HasValue || nullable.Value == state1)
                return;
            State state2 = nullable.Value;
            bool flag = state2 > state1;
            do
            {
                this.ExitStateAction(state1, datum);
                if (flag)
                    ++state1;
                else
                    --state1;
            }
            while (state1 != state2);
            this.CurrentState = new State?(state2);
        }

        public void ExitStateAction(State state, DataRecordSingle datum)
        {
            DateTime timestamp = datum.Timestamp;
            switch (state)
            {
                case State.Waiting:
                    this.Pending.RiseStartTime = timestamp;
                    break;
                case State.RisingLo:
                    this.Pending.MiddleStartTime = timestamp;
                    break;
                case State.RisingHi:
                    this.Pending.RiseStopTime = timestamp;
                    break;
                case State.Running:
                    this.Pending.FallStartTime = timestamp;
                    break;
                case State.FallingHi:
                    this.Pending.MiddleStopTime = timestamp;
                    break;
                case State.FallingLo:
                    this.Pending.FallStopTime = timestamp;
                    break;
                case State.Accept:
                    throw new NotSupportedException();
            }
        }

        protected State NextState(Zone zone)
        {
            int index = (int)zone;
            switch (this.CurrentState.Value)
            {
                case State.Waiting:
                case State.RisingLo:
                case State.RisingHi:
                    return this.LeadingEdge[index];
                case State.Running:
                case State.FallingHi:
                case State.FallingLo:
                    return this.TrailingEdge[index];
                default:
                    return State.Waiting;
            }
        }

        public State StartingState { get; protected set; }

        public State StoppingState { get; protected set; }

        protected void CalculateStartStopSates(Level startThreshold, Level stopThreshold)
        {
            switch (startThreshold)
            {
                case Level.Lower:
                    this.StartingState = State.RisingLo;
                    break;
                case Level.Middle:
                    this.StartingState = State.RisingHi;
                    break;
                case Level.Upper:
                    this.StartingState = State.Running;
                    break;
            }
            switch (stopThreshold)
            {
                case Level.Lower:
                    this.StoppingState = State.Accept;
                    break;
                case Level.Middle:
                    this.StoppingState = State.FallingLo;
                    break;
                case Level.Upper:
                    this.StoppingState = State.FallingHi;
                    break;
            }
        }

        public void UpdateEnergy(DataRecordSingle datum)
        {
            if (!this.CurrentState.HasValue)
                return;
            State state = this.CurrentState.Value;
            switch (this.EnergyState)
            {
                case EnergyState.Searching:
                    if (state < this.StartingState)
                        return;
                    this.EnergyState = EnergyState.Calculating;
                    break;
                case EnergyState.Calculating:
                    if (state >= this.StoppingState)
                    {
                        this.EnergyState = EnergyState.Finished;
                        break;
                    }
                    break;
                default:
                    return;
            }
            this.Pending.Energy.Add(datum);
        }

        public void WriteReport(string filename)
        {
            using (TextWriter textUtF8 = (TextWriter)FileX.CreateTextUTF8(filename))
                this.WriteReport(textUtF8);
        }

        protected void WriteReport(TextWriter writer)
        {
            this.Results.WriteReport(writer);
            writer.WriteLine();
            this.Options.WriteReport(writer);
        }

        [Conditional("TRACE")]
        protected void Trace(string format, params object[] args)
        {
        }

        [Conditional("TRACE_ADD")]
        protected void TraceAdd(string format, params object[] args)
        {
        }

        [Conditional("TRACE_STATE")]
        protected void TraceState(string format, params object[] args) => this.Trace(string.Format(format, args));

        [Conditional("TRACE_ADD_PULSE")]
        protected void TraceAddPulse(string format, params object[] args) => this.Trace(string.Format(format, args));

        [Conditional("TRACE_ENERGY")]
        protected void TraceEnergy(string format, params object[] args) => this.Trace(string.Format("Energy: " + format, args));

        [Conditional("TRACE_RESULTS")]
        protected void TraceResults(string format, params object[] args) => this.Trace(string.Format("Energy: " + format, args));

        [Conditional("TRACE_ELAPSED")]
        public void TraceElapsed(string fmt, params object[] args)
        {
        }
    }
}
