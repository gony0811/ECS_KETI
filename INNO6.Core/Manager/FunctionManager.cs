using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using INNO6.Core.Manager.Model;
using INNO6.Core.Function;
using System.Data;
using System.Reflection;
using System.Threading;
using System.Collections.Concurrent;
using System.IO;

namespace INNO6.Core.Manager
{
    public class FunctionManager
    {
        public delegate void ExecuteResultCallback(string functionName, object result);

        public enum FunctionStatus
        {
            UNKNOWN = 0,
            SUCCESS = 1,
            FAIL = 2,
            RUNNING = 3,
        }

        public static string FUNC_RESULT_FAIL = "FAIL";
        public static string FUNC_RESULT_SUCCESS = "SUCCESS";
        public static string FUNC_RESULT_NOTDEFINE = "NOTDEFINE";
        public static string FUNC_RESULT_TIMEOUT = "TIMEOUT";
        public static string FUNC_RESULT_NOTUSE = "NOTUSE";
        public static string FUNC_RESULT_CANNOT_EXECUTE = "CANNOTEXECUTE";
        public static string FUNC_RESULT_AREADY_EXECUTE = "AREADYEXECUTE";

        private const string FUNCTION_EXECUTE = "Execute";
        private const string FUNCTION_CANEXECUTE = "CanExecute";
        private const string FUNCTION_POSTEXECUTRE = "PostExecute";

        public static readonly FunctionManager Instance = new FunctionManager();
        private readonly object eventLock = new object();
        private string _dbPath;
        private SortedDictionary<string, FUNCTION_INFO> _functionInfoList;
        private SortedDictionary<string, object> _functionList = new SortedDictionary<string, object>();
        private ConcurrentDictionary<string, Task> _taskList = new ConcurrentDictionary<string, Task>();
        private Task _taskManagerThread;
        private bool _threadRunFlag = true;

        private FunctionManager() { }

        public void Initialize(string dbPath)
        {

            _dbPath = Path.GetFullPath(dbPath);
            _threadRunFlag = true;
            _functionInfoList = GetAllFunctionInfoList();

            LoadAssembly();

            _taskManagerThread = new Task(ThreadTaskJobManaging);
            _taskManagerThread.Start();
        }

        public void Deinitialize()
        {
            _threadRunFlag = false;

            if (_taskManagerThread.Status == TaskStatus.Running)
            {
                _taskManagerThread.Wait();
                _taskManagerThread.Dispose();
            }              
        }

        private void ThreadTaskJobManaging()
        {
            while (_threadRunFlag)
            {
                foreach (string executeName in _taskList.Keys)
                {
                    if (_taskList[executeName].Status == TaskStatus.Running ||                      
                        _taskList[executeName].Status == TaskStatus.WaitingForActivation || 
                        _taskList[executeName].Status == TaskStatus.WaitingToRun ||
                        _taskList[executeName].Status == TaskStatus.WaitingForChildrenToComplete)
                    {
                        continue;
                    }
                    else if (_taskList[executeName].Status == TaskStatus.RanToCompletion)
                    {
                        if(_taskList.TryRemove(executeName, out Task task))
                        {
                            LogHelper.Instance.SystemLog.DebugFormat("[FUNCTION] EXECUTE(ASYNC-MODE) SUCCESS : FUNCTION={0}", executeName);
                        }                       
                    }
                    else
                    {
                        if(_taskList.TryRemove(executeName, out Task task))
                        {
                            LogHelper.Instance.SystemLog.DebugFormat("[FUNCTION] EXECUTE(ASYNC-MODE) UNKNOWN FAULT : FUNCTION={0}", executeName);
                        }                       
                    }
                }

                Thread.Sleep(10);
            }
        }

        public void ABORT_FUNCTION_ALL()
        {
            foreach(var T in _taskList)
            {
                object instance = _functionList[T.Key];
                Type type = instance.GetType();
                PropertyInfo property = type.GetProperty("Abort");
                property.SetValue(instance, true);
            }
        }

