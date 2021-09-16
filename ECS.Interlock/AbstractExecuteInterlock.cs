using ECS.Common.Helper;
using INNO6.Core.Interlock.Interface;
using INNO6.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS.Interlock
{
    public abstract class AbstractExecuteInterlock : IExecuteInterlock
    {
        public virtual void PreExecute()
        {
           
        }

        public abstract bool Execute(object setValue);
    }
}
