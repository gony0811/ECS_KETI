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
    /// NewRecipeWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class NewRecipeWindow : Window
    {
        public MSGBOX_RESULT m_result;
        public string NewRecipeName;

        public NewRecipeWindow(string caption)
        {
            
            InitializeComponent();
            gbHeader.Header = caption;
            m_result = MSGBOX_RESULT.CANCEL;
            NewRecipeName = tbNewRecipeName.Text;
        }

        private void btnYes_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LogHelper.Instance.UILog.DebugFormat("[{0}] btnYes_Click.", this.GetType().Name);
                m_result = MSGBOX_RESULT.OK;
                NewRecipeName = tbNewRecipeName.Text;
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
                NewRecipeName = tbNewRecipeName.Text;
                Close();
            }
            catch (Exception ex)
            {
                LogHelper.Instance.ErrorLog.DebugFormat("{0} -> {1}", this.GetType().Name, ex.Message);
            }
        }
    }
}
