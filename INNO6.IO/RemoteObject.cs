using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using INNO6.IO;
using INNO6.Core;

namespace INNO6.IO.Remote
{
    public delegate void DataChangedEvent(string key, object data);

    public class RemoteObject : MarshalByRefObject
    {
        public DataChangedEvent DataChanged;
        public List<Data> DataList;

        public RemoteObject()
        {
        }

        public bool Add(string name, Data data)
        {
            if (DataConfigList.ContainsKey(name))
            {
                LogHelper.Instance.SystemLog.DebugFormat("[ERROR] Add - Don't have {0}", name);
                return false;
            }

            DataConfigList.Add(name, data);
            DataList = DataConfigList.Values.ToList();
            return true;
        }

        public bool ContainsKey(string name)
        {
            return DataConfigList.ContainsKey(name);
        }

        public bool GetData(string name, out Data data)
        {
            if (!DataConfigList.ContainsKey(name))
            {
                LogHelper.Instance.SystemLog.DebugFormat("[ERROR] GetData - Don't have {0}", name);
                data = null;
                return false;
            }

            data = DataConfigList[name];
            return true;
        }

        public bool SetData(string name, Data data)
        {
            if (!DataConfigList.ContainsKey(name))
            {
                LogHelper.Instance.SystemLog.DebugFormat("[ERROR] SetData - Don't have {0}", name);
                return false;
            }

            DataConfigList[name] = data;
            return true;
        }

        public string GetDataId(string name)
        {
            if (!DataConfigList.ContainsKey(name))
            {
                LogHelper.Instance.SystemLog.DebugFormat("[ERROR] GetDataId - Don't have {0}", name);
                return "";
            }
            return DataConfigList[name].Name;
        }

        public object GetValue(string name)
        {
            if (!DataConfigList.ContainsKey(name))
            {
                LogHelper.Instance.SystemLog.DebugFormat("[ERROR] GetValue - Don't have {0}", name);
                return null;
            }
            return DataConfigList[name].Value;
        }

        public bool SetValue(string name, object value)
        {
            if (!DataConfigList.ContainsKey(name))
            {
                LogHelper.Instance.SystemLog.DebugFormat("[ERROR] SetValue - Don't have {0}", name);
                return false;
            }

            DataConfigList[name].Value = value;
            //DataConfigList[name].CheckTime = DateTime.Now;

            if (value.ToString() != DataConfigList[name].DefaultValue && DataConfigList[name].DataResetTimeout > 0)
                DataConfigList[name].DataSetTime = Environment.TickCount;
            else
                DataConfigList[name].DataSetTime = null;

            return true;
        }

        public int GetListCount()
        {
            return DataConfigList.Count;
        }

        private static SortedList<string, Data> DataConfigList = new SortedList<string, Data>();
    }  
}
