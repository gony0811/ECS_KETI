using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Data;
using INNO6.IO.Remote;
using INNO6.Core;
using INNO6.Core.Manager;
using INNO6.Core.Manager.Model;

namespace INNO6.IO
{
    public class DataManager : IDisposable
    {
        private static readonly object _key = new object();

        private eRemoteConnectionMode _rmtMode;
        private string _port;
        private RemoteServer _rmtSrv;
        private string _objectUri;
        private string _configIniFilePath;
        private ConfigManager _config;
        private int _dataCount;
        private MemoryAccess _dataAccess;

        public static readonly DataManager Instance = new DataManager();


        //public RemoteObject RemoteObject { get { return base._rmtObj; } }
        public bool Initialized { get; private set; }

        public eRemoteConnectionMode RmtMode
        {
            get { return _rmtMode; }
            set { _rmtMode = value; }
        }

        public MemoryAccess DataAccess { get { return _dataAccess; } }

        public string Port { get { return _port; } set { _port = value; } }


        private DataManager()
        {

        }

        public void Initialize(string configFilePath)
        {
            _config = new ConfigManager(configFilePath);
            _configIniFilePath = configFilePath;

            if (!DataManagerSettings())
            {
                LogHelper.Instance.SystemLog.DebugFormat("[Error] DataManager Setting Failed!!!");
                return;
            }

            InitializeIoDataInfo();
            DeviceManager.Instance.Initialize(configFilePath);
        }


        private bool DataManagerSettings()
        {
            if (_config.GetIniValue("SERVER", "MODE") == "TCP") RmtMode = eRemoteConnectionMode.TCP;
            else RmtMode = eRemoteConnectionMode.IPC;

            _port = _config.GetIniValue("SERVER", "PORT");
            _objectUri = _config.GetIniValue("SERVER", "URI");

            _rmtSrv = new RemoteServer(_objectUri);

            if (!_rmtSrv.Open(_port, RmtMode))
            {
                return false;
            }
            _dataAccess = new MemoryAccess();
            _dataAccess.RemoteObject = _rmtSrv.RemoteObject;
            LogHelper.Instance.SystemLog.DebugFormat("[DataManagerSettings] Server Open Success. Server Port : {0}, Server URI : {1}", _port, _objectUri);
            return true;
        }

        private bool InitializeIoDataInfo()
        {
            InitializeDataInfo dataInfo = new InitializeDataInfo(_configIniFilePath);

            _dataCount = dataInfo.ReadDataInfoList(_dataAccess.RemoteObject);

            return true;
        }



        #region IDisposable 멤버

        void IDisposable.Dispose()
        {
        }

        #endregion

        public Interface.eDevMode IsDeviceMode(string driverName)
        {
            return DeviceManager.Instance.IsDeviceMode(driverName);
        }

        public IEnumerable<Data> GET_POLLING_LIST(string HandlerName)
        {            
            List<Data> dataList = DataManager.Instance.DataAccess.RemoteObject.DataList.Where(t => (t.PollingTime > 0 && t.Use && t.Direction == eDirection.IN && t.DriverName == HandlerName)).ToList();
            int count = dataList.Count;
            Data[] dataArray = new Data[count];
            dataList.CopyTo(dataArray);
            return dataArray.ToList();
        }

        public bool SET_INT_DATA(string name, int value)
        {
            lock (_key)
            {
                bool result = false;
                Data data;

                if (!_dataAccess.RemoteObject.GetData(name, out data))
                    return false;

                if (data.Logging)
                    LogHelper.Instance.SystemLog.InfoFormat("[SET_INT_DATA] {0} : {1}", name, value.ToString());

                if (CheckInterlockEx(name, value))
                {
                    return false;
                }
                else
                {
                    if (DeviceManager.Instance.SetDataToDevice(name, value))
                    {
                        result = _dataAccess.SET_INT_DATA(name, value);
                    }
                    else
                    {
                        result = false;
                    }

                    return result;
                }
            }
        }

        public bool SET_DOUBLE_DATA(string name, double value)
        {
            lock (_key)
            {
                bool result = false;
                if (!_dataAccess.RemoteObject.GetData(name, out Data data))
                    return false;

                if (data.Logging)
                    LogHelper.Instance.SystemLog.DebugFormat("[SET_DOUBLE_DATA] {0} : {1}", name, value.ToString());

                if (CheckInterlockEx(name, value))
                {
                    return false;
                }

                DeviceManager.Instance.SetDataToDevice(name, value);

                result = _dataAccess.SET_DOUBLE_DATA(name, value);

                return result;
            }
        }

        public bool SET_STRING_DATA(string name, string value)
        {
            lock (_key)
            {
                bool result = false;
                Data data;
                if (!_dataAccess.RemoteObject.GetData(name, out data))
                    return false;

                if (data.Logging)
                    LogHelper.Instance.SystemLog.DebugFormat("[SET_STRING_DATA] {0} : {1}", name, value.ToString());

                if(CheckInterlockEx(name, value))
                {
                    return false;
                }
                else
                {
                    DeviceManager.Instance.SetDataToDevice(name, value);
                    result = _dataAccess.SET_STRING_DATA(name, value);

                    return result;
                }
            }
        }

