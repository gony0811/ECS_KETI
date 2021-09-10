using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Pcomm32Functions
{
    static class Constants
    {
        public const uint COMM_EOT = 0x80000000;// An acknowledge character(ACK ASCII 9) was received indicating end of transmission from PMAC to Host PC.
        public const uint COMM_TIMEOUT = 0xC0000000;//A timeout occurred.The time for the PC to wait for PMAC to respond had been exceeded.
        public const uint COMM_BADCKSUM = 0xD0000000;//Used when using Checksum communication.If a bad checksum occurred, this error will be returned.
        public const uint COMM_ERROR = 0xE0000000;// Unable to communicate.
        public const uint COMM_FAIL = 0xF0000000;//Serious failure.
        public const uint COMM_ANYERR = 0x70000000;//Some error occurred.
        public const uint COMM_UNSOLICITED = 0x10000000;//An unsolicited response has been received from PMAC. Usually caused by PLC’s or Motion Programs that have “Send” or “Command” statements.


        public const string STR_COMM_EOT = "An acknowledge character(ACK ASCII 9) was received indicating end of transmission from PMAC to Host PC.\n";
        public const string STR_COMM_TIMEOUT = "A timeout occurred.The time for the PC to wait for PMAC to respond had been exceeded.\n";
        public const string STR_COMM_BADCKSUM = "Used when using Checksum communication.If a bad checksum occurred, this error will be returned.\n";
        public const string STR_COMM_ERROR = "Unable to communicate.\n";
        public const string STR_COMM_FAIL = "Serious failure.\n";
        public const string STR_COMM_ANYERR = "Some error occurred.\n";
        public const string STR_COMM_UNSOLICITED = "An unsolicited response has been received from PMAC. Usually caused by PLC’s or Motion Programs that have “Send” or “Command” statements.\n";


        //#define COMM_CHARS(c) (c & 0x0FFFFFFF) // Returns the number of characters
        //#define COMM_STATUS(c) (c & 0xF0000000) // Returns the status byte To check for individual error codes the MACROs below are very useful:
        //#define IS_COMM_MORE(c) ((c & COMM_FAIL) == 0)
        //#define IS_COMM_EOT(c) ((c & COMM_FAIL) == COMM_EOT)
        //#define IS_COMM_TIMEOUT(c) ((c & COMM_FAIL) == COMM_TIMEOUT)
        //#define IS_COMM_BADCKSUM(c) ((c & COMM_FAIL) == COMM_BADCKSUM)
        //#define IS_COMM_ERROR(c) ((c & COMM_FAIL) == COMM_ERROR)
        //#define IS_COMM_FAIL(c) ((c & COMM_FAIL) == COMM_FAIL)
        //#define IS_COMM_ANYERROR(c) ((c & COMM_ANYERR) > 0)
        //#define IS_COMM_UNSOLICITED(c) ((c & 0xF0000000) == COMM_UNSOLICITED)



    }

    public class PCOMM32
    {
        // Returns the number of characters
        public static long getCOMM_CHARS(long c) { return (c & 0x0FFFFFFF); }

        // Returns the status byte
        public static long COMM_STATUS(long c) { return (c & 0xF0000000); }

        //To check for individual error codes the MACROs below are very useful:
        public static bool IS_COMM_MORE(long c) { return ((c & Constants.COMM_FAIL) == 0); }
        public static bool IS_COMM_EOT(long c) { return ((c & Constants.COMM_FAIL) == Constants.COMM_EOT); }
        public static bool IS_COMM_TIMEOUT(long c) { return ((c & Constants.COMM_FAIL) == Constants.COMM_TIMEOUT); }
        public static bool IS_COMM_BADCKSUM(long c) { return ((c & Constants.COMM_FAIL) == Constants.COMM_BADCKSUM); }
        public static bool IS_COMM_ERROR(long c) { return ((c & Constants.COMM_FAIL) == Constants.COMM_ERROR); }
        public static bool IS_COMM_FAIL(long c) { return ((c & Constants.COMM_FAIL) == Constants.COMM_FAIL); }
        public static bool IS_COMM_ANYERROR(long c) { return ((c & Constants.COMM_ANYERR) > 0); }
        public static bool IS_COMM_UNSOLICITED(long c) { return ((c & 0xF0000000) == Constants.COMM_UNSOLICITED); }



        //PCOMM32 PRO.pdf - page 95
        /*
        typedef long (FAR WINAPI *DOWNLOADGETPROC)(long nIndex,LPTSTR lpszBuffer,long nMaxLength);
            This function type is used by some program downloading procedures to offer the option of extracting text
            lines for the download from another source other than a disk file.

            Arguments
                nIndex Line number asked for.
                lpszBuffer Buffer pointer to copy text line into.
                nMaxLength Maximum length of buffer.
            Return Value
                The number of characters copied into the buffer.
        */
        public delegate int DOWNLOADGETPROC(Int32 nIndex, String lpszBuffer, Int32 nMaxLength);

        /*
        typedef void (FAR WINAPI *DOWNLOADPROGRESS)(long nPercent);
           This function type is used by some program downloading procedures to offer the option of setting the
           current percent of progress during the procedure.

           Arguments
               nPercent Current percent of progress.
           Return Value : None
        */
        public delegate void DOWNLOADPROGRESS(Int32 nPercent);

        /*
        typedef void (FAR WINAPI *DOWNLOADMSGPROC)(LPTSTR str,BOOL newline);
           This function type is used by some program downloading procedures to offer the option of reporting a
           status message.

           Arguments
               str Message string.
               newline Indicates if a new line should be added by the called procedure.
           Return Value : None
        */
        public delegate void DOWNLOADMSGPROC(String str, Boolean newline);

        //DPRTESTMSGPROC 
        //typedef void (FAR WINAPI *DPRTESTMSGPROC)(LONG NumErrors, LPTSTR Action,LONG CurrentOffset);
        public delegate void DPRTESTMSGPROC(Int32 nNumErrors, String lpszAction, Int32 nCurrentOffset);

        //typedef void (FAR WINAPI *DPRTESTPROGRESS)(LONG Percent);
        public delegate void DPRTESTPROGRESS(Int32 nPercent);

        //public delegate void DOWNLOADERRORPROC(String fname, Int32 err, Int32 line, String szLine);


        [DllImport("Pcomm32.dll")]
        public static extern Int32 OpenPmacDevice(UInt32 dwDevice);
        [DllImport("Pcomm32.dll")]
        public static extern UInt32 PmacSelect(UInt32 dwDevice);
        [DllImport("Pcomm32.dll")]
        public static extern UInt32 ClosePmacDevice(UInt32 dwDevice);
        [DllImport("Pcomm32.dll")]
        //public static extern Int32 PmacGetResponseA(UInt32 dwDevice, StringBuilder s, UInt32 maxchar, StringBuilder outstr);
        public static extern Int32 PmacGetResponseA(UInt32 dwDevice, Byte[] s, UInt32 maxchar, Byte[] outstr);
        [DllImport("Pcomm32.dll")]
        public static extern Int32 PmacGetResponseA(UInt32 dwDevice, StringBuilder s, UInt32 maxchar, StringBuilder outstr);

        [DllImport("Pcomm32.dll")]
        public static extern long PmacGetResponseExA(uint dwDevice, IntPtr response, uint maxchar, IntPtr command);
        // [DllImport("Pcomm32.dll")]
        //public static extern long PmacGetResponseExA(uint dwDevice, ref IntPtr response, uint maxchar, StringBuilder command);

        [DllImport("Pcomm32.dll")]
        public static extern long PmacGetResponseExA(uint dwDevice, IntPtr response, uint maxchar, StringBuilder command);
        /*
   long PmacGetResponseExA(DWORD dwDevice,PCHAR response,UINT maxchar,PCHAR command);

            dwDevice Device number
            response Pointer to string buffer to copy the PMAC’s response into.
            maxchar Maximum characters to copy.
            command Pointer to NULL terminated string to be sent to the PMAC as a question/command.

            Return Value : If successful Not False(0)
         */
        [DllImport("Pcomm32.dll")]
        public static extern Int32 PmacDownloadA(UInt32 dwDevice, DOWNLOADMSGPROC msgp, DOWNLOADGETPROC getp, DOWNLOADPROGRESS pprg, Byte[] filename, Int32 macro, Int32 map, Int32 log, Int32 dnld);
        [DllImport("Pcomm32.dll")]
        public static extern Int32 PmacDownloadA(UInt32 dwDevice, DOWNLOADMSGPROC msgp, DOWNLOADGETPROC getp, DOWNLOADPROGRESS pprg,
            string filename, Int32 macro, Int32 map, Int32 log, Int32 dnld);


        /*
          PVOID PmacDPRGetMem(DWORD dwDevice, DWORD offset, size_t count, PVOID val);
Copies a block of dual-ported RAM memory.
Arguments
dwDevice : Device number
offset : Offset from the start of dual-ported RAM.
count : Size of memory block to copy.
val  : Pointer to destination.

            Return - Pointer to destination.
         */
        [DllImport("Pcomm32.dll")]
        public static extern IntPtr PmacDPRGetMem(UInt32 dwDevice, UInt32 offset, UInt32 count, IntPtr val);


        /*
        PmacDPRSetMem()
PVOID PmacDPRSetMem(DWORD dwDevice, DWORD offset, size_t count, PVOID val);
Copies a block of memory into dual-ported RAM.

Arguments
  dwDevice Device number.
  offset Offset from the start of dual-ported RAM.
  count Size of data to transfer.
  val Pointer to memory to transfer.
Return Values - Pointer to transferred data.
        */
        [DllImport("Pcomm32.dll")]
        public static extern IntPtr PmacDPRSetMem(UInt32 dwDevice, UInt32 offset, UInt32 count, IntPtr val);

        [DllImport("Pcomm32.dll")]
        //int CALLBACK PmacGetErrorStrA(DWORD dwDevice, PCHAR str, int maxchar);
        public static extern Int32 PmacGetErrorStrA(UInt32 dwDevice, IntPtr str, UInt32 maxchar);
        [DllImport("Pcomm32.dll")]
        public static extern Int32 PmacGetErrorStrA(UInt32 dwDevice, StringBuilder str, UInt32 maxchar);


    }

}
