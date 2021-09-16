using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using INNO6.Core.Manager.Model;

namespace INNO6.Core.Manager
{
    public class AlarmEventArgs : EventArgs
    {
        public List<ALARM> Alarms { get; set; }
        public ALARM Alarm { get; set; }

        public AlarmEventArgs(ALARM alarm)
        {
            Alarm = alarm;
        }

        public AlarmEventArgs(List<ALARM> alarms)
        {
            Alarms = alarms;
        }
    }

    public class AlarmManager
    {
        public static readonly AlarmManager Instance = new AlarmManager();
        private readonly object eventLock = new object();
        private string _dbPath;
        private int _alarmHistMaxCount = 100;
        private List<ALARM> _alarmDefineList;
        private List<ALARM_HISTORY> _alarmHistoryList = new List<ALARM_HISTORY>();
        private EventHandler setAlarmEvent;
        private EventHandler resetAlarmEvent;

        public event EventHandler SetAlarmEvent
        {
            add
            {
                lock (eventLock)
                {
                    setAlarmEvent += value;
                }
            }
            remove
            {
                lock (eventLock)
                {
                    setAlarmEvent -= value;
                }
            }
        }

        public event EventHandler ResetAlarmEvent
        {
            add
            {
                lock (eventLock)
                {
                    resetAlarmEvent += value;
                }
            }
            remove
            {
                lock (eventLock)
                {
                    resetAlarmEvent -= value;
                }
            }
        }

        private AlarmManager()
        {

        }

        public string DatabaseFilePath { set { _dbPath = value; } }
        public int AlarmHistoryMaxCount { set { _alarmHistMaxCount = value; } }


        public void Initialize(string dbPath)
        {
            _dbPath = Path.GetFullPath(dbPath);

            _alarmDefineList = GetAllDefineAlarmList();
        }


        public void SetAlarm(string alarm_id)
        {
            if (ContainsCurrentAlarm(alarm_id)) return;

            ALARM alarm = _alarmDefineList.FindAll((x) => (x.ID == alarm_id)).FirstOrDefault();

            

            if (alarm != null)
            {
                alarm.STATUS = eALST.SET;
            }
            else
            {
                alarm = new ALARM();
                alarm.ID = alarm_id;
                alarm.TEXT = "NOT_DEFINE_ALARM";
                alarm.LEVEL = eALCD.Heavy;
                alarm.STATUS = eALST.SET;
                alarm.ENABLE = eALED.Enable;
                alarm.DESCRIPTION = "NOT_DEFINE_ALARM";
            }


            UpdateCurrnetAlarm(alarm);
            UpdateAlarmHistory(alarm);

            AlarmEventArgs eventArgs = new AlarmEventArgs(alarm);
            if (setAlarmEvent != null) setAlarmEvent(this, eventArgs);
        }

        public void ResetAlarm(string alarm_id)
        {
            List<ALARM> currentAlarms = GetCurrentAlarmAsList();

            ALARM alarm = currentAlarms.FindAll((x) => (x.ID == alarm_id)).FirstOrDefault();

            if(alarm != null && DeleteCurrentAlarm(alarm) > 0)
            {
                alarm.STATUS = eALST.RESET;
                UpdateAlarmHistory(alarm);
                AlarmEventArgs eventArgs = new AlarmEventArgs(alarm);
                resetAlarmEvent(this, eventArgs);
            }
        }

        public void ResetAlarmAll()
        {
            List<ALARM> currentAlarms = GetCurrentAlarmAsList();

            foreach(ALARM alarm in currentAlarms)
            {
                if (alarm != null && DeleteCurrentAlarm(alarm) > 0)
                {
                    UpdateAlarmHistory(alarm);
                    AlarmEventArgs eventArgs = new AlarmEventArgs(alarm);
                    resetAlarmEvent(this, eventArgs);
                }
            }
        }

