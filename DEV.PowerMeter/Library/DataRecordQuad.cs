
using SharedLibrary;
using System;
using System.Globalization;
using System.IO;

namespace DEV.PowerMeter.Library
{
    [API(APICategory.Measurement, "This class handles data originating from Quad sensors. Mainly it includes X and Y offsets (in mm) of the beam position.")]
    public class DataRecordQuad : DataRecordSingle, IDataRecordQuad, IDataRecordSingle, IEncodable
    {
        public readonly HexGroups HexGroupsQuad = new HexGroups(new int[6]
        {
      4,
      8,
      12,
      14,
      18,
      22
        });
        public const int MinimumColumnsQuad = 2;

        [API("X offset (in mm) of the beam position from the center of the sensor.")]
        public double X { get; set; }

        [API("Y offset (in mm) of the beam position from the center of the sensor.")]
        public double Y { get; set; }

        public override bool Validate() => base.Validate() & this.TruncateImpossible(this.X) & this.TruncateImpossible(this.Y);

        [API("Create an instance, initialized to defaults. ")]
        public DataRecordQuad()
        {
        }

        [API("Create an instance, from a CSV string.  Used by Meter for Polling.")]
        public DataRecordQuad(string record)
        {
            string[] strArray1 = record.Split(',');
            int num1 = 0;
            string[] strArray2 = strArray1;
            int index1 = num1;
            int num2 = index1 + 1;
            this.Measurement = FromDevice.Real(strArray2[index1]);
            if (strArray1.Length >= 3 && (DataRecordBase.SelectedDataFields & DataFieldFlags.Quad) != (DataFieldFlags)0)
            {
                string[] strArray3 = strArray1;
                int index2 = num2;
                int num3 = index2 + 1;
                this.X = FromDevice.Real(strArray3[index2]);
                string[] strArray4 = strArray1;
                int index3 = num3;
                num2 = index3 + 1;
                this.Y = FromDevice.Real(strArray4[index3]);
            }
            if ((DataRecordBase.SelectedDataFields & DataFieldFlags.Flags) != (DataFieldFlags)0 && num2 < strArray1.Length)
                this.Flags = (MeasurementFlags)FromDevice.Hex(strArray1[num2++]);
            else
                this.Flags = (MeasurementFlags)0;
            if ((DataRecordBase.SelectedDataFields & DataFieldFlags.Sequence) != (DataFieldFlags)0 && num2 < strArray1.Length)
                this.Sequence = (ulong)FromDevice.Uint(strArray1[num2++]);
            else
                this.Sequence = 0UL;
            if ((DataRecordBase.SelectedDataFields & DataFieldFlags.Period) != (DataFieldFlags)0 && num2 < strArray1.Length)
            {
                string[] strArray5 = strArray1;
                int index4 = num2;
                int num4 = index4 + 1;
                this.Period = FromDevice.Uint(strArray5[index4]);
            }
            else
                this.Period = 0U;
        }

        [API("Create an instance, initialized to match another instance (a \"copy constructor\"). ")]
        public DataRecordQuad(IDataRecordQuad original) => this.Set((IDataRecordSingle)original);

        [API("Initialize an instance to match another instance. ")]
        public override void Set(IDataRecordSingle original)
        {
            base.Set(original);
            if (original is IDataRecordQuad dataRecordQuad)
            {
                this.X = dataRecordQuad.X;
                this.Y = dataRecordQuad.Y;
            }
            else
                this.X = this.Y = 0.0;
            this.Validate();
        }

        protected DataRecordQuad(DataFieldFlags dre, string[] fields)
          : this()
        {
            int num1 = (int)dre;
            int num2 = 0;
            if ((num1 & 1) != 0)
            {
                string[] strArray1 = fields;
                int index1 = num2;
                int num3 = index1 + 1;
                this.Measurement = FromDevice.Real(strArray1[index1]);
                string[] strArray2 = fields;
                int index2 = num3;
                int num4 = index2 + 1;
                this.X = FromDevice.Real(strArray2[index2]);
                string[] strArray3 = fields;
                int index3 = num4;
                num2 = index3 + 1;
                this.Y = FromDevice.Real(strArray3[index3]);
            }
            if ((num1 & 4) != 0)
                this.Flags = (MeasurementFlags)FromDevice.Uint(fields[num2++]);
            if ((num1 & 8) == 0)
                return;
            string[] strArray = fields;
            int index = num2;
            int num5 = index + 1;
            this.Sequence = FromDevice.Uint64(strArray[index]);
        }

