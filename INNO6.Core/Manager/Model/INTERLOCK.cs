using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INNO6.Core.Manager.Model
{
    public class INTERLOCK
    {
       public string Name { get; set; }
       public bool IsUse { get; set; }

       public string Type { get; set; }
       public string IoName { get; set; }
       public string Status { get; set; }
       public string Description { get; set; }

       public string AssemblyName { get; set; }

       public string DllFilePath { get; set; }
    }
}
