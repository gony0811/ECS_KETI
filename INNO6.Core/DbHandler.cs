using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Threading;
using System.Data.OleDb;
using INNO6.Core.Threading;

namespace INNO6.Core
{
    public class DbHandler
    {
        public static readonly DbHandler Instance = new DbHandler(); 

        private DbHandler()
		{
           
        }

        private string _connectionString = string.Empty;

        private void SetConnectionString(string dbfilePath)
        {
            _connectionString = @"Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + dbfilePath;
        }

        public void ExecuteNonQuery(string dbfile, List<string> aSqlStrings)
        {
            SetConnectionString(dbfile);

            try
            {
                using (OleDbConnection sqlConnection = new OleDbConnection(_connectionString))
                {
                    sqlConnection.Open();
                    
                    foreach(string cmd in aSqlStrings)
                    {
                        OleDbCommand sqlCommand = new OleDbCommand(cmd, sqlConnection);
                        sqlCommand.CommandType = CommandType.Text;                      
                        sqlCommand.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                _connectionString = string.Empty;

                LogHelper.Instance.ErrorLog.DebugFormat("[ERROR] ExecuteNonQuery Exception, DB Path : {0}, Query : {1}", dbfile, aSqlStrings[0]);
                LogHelper.Instance.ErrorLog.DebugFormat(ex.Message);
            }
        }

        public void ExecuteQueryToWorkQueue(string dbfile, List<string> aSqlStrings)
        {
            SetConnectionString(dbfile);

            WorkQueue.Instance.Add(new HandlerWorkerTask(this, null, new WorkEventHandler((sender, param) =>
            {

                try
                {
                    using (OleDbConnection sqlConnection = new OleDbConnection(_connectionString))
                    {
                        sqlConnection.Open();

                        foreach (string cmd in aSqlStrings)
                        {
                            OleDbCommand sqlCommand = new OleDbCommand(cmd, sqlConnection);
                            sqlCommand.CommandType = CommandType.Text;
                            sqlCommand.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception ex)
                {
                    _connectionString = string.Empty;
                    if (aSqlStrings.Count > 0)
                        LogHelper.Instance.ErrorLog.DebugFormat("[ERROR] ExecuteNonQuery Exception, DB Path : {0}, Query : {1}", dbfile, aSqlStrings[0]);
                    LogHelper.Instance.ErrorLog.DebugFormat(ex.Message);
                }

            })));
            if (aSqlStrings.Count > 0)
                LogHelper.Instance.DBManagerLog.DebugFormat("[INFO] ExecuteQueryToWorkQueue::" + dbfile + aSqlStrings[0]);
        }

        public int ExecuteNonQuery(string dbfile, string aSqlString, Dictionary<string, object> aParameters = null)
        {
            SetConnectionString(dbfile);

            int result = 0;
            try
            {
                using (OleDbConnection sqlConnection = new OleDbConnection(_connectionString))
                {

                    OleDbCommand sqlCommand = new OleDbCommand(aSqlString, sqlConnection);

                    if (aParameters != null)
                        foreach (KeyValuePair<string, object> dBParameter in aParameters)
                            sqlCommand.Parameters.Add(new OleDbParameter(dBParameter.Key, dBParameter.Value));

                    sqlCommand.CommandType = CommandType.Text;
                    sqlConnection.Open();
                    result = sqlCommand.ExecuteNonQuery();

                    LogHelper.Instance.DBManagerLog.DebugFormat("[INFO] DB Excute Result : {0}, Query : {1}", result, aSqlString);
                }
            }
            catch (Exception ex)
            {                           
                _connectionString = string.Empty;

                LogHelper.Instance.ErrorLog.DebugFormat("[ERROR] ExecuteNonQuery Exception, DB Path : {0}, Query : {1}", dbfile, aSqlString);
                LogHelper.Instance.ErrorLog.DebugFormat(ex.Message);
            }

            return result;
        }

        public DataTable ExecuteQuery(string dbfile, string aSqlString, Dictionary<string, object> aParameters = null)
        {
            SetConnectionString(dbfile);

            DataTable dataTable = new DataTable();

            try
            {
                using (OleDbConnection sqlConnection = new OleDbConnection(_connectionString))
                {

                    OleDbCommand sqlCommand = new OleDbCommand(aSqlString, sqlConnection);

                    sqlCommand.CommandType = CommandType.Text;

                    if (aParameters != null)
                        foreach (KeyValuePair<string, object> dBParameter in aParameters)
                            sqlCommand.Parameters.Add(new OleDbParameter(dBParameter.Key, dBParameter.Value));

                    OleDbDataAdapter sqlDataAdapter = new OleDbDataAdapter(sqlCommand);

                    sqlDataAdapter.Fill(dataTable);

                    //Console.WriteLine("DB Excute Result : {0}, Query : {1}", dataTable != null ? 1 : -1, aSqlString);
                    
                    LogHelper.Instance.DBManagerLog.DebugFormat("[INFO] DB Excute Result : {0}, Query : {1}", dataTable != null ? 1 : -1, aSqlString);
                }
            }
            catch (Exception ex)
            {

                LogHelper.Instance.ErrorLog.DebugFormat("[INFO] ExecuteNonQuery Exception, DB Path : {0}, Query : {1}", dbfile, aSqlString);
                LogHelper.Instance.ErrorLog.DebugFormat(ex.Message);

                _connectionString = string.Empty;
                
            }

            return dataTable;
        }

        public void ExecuteQueryToWorkQueue(string dbFileName, string sqlString, Dictionary<string, object> parameters = null)
        {
            SetConnectionString(dbFileName);

            WorkQueue.Instance.Add(new HandlerWorkerTask(this, null, new WorkEventHandler((sender, param) => {

                using (OleDbConnection sqlConnection = new OleDbConnection(_connectionString))
                {
                    OleDbCommand sqlCommand = new OleDbCommand(sqlString, sqlConnection);

                    if (parameters != null)
                        foreach (KeyValuePair<string, object> dbParameter in parameters)
                            sqlCommand.Parameters.Add(new OleDbParameter(dbParameter.Key, dbParameter.Value));

                    sqlCommand.CommandType = CommandType.Text;

                    sqlConnection.Open();
                    int result = sqlCommand.ExecuteNonQuery();
                }

            })));
            LogHelper.Instance.DBManagerLog.DebugFormat("[INFO] ExecuteQueryToWorkQueue::" + dbFileName + sqlString);
        }

        public void InsertData(string dbfile, DataTable datatable)
        {
            //List<string> columnNames = new List<string>();
            //foreach (DataColumn item in datatable.Columns)
            //{
            //    if (item.ColumnName == "GROUP")
            //        columnNames.Add("`" + item.ColumnName + "`");
            //    else
            //        columnNames.Add(item.ColumnName);
            //}
            //string columns = string.Join(",", columnNames);

            //string sql_insert = string.Format("INSERT INTO {0}({1}) values ",datatable.TableName, columns);

            string sql_insert = string.Format("INSERT INTO {0} values ", datatable.TableName);

            List<string> valuelist = new List<string>();

            List<string> sql_insertCommands = new List<string>();
            int rowCount = 0;
            foreach (DataRow row in datatable.Rows)
            {
                if (rowCount % 100 == 0 && rowCount != 0)
                {
                    sql_insertCommands.Add(sql_insert + string.Join(",", valuelist));
                    valuelist = new List<string>();
                }
                else
                {

                }

                List<string> values = new List<string>();
                for (int i = 0; i < row.ItemArray.Length; i++)
                {
                    if (datatable.Columns[i].DataType == typeof(long))
                    {
                        values.Add(row.ItemArray[i].ToString());
                    }
                    else
                    {
                        values.Add("'" + row.ItemArray[i].ToString() + "'");
                    }
                }

                string valuesString = string.Format("({0})", string.Join(",", values));

                valuelist.Add(valuesString);

                rowCount++;
            }

            if (datatable.Rows.Count <= 0)
                return;

            sql_insertCommands.Add(sql_insert + string.Join(",", valuelist));
            valuelist = new List<string>();

            ExecuteQueryToWorkQueue(dbfile, sql_insertCommands);

            Console.WriteLine("{0} Sheet DB 입력 성공!", datatable.TableName);
            //sql_insert = sql_insert + string.Join(",",valuelist);

            //ExecuteNonQuery(dbfile, sql_insert);

        }
    }
}
