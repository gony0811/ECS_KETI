/*************************************
 * 
 * writer       : 
 * date         : 
 * description  : 메세지박스 소스입니다.
 * 
 * ***********************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECS.UI
{
    public class MessageBoxManager
    {
        public static MSGBOX_RESULT ShowYesNoBox(String message, String Caption)
        {
            QuestionBoxWindow box = new QuestionBoxWindow(message, Caption);
            box.ShowDialog();

            return box.m_result;
        }
        public static void ShowMessageBox(String message)
        {
            MessageBoxWindow box = new MessageBoxWindow(message);
            box.ShowDialog();
        }
        public static void ShowMessageBox(String message, bool isOk)
        {
            MessageBoxWindow box = new MessageBoxWindow(message, isOk);
            box.ShowDialog();
        }
        public static void ShowMessageBox(String message, int viewTimeSec)
        {
            MessageBoxWindow box = new MessageBoxWindow(message, viewTimeSec);
            box.ShowDialog();
        }

        public static void ShowAlarmMessageBox(String alarmId, String alarmName, String alarmText, String alarmLevel)
        {
            AlarmMessageWindow box = new AlarmMessageWindow(alarmId, alarmName, alarmText, alarmLevel);
            box.ShowDialog();
        }       

        public static PROCESS_RESULT ShowProgressWindow(String progressTitle, String message, String executeName)
        {
            ProgressWindow window = new ProgressWindow(progressTitle, message, executeName);
            window.ShowDialog();


            return window.Result;
        }

        public static PROCESS_RESULT ShowProgressRingWindow(string windowTitle, string executeName)
        {
            ProgressRingWindow window = new ProgressRingWindow(windowTitle, executeName);
            window.ShowDialog();

            return window.Result;
        }

        public static MSGBOX_RESULT ShowNewRecipeWindow(string caption, out string newRecipeName)
        {
            newRecipeName = "NoName";
            NewRecipeWindow window = new NewRecipeWindow(caption);
            window.ShowDialog();
            newRecipeName = window.NewRecipeName;

            return window.m_result;
        }
    }
}
