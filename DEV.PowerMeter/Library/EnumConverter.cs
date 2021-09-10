using SharedLibrary;
using System;
using System.Collections.Generic;
using System.Linq;


namespace DEV.PowerMeter.Library
{
    public class EnumConverter<T> : IEnumConverter<T> where T : IConvertible
    {
        public Dictionary<string, T> ScpiToEnum = new Dictionary<string, T>((IEqualityComparer<string>)StringComparer.InvariantCultureIgnoreCase);
        public Dictionary<T, string> EnumToScpi = new Dictionary<T, string>();

        public IErrorReporter ErrorReporter;

        public Type SubType => typeof(T);

        public string SubTypeName => this.SubType.ToString();

        public string[] Keys => this.ScpiToEnum.Keys.ToArray<string>();

        public EnumConverter(Dictionary<string, T> scpiToEnum)
        {
            foreach (KeyValuePair<string, T> keyValuePair in scpiToEnum)
                this.Add(keyValuePair.Key, keyValuePair.Value);
        }

        protected void Add(string s, T e)
        {
            this.ScpiToEnum.Add(s, e);
            this.EnumToScpi.Add(e, s);
        }

        public T FromString(string s)
        {
            if (!this.ScpiToEnum.ContainsKey(s))
                this.ReportError(s);
            return this.ScpiToEnum[s];
        }

        public T FromString(string s, T ifMissing) => !this.ScpiToEnum.ContainsKey(s) ? ifMissing : this.ScpiToEnum[s];

        public string ToString(T e) => this.EnumToScpi[e];

        private void ReportError(string key) => this.ErrorReporter.ReportError("ScpiToEnum conversion error: Key '{0}' not valid for {1}", (object)key, (object)this.SubTypeName);
    }
}
