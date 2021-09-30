using INNO6.Core.Manager.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using INNO6.Core;
using System.IO;

namespace INNO6.Core.Manager
{
    public class InterlockEventArgs : EventArgs
    {
        public string InterlockName { get; set; }
        public string InterlockMode { get; set; }

        public InterlockEventArgs(string interlockName, string interlockMode)
        {
            InterlockName = interlockName;
            InterlockMode = interlockMode;
        }
    }
    

    public class InterlockManager
    {
        public enum eDataType
        {
            Int,
            Double,
            String,
            Object,
        }

        public static string INTERLOCK_ON = "ON";
        public static string INTERLOCK_OFF = "OFF";

        public static string INTERLOCK_RESULT_FAIL = "FAIL";
        public static string INTERLOCK_RESULT_SUCCESS = "SUCCESS";
        public static string INTERLOCK_RESULT_NOTDEFINE = "NOTDEFINE";
        public static string INTERLOCK_RESULT_NOTUSE = "NOTUSE";

        private const string INTERLOCK_EXECUTE = "Execute";

        public static readonly InterlockManager Instance = new InterlockManager();
        private readonly object eventLock = new object();
        private string _dbPath;

        private SortedDictionary<string, List<INTERLOCK>> _InterlockListByIoName;

        private SortedDictionary<string, SETPOINT_INTERLOCK> _SetpointInterlockInfoList;
        private SortedDictionary<string, VALUE_INTERLOCK> _ValueInterlockInfoList;

        private SortedDictionary<string, object> _SetpointInterlockList = new SortedDictionary<string, object>();
        private SortedDictionary<string, object> _ValueInterlockList = new SortedDictionary<string, object>();

        private InterlockManager() { }

        private EventHandler _InterlockEvent;

        public void Initialize(string dbPath)
        {
            _dbPath = Path.GetFullPath(dbPath);

           
            _SetpointInterlockInfoList = GetSetpointInterlockInfoList();
            _ValueInterlockInfoList = GetValueInterlockInfoList();

            LoadAssemblySetpointInterlock();
            LoadAssemblyValueInterlock();

            _InterlockListByIoName = GetInterlockListByIoName();
        }

        public event EventHandler InterlockEvent
        {
            add
            {
                lock (eventLock)
                {
                    _InterlockEvent += value;
                }
            }
            remove
            {
                lock (eventLock)
                {
                    _InterlockEvent -= value;
                }
            }
        }

        private SortedDictionary<string, List<INTERLOCK>> GetInterlockListByIoName()
        {
            SortedDictionary<string, List<INTERLOCK>> interlocks = new SortedDictionary<string, List<INTERLOCK>>();

            foreach (VALUE_INTERLOCK value in _ValueInterlockInfoList.Values)
            {
                if(interlocks.ContainsKey(value.IoName))
                {
                    interlocks[value.IoName].Add(value);
                }
                else
                {
                    interlocks.Add(value.IoName, new List<INTERLOCK>() { value });
                }
            }


            foreach (SETPOINT_INTERLOCK value in _SetpointInterlockInfoList.Values)
            {
                if (interlocks.ContainsKey(value.IoName))
                {
                    interlocks[value.IoName].Add(value);
                }
                else
                {
                    interlocks.Add(value.IoName, new List<INTERLOCK>() { value });
                }
            }

            return interlocks;
        }

        private SortedDictionary<string, VALUE_INTERLOCK> GetValueInterlockInfoList()
        {
            string dbFilePath = _dbPath;
            string queryCommand = @"SELECT * FROM sys_interlock_value";

            SortedDictionary<string, VALUE_INTERLOCK> interlockList = new SortedDictionary<string, VALUE_INTERLOCK>();

            DataTable dt = DbHandler.Instance.ExecuteQuery(dbFilePath, queryCommand);

            foreach (DataRow dr in dt.Rows)
            {
                VALUE_INTERLOCK interlockInfo = new VALUE_INTERLOCK();

                interlockInfo.Name = dr["INTERLOCKNAME"] as string;
                interlockInfo.LowValue = dr["LOWVALUE"] as string;
                interlockInfo.HighValue = dr["HIGHVALUE"] as string;

                if (!Convert.IsDBNull(dr["USE"]) && (dr["USE"] as bool?) == true)
                {
                    interlockInfo.IsUse = true;
                }
                else
                {
                    interlockInfo.IsUse = false;
                }

                if (!Convert.IsDBNull(dr["NOTFLAG"]) && (dr["NOTFLAG"] as bool?) == true)
                {
                    interlockInfo.NotFlag = true;
                }
                else
                {
                    interlockInfo.NotFlag = false;
                }

                interlockInfo.IoName = dr["IoName"] as string;

                string status = string.Empty;
                if (Convert.IsDBNull(dr["Status"])) status = "OFF";
                else status = "ON";
                _ = status.Contains("OFF") ? interlockInfo.Status = "OFF" : interlockInfo.Status = "ON";
                interlockInfo.Status = dr["Status"] as string;

                interlockInfo.AssemblyName = dr["ASSEMBLYNAME"] as string;
                interlockInfo.DllFilePath = dr["DLLFILEPATH"] as string;
                interlockInfo.Description = dr["DESCRIPTION"] as string;
                interlockInfo.Type = "VALUE";
                interlockList.Add(interlockInfo.Name, interlockInfo);
            }

            return interlockList;
        }

