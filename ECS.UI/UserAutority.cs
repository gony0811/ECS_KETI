using ECS.UI.ViewModel;

namespace ECS.UI
{
    public static class UserAutority
    {
        public static bool userAuthority = false;

        public static bool Check(MainWindowViewModel viewModel)
        {
            if (!userAuthority)
            {
                LoginWindow dlg = new LoginWindow(viewModel);
                bool? result = dlg.ShowDialog();
                if (result == true)
                {
                    userAuthority = true;
                }
            }
            else
            {
                LogOutWindow dlg = new LogOutWindow(viewModel);
                bool? result = dlg.ShowDialog();
                if (result == true)
                {
                    userAuthority = false;
                }
            }
            return userAuthority;
        }
    }
}
