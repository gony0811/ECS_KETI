using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using Microsoft.VisualBasic;

namespace INNO6.Core
{
    public static class AppUtil
    {


        public static string NullToZero(object Value)
        {
            return Value == null ? "0" : Value.ToString();
        }

        public static string NullToString(object Value)
        {
            return Value == null ? "" : Value.ToString();
        }

        public static string ToAscii(string sValue, int iSize, string sZero = "")
        {
            if (sValue == null) sValue = "";

            if (sZero == "0")
                return sValue.PadRight(iSize, '0').Substring(0, iSize);
            else
                return sValue.PadRight(iSize).Substring(0, iSize);
        }

        public static string ConvertHexToBinary(short[] tmpBuf)
        {
            Int16 points = (Int16)tmpBuf.Length;

            byte[] bytes = new byte[points * 2];
            string rtnString = "";
            string[] hexs = new string[points];
            char[] rtnChar = new char[points * 2];
            int iCount = points - 1;

            foreach (int iGetValue in tmpBuf)
            {
                if (iCount >= 0)
                {
                    hexs[iCount] = string.Format("{0:X}", iGetValue);
                }
                iCount--;
            }
            iCount = 0;
            foreach (string st in hexs)
            {
                if (st != null)
                {
                    rtnString += convertHexToBinary(st);
                }
            }
            return rtnString;

        }

        /// <summary>
        /// Binary -> Hex 로 변경
        /// </summary>
        /// <param name="binary"></param>
        /// <returns></returns>
        public static string ConvertBinaryToHex(string binary)
        {
            StringBuilder sbHex = new StringBuilder(binary.Length / 8 + 1);      // TODO: check all 1's or 0's... Will throw otherwise      
            int mod4Len = binary.Length % 8;

            if (mod4Len != 0)
            {
                // pad to length multiple of 8         
                binary = binary.PadLeft(((binary.Length / 8) + 1) * 8, '0');
            }

            for (int i = 0; i < binary.Length; i += 8)
            {
                string eightBits = binary.Substring(i, 8);
                sbHex.AppendFormat("{0:X2}", Convert.ToByte(eightBits, 2));
            }

            return sbHex.ToString();
        }

        public static string ConvertHexToString(short[] tmpBuf)
        {
            Int16 points = (Int16)tmpBuf.Length;

            byte[] bytes = new byte[points * 2];
            StringBuilder sbString = new StringBuilder("");
            string[] hexs = new string[points];
            char[] rtnChar = new char[points * 2];
            int iCount = 0;

            foreach (int iGetValue in tmpBuf)
            {
                if (iCount < points)
                {
                    hexs[iCount] = string.Format("{0:X}", iGetValue);
                }
                iCount++;
            }
            if (points < 2) return hexs[0];

            iCount = 0;
            foreach (string st in hexs)
            {
                if (st != "0" && st != null)
                {
                    //Byte 로 변환
                    if (st.Length == 4)
                    {
                        bytes[iCount * 2 + 1] = HexToByte(Mid(st, 0, 2));
                        bytes[iCount * 2 + 0] = HexToByte(Mid(st, 2, 2));
                    }
                    else if (st.Length == 2)
                    {
                        bytes[iCount * 2 + 0] = HexToByte(Mid(st, 0, 2));
                    }
                    iCount++;
                }
            }
            //바이트를 Char 로 변환
            rtnChar = convertHexToChar(bytes);

            foreach (char ch in rtnChar)
            {
                if (ch != '\0')
                    sbString.Append(ch.ToString());
            }
            return sbString.ToString();
        }

        public static byte[] ConvertHexToByte(short[] tmpBuf)
        {
            Int16 points = (Int16)tmpBuf.Length;

            byte[] bytes = new byte[points * 2];
            string[] hexs = new string[points];
            int iCount = 0;

            foreach (int iGetValue in tmpBuf)
            {
                if (iCount < points)
                {
                    hexs[iCount] = string.Format("{0:X2}", iGetValue);
                    if (hexs[iCount].Length > 4)
                        hexs[iCount] = Mid(hexs[iCount], 4, 4);
                    else if (hexs[iCount].Length == 3)
                        hexs[iCount] = "0" + hexs[iCount];
                }
                iCount++;
            }
            iCount = 0;
            foreach (string st in hexs)
            {
                if (st != "0" && st != null)
                {
                    //Byte 로 변환
                    if (st.Length == 4)
                    {
                        bytes[iCount * 2 + 0] = HexToByte(Mid(st, 0, 2));
                        bytes[iCount * 2 + 1] = HexToByte(Mid(st, 2, 2));
                    }
                    else if (st.Length == 2)
                    {
                        bytes[iCount * 2 + 0] = HexToByte(Mid(st, 0, 2));
                    }
                    iCount++;
                }
            }
            return bytes;
        }

