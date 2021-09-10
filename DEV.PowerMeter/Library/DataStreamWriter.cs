using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace DEV.PowerMeter.Library
{
    public class DataStreamWriter : BinaryWriter
    {
        public Encoding UTF8 = Encoding.UTF8;

        public Stopwatch Stopwatch { get; protected set; }

        public string Filename { get; protected set; }

        public DataStreamWriter(Stream stream)
          : base(stream)
        {
            this.Stopwatch = Stopwatch.StartNew();
        }

        public DataStreamWriter(string filename)
          : this((Stream)new FileStream(filename, FileMode.Create))
        {
            this.Filename = filename;
            this.Write(new DataStreamItem()
            {
                ItemType = DataStreamItemType.Header,
                Length = 1,
                Data = new byte[1]
            });
        }

        public void Write(DataStreamItem record) => record.Write((BinaryWriter)this);

        public void Write(int interval, DataStreamItemType type, byte[] data) => this.Write(new DataStreamItem(type, interval, data));

        public void Write(DataStreamItemType type, byte[] data) => this.Write((int)this.Stopwatch.ElapsedMilliseconds, type, data);

        public void WriteRemark(string remark) => this.Write(DataStreamItemType.Remark, this.UTF8.GetBytes(remark));

        public void Write(DataStreamItemType type, int offset, byte[] source, int count)
        {
            byte[] data = new byte[count];
            Array.Copy((Array)source, offset, (Array)data, 0, count);
            this.Write(type, data);
        }

        public void Write(DataStreamItemType type, int offset, byte[] source)
        {
            int length = source.Length - offset;
            byte[] data = new byte[length];
            Array.Copy((Array)source, offset, (Array)data, 0, length);
            this.Write(type, data);
        }

        public void WriteSettings(string settings) => this.Write(DataStreamItemType.Settings, this.UTF8.GetBytes(settings));

        public void WriteCommand(string command) => this.Write(DataStreamItemType.Command, this.UTF8.GetBytes(command));

        public void WriteData(string data) => this.Write(DataStreamItemType.Data, this.UTF8.GetBytes(data));

        public void WriteData(byte[] data, int offset, int count) => this.Write(DataStreamItemType.Data, offset, data, count);
    }
}
