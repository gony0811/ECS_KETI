using DEV.PowerMeter.Library.ImportExport;
using System;
using System.Collections;
using System.Collections.Generic;

namespace DEV.PowerMeter.Library
{
    public interface ICaptureBuffer : IEnumerable<DataRecordSingle>, IEnumerable, IIsQuadOrPyro
    {
        Header Header { get; set; }

        DataRecordSingle this[DateTime dateTime] { get; }

        DataRecordSingle this[int index] { get; set; }

        DataRecordSingle First { get; }

        DataRecordSingle Last { get; }

        IEnumerable<DataRecordSingle> Slice(BufferBounds bounds);

        IEnumerable<DataRecordSingle> Slice(DateTime first, DateTime last);

        void Load(IEnumerable<DataRecordSingle> data);

        void TimestampAndAdd(DataRecordSingle item);

        void AddTimestampedItem(DataRecordSingle item);

        void AddEnergyRecord(DataRecordSingle item);

        EnergyRecords EnergyRecords { get; }

        IEnumerable<DataRecordSingle> PowerAndEnergy { get; }

        Units Units { get; }

        uint Count { get; }

        void Clear();

        int TriggerCount { get; }

        bool HasUnsavedData { get; }

        string FileBasename { get; }

        DataSkipBuffer SkipBuffer { get; set; }
    }
}
