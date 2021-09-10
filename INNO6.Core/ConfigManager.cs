using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;

namespace INNO6.Core
{
    public class ConfigManager
    {
        private string _path = @".\Config.ini";

        public ConfigManager()
        {

        }
        public ConfigManager(string sPath)
        {
            _path = Path.GetFullPath(sPath);
        }

        public string[] GetIniValue(string section)
        {
            byte[] ba = new byte[255];
            uint Flag = GetPrivateProfileSection(section, ba, 255, _path);
            return Encoding.Default.GetString(ba).Split(new char[1] { '\0' }, StringSplitOptions.RemoveEmptyEntries);
        }

        public string GetIniValue(string section, string key)
        {
            StringBuilder sb = new StringBuilder(500);
            int Flag = GetPrivateProfileString(section, key, "", sb, 500, _path);
            return sb.ToString();
        }

        public uint GetIniValue(string section, string key, int defaultVal)
        {
            return GetPrivateProfileInt(section, key, defaultVal, _path);
        }

        public bool SetIniValue(string section, string key, string value)
        {
            return (WritePrivateProfileString(section, key, value, _path));
        }

        [DllImport("kernel32")]
        public static extern int GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString, int nSize, string lpFileName);

        [DllImport("kernel32")]
        public static extern bool WritePrivateProfileString(string lpAppName, string lpKeyName, string lpString, string lpFileName);

        [DllImport("kernel32")]
        public static extern uint GetPrivateProfileInt(string lpAppName, string lpKeyName, int nDefault, string lpFileName);

        [DllImport("kernel32")]
        public static extern uint GetPrivateProfileSection(string lpAppName, byte[] lpPairValues, uint nSize, string lpFileName);

        [DllImport("kernel32")]
        public static extern uint GetPrivateProfileSectionNames(byte[] lpSections, uint nSize, string lpFileName);
    }
}
