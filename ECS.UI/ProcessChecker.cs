
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;

namespace ECS.UI
{

    static class ProcessChecker
    {
        /// <summary>
        /// Mutex ProcessName 확인
        /// </summary>
        /// <returns>ProcessName</returns>
        //static System.Configuration.AppSettingsReader appReader;
        static public string CheckProcess()
        {
            string m_ProgramName = string.Empty;

            //appReader = new System.Configuration.AppSettingsReader();
            //object serviceName = appReader.GetValue("Service", typeof(string)); //App.Config에서 Service 명을 읽는다.

            //m_ProgramName = (string)serviceName; //설비별 UI 처리를 위해 추가.
            m_ProgramName = "ECS.UI";

            //
            string path = Process.GetCurrentProcess().MainModule.FileName; //현재 프로그램 경로 저장
            string[] sArray = path.Split('\\');

            m_ProgramName = sArray[sArray.Length - 2];
            //
            // 실행 프로그램의 경로 저장 ( 폴더별 실행 가능 하도록 )
            //string m_ProgramName = string.Empty;
            // 실행 프로그램 절대 경로 저장
            //string[] m_NamePath = Application.StartupPath.ToString().Split('\\');

            //// 실행 프로그램의 경로 찾기
            //for (int i = 0; i < m_NamePath.Length; i++)
            //{
            //    if (m_NamePath[i] == "debug")
            //    {
            // Ex) C:\CIMSERVER\Stacking01\bin 이면 "Stacking01"를 찾음
            //        m_ProgramName = m_NamePath[i - 1];
            //    }
            //}

            return m_ProgramName;
        }

        /// <summary>
        /// 찾아야 할 캡션
        /// </summary>
        static string _requiredString;

        /// <summary>
        /// Contains signatures for C++ DLLs using interop.
        /// </summary>
        internal static class NativeMethods
        {
            /// <summary>
            /// 현재 실행중인 윈도우의 상태를 보여준다.
            /// </summary>
            /// <param name="hWnd"></param>
            /// <param name="nCmdShow"></param>
            /// <returns></returns>
            [DllImport("user32.dll", SetLastError = true)]
            public static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

            /// <summary>
            /// 선택한 윈도우를 뒤에 숨어있었으면 앞으로, 최소화상태였으면 원래상태로 되돌려놓으며 활성화시킨다.
            /// </summary>
            /// <param name="hWnd"></param>
            /// <returns></returns>
            [DllImport("user32.dll")]
            public static extern bool SetForegroundWindow(IntPtr hWnd);

            /// <summary>
            /// EnumWindows 함수는 모든 최상위 윈도우를 검색해서 그 핸들을 콜백함수로 전달하되
            /// 모든 윈도우를 다 찾거나 콜백함수가 FALSE를 리턴할 때까지 검색을 계속한다.
            /// 콜백함수는 검색된 윈도우의 핸들을 전달받으므로 모든 윈도우에 대해 모든 작업을 다 할 수 있다.
            /// EnumWindows 함수는 차일드 윈도우는 검색에서 제외된다.
            /// 단 시스템이 생성한 일부 최상위 윈도우는 WS_CHILD 스타일을 가지고 있더라도 예외적으로 검색에 포함된다.
            /// </summary>
            /// <param name="lpEnumFunc">EnumWindows의 실행 결과를 받아줄 콜백함수이다. EnumWindows는 이 함수 결과가 false가 될 때까지 계속 윈도우를 검색하게 된다.</param>
            /// <param name="lParam"></param>
            /// <returns></returns>
            [DllImport("user32.dll")]
            public static extern bool EnumWindows(EnumWindowsProcDel lpEnumFunc, Int32 lParam);

            /// <summary>
            /// HWND 값을 이용하여 프로세스 ID를 알려주는 함수이다.
            /// </summary>
            /// <param name="hWnd"></param>
            /// <param name="lpdwProcessId"></param>
            /// <returns></returns>
            [DllImport("user32.dll")]
            public static extern int GetWindowThreadProcessId(IntPtr hWnd, ref Int32 lpdwProcessId);

            /// <summary>
            /// 윈도우의 캡션을 가져온다.
            /// </summary>
            /// <param name="hWnd"></param>
            /// <param name="lpString"></param>
            /// <param name="nMaxCount"></param>
            /// <returns></returns>
            [DllImport("user32.dll")]
            public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, Int32 nMaxCount);

            //윈도우의 상태를 normal로 하게 하는 상수
            public const int SW_SHOWNORMAL = 1;
        }

        /// <summary>
        /// EnumWindows의 실행 결과를 받아줄 콜백함수이다.
        /// EnumWindows는 이 함수 결과가 false가 될 때까지 계속 윈도우를 검색하게 된다.
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        public delegate bool EnumWindowsProcDel(IntPtr hWnd, Int32 lParam);

        /// <summary>
        /// Perform finding and showing of running window.
        /// 모든 실행중인 윈도우를 검색하며 찾고자 하는 캡션의 윈도우를 발견하면 활성화시킨다.
        /// </summary>
        /// <returns>Bool, which is important and must be kept to match up
        /// with system call.</returns>
        static private bool EnumWindowsProc(IntPtr hWnd, Int32 lParam)
        {
            int processId = 0;
            NativeMethods.GetWindowThreadProcessId(hWnd, ref processId);

            StringBuilder caption = new StringBuilder(1024);
            NativeMethods.GetWindowText(hWnd, caption, 1024); //방금 검색한 윈도우의 캡션을 가져온다.

            //찾을 윈도우명과 가져온 캡션이 일치한다면,
            if (processId == lParam && (caption.ToString().IndexOf(_requiredString, StringComparison.OrdinalIgnoreCase) != -1))
            {
                //윈도우를 normal 상태로 바꾸고 제일 앞으로 가져온다.
                NativeMethods.ShowWindowAsync(hWnd, NativeMethods.SW_SHOWNORMAL);
                NativeMethods.SetForegroundWindow(hWnd);
            }
            return true;
        }

        /// <summary>
        /// 지금 실행하려는 프로그램이 이미 실행중인지 아닌지 찾아보고 결과를 알려준다.
        /// </summary>
        /// <param name="forceTitle">찾으려는 윈도우의 캡션, 즉 프로그램 타이틀</param>
        /// <returns>해당 캡션의 윈도우가 이미 실행중이라면 False,
        /// 처음 실행하는 것이라면 True를 반환한다.</returns>
        static public bool IsOnlyProcess(string forceTitle)
        {
            _requiredString = forceTitle;
            //먼저 실행파일의 이름으로 이름이 같은 프로세스를 검색해본다.
            foreach (Process proc in Process.GetProcessesByName(System.Windows.Application.ResourceAssembly.GetName().Name))
            {
                if (proc.MainWindowTitle == _requiredString)
                {
                    NativeMethods.EnumWindows(new EnumWindowsProcDel(EnumWindowsProc), proc.Id);
                    System.Windows.Forms.MessageBox.Show("!! 프로그램이 이미 실행 중입니다. !!", "Warning", System.Windows.Forms.MessageBoxButtons.OK);
                    return false;
                }
            }
            return true;
        }

        //static public string GetService(string name)
        //{
        //    object serviceName = appReader.GetValue(name, typeof(string)); //App.Config에서 Service 명을 읽는다.
        //    return (string)serviceName;
        //}
    }
}