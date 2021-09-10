using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using System.Threading;
using INNO6.Core;
using System.IO;

namespace INNO6.IO
{
    public delegate void DeviceLoadEvent(string deviceName);

    public enum eDeviceReadWriteResult
    {
        UnknownError,
        InvalidDeviceName,
        InvalidDataName,
        VirtualData,
        Success
    }

    public class DeviceManager
    {
        public static readonly DeviceManager Instance = new DeviceManager();
        private SortedList<string, DeviceInfo> _deviceInfoList = new SortedList<string,DeviceInfo>();

        private ConfigManager _config;
        private string _configIniFilePath;
        private int _deviceCount;

        public event DeviceLoadEvent DeviceAttached;
        public event DeviceLoadEvent DeviceDettached;
       

        private DeviceManager()
        {

        }

        public void Initialize(string configFilePath)
        {
            _config = new ConfigManager(configFilePath);
            _configIniFilePath = configFilePath;

            InitializeDeviceInfo();
        }

        private bool InitializeDeviceInfo()
        {
            try
            {
                bool deviceAttachSuccess = true;
                InitializeDataInfo deviceInfo = new InitializeDataInfo(_configIniFilePath);
                _deviceCount = deviceInfo.ReadDeviceInfoList(_deviceInfoList);

                foreach (DeviceInfo device in _deviceInfoList.Values)
                {
                    if (device.Use == false) continue;
                    string pullPath = Path.GetFullPath(device.FileName);
                    Assembly assembly = Assembly.LoadFile(pullPath);
                    object device_instance = assembly.CreateInstance(device.InstanceName);
                    DeviceHandler handler = new DeviceHandler(device, device_instance);
                    
                    if(handler == null || device_instance == null ) LogHelper.Instance.SystemLog.DebugFormat("[ERROR] Device file has some problems (Device filename={0} | Device Name={1})", device.FileName, device.DeviceName);

                    device.DeviceHandler = handler;

                    if(device.Use)
                    {
                        deviceAttachSuccess = handler.DeviceAttach(device.Arguments[0], device.Arguments[1], device.Arguments[2], device.Arguments[3], device.Arguments[4], device.Arguments[5], 
                             device.Arguments[6], device.Arguments[7], device.Arguments[8], device.Arguments[9]);

                        if (!deviceAttachSuccess)
                        {
                            LogHelper.Instance.SystemLog.DebugFormat("[ERROR] Device({0}) Attach Failed : DLL file name is {1}", device.DeviceName, device.FileName);
                            continue;
                        } 
                    }
                    else
                    {
                        LogHelper.Instance.SystemLog.DebugFormat("[INFO] Device({0}) will be executed simulation mode", device.DeviceName);
                        continue;
                    }

                    if(DeviceAttached != null)
                        DeviceAttached(device.DeviceName);
                }

                DataPollingStart();

                return true;
            }
            catch(System.IO.FileLoadException ex)
            {
                LogHelper.Instance.ErrorLog.DebugFormat("[ERROR] Device file load fali : {0}", ex.Message);
                throw ex;
            }
            catch(System.IO.FileNotFoundException ex)
            {
                LogHelper.Instance.ErrorLog.DebugFormat("[ERROR] Device file not found : {0}", ex.Message);
                throw ex;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.ErrorLog.DebugFormat("[ERROR] Add device failed or Device instance creation failed : {0}", ex.Message);
                throw ex;
            }
            
        }


        public bool DeinitializeDeviceInfo()
        {
            try
            {
                bool deviceDettachSuccess = true;

                foreach (DeviceInfo device in _deviceInfoList.Values)
                {
                    if (device.Use == false) continue;

                    DeviceHandler handler = device.DeviceHandler;

                    if (handler.IsDevMode() == Interface.eDevMode.CONNECT)
                    {
                        deviceDettachSuccess = handler.DeviceDettach();

                        DeviceDettached?.Invoke(device.DeviceName);

                        if (!deviceDettachSuccess)
                        {
                            LogHelper.Instance.ErrorLog.DebugFormat("[ERROR] Device({0}) Dettach Failed", device.DeviceName);
                        }
                    }
                }

                return true;
            }
            catch(Exception ex)
            {
                LogHelper.Instance.ErrorLog.DebugFormat("[ERROR]" + ex.Message);
                throw ex;
            }

        }

