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
    public class ECManager
    {
        private Dictionary<string, EC> _ecInfoList = new Dictionary<string, EC>();
        private List<Data> _ecDataList = new List<Data>();

        private string _dbFilePath;

        public static readonly ECManager Instance = new ECManager();

        private ECManager()
        {
            _dbFilePath = "./io_db.mdb";
        }

        public void Initialize(string dbPath)
        {
            _dbFilePath = dbPath;

            _ecDataList = DataManager.Instance.DataAccess.RemoteObject.DataList.Where(io => io.Group.ToUpper() == "ECM").ToList();

            string queryCommand = @"SELECT * FROM tbl_eqconst";

            DataTable dt = DbHandler.Instance.ExecuteQuery(_dbFilePath, queryCommand);

            foreach (DataRow dr in dt.Rows)
            {
                EC ec = new EC()
                {
                    INDEX = Convert.ToInt32(dr["DataIndex"]).ToString(),
                    ECID = dr["ECID"] as string,
                    ECNAME = dr["ECName"] as string,
                    MODULE = dr["Module"] as string,
                    UNIT = dr["Unit"] as string,
                    DESCRIPTION = dr["Description"] as string
                };

                _ecInfoList.Add(ec.ECID, ec);
            }
            LogHelper.Instance.DBManagerLog.DebugFormat("[INFO] ECID Initialize Success.");
        }

        public Dictionary<EC, ECValue> GetECValueList(string[] ecIds)
        {
            bool result;
            Dictionary<EC, ECValue> ecValues = new Dictionary<EC, ECValue>();

            if (ecIds != null && ecIds.Length > 0)
            {
                foreach (string ecid in ecIds)
                {
                    var ecinfo = _ecInfoList.Where(ec => (ec.Key == ecid)).FirstOrDefault();
                    if (ecinfo.Key == null || ecinfo.Value == null) return null;

                    var ecData = _ecDataList.Where(data => ((data.Index.ToString() == ecinfo.Value.INDEX) && (data.Module == ecinfo.Value.MODULE))).FirstOrDefault();
                    if (ecData == null) return null;

                    ECValue ecValue = new ECValue();

                    switch (ecData.Type)
                    {
                        case eDataType.Int:
                            {
                                var value = DataManager.Instance.GET_INT_DATA(ecData.Name, out result);
                                if (result) ecValue.ECDEF = value.ToString();
                            }
                            break;
                        case eDataType.Double:
                            {
                                var value = DataManager.Instance.GET_DOUBLE_DATA(ecData.Name, out result);
                                if (result) ecValue.ECDEF = value.ToString();
                            }
                            break;
                        case eDataType.String:
                            {
                                var value = DataManager.Instance.GET_STRING_DATA(ecData.Name, out result);
                                if (result) ecValue.ECDEF = value;
                            }
                            break;
                        default:
                            {
                                LogHelper.Instance.DBManagerLog.DebugFormat("[ERROR] EC Type not defined : {0}", ecData.Name);
                            }
                            break;
                    }
                    ecValues.Add(ecinfo.Value, ecValue);
                }
            }
            else
            {
                foreach (EC ec in _ecInfoList.Values)
                {
                    var ecTag = _ecDataList.Where(s => s.Index.ToString() == ec.INDEX && s.Module == ec.MODULE).FirstOrDefault();

                    if (ecTag == null) return ecValues;

                    ECValue ecValue = new ECValue();

                    switch (ecTag.Type)
                    {
                        case eDataType.Int:
                            {
                                var value = DataManager.Instance.GET_INT_DATA(ecTag.Name, out result);
                                if (result) ecValue.ECDEF = value.ToString();
                            }
                            break;
                        case eDataType.Double:
                            {
                                var value = DataManager.Instance.GET_DOUBLE_DATA(ecTag.Name, out result);
                                if (result) ecValue.ECDEF = value.ToString();
                            }
                            break;
                        case eDataType.String:
                            {
                                var value = DataManager.Instance.GET_STRING_DATA(ecTag.Name, out result);
                                if (result) ecValue.ECDEF = value;
                            }
                            break;
                        default:
                            {
                                LogHelper.Instance.DBManagerLog.DebugFormat("[ERROR] EC Type not defined : {0}", ecTag.Name);
                            }
                            break;
                    }

                    ecValues.Add(ec, ecValue);
                }

            }
            return ecValues;
        }

        public bool IsExistECID(string ecid)
        {
            return _ecInfoList.ContainsKey(ecid);
        }
    }
}
