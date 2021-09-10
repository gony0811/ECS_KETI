using DEV.PowerMeter.Library.ImportExport;
using SharedLibrary;
using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace DEV.PowerMeter.Library
{
    [API(APICategory.Measurement, "This class handles data originating from non-Quad sensors.")]
    public class DataRecordSingle : DataRecordBase, IDataRecordSingle, IEncodable
    {
        public const string TimeFormat = "F7";
        public const string XYFormat = "F3";
        public readonly HexGroups HexGroupsSingle = new HexGroups(new int[4]
        {
      4,
      6,
      10,
      14
        });
        public const int MinimumColumns = 4;
        public const int MinimumColumnsLegacy = 2;

        public DateTime Timestamp { get; set; }

        [API("The primary measurement value for this sample. Units are implicit, determined by measurement mode. Data is omitted (field left zero) if DataFieldFlags.Primary is NOT set in SelectedDataFields.")]
        public double Measurement { get; set; }

        [API("Sequence ID for this sample, if any. Data is omitted (field left zero) if DataFieldFlags.Sequence is NOT set in SelectedDataFields.")]
        public ulong Sequence { get; set; }

        [API("MeasurementFlags for this sample, if any. Data is omitted (field left zero) if DataFieldFlags.Flags is NOT set in SelectedDataFields.")]
        public MeasurementFlags Flags { get; set; }

        [API("This property indicates that the meter unilaterally decided it needed to abort the data acquisition sequence. Meter will send no more samples following this one.The most common cause is that the Sensor became disconnected.")]
        public bool Terminated => (uint)(this.Flags & MeasurementFlags.Terminated) > 0U;

        [API("If RetainBinary is true, the bytes received from the meter for this sample will appear here. ")]
        public byte[] DataBytes { get; set; }

        [API("The pulse period (in microseconds) for Energy measurements only. Data is omitted (field left zero) if DataFieldFlags.Period is NOT set in SelectedDataFields.")]
        public uint Period { get; set; }

        public virtual bool Validate() => this.TruncateImpossible(this.Measurement);

        [API("Truncate impossibly huge measurement values to something that does not crash the graphics package. ImpossiblyLargeMeasurement is a static global variable, and defaults to a million. Such huge values usually only happen when there are communication errors, but even then crashing the app is not acceptable.")]
        public bool TruncateImpossible(double value)
        {
            bool flag = true;
            if (Math.Abs(value) > DataRecordBase.ImpossiblyLargeMeasurement)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendFormat("Truncating Measurement: {0} > Max {1}", (object)this.Measurement, (object)DataRecordBase.ImpossiblyLargeMeasurement);
                if (DataRecordBase.RetainBinary && this.DataBytes != null)
                {
                    stringBuilder.Append("\n              ToString: " + this.ToString());
                    stringBuilder.Append("\n                 AsHex: " + this.ToString_AsHex());
                }
                DataRecordBase.ErrorReporter.ReportError("{0}", (object)stringBuilder.ToString());
                this.Measurement = DataRecordBase.ImpossiblyLargeMeasurement * (value < 0.0 ? -1.0 : 1.0);
                flag = false;
            }
            if (double.IsNaN(value))
            {
                DataRecordBase.ErrorReporter.ReportError("Truncating NaN Measurement: {0}", (object)this.Measurement);
                flag = false;
            }
            if (!flag)
                this.Flags |= MeasurementFlags.Impossible;
            return flag;
        }

        [API("Create an instance, initialized to defaults. This parameterless constructor is needed by CircularBuffer. To maximize performance, CircularBuffer pre-allcoates all the DataRecords before starting Data Acquisition.Then, when running, it merely writes the data into the pre-allocated records, without requiring any dynamic storage allcoation.")]
        public DataRecordSingle() => this.Validate();

        [API("Create an instance, initialized to match another instance (a \"copy constructor\"). ")]
        public DataRecordSingle(IDataRecordSingle original) => this.Set(original);

        [API("Initialize an instance to match another instance. ")]
        public virtual void Set(IDataRecordSingle original)
        {
            this.Timestamp = original.Timestamp;
            this.Sequence = original.Sequence;
            this.Measurement = original.Measurement;
            this.DataBytes = original.DataBytes;
            this.Period = original.Period;
            this.Flags = original.Flags;
            this.Validate();
        }

        protected DataRecordSingle(DataFieldFlags selectedFields, string[] fields)
          : this()
        {
            int num1 = 0;
            if ((selectedFields & DataFieldFlags.Primary) != (DataFieldFlags)0)
                this.Measurement = FromDevice.Real(fields[num1++]);
            if ((selectedFields & DataFieldFlags.Flags) != (DataFieldFlags)0)
                this.Flags = (MeasurementFlags)FromDevice.Hex(fields[num1++]);
            if ((selectedFields & DataFieldFlags.Sequence) != (DataFieldFlags)0)
                this.Sequence = FromDevice.Uint64(fields[num1++]);
            if ((selectedFields & DataFieldFlags.Period) != (DataFieldFlags)0)
            {
                string[] strArray = fields;
                int index = num1;
                int num2 = index + 1;
                this.Period = FromDevice.Uint(strArray[index]);
            }
            this.Validate();
        }

        protected DataRecordSingle(DataFieldFlags selectedFields, string record)
          : this(selectedFields, record.Split(','))
        {
        }

        [API("Create an instance, from a CSV string.  Used by Meter for Polling.")]
        public DataRecordSingle(string record)
        {
            string[] strArray1 = record.Split(',');
            int num1 = 0;
            string[] strArray2 = strArray1;
            int index1 = num1;
            int num2 = index1 + 1;
            this.Measurement = FromDevice.Real(strArray2[index1]);
            if (strArray1.Length >= 3 && DataRecordSingle.IsReal(strArray1[1]) && DataRecordSingle.IsReal(strArray1[2]))
                num2 = 3;
            this.Flags = (DataRecordBase.SelectedDataFields & DataFieldFlags.Flags) == (DataFieldFlags)0 || num2 >= strArray1.Length ? (MeasurementFlags)0 : (MeasurementFlags)FromDevice.Hex(strArray1[num2++]);
            this.Sequence = (DataRecordBase.SelectedDataFields & DataFieldFlags.Sequence) == (DataFieldFlags)0 || num2 >= strArray1.Length ? 0UL : (ulong)FromDevice.Uint(strArray1[num2++]);
            if ((DataRecordBase.SelectedDataFields & DataFieldFlags.Period) != (DataFieldFlags)0 && num2 < strArray1.Length)
            {
                string[] strArray3 = strArray1;
                int index2 = num2;
                int num3 = index2 + 1;
                this.Period = FromDevice.Uint(strArray3[index2]);
            }
            else
                this.Period = 0U;
        }

        public static bool IsReal(string s) => s.Contains(".");

        public DataRecordSingle(byte[] bytes)
          : this(DataRecordBase.SelectedDataFields, bytes, 0)
        {
            this.Validate();
        }

        public DataRecordSingle(DataFieldFlags dre, byte[] bytes)
          : this(dre, bytes, 0)
        {
            this.Validate();
        }

        public DataRecordSingle(DataFieldFlags dre, byte[] bytes, int index)
          : this()
        {
            this.Read(dre, bytes, ref index);
            this.Validate();
        }

        public override string ToString() => string.Format("{0} {1} {2} {3} {4}", (object)this.Timestamp.ToStringMicrosec(), (object)this.Measurement, (object)this.Sequence, (object)this.Flags.ToString().Replace(",", "|"), (object)this.Period);

        public string ToStringEx() => string.Format("{0} {1} {2} 0x{3} {4} // {5}", (object)this.Timestamp.ToStringMicrosec(), (object)this.Measurement, (object)this.Sequence, (object)((int)this.Flags).ToString("X"), (object)this.Period, (object)this.Flags.ToString().Replace(",", "|"));

        public virtual string ToString_AsHex() => this.HexGroupsSingle.ToString(this.DataBytes);

        public virtual string[] Encode(CultureInfo culture)
        {
            int num = !DataRecordBase.RetainBinary ? 0 : (this.DataBytes != null ? 1 : 0);
            string[] strArray = new string[num != 0 ? 7 : 6];
            strArray[0] = DataRecordSingle.EncodeDateTime(this.Timestamp, culture);
            strArray[1] = DataRecordSingle.EncodeMeasurement(this.Measurement, culture);
            strArray[2] = ToDevice.Uint64(this.Sequence);
            strArray[3] = ToDevice.Uint((uint)this.Flags);
            strArray[4] = this.Flags.ToString().Replace(",", "|");
            strArray[5] = ToDevice.Uint(this.Period);
            if (num != 0)
                strArray[6] = this.HexGroupsSingle.ToString(this.DataBytes);
            return strArray;
        }

        public static string EncodeDateTime(DateTime DateTime, CultureInfo culture) => DateTime.TotalSeconds().ToString("F7", (IFormatProvider)culture);

        public static string EncodeMeasurement(double Measurement, CultureInfo culture) => Measurement.ToString((IFormatProvider)culture);

        public virtual void Decode(string[] fields, CultureInfo culture)
        {
            DataRecordSingle.ValidateFieldsMinimum(fields.Length, 4);
            this.Timestamp = DataRecordSingle.DecodeDateTime(fields[0], culture);
            this.Measurement = DataRecordSingle.DecodeReal(fields[1], culture);
            this.Sequence = DataRecordSingle.DecodeULong(fields[2], culture);
            this.Flags = (MeasurementFlags)DataRecordSingle.DecodeUInt(fields[3], culture);
            if (fields.Length < 6)
                return;
            this.Period = FromDevice.Uint(fields[5]);
        }

        public static DateTime DecodeDateTime(string field, CultureInfo culture) => DateTimeExtensions.FromSeconds(DataRecordSingle.DecodeReal(field, culture));

        public void DecodeLegacy(string[] fields, CultureInfo culture)
        {
            DataRecordSingle.ValidateFieldsMinimum(fields.Length, 2);
            this.Measurement = DataRecordSingle.DecodeReal(fields[0], culture);
            this.Sequence = DataRecordSingle.DecodeULong(fields[1], culture);
        }

        public static void ValidateFieldsMinimum(int actual, int minimum)
        {
            if (actual < minimum)
                throw new DecodeException(string.Format("Data Record has {0} columns, needs minimum of {1}", (object)actual, (object)minimum));
        }

        public static double DecodeReal(string s, CultureInfo culture)
        {
            double result = 0.0;
            if (s.IsNullOrEmpty() || !double.TryParse(s, NumberStyles.Float, (IFormatProvider)culture, out result))
                throw new DecodeException(string.Format("RealDecoder: Unable to decode real number: \"{0}\"", (object)s));
            return result;
        }

        public static ulong DecodeULong(string s, CultureInfo culture)
        {
            ulong result = 0;
            if (s.IsNullOrEmpty() || !ulong.TryParse(s, NumberStyles.Float, (IFormatProvider)culture, out result))
                throw new DecodeException(string.Format("Unable to decode ulong number: \"{0}\"", (object)s));
            return result;
        }

        public static uint DecodeUInt(string s, CultureInfo culture)
        {
            uint result = 0;
            if (s.IsNullOrEmpty() || !uint.TryParse(s, NumberStyles.Float, (IFormatProvider)culture, out result))
                throw new DecodeException(string.Format("Unable to decode uint number: \"{0}\"", (object)s));
            return result;
        }

        public static int BinaryRecordLength() => DataRecordSingle.BinaryRecordLength(DataRecordBase.SelectedDataFields);

        public static int BinaryRecordLength(DataFieldFlags selectedFields) => ((selectedFields & DataFieldFlags.Primary) != (DataFieldFlags)0 ? 4 : 0) + ((selectedFields & DataFieldFlags.Flags) != (DataFieldFlags)0 ? 2 : 0) + ((selectedFields & DataFieldFlags.Sequence) != (DataFieldFlags)0 ? 4 : 0) + ((selectedFields & DataFieldFlags.Period) != (DataFieldFlags)0 ? 4 : 0);

        public void Read(byte[] bytes)
        {
            int index = 0;
            this.Read(DataRecordBase.SelectedDataFields, bytes, ref index);
        }

        protected void Read(DataFieldFlags selectedFields, byte[] bytes)
        {
            int index = 0;
            this.Read(selectedFields, bytes, ref index);
        }

        protected void Read(DataFieldFlags selectedFields, byte[] bytes, ref int index)
        {
            this.ReadBytes(selectedFields, bytes, ref index);
            if (!DataRecordBase.RetainBinary)
                return;
            this.DataBytes = (byte[])bytes.Clone();
        }

        protected virtual void ReadBytes(DataFieldFlags selectedFields, byte[] bytes, ref int index)
        {
            this.ReadPrimary(selectedFields, bytes, ref index);
            this.ReadRemainder(selectedFields, bytes, ref index);
        }

        protected void ReadPrimary(DataFieldFlags selectedFields, byte[] bytes, ref int index)
        {
            if ((selectedFields & DataFieldFlags.Primary) == (DataFieldFlags)0)
                return;
            this.Measurement = this.ReadFloat(bytes, ref index);
        }

        protected void ReadRemainder(DataFieldFlags selectedFields, byte[] bytes, ref int index)
        {
            if ((selectedFields & DataFieldFlags.Flags) != (DataFieldFlags)0)
                this.Flags = (MeasurementFlags)this.ReadFlags(bytes, ref index);
            if ((selectedFields & DataFieldFlags.Sequence) != (DataFieldFlags)0)
            {
                this.Sequence = this.ReadSequence(bytes, ref index);
                this.Timestamp = Timestamper.TimestampFromSequence(this.Sequence);
            }
            if ((selectedFields & DataFieldFlags.Period) != (DataFieldFlags)0)
                this.Period = this.ReadUInt32(bytes, ref index);
            this.Validate();
        }

        protected double ReadFloat(byte[] bytes, ref int index)
        {
            double single = (double)BitConverter.ToSingle(bytes, index);
            index += 4;
            return single;
        }

        protected uint ReadFlags(byte[] bytes, ref int index)
        {
            int uint16 = (int)BitConverter.ToUInt16(bytes, index);
            index += 2;
            return (uint)uint16;
        }

        protected ulong ReadSequence(byte[] bytes, ref int index) => (ulong)this.ReadUInt32(bytes, ref index);

        protected uint ReadUInt32(byte[] bytes, ref int index)
        {
            int uint32 = (int)BitConverter.ToUInt32(bytes, index);
            index += 4;
            return (uint)uint32;
        }

        protected ulong ReadUInt64(byte[] bytes, ref int index)
        {
            long uint64 = (long)BitConverter.ToUInt64(bytes, index);
            index += 8;
            return (ulong)uint64;
        }

        public virtual void Write(BinaryWriter writer, DataFieldFlags selectedFields)
        {
            this.WritePrimary(writer, selectedFields);
            this.WriteRemainder(writer, selectedFields);
        }

        public void WritePrimary(BinaryWriter writer, DataFieldFlags selectedFields)
        {
            if ((selectedFields & DataFieldFlags.Primary) == (DataFieldFlags)0)
                return;
            writer.Write((float)this.Measurement);
        }

        public void WriteRemainder(BinaryWriter writer, DataFieldFlags selectedFields)
        {
            if ((selectedFields & DataFieldFlags.Flags) != (DataFieldFlags)0)
                writer.Write((ushort)this.Flags);
            if ((selectedFields & DataFieldFlags.Sequence) != (DataFieldFlags)0)
                writer.Write(this.Sequence);
            if ((selectedFields & DataFieldFlags.Period) == (DataFieldFlags)0)
                return;
            writer.Write(this.Period);
        }

        [API("Deep Copy an array of samples, so new array and original one refer to different data samples.")]
        public static DataRecordSingle[] DeepCopy(DataRecordSingle[] array)
        {
            DataRecordSingle[] dataRecordSingleArray = (DataRecordSingle[])array.Clone();
            for (int index = 0; index < dataRecordSingleArray.Length; ++index)
                dataRecordSingleArray[index] = new DataRecordSingle((IDataRecordSingle)dataRecordSingleArray[index]);
            return dataRecordSingleArray;
        }
    }
}
