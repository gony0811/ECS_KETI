
using SharedLibrary;


namespace DEV.PowerMeter.Library.ViewModels
{
    public class ServiceLocator
    {
        static ServiceLocator()
        {
            ServiceLocator.Register<IErrorReporter, ErrorLogger>();
            ServiceLocator.Register<MainViewModel, MainViewModel>();
            ServiceLocator.Register<PhoenixMeter, PhoenixMeter>();
            ServiceLocator.Register<PortManager, PortManager>();
        }

        public static MainViewModel MainViewModel => ServiceLocator.Resolve<MainViewModel>();

        public static PhoenixMeter PhoenixMeter => ServiceLocator.Resolve<PhoenixMeter>();

        public static PulseAnalysisOptions PulseAnalysisOptions => ServiceLocator.PhoenixMeter.PulseAnalysisOptions;

        public static void Register<TInterface, TImplementation>() => MyToolkit.Composition.ServiceLocator.Default.RegisterSingleton<TInterface, TImplementation>();

        public static void Register<TInterface, TImplementation>(TImplementation service) => MyToolkit.Composition.ServiceLocator.Default.RegisterSingleton<TInterface, TImplementation>(service);

        public static TInterface Resolve<TInterface>() => MyToolkit.Composition.ServiceLocator.Default.Resolve<TInterface>();
    }
}