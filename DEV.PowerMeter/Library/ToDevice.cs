using System;
using System.Globalization;

namespace DEV.PowerMeter.Library
{
    public static class ToDevice
    {
        public static string Bool(bool b) => !b ? "OFF" : "ON";

        public static string Real(double value) => value.ToString((IFormatProvider)CultureInfo.InvariantCulture);

        public static string Uint(uint value) => value.ToString((IFormatProvider)CultureInfo.InvariantCulture);

        public static string Uint64(ulong value) => value.ToString((IFormatProvider)CultureInfo.InvariantCulture);

        public static string Diameter(double value) => value == FromDevice.Default_NA ? "NA" : ToDevice.Real(value);

        public static string Temperature(double value) => value == FromDevice.Default_NA ? "NA" : ToDevice.Real(value);
    }
}