        private void DataPollingStart()
        {
            foreach(string name in _deviceInfoList.Keys)
            {
                if(!_deviceInfoList[name].Use) continue;


                //DeviceTimer deviceTimer = new DeviceTimer(_deviceInfoList[name].DeviceHandler, DataPollingMethod);
                DeviceThread deviceThread = new DeviceThread(_deviceInfoList[name].DeviceHandler, DataPollingMethod);
                deviceThread.ThreadStart();

                _deviceInfoList[name].DevicePolling = deviceThread;
            }
        }    

        private void DataPollingMethod(object deviceHandler)
        {        
            DeviceHandler handler = deviceHandler as DeviceHandler;

            if (handler.IsDevMode() == Interface.eDevMode.DISCONNECT || handler.IsDevMode() == Interface.eDevMode.UNKNOWN) return;

            Parallel.ForEach<Data>(handler.HandlerDataList, (data) =>
            //foreach (Data data in dataList)
            //for (int i = 0; i < dataList.Count; i++ )
            {
                TimeSpan ts = DateTime.Now - data.CheckTime;
                //if (ts.TotalMilliseconds > 200) Console.WriteLine("[Warning] Data Polling too slow ({0}) : {1} / {2}", data.Name, ts.TotalMilliseconds.ToString(), data.PollingTime.ToString());
                //if (ts.TotalMilliseconds > 100) LogHelper.Instance._debug.DebugFormat("[Warning] Data Polling too slow ({0}) : {1} / {2}", data.Name, ts.TotalMilliseconds.ToString(), data.PollingTime.ToString());//Console.WriteLine("[Warning] Data Polling too slow ({0}) : {1} / {2}", data.Name, ts.TotalMilliseconds.ToString(), data.PollingTime.ToString());

                //if (data.Group == "ALARM" && ts.Milliseconds > 100) LogHelper.Instance._debug.DebugFormat("[Warning] Data Polling too slow ({0}) : {1} / {2} / {3}", data.Name, ts.TotalMilliseconds.ToString(), data.PollingTime.ToString(), Thread.CurrentThread.ManagedThreadId);

                bool result = false;

                switch (data.Type)
                {
                    case eDataType.Int:
                        {
                            int value = handler.GET_INT_IN(data.Config1, data.Config2, data.Config3, data.Config4, ref result);
                            if (result) { DataManager.Instance.DataAccess.SET_INT_DATA(data.Name, value); }
                        }
                        break;
                    case eDataType.Double:
                        {
                            double value = handler.GET_DOUBLE_IN(data.Config1, data.Config2, data.Config3, data.Config4, ref result);
                            if (result)
                            {
                                double format = data.Format;
                                if (format <= 0) format = 1.0;
                                value /= format;
                                DataManager.Instance.DataAccess.SET_DOUBLE_DATA(data.Name, value);
                            }
                        }
                        break;
                    case eDataType.String:
                        {
                            string value = handler.GET_STRING_IN(data.Config1, data.Config2, data.Config3, data.Config4, ref result);
                            if (result)
                            {
                                DataManager.Instance.DataAccess.SET_STRING_DATA(data.Name, value);
                            }
                        }
                        break;
                    default:
                        {
                            LogHelper.Instance.SystemLog.DebugFormat("[Error] DataType is unknown!!! : {0} / {1}", data.Name, data.Type.ToString());
                        }
                        break;
                }

                data.CheckTime = DateTime.Now;
            });
        }


        public bool SetDataToDevice(string name, object value)
        {
            bool result = false;

            try
            {
                var data = DataManager.Instance.DataAccess.RemoteObject.DataList.Where(io => io.Name == name).FirstOrDefault();

                if (data == null ) return false;

                if (_deviceInfoList.ContainsKey(data.DriverName) == false || data.DriverName.ToUpper() == "VIRTUAL") return true;

                else if (_deviceInfoList[data.DriverName].Use == false)
                {
                    return true;
                }

                var devMode = _deviceInfoList[data.DriverName].DeviceHandler.IsDevMode();
                
                if (devMode == Interface.eDevMode.DISCONNECT || devMode == Interface.eDevMode.UNKNOWN)
                {
                    return false;
                }
                else
                {
                    switch (data.Type)
                    {
                        case eDataType.Int:
                            {
                                _deviceInfoList[data.DriverName].DeviceHandler.SET_INT_OUT(data.Config1, data.Config2, data.Config3, data.Config4, (int)value, ref result);
                            }
                            break;
                        case eDataType.Double:
                            {
                                double format = data.Format;
                                if (data.Format <= 0) format = 1;
                                _deviceInfoList[data.DriverName].DeviceHandler.SET_DOUBLE_OUT(data.Config1, data.Config2, data.Config3, data.Config4, (double)value * format, ref result);
                            }
                            break;
                        case eDataType.String:
                            {
                                _deviceInfoList[data.DriverName].DeviceHandler.SET_STRING_OUT(data.Config1, data.Config2, data.Config3, data.Config4, (string)value, ref result);
                            }
                            break;
                        case eDataType.Object:
                            {
                                _deviceInfoList[data.DriverName].DeviceHandler.SET_DATA_OUT(data.Config1, data.Config2, data.Config3, data.Config4, value, ref result);                         
                            }
                            break;
                        default:
                            {
                                LogHelper.Instance.SystemLog.DebugFormat("[ERROR] DataType is unknown!!! : {0} / {1}", data.Name, data.Type.ToString());
                            }
                            break;
                    }
                }

            }
            catch(Exception ex)
            {
                LogHelper.Instance.ErrorLog.DebugFormat("[ERROR] SetDataToDevice() : {0}", ex.Message);
                return false;
            }

            if (!result) return false;
            
            return true;
        }

