using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Data;
using INNO6.Core.Threading;
using INNO6.Core;
using INNO6.IO;
using System.Runtime.InteropServices;
using System.Collections.Concurrent;

namespace CIM.Manager
{
    public delegate void AlarmReportEvent(object sender, object param);

    public class AlarmManager
    {
        public static readonly AlarmManager Instance = new AlarmManager();
        private readonly object eventLock = new object();
        private string _dbPath;
        private uint _alarmHistMaxCount = 100;
        private List<ALARM> _alarmList;
        private List<ALARM_HISTORY> _alarmHistList = new List<ALARM_HISTORY>();
        
        private SortedList<string, bool[]> _currentAlarmBitList;
        
        public string DatabaseFilePath { set { _dbPath = value; } }
        public uint AlarmHistoryMaxCount { set { _alarmHistMaxCount = value; } }
        public event EventHandler AlarmOccoured
        {
            add
            {
                lock (eventLock)
                {
                    alarmOccoured += value;
                }
            }
            remove
            {
                lock (eventLock)
                {
                    alarmOccoured -= value;
                }
            }
        }
        private EventHandler alarmOccoured;
        private AlarmManager()
        {
            
        }

        public void Initialize(string dbPath)
        {
            _dbPath = dbPath;

            _alarmList = GetAllAlarmList();

            InitializeAlarmBitList();

            LogHelper.Instance.DBManagerLog.DebugFormat("[INFO] AlarmManager Initialize! ");
            DataManager.Instance.DataAccess.DataChangedEvent += OnAlarmChanged;
        }

        private void InitializeAlarmBitList()
        {
            bool result;
            List<Data> alarmTagList = DataManager.Instance.DataAccess.RemoteObject.DataList.Where(io => io.Group.Equals("ALARM")).ToList();

            _currentAlarmBitList = new SortedList<string, bool[]>(alarmTagList.Count);

            foreach(Data data in alarmTagList)
            {
                // Config2 = Word 개수
                byte[] alarmData = System.Text.Encoding.BigEndianUnicode.GetBytes(DataManager.Instance.GET_STRING_DATA(data.Name, out result).ToString());
                _currentAlarmBitList.Add(data.Module, ByteArrayToBoolArray(alarmData));
            }
            LogHelper.Instance.DBManagerLog.DebugFormat("[INFO] InitializeAlarmBitList {0} .", alarmTagList.Count);
        }

        private bool[] ByteArrayToBoolArray(byte[] byteArray)
        {
            bool[] bitArray = new bool[] {};

            bitArray = bitArray.Concat(new BitArray(byteArray).Cast<bool>().ToArray()).ToArray();
            
            return bitArray;
        }

        private List<ALARM> GetAllAlarmList()
        {
            string dbFilePath = _dbPath;
            string queryCommand = @"SELECT * FROM tbl_alarm";

            List<ALARM> alarmList = new List<ALARM>();

            DataTable dt = DbHandler.Instance.ExecuteQuery(dbFilePath, queryCommand);

            foreach (DataRow dr in dt.Rows)
            {
                ALARM alarm = new ALARM();

                alarm.INDEX = Convert.ToInt64(dr["NO"]).ToString();
                alarm.ID = dr["ID"] as string;
                alarm.TEXT = dr["Comment"] as string;
                alarm.MODULE = dr["Module"] as string;
                string level = dr["Level"] as string;

                if (level.Substring(0, 1).ToUpper().Equals("H")) alarm.LEVEL = ALARM.eALCD.Serious;
                else alarm.LEVEL = ALARM.eALCD.Light;
                alarm.DESCRIPTION = dr["Description"] as string;
                if (string.IsNullOrEmpty(alarm.DESCRIPTION)) alarm.DESCRIPTION = "";
                alarmList.Add(alarm);
            }
            LogHelper.Instance.DBManagerLog.DebugFormat("[INFO] Get Alarm List Success.");
            return alarmList;
        }

        private ALARM GetAlarmInfoByBitInfo(string module, int bitIndex, bool value)
        {
            var alarm = _alarmList.Where(a => (a.MODULE == module && a.INDEX == bitIndex.ToString())).FirstOrDefault();
            if(alarm == null)
            {
                alarm = new ALARM();
                alarm.INDEX = bitIndex.ToString();
                alarm.ID = bitIndex.ToString();
                alarm.LEVEL = ALARM.eALCD.Serious;
                alarm.STATUS = value ? ALARM.eALST.SET : ALARM.eALST.RESET;
                alarm.TEXT = "Undefined Alarm";
                alarm.ENABLE = ALARM.eALED.Enable;
                alarm.MODULE = module;
                alarm.DESCRIPTION = "";
            }
            else
            {
                alarm.STATUS = value ? ALARM.eALST.SET : ALARM.eALST.RESET;
            }
            LogHelper.Instance.DBManagerLog.DebugFormat("[INFO] GetAlarmInfoByBitInfo List Success.");
            return alarm;
        }