        public bool ABORT_FUNCTION(string executeName)
        {
            if (CHECK_EXECUTING_FUNCTION_EXSIST(executeName))
            {
                object instance = _functionList[executeName];
                Type type =instance.GetType();
                PropertyInfo property = type.GetProperty("Abort");
                property.SetValue(instance, true);

                return true;
            }

            return false;
        }
        public bool CHECK_EXECUTING_FUNCTION_EXSIST(string executeName)
        {
            return _taskList.Keys.Contains(executeName);
        }

        public int GET_FUNCTION_PROGRESS(string executeName)
        {
            if (CHECK_EXECUTING_FUNCTION_EXSIST(executeName))
            {
                object instance = _functionList[executeName];
                Type type = instance.GetType();
                PropertyInfo property = type.GetProperty("ProgressRate");
                object value = property.GetValue(instance);
                return (int)value;
            }
            else
            {
                return -1;
            }
        }

        public string GET_FUNCTION_PROCESS_MESSAGE(string executeName)
        {
            if (CHECK_EXECUTING_FUNCTION_EXSIST(executeName))
            {
                object instance = _functionList[executeName];
                Type type = instance.GetType();
                PropertyInfo property = type.GetProperty("ProcessingMessage");
                object value = property.GetValue(instance);
                return (string)value;
            }
            else
            {
                return string.Empty;
            }
        }



        public FunctionStatus CHECK_EXECUTING_FUNCTION_STATUS(string executeName)
        {
            if(CHECK_EXECUTING_FUNCTION_EXSIST(executeName))
            {
                switch(_taskList[executeName].Status)
                {
                    case TaskStatus.RanToCompletion:
                        {
                            return FunctionStatus.SUCCESS;
                        }
                    case TaskStatus.Running:
                        {
                            return FunctionStatus.RUNNING;
                        }
                    case TaskStatus.Faulted:
                        {
                            return FunctionStatus.FAIL;
                        }
                    default:
                        {
                            return FunctionStatus.UNKNOWN;
                        }
                }
            }
            else
            {
                return FunctionStatus.UNKNOWN;
            }
        }

        public string EXECUTE_FUNCTION_SYNC(string executeName)
        {
            lock (Instance)
            {
                bool resultCanExecute = false;
                string resultExecute = "";

                if (!_functionInfoList.ContainsKey(executeName) || !_functionList.ContainsKey(executeName))
                {
                    LogHelper.Instance.SystemLog.DebugFormat("[ERROR] FUNCTION NOT DEFINE : FUNCTION={0}", executeName);
                    return FUNC_RESULT_NOTDEFINE;
                }

                if (_functionInfoList[executeName].IsUse == false)
                {
                    LogHelper.Instance.SystemLog.DebugFormat("[ERROR] FUNCTION NOT USE : FUNCTION={0}", executeName);
                    return FUNC_RESULT_NOTUSE;
                }

                object instance = _functionList[executeName];

                Type type = instance.GetType();
                MethodInfo canExecute = type.GetMethod(FUNCTION_CANEXECUTE);
                resultCanExecute = (bool)canExecute.Invoke(instance, null);

                if (resultCanExecute)
                {
                    LogHelper.Instance.SystemLog.DebugFormat("[FUNCTION] EXECUTE(SYNC-MODE) : FUNCTION={0}", executeName);
                    MethodInfo execute = type.GetMethod(FUNCTION_EXECUTE);
                    resultExecute = (string)execute.Invoke(instance, null);
                    PropertyInfo propertyInfo = type.GetProperty("FunctionResult");
                    propertyInfo.SetValue(instance, resultExecute);
                    MethodInfo postExecute = type.GetMethod(FUNCTION_POSTEXECUTRE);
                    postExecute.Invoke(instance, null);

                    return resultExecute;
                }
                else
                {
                    LogHelper.Instance.SystemLog.DebugFormat("[FUNCTION] CANNOT EXECUTE(SYNC-MODE) : FUNCTION={0}", executeName);
                    return FUNC_RESULT_CANNOT_EXECUTE;
                }
            }
        }