        private bool CheckInterlockEx(string name, object value, Data data = null)
        {
            bool result = false;

            if (data == null && !_dataAccess.RemoteObject.GetData(name, out data))
            {
                return false;
            }

            List<INTERLOCK> interlocks = InterlockManager.Instance.GET_INTERLOCK_LIST(name);

            if (interlocks == null) return false;

            foreach(INTERLOCK i in interlocks)
            {
                if(i.Type.StartsWith("V"))
                {
                    if(result |= InterlockManager.Instance.VALUE_INTERLOCK(i.Name, value, (int)data.Type))
                        LogHelper.Instance.SystemLog.DebugFormat("VALUE INTERLOCK={0} : IONAME={1}, VALUE={2}", i.Name, data.Name, value.ToString());
                }
                else if(i.Type.StartsWith("S"))
                {
                    if (result |= InterlockManager.Instance.SETPOINT_INTERLOCK(i.Name, value, (int)data.Type))
                        LogHelper.Instance.SystemLog.DebugFormat("SETPOINT INTERLOCK={0} : IONAME={1}, VALUE={2}", i.Name, data.Name, value.ToString());
                }
            }

            return result;
        }

        //private bool CheckInterlock(string name, object value, Data data = null)
        //{
        //    if (data == null && !_dataAccess.RemoteObject.GetData(name, out data))
        //    {
        //        return false;
        //    }           

        //    if (!string.IsNullOrEmpty(data.InterlockName) &&
        //          data.InterlockMode == eInterlock.SETPOINT &&
        //          InterlockManager.Instance.SETPOINT_INTERLOCK(data.InterlockName, value, (int)data.Type))
        //    {
        //        if (data.Logging)
        //            LogHelper.Instance.SystemLog.DebugFormat("[SET_INT_DATA] SETPOINT INTERLOCK {0} : {1}", name, value.ToString());

        //        return true;
        //    }
        //    else if (!string.IsNullOrEmpty(data.InterlockName) &&
        //        data.InterlockMode == eInterlock.SETVALUE &&
        //        InterlockManager.Instance.VALUE_INTERLOCK(data.InterlockName, value, (int)data.Type))
        //    {
        //        if (data.Logging)
        //            LogHelper.Instance.SystemLog.DebugFormat("[SET_INT_DATA] SETVALUE INTERLOCK {0} : {1}", name, value.ToString());

        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        public bool SET_OBJECT_DATA(string name, object value)
        {
            lock (_key)
            {
                bool result = false;
                Data data;
                if (!_dataAccess.RemoteObject.GetData(name, out data))
                    return false;

                if (data.Logging)
                    LogHelper.Instance.SystemLog.DebugFormat("[SET_OBJECT_DATA] {0} : {1}", name, value.ToString());

                if (CheckInterlockEx(name, value, data))
                {
                    return false;
                }

                DeviceManager.Instance.SetDataToDevice(name, value);
                result = _dataAccess.SET_OBJECT_DATA(name, value);

                return result;
            }
        }

        public int GET_INT_DATA(string name, out bool result)
        {
            int mmf_value = _dataAccess.GET_INT_DATA(name, out result);

            object value;

            if (DeviceManager.Instance.GetDataFromDevice(name, out value))
            {
                CheckInterlockEx(name, value);

                if ((int)value != mmf_value)
                {
                    //CheckInterlock(name, value);
                    _dataAccess.SET_INT_DATA(name, (int)value);               
                }               
            }
            else
            {
                return mmf_value;
            }

            if(_dataAccess.IsDataLogging(name)) LogHelper.Instance.SystemLog.DebugFormat("[GET_INT_DATA] {0} : {1}", name, (int)value);

            return (int)value;
        }


        public double GET_DOUBLE_DATA(string name, out bool result)
        {
            double mmf_value = _dataAccess.GET_DOUBLE_DATA(name, out result);
            object value;

            if (DeviceManager.Instance.GetDataFromDevice(name, out value))
            {
                CheckInterlockEx(name, value);

                if ((double)value != mmf_value)
                {
                    //CheckInterlock(name, value);
                    _dataAccess.SET_DOUBLE_DATA(name, (double)value);
                }                  
            }
            else
            {               
                return mmf_value;
            }
            if (_dataAccess.IsDataLogging(name)) LogHelper.Instance.SystemLog.DebugFormat("[GET_DOUBLE_DATA] {0} : {1}", name, (double)value);
            return (double)value;
        }

