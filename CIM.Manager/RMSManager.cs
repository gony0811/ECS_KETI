using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using INNO6.Core.Threading;
using INNO6.Core;
using INNO6.IO;

namespace CIM.Manager
{
    public class RMSManager
    {
        private string _dbFilePath;
        private List<Data> _rmsDataList;
        private List<RECIPE> _recipeList;
        private List<RECIPEPARAMETER> _recipeParamList;
        private SortedList<int, string> _storedPPIDNameList;
        private string _masterModuleName;
        public static readonly RMSManager Instance = new RMSManager();

        private RMSManager()
        {
            _recipeParamList = new List<RECIPEPARAMETER>();
            _recipeList = new List<RECIPE>();
            _storedPPIDNameList = new SortedList<int, string>();
        }

        public string CurrentPPID { get; set; }
        public string PostPPID { get; set; }

        public List<RECIPEPARAMETER> RecipeParameterList
        {
            get { return _recipeParamList.OrderBy(x=>x.PARAMETERNAME).ToList(); }
        }

        public SortedList<int, string> StoredPPIDNameList
        {
            get { return _storedPPIDNameList; }
        }

        public void Initialize(string dbPath, string currentPPID, string master_moduleName)
        {
            _dbFilePath = dbPath; CurrentPPID = currentPPID; _masterModuleName = master_moduleName;
            DataTable recipeParamTable = DbHandler.Instance.ExecuteQuery(_dbFilePath, "SELECT * FROM tbl_recipeparam ORDER BY ParameterName ASC", new Dictionary<string, object>());
            
            _rmsDataList = DataManager.Instance.DataAccess.RemoteObject.DataList.Where(io => io.Group.ToUpper() == "RMS").ToList();          

            foreach (DataRow row in recipeParamTable.Rows)
            {
                RECIPEPARAMETER param = new RECIPEPARAMETER();

                param.INDEX = row.Field<string>("DataIndex");
                param.PARAMETERNAME = row.Field<string>("ParameterName");
                param.MODULE = row.Field<string>("MODULE");
                param.UNIT = row.Field<string>("UNIT");
                param.MIN = row.Field<string>("MIN");
                param.MAX = row.Field<string>("MAX");
                param.APC = row.Field<string>("APC").Substring(0, 1) == "Y" ? true : false;
                param.DESCRIPTION = row.Field<string>("DESCRIPTION");

                _recipeParamList.Add(param);
            }

          
            List<Data> ppidDataTag = DataManager.Instance.DataAccess.RemoteObject.DataList.Where(t => t.Group == "PPID" && t.Module == master_moduleName).OrderBy(x => x.Index).ToList();

            bool readDataResult;

            foreach(Data tag in ppidDataTag)
            {
                string ppid = DataManager.Instance.GET_STRING_DATA(tag.Name, out readDataResult);

                if (!readDataResult || string.IsNullOrWhiteSpace(ppid) || _storedPPIDNameList.ContainsKey(tag.Index))
                {
                    _storedPPIDNameList.Add(tag.Index, ppid);
                }
            }
            LogHelper.Instance.DBManagerLog.DebugFormat("[INFO] RMS Initialize Success.");

        }

        public List<RECIPE> GetRecipeDataList(string ppid)
        {
            DataTable recipeTable = DbHandler.Instance.ExecuteQuery(_dbFilePath, "SELECT * FROM tbl_recipe", new Dictionary<string, object>());

            _recipeList.Clear();

            foreach (DataRow row in recipeTable.Rows)
            {
                RECIPE rcp = new RECIPE();
                rcp.INDEX = row.Field<string>("DataIndex");
                rcp.PPID = row.Field<string>("PPID");
                rcp.MODULE = row.Field<string>("Module");
                rcp.PARAMETERNAME = row.Field<string>("ParameterName");
                rcp.PARAMETERVALUE = row.Field<string>("ParameterValue");
                rcp.DESCRIPTION = row.Field<string>("Description");
                rcp.UPDATETIME = DateTime.Parse(row.Field<string>("UpdateTime"));

                _recipeList.Add(rcp);
            }
            LogHelper.Instance.DBManagerLog.DebugFormat("[INFO] Get Recipe Data List Success.");
            return _recipeList.Where(r => r.PPID == ppid).ToList();
        }

        public DataTable GetPPIDListAsDataTable()
        {
            string PPIDName = string.Empty;
            DataTable returnData = new DataTable("PPIDList");
            returnData.Columns.Add(new DataColumn("NO", typeof(int)));
            returnData.Columns.Add(new DataColumn("PPIDNAME"));

            List<Data> ppidData = DataManager.Instance.DataAccess.RemoteObject.DataList.Where(t => t.Group == "PPID" && t.Module == _masterModuleName).OrderBy(x => x.Index).ToList();
        

            bool readDataResult;

            for(int i = 0; i < ppidData.Count; i++)
            {               
                PPIDName = DataManager.Instance.GET_STRING_DATA(ppidData[i].Name, out readDataResult);

                if(readDataResult && !string.IsNullOrWhiteSpace(PPIDName))
                    returnData.Rows.Add(i+1, PPIDName);
            }
            LogHelper.Instance.DBManagerLog.DebugFormat("[INFO] Get PPID List As DataTable Success.");
            return returnData;
        }