        private static string convertHexToBinary(string hexvalue)
        {
            StringBuilder sbHexValue = new StringBuilder("");

            //hex값을 binary로 변화해 주는 함수 
            string binaryval = Convert.ToString(Convert.ToInt32(hexvalue, 16), 2);

            if (binaryval.Length < 16)
            {
                for (int i = binaryval.Length; i < 16; i++)
                {
                    sbHexValue.Append("0");
                }
                sbHexValue.Append(binaryval);
                binaryval = sbHexValue.ToString();
            }
            return binaryval;
        }

        private static char[] convertHexToChar(byte[] bytes)
        {
            //바이트를 Char 로 변환
            return ASCIIEncoding.ASCII.GetChars(bytes);
        }

        public static bool[] ConvertByteToBoolArray(byte b)
        {
            // prepare the return result
            bool[] result = new bool[8];

            // check each bit in the byte. if 1 set to true, if 0 set to false
            for (int i = 0; i < 8; i++)
                result[i] = (b & (1 << i)) == 0 ? false : true;

            // reverse the array
            Array.Reverse(result);

            return result;
        }

        public static byte ConvertBoolArrayToByte(bool[] source)
        {
            byte result = 0;
            // This assumes the array never contains more than 8 elements!
            int index = 8 - source.Length;

            // Loop through the array
            foreach (bool b in source)
            {
                // if the element is 'true' set the bit at that position
                if (b)
                    result |= (byte)(1 << (7 - index));

                index++;
            }

            return result;
        }


        public static byte HexToByte(string hex)
        {
            if (hex.Length > 2 || hex.Length <= 0)
                throw new ArgumentException("hex must be 1 or 2 characters in length");
            byte newByte = byte.Parse(hex, System.Globalization.NumberStyles.HexNumber);
            return newByte;
        }

        public static string Mid(string Text, int Startint, int Endint)
        {
            string ConvertText;
            if (Startint < Text.Length || Endint < Text.Length)
            {
                ConvertText = Text.Substring(Startint, Endint);
                return ConvertText;
            }
            else
            {
                return Text;
            }
        }

        public static string IntToHex(int nor)
        {
            byte[] bytes = BitConverter.GetBytes(nor);
            string hexString = "";
            for (int ii = 0; ii < bytes.Length; ii++)
            {
                hexString += bytes[ii].ToString("X2");
            }
            return hexString;
        }

        public static string ConvertHexToAscii(string hexString)
        {
            ///2014.11.05 검증 완료 By SJ

            StringBuilder sbASCII = new StringBuilder();
            string sConvert = "";

            for (int i = 0; i < hexString.Length; i += 2)
            {
                sConvert = Char.ConvertFromUtf32(Convert.ToInt32(hexString.Substring(i, i + 2), 16));

                if (sConvert == "\0")
                {
                    sConvert = " ";
                }

                sbASCII.Append(sConvert);

            }
            return sbASCII.ToString();
        }

        ///2014.06.27 DColl 작성하면서 추가함 By SJ
        ///
        public static double ArrayAverage(double[] arr)
        {
            double total = 0;

            for (int i = 0; i < arr.Length; i++)
            {
                total = total + arr[i];
            }

            return total / arr.Length;
        }

        public static double ArrayMaxValue(double[] arr)
        {
            int maxIndex = 0;

            for (int i = 1; i < arr.Length; i++)
            {
                if (arr[i] > arr[maxIndex])
                {
                    maxIndex = i;
                }
            }

            return arr[maxIndex];
        }

