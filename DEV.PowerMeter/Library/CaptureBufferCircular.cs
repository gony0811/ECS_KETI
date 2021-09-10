using DEV.PowerMeter.Library.ImportExport;
using SharedLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DEV.PowerMeter.Library
{
    [API(APICategory.Measurement, "CaptureBufferCircular is a concrete class, which can accept data at high-speed indefinitely and with constant access times. However, it's capacity is necessarily fixed. It's the destination for all data acquisition by the Meter.")]
    public class CaptureBufferCircular : CaptureBuffer
    {
        protected DataRecordSingle[] DataBuffer;
        protected uint count;
        private object Buffer_Lock = new object();

        [API("user-specified capacity of buffer.")]
        public uint Capacity { get; protected set; }

        public uint DataIn { get; protected set; }

        public uint DataOut { get; protected set; }

        public override uint Count => this.count;

        public override bool IsPreview => false;

        public override IEnumerator<DataRecordSingle> GetEnumerator() => (IEnumerator<DataRecordSingle>)new CircularBufferEnumerator<DataRecordSingle>((IList<DataRecordSingle>)this.DataBuffer, this.Count, this.DataOut);

        [API("Create Circular CaptureBuffer of given Capacity, with an initialized Header.")]
        public CaptureBufferCircular(Header header, uint capacity)
          : base(header)
        {
            this.SetCapacity(capacity);
        }

        public CaptureBufferCircular(Units units, uint capacity)
          : base(new Header(units))
        {
            this.SetCapacity(capacity);
        }

        [API("Change buffer capacity without re-creating CaptureBufferCircular object.")]
        public void SetCapacity(uint capacity)
        {
            lock (this.Buffer_Lock)
            {
                this.Capacity = capacity;
                this.DataBuffer = !this.Sensor_IsQuad ? new DataRecordSingle[(int)capacity] : (DataRecordSingle[])new DataRecordQuad[(int)capacity];
                if (this.Sensor_IsQuad)
                {
                    for (int index = 0; (long)index < (long)capacity; ++index)
                        this.DataBuffer[index] = (DataRecordSingle)new DataRecordQuad();
                }
                else
                {
                    for (int index = 0; (long)index < (long)capacity; ++index)
                        this.DataBuffer[index] = new DataRecordSingle();
                }
                this.Clear();
            }
        }

        [API("Discard all captured data.")]
        public override void Clear()
        {
            base.Clear();
            lock (this.Buffer_Lock)
                this.DataIn = this.DataOut = this.count = 0U;
        }

        public override DataRecordSingle this[int index]
        {
            get => this.DataBuffer[(int)this.Index(index)];
            set => this.DataBuffer[(int)this.Index(index)] = value;
        }

        protected uint Index(int index) => index >= 0 ? (this.DataOut + (uint)index) % this.Capacity : (uint)((int)this.Capacity + (int)this.DataIn - Math.Abs(index)) % this.Capacity;

        [API("Add a DataRecord that already has a valid timestamp")]
        protected override void Add(DataRecordSingle item)
        {
            lock (this.Buffer_Lock)
                this.DataBuffer[(int)this.MakeRoom()].Set((IDataRecordSingle)item);
        }

        public virtual uint MakeRoom()
        {
            lock (this.Buffer_Lock)
            {
                int dataIn = (int)this.DataIn;
                if (this.Count >= this.Capacity)
                {
                    ++this.DataOut;
                    this.DataOut %= this.Capacity;
                }
                else
                    ++this.count;
                ++this.DataIn;
                this.DataIn %= this.Capacity;
                return (uint)dataIn;
            }
        }

        public DataRecordSingle[] MostRecent(uint N)
        {
            if (N >= this.Count)
                N = this.Count;
            DataRecordSingle[] dataRecordSingleArray = new DataRecordSingle[(int)N];
            uint num1 = this.Index(-(int)N);
            if (num1 + N <= this.Count)
            {
                Array.Copy((Array)this.DataBuffer, (long)num1, (Array)dataRecordSingleArray, 0L, (long)N);
            }
            else
            {
                uint num2 = this.Count - num1;
                uint num3 = N - num2;
                Array.Copy((Array)this.DataBuffer, (long)num1, (Array)dataRecordSingleArray, 0L, (long)num2);
                Array.Copy((Array)this.DataBuffer, 0L, (Array)dataRecordSingleArray, (long)num2, (long)num3);
            }
            return dataRecordSingleArray;
        }

        public void Update(CaptureBufferCircular captureBuffer)
        {
            lock (this.Buffer_Lock)
            {
                DataRecordSingle[] dataRecordSingleArray = captureBuffer.MostRecent(this.Capacity);
                for (int index = 0; index < dataRecordSingleArray.Length; ++index)
                    this[index].Set((IDataRecordSingle)dataRecordSingleArray[index]);
                this.DataIn = this.DataOut = 0U;
                this.count = (uint)dataRecordSingleArray.Length;
            }
        }

        public string ToStringEx() => string.Format("count={0}, in={1}, out={2}", (object)this.Count, (object)this.DataIn, (object)this.DataOut);

        [Conditional("TRACE_BUFFER")]
        public new static void TRACE(string fmt, params object[] args)
        {
        }

        [Conditional("TRACE_ADD")]
        public void TraceAdd(uint index, DataRecordSingle item)
        {
        }

        [Conditional("TRACE_ADD_REMOVE")]
        public static void TRACE_ADD_REMOVE(string fmt, params object[] args)
        {
        }

        [Conditional("TRACE_MOST_RECENT")]
        public static void TraceMostRecent(string fmt, params object[] args)
        {
        }

        [Conditional("TRACE_MOST_RECENT_TIMING")]
        public static void TraceMostRecentTiming(string fmt, params object[] args)
        {
        }

        [Conditional("TRACE_NEGATIVE_INDICIES")]
        public static void TraceNegativeIndicies(string fmt, params object[] args)
        {
        }

        [Conditional("TRACE_SELECTION")]
        public static void TraceSelection(string fmt, params object[] args)
        {
        }

        [Conditional("TRACE_SELECTION")]
        public static void TraceSelection(IEnumerable<DataRecordSingle> selection)
        {
            foreach (DataRecordSingle dataRecordSingle in selection)
                ;
        }

        [Conditional("CAPTURE_TRACE_OUTPUT")]
        public static void AppendFormat(string fmt, params object[] args)
        {
        }

        [Conditional("TRACE_VALIDATION")]
        public static void TraceValidation(string fmt, params object[] args)
        {
        }

        public static string ToString(DataRecordSingle item) => string.Format(" {0}/{1} {2}", (object)item.Timestamp.Ticks, (object)item.Timestamp.ToStringMicrosec(), (object)item.Measurement);

        public static bool Validate(DataRecordSingle older, DataRecordSingle newer, int index)
        {
            DateTime timestamp = older.Timestamp;
            long ticks1 = timestamp.Ticks;
            timestamp = newer.Timestamp;
            long ticks2 = timestamp.Ticks;
            return ticks1 <= ticks2;
        }

        public static int ValidateRange(IEnumerable<DataRecordSingle> selection)
        {
            DataRecordSingle older = selection.FirstOrDefault<DataRecordSingle>();
            DataRecordSingle dataRecordSingle = older;
            int num = 0;
            int index = 0;
            if (older == null)
                return 0;
            foreach (DataRecordSingle newer in selection.Skip<DataRecordSingle>(1))
            {
                if (!CaptureBufferCircular.Validate(dataRecordSingle, newer, index++))
                    ++num;
                dataRecordSingle = newer;
            }
            if (!CaptureBufferCircular.Validate(older, dataRecordSingle, index))
                ++num;
            return num;
        }
    }
}