        public void UpdateAlarmData(Data data, out List<ALARM> ChangedAlarms)
        {
            int length = Convert.ToInt32(data.Config2) * 2;
            byte[] byteArray = new byte[length];
            byteArray = System.Text.Encoding.BigEndianUnicode.GetBytes(data.StringValue);
            bool[] newAlarmData = ByteArrayToBoolArray(byteArray);
            ChangedAlarms = new List<ALARM>();
            ConcurrentBag<ALARM> alarms = new ConcurrentBag<ALARM>();

            Parallel.For(0, newAlarmData.Length, new Action<int>(i => {

                if (_currentAlarmBitList[data.Module][i] != newAlarmData[i])
                {
                    alarms.Add(GetAlarmInfoByBitInfo(data.Module, i, newAlarmData[i]));
                    _currentAlarmBitList[data.Module][i] = newAlarmData[i];
                }
            }));

            ChangedAlarms = alarms.ToList();

            //for(int i = 0; i < newAlarmData.Length; i++)
            //{
            //    if(_currentAlarmBitList[data.Module][i] != newAlarmData[i])
            //    {
            //        ChangedAlarms.Add(GetAlarmInfoByBitInfo(data.Module, i, newAlarmData[i]));
            //        _currentAlarmBitList[data.Module][i] = newAlarmData[i];
            //    }
            //}
        }

        public void OnAlarmChanged(object sender, DataChangedEventHandlerArgs args)
        {
            Data data = args.Data;

            if (!data.Group.ToUpper().Contains("ALARM")) return;

            //Console.WriteLine("[Debug] OnAlarmChanged Evnet!!!");
            LogHelper.Instance.PLCDataChange.DebugFormat("[DEBUG] OnAlarmChanged Evnet!!!");

            ThreadPool.QueueUserWorkItem((param) => {

                List<ALARM> changedAlarms;
                this.UpdateAlarmData(data, out changedAlarms);

                foreach (ALARM alarm in changedAlarms)
                {
                    this.alarmOccoured.BeginInvoke(alarm, null, null, null);
                }

            });

            //WorkQueue.Instance.Add(new HandlerWorkerTask(this, data, new WorkEventHandler( (sender, param) => {                
            //    AlarmManager alarmMgr = sender as AlarmManager;
            //    Data alarmData = param as Data;
            //    List<ALARM> changedAlarms;
            //    alarmMgr.UpdateAlarmData(alarmData, out changedAlarms);

            //    foreach(ALARM alarm in changedAlarms)
            //    {
            //        alarmMgr.alarmOccoured.BeginInvoke(alarm, null, null, null);
            //    }

            //})));
            LogHelper.Instance.DBManagerLog.DebugFormat("[INFO] WorkQueue Instance Add -> {0}", data.Value);
        }  
    

        public List<ALARM> GetDefinedAlarms()
        {
            string sql_DefinedAlarmQuery = @"SELECT * FROM tbl_alarm order by id";

            DataTable definedAlarmTable = DbHandler.Instance.ExecuteQuery(_dbPath, sql_DefinedAlarmQuery);

            List<ALARM> definedAlarmList = new List<ALARM>();

            foreach(DataRow row in definedAlarmTable.Rows)
            {
                int no = int.Parse(row["NO"].ToString());
                string id = row["ID"] != null ? row["ID"].ToString() : string.Empty;
                string level = row["Level"] as string;
                ALARM.eALCD code = (level.Substring(0, 1).ToUpper() == "H") ? ALARM.eALCD.Serious : ALARM.eALCD.Light;               
                string text = row["Comment"] != null ? row["Comment"].ToString() : string.Empty;
                string unitid = row["MODULE"] != null ? row["MODULE"].ToString() : string.Empty;
                string description = row["Description"] != null ? row["Description"].ToString() : string.Empty;

                definedAlarmList.Add(new ALARM()
                {
                    INDEX = no.ToString(),
                    ID = row["ID"] != null ? row["ID"].ToString() : string.Empty,
                    LEVEL = code,
                    STATUS = ALARM.eALST.RESET,
                    TEXT = text,
                    MODULE = unitid,
                    DESCRIPTION = description
                });
            }
            LogHelper.Instance.DBManagerLog.DebugFormat("[INFO] Get Defined Alarms Success.");
            return definedAlarmList;
        }

