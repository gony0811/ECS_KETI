using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using INNO6.Core;
using INNO6.IO.Remote;

namespace INNO6.IO
{
    public class InitializeDataInfo
    {
        private ConfigManager _config;

        public InitializeDataInfo(string configIniFilePath)
        {
            _config = new ConfigManager(configIniFilePath);

        }

        public int ReadDataInfoList(RemoteObject remoteObject)
        {
            string dbFilePath = _config.GetIniValue("DATADB", "PATH");
            string queryCommand= @"SELECT * FROM master_io";

            DataTable dt = DbHandler.Instance.ExecuteQuery(dbFilePath, queryCommand);

            foreach(DataRow dr in dt.Rows)
            {
                try
                {
                    Data data = new Data();

                    data.Name = dr["Name"] as string;
                    data.Module = dr["Module"] as string;
                    data.Group = dr["Group"] as string;

                    if (string.IsNullOrEmpty(data.Group))
                        data.Group = "NORMAL";

                    data.DriverName = dr["DriverName"] as string;
                    string direction = dr["Direction"] as string;
                    data.DefaultValue = dr["DefaultValue"] as string;

                    if(string.IsNullOrEmpty(direction))
                        data.Direction = eDirection.BOTH;
                    else if (direction.Equals("OUT"))
                        data.Direction = eDirection.OUT;
                    else if (direction.Equals("IN"))
                        data.Direction = eDirection.IN;
                    else
                        data.Direction = eDirection.BOTH;

                    string type = (dr["Type"] as string);

                    if(string.IsNullOrEmpty(type))
                    {
                        data.Type = eDataType.String;
                        data.Value = data.DefaultValue;
                    }
                    else if (type.Substring(0, 1) == "I")
                    {
                        data.Type = eDataType.Int;
                        int value = 0;
                        if (int.TryParse(data.DefaultValue, out value)) data.Value = (object)value;
                        else data.Value = (object)0;
                    }
                    else if (type.Substring(0, 1) == "D")
                    {
                        data.Type = eDataType.Double;
                        double value = 0.0;
                        if (double.TryParse(data.DefaultValue, out value)) data.Value = (object)value;
                        else data.Value = (object)0.0;
                    }
                    else if(type.Substring(0, 1) == "O")
                    {
                        data.Type = eDataType.Object;                                          
                    }
                    else
                    {
                        data.Type = eDataType.String;
                        if (string.IsNullOrEmpty(data.DefaultValue))
                        {
                            data.Value = "";
                        }
                        else
                        {
                            data.Value = data.DefaultValue;
                        }
                            
                    }

                    data.Config1 = dr["Config1"] as string;
                    data.Config2 = dr["Config2"] as string;
                    data.Config3 = dr["Config3"] as string;
                    data.Config4 = dr["Config4"] as string;

                    string sPollingTime = dr["PollingTime"] as string;
                    int iPollingTime;
                    if (int.TryParse(sPollingTime, out iPollingTime)) data.PollingTime = iPollingTime;
                    else data.PollingTime = 0;

                    string sDataResetTime = dr["DataResetTime"] as string;
                    int iDataResetTime;
                    if (int.TryParse(sDataResetTime, out iDataResetTime)) data.DataResetTimeout = iDataResetTime;
                    else data.DataResetTimeout = 0;

                    double resultValue = 1.0;
                    string format = dr["Format"] as string;
                    if (double.TryParse(format, out resultValue)) data.Format = resultValue;                  

                    string use = (dr["Use"] as string);                 
                    if (string.IsNullOrEmpty(use)) data.Use = false;
                    else if (use.Equals("N")) data.Use = false;
                    else data.Use = true;

                    //string logging = (dr["Logging"] as string);
                    //if (string.IsNullOrEmpty(logging)) data.Logging = false;
                    //else if (logging.Equals("N")) data.Logging = false;
                    //else data.Logging = true;

                    if (Convert.IsDBNull(dr["Index"])) { data.Index = 0; }
                    else { data.Index = Convert.ToInt32(dr["DataIndex"]); }
                    data.Description = dr["Description"] as string;
                    data.CheckTime = DateTime.Now;

                    //data.InterlockName = dr["InterlockName"] as string;

                    //string mode = dr["InterlockMode"] as string;

                    //if(string.IsNullOrEmpty(mode)) { data.InterlockMode = eInterlock.NONE; }
                    //else if (mode.ToUpper().Contains("POINT")) { data.InterlockMode = eInterlock.SETPOINT; }
                    //else if (mode.ToUpper().Contains("VALUE")) { data.InterlockMode = eInterlock.SETVALUE; }
                    //else { data.InterlockMode = eInterlock.NONE; }

                    remoteObject.Add(data.Name, data);
                }
                catch(Exception ex)
                {
                    LogHelper.Instance.ErrorLog.DebugFormat("[ERROR] InitializeDataInfo.ReadDataInfoList Failed : {0}", ex.Message);
                    return 0;
                }

            }

            return remoteObject.GetListCount();

            
        }


        public int ReadDeviceInfoList(SortedList<string, DeviceInfo> deviceList)
        {
            string dbFilePath = _config.GetIniValue("DATADB", "PATH");
            string queryCommand = @"SELECT * FROM master_handler";

            DataTable dt = DbHandler.Instance.ExecuteQuery(dbFilePath, queryCommand);

            foreach (DataRow dr in dt.Rows)
            {
                try
                {
                    DeviceInfo deviceInfo = new DeviceInfo();

                    deviceInfo.DeviceName = dr["driver_name"] as string;
                    deviceInfo.InstanceName = dr["assembly_name"] as string;
                    deviceInfo.FileName = dr["file_name"] as string;

                    string use = dr["use_driver"] as string;

                    if (!string.IsNullOrEmpty(use) && use.Equals("Y")) deviceInfo.Use = true;
                    else deviceInfo.Use = false;

                    string arg = string.Empty;
                    arg = dr["arg_0"] as string; deviceInfo.Arguments.Add(arg);
                    arg = dr["arg_1"] as string; deviceInfo.Arguments.Add(arg);
                    arg = dr["arg_2"] as string; deviceInfo.Arguments.Add(arg);
                    arg = dr["arg_3"] as string; deviceInfo.Arguments.Add(arg);
                    arg = dr["arg_4"] as string; deviceInfo.Arguments.Add(arg);
                    arg = dr["arg_5"] as string; deviceInfo.Arguments.Add(arg);
                    arg = dr["arg_6"] as string; deviceInfo.Arguments.Add(arg);
                    arg = dr["arg_7"] as string; deviceInfo.Arguments.Add(arg);
                    arg = dr["arg_8"] as string; deviceInfo.Arguments.Add(arg);
                    arg = dr["arg_9"] as string; deviceInfo.Arguments.Add(arg);

                    deviceInfo.Description = dr["description"] as string;

                    deviceList.Add(deviceInfo.DeviceName, deviceInfo);                
                    LogHelper.Instance.SystemLog.DebugFormat("[ReadDeviceInfoList] Device Handler Info Read Success.");
                }
                catch(Exception ex)
                {
                    LogHelper.Instance.ErrorLog.DebugFormat("[ERROR] Device Handler Info Read Failed : {0}", ex.Message);
                    return 0;
                }             
            }
            return deviceList.Count;

        }

            

    }
}