        private SortedDictionary<string, SETPOINT_INTERLOCK> GetSetpointInterlockInfoList()
        {
            string dbFilePath = _dbPath;
            string queryCommand = @"SELECT * FROM sys_interlock_setpoint";

            SortedDictionary<string, SETPOINT_INTERLOCK> interlockList = new SortedDictionary<string, SETPOINT_INTERLOCK>();

            DataTable dt = DbHandler.Instance.ExecuteQuery(dbFilePath, queryCommand);

            foreach (DataRow dr in dt.Rows)
            {
                SETPOINT_INTERLOCK interlockInfo = new SETPOINT_INTERLOCK();

                interlockInfo.Name = dr["INTERLOCKNAME"] as string;
                interlockInfo.SetPoint = dr["SETPOINT"] as string;

                string status = string.Empty;

                if (Convert.IsDBNull(dr["Status"])) status = "OFF";
                else status = "ON";

                _ = status.Contains("OFF") ? interlockInfo.Status = "OFF" : interlockInfo.Status = "ON";

                interlockInfo.IoName = dr["IoName"] as string;

                if (!Convert.IsDBNull(dr["USE"]) && (dr["USE"] as bool?) == true)
                {
                    interlockInfo.IsUse = true;
                }
                else
                {
                    interlockInfo.IsUse = false;
                }

                if (!Convert.IsDBNull(dr["NOTFLAG"]) && (dr["NOTFLAG"] as bool?) == true)
                {
                    interlockInfo.NotFlag = true;
                }
                else
                {
                    interlockInfo.NotFlag = false;
                }


                interlockInfo.AssemblyName = dr["ASSEMBLYNAME"] as string;
                interlockInfo.DllFilePath = dr["DLLFILEPATH"] as string;
                interlockInfo.Description = dr["DESCRIPTION"] as string;
                interlockInfo.Type = "SETPOINT";
                interlockList.Add(interlockInfo.Name, interlockInfo);
            }

            return interlockList;
        }

        private void LoadAssemblySetpointInterlock()
        {
            foreach (SETPOINT_INTERLOCK info in _SetpointInterlockInfoList.Values)
            {
                string fullpath = Path.GetFullPath(info.DllFilePath);
                Assembly assembly = Assembly.LoadFile(fullpath);
                object instance = assembly.CreateInstance(info.AssemblyName);
                if (instance == null)
                {
                    LogHelper.Instance.ErrorLog.ErrorFormat("[ERROR] AssemblyName cannot find : {0}", info.AssemblyName);
                    continue;
                }
                Type type = instance.GetType();
                _SetpointInterlockList.Add(info.Name, instance);
            }
        }

        private void LoadAssemblyValueInterlock()
        {
            foreach (VALUE_INTERLOCK info in _ValueInterlockInfoList.Values)
            {
                string fullpath = Path.GetFullPath(info.DllFilePath);
                Assembly assembly = Assembly.LoadFile(fullpath);
                object instance = assembly.CreateInstance(info.AssemblyName);
                if (instance == null)
                {
                    LogHelper.Instance.ErrorLog.ErrorFormat("[ERROR] AssemblyName cannot find : {0}", info.AssemblyName);
                    continue;
                }
                Type type = instance.GetType();
                _ValueInterlockList.Add(info.Name, instance);
            }
        }

