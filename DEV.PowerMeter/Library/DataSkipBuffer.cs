using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DEV.PowerMeter.Library
{
    public class DataSkipBuffer
    {
        protected MemoryStream Data;
        public readonly byte[] OK = new byte[4]
        {
      (byte) 79,
      (byte) 75,
      (byte) 13,
      (byte) 10
        };
        public const int BytesPerRow = 10;
        public static readonly Dictionary<char, string> MapChars = new Dictionary<char, string>()
    {
      {
        '\r',
        "\\r"
      },
      {
        '\n',
        "\\n"
      },
      {
        'O',
        "O"
      },
      {
        'K',
        "K"
      }
    };

        public DataSkipBuffer() => this.Data = new MemoryStream();

        public int Count => (int)this.Data.Length;

        public void Add(byte[] data, int count)
        {
            if (count <= 0)
                return;
            this.Data.Write(data, 0, count);
        }

        public bool EndsWith(byte[] pattern)
        {
            int length = pattern.Length;
            if (this.Data.Length >= (long)length)
            {
                byte[] buffer = new byte[length];
                this.Data.Seek((long)-length, SeekOrigin.End);
                if (this.Data.Read(buffer, 0, length) == length)
                {
                    for (int index = 0; index < length; ++index)
                    {
                        if ((int)buffer[index] != (int)pattern[index])
                            return false;
                    }
                    return true;
                }
            }
            return false;
        }

        public bool EndsWithOK() => this.EndsWith(this.OK);

        public byte[] ToArray() => this.Data.ToArray();

        public override string ToString()
        {
            byte[] array = this.ToArray();
            int length = array.Length;
            StringBuilder stringBuilder1 = new StringBuilder();
            stringBuilder1.AppendFormat("UnusedData: {0} bytes\n", (object)this.Count);
            for (int index1 = 0; index1 < length; index1 += 10)
            {
                StringBuilder stringBuilder2 = new StringBuilder();
                StringBuilder stringBuilder3 = new StringBuilder();
                int num1 = Math.Min(10, length - index1);
                for (int index2 = 0; index2 < num1; ++index2)
                {
                    int index3 = index1 + index2;
                    int num2 = (int)array[index3];
                    char key = Convert.ToChar(num2);
                    stringBuilder2.AppendFormat("{0:x2} ", (object)num2);
                    if (index2 == 3 || index2 == 5)
                        stringBuilder2.Append(" ");
                    if (DataSkipBuffer.MapChars.ContainsKey(key))
                        stringBuilder3.AppendFormat("{0}", (object)DataSkipBuffer.MapChars[key]);
                    else
                        stringBuilder3.AppendFormat(".");
                }
                stringBuilder1.AppendFormat("UnusedData: {0} {1}\n", (object)stringBuilder2.ToString(), (object)stringBuilder3.ToString());
            }
            return stringBuilder1.ToString();
        }

        public string ToSeparatedValues(string seperator)
        {
            byte[] array = this.ToArray();
            int length = array.Length;
            StringBuilder stringBuilder1 = new StringBuilder();
            string format = "Skipped" + stringBuilder1.Insert(0, seperator, 5).ToString() + "\"{0}\"" + seperator + "\"{1}\"\n";
            stringBuilder1.Clear();
            stringBuilder1.AppendFormat("\n");
            for (int index1 = 0; index1 < length; index1 += 10)
            {
                StringBuilder stringBuilder2 = new StringBuilder();
                StringBuilder stringBuilder3 = new StringBuilder();
                int num1 = Math.Min(10, length - index1);
                for (int index2 = 0; index2 < num1; ++index2)
                {
                    int index3 = index1 + index2;
                    int num2 = (int)array[index3];
                    char key = Convert.ToChar(num2);
                    stringBuilder2.AppendFormat("{0:x2}", (object)num2);
                    if (index2 == 3 || index2 == 5)
                        stringBuilder2.Append(" ");
                    if (DataSkipBuffer.MapChars.ContainsKey(key))
                        stringBuilder3.AppendFormat("{0}", (object)DataSkipBuffer.MapChars[key]);
                    else
                        stringBuilder3.AppendFormat(".");
                }
                stringBuilder1.AppendFormat(format, (object)stringBuilder2.ToString(), (object)stringBuilder3.ToString());
            }
            return stringBuilder1.ToString();
        }
    }
}
