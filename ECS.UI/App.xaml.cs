
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Runtime.InteropServices;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using INNO6.Core;
using System.Diagnostics;
using IWshRuntimeLibrary;

namespace ECS.UI
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : System.Windows.Application
    {
        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        /// 
        [STAThread]
        private void Application_StartUp(object sender, StartupEventArgs e)
        {
            try
            {
                bool bNewProgram;

                #region Mutex

                // 실행 프로그램 Path 
                string pName = ProcessChecker.CheckProcess();//"EQ.UI";

                // Mutex
                Mutex mutex = new Mutex(true, pName, out bNewProgram);

                // Program 중복 실행 방지
                if (bNewProgram)
                {
                    (new MainWindow()).ShowDialog();

                    try
                    {
                        // Mutex Release
                        mutex.ReleaseMutex();
                        mutex.Dispose();
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Instance.ErrorLog.DebugFormat("Application Exit Error");
                        MessageBox.Show(ex.ToString(), "Application Exit Error");
                    }
                }
                else
                {
                    Environment.Exit(0);
                    // 소유권이 부여되지 않음
                    MessageBox.Show(string.Format("이미 실행 중입니다.", pName), "중복실행");

                }
                #endregion

            }
            catch (Exception ex)
            {
                LogHelper.Instance.ErrorLog.DebugFormat(ex.Message);
                MessageBox.Show(ex.Message);
            }
        }

        private static void StartUpKeepAlive(string arg)
        {
            Process proc = Process.Start(arg);
        }

        private static void MakeShortcut()
        {
            WshShellClass WshShell;

            try
            {
                WshShell = new WshShellClass();

                string CurrentDirectory = WshShell.CurrentDirectory; //System.IO.Directory.GetParent(Application.Current.ExecutablePath).ToString();
                string ParentPath = System.IO.Directory.GetParent(CurrentDirectory).ToString();
                string ExecuterName = ConfigurationManager.AppSettings["RunProcessFilePath"];
                string TargetPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);

                IWshRuntimeLibrary.IWshShortcut MyShortcut;

                //기존 바로가기 Delete
                if (System.IO.File.Exists(TargetPath + @"\OTSAgent.lnk"))
                {
                    System.IO.File.Delete(TargetPath + @"\OTSAgent.lnk");
                }
                MyShortcut = (IWshRuntimeLibrary.IWshShortcut)WshShell.CreateShortcut(TargetPath + @"\OTSAgent.lnk");

                if (System.IO.File.Exists(TargetPath + @"\OTS.lnk"))
                {
                    System.IO.File.Delete(TargetPath + @"\OTS.lnk");
                }

                //바로가기에 프로그램의 경로를 지정한다
                MyShortcut.TargetPath = CurrentDirectory + @"\" + ExecuterName;
                //바로가기의 시작 위치를 지정한다.
                MyShortcut.WorkingDirectory = CurrentDirectory;
                //바로가기의 description을 지정한다.
                MyShortcut.Description = "OTSAgent";
                //바로가기 ICON 지정한다.
                //MyShortcut.IconLocation = CurrentDirectory + @"\UFO.ico";
                MyShortcut.Save();
            }
            catch (Exception ex)
            {
                LogHelper.Instance.ErrorLog.DebugFormat(ex.Message);
                MessageBox.Show(ex.Message);
            }
        }
    }
}
