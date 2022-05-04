/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:ECS.UI"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

namespace ECS.UI.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        public static ViewModelLocator Instance = new ViewModelLocator();

        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<MainWindowViewModel>();
            SimpleIoc.Default.Register<VisionCameraViewModel>();
            SimpleIoc.Default.Register<MainSystemViewModel>();
            SimpleIoc.Default.Register<IoListViewModel>();
            SimpleIoc.Default.Register<CurrentAlarmViewModel>();
            SimpleIoc.Default.Register<MotionControlViewModel>();
            SimpleIoc.Default.Register<SettingParameterViewModel>();
            SimpleIoc.Default.Register<LaserControlViewModel>();
            //SimpleIoc.Default.Register<ECParameterViewModel>();
            SimpleIoc.Default.Register<RecipeManagerViewModel>();
            SimpleIoc.Default.Register<AlarmHistoryViewModel>();
            SimpleIoc.Default.Register<OperationAutoViewModel>();
            SimpleIoc.Default.Register<StandbyScreenViewModel>();
            //SimpleIoc.Default.Register<PLCMapViewModel>();
            //SimpleIoc.Default.Register<MaterialViewModel>();
            //SimpleIoc.Default.Register<TrackingLogViewModel>();
        }

        public MainWindowViewModel MainWindowViewModel
        {
            get { return ServiceLocator.Current.GetInstance<MainWindowViewModel>(); }
        }

        public MainSystemViewModel MainSystemViewModel
        {
            get { return ServiceLocator.Current.GetInstance<MainSystemViewModel>(); }
        }

        public VisionCameraViewModel VisionCameraViewModel
        {
            get { return ServiceLocator.Current.GetInstance<VisionCameraViewModel>(); }
        }

        public IoListViewModel IoListViewModel
        {
            get { return ServiceLocator.Current.GetInstance<IoListViewModel>(); }
        }

        public CurrentAlarmViewModel CurrentAlarmViewModel
        {
            get { return ServiceLocator.Current.GetInstance<CurrentAlarmViewModel>(); }
        }

        public MotionControlViewModel MotionControlViewModel
        {
            get { return ServiceLocator.Current.GetInstance<MotionControlViewModel>(); }
        }

        public SettingParameterViewModel SettingParameterViewModel
        {
            get { return ServiceLocator.Current.GetInstance<SettingParameterViewModel>(); }
        }

        public LaserControlViewModel LaserControlViewModel
        {
            get { return ServiceLocator.Current.GetInstance<LaserControlViewModel>(); }
        }

        public RecipeManagerViewModel RecipeManagerViewModel
        {
            get { return ServiceLocator.Current.GetInstance<RecipeManagerViewModel>(); }
        }

        public OperationAutoViewModel OperationAutoViewModel
        {
            get { return ServiceLocator.Current.GetInstance<OperationAutoViewModel>(); }
        }

        public StandbyScreenViewModel StandbyScreenViewModel
        {
            get { return ServiceLocator.Current.GetInstance<StandbyScreenViewModel>(); }
        }

        //public CommonViewModel CommonViewModel
        //{
        //    get { return ServiceLocator.Current.GetInstance<CommonViewModel>(); }
        //}

        //public MainViewModel MainViewModel
        //{
        //    get { return ServiceLocator.Current.GetInstance<MainViewModel>(); }
        //}

        //public SVParameterViewModel SVParameterViewModel
        //{
        //    get { return ServiceLocator.Current.GetInstance<SVParameterViewModel>(); }
        //}

        //public ECParameterViewModel ECParameterViewModel
        //{
        //    get { return ServiceLocator.Current.GetInstance<ECParameterViewModel>(); }
        //}

        //public RecipesViewModel RecipesViewModel
        //{
        //    get { return ServiceLocator.Current.GetInstance<RecipesViewModel>(); }
        //}

        public AlarmHistoryViewModel AlarmHistoryViewModel
        {
            get { return ServiceLocator.Current.GetInstance<AlarmHistoryViewModel>(); }
        }

        //public PLCMapViewModel PLCMapViewModel
        //{
        //    get { return ServiceLocator.Current.GetInstance<PLCMapViewModel>(); }
        //}

        //public MaterialViewModel MaterialViewModel
        //{
        //    get { return ServiceLocator.Current.GetInstance<MaterialViewModel>(); }
        //}

        //public TrackingLogViewModel TrackingLogViewModel
        //{
        //    get { return ServiceLocator.Current.GetInstance<TrackingLogViewModel>(); }
        //}

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}