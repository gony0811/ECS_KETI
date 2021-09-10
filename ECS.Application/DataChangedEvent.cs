using ECS.Common.Helper;
using INNO6.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS.Application
{
    public class DataChangedEvent
    {
        public static void DataManager_DataChangedEvent(object sender, DataChangedEventHandlerArgs args)
        {
            switch(args.Data.Name)
            {
                case IoNameHelper.IN_STR_LASER_STATUS_INTERLOCK:
                    {
                        if ((string)args.Data.Value != "NONE")
                        {
                            DataManager.Instance.SET_INT_DATA(IoNameHelper.V_INT_SAFETY_LASER_INTERLOCK, 1);
                        }
                        else
                        {
                            DataManager.Instance.SET_INT_DATA(IoNameHelper.V_INT_SAFETY_LASER_INTERLOCK, 0);
                        }
                    }
                    break;
            }
        }
    }
}
