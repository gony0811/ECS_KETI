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
    public class AttributeManager
    {
        private List<Attribute> _attrInfoList = new List<Attribute>();
        private List<Data> _attrDataList = new List<Data>();

        private string _dbFilePath;

        public static readonly AttributeManager Instance = new AttributeManager();

        private AttributeManager()
        {
            _dbFilePath = "./db_io.mdb";
        }


        public void Initialize(string dbPath)
        {
            _dbFilePath = dbPath;

            _attrDataList = DataManager.Instance.DataAccess.RemoteObject.DataList.Where(io => io.Group.ToUpper() == "ATTR").ToList();

            string queryCommand = @"SELECT * FROM tbl_attribute";

            DataTable dt = DbHandler.Instance.ExecuteQuery(_dbFilePath, queryCommand);

            foreach (DataRow dr in dt.Rows)
            {
                Attribute attribute = new Attribute()
                {
                    INDEX = Convert.ToInt32(dr["DataIndex"]).ToString(),
                    ID = dr["ID"] as string,
                    ATTRIBUTENAME = dr["AttributeName"] as string,
                    MODULE = dr["Module"] as string,
                    UNIT = dr["Unit"] as string,
                    FORMAT = dr["Format"] as string,
                    DESCRIPTION = dr["Description"] as string
                };

                _attrInfoList.Add(attribute);
            }
            LogHelper.Instance.DBManagerLog.DebugFormat("[INFO] Attribute Initialize Success.");
        }

        //public Dictionary<string, string> GetAttributeValueList(string moduleName, string portName)
        //{
        //    bool result;
        //    Dictionary<string, string> attributeValueList = new Dictionary<string, string>();

        //    List<Attribute> attrInfoList = _attrInfoList.Where(info => info.MODULE == moduleName).ToList();
        //    try
        //    {
        //        if (attrInfoList != null && attrInfoList.Count > 0)
        //        {
        //            foreach (Attribute attribute in attrInfoList)
        //            {
        //                string retValue = string.Empty;
        //                //var dvData = _dvDataList.Where(data => (data.Index.ToString() == dv.INDEX)).FirstOrDefault();
        //                var attrData = _attrDataList.Where(data => (data.Module == attribute.MODULE) && (data.Index.ToString() == attribute.INDEX)).FirstOrDefault();
        //                switch (attrData.Type)
        //                {
        //                    case eDataType.Int:
        //                        {
        //                            var value = DataManager.Instance.GET_INT_DATA(attrData.Name, out result);
        //                            if (result) retValue = value.ToString();
        //                        }
        //                        break;
        //                    case eDataType.Double:
        //                        {
        //                            var value = DataManager.Instance.GET_DOUBLE_DATA(attrData.Name, out result);
        //                            if (result) retValue = value.ToString();
        //                        }
        //                        break;
        //                    case eDataType.String:
        //                        {
        //                            var value = DataManager.Instance.GET_STRING_DATA(attrData.Name, out result);
        //                            if (result) retValue = value;
        //                        }
        //                        break;
        //                    default:
        //                        {
        //                            retValue = string.Empty;
        //                            //Console.WriteLine("[Error] DV Type not defined : {0}", dvData.Name);
        //                            LogHelper.Instance.DBManagerLog.DebugFormat("[ERROR] Attribute Type not defined : {0}", attrData.Name);
        //                        }
        //                        break;
        //                }
        //                LogHelper.Instance.DBManagerLog.DebugFormat("[INFO] Get Attribute Value List Success.");
        //                attributeValueList.Add(attribute.ATTRIBUTENAME, retValue);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogHelper.Instance.ErrorLog.DebugFormat("[ERROR] Data Access Failed : {0} | {1}", ex.Message, ex.InnerException);
        //    }

        //    return attributeValueList;
        //    }
        //}

        public string GetValueByattr(string attr, string module)
        {
            var attrInfo = _attrInfoList.Where(s => ((s.ID == attr) && s.MODULE == module)).FirstOrDefault();
            var attrTag = _attrDataList.Where(s => (s.Index.ToString() == attrInfo.ID) && (s.Module == attrInfo.MODULE)).FirstOrDefault();

            if (attrTag == null)
            {
                LogHelper.Instance.DBManagerLog.DebugFormat("[ERROR] GetValueByAttribute - (attrTag == null)");
                return string.Empty;
            }
            else
            {
                switch (attrTag.Type)
                {
                    case eDataType.Int:
                        {
                            return "Int";
                        }
                    case eDataType.Double:
                        {
                            return "Double";
                        }
                    case eDataType.String:
                        {
                            return "String";
                        }
                    default:
                        {
                            LogHelper.Instance.DBManagerLog.DebugFormat("[ERROR] GetValueByAttribute - Not define DataType");
                            return string.Empty;
                        }
                }
            }
        }
    }
}
