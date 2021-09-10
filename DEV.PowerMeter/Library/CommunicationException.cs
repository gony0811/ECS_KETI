using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEV.PowerMeter.Library
{
    public class CommunicationException : MeterException
    {
        public CommunicationException(string message)
          : base(message)
        {
        }
    }
}
