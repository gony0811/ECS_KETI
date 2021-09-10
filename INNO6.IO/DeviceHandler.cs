using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Reflection;
using INNO6.IO.Interface;
using INNO6.Core;

namespace INNO6.IO
{
    public enum eDeviceMethod
    {
        DeviceAttach,
        DeviceDettach,
        DeviceInit,
        DeviceReset,
        IsDevMode,
        SET_INT_OUT,
        GET_INT_IN,
        SET_DOUBLE_OUT,
        GET_DOUBLE_IN,
        SET_STRING_OUT,
        GET_STRING_IN,
        SET_DATA_OUT,
        GET_DATA_IN,
    }

    public class DeviceHandler : IDeviceHandler
    {

        public object DeviceObject { private get; set; }
        private DeviceInfo _deviceInfo;
        private Type _type;
        private static Mutex mutex = new Mutex();
        private static object _key = new object();

        public ConcurrentBag<Data> HandlerDataList;

        public string HandlerName
        {
            get;

            private set;
        }

        public DeviceHandler()
        {
           
        }

        public DeviceHandler(DeviceInfo info, object deviceObject)
        {
            _deviceInfo = info;
            HandlerName = info.DeviceName;
            DeviceObject = deviceObject;
            HandlerDataList = new ConcurrentBag<Data>(DataManager.Instance.GET_POLLING_LIST(HandlerName));
        }

        #region IDeviceHandler 멤버

        public bool DeviceAttach(string arg0, string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7, string arg8, string arg9)
        {
            bool result;
            if (DeviceObject == null) return false;
            object[] param = { arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9 };
            _type = DeviceObject.GetType();
            MethodInfo mInfo = _type.GetMethod(eDeviceMethod.DeviceAttach.ToString());         
            result = (bool)mInfo.Invoke(DeviceObject, param);
            LogHelper.Instance.SystemLog.InfoFormat("[DeviceAttach] {0} , {1} , {2} , {3} , {4} , {5} , {6} , {7} , {8} , {9} ", arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, result);
            return result;
            
        }

        public bool DeviceDettach()
        {        
            MethodInfo mInfo = _type.GetMethod(eDeviceMethod.DeviceDettach.ToString());
            bool result = (bool)mInfo.Invoke(DeviceObject, null);
            LogHelper.Instance.SystemLog.InfoFormat("[DeviceDettach] {0}", result);
            return result;
        }

        public bool DeviceInit()
        {
            MethodInfo mInfo = _type.GetMethod(eDeviceMethod.DeviceInit.ToString());
            bool result = (bool)mInfo.Invoke(DeviceObject, null);
            LogHelper.Instance.SystemLog.InfoFormat("[DeviceInit] {0}", result);
            return result;
        }

        public bool DeviceReset()
        {
            MethodInfo mInfo = _type.GetMethod(eDeviceMethod.DeviceReset.ToString());
            bool result = (bool)mInfo.Invoke(DeviceObject, null);
            LogHelper.Instance.SystemLog.InfoFormat("[DeviceReset] {0}", result);
            return result;
        }

        public eDevMode IsDevMode()
        {
            if (_type == null) return eDevMode.SIMULATE;
            MethodInfo mInfo = _type.GetMethod(eDeviceMethod.IsDevMode.ToString());
            eDevMode mode = (eDevMode)mInfo.Invoke(DeviceObject, null);
            return mode;
        }

        public void SET_INT_OUT(string id_1, string id_2, string id_3, string id_4, int value, ref bool result)
        {
            object[] param = { id_1, id_2, id_3, id_4, value, result };
            int refIndex = param.Length - 1;
            MethodInfo mInfo = _type.GetMethod(eDeviceMethod.SET_INT_OUT.ToString());
            mInfo.Invoke(DeviceObject, param);
            result = (bool)param[refIndex];
            
        }

        public int GET_INT_IN(string id_1, string id_2, string id_3, string id_4, ref bool result)
        {
            object[] param = { id_1, id_2, id_3, id_4, result };
            int refIndex = param.Length - 1;            
            MethodInfo mInfo = _type.GetMethod(eDeviceMethod.GET_INT_IN.ToString());
            int value = (int)mInfo.Invoke(DeviceObject, param);
            result = (bool)param[refIndex];
            return value;
        }

        public void SET_DOUBLE_OUT(string id_1, string id_2, string id_3, string id_4, double value, ref bool result)
        {
            object[] param = { id_1, id_2, id_3, id_4, value, result };
            int refIndex = param.Length - 1;
            MethodInfo mInfo = _type.GetMethod(eDeviceMethod.SET_DOUBLE_OUT.ToString());
            mInfo.Invoke(DeviceObject, param);
            result = (bool)param[refIndex];
            LogHelper.Instance.SystemLog.InfoFormat("[SET_DOUBLE_OUT] {0} , {1} , {2} , {3} , {4} , {5} ", id_1, id_2, id_3, id_4, value, result);
            
        }

        public double GET_DOUBLE_IN(string id_1, string id_2, string id_3, string id_4, ref bool result)
        {
            object[] param = { id_1, id_2, id_3, id_4, result };
            int refIndex = param.Length - 1;
            MethodInfo mInfo = _type.GetMethod(eDeviceMethod.GET_DOUBLE_IN.ToString());
            double value = (double)mInfo.Invoke(DeviceObject, param);

            result = (bool)param[refIndex];
            return value;
        }

        public void SET_STRING_OUT(string id_1, string id_2, string id_3, string id_4, string value, ref bool result)
        {
            lock (_key)
            {
                object[] param = { id_1, id_2, id_3, id_4, value, result };
                int refIndex = param.Length - 1;
                MethodInfo mInfo = _type.GetMethod(eDeviceMethod.SET_STRING_OUT.ToString());
                mInfo.Invoke(DeviceObject, param);
                result = (bool)param[refIndex];
            }
        }

        public string GET_STRING_IN(string id_1, string id_2, string id_3, string id_4, ref bool result)
        {
            object[] param = { id_1, id_2, id_3, id_4, result };
            int refIndex = param.Length - 1;
            MethodInfo mInfo = _type.GetMethod(eDeviceMethod.GET_STRING_IN.ToString());
            string value = (string)mInfo.Invoke(DeviceObject, param);

            result = (bool)param[refIndex];
            return value;
        }

        public void SET_DATA_OUT(string id_1, string id_2, string id_3, string id_4, object value, ref bool result)
        {
            lock (_key)
            {
                object[] param = { id_1, id_2, id_3, id_4, value, result };
                int refIndex = param.Length - 1;
                MethodInfo mInfo = _type.GetMethod(eDeviceMethod.SET_DATA_OUT.ToString());
                mInfo.Invoke(DeviceObject, param);
                result = (bool)param[refIndex];
            }
        }

        public object GET_DATA_IN(string id_1, string id_2, string id_3, string id_4, ref bool result)
        {
            object[] param = { id_1, id_2, id_3, id_4, result };
            int refIndex = param.Length - 1;
            MethodInfo mInfo = _type.GetMethod(eDeviceMethod.GET_DATA_IN.ToString());
            object value = (object)mInfo.Invoke(DeviceObject, param);

            result = (bool)param[refIndex];

            return value;
        }

        #endregion
    }
}
