using INNO6.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ECS.UI
{
    /// <summary>
    /// MessageAlarmWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class AlarmMessageWindow : Window
    {
        public AlarmMessageWindow()
        {
            InitializeComponent();
        }

        public AlarmMessageWindow(String alarmId, String alarmName, String alarmText, String alarmLevel)
        {
            LogHelper.Instance.UILog.DebugFormat("[{0}] Alarm Id={1}, Alarm Name={2}, Alarm Text={3}, Alarm Level={4}", 
                GetType(), alarmId, alarmName, alarmText, alarmLevel);
            InitializeComponent();
            this.gbHeader.Header = string.Format("[ALARM] {0}", alarmId);
            this.tbAlarmName.Text = string.Format("{0} (Level={1})", alarmName, alarmLevel);
            this.tbAlarmMessage.Text = alarmText;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LogHelper.Instance.UILog.DebugFormat("[{0}] Alarm Message Box OK button Click.", this.GetType().Name);
                Close();
            }
            catch (Exception ex)
            {
                LogHelper.Instance.ErrorLog.DebugFormat("{0} -> {1}", this.GetType().Name, ex.Message);
            }
        }
    }
}
