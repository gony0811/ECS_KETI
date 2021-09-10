using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEV.PowerMeter.Library
{
    public class UnexpectedEofException : CommunicationException
    {
        public UnexpectedEofException(string message)
          : base(message)
        {
        }
    }
}
