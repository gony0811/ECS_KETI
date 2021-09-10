
using SharedLibrary;
using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace DEV.PowerMeter.Library
{
    [API(APICategory.Formatting, "ValueFormatter is a static class, which provides a variety of methods\r\nfor formatting numeric information for human consumption.\r\nInterpreting a global setting for NumberResolution is one of ValueFormatter's several responsibilities.\r\nIt also utilizes ScaleFactor class to scale the number.\r\nFinally it applies a suitable suffix to the numeric result to represent\r\nthe Units in effect when the number originated from the Data Acquisition subsystem.\r\n")]
    public static class ValueFormatter
    {
        public static readonly Regex LegalDigits = new Regex("([\\de+\\-\\.]+)(.*)", RegexOptions.IgnoreCase);
        [API("Converted representation for IEEE 754 floating-point standard NaN (Not A Number) representation.")]
        public const string NotaNumber = "NaN";
        public const string MinOrMax = "None";
        [API("Constant zero as a string (substituted for extremely tiny values)")]
        public const string Zero = "0.0";

        public static bool TryParseWithSuffix(
          string text,
          out double value,
          out string diagnostic,
          string units = null)
        {
            try
            {
                value = ValueFormatter.ParseWithSuffix(text, units);
                diagnostic = (string)null;
                return true;
            }
            catch (Exception ex)
            {
                diagnostic = ex.Message;
                value = 0.0;
                return false;
            }
        }

        public static double ParseWithSuffix(string original, string units = null)
        {
            ValueFormatter.ParseResult r;
            ValueFormatter.SplitSuffix(original, out r);
            r.value = double.Parse(r.prefix);
            if (!r.scale.IsNullOrEmpty())
            {
                ScaleFactor scaleFactor = ScaleFactor.Find(r.scale);
                r.value *= scaleFactor.Factor;
            }
            if (units != null && !string.IsNullOrEmpty(r.suffix) && !units.EqualsIgnoreCase(r.suffix))
                ValueFormatter.ConversionFailure("Bad units: " + r.suffix);
            return r.value;
        }

        private static void ConversionFailure(string message) => throw new FormatException(message);

        public static void SplitSuffix(string text, out ValueFormatter.ParseResult r)
        {
            r = new ValueFormatter.ParseResult()
            {
                original = text
            };
            Match match = ValueFormatter.LegalDigits.Match(text);
            if (!match.Success || match.Groups.Count < 2)
                ValueFormatter.ConversionFailure("bad format: " + text);
            string str = match.Groups[1].Value;
            string suffix = match.Groups[2].Value.Trim();
            string scalePrefix = ScaleFactor.ExtractScalePrefix(suffix);
            if (!string.IsNullOrEmpty(scalePrefix))
                suffix = suffix.Substring(scalePrefix.Length).Trim();
            r.prefix = str;
            r.scale = scalePrefix;
            r.suffix = suffix;
        }

        public static string ValidateUnits(string suffix)
        {
            string str = (string)null;
            if (!string.IsNullOrWhiteSpace(suffix))
            {
                foreach (string key in Units.PhysicalUnitsConverter.Keys)
                {
                    if (!string.IsNullOrEmpty(key) && suffix.EndsWith(key, StringComparison.CurrentCultureIgnoreCase))
                    {
                        str = key;
                        break;
                    }
                }
            }
            if (str == null)
                ValueFormatter.ConversionFailure("invalid units: " + suffix);
            return str;
        }

        [API("The default number of significant digits resulting from conversions\r\n(some methods allow this default to be overriden).\r\nThis essentially is a global default for all users of this class.")]
        public static NumberResolution Resolution { get; set; }

        [API("Most recent Units, utilized in a conversion.")]
        private static Units Units { get; set; }

        [API("Most recent ScaleFactor result (set in FormatScaled).")]
        public static ScaleFactor ScaleFactor { get; private set; }

        [API("Abbreviated name for the most recent ScaleFactor result.")]
        public static string ScaleFactorName => ValueFormatter.ScaleFactor.Abbreviation;

        [API("Most recent conversion Suffix (ScaleFactor+Units, both abbreviated) as a String.")]
        public static string Suffix => ValueFormatter.ScaleFactor.Abbreviation + ValueFormatter.Units.Suffix;

        [API("Most recent conversion Suffix as a String (but NOT abbreviated, e.g. for chart axis titles).")]
        public static string VerboseSuffix => ValueFormatter.OneOrBoth(ValueFormatter.ScaleFactor.Name, ValueFormatter.Units.Name);

        public static string OneOrBoth(string one, string two) => !string.IsNullOrEmpty(one) ? one + " " + two : two;

        static ValueFormatter()
        {
            ValueFormatter.ScaleFactor = ScaleFactor.UnitaryScaleFactor;
            ValueFormatter.Resolution = NumberResolution.D3;
            ValueFormatter.Units = new Units(PhysicalUnits.Watts);
        }

        [API("Format a real number for a specific Units instance, \r\nand overriding the default NumberResolution.")]
        private static string FormatWithSuffix(double value, Units units, NumberResolution resolution)
        {
            string str = ValueFormatter.FormatSpecialValues(value);
            if (str != null)
                return str;
            ValueFormatter.Units = units;
            return ValueFormatter.FormatScaled(value, resolution) + " " + ValueFormatter.Suffix;
        }

        [API("Format a real number for a specific Units instance and default NumberResolution.")]
        public static string FormatWithSuffix(double value, Units units) => ValueFormatter.FormatWithSuffix(value, units, ValueFormatter.Resolution);

        [API("Format a real number for a specific PhysicalUnits instance, \r\nand overriding the default NumberResolution.")]
        public static string FormatWithSuffix(
          double value,
          PhysicalUnits units,
          NumberResolution resolution)
        {
            return ValueFormatter.FormatWithSuffix(value, new Units(units), resolution);
        }

        [API("Format a TimeSpan interval, using the default NumberResolution.")]
        public static string FormatWithSuffix(TimeSpan value) => ValueFormatter.FormatWithSuffix(value.TotalSeconds, Units.Seconds, ValueFormatter.Resolution);

        [API("Format a DateTime timestamp with options. \r\nif energyMode = true then return integer Pulse ID as a string;\r\nelse if showTimesAsMicrosec = true then return the timestamp as microseconds\r\nelse return Timestamp as real number total elapsed Seconds.\r\n")]
        public static string FormatWithSuffix(
          DateTime dateTime,
          bool energyMode = false,
          bool showTimesAsMicrosec = true)
        {
            if (energyMode)
                return dateTime.Ticks.ToString();
            return showTimesAsMicrosec ? dateTime.ToStringMicrosec() + " S" : ValueFormatter.FormatWithSuffix(dateTime.TotalSeconds(), Units.Seconds);
        }

        public static string FormatTimeWithSuffix(double value) => ValueFormatter.FormatWithSuffix(value, Units.Seconds);

        public static string FormatWattsWithSuffix(double value) => ValueFormatter.FormatWithSuffix(value, Units.Seconds);

        public static string FormatSpecialValues(double value)
        {
            if (value == double.MinValue || value == double.MaxValue)
                return "None";
            return double.IsNaN(value) ? "NaN" : (string)null;
        }

        [API("Formats number with suffix including ScaleFactor but omitting Units, \r\nand using default NumberResolution.")]
        public static string FormatScaled(double value) => ValueFormatter.FormatScaled(value, ValueFormatter.Resolution);

        [API("Formats number with suffix including ScaleFactor but omitting Units, \r\nand overriding default NumberResolution.")]
        private static string FormatScaled(double value, NumberResolution resolution) => ValueFormatter.FormatScaled(value, (int)resolution);

        [API("Formats number with suffix including ScaleFactor but omitting Units, \r\nand overriding default NumberResolution with arbitrary number of digits.\r\nThis method may throw if resolution strays outside a small number of digits.")]
        private static string FormatScaled(double value, int resolution)
        {
            string str = ValueFormatter.FormatSpecialValues(value);
            if (str != null)
                return str;
            double num;
            if (ScaleFactor.EffectivelyZero(value))
            {
                num = 0.0;
                ValueFormatter.ScaleFactor = ScaleFactor.UnitaryScaleFactor;
            }
            else if (ValueFormatter.Units.PhysicalUnits == PhysicalUnits.Dbm)
            {
                ValueFormatter.ScaleFactor = ScaleFactor.UnitaryScaleFactor;
                num = value;
            }
            else
            {
                ValueFormatter.ScaleFactor = ScaleFactor.Find(value);
                num = value / ValueFormatter.ScaleFactor.Factor;
            }
            return ValueFormatter.FormatReal(num, resolution);
        }

        [API("Format a real, with no scaling or suffix, and using default NumberResolution. ")]
        public static string FormatReal(double value) => ValueFormatter.FormatReal(value, ValueFormatter.Resolution);

        [API("Format a real, with no scaling or suffix, overriding default NumberResolution. ")]
        private static string FormatReal(double value, NumberResolution resolution) => ValueFormatter.FormatReal(value, (int)resolution);

        [API("Format a Real, with no scaling or suffix, overriding default NumberResolution\r\n\t\twith an arbitrary number of digits. May throw if number of digits is too large.")]
        private static string FormatReal(double value, int resolution)
        {
            string decimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            string str = ValueFormatter.FormatSpecialValues(value);
            if (str != null)
                return str;
            int num = ValueFormatter.LeadingDigits(value);
            return string.Format(ValueFormatter.GetFormat(resolution - num), (object)value);
        }

        [API("Format a real into Scientific Notation (with no scaling or suffix), \r\nusing default NumberResolution. ")]
        public static string FormatScientific(double value) => ValueFormatter.FormatScientific(value, ValueFormatter.Resolution);

        [API("Format a real into Scientific Notation (with no scaling or suffix), \r\noverriding default NumberResolution. ")]
        private static string FormatScientific(double value, NumberResolution resolution) => ValueFormatter.FormatSpecialValues(value) ?? string.Format(ValueFormatter.GetScientificFormat((int)resolution), (object)value);

        [API("Return the number of digits left of the decimal point, given a scaled Real Number.")]
        public static int LeadingDigits(double value) => ScaleFactor.EffectivelyZero(value) ? 1 : string.Format("{0}", (object)(int)Math.Abs(value)).Length;

        public static string GetFormat(int digitsAfterDp) => "{" + string.Format("0:F{0}", (object)Math.Max(0, digitsAfterDp)) + "}";

        public static string GetScientificFormat(int digitsAfterDp) => "{" + string.Format("0:E{0}", (object)Math.Max(0, digitsAfterDp)) + "}";

        [API("Return the smallest, positive, non-zero value possible for the default NumberResolution.")]
        public static double SmallestNonZero() => ValueFormatter.SmallestNonZero(ValueFormatter.Resolution);

        [API("Return the smallest, positive, non-zero value possible for a given NumberResolution.")]
        public static double SmallestNonZero(NumberResolution resolution) => ValueFormatter.SmallestNonZero((int)resolution);

        [API("Return the smallest, positive, non-zero value possible for an arbitrary resolution [may throw on excessive values].")]
        public static double SmallestNonZero(int resolution) => Math.Pow(10.0, (double)(1 - resolution));

        [API("Format an int.")]
        public static string Format(int value) => value.ToString();

        [API("Format a uint.")]
        public static string Format(uint value) => value.ToString();

        [API("Format a percentage value, where value is a ratio (e.g. 0.1 is 10%).")]
        public static string FormatPercent(double value)
        {
            value *= 100.0;
            return ValueFormatter.FormatReal(value);
        }

        [API("Format a percentage, including an explicit percent sign suffix.")]
        public static string FormatPercentWithSuffix(double value) => ValueFormatter.FormatPercent(value) + " %";

        public class ParseResult
        {
            public string original;
            public string prefix;
            public string scale;
            public string suffix;
            public double value;
            public bool success;
        }
    }
}
