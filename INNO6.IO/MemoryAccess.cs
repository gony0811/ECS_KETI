using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using INNO6.IO.Remote;
using INNO6.IO.Interface;
using INNO6.Core;

namespace INNO6.IO
{
    public class DataChangedEventHandlerArgs
    {
        public Data Data { get; set; }

        public DataChangedEventHandlerArgs(Data data)
        {
            Data = data;
        }
    }

    public class MemoryAccess : IMemoryAccess
    {
        protected static RemoteObject _rmtObj;
        
        public EventHandler<DataChangedEventHandlerArgs> DataChangedEvent;
        private static object _key = new object();
        private BlockingCollection<Data> _dataChangedQueue = new BlockingCollection<Data>(new ConcurrentQueue<Data>());
        public bool WatchDogThreadRunning { get; private set; }

        public MemoryAccess()
        {
            WatchDogThreadRunning = true;
            ThreadPool.QueueUserWorkItem(DataChangedWatchDog, this);
        }

        public BlockingCollection<Data> DataChangedQueue
        {
            get { return _dataChangedQueue; }
        }

        public MemoryAccess(RemoteObject remoteObject)
        {
            _rmtObj = remoteObject;
        }

        public RemoteObject RemoteObject
        {
            get
            {
                return _rmtObj;
            }

            set
            {
                _rmtObj = value;
            }
        }

        private void DataChangedWatchDog(object args)
        {
            while (WatchDogThreadRunning)
            {

                Data changedData;
                if (DataChangedQueue.TryTake(out changedData, -1))
                {
                    if (DataChangedEvent != null)
                    {
                        DataChangedEvent(this, new DataChangedEventHandlerArgs(changedData));
                    }
                }

                Thread.Sleep(10);
            }
        }

        public bool IsDataLogging(string name)
        {
            Data data = RemoteObject.DataList.Where(t => (t.Name == name)).FirstOrDefault();

            if (data == null) return false;

            return data.Logging;
        }

        #region IDataAccess 멤버
        public virtual bool SET_INT_DATA(string name, int value)
        {
            try
            {

                Data data;
                bool result = true;

                if (!_rmtObj.GetData(name, out data) || data.Type != eDataType.Int)
                {
                    LogHelper.Instance.SystemLog.DebugFormat("[ERROR] SET_INT_DATA - Can't Set Data.");
                    return false;
                }

                if (!data.Value.Equals(value))
                {                   
                    result = _rmtObj.SetValue(name, value);

                    if (DataChangedEvent != null && data.PollingTime > 0)
                    {
                        DataChangedQueue.Add(new Data(data));
                    }
                }
                return result;
            }

            catch (Exception ex)
            {
                LogHelper.Instance.ErrorLog.DebugFormat("[ERROR] Data Access Failed : {0}", ex.Message);
                return false;
            }
        }

        public virtual int GET_INT_DATA(string name, out bool result)
        {
            try
            {
                Data data;
                int value;

                if (!_rmtObj.GetData(name, out data) || data.Type != eDataType.Int)
                {
                    LogHelper.Instance.SystemLog.DebugFormat("[ERROR] GET_INT_DATA - Can't Get Data.");
                    result = false;
                    return 0;
                }

                if (data.Value == null)
                {
                    if (int.TryParse(data.DefaultValue, out value)) data.Value = (int)value;
                    else data.Value = 0;
                }


                result = true;
                return (int)data.Value;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.ErrorLog.DebugFormat("[Error] Data Access Failed : {0}", ex.Message);
                result = false;
                return 0;
            }

        }

