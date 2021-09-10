using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ECS.UI.Converter
{
    [ValueConversion(typeof(Double), typeof(String))]
    public class DoubleToStringConverter : IValueConverter
    {

        public object Convert(object value,
                              Type targetType,
                              object parameter, // I would like to use this!!
                              System.Globalization.CultureInfo culture)
        {

            string resultado = string.Format("{0:0.000}", // Shouldn't be a hardcoded format!
                                               value);
            return resultado;

        }

        public object ConvertBack(object value,
                                  Type targetType,
                                  object parameter,
                                  System.Globalization.CultureInfo culture)
        {

            string entrada = value as string;

            double resultado = System.Convert.ToDouble(entrada.Replace('.', ','));

            return resultado;

        }

    }
}
