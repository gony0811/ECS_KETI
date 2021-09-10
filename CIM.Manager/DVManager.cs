using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using INNO6.Core;
using INNO6.IO;

namespace CIM.Manager
{
    public class DVManager
    {
        private List<DV> _dvInfoList = new List<DV>();
        private List<Data> _dvDataList = new List<Data>();

        private string _dbFilePath;

        public static readonly DVManager Instance = new DVManager();

        private DVManager()
        {
            _dbFilePath = "./db_io.mdb";
        }


        public void Initialize(string dbPath)
        {
            _dbFilePath = dbPath;

            _dvDataList = DataManager.Instance.DataAccess.RemoteObject.DataList.Where(io => io.Group.ToUpper() == "DV").ToList();

            string queryCommand = @"SELECT * FROM tbl_datavalue";

            DataTable dt = DbHandler.Instance.ExecuteQuery(_dbFilePath, queryCommand);

            foreach (DataRow dr in dt.Rows)
            {
                DV dv = new DV()
                {
                    INDEX = Convert.ToInt32(dr["DataIndex"]).ToString(),
                    DVNAME = dr["DVName"] as string,
                    MODULE = dr["Module"] as string,
                    PORTNAME = dr["PortName"] as string,
                    UNIT = dr["Unit"] as string,
                    DESCRIPTION = dr["Description"] as string
                };

                _dvInfoList.Add(dv);
            }
            LogHelper.Instance.DBManagerLog.DebugFormat("[INFO] DV Initialize Success.");
        }

        public Dictionary<string, string> GetDVNameValueList(string moduleName, string portName)
        {
            bool result;
            Dictionary<string, string> dvNameValueList = new Dictionary<string, string>();

            List<DV> dvInfoList1 = _dvInfoList.Where(info => info.MODULE == moduleName && info.PORTNAME == portName).ToList();

            SortedList<string, DV> dvInfo = new SortedList<string, DV>();
            foreach(DV dv in dvInfoList1)
            {
                dvInfo.Add(dv.INDEX, dv);
            }

            List<DV> dvInfoList = dvInfo.Values.ToList();

            try
            {
                if (dvInfoList != null && dvInfoList.Count > 0)
                {
                    foreach (DV dv in dvInfoList)
                    {
                        string retValue = string.Empty;
                        //var dvData = _dvDataList.Where(data => (data.Index.ToString() == dv.INDEX)).FirstOrDefault();
                        var dvData = _dvDataList.Where(data => (data.Module == dv.MODULE) && (data.Index.ToString() == dv.INDEX)).FirstOrDefault();
                        switch (dvData.Type)
                        {
                            case eDataType.Int:
                                {
                                    var value = DataManager.Instance.GET_INT_DATA(dvData.Name, out result);
                                    if (result) retValue = value.ToString();
                                }
                                break;
                            case eDataType.Double:
                                {
                                    var value = DataManager.Instance.GET_DOUBLE_DATA(dvData.Name, out result);
                                    if (result) retValue = value.ToString();
                                }
                                break;
                            case eDataType.String:
                                {
                                    var value = DataManager.Instance.GET_STRING_DATA(dvData.Name, out result);
                                    if (result) retValue = value;
                                }
                                break;
                            default:
                                {
                                    retValue = string.Empty;
                                    //Console.WriteLine("[Error] DV Type not defined : {0}", dvData.Name);
                                    LogHelper.Instance.DBManagerLog.DebugFormat("[ERROR] DV Type not defined : {0}", dvData.Name);
                                }
                                break;
                        }
                        LogHelper.Instance.DBManagerLog.DebugFormat("[INFO] Get DV Name Value List Success.");
                        dvNameValueList.Add(dv.DVNAME, retValue);
                    }
                }
            }
            catch(Exception ex)
            {
                LogHelper.Instance.ErrorLog.DebugFormat("[ERROR] Data Access Failed : {0} | {1}", ex.Message, ex.InnerException);
            }

            return dvNameValueList;
        }
    }
}
