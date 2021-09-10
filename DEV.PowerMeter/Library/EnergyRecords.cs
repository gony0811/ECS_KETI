using SharedLibrary;
using System;
using System.Collections;
using System.Collections.Generic;

namespace DEV.PowerMeter.Library
{
    [API(APICategory.Measurement, "This object maintains a sequence of data records, all of which are energy samples. Only used for LPEM mode, as a sub-component of CaptureBuffer.")]
    public class EnergyRecords : IEnumerable, IEnumerable<DataRecordSingle>
    {
        [API("List of DataRecords")]
        public List<DataRecordSingle> Data { get; protected set; }

        [API("Count of DataRecords; read-only synonym for Data.Count.")]
        public int Count => this.Data.Count;

        [API("Ordinary Constructor.")]
        public EnergyRecords() => this.Clear();

        [API("Clear the buffer.")]
        public void Clear() => this.Data = new List<DataRecordSingle>();

        [API("Load data from any suitable Enumerable source.")]
        public void Load(IEnumerable<DataRecordSingle> data)
        {
            this.Clear();
            foreach (DataRecordSingle dataRecordSingle in data)
                this.Add(dataRecordSingle);
        }

        [API("Random access to individual records by numeric index.")]
        public DataRecordSingle this[int index]
        {
            get => this.Data[index];
            set => this.Data[index] = value;
        }

        [API("Oldest record in the buffer.")]
        public DataRecordSingle Oldest => this[0];

        [API("Newest record in the buffer.")]
        public DataRecordSingle Newest => this[this.Count - 1];

        [API("Add an individual DataRecord.")]
        public void Add(DataRecordSingle item) => this.Data.Add(item);

        [API("Remove all DataRecord older than the given timestamp.")]
        public void Remove(DateTime oldest)
        {
            while (this.Oldest.Timestamp < oldest)
                this.Data.RemoveAt(0);
        }

        IEnumerator IEnumerable.GetEnumerator() => (IEnumerator)this.GetEnumerator();

        public IEnumerator<DataRecordSingle> GetEnumerator() => (IEnumerator<DataRecordSingle>)this.Data.GetEnumerator();
    }
}