        public void EXECUTE_FUNCTION_ASYNC(string executeName, ExecuteResultCallback executeResultCallback = null)
        {
            lock (Instance)
            {
                bool resultCanExecute = false;
                string resultExecute = "";

                if (!_functionInfoList.ContainsKey(executeName) || !_functionList.ContainsKey(executeName))
                {
                    LogHelper.Instance.SystemLog.DebugFormat("[ERROR] FUNCTION NOT DEFINE : FUNCTION={0}", executeName);
                    return;
                }

                if (_functionInfoList[executeName].IsUse == false)
                {
                    LogHelper.Instance.SystemLog.DebugFormat("[ERROR] FUNCTION NOT USE : FUNCTION={0}", executeName);
                    return;
                }

                object instance = _functionList[executeName];

                Type type = instance.GetType();
                MethodInfo canExecute = type.GetMethod(FUNCTION_CANEXECUTE);
                resultCanExecute = (bool)canExecute.Invoke(instance, null);

                if (_taskList.Keys.Contains(executeName))
                {
                    LogHelper.Instance.SystemLog.DebugFormat("[FUNCTION] AREADY EXECUTING FUNCTION (ASYNC-MODE) : FUNCTION={0}", executeName);
                    return;
                }

                LogHelper.Instance.SystemLog.DebugFormat("[FUNCTION] EXECUTE FUNCTION(ASYNC-MODE) : FUNCTION={0}", executeName);

                Task task = Task.Run(new Action(() =>
                {
                    if (resultCanExecute)
                    {
                        MethodInfo execute = type.GetMethod(FUNCTION_EXECUTE);
                        resultExecute = (string)execute.Invoke(instance, null);
                        if(executeResultCallback != null)
                            executeResultCallback(executeName, resultExecute);
                        MethodInfo postExecute = type.GetMethod(FUNCTION_POSTEXECUTRE);
                        postExecute.Invoke(instance, null);
                    }
                }));

                _taskList.TryAdd(executeName, task);

                return;
            }
        }

        private void LoadAssembly()
        {
            foreach (FUNCTION_INFO info in _functionInfoList.Values)
            {
                string fullPath = Path.GetFullPath(info.DllFilePath);
                Assembly assembly = Assembly.LoadFile(fullPath);
                object instance = assembly.CreateInstance(info.AssemblyName);

                if (instance == null) continue;

                Type type = instance.GetType();
                var propertyInfo = type.GetProperty("TimeoutMiliseconds");
                propertyInfo.SetValue(instance, info.TimeoutMiliseconds);
                _functionList.Add(info.ExecuteName, instance);
            }
        }

        private SortedDictionary<string, FUNCTION_INFO> GetAllFunctionInfoList()
        {
            string dbFilePath = _dbPath;
            string queryCommand = @"SELECT * FROM sys_function_info";

            SortedDictionary<string, FUNCTION_INFO> functionInfoList = new SortedDictionary<string, FUNCTION_INFO>();

            DataTable dt = DbHandler.Instance.ExecuteQuery(dbFilePath, queryCommand);

            foreach (DataRow dr in dt.Rows)
            {
                FUNCTION_INFO funcInfo = new FUNCTION_INFO();

                funcInfo.ExecuteName = dr["ExecuteName"] as string;

                string isAsyncMode = dr["IsAsyncMode"] as string;

                if (string.IsNullOrEmpty(isAsyncMode)) isAsyncMode = "N";

                if (isAsyncMode.Substring(0, 1).ToUpper().Equals("Y")) funcInfo.IsAsyncMode = true;
                else funcInfo.IsAsyncMode = false;

                funcInfo.DllFilePath = dr["DllFilePath"] as string;
                funcInfo.AssemblyName = dr["AssemblyName"] as string;

                string isUse = dr["IsUse"] as string;

                if (string.IsNullOrEmpty(isUse)) isUse = "N";

                if (isUse.Substring(0, 1).ToUpper().Equals("Y")) funcInfo.IsUse = true;
                else funcInfo.IsUse = false;

                string timeout = dr["TimeoutMiliseconds"] as string;
                int timeoutMiliseconds = 1000;

                if(int.TryParse(timeout, out timeoutMiliseconds))
                {
                    funcInfo.TimeoutMiliseconds = timeoutMiliseconds;
                }
                else
                {
                    funcInfo.TimeoutMiliseconds = 1000;
                }

                functionInfoList.Add(funcInfo.ExecuteName, funcInfo);
            }

            return functionInfoList;
        }
    }
}
