using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using INNO6.Core;
using INNO6.IO;
using CIM.Manager;
using CIM.Common;
using SYSWIN.Secl;
using System.Globalization;
using System.Runtime.InteropServices;

namespace CIM.Host.SECSII
{ 

    public class S2F18 : SFMessage
    {

        [DllImport("kernel32.dll")]
        private extern static void GetLocalTime(ref SYSTEMTIME lpSystemTime);

        [DllImport("kernel32.dll")]
        private extern static uint SetLocalTime(ref SYSTEMTIME lpSystemTime);


        public struct SYSTEMTIME
        {
            public ushort wYear;
            public ushort wMonth;
            public ushort wDayOfWeek;
            public ushort wDay;
            public ushort wHour;
            public ushort wMinute;
            public ushort wSecond;
            public ushort wMilliseconds;
        }


        public S2F18(SECSDriverBase driver)
            : base(driver)
        {
            Stream = 2; Function = 18;
        }

        private void SetSystemTime(string Time)
        {
            // Call the native GetSystemTime method
            // with the defined structure.
            if (Time.Length < 14) return;

            SYSTEMTIME st = new SYSTEMTIME
            {
                wYear = (ushort)Convert.ToUInt32(Time.Substring(0, 4)),
                wMonth = (ushort)Convert.ToUInt32(Time.Substring(4, 2)),
                wDay = (ushort)Convert.ToUInt32(Time.Substring(6, 2)),
                wHour = (ushort)Convert.ToUInt32(Time.Substring(8, 2)),
                wMinute = (ushort)Convert.ToUInt32(Time.Substring(10, 2)),
                wSecond = (ushort)Convert.ToUInt32(Time.Substring(12, 2))
            };
            uint rst = SetLocalTime(ref st);

            // Set the system clock ahead one hour.
            //systime.wHour = (ushort)(systime.wHour + 1 % 24);
            //SetSystemTime(ref systime);
            //MessageBox.Show("New time: " + systime.wHour.ToString() + ":"
            //    + systime.wMinute.ToString());
        }

        private SYSTEMTIME GetSystemTime()
        {
            // Call the native GetSystemTime method
            // with the defined structure.
            SYSTEMTIME stime = new SYSTEMTIME();
            GetLocalTime(ref stime);

            return stime;

            // Show the current time.           
            //MessageBox.Show("Current Time: " +
            //    stime.wHour.ToString() + ":"
            //    + stime.wMinute.ToString());
        }

        public override void DoWork(string driverName, object obj)
        {
            LogHelper.Instance.BizLog.DebugFormat("[{0}]DoWork : {1}, {2}", this.GetType().Name, driverName, obj);
            CommonData.Instance.OnStreamFunctionAdd("MAIN", "H->E", "TIMESET", "S2F18", null, "Data and Time Data Set", null);
            SecsMessage primaryMessage = obj as SecsMessage;

            string sTIME = string.Empty;

            sTIME = primaryMessage.GetItem().GetAscii();                        // A14 TIME (YYYYMMDDHHMMSS)

            //PC time set
            try
            {
                SetSystemTime(sTIME);

                SYSTEMTIME st = GetSystemTime();

                StringBuilder CurrentTime = new StringBuilder(string.Empty);

                CurrentTime.AppendFormat("{0}{1}{2}{3}{4}{5}",
                    st.wYear.ToString("0000"),
                    st.wMonth.ToString("00"),
                    st.wDay.ToString("00"),
                    st.wHour.ToString("00"),
                    st.wMinute.ToString("00"),
                    st.wSecond.ToString("00"));

                string sendplcTime = CurrentTime.ToString();

                foreach (MODULE m in CommonData.Instance.MODULE_SETTINGS)
                {
                    bool bResult;
                    string dateTimeSetReplyBitTag = string.Format("i{0}.Reply.DateTimeSet", m.MODULE_NAME);
                    string dateTimeSetBitTag = string.Format("o{0}.Send.DateTimeSet", m.MODULE_NAME);
                    string dateTimeSetDataTag = string.Format("o{0}.DatetimeSet.Datetime", m.MODULE_NAME);

                    if (DataManager.Instance.GET_INT_DATA(dateTimeSetReplyBitTag, out bResult) == 1)
                    {
                        //Console.WriteLine("{0} Reply Bit On Error : {1}", dateTimeSetReplyBitTag, 1);
                        LogHelper.Instance.PLCDataChange.DebugFormat("{0} Reply Bit On Error : {1}", dateTimeSetReplyBitTag, 1);
                    }
                    else
                    {
                        DataManager.Instance.SET_STRING_DATA(dateTimeSetDataTag, sendplcTime);
                        DataManager.Instance.SET_INT_DATA(dateTimeSetBitTag, 1);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.ErrorLog.DebugFormat("[ERROR] System time set failed : {0}", ex.Message);
            }
        }
    }
}