        public bool ContainsCurrentAlarm(string alarmID)
        {
            ALARM alarm = GetCurrentAlarmAsList().Find((o) => (o.ID == alarmID));

            if (alarm != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public DataTable GetCurrentAlarmAsDataTable()
        {
            string dbFilePath = _dbPath;
            string queryCommand = @"SELECT * FROM sys_alarm_current";

            DataTable dt = DbHandler.Instance.ExecuteQuery(dbFilePath, queryCommand);

            return dt;
        }

        public List<ALARM> GetCurrentAlarmAsList()
        {
            DataTable dt = GetCurrentAlarmAsDataTable();

            List<ALARM> currentAlarmList = new List<ALARM>();

            foreach (DataRow dr in dt.Rows)
            {
                ALARM alarm = new ALARM();

                alarm.ID = dr["ID"] as string;
                alarm.TEXT = dr["TEXT"] as string;
                alarm.DESCRIPTION = dr["DESCRIPTION"] as string;
                if (string.IsNullOrEmpty(alarm.DESCRIPTION)) alarm.DESCRIPTION = "";

                string level = dr["LEVEL"] as string;
                string status = dr["STATUS"] as string;
                string enable = dr["ENABLE"] as string;
                alarm.SETTIME = DateTime.Parse(dr["SETTIME"].ToString());

                if (level.Substring(0, 1).ToUpper().Equals("H")) alarm.LEVEL = eALCD.Heavy;
                else alarm.LEVEL = eALCD.Light;

                if (enable.Substring(0, 1).ToUpper().Equals("E")) alarm.ENABLE = eALED.Enable;
                else alarm.ENABLE = eALED.Disable;

                if (status.Substring(0, 1).ToUpper().Equals("S")) alarm.STATUS = eALST.SET;
                else if (status.Substring(0, 1).ToUpper().Equals("R")) alarm.STATUS = eALST.RESET;
                else alarm.STATUS = eALST.Unknown;
                
                currentAlarmList.Add(alarm);
            }

            return currentAlarmList;
        }

        public DataTable GetAlarmHistory(DateTime fromDate, DateTime toDate)
        {
            string sql_SelectAlarmHist = @"SELECT * FROM sys_alarm_history WHERE UPDATETIME BETWEEN @FROM AND @TO";
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("@FROM", fromDate.ToString("yyyy-MM-dd HH:mm:ss"));
            param.Add("@TO", toDate.ToString("yyyy-MM-dd HH:mm:ss"));
            DataTable alarmHistData = DbHandler.Instance.ExecuteQuery(_dbPath, sql_SelectAlarmHist, param);
            LogHelper.Instance.DBManagerLog.DebugFormat("[INFO] Get Alarm History Success.");
            return alarmHistData;
        }

        public bool IsExistCurrentAlarm(string alarmId)
        {
            List<ALARM> currentAlarms = GetCurrentAlarmAsList();
            Dictionary<string, object> parameters = new Dictionary<string, object>();

            if (currentAlarms.FindAll((a) => (a.ID == alarmId)).Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }

        public int UpdateCurrnetAlarm(ALARM alarm)
        {
            int rowCount = 0;
            List<ALARM> currentAlarms = GetCurrentAlarmAsList();
            Dictionary<string, object> parameters = new Dictionary<string, object>();

            if (currentAlarms.FindAll((a) => (a.ID == alarm.ID)).Count > 0)
            {
                string sql = string.Format(@"UPDATE sys_alarm_current SET SETTIME = ""{0}"" WHERE ID = ""{1}"";", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), alarm.ID);

                //parameters = new Dictionary<string, object>();
                //parameters.Add("@ID", alarm.ID);
                //parameters.Add("@SETTIME", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                rowCount = DbHandler.Instance.ExecuteNonQuery(_dbPath, sql);
            }
            else
            {             
                string sql = @"INSERT INTO sys_alarm_current VALUES(
                                                                @ID,                                                                      
                                                                @LEVEL,                                                                
                                                                @TEXT,
                                                                @STATUS,
                                                                @ENABLE,
                                                                @DESCRIPTION,
                                                                @SETTIME
                                                                );";

                parameters = new Dictionary<string, object>();
                parameters.Add("@ID", alarm.ID);
                parameters.Add("@LEVEL", alarm.LEVEL.ToString());
                parameters.Add("@TEXT", alarm.TEXT);
                parameters.Add("@STATUS", alarm.STATUS.ToString());
                parameters.Add("@ENABLE", alarm.ENABLE.ToString());
                parameters.Add("@DESCRIPTION", alarm.DESCRIPTION);
                parameters.Add("@SETTIME", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                rowCount = DbHandler.Instance.ExecuteNonQuery(_dbPath, sql, parameters);
            }
         
            LogHelper.Instance.DBManagerLog.DebugFormat("[INFO] Update Current Alarm Success.");

            return rowCount;
        }

        public int DeleteCurrentAlarm(ALARM alarm)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            string sql_deleteCurrentAlarm = @"DELETE FROM sys_alarm_current WHERE ID = @ID";

            parameters.Add("@ID", alarm.ID);

            return DbHandler.Instance.ExecuteNonQuery(_dbPath, sql_deleteCurrentAlarm, parameters);
        }

        public int DeleteCurrentAlarmAll()
        {
            string sql_deleteCurrentAlarm = @"DELETE FROM sys_alarm_currnet";
            return DbHandler.Instance.ExecuteNonQuery(_dbPath, sql_deleteCurrentAlarm);
        }

        public void UpdateAlarmHistory(ALARM alarm)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();

            string sql_AlarmHistCount = @"SELECT COUNT(*) FROM sys_alarm_history";
            

            int historyRowCount = Convert.ToUInt16(DbHandler.Instance.ExecuteQuery(_dbPath, sql_AlarmHistCount).Rows[0][0].ToString());
            if (historyRowCount >= _alarmHistMaxCount)
            {
                int deleteCount = ((historyRowCount - (_alarmHistMaxCount - 1)));
                //parameters.Add("@LIMIT", deleteCount);
                string sql_deleteOldestAlarmHist = string.Format(@"DELETE FROM sys_alarm_history WHERE ID IN (SELECT TOP {0} ID FROM sys_alarm_history order by UPDATETIME ASC)", deleteCount);
               

                DbHandler.Instance.ExecuteNonQuery(_dbPath, sql_deleteOldestAlarmHist);
            }

            string sql = @"INSERT INTO sys_alarm_history VALUES(
                                                                @ID,                                                                      
                                                                @LEVEL,                                                                
                                                                @ATEXT,
                                                                @STATUS,
                                                                @ENABLE,
                                                                @DESCRIPTION,
                                                                @UPDATETIME
                                                                );";

