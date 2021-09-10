using SharedLibrary;

namespace DEV.PowerMeter.Library
{
    public static class FromUser
    {
        public static IErrorReporter ErrorReporter;// => ServiceLocator.Resolve<IErrorReporter>();

        public static double Real(string s)
        {
            double result = 0.0;
            if (s.IsNullOrEmpty() || !double.TryParse(s, out result))
                FromUser.ErrorReporter.ReportError("Unable to decode float: \"{0}\"", (object)s);
            return result;
        }

        public static uint Uint(string text)
        {
            uint num;
            if (!FromUser.UInt32(text, out num))
                FromUser.ErrorReporter.ReportError("Unable to decode Uint: \"{0}\"", (object)text);
            return num;
        }

        public static bool UInt32(string text, out uint value)
        {
            value = 0U;
            bool flag = uint.TryParse(text, out value);
            if (!flag)
            {
                double num;
                flag = FromUser.Real(text, out num) && num >= 0.0 && num <= (double)uint.MaxValue;
                if (flag)
                    value = (uint)num;
            }
            return flag;
        }

        public static bool Real(string text, out double value)
        {
            value = 0.0;
            return double.TryParse(text, out value);
        }

        public static double Real(string text, double defaultValue = 0.0)
        {
            double num;
            return FromUser.Real(text, out num) ? num : defaultValue;
        }
    }
}
