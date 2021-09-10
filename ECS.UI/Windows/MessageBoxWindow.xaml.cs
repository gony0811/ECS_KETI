using INNO6.Core;
using System;
using System.Windows;
using System.Windows.Threading;

namespace ECS.UI
{
    /// <summary>
    /// MessageBoxWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MessageBoxWindow : Window
    {
        private DispatcherTimer tmrClose;
        public bool bClose = false;
        public int viewTime = 5;
        public string RecipeMessage = string.Empty;

        public MessageBoxWindow()
        {
            InitializeComponent();
        }

        public MessageBoxWindow(String message)
        {
            LogHelper.Instance.UILog.DebugFormat("[{0}] {1}", this.GetType().Name, message);
            InitializeComponent();
            tbMessageBox.Text = message;
        }

        public MessageBoxWindow(String message, bool isOK)
        {
            LogHelper.Instance.UILog.DebugFormat("[{0}] {1}", this.GetType().Name, message);
            InitializeComponent();
            bClose = isOK;
            tbMessageBox.Text = message;

        }
        
        public MessageBoxWindow(String message, int ViewTimeSec)
        {
            LogHelper.Instance.UILog.DebugFormat("[{0}] {1}", this.GetType().Name, message);
            InitializeComponent();
            viewTime = ViewTimeSec;
            RecipeMessage = message;
            tbMessageBox.Text = string.Format("{0}...{1}", RecipeMessage, viewTime--);

            tmrClose = new DispatcherTimer();
            tmrClose.Interval = TimeSpan.FromMilliseconds(1000);
            tmrClose.Tick += new EventHandler(TmrClose_Tick);
            tmrClose.Start();
        }

        private void TmrClose_Tick(object sender, EventArgs e)
        {
            try
            {
                tbMessageBox.Text = string.Format("{0}...{1}", RecipeMessage, viewTime--);
                if (viewTime < 0)
                {
                    tmrClose.Stop();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.ErrorLog.DebugFormat("{0} -> {1}", this.GetType().Name, ex.Message);
            }
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LogHelper.Instance.UILog.DebugFormat("[{0}] Message Box OK button Click.", this.GetType().Name);
                Close();
            }
            catch (Exception ex)
            {
                LogHelper.Instance.ErrorLog.DebugFormat("{0} -> {1}", this.GetType().Name, ex.Message);
            }
        }
    }
}