            parameters = new Dictionary<string, object>();
            parameters.Add("@ID", alarm.ID);
            parameters.Add("@LEVEL", alarm.LEVEL.ToString());
            parameters.Add("@TEXT", alarm.TEXT);
            parameters.Add("@STATUS", alarm.STATUS.ToString());
            parameters.Add("@ENABLE", alarm.ENABLE.ToString());
            parameters.Add("@DESCRIPTION", alarm.DESCRIPTION);
            parameters.Add("@UPDATETIME", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));


            int rowCount = DbHandler.Instance.ExecuteNonQuery(_dbPath, sql, parameters);
            LogHelper.Instance.DBManagerLog.DebugFormat("[INFO] Update Alarm History Success.");
        }

        private List<ALARM> GetAllDefineAlarmList()
        {
            string dbFilePath = _dbPath;
            string queryCommand = @"SELECT * FROM sys_alarm_define";

            List<ALARM> alarmList = new List<ALARM>();

            DataTable dt = DbHandler.Instance.ExecuteQuery(dbFilePath, queryCommand);

            foreach (DataRow dr in dt.Rows)
            {
                ALARM alarm = new ALARM();

                alarm.ID = dr["ID"] as string;
                alarm.TEXT = dr["TEXT"] as string;
                alarm.DESCRIPTION = dr["DESCRIPTION"] as string;
                if (string.IsNullOrEmpty(alarm.DESCRIPTION)) alarm.DESCRIPTION = "";

                string level = dr["LEVEL"] as string;
                string enable = dr["ENABLE"] as string;

                if (level.Substring(0, 1).ToUpper().Equals("H")) alarm.LEVEL = eALCD.Heavy;
                else alarm.LEVEL = eALCD.Light;

                if (enable.Substring(0, 1).ToUpper().Equals("E")) alarm.ENABLE = eALED.Enable;
                else alarm.ENABLE = eALED.Disable;

                alarmList.Add(alarm);
            }

            return alarmList;
        }


    }
}