        public virtual bool SET_DOUBLE_DATA(string name, double value)
        {
            try
            {
                bool result = true;
                Data data;

                if (!_rmtObj.GetData(name, out data) || data.Type != eDataType.Double)
                {
                    LogHelper.Instance.SystemLog.DebugFormat("[ERROR] SET_DOUBLE_DATA - Can't Set Data.");
                    return result;
                }

                if (!data.Value.Equals(value))
                {
                    result = _rmtObj.SetValue(name, value);
                    if (DataChangedEvent != null && data.PollingTime > 0)
                    {
                        //DataChanged(new Data(data));
                        DataChangedQueue.Add(new Data(data));
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.ErrorLog.DebugFormat("[Error] Data Access Failed : {0}", ex.Message);
                return false;
            }
            
        }

        public virtual double GET_DOUBLE_DATA(string name, out bool result)
        {
            try
            {
                Data data;
                double value = 0.0;

                if (!_rmtObj.GetData(name, out data) || data.Type != eDataType.Double)
                {
                    LogHelper.Instance.SystemLog.DebugFormat("[ERROR] GET_DOUBLE_DATA - Can't Get Data.");
                    result = false;
                    return 0.0;
                }

                if (data.Value == null)
                {
                    if (double.TryParse(data.DefaultValue, out value)) data.Value = value;
                    else data.Value = 0.0;
                }

                result = true;
                return (double)data.Value;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.ErrorLog.DebugFormat("[Error] Data Access Failed : {0}", ex.Message);
                result = false;
                return 0.0;
            }
        }

        public virtual bool SET_STRING_DATA(string name, string value)
        {
            try
            {
                bool result = true;
                Data data;

                if (!_rmtObj.GetData(name, out data) || data.Type != eDataType.String)
                {
                    LogHelper.Instance.SystemLog.DebugFormat("[ERROR] SET_STRING_DATA - Can't Set Data.");
                    return result;
                }

                if (!data.Value.Equals(value))
                {
                    result = _rmtObj.SetValue(name, value);

                    if (DataChangedEvent != null && data.PollingTime > 0)
                    {
                        DataChangedQueue.Add(new Data(data));
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.ErrorLog.DebugFormat("[ERROR] Data Access Failed : {0}", ex.Message);
                return false;
            }
        }

        public virtual string GET_STRING_DATA(string name, out bool result)
        {
            try
            {
                Data data;
                string value = string.Empty;

                if (!_rmtObj.GetData(name, out data) || data.Type != eDataType.String)
                {
                    LogHelper.Instance.SystemLog.DebugFormat("[ERROR] GET_STRING_DATA - Can't Get Data.");
                    result = false;
                    return string.Empty;
                }

                if (data.Value == null)
                {
                    if (data.DefaultValue != null) data.Value = data.DefaultValue;
                    else data.Value = string.Empty;
                }
  
                result = true;
                return (string)data.Value;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.ErrorLog.DebugFormat("[ERROR] Data Access Failed : {0}", ex.Message);
                result = false;
                return "";
            }
        }

        public virtual bool SET_OBJECT_DATA(string name, object value)
        {
            try
            {
                bool result = true;
                Data data;

                if (!_rmtObj.GetData(name, out data) || data.Type != eDataType.Object) return result;

                if (!data.Value.Equals(value))
                {
                    result = _rmtObj.SetValue(name, value);

                    if (DataChangedEvent != null && data.PollingTime > 0)
                    {
                        //Console.WriteLine("[DataChanged] {0} : {1}", name, value);                           
                        DataChangedQueue.Add(new Data(data));
                    }
                }



                return result;
            }
            catch (Exception ex)
            {
                //Console.WriteLine("[Error] Data Access Failed : {0}", ex.Message);
                LogHelper.Instance.ErrorLog.DebugFormat("[ERROR] Data Access Failed : {0}", ex.Message);
                return false;
            }
        }


        public virtual object GET_OBJECT_DATA(string name, out bool result)
        {
            try
            {
                Data data;
                string value = string.Empty;

                if (!_rmtObj.GetData(name, out data) || data.Type != eDataType.Object)
                {
                    result = false;
                    return string.Empty;
                }

                if (data.Value == null)
                {
                    if (data.DefaultValue != null) data.Value = data.DefaultValue;
                    else data.Value = string.Empty;
                }

                result = true;
                return data.Value;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.ErrorLog.DebugFormat("[ERROR] Data Access Failed : {0}", ex.Message);
                result = false;
                return null;
            }
        }
        #endregion
    }
}