        public List<string> GetPPIDListAsList()
        {
            bool result;

            List<string> ppidListAll = new List<string>();

            List<Data> dt = DataManager.Instance.DataAccess.RemoteObject.DataList.Where(t => t.Group == "PPID" && t.Module == _masterModuleName).ToList();
            
            foreach(Data t in dt)
            {
                string ppid = DataManager.Instance.GET_STRING_DATA(t.Name, out result);
                if (string.IsNullOrWhiteSpace(ppid) || string.IsNullOrEmpty(ppid)) continue;
                    
                ppidListAll.Add(ppid);
            }
            LogHelper.Instance.DBManagerLog.DebugFormat("[INFO] Get PPIDList As List Success.");
            return ppidListAll;
        }

        public void SetParametersToDBEx(string ppid, List<RECIPE> parameters)
        {
            if (GetParameterNameValueByPPID(ppid).Count > 0)
                DeleteParametersByPPID(ppid);

            List<string> sqlCommands = new List<string>();

            for (int i = 0; i < parameters.Count; i++)
            {
                string sqlString = string.Format(@"INSERT INTO tbl_recipe VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');",
                    parameters[i].INDEX,
                    parameters[i].PPID,
                    parameters[i].MODULE,
                    parameters[i].PARAMETERNAME,
                    parameters[i].PARAMETERVALUE,
                    " ",
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    );

                sqlCommands.Add(sqlString);
            }

            DbHandler.Instance.ExecuteNonQuery(_dbFilePath, sqlCommands);
        }
        public void SetParametersToDB(string ppid, List<RECIPE> parameters)
        {
            if(GetParameterNameValueByPPID(ppid).Count > 0)
                DeleteParametersByPPID(ppid);

            string sqlString = @"INSERT INTO tbl_recipe VALUES (@INDEX, @PPID, @Module, @ParameterName, @ParameterValue, @Description, @UpdateTime);";

            for (int i = 0; i < parameters.Count; i++)
            {

                Dictionary<string, object> p = new Dictionary<string, object>();

                p.Add("@INDEX", parameters[i].INDEX);
                p.Add("@PPID", parameters[i].PPID);
                p.Add("@Module", parameters[i].MODULE);
                p.Add("@ParameterName", parameters[i].PARAMETERNAME);
                p.Add("@ParameterValue", parameters[i].PARAMETERVALUE);
                p.Add("@Description", " ");
                p.Add("@UpdateTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                DbHandler.Instance.ExecuteNonQuery(_dbFilePath, sqlString, p);
            }
            LogHelper.Instance.DBManagerLog.DebugFormat("[INFO] Set Parameters To DB Success.");
        }

        public void DeleteParametersByPPID(string ppid)
        {
            string sqlString = @"DELETE FROM tbl_recipe WHERE PPID = @PPID";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@PPID", ppid);

            DbHandler.Instance.ExecuteNonQuery(_dbFilePath, sqlString, parameters);
            LogHelper.Instance.DBManagerLog.DebugFormat("[INFO] Delete Parameters By PPID Success.");
        }


        public string GetCurrentPPID()
        {
            bool result;

            PostPPID = CurrentPPID;
            CurrentPPID = DataManager.Instance.GET_STRING_DATA("iPLC1.EQStatus.PPID", out result);

            if (result) return CurrentPPID;
            else
            {
                LogHelper.Instance.ErrorLog.DebugFormat("[ERROR] Current PPID data name is wrong.");
                throw new Exception("[ERROR] Current PPID data name is wrong.");
            }
        }


        public string GetParameterNameByIndex(string index)
        {
            try
            {
                var result = (from m in _recipeParamList.AsEnumerable() 
                              where m.INDEX == index select m.PARAMETERNAME).FirstOrDefault();

                return result as string;
            }
            catch(Exception e)
            {
                LogHelper.Instance.ErrorLog.DebugFormat("[ERROR] RMSManager::GetParameterNameByIndex() : {0}", e.Message);
                throw e;
            }
        }

        public string GetIndexByRecipeParameterName(string paramName)
        {
            try
            {
                var result = (from m in _recipeParamList.AsEnumerable() where m.PARAMETERNAME == paramName select m.INDEX).FirstOrDefault();
                return result.ToString();
            }
            catch (Exception ex)
            {
                LogHelper.Instance.ErrorLog.DebugFormat("[ERROR] RMSManager::GetIndexByRecipeParameterName() : {0}", ex.Message);
                return null;
            }
        }

        public Dictionary<string, string> GetParameterNameValueByPPID(string ppid)
        {
            string sqlString = @"SELECT * FROM tbl_recipe WHERE PPID = @PPID ORDER BY ParameterName ASC";
            Dictionary<string, object> dbParam = new Dictionary<string, object>();
            dbParam.Add("@PPID", ppid);
            DataTable parameterInfo = DbHandler.Instance.ExecuteQuery(_dbFilePath, sqlString, dbParam);

            LogHelper.Instance.DBManagerLog.DebugFormat("[INFO] Get Parameter Name Value By PPID Success");
            return parameterInfo.AsEnumerable().Select(x => new KeyValuePair<string, string>(x.Field<string>("ParameterName"), x.Field<string>("ParameterValue"))).ToDictionary(x => x.Key, x => x.Value);
        }

    }
}
