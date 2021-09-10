using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CIM.Log;

namespace INNO6.Core
{
    public class Logger
    {
        public static string IniPath = @"./Server.Config.ini";
        public static readonly ILog SystemLog = LogManager.GetNewLogger("System", GetLogPathSetting("LOG", "SYSTEM"), "System", true, ConfigManager.GetPrivateProfileInt("LogKeepDay", "System", 30, IniPath));

        public static string GetLogPathSetting(string section, string key)
        {
            StringBuilder sb = new StringBuilder(500);
            int Flag = ConfigManager.GetPrivateProfileString(section, key, "", sb, 500, IniPath);
            return sb.ToString();
        }
    }
}