        public string GET_STRING_DATA(string name, out bool result)
        {
            string mmf_value = _dataAccess.GET_STRING_DATA(name, out result);
            object value;

            if (DeviceManager.Instance.GetDataFromDevice(name, out value))
            {
                CheckInterlockEx(name, value);

                if (value.ToString() != mmf_value)
                {
                    //CheckInterlock(name, value);
                    _dataAccess.SET_STRING_DATA(name, (string)value);
                }
            }
            else
            {
                return mmf_value;
            }
            if (_dataAccess.IsDataLogging(name)) LogHelper.Instance.SystemLog.DebugFormat("[GET_STRING_DATA] {0} : {1}", name, value);
            return (string)value;
        }

        public object GET_OBJECT_DATA(string name, out bool result)
        {
            object mmf_value = _dataAccess.GET_OBJECT_DATA(name, out result);
            object value;

            if (DeviceManager.Instance.GetDataFromDevice(name, out value))
            {
                CheckInterlockEx(name, value);

                if (value != mmf_value)
                {                    
                    _dataAccess.SET_OBJECT_DATA(name, (byte[])value);
                }
            }
            else
            {
                return mmf_value;
            }
            if (_dataAccess.IsDataLogging(name)) LogHelper.Instance.SystemLog.DebugFormat("[GET_OBJECT_DATA] {0} : {1}", name, value.ToString());
            return value;
        }

        public object GET_DATA(string name)
        {
            Data data = _dataAccess.RemoteObject.DataList.Where(d => d.Name == name).FirstOrDefault();
            bool result;
            object value = null;

            if (data == null)
            {
                LogHelper.Instance.SystemLog.DebugFormat("[ERROR] Invalid data request (Unknown Data Name) : {0}", name);
                return null;
            }

            if(IsDeviceMode(data.DriverName) == Interface.eDevMode.CONNECT)
            {

                switch (data.Type)
                {
                    case eDataType.String:
                        {
                            value = GET_STRING_DATA(data.Name, out result);
                        }
                        break;
                    case eDataType.Int:
                        {
                            value = GET_INT_DATA(data.Name, out result);
                        }
                        break;
                    case eDataType.Double:
                        {
                            value = GET_DOUBLE_DATA(data.Name, out result);
                        }
                        break;
                    case eDataType.Object:
                        {
                            value = GET_OBJECT_DATA(data.Name, out result);
                        }
                        break;
                }
            }

            return value;
        }

        public bool SET_DATA(string name, object value)
        {
            Data data = _dataAccess.RemoteObject.DataList.Where(d => d.Name == name).FirstOrDefault();
            bool result = false;

            if (data == null)
            {
                LogHelper.Instance.SystemLog.DebugFormat("[ERROR] Invalid data request (Unknown Data Name) : {0}", name);
                return false;
            }


            switch (data.Type)
            {
                case eDataType.String:
                    {
                        result = SET_STRING_DATA(data.Name, (string)value);
                    }
                    break;
                case eDataType.Int:
                    {
                        result = SET_INT_DATA(data.Name, (int)value);
                    }
                    break;
                case eDataType.Double:
                    {
                        result = SET_DOUBLE_DATA(data.Name, (double)value);
                    }
                    break;
                case eDataType.Object:
                    {
                        result = SET_OBJECT_DATA(data.Name, (byte[])value);
                    }
                    break;
            }

            return result;
        }

        public Data GET_DATA_INFO(string name)
        {
            if (_dataAccess.IsDataLogging(name)) LogHelper.Instance.SystemLog.DebugFormat("[GET_DATA_INFO] {0}", name);
            Data data = _dataAccess.RemoteObject.DataList.Where(d => d.Name == name).FirstOrDefault();

            if (data == null) return new Data();
            
            return new Data(data);
        }

        public bool CHANGE_DEFAULT_DATA(string name, object value)
        {
            Data data = _dataAccess.RemoteObject.DataList.Where(d => d.Name == name).FirstOrDefault();
            string query = string.Empty;
            string dbFilePath = _config.GetIniValue("DATADB", "PATH");

            if (data == null) return false;

            switch (data.Type)
            {
                case eDataType.String:
                    {
                        query = string.Format(@"UPDATE master_io SET DefaultValue=""{0}"" WHERE Name=""{1}""", value, name);
                    }
                    break;
                case eDataType.Int:
                    {
                        query = string.Format(@"UPDATE master_io SET DefaultValue=""{0}"" WHERE Name=""{1}""", (int)value, name);
                    }
                    break;
                case eDataType.Double:
                    {
                        query = string.Format(@"UPDATE master_io SET DefaultValue=""{0:N3}"" WHERE Name=""{1}""", (double)value, name);
                    }
                    break;
                case eDataType.Object:
                    {
                        query = string.Format(@"UPDATE master_io SET DefaultValue=""{0}"" WHERE Name=""{1}""", value, name);
                    }
                    break;
            }

            if (!string.IsNullOrEmpty(query))
            {
                return DbHandler.Instance.ExecuteNonQuery(dbFilePath, query) > 0;
            }
            else
            {
                return false;
            }
        }
    }
}
