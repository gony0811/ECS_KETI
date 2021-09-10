using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INNO6.Core.Manager.Model
{
    public class FUNCTION_INFO
    {
        public string ExecuteName { get; set; }

        public bool IsAsyncMode { get; set; }

        public string DllFilePath { get; set; }

        public string AssemblyName { get; set; }

        public bool IsUse { get; set; }

        public int TimeoutMiliseconds { get; set; }
    }
}
