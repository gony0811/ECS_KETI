using INNO6.Core;
using System;
using System.Windows;


namespace ECS.UI
{
    /// <summary>
    /// QuestionBoxWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class QuestionBoxWindow : Window
    {
        public MSGBOX_RESULT m_result;
        public QuestionBoxWindow()
        {
            InitializeComponent();
        }
        public QuestionBoxWindow(string message, string caption)
        {
            InitializeComponent();

            tbMessageBox.Text = message;
            gbHeader.Header = caption;
            m_result = MSGBOX_RESULT.CANCEL;
        }

        private void btnYes_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LogHelper.Instance.UILog.DebugFormat("[{0}] btnYes_Click.", this.GetType().Name);
                m_result = MSGBOX_RESULT.OK;
                Close();
            }
            catch (Exception ex)
            {
                LogHelper.Instance.ErrorLog.DebugFormat("{0} -> {1}", this.GetType().Name, ex.Message);
            }
        }

        private void btnNo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LogHelper.Instance.UILog.DebugFormat("[{0}] btnNo_Click.", this.GetType().Name);
                m_result = MSGBOX_RESULT.CANCEL;
                Close();
            }
            catch (Exception ex)
            {
                LogHelper.Instance.ErrorLog.DebugFormat("{0} -> {1}", this.GetType().Name, ex.Message);
            }
        }
    }
}
