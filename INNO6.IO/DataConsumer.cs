using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using INNO6.Core;
using INNO6.IO.Remote;
using INNO6.IO;

namespace INNO6.IO.Service
{
    public class DataConsumer : IDisposable
    {       
        private eRemoteConnectionMode _rmtMode;
        private string _ipAddress;
        private string _port;
        private ConfigManager _config;
        private RemoteClient _rmtClient;
        private string _objectUri;
        private MemoryAccess _dataAccess;
        public static readonly DataConsumer Instance = new DataConsumer();

        public bool Initialized { get; private set; }

        public eRemoteConnectionMode RmtMode
        {
            get { return _rmtMode; }
            set { _rmtMode = value; }
        }

        public string IpAddress { get { return _ipAddress; } set { _ipAddress = value; } }
        public string Port { get { return _port; } set { _port = value; } }

        private DataConsumer()
        { 
        }

        public void Initialize(string configFilePath)
        {
            try
            {
                _config = new ConfigManager(configFilePath);
                if (_config.GetIniValue("CLIENT", "MODE") == "TCP") RmtMode = eRemoteConnectionMode.TCP;
                else RmtMode = eRemoteConnectionMode.IPC;

                _port = _config.GetIniValue("CLIENT", "PORT");
                _objectUri = _config.GetIniValue("CLIENT", "URI");
                _rmtClient = new RemoteClient(_objectUri);

                if (!_rmtClient.Connect(RmtMode, _ipAddress, _port))
                {
                    Initialized = false;
                }
                _dataAccess = new MemoryAccess(_rmtClient.RemoteObject);

                Initialized = true;
                LogHelper.Instance.SystemLog.DebugFormat("[Initialize] Initialize Success. : {0}", configFilePath);
            }
            catch(Exception ex)
            {
                LogHelper.Instance.ErrorLog.DebugFormat("[ERROR] Data Consumer Initialize Error : {0}", ex.Message);
                Initialized = false;
            }

        }

        #region IDisposable 멤버

        public void Dispose()
        {
        }

        #endregion

        public bool SET_INT_DATA(string name, int setValue)
        {
            return _dataAccess.SET_INT_DATA(name, setValue);
        }

        public bool SET_DOUBLE_DATA(string name, double setValue)
        {
            return _dataAccess.SET_DOUBLE_DATA(name, setValue);
        }

        public bool SET_STRING_DATA(string name, string setValue)
        {
            return _dataAccess.SET_STRING_DATA(name, setValue);
        }

        public bool SET_OBJECT_DATA(string name, object setValue)
        {
            return _dataAccess.SET_OBJECT_DATA(name, setValue);
        }

        public int GET_INT_DATA(string name)
        {
            bool result;
            var data = _dataAccess.GET_INT_DATA(name, out result);

            if (result) return data;
            else return 0;
        }

        public double GET_DOUBLE_DATA(string name)
        {
            bool result;
            var data = _dataAccess.GET_DOUBLE_DATA(name, out result);

            if (result) return data;
            else return 0;
        }

        public string GET_STRING_DATA(string name)
        {
            bool result;
            var data = _dataAccess.GET_STRING_DATA(name, out result);

            if (result) return data;
            else return string.Empty;
        }

        public object GET_OBJECT_DATA(string name)
        {
            bool result;
            var data = _dataAccess.GET_OBJECT_DATA(name, out result);

            if (result) return data;
            else return 0;
        }
    }
}
