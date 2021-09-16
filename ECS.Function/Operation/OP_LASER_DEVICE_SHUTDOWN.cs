using ECS.Common.Helper;
using INNO6.Core.Manager;
using INNO6.Core.Manager.Model;
using INNO6.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ECS.Function.Operation
{
    public class OP_LASER_DEVICE_SHUTDOWN : AbstractFunction
    {
        public override string Execute()
        {
            string result = F_RESULT_SUCCESS;

            IsProcessing = true;

            if (FunctionManager.Instance.EXECUTE_FUNCTION_SYNC(FuncNameHelper.LASER_OFF) != F_RESULT_SUCCESS)
            {
                return F_RESULT_FAIL;
            }

            if (FunctionManager.Instance.EXECUTE_FUNCTION_SYNC(FuncNameHelper.LASER_SHUTDOWN) != F_RESULT_SUCCESS)
            {
                return F_RESULT_FAIL;
            }

            return F_RESULT_SUCCESS;
        }

        public override void PostExecute()
        {
            
        }
    }
}
