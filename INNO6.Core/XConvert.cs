using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Reflection;
using System.Data;

namespace INNO6.Core
{

    public enum ByteOrder
    {
        LittleEndian,
        BigEndian
    }

    public enum Compatibility
    {
        Match,
        Compatible
    }

    public class XConvert
    {
        [DllImport("kernel32.dll")]
        public static extern UInt32 GetTickCount();

        public static string ConvertToString(short word, ByteOrder byteorder)
        {
            try
            {
                char ch1, ch2;
                string sTemp = "";

                ch1 = Convert.ToChar((word & 0x00FF));
                if (ch1 == 0) ch1 = ' ';
                ch2 = Convert.ToChar(((word >> 8) & 0x00FF));
                if (ch2 == 0) ch2 = ' ';

                if (byteorder == ByteOrder.BigEndian)
                {
                    sTemp += ch1.ToString();
                    sTemp += ch2.ToString();
                }
                else
                {
                    sTemp += ch2.ToString();
                    sTemp += ch1.ToString();
                }
                return sTemp;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.ErrorLog.DebugFormat(ex.Message);
                return "";
            }
        }

        public static string ConvertToString(short[] word, int startIndex, int count, ByteOrder byteorder)
        {
            try
            {
                string stemp = "";
                for (int i = 0; i < count; i++)
                {
                    short nData = word[startIndex + i];
                    stemp += ConvertToString(nData, byteorder);
                }

                return stemp;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.ErrorLog.DebugFormat(ex.Message);
                return "";
            }
        }

        public static short ConvertToWord(string sdata, ByteOrder byteorder)
        {
            try
            {
                char ch1, ch2;

                if (sdata.Length <= 0) ch1 = (char)0x20;
                else ch1 = Convert.ToChar(sdata.Substring(0, 1));

                if (sdata.Length <= 1) ch2 = (char)0x20;
                else ch2 = Convert.ToChar(sdata.Substring(1, 1));

                if (ch1 == ' ') ch1 = (char)0x20;
                if (ch2 == ' ') ch2 = (char)0x20;
                if (ch1 == '\r') ch1 = (char)0x00;
                if (ch2 == '\r') ch2 = (char)0x00;


                ushort ntemp;
                if (byteorder == ByteOrder.BigEndian)
                {
                    ntemp = (ushort)((ch2 << 8) | ch1);
                }
                else
                {
                    ntemp = (ushort)((ch1 << 8) | ch2);
                }

                return (short)ntemp;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.ErrorLog.DebugFormat(ex.Message);
                return 0;
            }
        }

