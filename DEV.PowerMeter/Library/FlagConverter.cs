using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace DEV.PowerMeter.Library
{
    public class FlagConverter<T> : EnumConverter<T> where T : IConvertible
    {
        public FlagConverter(Dictionary<string, T> scpiToEnum)
          : base(scpiToEnum)
        {
        }

        protected List<T> ToEnumList(string arg)
        {
            string[] strArray = arg.Split(SCPI.CommaChar);
            List<T> objList = new List<T>();
            foreach (string str in strArray)
                objList.Add(this.FromString(str.Trim()));
            return objList;
        }

        protected string ToStringList(List<T> values) => string.Join(",", values.Select<T, string>((Func<T, string>)(e => this.ToString(e))));

        protected List<T> ToEnumList(uint values)
        {
            List<T> objList = new List<T>();
            foreach (T key in this.EnumToScpi.Keys)
            {
                if (((int)key.ToUInt32((IFormatProvider)CultureInfo.InvariantCulture) & (int)values) != 0)
                    objList.Add(key);
            }
            return objList;
        }

        protected string ToStringList(uint values) => this.ToStringList(this.ToEnumList(values));

        public string ToStringList(T value) => this.ToStringList(value.ToUInt32((IFormatProvider)CultureInfo.InvariantCulture));

        public uint ToBitMask(string value)
        {
            uint num = 0;
            foreach (T obj in this.ToEnumList(value))
                num |= obj.ToUInt32((IFormatProvider)CultureInfo.InvariantCulture);
            return num;
        }
    }
}