        public static int ArrayMaxValueIndex(double[] arr)
        {
            int maxIndex = 0;

            for (int i = 1; i < arr.Length; i++)
            {
                if (arr[i] > arr[maxIndex])
                {
                    maxIndex = i;
                }
            }

            return maxIndex;
        }

        public static int[] ArrayMaxValueIndex_TwoDimension(double[][] arr)
        {
            int maxIndexX = 0;
            int maxIndexY = 0;

            for (int i = 0; i < arr.Length; i++)
            {
                for (int j = 0; j < arr[i].Length; j++)
                {
                    if (arr[i][j] > arr[maxIndexX][maxIndexY])
                    {
                        maxIndexX = i; maxIndexY = j;
                    }
                }
            }

            return new int[] { maxIndexX, maxIndexY };
        }

        public static double ArrayMinValue(double[] arr)
        {
            int minIndex = 0;

            for (int i = 1; i < arr.Length; i++)
            {
                if (arr[i] < arr[minIndex])
                {
                    minIndex = i;
                }
            }

            return arr[minIndex];
        }

        public static double ArrayMedianValue(double[] arr)
        {
            ///배열의 값중 중간 값을 Return 해주는 함수!! -> 검증완료 By SJ
            double[] arrClone = new double[arr.Length];
            ArrayList SortArray = new ArrayList();

            arrClone = arr;

            ///정렬하기 위해 ArrayList 로 복사한다
            SortArray.AddRange(arrClone);
            SortArray.Sort();   //오름차순 정렬

            ///정렬한 후 Return 하기 위해 다시 Array 에 저장한다
            arrClone = (double[])SortArray.ToArray(typeof(double));

            return arrClone[(int)(Math.Round(arrClone.Length / 2.0) - 1)];
        }

        public static string BinaryTo16Word(string strBinary)
        {
            ///2014.07.04 입력된 2진수가 16자리가 아니면 16자리가 되도록 숫자앞에 "0" 을 채워 반환한다 -> 검증완료. By SJ 
            string rtnStr = "";

            if (strBinary.Length > 0)
                rtnStr = strBinary.PadRight(16, '0');   // 숫자앞에 "0"을 채움
            else
                rtnStr = "-1";

            return rtnStr;
        }

        public static string DecToBinary(int intDec)
        {
            ///2014.07.04 입력된 10진수를 2진수 String 으로 변환해서 반환한다 -> 검증완료. By SJ 
            string rtnStr = "";

            if (intDec >= 0)
            {
                rtnStr = Convert.ToString(intDec, 2);
            }
            else
            {
                rtnStr = "-1";
            }

            return rtnStr;
        }

        /// <summary>
        /// 2014.07.04 입력된 10진수를 16진수 4워드 String 으로 변환해서 반환한다 -> 검증완료. By SJ 
        /// </summary>
        /// <param name="iDec"></param>
        /// <returns></returns>
        public static string ConvertDecToHexFor4Word(int iDec)
        {
            string sResult = string.Empty;
            string sChangeValue = string.Empty;

            sChangeValue = String.Format("{0:X04}", iDec);

            if (sChangeValue.Length < 5)
            {
                sResult = sChangeValue;
            }
            else
            {
                if (sChangeValue.Length == 8)
                {
                    sResult = sChangeValue.Substring(4, 4);
                }
            }

            return sResult;
        }

        /// <summary>
        /// 2014.07.17 SaveDCollGlassData 함수 만들면서 변수의 값이 계산가능한 숫자값인지를 검증하기 위해 만듬 By SJ
        /// </summary>
        /// <param name="Expression"></param>
        /// <returns></returns>
        public static bool IsNumeric(object Expression)
        {
            /// Expression 변수는 보통 String 이며 변수에 숫자가 아닌 문자가 있으면 false 를 리턴한다.
            /// TryParse 메서드의 반환 값을 수집하는 변수입니다.
            bool isNum;

            /// Define variable to collect out parameter of the TryParse method. If the conversion fails, the out parameter is zero.
            double retNum;

            /// TryParse 메서드의 매개 변수를 수집하는 변수를 정의합니다.변환이 실패 할 경우, 아웃 파라미터는 제로이다.
            /// 변환이 실패 할 경우 TryParse 메서드는 예외를 생성하지 않습니다. 변환이 통과하면, 참이 반환됩니다. 그렇지 않은 경우, 거짓이 반환됩니다.
            isNum = Double.TryParse(Convert.ToString(Expression), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);
            return isNum;
        }

