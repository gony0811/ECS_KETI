using System;
using System.Globalization;
using System.Windows.Data;

namespace DEV.PowerMeter.Library
{
    [ValueConversion(typeof(DateTime), typeof(string))]
    public class TimestampConverter : IValueConverter
    {
        public const int LowSpeedThreshold_Ticks = 10000;

        public static bool EnergyMode { get; set; }

        public static bool ShowMicroseconds { get; private set; }

        public static void SetSamplePeriod(long ticks) => TimestampConverter.ShowMicroseconds = ticks < 10000L;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType == typeof(string))
            {
                if (value is double)
                    value = (object)DateTimeExtensions.FromSeconds((double)value);
                if (value is DateTime datetime2)
                {
                    if (TimestampConverter.EnergyMode)
                        return (object)datetime2.Ticks.ToString();
                    return TimestampConverter.ShowMicroseconds ? (object)datetime2.ToStringMicrosec() : (object)datetime2.ToStringMillisec();
                }
            }
            throw new NotImplementedException();
        }

        public object ConvertBack(
          object value,
          Type targetType,
          object parameter,
          CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
