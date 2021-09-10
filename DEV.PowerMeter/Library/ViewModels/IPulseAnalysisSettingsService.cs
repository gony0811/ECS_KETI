using SharedLibrary;
using System;
using System.Windows.Threading;

namespace DEV.PowerMeter.Library.ViewModels
{
    public interface IPulseAnalysisSettingsService : IPersistentWindow, ILocationAndSize
    {
        void LoadSettings();

        event Action<PulseAnalysisOptions, bool> OptionsChanged;

        Dispatcher Dispatcher { get; }
    }
}
