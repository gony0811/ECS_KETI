
using SharedLibrary;
using System;
using System.Diagnostics;
using System.Globalization;


namespace DEV.PowerMeter.Library
{
    [API(APICategory.Formatting, "A class of extension methods for useful conversions involving DateTime instances.")]
    public static class DateTimeExtensions
    {
        [API("The starting time for all Data Acquisition sequences.")]
        public static readonly DateTime ZeroDateTime;
        public const double OneTick = 1E-07;

        [API("True iff given DateTime is Zero.")]
        public static bool IsZero(this DateTime datetime) => datetime == DateTimeExtensions.ZeroDateTime;

        [API("Create a DateTime corresponding to given number of seconds since start.")]
        public static DateTime FromSeconds(double seconds)
        {
            Trace.Assert(seconds >= 0.0, "DateTimeExtensions.FromSeconds argument cannot be negative.");
            return new DateTime((long)(seconds * 10000000.0));
        }

        [API("Create a DateTime corresponding to given number of milliseconds since start.")]
        public static DateTime FromMilliSeconds(double milliSeconds) => new DateTime((long)(milliSeconds * 10000.0));

        [API("Return a Real number, representing the offset for the given DateTIme.")]
        public static double TotalSeconds(this DateTime datetime) => datetime.Subtract(DateTimeExtensions.ZeroDateTime).TotalSeconds;

        [API("Compare a DateTime to a double offset in Seconds.")]
        public static bool LessThan(this DateTime datetime, double b) => datetime.TotalSeconds() < b;

        [API("Compare a DateTime to a double offset in Seconds.")]
        public static bool GreaterThan(this DateTime datetime, double b) => datetime.TotalSeconds() > b;

        [API("Format a DateTime to its offset in Seconds.\r\nThe number shows 7 digits right of the dp, so resolution shows to 1/10th of a microsec.\r\nNumber conversion uses the specified Culture.")]
        public static string ToStringMicrosec(this DateTime datetime, CultureInfo culture) => string.Format((IFormatProvider)culture, "{0:0.0000000}", (object)datetime.TotalSeconds());

        [API("Format a DateTime to its offset in Seconds.\r\nThe number shows 7 digits right of the dp, so resolution shows to 1/10th of a microsec.\r\nNumber conversion uses the App's Current Culture.")]
        public static string ToStringMicrosec(this DateTime datetime) => string.Format("{0:0.0000000}", (object)datetime.TotalSeconds());

        [API("Format a DateTime to its offset in Seconds.\r\nThe number shows 3 digits right of the dp, for millisecond resolution.\r\nNumber conversion uses the App's Current Culture.")]
        public static string ToStringMillisec(this DateTime datetime) => string.Format("{0:0.000}", (object)datetime.TotalSeconds());

        [API("Convert a DateTime to its offset in Seconds, as a Real number.")]
        public static double ToSeconds(this DateTime datetime) => datetime.TotalSeconds();

        public static DateTime AddSecondsAccurately(this DateTime datetime, double seconds)
        {
            if (Math.Abs(seconds) < 1E-07)
                throw new ArgumentException("seconds must be >= 1e-7");
            return new DateTime(datetime.Ticks + (long)(seconds * 10000000.0));
        }
    }
}
