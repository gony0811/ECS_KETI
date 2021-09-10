// XTrace.cs  Version 1.2 - see article at CodeProject.com
//
// Author:  Hans Dietrich
//          hdietrich@gmail.com
//
// Description:
//     XTrace.cs provides MFC-style Trace with printf formatting to C# apps.
//
// History
//     Version 1.2 - 2010 March 5
//     - Initial public release
//
// License:
//     This software is released under the Code Project Open License (CPOL),
//     which may be found here:  http://www.codeproject.com/info/eula.aspx
//     You are free to use this software in any way you like, except that you 
//     may not sell this source code.
//
//     This software is provided "as is" with no expressed or implied warranty.
//     I accept no liability for any damage or loss of business that this 
//     software may cause.
//
///////////////////////////////////////////////////////////////////////////////

using System;
using System.IO;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Diagnostics;

// disable warning from using AppDomain.GetCurrentThreadId()
#pragma warning disable 618

namespace INNO6.Core
{
    public static class XTrace
    {
        private static bool XTRACE_FULL_PATH = false;	// true = display full path, even when
        // not running under the debugger
        private static bool XTRACE_THREAD_ID = true;	// true = display win32 and managed 
        // thread ids

        /// <summary>
        /// Provides functionality like MFC TRACE().
        /// </summary>
        /// <param name="format">printf-style format string</param>
        /// <param name="parameters">variable length list of args</param>
        [Conditional("DEBUG")]
        public static void Trace(string format, params object[] parameters)
        {
            string prefix = GetPrefix();

            // now get formatted parameters
            string trace = Tools.sprintf(format, parameters);

            // output using OutputDebugString
            Debug.WriteLine(prefix + trace);
        }

        /// <summary>
        /// Provides functionality like MFC TRACE(); includes method name.
        /// </summary>
        /// <param name="format">printf-style format string</param>
        /// <param name="parameters">variable length list of args</param>
        [Conditional("DEBUG")]
        public static void TraceEnter(string format, params object[] parameters)
        {
            string prefix = GetPrefix();

            string method = "in " + GetMethodName() + " ";

            // now get formatted parameters
            string trace = Tools.sprintf(format, parameters);

            // output using OutputDebugString
            Debug.WriteLine(prefix + method + trace);
        }

        /// <summary>
        /// Provides functionality like MFC TRACE(); includes method name.
        /// </summary>
        [Conditional("DEBUG")]
        public static void TraceEnter()
        {
            string prefix = GetPrefix();

            string method = "in " + GetMethodName() + " ";

            // output using OutputDebugString
            Debug.WriteLine(prefix + method);
        }

        /// <summary>
        /// Provides functionality like MFC TRACE(); outputs desc and Rectangle values.
        /// </summary>
        [Conditional("DEBUG")]
        public static void TraceRect(string desc, Rectangle rect)
        {
            string prefix = GetPrefix();

            // now get formatted rect
            string trace = String.Format("left={0}  top={1}  right={2}  bottom={3}",
                rect.Left, rect.Top, rect.Right, rect.Bottom);

            // output using OutputDebugString
            Debug.WriteLine(prefix + desc + trace);
        }

        /// <summary>
        /// Provides functionality like MFC TRACE(); outputs desc and Point values.
        /// </summary>
        [Conditional("DEBUG")]
        public static void TracePoint(string desc, Point point)
        {
            string prefix = GetPrefix();

            // now get formatted point
            string trace = String.Format("X={0}  Y={1}", point.X, point.Y);

            // output using OutputDebugString
            Debug.WriteLine(prefix + desc + trace);
        }

        /// <summary>
        /// Provides functionality like MFC TRACE(); outputs desc and Size values.
        /// </summary>
        [Conditional("DEBUG")]
        public static void TraceSize(string desc, Size size)
        {
            string prefix = GetPrefix();

            // now get formatted size
            string trace = String.Format("Width={0}  Height={1}", size.Width, size.Height);

            // output using OutputDebugString
            Debug.WriteLine(prefix + desc + trace);
        }

        //============================================================================

        private static string GetPrefix()
        {
            string prefix = "";
            try
            {
                // get previous stack frame to determine caller module & line no.
                var stacktrace = new StackTrace(true);
                StackFrame caller = stacktrace.GetFrame(2);
                string filename = caller.GetFileName();

                // we display full path always when running under debugger -
                // this allows double-clicking the line in the output window
                if (!XTRACE_FULL_PATH && !Debugger.IsAttached)
                {
                    // extract file name from path
                    int index = filename.LastIndexOf('\\');
                    if (index > 0)
                    {
                        int len = filename.Length;
                        filename = filename.Substring(index + 1, len - index - 1);
                    }
                }

                // add line number in parens
                prefix = String.Format("{0}({1}): ", filename, caller.GetFileLineNumber());

                if (XTRACE_THREAD_ID)
                {
                    // display thread id -
                    // we want to see the same thread id that the debugger displays
                    int win32_threadid = AppDomain.GetCurrentThreadId();
                    // and the .Net logical thread id
                    int managed_threadid = System.Threading.Thread.CurrentThread.ManagedThreadId;
                    string id = String.Format("[{0:X}:{1:X}]  ", win32_threadid, managed_threadid);
                    prefix = prefix + id;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.ErrorLog.DebugFormat(ex.Message);
            }

            return prefix;
        }

        private static string GetMethodName()
        {
            // get previous stack frame to determine caller module & line no.
            string name = "<unavailable>";
            try
            {
                var stacktrace = new StackTrace(true);
                int count = stacktrace.FrameCount;
                //Debug.WriteLine(String.Format("count={0}", count));
                if (count > 2)
                {
                    StackFrame caller = stacktrace.GetFrame(2);
                    name = caller.GetMethod().Name;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.ErrorLog.DebugFormat(ex.Message);
            }
            return name;
        }
    }
}
