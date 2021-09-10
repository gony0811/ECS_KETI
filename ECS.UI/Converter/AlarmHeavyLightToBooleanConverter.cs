using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ECS.UI.Converter
{
    public class AlarmHeavyLightToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value.ToString().ToUpper())
            {
                case "HEAVY":
                    return true;                    
                case "LIGHT":
                    return false;
                default:
                    return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is bool)
            {
                if ((bool)value == true)
                    return "HEAVY";
                else
                    return "LIGHT";
            }
            return "LIGHT";
        }
    }
}
