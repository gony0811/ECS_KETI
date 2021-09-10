using System;

namespace DEV.PowerMeter.Library
{
    public interface IEnumConverter<T> where T : IConvertible
    {
        T FromString(string s);

        string ToString(T e);
    }
}
