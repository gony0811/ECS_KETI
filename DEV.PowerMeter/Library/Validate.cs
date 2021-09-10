using System.Text.RegularExpressions;

namespace DEV.PowerMeter.Library
{
    public static class Validate
    {
        public static readonly Regex LegalWavelength = new Regex("(\\d+)(.*)");

        public static string UInt32(string text)
        {
            uint result = 0;
            if (uint.TryParse(text, out result))
                return (string)null;
            double num;
            if (Validate.Real(text, out num) != null)
                return "Badly formatted number";
            if (num < 0.0)
                return "Negative numbers not allowed";
            if (num > (double)uint.MaxValue)
                return "Number is too large";
            return (string)null;
        }

        public static string UInt32(string text, uint maximum)
        {
            string str1 = Validate.UInt32(text);
            if (str1 != null)
                return str1;
            double num;
            string str2 = Validate.Real(text, out num);
            if (str2 != null)
                return str2;
            return num <= (double)maximum ? (string)null : string.Format("Number must be ≤ {0}", (object)maximum);
        }

        public static string UInt32(string text, uint minimum, uint maximum)
        {
            string str1 = Validate.UInt32(text, maximum);
            if (str1 != null)
                return str1;
            double num;
            string str2 = Validate.Real(text, out num);
            if (str2 != null)
                return str2;
            return num >= (double)minimum ? (string)null : string.Format("Number must be ≥ {0}", (object)minimum);
        }

        public static string StripSuffix(string value)
        {
            Match match = Validate.LegalWavelength.Match(value);
            return !match.Success || match.Groups.Count < 2 ? (string)null : match.Groups[1].Value;
        }

        public static string Real(string text) => Validate.Real(text, out double _);

        public static string Real(string text, out double value)
        {
            value = 0.0;
            return !FromUser.Real(text, out value) ? "Invalid real number" : (string)null;
        }

        public static string Real(string text, double? minimum = null, double? maximum = null)
        {
            double num;
            return Validate.Real(text, out num) ?? Validate.Real(num, minimum, maximum);
        }

        public static string Real(double value, double? minimum = null, double? maximum = null)
        {
            if (minimum.HasValue && minimum.Value > value)
                return string.Format("Value must be ≥ {0}", (object)minimum.Value);
            return maximum.HasValue && maximum.Value < value ? string.Format("Value must be ≤ {0}", (object)maximum.Value) : (string)null;
        }

        public static string Real_GZ(string text) => Validate.Real_GZ(text, out double _);

        public static string Real_GZ(string text, out double value)
        {
            string str = Validate.Real(text, out value);
            if (str != null)
                return str;
            return value <= 0.0 ? "Value must be strictly > 0" : (string)null;
        }

        public static bool Real_GZ(double value) => value > 0.0;

        public static string UInt_GZ(string text) => Validate.UInt_GZ(text, out uint _);

        public static string UInt_GZ(string text, out uint value)
        {
            if (!FromUser.UInt32(text, out value))
                return "Invalid unsigned integer";
            return value <= 0U ? "Value must be strictly > 0" : (string)null;
        }

        public static bool Uint_GZ(uint value) => value > 0U;
    }
}