        protected DataRecordQuad(DataFieldFlags dre, string record)
          : this(dre, record.Split(','))
        {
        }

        public DataRecordQuad(DataFieldFlags dre, byte[] bytes)
          : this(dre, bytes, 0)
        {
        }

        public DataRecordQuad(DataFieldFlags dre, byte[] bytes, int index)
          : this()
        {
            this.Read(dre, bytes, ref index);
        }

        public override string ToString() => string.Format("{0} {1} {2} {3} {4} {5}", (object)this.Timestamp.ToStringMillisec(), (object)this.Measurement, (object)this.X, (object)this.Y, (object)this.Sequence, (object)this.Flags.ToString().Replace(",", "|"));

        public override string ToString_AsHex() => this.HexGroupsQuad.ToString(this.DataBytes);

        public override string[] Encode(CultureInfo culture)
        {
            int num1 = !DataRecordBase.RetainBinary ? 0 : (this.DataBytes != null ? 1 : 0);
            string[] strArray1 = new string[num1 != 0 ? 9 : 8];
            strArray1[0] = DataRecordSingle.EncodeDateTime(this.Timestamp, culture);
            strArray1[1] = DataRecordSingle.EncodeMeasurement(this.Measurement, culture);
            string[] strArray2 = strArray1;
            double num2 = this.X;
            string str1 = num2.ToString("F3", (IFormatProvider)culture);
            strArray2[2] = str1;
            string[] strArray3 = strArray1;
            num2 = this.Y;
            string str2 = num2.ToString("F3", (IFormatProvider)culture);
            strArray3[3] = str2;
            strArray1[4] = ToDevice.Uint64(this.Sequence);
            strArray1[5] = ToDevice.Uint((uint)this.Flags);
            strArray1[6] = this.Flags.ToString().Replace(",", "|");
            strArray1[7] = ToDevice.Uint(this.Period);
            if (num1 != 0)
                strArray1[8] = this.HexGroupsQuad.ToString(this.DataBytes);
            return strArray1;
        }

        public override void Decode(string[] fields, CultureInfo culture)
        {
            DataRecordSingle.ValidateFieldsMinimum(fields.Length, 2);
            this.Timestamp = DateTimeExtensions.FromSeconds(DataRecordSingle.DecodeReal(fields[0], culture));
            this.Measurement = DataRecordSingle.DecodeReal(fields[1], culture);
            this.X = DataRecordSingle.DecodeReal(fields[2], culture);
            this.Y = DataRecordSingle.DecodeReal(fields[3], culture);
            this.Sequence = DataRecordSingle.DecodeULong(fields[4], culture);
            this.Flags = (MeasurementFlags)DataRecordSingle.DecodeUInt(fields[5], culture);
            if (fields.Length < 8)
                return;
            this.Period = FromDevice.Uint(fields[7]);
        }

        public new static int BinaryRecordLength() => DataRecordQuad.BinaryRecordLength(DataRecordBase.SelectedDataFields);

        public new static int BinaryRecordLength(DataFieldFlags selectedFields) => DataRecordSingle.BinaryRecordLength(selectedFields) + ((selectedFields & DataFieldFlags.Primary) != (DataFieldFlags)0 ? 8 : 0);

        protected override void ReadBytes(DataFieldFlags selectedFields, byte[] bytes, ref int index)
        {
            this.ReadPrimary(selectedFields, bytes, ref index);
            if ((selectedFields & DataFieldFlags.Primary) != (DataFieldFlags)0)
            {
                this.X = this.ReadFloat(bytes, ref index);
                this.Y = this.ReadFloat(bytes, ref index);
            }
            this.ReadRemainder(selectedFields, bytes, ref index);
        }

        public override void Write(BinaryWriter writer, DataFieldFlags selectedFields)
        {
            this.WritePrimary(writer, selectedFields);
            if ((selectedFields & DataFieldFlags.Primary) != (DataFieldFlags)0)
            {
                writer.Write(this.X);
                writer.Write(this.Y);
            }
            this.WriteRemainder(writer, selectedFields);
        }
    }
}