        /// <summary>
        /// 2014.09.10 AsciiData 를 int 로 Convert 하기 위해 함수 생성 By SJ
        /// </summary>
        /// <param name="sAscii"></param>
        /// <returns></returns>
        public static int ConvertAsciiToInt(char ascii)
        {
            ///주의! 이 함수는 Ascii 문자 하나만 매개변수로 입력해야됩니다. SJ 검증 완료
            int iValue = -1;

            char CharValue = ascii;
            iValue = Convert.ToInt32(CharValue);

            return iValue;
        }

        /// <summary>
        /// 2014.07.12 아스키를 Hex 로 변경할때는 PLC 에서 필요한 자리가 변경되어야 해서 전용함수를 만듬 By SJ
        /// 2014.09.10 PlcDataWrite Class 에 있던 함수를 AppUtil 로 이동시킴 By SJ
        /// </summary>
        /// <returns></returns>
        public static string ConvertAsciiToHexForPLC(string StringData, string strWordSize)
        {
            //string rtnReturn = "";
            //string sChange = "";
            string HexValue = "";
            StringBuilder sbChange = new StringBuilder("");
            //int iLenth = -1;
            int iWordSize = Convert.ToInt32(strWordSize);

            ///1.PLC 에 Write 하기 위해 문자의 순서를 변경한다
            for (int i = 0; i < StringData.Length; i++)
            {
                if (i % 2 == 0)
                {
                    ///1문자 기준으로 0번째 문자부터 1개를 가져온다
                    sbChange.Append(StringData.Substring(1 * (i + 1), 1));
                    //sbChange.Append(StringData[1 * (1 + 1)]);
                    //sChange += StringData.Substring(1 * (i + 1), 1);
                }
                else
                {
                    ///1문자 기준으로 1번째 문자부터 1개를 가져온다
                    sbChange.Append(StringData.Substring(1 * (i - 1), 1));
                    //sbChange.Append(StringData[1 * (i - 1)]);
                    //sChange += StringData.Substring(1 * (i - 1), 1);
                }
            }


            ///2. 문자를 16진수로 변경한다
            foreach (char EachChar in sbChange.ToString())
            {
                int iValue = Convert.ToInt32(EachChar);
                HexValue += Convert.ToString(iValue, 16);
            }

            //char[] CharValue = sbChange.ToString().ToCharArray();
            //foreach (char EachChar in CharValue)
            //{
            //    int iValue = Convert.ToInt32(EachChar);
            //    HexValue += Convert.ToString(iValue, 16);
            //}

            HexValue = HexValue.PadLeft(4 * iWordSize, '0');
            ///3. Data 의 길이를 맞춘다
            //iLenth = 4 * iWordSize - HexValue.Length;
            //for (int kk = 0; kk < iLenth; kk++)
            //{
            //    HexValue = "0" + HexValue;
            //}

            ///시스템 부하를 줄이기 위해 위의 For 문을 삭제하고 아래의 PadLeft 함수를 사용함...검증 필요 By SJ
            //HexValue = HexValue.PadLeft(iLenth, '0');

            return HexValue;

            //rtnReturn = HexValue;

            //return rtnReturn;
        }

        /// <summary>
        /// 이 함수는 PLC 1 Word 에 해당되는 Ascii Data 를 변경하는데 최적화 되어 있습니다.(출력은 2개 문자로 됩니다)
        /// ex) "AB", "A ", " B"
        /// 2014.09.17 이 함수 호출 시 int intData 는 반드시 PLC 1Word(최대 문자 2개) 에 해당하는 값만 변수로 입력
        /// </summary>
        /// <param name="iData"></param>
        /// <returns></returns>
        public static string GetIntToAscii(int iData)
        {
            StringBuilder sbIntToASCII = new StringBuilder("");

            ///1. 기존 10진수 Data를 16진수로 변경
            string sResultPLCIO = Convert.ToString(iData, 16);

            if (sResultPLCIO.Length < 4)
            {
                sResultPLCIO = sResultPLCIO.PadLeft(4, '0');
            }

            int iStartIndex = 0;
            for (int i = 0; i < 2; i++)
            {
                if (i == 0) iStartIndex = 2;
                else iStartIndex = 0;
                int iValue = Convert.ToInt32(sResultPLCIO.Substring(iStartIndex, 2), 16);
                string sExam = Convert.ToString((char)iValue);
                if (sExam == "\0") sExam = " ";
                sbIntToASCII.Append(sExam);
            }

            return sbIntToASCII.ToString();
        }

