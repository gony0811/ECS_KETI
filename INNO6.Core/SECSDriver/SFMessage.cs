using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INNO6.Core
{
    public abstract class SFMessage
    {
        public SECSDriverBase SecsDriver { get; protected set; }
        public SFMessage(SECSDriverBase driver)
        {
            SecsDriver = driver;
        }

        public int Stream { get; set; }
        public int Function { get; set; }
        public string Name { get; set; }

        public abstract void DoWork(string driverName, object obj);
    }
}
