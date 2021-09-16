﻿using ECS.Common.Helper;
using INNO6.Core.Interlock.Interface;
using INNO6.Core.Manager;
using INNO6.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS.Interlock.SetPoint
{
    public class I_EMERGENCY_STOP : AbstractExecuteInterlock
    {
        public override bool Execute(object setvalue)
        {
            FunctionManager.Instance.ABORT_FUNCTION_ALL();

            FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(FuncNameHelper.LASER_SHUTDOWN);
      
            AlarmManager.Instance.SetAlarm("E9999");

            return true;
        }
    }
}