        /// <summary>
        /// 10진수 String 을 2진수로 변환한다 (코드 정리 : GetBit() 함수를 이름 변경하고 위치 이동)
        /// </summary>
        /// <param name="lData"></param>
        /// <param name="cnt"></param>
        /// <returns></returns>
        public static int ConvertStringToBit(long lData, int iBit)
        {
            string sReadResult = "";
            string strTemp = "";
            int iResultBit = -1;

            ///1.입력된 10진수를 2진수로 변환한다
            strTemp = Convert.ToString(lData, 2);

            ///2. 2진수를 16자리로 자리맞춤한다
            strTemp = strTemp.PadLeft(16, '0');

            ///3. 16자리가 초과된 2진수 배열이면 16자리로만 자른다(-32767 등의 10진수 값이 입력되면!)
            sReadResult = strTemp.Substring(strTemp.Length - 16, 16);

            ///4. 원하는 자리수의 2진수 값을 String 으로 반환한다
            sReadResult = sReadResult.Substring(15 - iBit, 1);

            iResultBit = Convert.ToInt32(sReadResult);

            return iResultBit;
        }

        /// <summary>
        /// 프로그램 디버그시 i값을 변경해서 하면 됨.
        /// </summary>
        /// <param name="iDebug"></param>
        public static void DebugMode(int iDebug)
        {
            int i = 0;
            if (iDebug == i)
            {
                i = iDebug;
            }
        }

        /// <summary>
        /// 141018 압력 단위 Pa 값을 PLC 에 가수, 지수로 구분하여 Write 하기 위해 생성 By SJ Japan
        /// </summary>
        /// <param name="sValue"></param>
        /// <returns></returns>
        public static int GetMetissa(string sValue)
        {
            string strTemp = string.Empty;
            int iIndex = -1;
            int iResult = 0;

            if (sValue.IndexOf(".") == 1)       //0.0001 이면
            {
                sValue = sValue.Replace("0.", "");

                for (int ii = 0; ii < sValue.Length; ii++)
                {
                    strTemp = sValue.Substring(ii, 1);

                    if (strTemp != "0")
                    {
                        iIndex = ii;
                        break;
                    }
                }

                iResult = Convert.ToInt32(sValue.Substring(iIndex));
            }
            else        //40.0 이면
            {
                sValue = sValue.Replace(".0", "");

                for (int ii = 0; ii < sValue.Length; ii++)
                {
                    strTemp = sValue.Substring(sValue.Length - ii - 1, 1);

                    if (strTemp != "0")
                    {
                        iIndex = ii;
                        break;
                    }
                }

                iResult = Convert.ToInt32(sValue.Substring(0, sValue.Length - iIndex));
            }

            return iResult;
        }

        /// <summary>
        /// 141018 압력 단위 Pa 값을 PLC 에 가수, 지수로 구분하여 Write 하기 위해 생성 By SJ Japan
        /// </summary>
        /// <param name="sValue"></param>
        /// <returns></returns>
        public static int GetExponent(string sValue)
        {
            int iResult = 0;
            string sChagneValue = string.Empty;

            if (sValue.IndexOf(".") == 1)        //값이 0.0002 이면
            {
                sValue = sValue.Replace("0.", "");
                iResult = (sValue.Length - 1) * -1;     //Tokki Request
                //iResult = (sValue.Length) * -1;

                sChagneValue = ConvertDecToHexFor4Word(iResult);
                iResult = Convert.ToInt32(sChagneValue, 16);
            }
            else       //값이 40.000 이면
            {
                if (sValue.Contains("."))
                {
                    sValue = sValue.Substring(0, sValue.IndexOf("."));
                    iResult = sValue.Length - 1;
                }
                else
                {
                    iResult = sValue.Length - 1;      //Tokki Request
                }
            }

            return iResult;
        }

