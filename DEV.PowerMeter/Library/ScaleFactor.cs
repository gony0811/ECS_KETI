using SharedLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;


namespace DEV.PowerMeter.Library
{
    [API(APICategory.Formatting, "ScaleFactor is a class for managing scale factors.\r\n\r\nEach instance holds the multiplier and associated number suffix used for \r\nformatting numbers in a form familiar to humans.\r\n\r\nThe class includes numerous static methods and properties that collectively\r\nmanage all the ScaleFactors used by the system.\r\n\r\nThe All property holds an ScaleFactor[] array of all the ones used by the system.")]
    public class ScaleFactor : IXmlDomSerializable
    {
        [API("Values below this show as zero instead of tiny numbers")]
        public const double ZeroThreshold = 1.0000000458137E-18;
        [API("The system wide default scale factor (1.0 scale, no name or abbreviation)")]
        public static readonly ScaleFactor UnitaryScaleFactor = new ScaleFactor("", "", 1.0);
        public const int UnitaryScaleFactorIndex = 2;
        [API("This is the Singleton list of all supported ScaleFactors used by the software: \r\nMega, Kilo, Units (no name or abbreviation), Milli, Micro, Nano, Pico, Fempto.")]
        public static readonly ScaleFactor[] All = new ScaleFactor[9]
        {
      new ScaleFactor("Mega", "M", 1000000.0),
      new ScaleFactor("Kilo", "k", 1000.0),
      ScaleFactor.UnitaryScaleFactor,
      new ScaleFactor("Milli", "m", 1.0 / 1000.0),
      new ScaleFactor("Micro", "µ", 9.99999997475243E-07),
      new ScaleFactor("Micro", "u", 9.99999997475243E-07),
      new ScaleFactor("Nano", "n", 9.99999971718069E-10),
      new ScaleFactor("Pico", "p", 9.99999996004197E-13),
      new ScaleFactor("Fempto", "f", 1.00000000362749E-15)
        };

        [API("Test if an arbitrary value is effectively zero (abs value < ZeroThreshold)")]
        public static bool EffectivelyZero(double value) => Math.Abs(value) < 1.0000000458137E-18;

        [API("The name of this scale factor, e.g., 'Kilo' or 'Micro'")]
        public string Name { get; protected set; }

        [API("The single-character abbreviation for this scale factor, e.g., 'K' or 'µ'")]
        public string Abbreviation { get; protected set; }

        [API("The multiplicative factor to apply to the scaled value to return it to an unscaled value.")]
        public double Factor { get; protected set; }

        [API("Construct a ScaleFactor of a specific combination of properties.")]
        public ScaleFactor(string name, string abbreviation, double factor)
        {
            this.Abbreviation = abbreviation;
            this.Name = name;
            this.Factor = factor;
        }

        [API("Convert this ScaleFactor to a string suitable for display")]
        public override string ToString() => string.Format("{0} {1} {2}", (object)this.Name, (object)this.Abbreviation, (object)this.Factor.ToString());

        [API("Find the largest ScaleFactor less than a given value.")]
        public static ScaleFactor Find(double value) => ScaleFactor.EffectivelyZero(value) ? ScaleFactor.UnitaryScaleFactor : ((IEnumerable<ScaleFactor>)ScaleFactor.All).FirstOrDefault<ScaleFactor>((Func<ScaleFactor, bool>)(sf => sf.Factor <= Math.Abs(value))) ?? ScaleFactor.All[ScaleFactor.All.Length - 1];

        [API("Find the ScaleFactor corresponding to a given (case-sensitive) abbreviation.")]
        public static ScaleFactor Find(string abbreviation)
        {
            if (abbreviation == "u")
                abbreviation = "µ";
            return ((IEnumerable<ScaleFactor>)ScaleFactor.All).FirstOrDefault<ScaleFactor>((Func<ScaleFactor, bool>)(sf => sf.Abbreviation == abbreviation));
        }

        [API("find a multiple of a ScaleFactor that is greater than the given value, \r\n\t\tand yet smaller than the next higher ScaleFactor. This is useful for scaling Dial charts.")]
        public static double FindMaxRange(double value)
        {
            ScaleFactor scaleFactor = ScaleFactor.Find(value);
            double factor = scaleFactor.Factor;
            while (factor < value)
                factor += scaleFactor.Factor;
            return factor;
        }

        public static double ScaleValue(double value, string abbreviation)
        {
            ScaleFactor scaleFactor = ScaleFactor.GetScaleFactor(abbreviation);
            if (scaleFactor == null)
                throw new FormatException("unrecogized scale factor");
            return value * scaleFactor.Factor;
        }

        public static bool TryParse(string text, out double value)
        {
            ScaleFactor scaleFactor = ScaleFactor.GetScaleFactor(text);
            bool flag;
            if (scaleFactor == null)
            {
                flag = double.TryParse(text, out value);
            }
            else
            {
                text = text.Substring(0, text.Length - 1);
                flag = double.TryParse(text, out value);
                if (flag)
                    value *= scaleFactor.Factor;
            }
            return flag;
        }

        public static double Parse(string text)
        {
            ScaleFactor scaleFactor = ScaleFactor.GetScaleFactor(text);
            if (scaleFactor == null)
                return double.Parse(text);
            text = text.Substring(0, text.Length - 1);
            return double.Parse(text) * scaleFactor.Factor;
        }

        public static ScaleFactor GetScaleFactor(string text) => text.Length > 1 ? ScaleFactor.Find(text[text.Length - 1].ToString()) : (ScaleFactor)null;

        public static string ExtractScalePrefix(string suffix)
        {
            if (!string.IsNullOrEmpty(suffix))
            {
                foreach (ScaleFactor scaleFactor in ScaleFactor.All)
                {
                    if (scaleFactor.Abbreviation.Length > 0 && suffix.StartsWith(scaleFactor.Abbreviation))
                        return scaleFactor.Abbreviation;
                }
            }
            return string.Empty;
        }

        [API("Save Units to a XmlWriter [IXmlDomSerializable]")]
        public void SaveXml(XmlWriter writer)
        {
            writer.WriteAttributeString("Name", this.Abbreviation);
            writer.WriteAttributeString("VerboseName", this.Name);
            writer.WriteAttributeString("Factor", this.Factor.ToString());
        }

        [API("Load Units from an XmlNode [IXmlDomSerializable]")]
        public void LoadXml(XmlNode node)
        {
            this.Abbreviation = node.Attributes["Name"].Value;
            this.Name = node.Attributes["VerboseName"].Value;
            this.Factor = (double)float.Parse(node.Attributes["Factor"].Value);
        }
    }
}
