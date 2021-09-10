using SharedLibrary;
using System;
using System.Globalization;
using System.Threading;
using System.Windows.Data;

namespace DEV.PowerMeter.Library
{
    [API(APICategory.Formatting, "MagnitudeConverter is an IValueConverter, \r\nwhich may be used by charting software to label\r\nthe horizontal axis.")]
    [ValueConversion(typeof(float), typeof(string))]
    public class MagnitudeConverter : IValueConverter, IValueFormatter
    {
        public static Units Units = Units.Watts;

        public static string Format(double value) => ValueFormatter.FormatWithSuffix(value, MagnitudeConverter.Units);

        [API("Convert a Measurement value (a double) to a string suitable for display on the axis.")]
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType == typeof(string))
            {
                switch (value)
                {
                    case string _:
                        return (object)MagnitudeConverter.Format(double.Parse(value as string));
                    case float num3:
                        return (object)MagnitudeConverter.Format((double)num3);
                    case double num4:
                        return (object)MagnitudeConverter.Format(num4);
                }
            }
            throw new NotImplementedException();
        }

        [API("ConvertBack is not needed, not used, and throws NotImplementedException if called.")]
        public object ConvertBack(
          object value,
          Type targetType,
          object parameter,
          CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public string Format(object value) => (string)this.Convert(value, typeof(string), (object)null, Thread.CurrentThread.CurrentCulture);

        public bool TryParse(string s, out ValueOption option)
        {
            double num;
            if (DoubleValue.ConvertWithSuffix(s, out num, out string _))
            {
                option = new ValueOption(num, s);
                return true;
            }
            option = (ValueOption)null;
            return false;
        }

        public bool TryParse(string s, out object value)
        {
            ValueOption option;
            int num = this.TryParse(s, out option) ? 1 : 0;
            value = (object)option;
            return num != 0;
        }

        public object Parse(string s)
        {
            object obj;
            if (this.TryParse(s, out obj))
                return obj;
            throw new FormatException("Invalid Magnitude: " + s);
        }
    }
}
