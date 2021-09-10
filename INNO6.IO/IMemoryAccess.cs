using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INNO6.IO.Interface
{
    public enum eDevMode
    {
        UNKNOWN,
        CONNECT,
        DISCONNECT,
        SIMULATE,
        ERROR
    }

    public interface IMemoryAccess
    {
        double GET_DOUBLE_DATA(string name, out bool result);
        int GET_INT_DATA(string name, out bool result);
        string GET_STRING_DATA(string name, out bool result);
        object GET_OBJECT_DATA(string name, out bool result);
        bool SET_DOUBLE_DATA(string name, double value);
        bool SET_INT_DATA(string name, int value);
        bool SET_STRING_DATA(string name, string value);
        bool SET_OBJECT_DATA(string name, object value);

    }
}
