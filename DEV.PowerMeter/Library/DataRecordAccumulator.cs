using System;
using System.Diagnostics;
using System.Globalization;

namespace DEV.PowerMeter.Library
{
    public class DataRecordAccumulator : IDataRecordSingle, IEncodable
    {
        public DataRecordSingle Data;

        public uint Count { get; protected set; }

        public bool IsQuad => this.Data is DataRecordQuad;

        public DateTime Timestamp
        {
            get => this.Data.Timestamp;
            set => this.Data.Timestamp = value;
        }

        public DataRecordAccumulator() => this.Data = new DataRecordSingle();

        public virtual void Clear()
        {
            this.Count = 0U;
            this.Data.Timestamp = new DateTime();
            this.Data.Measurement = 0.0;
            this.Data.Flags = (MeasurementFlags)0;
            this.Data.Period = 0U;
        }

        public void Tally(IDataRecordSingle data)
        {
            ++this.Count;
            this.Timestamp = data.Timestamp;
        }

        public virtual void Add(IDataRecordSingle data)
        {
            this.Tally(data);
            this.Data.Flags |= data.Flags;
            this.Data.Measurement += data.Measurement;
            this.Data.Period += data.Period;
            this.Data.Sequence = data.Sequence;
        }

        public ulong Sequence => this.Data.Sequence;

        public MeasurementFlags Flags
        {
            get => this.Data.Flags;
            set => throw new NotSupportedException();
        }

        public double Measurement => this.Data.Measurement / (double)this.Count;

        public uint Period => this.Data.Period / this.Count;

        public byte[] DataBytes => (byte[])null;

        public virtual string[] Encode(CultureInfo culture) => new DataRecordSingle((IDataRecordSingle)this).Encode(culture);

        public override string ToString() => string.Format("{0} {1} {2} {3} {4} // DRA single", (object)this.Timestamp.ToStringMicrosec(), (object)this.Measurement, (object)this.Sequence, (object)this.Flags.ToString().Replace(",", "|"), (object)this.Period);

        [Conditional("TRACE")]
        public void Trace(string message)
        {
        }

        [Conditional("TRACE_TALLY")]
        public void TraceTally(string message) => this.Trace(message);

        [Conditional("TRACE_ENCODE")]
        public void TraceEncode(string message) => this.Trace(message);

        [Conditional("TRACE_ADD")]
        public void TraceAdd(string message) => this.Trace(message);
    }
}
