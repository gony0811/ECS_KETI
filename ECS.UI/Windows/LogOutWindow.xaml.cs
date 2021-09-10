using ECS.UI.ViewModel;
using INNO6.Core;
using System;
using System.Windows;

namespace ECS.UI
{
    /// <summary>
    /// LogOutWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LogOutWindow : Window
    {
        public MainWindowViewModel parent = null;
        public LogOutWindow(MainWindowViewModel viewModel)
        {
            InitializeComponent();
            parent = viewModel;
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //LogHelper.UILog.InfoFormat("{0} -> btnOK_Click", this.GetType().Name);
                parent.UserInfoUpdate("INNO6", "", "", "");
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                LogHelper.Instance.ErrorLog.DebugFormat("{0} -> {1}", this.GetType().Name, ex.Message);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //LogHelper.UILog.InfoFormat("{0} -> btnCancel_Click", this.GetType().Name);
                DialogResult = false;
                Close();
            }
            catch (Exception ex)
            {
                LogHelper.Instance.ErrorLog.DebugFormat("{0} -> {1}", this.GetType().Name, ex.Message);
            }
        }
    }
}