        private bool SetpointInterlockExecute(string interlockName, object setValue)
        {
            if (_SetpointInterlockList.ContainsKey(interlockName) && _SetpointInterlockInfoList.ContainsKey(interlockName))
            {
                if (_SetpointInterlockInfoList[interlockName].Status == INTERLOCK_ON) return false;

                object instance = _SetpointInterlockList[interlockName];
                Type type = instance.GetType();
                MethodInfo interlockExecute = type.GetMethod(INTERLOCK_EXECUTE);
                object returnValue = interlockExecute.Invoke(instance, new object[] { setValue });
                if((bool)returnValue)
                {
                    _InterlockEvent?.Invoke(this, new InterlockEventArgs(interlockName, "SETPOINT"));
                    _SetpointInterlockInfoList[interlockName].Status = INTERLOCK_ON;
                }
                    

                return (bool)returnValue;
            }
            else
            {
                return false;
            }
        }

        private bool ValueInterlockExecute(string interlockName, object setValue)
        {
            if (_ValueInterlockList.ContainsKey(interlockName) && _ValueInterlockInfoList.ContainsKey(interlockName))
            {
                if (_ValueInterlockInfoList[interlockName].Status == INTERLOCK_ON) return false;

                object instance = _ValueInterlockList[interlockName];
                Type type = instance.GetType();
                MethodInfo interlockExecute = type.GetMethod(INTERLOCK_EXECUTE);
                object returnValue = interlockExecute.Invoke(instance, new object[] { setValue });

                if ((bool)returnValue)
                {
                    _InterlockEvent?.Invoke(this, new InterlockEventArgs(interlockName, "VALUE"));
                    _ValueInterlockInfoList[interlockName].Status = INTERLOCK_ON;
                }
                   

                return (bool)returnValue;
            }
            else
            {
                return false;
            }

        }

        public List<INTERLOCK> GET_INTERLOCK_LIST(string ioName)
        {
            if(_InterlockListByIoName.ContainsKey(ioName))
            {
                return _InterlockListByIoName[ioName];
            }

            return null;
        }

        public void SET_SETPOINT_INTERLOCK_USE(string interlockName, bool use)
        {
            if (_SetpointInterlockInfoList.ContainsKey(interlockName))
            {
                SETPOINT_INTERLOCK interlock_info = _SetpointInterlockInfoList[interlockName];
                interlock_info.IsUse = use;
            }
        }

        public void SET_VALUE_INTERLOCK_USE(string interlockName, bool use)
        {
            if (_ValueInterlockInfoList.ContainsKey(interlockName))
            {
                VALUE_INTERLOCK interlock_info = _ValueInterlockInfoList[interlockName];
                interlock_info.IsUse = use;
            }
        }

        public bool GET_SETPOINT_INTERLOCK_USE(string interlockName)
        {
            if(_SetpointInterlockInfoList.ContainsKey(interlockName))
            {
                SETPOINT_INTERLOCK interlock_info = _SetpointInterlockInfoList[interlockName];
                return interlock_info.IsUse;
            }
            else
            {
                return false;
            }
        }

        public bool GET_VALUE_INTERLOCK_USE(string interlockName)
        {
            if (_ValueInterlockInfoList.ContainsKey(interlockName))
            {
                VALUE_INTERLOCK interlock_info = _ValueInterlockInfoList[interlockName];
                return interlock_info.IsUse;
            }
            else
            {
                return false;
            }
        }

