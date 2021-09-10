using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace DEV.PowerMeter.Library
{
    public class DataStreamItem
    {
        public const string DefaultExtension = ".dat";
        public Encoding UTF8 = Encoding.UTF8;

        public DataStreamItemType ItemType { get; set; }

        public int Interval { get; set; }

        public int Length { get; set; }

        public byte[] Data { get; set; }

        public string DataAsString
        {
            get => this.UTF8.GetString(this.Data);
            set
            {
                this.Data = this.UTF8.GetBytes(value);
                this.Length = this.Data.Length;
            }
        }

        public bool IsAbortOrStopCommand
        {
            get
            {
                if (this.ItemType != DataStreamItemType.Command)
                    return false;
                string upper = this.DataAsString.ToUpper();
                return upper == "ABORT" || upper == "STOP";
            }
        }

        public bool IsInitOrStartCommand
        {
            get
            {
                if (this.ItemType != DataStreamItemType.Command)
                    return false;
                string upper = this.DataAsString.ToUpper();
                return upper.StartsWith("INIT") || upper.StartsWith("STAR");
            }
        }

        public DataStreamItem()
        {
        }

        public DataStreamItem(BinaryReader reader) => this.Read(reader);

        public DataStreamItem(DataStreamItemType type, int interval, byte[] data)
        {
            this.Interval = interval;
            this.ItemType = type;
            this.Length = data.Length;
            this.Data = data;
        }

        public DataStreamItem(string remark, int interval = 0)
        {
            this.ItemType = DataStreamItemType.Remark;
            byte[] bytes = this.UTF8.GetBytes(remark);
            this.Length = bytes.Length;
            this.Interval = interval;
            this.Data = bytes;
        }

        public void Read(BinaryReader reader)
        {
            this.ItemType = (DataStreamItemType)reader.ReadInt32();
            this.Interval = reader.ReadInt32();
            this.Length = reader.ReadInt32();
            this.Data = reader.ReadBytes(this.Length);
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write((int)this.ItemType);
            writer.Write(this.Interval);
            writer.Write(this.Length);
            writer.Write(this.Data, 0, this.Length);
        }

        public override string ToString() => string.Format("{0}\t{1}\t[{2}]\t", (object)this.Interval, (object)this.ItemType, (object)this.Data.Length) + this.FormatData();

        public string FormatData()
        {
            StringBuilder stringBuilder = new StringBuilder();
            switch (this.ItemType)
            {
                case DataStreamItemType.Header:
                case DataStreamItemType.Data:
                    string str = string.Join(" ", ((IEnumerable<byte>)this.Data).Select<byte, string>((Func<byte, string>)(b => b.ToString("x2"))));
                    stringBuilder.Append(str);
                    stringBuilder.Append("  ");
                    break;
                default:
                    stringBuilder.Append(this.DataAsString);
                    break;
            }
            return stringBuilder.ToString();
        }

        [Conditional("TRACE_ITEM_READS")]
        public void TraceRead()
        {
        }

        [Conditional("TRACE_ITEM_WRITES")]
        public void TraceWrite()
        {
        }
    }
}
