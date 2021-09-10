using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.Management;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Data;
using INNO6.Core;
using INNO6.Core.Threading;
using INNO6.IO;
using CIM.Manager;
using CIM.Common;
using CIM.Host;
using CIM.Host.SECSII;

namespace CIM.Application
{
    public partial class StartUp
    {
        private string _configFilePath;
        private string _dbFilePath;
        private DataManager _dataManager;
        private SECSDriverBase _secsDriver;
        private WorkQueue _workQueue; 
        private int[] stats;
        private TimeSpan refreshInterval;
        private DateTime nextRefreshTime;

        private bool _isRunResetCheck;

        private Thread _resetCheckThread;
        private Task _resetCheckTask;


        public static readonly StartUp Instance = new StartUp();

        public string ConfigFilePath { set { _configFilePath = value; } }
        public string DbFilePath { set { _dbFilePath = value; } }

        private StartUp()
        {
           
        }

        public void Inialize()
        {         
            List<string> localIPs = GetLocalIPAddress(0);

            ObjectQuery oq = new ObjectQuery("SELECT * FROM Win32_NetworkAdapter");
            ManagementObjectSearcher query1 = new ManagementObjectSearcher(oq);

            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();

            string getEquipmentInfo = @"SELECT * FROM master_equipment";
            DataTable eqpData = DbHandler.Instance.ExecuteQuery(_dbFilePath, getEquipmentInfo);
            LogHelper.Instance.DBManagerLog.DebugFormat("[INFO] master_equipment Table ExecuteQuery Success");

            string getModuleInfo = @"SELECT * FROM master_module";
            DataTable moduleData = DbHandler.Instance.ExecuteQuery(_dbFilePath, getModuleInfo);
            LogHelper.Instance.DBManagerLog.DebugFormat("[INFO] master_module Table ExecuteQuery Success");

            foreach(DataRow dr in moduleData.Rows)
            {
                MODULE module = new MODULE();
                module.MODULE_NAME = dr["Module"] as string;
                module.UNIT_ID = dr["UnitId"] as string;
                string type = dr["Type"] as string;

                if(type.Equals("NORMAL"))
                {
                    module.TYPE = eModuleType.NORMAL;
                }
                else if(type.Equals("MASTER"))
                {
                    module.TYPE = eModuleType.MASTER;
                }
                else if(type.Equals("LOADER"))
                {
                    module.TYPE = eModuleType.LOADER;
                }
                else if(type.Equals("UNLOADER"))
                {
                    module.TYPE = eModuleType.UNLOADER;
                }
                else
                {
                    module.TYPE = eModuleType.UNKNOWN;
                }

                module.MATERIAL_PORT_COUNT = Convert.ToInt32(dr["MaterialPortCount"]);
                module.AGV_PORT_COUNT = Convert.ToInt32(dr["AGVPortCount"]);
                module.TRACK_IN_COUNT = Convert.ToInt32(dr["TrackInCount"]);
                module.TRACK_OUT_COUNT = Convert.ToInt32(dr["TrackOutCount"]);
                module.DESCRIPTION = dr["Description"] as string;
                
                CommonData.Instance.MODULE_SETTINGS.Add(module);
            }

            bool macAddressNotFound = true;
            bool ipAddressNotFound = true;

            foreach (NetworkInterface adapter in adapters)
            {
                PhysicalAddress pa = adapter.GetPhysicalAddress();

                CommonData.Instance.EQP_SETTINGS.MACADDRESS = pa.ToString();
                if (string.IsNullOrEmpty(CommonData.Instance.EQP_SETTINGS.MACADDRESS)) continue;

                var eqpInfo = (from m in eqpData.AsEnumerable()
                               where m.Field<string>("MACADDRESS") == CommonData.Instance.EQP_SETTINGS.MACADDRESS
                               select m).FirstOrDefault();

                if (eqpInfo != null)
                {

                    DataRow dr = eqpInfo as DataRow;


                    CommonData.Instance.EQP_SETTINGS.EQPID = dr["EQPID"] as string;
                    CommonData.Instance.EQP_SETTINGS.EQPNAME = dr["EQPNAME"] as string;
                    CommonData.Instance.EQP_SETTINGS.IPADDRESS = dr["IPADDRESS"] as string;
                    CommonData.Instance.EQP_SETTINGS.MACADDRESS = dr["MACADDRESS"] as string;
                    CommonData.Instance.EQP_SETTINGS.MATERIALPORTCOUNT = Convert.ToInt32(dr["MATERIALPORTCOUNT"]);
                    CommonData.Instance.EQP_SETTINGS.LOADPORTCOUNT = Convert.ToInt32(dr["LOADPORTCOUNT"]);
                    CommonData.Instance.EQP_SETTINGS.CELLPORTCOUNT = Convert.ToInt32(dr["CELLPORTCOUNT"]);
                    CommonData.Instance.EQP_SETTINGS.IQC_USE = Convert.ToBoolean(dr["IQC_USE"]);
                    CommonData.Instance.EQP_SETTINGS.FA_SPEC_VERSION = dr["FA_SPEC_VERSION"] as string;
                    CommonData.Instance.EQP_SETTINGS.ONLINE_MAP_VERSION = dr["ONLINE_MAP_VERSION"] as string;
                    CommonData.Instance.EQP_SETTINGS.CIM_SW_VERSION = dr["CIM_SW_VERSION"] as string;
                    CommonData.Instance.EQP_SETTINGS.EQP_SW_VERSION = dr["EQP_SW_VERSION"] as string;
                    CommonData.Instance.EQP_SETTINGS.EXEC_CMD = dr["EXEC_CMD"] as string;
                    CommonData.Instance.EQP_SETTINGS.MACADDRESS = dr["MACADDRESS"] as string;

                    try
                    {
                        String ipAddr = getIpAddress(CommonData.Instance.EQP_SETTINGS.MACADDRESS);

                        if (!string.IsNullOrEmpty(ipAddr))
                        {
                            CommonData.Instance.EQP_SETTINGS.IPADDRESS = ipAddr;
                            string setQuery = string.Format(@"UPDATE master_equipment SET IPADDRESS = '{0}' WHERE IPADDRESS = '{1}'", ipAddr, CommonData.Instance.EQP_SETTINGS.MACADDRESS);
                            DbHandler.Instance.ExecuteQueryToWorkQueue(_dbFilePath, setQuery);
                            LogHelper.Instance.DBManagerLog.DebugFormat("[INFO] UPDATE master_equipment SET IPADDRESS = '{0}' WHERE IPADDRESS = '{1}'", ipAddr, CommonData.Instance.EQP_SETTINGS.MACADDRESS);
                        }

                    }
                    catch (Exception e)
                    {
                        LogHelper.Instance.ErrorLog.DebugFormat("[ERROR] Fail UPDATE master_equipment SET IPADDRESS" + e.Message);
                        throw e;                        
                    }
                    macAddressNotFound = false;

                    break;
                }
            }

            if (macAddressNotFound)
            {
                
                foreach (var item in localIPs)
                {
                    var address = (from m in eqpData.AsEnumerable()
                                   where m.Field<string>("IPADDRESS") == item
                                   select m).FirstOrDefault();



                    if (address != null)
                    {
                        ipAddressNotFound = false;
                        DataRow dr = address as DataRow;

                        CommonData.Instance.EQP_SETTINGS.EQPID = dr["EQPID"] as string;
                        CommonData.Instance.EQP_SETTINGS.EQPNAME = dr["EQPNAME"] as string;
                        CommonData.Instance.EQP_SETTINGS.IPADDRESS = dr["IPADDRESS"] as string;
                        CommonData.Instance.EQP_SETTINGS.MACADDRESS = dr["MACADDRESS"] as string;
                        CommonData.Instance.EQP_SETTINGS.MATERIALPORTCOUNT = Convert.ToInt32(dr["MATERIALPORTCOUNT"]);
                        CommonData.Instance.EQP_SETTINGS.LOADPORTCOUNT = Convert.ToInt32(dr["LOADPORTCOUNT"]);
                        CommonData.Instance.EQP_SETTINGS.CELLPORTCOUNT = Convert.ToInt32(dr["CELLPORTCOUNT"]);
                        CommonData.Instance.EQP_SETTINGS.IQC_USE = Convert.ToBoolean(dr["IQC_USE"]);
                        CommonData.Instance.EQP_SETTINGS.FA_SPEC_VERSION = dr["FA_SPEC_VERSION"] as string;
                        CommonData.Instance.EQP_SETTINGS.ONLINE_MAP_VERSION = dr["ONLINE_MAP_VERSION"] as string;
                        CommonData.Instance.EQP_SETTINGS.CIM_SW_VERSION = dr["CIM_SW_VERSION"] as string;
                        CommonData.Instance.EQP_SETTINGS.EQP_SW_VERSION = dr["EQP_SW_VERSION"] as string;
                        CommonData.Instance.EQP_SETTINGS.EXEC_CMD = dr["EXEC_CMD"] as string;
                        CommonData.Instance.EQP_SETTINGS.MACADDRESS = dr["MACADDRESS"] as string;

                        try
                        {
                            String macAddr = getMacAddress(item);

                            if (!string.IsNullOrEmpty(macAddr) && !macAddr.Equals(CommonData.Instance.EQP_SETTINGS.MACADDRESS))
                            {
                                CommonData.Instance.EQP_SETTINGS.MACADDRESS = macAddr;                             
                                string setQuery = string.Format(@"UPDATE master_equipment SET MACADDRESS = '{0}' WHERE IPADDRESS = '{1}'", macAddr, CommonData.Instance.EQP_SETTINGS.IPADDRESS);
                                DbHandler.Instance.ExecuteQueryToWorkQueue(_dbFilePath, setQuery);
                                LogHelper.Instance.DBManagerLog.DebugFormat("[INFO] UPDATE master_equipment SET MACADDRESS = '{0}' WHERE IPADDRESS = '{1}'", macAddr, CommonData.Instance.EQP_SETTINGS.IPADDRESS);
                            }
                        }
                        catch (Exception e)
                        {
                            LogHelper.Instance.ErrorLog.DebugFormat("[ERROR] Fail UPDATE master_equipment SET MACADDRESS" + e.Message);
                            throw e;
                        }
                        break;
                    }
                    
                }
            }

            if (ipAddressNotFound && macAddressNotFound)
            {
                LogHelper.Instance.DBManagerLog.DebugFormat("[ERROR] Not found Equipment informations");
                throw new Exception("IO_DB.mdb has not IpAddress or MacAddress");               
            }
            
        }