        public bool SETPOINT_INTERLOCK(string interlockName, object setValue, int dataType)
        {
            bool notFlag = _SetpointInterlockInfoList[interlockName].NotFlag;
            if (!_SetpointInterlockInfoList.ContainsKey(interlockName) || !_SetpointInterlockList.ContainsKey(interlockName))
            {
                LogHelper.Instance.SystemLog.DebugFormat("[ERROR] INTERLOCK NOT DEFINE : INTERLOCK={0}", interlockName);
                return false;
            }

            if (_SetpointInterlockInfoList[interlockName].IsUse == false)
            {
                LogHelper.Instance.SystemLog.InfoFormat("[INFO] INTERLOCK NOT USE : INTERLOCK={0}", interlockName);
                return false;
            }

            if (_SetpointInterlockInfoList[interlockName].Status == INTERLOCK_ON) return true;


            switch (dataType)
            {
                case (int)eDataType.Int:
                    {
                        if(int.TryParse(_SetpointInterlockInfoList[interlockName].SetPoint, out int setPoint))
                        {
                            if ( !notFlag && setPoint.Equals(setValue))
                            {
                                return SetpointInterlockExecute(interlockName, setValue);
                            }
                            else if (notFlag && !setPoint.Equals(setValue))
                            {  
                                return SetpointInterlockExecute(interlockName, setValue);
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                case (int)eDataType.Double:
                    {
                        if (double.TryParse(_SetpointInterlockInfoList[interlockName].SetPoint, out double setPoint))
                        {
                            if (!notFlag && setPoint.Equals(setValue))
                            {                              
                                return SetpointInterlockExecute(interlockName, setValue);
                            }
                            else if (notFlag && !setPoint.Equals(setValue))
                            {                                
                                return SetpointInterlockExecute(interlockName, setValue);
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                case (int)eDataType.String:
                    {
                        if (!notFlag && _SetpointInterlockInfoList[interlockName].SetPoint.Equals(setValue))
                        {                            
                            return SetpointInterlockExecute(interlockName, setValue);
                        }
                        else if (notFlag && !_SetpointInterlockInfoList[interlockName].SetPoint.Equals(setValue))
                        {   
                            return SetpointInterlockExecute(interlockName, setValue);
                        }
                        else
                        { 
                            return false; 
                        }
                    }
                case (int)eDataType.Object:
                    {
                        if (!notFlag && _SetpointInterlockInfoList[interlockName].SetPoint.Equals(setValue))
                        {
                            return SetpointInterlockExecute(interlockName, setValue);
                        }
                        else if (notFlag && !_SetpointInterlockInfoList[interlockName].SetPoint.Equals(setValue))
                        {                           
                            return SetpointInterlockExecute(interlockName, setValue);
                        }
                        else
                        {
                            return false;
                        }
                    }
                default:
                    {
                        return false;
                    }
            }           
        }

        public bool VALUE_INTERLOCK(string interlockName, object setValue, int dataType)
        {
            bool notFlag = _ValueInterlockInfoList[interlockName].NotFlag;

            if (!_ValueInterlockInfoList.ContainsKey(interlockName) || !_ValueInterlockInfoList.ContainsKey(interlockName))
            {
                LogHelper.Instance.SystemLog.DebugFormat("[ERROR] INTERLOCK NOT DEFINE : INTERLOCK={0}", interlockName);
                return false;
            }

            if (_ValueInterlockInfoList[interlockName].IsUse == false)
            {
                LogHelper.Instance.SystemLog.InfoFormat("[INFO] INTERLOCK NOT USE : INTERLOCK={0}", interlockName);
                return false;
            }

            if (_ValueInterlockInfoList[interlockName].Status == INTERLOCK_ON) return true;

            switch (dataType)
            {
                case (int)eDataType.Int:
                    {
                        if (int.TryParse(_ValueInterlockInfoList[interlockName].HighValue, out int highValue) &&
                            int.TryParse(_ValueInterlockInfoList[interlockName].LowValue, out int lowValue))
                        {
                            int compareValue = (int)setValue;

                            if (!notFlag && (lowValue > compareValue || highValue < compareValue))
                            {
                                
                                return ValueInterlockExecute(interlockName, setValue);
                            }
                            else if (notFlag && !(lowValue > compareValue || highValue < compareValue))
                            {                                
                                return ValueInterlockExecute(interlockName, setValue);
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                case (int)eDataType.Double:
                    {
                        if (double.TryParse(_ValueInterlockInfoList[interlockName].HighValue, out double highValue) &&
                            double.TryParse(_ValueInterlockInfoList[interlockName].LowValue, out double lowValue))
                        {
                            double compareValue = (double)setValue;

                            if (!notFlag && (lowValue > compareValue || highValue < compareValue))
                            {
                                return ValueInterlockExecute(interlockName, setValue);
                            }
                            else if (notFlag && !(lowValue > compareValue || highValue < compareValue))
                            {
                                return ValueInterlockExecute(interlockName, setValue);
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }   
                default:
                    {
                        return false;
                    }
            }
        }

        public void INTERLOCK_RESET()
        {
            foreach(var setpoint in this._SetpointInterlockInfoList.Values)
            {
                if (setpoint.Status == INTERLOCK_ON) setpoint.Status = INTERLOCK_OFF;
            }

            foreach (var value in this._ValueInterlockInfoList.Values)
            {
                if (value.Status == INTERLOCK_ON) value.Status = INTERLOCK_OFF;
            }
        }
    }
}
