using ECS.UI.ViewModel;
using INNO6.Core;
using INNO6.Core.Manager;
using INNO6.Core.Manager.Model;
using System;
using System.Windows;
using System.Windows.Media;

namespace ECS.UI
{
    /// <summary>
    /// LoginWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LoginWindow : Window
    {
        Common baseinfo = Common.GetInstance();

        public MainWindowViewModel parent = null;

        public LoginWindow(MainWindowViewModel viewModel)
        {
            InitializeComponent();
            parent = viewModel;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LogHelper.Instance.UILog.DebugFormat("[{0}] btnCancel_Click.", this.GetType().Name);
                DialogResult = false;
                Close();
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
                if (!string.IsNullOrEmpty(tbLogInID.Text))
                {
                    LogHelper.Instance.UILog.DebugFormat("[{0}] btnOK_Click. (ID : {1})", this.GetType().Name, tbLogInID.Text.ToUpper());

                    string password = string.Empty;

                    if (string.IsNullOrEmpty(pwbLogInPW.Password)) password = string.Empty;
                    else password = pwbLogInPW.Password;

                    if (UserAuthorityManager.Instance.Authentication(tbLogInID.Text.ToUpper(), password, out eUserLevel userLevel))
                    {
                        switch (userLevel)
                        {
                            case eUserLevel.ADMIN:
                                {
                                    baseinfo.UserAccount = tbLogInID.Text.ToUpper();
                                    baseinfo.UserAuthority = userLevel;
                                    parent.UserInfoUpdate(tbLogInID.Text.ToUpper(), tbLogInID.Text.ToUpper(), "", eUserLevel.ADMIN.ToString());
                                    DialogResult = true;
                                    this.Close();
                                }
                                break;
                            case eUserLevel.ENGINEER:
                                {
                                    baseinfo.UserAccount = tbLogInID.Text.ToUpper();
                                    baseinfo.UserAuthority = userLevel;
                                    parent.UserInfoUpdate(tbLogInID.Text.ToUpper(), tbLogInID.Text.ToUpper(), "", eUserLevel.ENGINEER.ToString());
                                    DialogResult = true;
                                    this.Close();
                                }
                                break;
                            case eUserLevel.OPERATOR:
                                {
                                    baseinfo.UserAccount = tbLogInID.Text.ToUpper();
                                    baseinfo.UserAuthority = userLevel;
                                    parent.UserInfoUpdate(tbLogInID.Text.ToUpper(), tbLogInID.Text.ToUpper(), "", eUserLevel.OPERATOR.ToString());
                                    DialogResult = true;
                                    this.Close();
                                }
                                break;
                            case eUserLevel.GUEST:
                                {
                                    baseinfo.UserAccount = tbLogInID.Text.ToUpper();
                                    baseinfo.UserAuthority = userLevel;
                                    parent.UserInfoUpdate(tbLogInID.Text.ToUpper(), tbLogInID.Text.ToUpper(), "", eUserLevel.GUEST.ToString());
                                    DialogResult = true;
                                    this.Close();
                                }
                                break;
                            default:
                                {
                                    MessageBoxManager.ShowMessageBox("Not Correct ID/PASSWARD!");
                                }
                                break;
                        }


                    }

                }
                else
                {
                    tbLogInID.Background = Brushes.Red;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.ErrorLog.DebugFormat("{0} -> {1}", this.GetType().Name, ex.Message);
            }
        }
    }
}
