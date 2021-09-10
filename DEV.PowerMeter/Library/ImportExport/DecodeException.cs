using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEV.PowerMeter.Library.ImportExport
{
    public class DecodeException : Exception
    {
        public DecodeException(string message)
          : base(message)
        {
        }

        public DecodeException(string message, System.Exception inner)
          : base(message, inner)
        {
        }
    }
}