        public bool GetDataFromDevice(string name, out object value)
        {        
            bool result = false;
                
            try                
            {
                var data = DataManager.Instance.DataAccess.RemoteObject.DataList.Where(io => io.Name == name).FirstOrDefault();

                value = null;

                if (data == null) return false;

                if (!data.DriverName.ToUpper().Equals("VIRTUAL") && _deviceInfoList.ContainsKey(data.DriverName) == false)
                {
                    throw new KeyNotFoundException(string.Format("Execption : DeviceName is wrong [IO.DriverName = {0}]", data.DriverName));

                }
                //else if (data.Direction == eDirection.OUT)
                //{
                //    value = data.Value;
                //    return true;
                //    //throw new KeyNotFoundException(string.Format("Execption : io direction is wrong [IO.DriverName = {0}, IO.Direction = {1}]", data.DriverName, data.Direction));
                //}
                else if (data.Direction == eDirection.OUT || data.DriverName.ToUpper() == "VIRTUAL")
                {
                    value = data.Value;
                    return true;
                }

                if (_deviceInfoList[data.DriverName].DeviceHandler == null) return false;

                if (_deviceInfoList[data.DriverName].Use == false)
                {
                    value = data.Value;
                    return true;
                }

                var devMode = _deviceInfoList[data.DriverName].DeviceHandler.IsDevMode();
                
                if (devMode == Interface.eDevMode.DISCONNECT || devMode == Interface.eDevMode.UNKNOWN)
                {
                    return false;
                }
                else
                {
                    value = data.Value;

                    switch (data.Type)
                    {
                        case eDataType.Int:
                            {
                                value = _deviceInfoList[data.DriverName].DeviceHandler.GET_INT_IN(data.Config1, data.Config2, data.Config3, data.Config4, ref result);
                            }
                            break;
                        case eDataType.Double:
                            {
                                double format = data.Format;
                                value = _deviceInfoList[data.DriverName].DeviceHandler.GET_DOUBLE_IN(data.Config1, data.Config2, data.Config3, data.Config4, ref result);
                                if (format <= 0) format = 1;

                                value = (double)value / format;
                            }
                            break;
                        case eDataType.String:
                            {
                                value = _deviceInfoList[data.DriverName].DeviceHandler.GET_STRING_IN(data.Config1, data.Config2, data.Config3, data.Config4, ref result);
                            }
                            break;
                        case eDataType.Object:
                            {
                                value = _deviceInfoList[data.DriverName].DeviceHandler.GET_DATA_IN(data.Config1, data.Config2, data.Config3, data.Config4, ref result);                         
                            }
                            break;
                        default:
                            {
                                value = null;
                                LogHelper.Instance.SystemLog.DebugFormat("[ERROR] DataType is unknown!!! : {0} / {1}", data.Name, data.Type.ToString());
                            }
                            break;
                    }
                }

            }
            catch(Exception ex)
            {
                LogHelper.Instance.ErrorLog.DebugFormat("[ERROR] GetDataFromDevice() : {0}", ex.Message);
                value = null;
                return false;
            }

            if (!result) return false;

            return true;
        }

        public Interface.eDevMode IsDeviceMode(string driverName)
        {
            if(_deviceInfoList.ContainsKey(driverName) && _deviceInfoList[driverName].DeviceHandler != null)
            {
                return _deviceInfoList[driverName].DeviceHandler.IsDevMode();
            }
            else
            {
                return Interface.eDevMode.UNKNOWN;
            }
        }

    }
}
