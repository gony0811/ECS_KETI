using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;
using INNO6.Core;

namespace INNO6.Core
{
    public class ReadMdb<T> : IDisposable
    {
        protected OleDbConnection _connection;
        protected OleDbCommand _command;
        protected OleDbDataReader _reader;
        private List<T> _dataList = new List<T>();

        public OleDbDataReader DataReader
        {
            get { return _reader; }
            set { _reader = value; }
        }

        public List<T> DataList
        {
            get { return _dataList; }
            private set { _dataList = value; }
        }

        public string DatabasePathName { get; set; }
        public string DatabaseTableName { get; set; }

        public ReadMdb(string dbPathName, string dbTblName)
        {
            DatabasePathName = dbPathName;
            DatabaseTableName = dbTblName;
        }

        public bool Open()
        {
            string connectionString = @"Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + DatabasePathName;

            try
            {
                _connection = new OleDbConnection(connectionString);
                _connection.Open();
                _command = new OleDbCommand();
                _command.Connection = _connection;
                LogHelper.Instance.DBManagerLog.DebugFormat("[Read MDB] OLEDB Open Success.");
                return true;
            }
            catch
            {
                LogHelper.Instance.DBManagerLog.DebugFormat("[Read MDB] OLEDB Open Fail.");
                return false;
            }
        }

        public void Close()
        {
            if (_connection != null)
                _connection.Close();
        }

        public OleDbConnection GetConnection()
        {
            return _connection;
        }


        public int RowCount()
        {
            try
            {
                _command.CommandText = "select count(*) from " + DatabaseTableName;
                return (int)_command.ExecuteScalar();
            }
            catch
            {
                return 0;
            }
        }

        public bool SqlCommand(string query)
        {
            try
            {
                _command.CommandText = query;
                _command.ExecuteNonQuery();
                LogHelper.Instance.DBManagerLog.DebugFormat("[Read MDB] Sql Command ExecuteNonQuery Success.");
                return true;
            }
            catch
            {
                LogHelper.Instance.DBManagerLog.DebugFormat("[Read MDB] Sql Command ExecuteNonQuery Fail.");
                return false;
            }
        }

        public virtual bool GetDataListRead()
        {
            return true;
        }

        void IDisposable.Dispose()
        {
            Close();
        }
    }
}
