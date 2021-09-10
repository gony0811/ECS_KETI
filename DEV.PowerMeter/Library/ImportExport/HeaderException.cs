using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEV.PowerMeter.Library.ImportExport
{
    public class HeaderException : Exception
    {
        public HeaderException(string message)
          : base(message)
        {
        }

        public HeaderException(string message, System.Exception inner)
          : base(message, inner)
        {
        }
    }
}
