using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INNO6.Core.Function.Interface
{
    public interface IFunction
    {
        bool CanExecute();     

        string Execute();

        void PostExecute();

        void ExecuteWhenSimulate();
    }
}