        /// <summary>
        /// 2014.12.26 Catch 에서 Exception 발생시 해당화면 Capture 하고 Log Save By SJ
        /// </summary>
        /// <param name="sLog"></param>
        public static void CaptureDebugScreen(string sLog)
        {
            //Log 남기고 Capture 화면 Save
            //Logger.DebugScreen.Info(" " + sLog);

            //2014.12.26 ManagePC 는 많은 Exception 이 발생하여 화면 캡처 안함 SJ
            if (sLog.Contains("ManagePC") == false)
            {
                //int iW_ScreenSize = 0;
                //int iH_ScreenSize = 0;
                //Size siScreen;
                //Bitmap biScreen;
                //Graphics grScreen;

                //iW_ScreenSize = Screen.PrimaryScreen.Bounds.Width;
                //iH_ScreenSize = Screen.PrimaryScreen.Bounds.Height;
                //siScreen = new Size(iW_ScreenSize, iH_ScreenSize);
                //biScreen = new Bitmap(iW_ScreenSize, iH_ScreenSize);
                //grScreen = Graphics.FromImage(biScreen);

                //grScreen.CopyFromScreen(0, 0, 0, 0, siScreen);
                //biScreen.Save(@"..\Logs\DebugScreen\" + DateTime.Now.ToString("yyyy년MM월dd일HH시mm분ss초") + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);                
            }
        }

        //Melsec에서 옮겨온 함수

        public static string CompareBin(short nValue)
        {
            string DectoBin;
            string Result = "";

            DectoBin = Convert.ToString(nValue, 2);
            DectoBin = DectoBin.PadLeft(16, '0');

            for (int i = DectoBin.Length - 1; i >= 0; i--)
            {
                Result = Result + DectoBin.Substring(i, 1);
            }

            return Result;
        }

        public static short CompareDec(string sValue)
        {
            string sResult = "";

            for (int i = 0; i < sValue.Length; i++)
            {
                sResult = sValue.Substring(i, 1) + sResult;
            }

            return Convert.ToInt16(sResult, 2);
        }

        public static string Reverse(string sValue)
        {
            string sResult = "";

            for (int i = sValue.Length - 1; i >= 0; i--)
            {
                sResult = sResult + sValue.Substring(i, 1);
            }

            return sResult;
        }

        public static String DecToHex(short nValue)
        {
            string sHex = "";

            try
            {
                sHex = Convert.ToString(nValue, 16).PadLeft(4, '0');
            }
            catch (Exception ex)
            {
                LogHelper.Instance.ErrorLog.DebugFormat(ex.Message);
                sHex = ex.Message.ToString();
            }
            return sHex.ToUpper();
        }

        public static String DecToHex(int nValue)
        {
            string sHex = "";

            try
            {
                sHex = Convert.ToString(nValue, 16).PadLeft(4, '0');
            }
            catch (Exception ex)
            {
                LogHelper.Instance.ErrorLog.DebugFormat(ex.Message);
                sHex = ex.Message.ToString();
            }
            return sHex.ToUpper();
        }

        public static String DecToHex(string sValue)
        {
            string sHex = "";

            try
            {
                sHex = Convert.ToString(Convert.ToInt32(sValue), 16).PadLeft(4, '0');
            }
            catch (Exception ex)
            {
                LogHelper.Instance.ErrorLog.DebugFormat(ex.Message);
                sHex = ex.Message.ToString();
            }
            return sHex.ToUpper();
        }

        public static string DecToAsc(string sValue)
        {
            string sTemp = "";

            sTemp = DecToHex(Convert.ToInt32(sValue));

            return HexToAsc(sTemp);
        }

        public static String DecToBin(short nValue)
        {
            string sBin = "";

            try
            {
                sBin = Convert.ToString(nValue, 2).PadLeft(16, '0');
            }
            catch (Exception ex)
            {
                LogHelper.Instance.ErrorLog.DebugFormat(ex.Message);
                sBin = ex.Message.ToString();
            }
            return sBin.ToUpper();
        }

        public static String DecToBin(string sValue)
        {
            string sBin = "";

            try
            {
                sBin = Convert.ToString(Convert.ToInt32(sValue), 2).PadLeft(16, '0');
            }
            catch (Exception ex)
            {
                LogHelper.Instance.ErrorLog.DebugFormat(ex.Message);
                sBin = ex.Message.ToString();
            }
            return sBin.ToUpper();
        }

