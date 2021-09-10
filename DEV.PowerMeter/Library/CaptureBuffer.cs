using DEV.PowerMeter.Library.ImportExport;
using SharedLibrary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace DEV.PowerMeter.Library
{
    [API(APICategory.Measurement, "Abstract base class for Capture Buffers. Most properties and methods are implemented herein, though some are overriden and a few are implemented only in the sub-classes.")]
    public abstract class CaptureBuffer :
       IEnumerable,
       IEnumerable<DataRecordSingle>,
       IIsQuadOrPyro,
       ISamplePeriod,
       ICaptureBuffer
    {
        private object DataAddedEventLock = new object();

        [API("The number of active items; Count <= Capacity for CircularBuffers")]
        public abstract uint Count { get; }

        public bool IsImported => this is CaptureBufferUnbounded;

        [API("Current Header")]
        public Header Header { get; set; }

        [API("Current Header")]
        public bool BufferBounds_IsInitialized => this.Header.BufferBounds != null && this.Header.BufferBounds.IsInitialized;

        [API("The applicable units when data was acquired")]
        public Units Units => this.Header.Units;

        [API("Change Units")]
        public void SetUnits(Units units) => this.Header.Units = units;

        [API("True iff data originated  from a quad sensor")]
        public bool Sensor_IsQuad
        {
            get => this.Header.Sensor_IsQuad;
            set => this.Header.Sensor_IsQuad = value;
        }

        [API("True iff buffer contains energy data")]
        public bool Sensor_IsPyro => this.Header.Sensor_IsPyro;

        public abstract bool IsPreview { get; }

        [API("Optional energy records for LPEM mode (this property may be enumerated)")]
        public EnergyRecords EnergyRecords { get; protected set; }

        [API("count of trigger records")]
        public int TriggerCount => this.Count<DataRecordSingle>((Func<DataRecordSingle, bool>)(drb => (uint)(drb.Flags & MeasurementFlags.TriggerDetected) > 0U));

        public CaptureBuffer ParentBuffer { get; set; }

        public DataSkipBuffer SkipBuffer { get; set; }

        [API("Filename if this buffer was imported or exported")]
        public string Filename => this.Header.Filename;

        [API("Basename of Filename")]
        public string FileBasename => this.Filename == null ? (string)null : Path.GetFileName(this.Filename);

        [API("True if buffer contents have not been saved")]
        public bool HasUnsavedData => this.Count > 0U && this.Filename == null;

        [API("Estimated sample period of buffer contents, based on first two samples")]
        public long SamplePeriod
        {
            get
            {
                if (this.Sensor_IsPyro)
                    return 1;
                return this.Count > 1U ? (this[1].Timestamp - this[0].Timestamp).Ticks : 0L;
            }
        }

        [API("Return the index of the sample closest to the indicated DateTime")]
        public uint IndexOf(DateTime key)
        {
            if (this.Count == 0U)
                throw new IndexOutOfRangeException();
            uint num1 = this.Count - 1U;
            uint num2 = 0;
            while (num2 <= num1)
            {
                uint index = (num2 + num1) / 2U;
                if (this[index].Timestamp < key)
                {
                    num2 = index + 1U;
                }
                else
                {
                    if (!(this[index].Timestamp > key) || index <= 0U)
                        return index;
                    num1 = index - 1U;
                }
            }
            return num1;
        }

        [API("Access data sample closest to indicated DateTime")]
        public DataRecordSingle this[DateTime dateTime] => this[this.IndexOf(dateTime)];

        [API("First sample in buffer (throws if buffer empty)")]
        public DataRecordSingle First => this[0];

        [API("Last sample in buffer (throws if buffer empty)")]
        public DataRecordSingle Last => this[this.Count - 1U];

        [API("Access data sample based on numerical index")]
        public DataRecordSingle this[uint index] => this[(int)index];

        public abstract DataRecordSingle this[int index] { get; set; }

        protected uint FindNextTrigger(uint start)
        {
            uint index = start;
            while (index < this.Count - 1U)
            {
                ++index;
                if ((this[index].Flags & MeasurementFlags.TriggerDetected) != (MeasurementFlags)0)
                    return index;
            }
            throw new IndexOutOfRangeException(string.Format("FindNextTrigger( {0} )", (object)start));
        }

        protected uint FindPreviousTrigger(uint start)
        {
            uint index = start;
            while (index > 0U)
            {
                --index;
                if ((this[index].Flags & MeasurementFlags.TriggerDetected) != (MeasurementFlags)0)
                    return index;
            }
            throw new IndexOutOfRangeException(string.Format("FindNextTrigger( {0} )", (object)start));
        }

        [API("Find the index of a trigger sample closest to the indicated index")]
        public uint FindClosestTrigger(uint index)
        {
            if ((this[index].Flags & MeasurementFlags.TriggerDetected) != (MeasurementFlags)0)
                return index;
            uint num1 = 0;
            bool flag1 = true;
            try
            {
                num1 = this.FindNextTrigger(index);
            }
            catch (IndexOutOfRangeException ex)
            {
                flag1 = false;
            }
            uint num2 = 0;
            bool flag2 = true;
            try
            {
                num2 = this.FindPreviousTrigger(index);
            }
            catch (IndexOutOfRangeException ex)
            {
                flag2 = false;
            }
            if (flag2 & flag1)
                return num1 - index > index - num2 ? num2 : num1;
            if (flag2)
                return num2;
            if (flag1)
                return num1;
            throw new IndexOutOfRangeException(string.Format("FindClosestTrigger( {0} )", (object)index));
        }

        [API("Find the index of a trigger sample closest to the indicated DateTime")]
        public uint FindClosestTrigger(DateTime dateTime) => this.FindClosestTrigger(this.IndexOf(dateTime));

        public abstract IEnumerator<DataRecordSingle> GetEnumerator();

        [API("Enumerate the Power samples in this buffer")]
        IEnumerator IEnumerable.GetEnumerator() => (IEnumerator)this.GetEnumerator();

        [API("Enumerate the combined Power and Energy samples in this buffer")]
        public IEnumerable<DataRecordSingle> PowerAndEnergy => (IEnumerable<DataRecordSingle>)this.Concat<DataRecordSingle>((IEnumerable<DataRecordSingle>)this.EnergyRecords.Data).OrderBy<DataRecordSingle, DateTime>((Func<DataRecordSingle, DateTime>)(drs => drs.Timestamp));

        [API("Create a Slice of this buffer, bounded by the indicated BufferBounds")]
        public IEnumerable<DataRecordSingle> Slice(BufferBounds bounds) => this.Slice(bounds.First, bounds.Last);

        [API("Create a Slice of this buffer, bounded by the indicated DateTimes")]
        public IEnumerable<DataRecordSingle> Slice(
          DateTime first,
          DateTime last)
        {
            foreach (DataRecordSingle dataRecordSingle in this)
            {
                if (!(dataRecordSingle.Timestamp > last))
                {
                    if (dataRecordSingle.Timestamp >= first)
                        yield return dataRecordSingle;
                }
                else
                    break;
            }
        }

        [API("Create a Slice of this buffer, bounded by the indicated indices")]
        public IEnumerable<DataRecordSingle> Slice(int first, int last)
        {
            for (int i = first; i < last; ++i)
                yield return this[i];
        }

        protected CaptureBuffer(Header header)
        {
            this.Header = header;
            this.EnergyRecords = new EnergyRecords();
        }

        [API("Clear the buffer")]
        public virtual void Clear()
        {
            this.Header.Clear();
            this.EnergyRecords.Clear();
        }

        private event Library.DataAdded DataAddedEvent;

        [API("DataAdded event triggered each time a sample is added to the buffer")]
        public event Library.DataAdded DataAdded
        {
            add
            {
                lock (this.DataAddedEventLock)
                    this.DataAddedEvent += value;
            }
            remove
            {
                lock (this.DataAddedEventLock)
                    this.DataAddedEvent -= value;
            }
        }

        public void OnDataAdded(DataRecordSingle item)
        {
            lock (this.DataAddedEventLock)
            {
                Library.DataAdded dataAddedEvent = this.DataAddedEvent;
                if (dataAddedEvent == null)
                    return;
                dataAddedEvent((IDataRecordSingle)item);
            }
        }

        protected abstract void Add(DataRecordSingle item);

        [API("Add a DataRecord, computing and assigning a suitable timestamp")]
        public void TimestampAndAdd(DataRecordSingle item)
        {
            Timestamper.AssignNewTimestamp(item);
            this.AddTimestampedItem(item);
            this.OnDataAdded(item);
        }

        [API("Add a DataRecord that already has a valid timestamp")]
        public void AddTimestampedItem(DataRecordSingle item)
        {
            if ((item.Flags & MeasurementFlags.FinalEnergyRecord) != (MeasurementFlags)0)
            {
                this.AddEnergyRecord(item);
            }
            else
            {
                this.Sensor_IsQuad |= item is DataRecordQuad;
                this.Add(item);
            }
        }

        [API("Add an Energy DataRecord")]
        public void AddEnergyRecord(DataRecordSingle item)
        {
            DataRecordSingle dataRecordSingle = !(item is IDataRecordQuad) ? new DataRecordSingle((IDataRecordSingle)item) : (DataRecordSingle)new DataRecordQuad(item as IDataRecordQuad);
            dataRecordSingle.Timestamp = item.Timestamp;
            this.EnergyRecords.Add(dataRecordSingle);
            if (this.Count <= 0U)
                return;
            this.EnergyRecords.Remove(this.First.Timestamp);
        }

        [API("Clear the buffer and add all the supplied DataRecords")]
        public void Load(IEnumerable<DataRecordSingle> data)
        {
            this.Clear();
            this.AddRange(data);
        }

        [API("Append additional DataRecords to the buffer")]
        public void AddRange(IEnumerable<DataRecordSingle> data)
        {
            if (data == null)
                return;
            foreach (DataRecordSingle dataRecordSingle in data)
                this.AddTimestampedItem(dataRecordSingle);
        }

        private void LoadEnergyRecords(EnergyRecords energyRecords) => this.EnergyRecords.Load((IEnumerable<DataRecordSingle>)energyRecords.Data);

        [API("Returns a newly created buffer, after loading it with the contents of the indicated file")]
        public static CaptureBuffer Import(string filename)
        {
            Importer importer = new Importer(filename);
            importer.Import();
            return importer.Buffer;
        }

        [API("Export all or part of this buffer to an external file")]
        public void Export(string filename, bool sliceToBounds = false) => CaptureBuffer.Export(filename, this, sliceToBounds);

        [API("Export all or part of a buffer to an external file")]
        public static void Export(string filename, CaptureBuffer captureBuffer, bool sliceToBounds = false) => Exporter.Export(filename, captureBuffer, sliceToBounds);

        [API("Provide a succinct summary of the buffer contents")]
        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (this.ParentBuffer != null && this.ParentBuffer.Count > this.Count)
                stringBuilder.Append(string.Format("{0} (of {1}) samples", (object)this.Count, (object)this.ParentBuffer.Count));
            else
                stringBuilder.Append(string.Format("{0} samples", (object)this.Count));
            if (this.Count > 0U && !this.Last.Timestamp.IsZero())
            {
                if (this.Sensor_IsPyro)
                    stringBuilder.AppendFormat(", IDs {0} thru {1}", (object)this.First.Timestamp.Ticks, (object)this.Last.Timestamp.Ticks);
                else
                    stringBuilder.AppendFormat(", {0} thru {1} sec.", (object)this.First.Timestamp.ToStringMillisec(), (object)this.Last.Timestamp.ToStringMillisec());
            }
            return stringBuilder.ToString();
        }

        [Conditional("TRACE_BUFFER")]
        public static void TRACE1(string fmt, params object[] args)
        {
        }

        [Conditional("TRACE_BUFFER")]
        public static void TRACE(string fmt, params object[] args)
        {
        }

        [Conditional("TRACE_ADD_TIMESTAMPED")]
        public static void TraceAddTimestamped(string fmt, params object[] args)
        {
        }
    }
}