        public List<ALARM> GetCurrentOccurredAlarms()
        {
            List<ALARM> SetAlarmBits = new List<ALARM>();
            try
            {
                for (int i = 0; i < _currentAlarmBitList.Keys.Count; i++)
                {
                    string module = _currentAlarmBitList.Keys[i];

                    for (int j = 0; j < _currentAlarmBitList[module].Length; j++)
                    {
                        if (_currentAlarmBitList[module][j])
                        {
                            SetAlarmBits.Add(GetAlarmInfoByBitInfo(module, j, _currentAlarmBitList[module][j]));
                        }
                    }
                }
                LogHelper.Instance.DBManagerLog.DebugFormat("[INFO] Get Current Occurred Alarms Success.");
                return SetAlarmBits;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.ErrorLog.DebugFormat("[ERROR] Get Current Occurred Alarms Fail." + ex.Message);
                return SetAlarmBits;
            }
        }

        //public List<ALARM> GetCurrentOccurredSeriousAlarms()
        //{
        //    List<ALARM> SetAlarmBits = new List<ALARM>();
        //    try
        //    {
        //        for (int i = 0; i < _currentAlarmBitList.Keys.Count; i++)
        //        {
        //            string module = _currentAlarmBitList.Keys[i];

        //            for (int j = 0; j < _currentAlarmBitList[module].Length; j++)
        //            {
        //                if (_currentAlarmBitList[module][j])
        //                {
        //                    int alarmNo = j;
        //                    SetAlarmBits.Add(GetAlarmInfoByBitInfo(module, alarmNo, _currentAlarmBitList[module][j]));
        //                }
        //            }
        //        }

        //        return SetAlarmBits;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex);
        //        return SetAlarmBits;
        //    }
        //}

        public void UpdateAlarmHistory(ALARM alarm)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();

            string sql_AlarmHistCount = @"SELECT COUNT(*) FROM tbl_alarmhistory";
            string sql_deleteOldestAlarmHist = @"DELETE FROM tbl_alarmhistory WHERE ID IN (SELECT ID FROM tbl_alarmhistory order by UpdateTime ASC LIMIT @LIMIT)";

            int historyRowCount = Convert.ToUInt16(DbHandler.Instance.ExecuteQuery(_dbPath, sql_AlarmHistCount).Rows[0][0].ToString());
            if(historyRowCount >= _alarmHistMaxCount)
            {
                string deleteCount = ((historyRowCount - (_alarmHistMaxCount - 1))).ToString();

                parameters.Add("@LIMIT", deleteCount);

                DbHandler.Instance.ExecuteNonQuery(_dbPath, sql_deleteOldestAlarmHist, parameters);
            }

            string sql = @"INSERT INTO tbl_alarmhistory VALUES(
                                                                @ID,  
                                                                @MODULE,                                                              
                                                                @TEXT,                                                                
                                                                @LEVEL,
                                                                @CODE,
                                                                @DESCRIPTION,
                                                                @UPDATETIME,
                                                                @INDEX, );";

            parameters = new Dictionary<string, object>();
            parameters.Add("@ID", alarm.ID);
            parameters.Add("@MODULE", alarm.MODULE);
            parameters.Add("@TEXT", alarm.TEXT);
            parameters.Add("@LEVEL", alarm.LEVEL);
            parameters.Add("@CODE", alarm.STATUS);
            parameters.Add("@DESCRIPTION", alarm.DESCRIPTION);
            parameters.Add("@UPDATETIME", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            parameters.Add("@INDEX", alarm.INDEX);
                    
          
            int rowCount = DbHandler.Instance.ExecuteNonQuery(_dbPath, sql, parameters);
            LogHelper.Instance.DBManagerLog.DebugFormat("[INFO] Update Alarm History Success.");
        }

        public DataTable GetAlarmHist(DateTime fromDate, DateTime toDate)
        {
            string sql_SelectAlarmHist = @"SELECT * FROM tbl_alarmhistory WHERE UpdateTime BETWEEN @FROM AND @TO";
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("@FROM", fromDate.ToString("yyyy-MM-dd HH:mm:ss"));
            param.Add("@TO", toDate.ToString("yyyy-MM-dd HH:mm:ss"));
            DataTable alarmHistData = DbHandler.Instance.ExecuteQuery(_dbPath, sql_SelectAlarmHist, param);
            LogHelper.Instance.DBManagerLog.DebugFormat("[INFO] Get Alarm History Success.");
            return alarmHistData;
        }
    }
}
