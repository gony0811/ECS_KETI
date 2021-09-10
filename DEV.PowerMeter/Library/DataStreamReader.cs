
using System;
using System.Diagnostics;
using System.IO;

namespace DEV.PowerMeter.Library
{
    public class DataStreamReader : BinaryReader
    {
        public Stopwatch Elapsed { get; protected set; }

        public int IntervalMax { get; protected set; }

        public TimeSpan ElapsedActual => this.Elapsed.Elapsed;

        public TimeSpan ElapsedSimulated => new TimeSpan((long)this.IntervalMax * 10000L);

        public string Filename { get; protected set; }

        public DataStreamItem Header { get; protected set; }

        public string Remark { get; protected set; }

        public string Settings { get; protected set; }

        public event Action<DataStreamItem> UnexpectedRecordEncountered;

        protected void OnUnexpectedRecordEncountered(DataStreamItem command)
        {
            Action<DataStreamItem> recordEncountered = this.UnexpectedRecordEncountered;
            if (recordEncountered == null)
                return;
            recordEncountered(command);
        }

        public long Position => this.BaseStream.Position;

        public long Length => this.BaseStream.Length;

        public bool AtEOF => this.Position >= this.Length;

        public void Seek(long position) => this.BaseStream.Seek(position, SeekOrigin.Begin);

        public DataStreamReader(Stream stream)
          : base(stream)
        {
            this.Header = this.ReadRecord();
            this.Elapsed = Stopwatch.StartNew();
            this.IntervalMax = 0;
        }

        public DataStreamReader(string filename)
          : this((Stream)new FileStream(filename, FileMode.Open))
        {
            this.Filename = filename;
        }

        public void Initialize()
        {
            this.Remark = this.ReadRemark();
            this.Settings = this.ReadSettings();
            this.SkipPastInit();
        }

        public void SkipToFirstData()
        {
            this.Remark = this.ReadRemark();
            this.Settings = this.ReadSettings();
            int num = 0;
            long position;
            DataStreamItem dataStreamItem;
            do
            {
                position = this.Position;
                dataStreamItem = this.ReadRecord();
                ++num;
            }
            while (dataStreamItem.ItemType != DataStreamItemType.Data);
            this.Seek(position);
            this.TraceSkipping(string.Format("Skipped {0} Non-Data records", (object)(num - 1)));
        }

        public void SkipPastInit()
        {
            int num = 0;
            DataStreamItem dataStreamItem;
            do
            {
                dataStreamItem = this.ReadRecord();
                ++num;
            }
            while (!dataStreamItem.IsInitOrStartCommand);
            this.TraceSkipping(string.Format("Skipped {0} Non-Data records", (object)num));
        }

        public string ReadNextRemark()
        {
            while (!this.AtEOF)
            {
                DataStreamItem dataStreamItem = this.ReadRecord();
                if (dataStreamItem.ItemType == DataStreamItemType.Remark)
                    return dataStreamItem.DataAsString;
            }
            return (string)null;
        }

        public DataStreamItem ReadRecord()
        {
            DataStreamItem dataStreamItem = new DataStreamItem();
            dataStreamItem.Read((BinaryReader)this);
            this.IntervalMax = Math.Max(this.IntervalMax, dataStreamItem.Interval);
            return dataStreamItem;
        }

        public DataStreamItem ReadRecord(DataStreamItemType type)
        {
            DataStreamItem dataStreamItem;
            for (dataStreamItem = this.ReadRecord(); dataStreamItem.ItemType != type; dataStreamItem = this.ReadRecord())
                this.TraceSkipping(string.Format("Searching for type {0} Skipping: {1}", (object)type, (object)dataStreamItem));
            return dataStreamItem;
        }

        public DataStreamItem ReadDataRecord()
        {
            DataStreamItem command = this.ReadRecord();
            if (command.ItemType == DataStreamItemType.Data)
                return command;
            this.OnUnexpectedRecordEncountered(command);
            throw new TimeoutException();
        }

        protected string ReadString(DataStreamItemType type) => this.ReadRecord(type).DataAsString;

        public string ReadRemark() => this.ReadString(DataStreamItemType.Remark);

        public string ReadSettings() => this.ReadString(DataStreamItemType.Settings);

        public string ReadCommand() => this.ReadString(DataStreamItemType.Command);

        public string ReadData() => this.ReadDataRecord().DataAsString;

        public void ReadDataReset()
        {
        }

        public void WriteCommand(string command)
        {
            for (string str = this.ReadString(DataStreamItemType.Command); str != command; str = this.ReadString(DataStreamItemType.Command))
                this.TraceSkipping("DataStreamReader: skipping out of sequence command: expected " + str + " got " + command);
        }

        public int ReadData(byte[] data, int offset, int count)
        {
            DataStreamItem dataStreamItem = this.ReadDataRecord();
            Array.Copy((Array)dataStreamItem.Data, 0, (Array)data, offset, dataStreamItem.Length);
            return dataStreamItem.Length;
        }

        public void Trace(string message)
        {
        }

        [Conditional("TRACE_SKIPPING")]
        public void TraceSkipping(string message)
        {
        }
    }
}