        public static int HexToDec(string sValue)
        {
            try
            {
                int nValue;

                nValue = Convert.ToInt32(sValue, 16);

                return nValue;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.ErrorLog.DebugFormat(ex.Message);
                return -1;
            }
        }

        public static string HexToAsc(string sValue)
        {
            try
            {
                string sTemp = "";
                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < sValue.Length / 2; i++)
                {
                    int nDec = 0;

                    nDec = Convert.ToInt32(sValue.Substring(i * 2, 2), 16);

                    sTemp = Convert.ToChar(nDec).ToString();
                    sb.Insert(0, sTemp);
                }

                return sb.ToString();
            }
            catch (Exception ex)
            {
                LogHelper.Instance.ErrorLog.DebugFormat(ex.Message);
                return "";
            }
        }

        public static string HexToBin(string sValue)
        {
            try
            {
                string sTemp = "";
                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < sValue.Length / 2; i++)
                {
                    int nDec = 0;

                    nDec = Convert.ToInt32(sValue.Substring(i * 2, 2), 16);

                    sTemp = Convert.ToString(nDec, 2);
                    sb.Append(Strings.UCase(sTemp.PadLeft(8, '0')));
                }

                return sb.ToString();
            }
            catch (Exception ex)
            {
                LogHelper.Instance.ErrorLog.DebugFormat(ex.Message);
                return "";
            }
        }

        public static int AscToDec(string sValue)
        {
            int nDec = 0;
            string sTemp = AscToHex(sValue);

            nDec = Convert.ToInt32(sTemp, 16);

            return nDec;
        }

        public static string AscToHex(string sValue)
        {
            string sTemp = "";
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < sValue.Length; i++)
            {
                int nDec = Strings.Asc(sValue.Substring(i, 1));

                sTemp = Convert.ToString(nDec, 16);
                sb.Insert(0, Strings.UCase(sTemp.PadLeft(2, '0')));
            }

            if (sValue.Length == 1)
                sb.Insert(0, "00");

            return sb.ToString();
        }

        public static string AscToBin(string sValue)
        {
            string sTemp = "";
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < sValue.Length; i++)
            {
                int nDec = Strings.Asc(sValue.Substring(i, 1));

                sTemp = Convert.ToString(nDec, 2); // 10진수를 2진수로
                sb.Insert(0, Strings.UCase(sTemp.PadLeft(8, '0')));
            }

            if (sValue.Length == 1)
                sb.Insert(0, "00000000");

            return sb.ToString();

        }

        public static int BinToDec(string sValue)
        {
            try
            {
                int nDec = 0;
                nDec = Convert.ToInt32(sValue, 2);

                return nDec;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.ErrorLog.DebugFormat(ex.Message);
                return -1;
            }
        }

        public static string BinToAsc(string sValue)
        {
            try
            {
                int nLength = sValue.Length / 8;
                string sTemp = "";
                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < nLength; i++)
                {
                    int nDec = 0;

                    nDec = Convert.ToInt32(sValue.Substring(i * 8, 8), 2);

                    sTemp = Convert.ToChar(nDec).ToString();
                    sb.Insert(0, sTemp);
                }

                return sb.ToString();
            }
            catch (Exception ex)
            {
                LogHelper.Instance.ErrorLog.DebugFormat(ex.Message);
                return "";
            }
        }

        public static string BinToHex(string sValue)
        {
            try
            {
                string sTemp = "";
                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < sValue.Length / 8; i++)
                {
                    int nDec = 0;


                    nDec = Convert.ToInt32(sValue.Substring(i * 8, 8), 2);

                    sTemp = Convert.ToString(nDec, 16);
                    sb.Append(Strings.UCase(sTemp.PadLeft(2, '0')));
                }

                return sb.ToString();
            }
            catch (Exception ex)
            {
                LogHelper.Instance.ErrorLog.DebugFormat(ex.Message);
                return "";
            }

        }

        public static int GetAddressDec(string sAddress)
        {
            string sHex = sAddress.Substring(1);
            return (int)HexToDec(sHex);
        }

    }
}