        private void ResetCheckThreadStart()
        {
            if (_isRunResetCheck)
                ResetCheckThreadStop();

            _resetCheckTask = new Task(ResetCheckMethod);

            _isRunResetCheck = true;
            _resetCheckTask.Start();

            //_resetCheckThread = null;
            //_resetCheckThread = new Thread(ResetCheckMethod);
            //_isRunResetCheck = true;
            //_resetCheckThread.Start();
            LogHelper.Instance._debug.DebugFormat("[INFO] _resetCheckThread.Start();");
        }

        void ResetCheckMethod()
        {
            while (_isRunResetCheck)
            {
                try
                {
                    var dataList = DataManager.Instance.DataAccess.RemoteObject.DataList.Where(o => (o.Direction != eDirection.IN
                        && o.DataResetTimeout > 0
                        && !string.IsNullOrEmpty(o.DefaultValue))
                        && o.DataSetTime != null
                        && Environment.TickCount - o.DataSetTime > o.DataResetTimeout).ToList();

                    if (dataList != null && dataList.Count > 0)
                    {
                        foreach (Data data in dataList)
                        {
                            data.DataSetTime = null;


                            switch (data.Type)
                            {
                                case eDataType.Int:
                                    {
                                        DataManager.Instance.SET_INT_DATA(data.Name, Convert.ToInt32(data.DefaultValue));
                                    }
                                    break;
                                case eDataType.Double:
                                    {
                                        DataManager.Instance.SET_DOUBLE_DATA(data.Name, Convert.ToDouble(data.DefaultValue));
                                    }
                                    break;
                                case eDataType.String:
                                    {
                                        DataManager.Instance.SET_STRING_DATA(data.Name, data.DefaultValue);
                                    }
                                    break;
                                case eDataType.Object:
                                    {
                                        DataManager.Instance.SET_DATA(data.Name, data.DefaultValue);
                                    }
                                    break;
                                default:
                                    {
                                        throw new Exception("UNKNOWN DATA TYPE EXCEPTION");
                                    }
                            }

                            LogHelper.Instance._debug.DebugFormat("[DEBUG] Data Reset Timeout : {0} / {1}", data.Name, data.StringValue);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Instance._debug.DebugFormat("[ERROR] Data Reset Thread : {0}", ex.Message);
                    
                }

                Thread.Sleep(100);
            }
        }

        private void ResetCheckThreadStop()
        {
            if (_isRunResetCheck != false && !_resetCheckThread.IsAlive)
            {
                _isRunResetCheck = false;
                _resetCheckTask.Dispose();
                _resetCheckThread = null;
                LogHelper.Instance._debug.DebugFormat("[DEBUG] (_isRunResetCheck != false && !_resetCheckThread.IsAlive), ResetuCheckThreadStop() Return.");
                return;
            }

            _isRunResetCheck = false;
            _resetCheckTask.Dispose();
            //_resetCheckThread.Join();
            _resetCheckThread = null;
            LogHelper.Instance._debug.DebugFormat("[DEBUG] _resetCheckThread.Join();");
        }

        public void Start()
        {
            bool plcWriteResult;
            stats = new int[6];
            nextRefreshTime = DateTime.Now;
            refreshInterval = TimeSpan.FromSeconds(1.0);

            _dataManager = DataManager.Instance;
            _dataManager.Initialize(_configFilePath);
                       
            _workQueue = WorkQueue.Instance;
            _workQueue.ConcurrentLimit = 10;         
            _workQueue.AllWorkCompleted += new EventHandler(work_AllWorkCompleted);
            _workQueue.WorkerException += new ResourceExceptionEventHandler(work_WorkerException);
            _workQueue.ChangedWorkItemState += new ChangedWorkItemStateEventHandler(work_ChangedWorkItemState);

            ResetCheckThreadStart();

            InitializeProcess.Instance.DbFilePath = _dbFilePath;
            AlarmManager.Instance.Initialize(_dbFilePath);
            //Alarm이 발생되었을때 실행할 함수를 등록
            AlarmManager.Instance.AlarmOccoured += new EventHandler(report_AlarmOccoured);
            
            MODULE module = CommonData.Instance.MODULE_SETTINGS.Where(m => m.TYPE == eModuleType.MASTER).FirstOrDefault();
            RMSManager.Instance.Initialize(_dbFilePath, DataManager.Instance.GET_STRING_DATA(string.Format("i{0}.EQStatus.PPID", module.MODULE_NAME), out plcWriteResult), module.MODULE_NAME);
            ECManager.Instance.Initialize(_dbFilePath);
            FDCManager.Instance.Initialize(_dbFilePath);
            FDCManager.Instance.TraceDataSend += report_TraceDataSend;
            DVManager.Instance.Initialize(_dbFilePath);
            AttributeManager.Instance.Initialize(_dbFilePath);
            InitializeProcess.Instance.InitializeRMSData();

            _secsDriver = new SECSDriver();
            _secsDriver.Initialize(_configFilePath);
            _secsDriver.Start();

            InializeDataOut();
        }

        private void RefreshCounts()
        {
            LogHelper.Instance._debug.DebugFormat("[INFO] WorkQueue Completed Count : {0}", stats[(int)WorkItemState.Completed].ToString("N0"));
            LogHelper.Instance._debug.DebugFormat("[INFO] WorkQueue Failing Count : {0}", stats[(int)WorkItemState.Failing].ToString("N0"));
            LogHelper.Instance._debug.DebugFormat("[INFO] WorkQueue Queued Count : {0}", stats[(int)WorkItemState.Queued].ToString("N0"));
            LogHelper.Instance._debug.DebugFormat("[INFO] WorkQueue Running Count : {0}", stats[(int)WorkItemState.Running].ToString("N0"));
            LogHelper.Instance._debug.DebugFormat("[INFO] WorkQueue Scheduled Count : {0}", stats[(int)WorkItemState.Scheduled].ToString("N0"));
            LogHelper.Instance._debug.DebugFormat("[INFO] WorkQueue Completed Count : {0}", stats[(int)WorkItemState.Completed].ToString("N0"));
        }

        public static String getIpAddress(String macaddress)
        {
            String queryStr = "SELECT * FROM Win32_NetworkAdapterConfiguration WHERE IPEnabled='TRUE'";
            ObjectQuery objectQuery = new ObjectQuery(queryStr);
            ManagementObjectSearcher searcher = new System.Management.ManagementObjectSearcher(objectQuery);
            ManagementObjectCollection mos = searcher.Get();

            String IpAddress = null;

            foreach (ManagementObject mo in mos)
            {
                if (String.IsNullOrEmpty(macaddress))
                {
                    String[] address = (String[])mo["IPAddress"];

                    IpAddress = address[0];
                    break;
                }
                else
                {
                    String mac = mo["MACAddress"].ToString();
                    String transMac = "";
                    String[] macArray = mac.Split(':');

                    foreach (String m in macArray)
                    {
                        transMac += m;
                    }

                    if (macaddress.Equals(transMac))
                    {
                        String[] address = (String[])mo["IPAddress"];
                        IpAddress = address[0];
                        break;
                    }
                }
            }
            LogHelper.Instance.DBManagerLog.DebugFormat("[DEBUG] Get IP Address {0}", IpAddress);
            return IpAddress;
        }

        public static String getMacAddress(String ipaddress)
        {
            String queryStr = "SELECT * FROM Win32_NetworkAdapterConfiguration WHERE IPEnabled='TRUE'";
            ObjectQuery objectQuery = new ObjectQuery(queryStr);
            ManagementObjectSearcher searcher = new System.Management.ManagementObjectSearcher(objectQuery);
            ManagementObjectCollection mos = searcher.Get();

            String macAddress = null;

            foreach (ManagementObject mo in mos)
            {
                if (String.IsNullOrEmpty(ipaddress))
                {
                    macAddress = mo["MACAddress"].ToString();
                    break;
                }
                else
                {
                    String[] address = (String[])mo["IPAddress"];

                    if (ipaddress.Equals(address[0]))
                    {
                        macAddress = mo["MACAddress"].ToString();
                        String transMac = "";
                        String[] macArray = macAddress.Split(':');

                        foreach (String m in macArray)
                        {
                            transMac += m;
                        }


                        macAddress = transMac;
                        break;
                    }
                }
            }
            LogHelper.Instance.DBManagerLog.DebugFormat("[DEBUG] Get MAC Address {0}", macAddress);
            return macAddress;
        }

        private List<string> GetLocalIPAddress(int index)
        {
            List<IPAddress> interNetworks = new List<IPAddress>();
            foreach (var item in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                if (item.AddressFamily == AddressFamily.InterNetwork)
                {
                    interNetworks.Add(item);
                }
            }

            List<string> allLocalIpAddress = new List<string>();
            foreach (var item in interNetworks)
                allLocalIpAddress.Add(item.ToString());

            LogHelper.Instance.DBManagerLog.DebugFormat("[INFO] Get Local IP Address {0}", allLocalIpAddress);
            LogHelper.Instance._debug.DebugFormat("[DEBUG] Get Local IP Address {0}", allLocalIpAddress);
            return allLocalIpAddress;
        }

        void InializeDataOut()
        {
            var dataList = DataManager.Instance.DataAccess.RemoteObject.DataList.Where(o => (o.Direction == eDirection.OUT && !string.IsNullOrEmpty(o.DefaultValue))).ToList();

            foreach (Data data in dataList)
            {
                data.DataSetTime = null;

                switch (data.Type)
                {
                    case eDataType.Int:
                        {
                            DataManager.Instance.SET_INT_DATA(data.Name, Convert.ToInt32(data.DefaultValue));
                        }
                        break;
                    case eDataType.Double:
                        {
                            DataManager.Instance.SET_DOUBLE_DATA(data.Name, Convert.ToDouble(data.DefaultValue));
                        }
                        break;
                    case eDataType.String:
                        {
                            DataManager.Instance.SET_STRING_DATA(data.Name, data.DefaultValue);
                        }
                        break;
                    case eDataType.Object:
                        {
                            DataManager.Instance.SET_DATA(data.Name, data.DefaultValue);
                        }
                        break;
                    default:
                        {
                            throw new Exception("UNKNOWN DATA TYPE EXCEPTION");
                        }
                }
            }
        }
    }
}
