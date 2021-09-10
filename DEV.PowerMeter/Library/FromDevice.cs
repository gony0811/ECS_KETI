using SharedLibrary;
using System;
using System.Globalization;

namespace DEV.PowerMeter.Library
{
    public static class FromDevice
    {
        public static double Default_NA;

        public static IErrorReporter ErrorReporter;

        public static bool Bool(string s) => !string.IsNullOrEmpty(s) && s.IsOn();

        public static uint Hex(string s)
        {
            if (s.StartsWith("0x"))
                s = s.Substring(2);
            uint result = 0;
            if (!s.IsErr() && (string.IsNullOrEmpty(s) || !uint.TryParse(s, NumberStyles.HexNumber, (IFormatProvider)CultureInfo.InvariantCulture, out result)))
                FromDevice.ErrorReporter.ReportError("Unable to decode hex number: \"{0}\"", (object)s);
            return result;
        }

        public static int Int(string s)
        {
            int result = 0;
            if (!s.IsErr() && (string.IsNullOrEmpty(s) || !int.TryParse(s, out result)))
                FromDevice.ErrorReporter.ReportError("Unable to decode integer: \"{0}\"", (object)s);
            return result;
        }

        public static uint Uint(string s, uint Default = 0)
        {
            uint result = Default;
            if (!s.IsErr() && (string.IsNullOrEmpty(s) || !uint.TryParse(s, out result)))
                FromDevice.ErrorReporter.ReportError("Unable to decode uint: \"{0}\"", (object)s);
            return result;
        }

        public static ulong Uint64(string s, uint Default = 0)
        {
            ulong result = (ulong)Default;
            if (!s.IsErr() && (string.IsNullOrEmpty(s) || !ulong.TryParse(s, out result)))
                FromDevice.ErrorReporter.ReportError("Unable to decode ulong: \"{0}\"", (object)s);
            return result;
        }

        public static ushort Uint16(string s)
        {
            ushort result = 0;
            if (!s.IsErr() && (string.IsNullOrEmpty(s) || !ushort.TryParse(s, out result)))
                FromDevice.ErrorReporter.ReportError("Unable to decode integer: \"{0}\"", (object)s);
            return result;
        }

        public static double Real(string s)
        {
            double num = 0.0;
            FromDevice.Real(s, out num);
            return num;
        }

        public static bool Real(string s, out double value)
        {
            value = 0.0;
            if (s.IsErr())
                return false;
            bool flag = !s.IsNullOrEmpty() && double.TryParse(s, NumberStyles.Float, (IFormatProvider)CultureInfo.InvariantCulture, out value);
            if (!flag)
                FromDevice.ErrorReporter.ReportError("Unable to decode real number: \"{0}\"", (object)s);
            return flag;
        }

        public static double Range(string s)
        {
            double num = 0.0;
            if (!s.IsErr() && !FromDevice.Match(s.Trim(), "AUTO") && (!FromDevice.Real(s, out num) ? 0 : (num != 0.0 ? 1 : 0)) == 0)
                FromDevice.ErrorReporter.ReportError("Unable to decode RangeValue: \"{0}\"", (object)s);
            return num;
        }

        private static bool Match(string s1, string s2) => string.Compare(s1, s2, StringComparison.InvariantCultureIgnoreCase) == 0;

        public static double RealMaybeNA(string s)
        {
            double defaultNa = FromDevice.Default_NA;
            if (s.IsNullOrEmpty() || s.IsNA())
                return defaultNa;
            FromDevice.Real(s, out defaultNa);
            return defaultNa;
        }
    }
}
