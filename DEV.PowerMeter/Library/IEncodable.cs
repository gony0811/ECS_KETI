
using System.Globalization;

namespace DEV.PowerMeter.Library
{
    public interface IEncodable
    {
        string[] Encode(CultureInfo culture);
    }
}