        public static void ConvertToWord(string sdata, ref short[] stream, int startIndex, int count, ByteOrder byteorder)
        {
            try
            {
                string stemp = "";
                for (int i = 0; i < count; i++)
                {
                    if (sdata == null)
                    {
                        stemp = "";
                    }
                    else if (sdata.Length >= 2)
                    {
                        stemp = sdata.Substring(0, 2);
                        sdata = sdata.Remove(0, 2);
                    }
                    else if (sdata.Length >= 1)
                    {
                        stemp = sdata.Substring(0, 1);
                        sdata = sdata.Remove(0, 1);
                    }
                    else
                    {
                        stemp = "";
                    }

                    stream[startIndex + i] = ConvertToWord(stemp, byteorder);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.ErrorLog.DebugFormat(ex.Message);
            }
        }

        public static int ConvertBcdToInt(short value)
        {
            //string sBin = ConvertToBin(value);
            //string temp = "";
            //StringBuilder sb = new StringBuilder();

            //for (int i = 0; i < 4; i++)
            //{
            //    temp = sBin.Substring(i * 4, 4);

            //    sb.Append(Convert.ToInt32(temp, 2));
            //}

            //return sb.ToString();

            int nReturnVal = 0;

            nReturnVal = value & 0X0F;
            nReturnVal += (value >> 4 & 0X0F) * 10;
            nReturnVal += (value >> 8 & 0X0F) * 100;
            nReturnVal += (value >> 12 & 0X0F) * 1000;

            return nReturnVal;
        }

        public static short ConvertToAscii(string value)
        {
            int nReturnVal = 0;

            char[] temp;

            temp = value.ToCharArray();

            if (value.Length > 1)
            {
                nReturnVal = temp[1] << 8 | temp[0];
            }
            else nReturnVal = temp[0];

            return (short)nReturnVal;
        }

        public static short ConvertIntToBcd(int value)
        {
            int nReturnVal = 0;

            int[] nTmp = new int[4];
            nTmp[0] = value / 1000;
            nTmp[1] = (value - nTmp[0] * 1000) / 100;
            nTmp[2] = (value - nTmp[0] * 1000 - nTmp[1] * 100) / 10;
            nTmp[3] = (value - nTmp[0] * 1000 - nTmp[1] * 100 - nTmp[2] * 10);

            nReturnVal = nTmp[0] << 12 | nTmp[1] << 8 | nTmp[2] << 4 | nTmp[3];

            return (short)nReturnVal;
        }

        public static string ConvertToBin(short value)
        {
            string sBin = "";

            try
            {
                sBin = Convert.ToString(value, 2).PadLeft(16, '0');
            }
            catch (Exception err)    //Don't Use XFunc.ExceptionHandler.Add(err);
            {
                sBin = err.Message.ToString();
            }

            return sBin.ToUpper();
        }

        // jemoon
        // Class Type의 호환성 검사
        //
        public static bool CheckTypeCompatibility(Type type, Type target, Compatibility compatibility)
        {
            if (target == null)
            {
                return false;
            }
            else if (type == target)
            {
                return true;
            }
            else if (type.IsInterface)
            {
                Type[] types = target.GetInterfaces();
                foreach (Type typeofInterface in types)
                {
                    if (type == typeofInterface)
                    {
                        return true;
                    }
                }

                return false;
            }
            else if (compatibility == Compatibility.Match)
            {
                return false;
            }
            else
            {
                return CheckTypeCompatibility(type, target.BaseType, compatibility);
            }
        }

        // jemoon
        // Class의 모든 Property정보를 반환
        //
        public static PropertyInfo[] GetProperties(object obj)
        {
            PropertyInfo[] propertyInfos = obj.GetType().GetProperties();
            return propertyInfos;
        }

        // jemoon
        // Class의 Property정보중에서 Type호환성이 있는 정보만 반환
        //
        public static PropertyInfo[] GetProperties(object obj, Type type, Compatibility compatibility)
        {
            List<PropertyInfo> propertyInfos = new List<PropertyInfo>();
            PropertyInfo[] propertyInfosAll = GetProperties(obj);
            // 해당 object의 모든 property에서 type이 호환되는것만 가져온다.
            foreach (PropertyInfo info in propertyInfosAll)
            {
                if (true == CheckTypeCompatibility(type, info.PropertyType, compatibility))
                {
                    propertyInfos.Add(info);
                }
            }

            return propertyInfos.ToArray();
        }

        // jemoon
        // device name에 공백, 특수문자 등을 '_'로 치환
        public static string FilterigName(string name)
        {
            char[] chars = name.ToCharArray();
            int count = chars.Length;
            for (int i = 0; i < count; i++)
            {
                if (!char.IsLetterOrDigit(chars[i]))
                {
                    chars[i] = '_';
                }
            }

            string result = new string(chars);
            return result;
        }

        public static List<Exception> ExceptionHandler = new List<Exception>();

        // 구조체를 byte 배열로 변환해주는 함수
        public static byte[] StructureToByte(object obj)
        {
            byte[] data = null;

            try
            {
                int datasize = Marshal.SizeOf(obj);         // 구조체에 할당된 메모리의 크기를 구한다.
                data = new byte[datasize];                  // 구조체가 복사될 배열
                IntPtr buff = Marshal.AllocHGlobal(datasize);// 비관리 메모리 영역에 구조체 크기만큼의 메모리를 할당한다.
                Marshal.StructureToPtr(obj, buff, false);   // 할당된 구조체 객체의 주소를 구한다.
                Marshal.Copy(buff, data, 0, datasize);      // 구조체 객체를 배열에 복사
                Marshal.FreeHGlobal(buff);                  // 비관리 메모리 영역에 할당했던 메모리를 해제함
                //return data; // 배열을 리턴
            }
            catch (Exception err)
            {
                err.Message.ToString();
                data = null;
            }

            return data;
        }

        //byte 배열을 구조체로 변환해주는 함수
        public static object ByteToStructure(byte[] data, Type type)
        {
            object obj = null;

            try
            {
                IntPtr buff = Marshal.AllocHGlobal(data.Length);// 배열의 크기만큼 비관리 메모리 영역에 메모리를 할당한다.
                Marshal.Copy(data, 0, buff, data.Length);       // 배열에 저장된 데이터를 위에서 할당한 메모리 영역에 복사한다.
                obj = Marshal.PtrToStructure(buff, type);       // 복사된 데이터를 구조체 객체로 변환한다.
                Marshal.FreeHGlobal(buff);                      // 비관리 메모리 영역에 할당했던 메모리를 해제함
                if (Marshal.SizeOf(obj) != data.Length)         // 구조체와 원래의 데이터의 크기 비교
                    obj = null;                                 // 크기가 다르면 null 리턴
                //return obj; // 구조체 리턴
            }
            catch (Exception err)
            {
                LogHelper.Instance.ErrorLog.DebugFormat(err.Message);
                err.Message.ToString();
                obj = null;
            }

            return obj;
        }

        public static object ByteToStructure(byte[] data, Type type, int size)
        {
            object obj = null;

            try
            {
                IntPtr buff = Marshal.AllocHGlobal(data.Length);// 배열의 크기만큼 비관리 메모리 영역에 메모리를 할당한다.
                Marshal.Copy(data, 0, buff, data.Length);       // 배열에 저장된 데이터를 위에서 할당한 메모리 영역에 복사한다.
                obj = Marshal.PtrToStructure(buff, type);       // 복사된 데이터를 구조체 객체로 변환한다.
                Marshal.FreeHGlobal(buff);                      // 비관리 메모리 영역에 할당했던 메모리를 해제함
                //if (size != data.Length)         // 구조체와 원래의 데이터의 크기 비교
                //    obj = null;                                 // 크기가 다르면 null 리턴
                //return obj; // 구조체 리턴
            }
            catch (Exception err)
            {
                LogHelper.Instance.ErrorLog.DebugFormat(err.Message);
                err.Message.ToString();
                obj = null;
            }

            return obj;
        }
    }
}
