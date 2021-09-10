using DEV.PowerMeter.Library.ImportExport;
using DEV.PowerMeter.Library.ViewModels;
using MvvmFoundation.Wpf;
using SharedLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Input;

namespace DEV.PowerMeter.Library
{
    [DataContract]
    [API(APICategory.Analysis, "The PulseAnalysisOptions class contains properties which control the operation\r\n\t\tof the pulse analyzer. Each property should have a clear relationship with\r\n\t\tthe corresponding settings in the Pulse Analysis Settings Window.")]
    public class PulseAnalysisOptions : ViewModelBase, IPulseAnalysisOptions
    {
        private bool isRunning;
        private bool _BufferIsPower;
        private bool resultsPresent;
        private bool Updating;
        private object Lock;
        private EnergyBaselineOption energyBaselineOption;
        private double energyBaselineLowerCursorLevel;
        private string energyBaselineLowerCursorLevelText;
        private double energyBaselineCustomLevel;
        private bool pulseAnalysisEnabled;
        private bool showSelectionBounds_IsChecked;
        private bool showTrackingCursor_IsChecked;
        private bool showTriggerMarks_IsChecked;
        private bool snapToTriggers_IsChecked;
        private bool showAnalysisLevelHigh_IsChecked;
        private bool showAnalysisLevelMiddle_IsChecked;
        private bool showAnalysisLevelLow_IsChecked;
        private bool showEnergyBaselineLevel_IsChecked;
        private bool showMinimumLevel_IsChecked;
        private bool showMaximumLevel_IsChecked;
        private bool showPeakPowerLevel_IsChecked;
        private bool showAveragePowerLevel_IsChecked;
        private bool showRiseTimeStart_IsChecked;
        private bool showRiseTimeStop_IsChecked;
        private bool showFallTimeStart_IsChecked;
        private bool showFallTimeStop_IsChecked;
        private bool showMiddleTimeStart_IsChecked;
        private bool showMiddleTimeStop_IsChecked;
        private bool showEnergyTimeStart_IsChecked;
        private bool showEnergyTimeStop_IsChecked;
        private bool showPeakPowerTime_IsChecked;
        protected double lowerPercent;
        protected double middlePercent;
        protected double upperPercent;
        private double minimumThreshold;
        private double maximumThreshold;
        private double lowerThreshold;
        private double middleThreshold;
        private double upperThreshold;
        public const double DefaultLowerPercent = 0.1;
        public const double DefaultMiddlePercent = 0.5;
        public const double DefaultUpperPercent = 0.9;
        public readonly double[] DefaultThresholds = new double[3]
        {
      0.1,
      0.5,
      0.9
        };
        public static readonly Library.PopularOptions StandardOptions = new Library.PopularOptions((IEnumerable<PopularOption>)new PopularOption[3]
        {
      PopularOption.CustomOption,
      new PopularOption(new LevelCombination(0.1, 0.5, 0.9)),
      new PopularOption(new LevelCombination(0.1, 0.2, 0.5))
        });
        [DataMember]
        public Library.PopularOptions PopularAdditions;
        public const Level DefaultWidthStartThreshold = Level.Lower;
        public const Level DefaultWidthStopThreshold = Level.Lower;
        private Level widthStartThreshold;
        private Level widthStopThreshold;
        private ICommand _ExportAnalysisReport;
        private ICommand _ExportPulseAnalysisSettingsCommand;
        public const string ImportExportPathnameDefault = "PulseAnalysisSettings.txt";
        private string _ImportExportPathname;
        private ICommand _ImportPulseAnalysisSettingsCommand;
        public const string SerializationVersion = "V1.5.0.7";
        public const string CsvSeparator = ", ";
        public const string TsvSeparator = "\t";
        public static readonly string Separator = ", ";

        protected IPulseAnalysisSettingsService PulseAnalysisSettingsService => ServiceLocator.Resolve<IPulseAnalysisSettingsService>();

        protected PhoenixMeter PhoenixMeter => ServiceLocator.Resolve<PhoenixMeter>();

        protected Computations Computations => this.PhoenixMeter?.Computations;

        public PulseAnalysisOptions() => this.SetDefaults();

        public void SetDefaults()
        {
            this.Lock = new object();
            this.PopularAdditions = new Library.PopularOptions();
            this.energyBaselineOption = EnergyBaselineOption.Zero;
            this.ShowAnalysisLevelHigh_IsChecked = this.ShowAnalysisLevelLow_IsChecked = this.ShowRiseTimeStart_IsChecked = this.ShowFallTimeStop_IsChecked = this.ShowPeakPowerTime_IsChecked = true;
            this.PopularAdditions.Clear();
            this.LowerPercent = 0.1;
            this.MiddlePercent = 0.5;
            this.UpperPercent = 0.9;
            this.InitializeWidthThresholds();
        }

        public void AttachMainViewModel(MainViewModel mainViewModel)
        {
            this.IsRunning = mainViewModel.IsRunning;
            this.BufferIsPower = mainViewModel.BufferIsPower;
            mainViewModel.PropertyChanged += new PropertyChangedEventHandler(this.MainViewModel_PropertyChanged);
        }

        private void MainViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            MainViewModel mainViewModel = sender as MainViewModel;
            if (sender == null)
                return;
            string propertyName = e.PropertyName;
            if (!(propertyName == "IsRunning"))
            {
                if (!(propertyName == "BufferIsPower"))
                    return;
                this.Invoke((Action)(() => this.BufferIsPower = mainViewModel.BufferIsPower));
            }
            else
                this.Invoke((Action)(() => this.IsRunning = mainViewModel.IsRunning));
        }

        public void Invoke(Action action) => this.PulseAnalysisSettingsService.Dispatcher.Invoke(action);

        protected bool IsRunning
        {
            get => this.isRunning;
            set
            {
                if (!this.OnPropertyChanged<bool>(ref this.isRunning, value, nameof(IsRunning)))
                    return;
                this.NotificationHandlerCommonTail();
            }
        }

        protected bool BufferIsPower
        {
            get => this._BufferIsPower;
            set
            {
                if (!this.OnPropertyChanged<bool>(ref this._BufferIsPower, value, nameof(BufferIsPower)))
                    return;
                this.NotificationHandlerCommonTail();
            }
        }

        private void NotificationHandlerCommonTail()
        {
            this.OnPropertyChanged("AnalysisAllowed", (object)this.AnalysisAllowed);
            this.OnPropertyChanged("EditOptionsAllowed");
            this.OnPropertyChanged("CanExportAnalysisReport");
        }

        public bool AnalysisAllowed => !this.IsRunning && this.BufferIsPower;

        public bool EditOptionsAllowed => !this.IsRunning;

        public bool CanExportAnalysisReport => !this.IsRunning;

        public bool ResultsPresent
        {
            get => this.resultsPresent;
            set
            {
                this.OnPropertyChanged<bool>(ref this.resultsPresent, value, nameof(ResultsPresent));
                this.OnPropertyChanged("CanExportAnalysisReport");
                this.OnPropertyChanged("VisibleIfResultsPresent");
                this.OnPropertyChanged("VisibleIfResultsMissing");
            }
        }

        public Visibility VisibleIfResultsPresent => !this.ResultsPresent ? Visibility.Collapsed : Visibility.Visible;

        public Visibility VisibleIfResultsMissing => !this.ResultsPresent ? Visibility.Visible : Visibility.Collapsed;

        public event Action UpdateView;

        private void OnUpdateView()
        {
            Action updateView = this.UpdateView;
            if (updateView == null)
                return;
            updateView();
        }

        public event Action UpdateComputations;

        private void OnUpdateComputations()
        {
            if (this.Lock == null)
                return;
            lock (this.Lock)
            {
                if (this.Updating)
                    return;
                this.Updating = true;
            }
            if (this.UpdateComputations != null)
                this.UpdateComputations();
            this.Updating = false;
        }

        protected bool PropertyChangingRecompute<T>(ref T value, T newValue, [CallerMemberName] string name = null)
        {
            int num = this.OnPropertyChanged<T>(ref value, newValue, name) ? 1 : 0;
            this.OnUpdateComputations();
            return num != 0;
        }

        protected bool PropertyChangingRedraw<T>(ref T value, T newValue, [CallerMemberName] string name = null)
        {
            int num = this.OnPropertyChanged<T>(ref value, newValue, name) ? 1 : 0;
            this.OnUpdateView();
            return num != 0;
        }

        public void OnBaselinePropertiesChanged()
        {
            this.OnPropertyChanged("EnergyBaselineOption");
            this.OnPropertyChanged("EnergyBaselineLevel");
            this.OnPropertyChanged("EnergyBaseline_LiteralZero_IsSelected");
            this.OnPropertyChanged("EnergyBaseline_LowerCursor_IsSelected");
            this.OnPropertyChanged("EnergyBaseline_CustomLevel_IsSelected");
            this.OnPropertyChanged("EnergyBaselineLowerCursorLevel");
            this.OnPropertyChanged("EnergyBaselineCustomLevel");
        }

        [API(APICategory.Unclassified)]
        [DataMember]
        public EnergyBaselineOption EnergyBaselineOption
        {
            get => this.energyBaselineOption;
            set
            {
                if (!this.OnPropertyChanged<EnergyBaselineOption>(ref this.energyBaselineOption, value, nameof(EnergyBaselineOption)))
                    return;
                this.OnPropertyChanged("EnergyBaselineLevel");
                this.OnPropertyChanged("EnergyBaseline_LiteralZero_IsSelected");
                this.OnPropertyChanged("EnergyBaseline_LowerCursor_IsSelected");
                this.OnPropertyChanged("EnergyBaseline_CustomLevel_IsSelected");
                this.OnUpdateComputations();
            }
        }

        [API(APICategory.Unclassified)]
        public bool EnergyBaseline_LiteralZero_IsSelected
        {
            get => this.EnergyBaselineOption == EnergyBaselineOption.Zero;
            set
            {
                if (!value)
                    return;
                this.EnergyBaselineOption = EnergyBaselineOption.Zero;
            }
        }

        [API(APICategory.Unclassified)]
        public bool EnergyBaseline_LowerCursor_IsSelected
        {
            get => this.EnergyBaselineOption == EnergyBaselineOption.LowerCursor;
            set
            {
                if (!value)
                    return;
                this.EnergyBaselineOption = EnergyBaselineOption.LowerCursor;
            }
        }

        [API(APICategory.Unclassified)]
        public bool EnergyBaseline_CustomLevel_IsSelected
        {
            get => this.EnergyBaselineOption == EnergyBaselineOption.Custom;
            set
            {
                if (!value)
                    return;
                this.EnergyBaselineOption = EnergyBaselineOption.Custom;
            }
        }

        [API(APICategory.Unclassified)]
        [DataMember]
        public double EnergyBaselineLowerCursorLevel
        {
            get => this.energyBaselineLowerCursorLevel;
            set
            {
                if (!this.OnPropertyChanged<double>(ref this.energyBaselineLowerCursorLevel, value, nameof(EnergyBaselineLowerCursorLevel)))
                    return;
                this.EnergyBaselineLowerCursorLevelText = ValueFormatter.FormatWithSuffix(this.energyBaselineLowerCursorLevel, this.Computations.Units);
                if (!this.EnergyBaseline_LowerCursor_IsSelected)
                    return;
                this.OnPropertyChanged("EnergyBaseline");
                this.OnPropertyChanged("EnergyBaselineLevel");
            }
        }

        [API(APICategory.Unclassified)]
        [DataMember]
        public string EnergyBaselineLowerCursorLevelText
        {
            get => this.energyBaselineLowerCursorLevelText;
            set => this.OnPropertyChanged<string>(ref this.energyBaselineLowerCursorLevelText, value, nameof(EnergyBaselineLowerCursorLevelText));
        }

        [API(APICategory.Unclassified)]
        [DataMember]
        public double EnergyBaselineCustomLevel
        {
            get => this.energyBaselineCustomLevel;
            set
            {
                if (!this.OnPropertyChanged<double>(ref this.energyBaselineCustomLevel, value, nameof(EnergyBaselineCustomLevel)))
                    return;
                if (this.EnergyBaseline_CustomLevel_IsSelected)
                {
                    this.OnPropertyChanged("EnergyBaseline");
                    this.OnPropertyChanged("EnergyBaselineLevel");
                }
                this.OnUpdateComputations();
            }
        }

        [API(APICategory.Unclassified)]
        [DataMember]
        public bool PulseAnalysisEnabled
        {
            get => this.pulseAnalysisEnabled;
            set => this.OnPropertyChanged<bool>(ref this.pulseAnalysisEnabled, value, nameof(PulseAnalysisEnabled));
        }

        [API(APICategory.Unclassified)]
        [DataMember]
        public bool ShowSelectionBounds_IsChecked
        {
            get => this.showSelectionBounds_IsChecked;
            set => this.OnPropertyChanged<bool>(ref this.showSelectionBounds_IsChecked, value, nameof(ShowSelectionBounds_IsChecked));
        }

        [API(APICategory.Unclassified)]
        [DataMember]
        public bool ShowTrackingCursor_IsChecked
        {
            get => this.showTrackingCursor_IsChecked;
            set => this.OnPropertyChanged<bool>(ref this.showTrackingCursor_IsChecked, value, nameof(ShowTrackingCursor_IsChecked));
        }

        [API(APICategory.Unclassified)]
        [DataMember]
        public bool ShowTriggerMarks_IsChecked
        {
            get => this.showTriggerMarks_IsChecked;
            set => this.OnPropertyChanged<bool>(ref this.showTriggerMarks_IsChecked, value, nameof(ShowTriggerMarks_IsChecked));
        }

        [API(APICategory.Unclassified)]
        [DataMember]
        public bool SnapToTriggers_IsChecked
        {
            get => this.snapToTriggers_IsChecked;
            set => this.OnPropertyChanged<bool>(ref this.snapToTriggers_IsChecked, value, nameof(SnapToTriggers_IsChecked));
        }

        [API(APICategory.Unclassified)]
        [DataMember]
        public bool ShowAnalysisLevelHigh_IsChecked
        {
            get => this.showAnalysisLevelHigh_IsChecked;
            set => this.OnPropertyChanged<bool>(ref this.showAnalysisLevelHigh_IsChecked, value, nameof(ShowAnalysisLevelHigh_IsChecked));
        }

        [API(APICategory.Unclassified)]
        [DataMember]
        public bool ShowAnalysisLevelMiddle_IsChecked
        {
            get => this.showAnalysisLevelMiddle_IsChecked;
            set => this.OnPropertyChanged<bool>(ref this.showAnalysisLevelMiddle_IsChecked, value, nameof(ShowAnalysisLevelMiddle_IsChecked));
        }

        [API(APICategory.Unclassified)]
        [DataMember]
        public bool ShowAnalysisLevelLow_IsChecked
        {
            get => this.showAnalysisLevelLow_IsChecked;
            set => this.OnPropertyChanged<bool>(ref this.showAnalysisLevelLow_IsChecked, value, nameof(ShowAnalysisLevelLow_IsChecked));
        }

        [API(APICategory.Unclassified)]
        [DataMember]
        public bool ShowEnergyBaselineLevel_IsChecked
        {
            get => this.showEnergyBaselineLevel_IsChecked;
            set => this.OnPropertyChanged<bool>(ref this.showEnergyBaselineLevel_IsChecked, value, nameof(ShowEnergyBaselineLevel_IsChecked));
        }

        [API(APICategory.Unclassified)]
        [DataMember]
        public bool ShowMinimumLevel_IsChecked
        {
            get => this.showMinimumLevel_IsChecked;
            set => this.OnPropertyChanged<bool>(ref this.showMinimumLevel_IsChecked, value, nameof(ShowMinimumLevel_IsChecked));
        }

        [API(APICategory.Unclassified)]
        [DataMember]
        public bool ShowMaximumLevel_IsChecked
        {
            get => this.showMaximumLevel_IsChecked;
            set => this.OnPropertyChanged<bool>(ref this.showMaximumLevel_IsChecked, value, nameof(ShowMaximumLevel_IsChecked));
        }

        [API(APICategory.Unclassified)]
        [DataMember]
        public bool ShowPeakPowerLevel_IsChecked
        {
            get => this.showPeakPowerLevel_IsChecked;
            set => this.OnPropertyChanged<bool>(ref this.showPeakPowerLevel_IsChecked, value, nameof(ShowPeakPowerLevel_IsChecked));
        }

        [API(APICategory.Unclassified)]
        [DataMember]
        public bool ShowAveragePowerLevel_IsChecked
        {
            get => this.showAveragePowerLevel_IsChecked;
            set => this.OnPropertyChanged<bool>(ref this.showAveragePowerLevel_IsChecked, value, nameof(ShowAveragePowerLevel_IsChecked));
        }

        [API(APICategory.Unclassified)]
        [DataMember]
        public bool ShowRiseTimeStart_IsChecked
        {
            get => this.showRiseTimeStart_IsChecked;
            set => this.OnPropertyChanged<bool>(ref this.showRiseTimeStart_IsChecked, value, nameof(ShowRiseTimeStart_IsChecked));
        }

        [API(APICategory.Unclassified)]
        [DataMember]
        public bool ShowRiseTimeStop_IsChecked
        {
            get => this.showRiseTimeStop_IsChecked;
            set => this.OnPropertyChanged<bool>(ref this.showRiseTimeStop_IsChecked, value, nameof(ShowRiseTimeStop_IsChecked));
        }

        [API(APICategory.Unclassified)]
        [DataMember]
        public bool ShowFallTimeStart_IsChecked
        {
            get => this.showFallTimeStart_IsChecked;
            set => this.OnPropertyChanged<bool>(ref this.showFallTimeStart_IsChecked, value, nameof(ShowFallTimeStart_IsChecked));
        }

        [API(APICategory.Unclassified)]
        [DataMember]
        public bool ShowFallTimeStop_IsChecked
        {
            get => this.showFallTimeStop_IsChecked;
            set => this.OnPropertyChanged<bool>(ref this.showFallTimeStop_IsChecked, value, nameof(ShowFallTimeStop_IsChecked));
        }

        [API(APICategory.Unclassified)]
        [DataMember]
        public bool ShowMiddleTimeStart_IsChecked
        {
            get => this.showMiddleTimeStart_IsChecked;
            set => this.OnPropertyChanged<bool>(ref this.showMiddleTimeStart_IsChecked, value, nameof(ShowMiddleTimeStart_IsChecked));
        }

        [API(APICategory.Unclassified)]
        [DataMember]
        public bool ShowMiddleTimeStop_IsChecked
        {
            get => this.showMiddleTimeStop_IsChecked;
            set => this.OnPropertyChanged<bool>(ref this.showMiddleTimeStop_IsChecked, value, nameof(ShowMiddleTimeStop_IsChecked));
        }

        [API(APICategory.Unclassified)]
        [DataMember]
        public bool ShowEnergyTimeStart_IsChecked
        {
            get => this.showEnergyTimeStart_IsChecked;
            set => this.OnPropertyChanged<bool>(ref this.showEnergyTimeStart_IsChecked, value, nameof(ShowEnergyTimeStart_IsChecked));
        }

        [API(APICategory.Unclassified)]
        [DataMember]
        public bool ShowEnergyTimeStop_IsChecked
        {
            get => this.showEnergyTimeStop_IsChecked;
            set => this.OnPropertyChanged<bool>(ref this.showEnergyTimeStop_IsChecked, value, nameof(ShowEnergyTimeStop_IsChecked));
        }

        [API(APICategory.Unclassified)]
        [DataMember]
        public bool ShowPeakPowerTime_IsChecked
        {
            get => this.showPeakPowerTime_IsChecked;
            set => this.OnPropertyChanged<bool>(ref this.showPeakPowerTime_IsChecked, value, nameof(ShowPeakPowerTime_IsChecked));
        }

        [API(APICategory.Unclassified)]
        [DataMember]
        public double LowerPercent
        {
            get => this.lowerPercent;
            set
            {
                if (!this.OnPropertyChanged<double>(ref this.lowerPercent, value, nameof(LowerPercent)))
                    return;
                this.lowerThreshold = this.CalculateThreshold(this.lowerPercent);
                if (this.middlePercent >= this.lowerPercent)
                    return;
                this.MiddlePercent = this.lowerPercent;
            }
        }

        [API(APICategory.Unclassified)]
        [DataMember]
        public double MiddlePercent
        {
            get => this.middlePercent;
            set
            {
                if (!this.OnPropertyChanged<double>(ref this.middlePercent, value, nameof(MiddlePercent)))
                    return;
                this.middleThreshold = this.CalculateThreshold(this.middlePercent);
                if (this.lowerPercent > this.middlePercent)
                    this.LowerPercent = this.middlePercent;
                if (this.upperPercent >= this.middlePercent)
                    return;
                this.UpperPercent = this.middlePercent;
            }
        }

        [API(APICategory.Unclassified)]
        [DataMember]
        public double UpperPercent
        {
            get => this.upperPercent;
            set
            {
                if (!this.OnPropertyChanged<double>(ref this.upperPercent, value, nameof(UpperPercent)))
                    return;
                this.upperThreshold = this.CalculateThreshold(this.upperPercent);
                if (this.middlePercent <= this.upperPercent)
                    return;
                this.MiddlePercent = this.upperPercent;
            }
        }

        [API(APICategory.Unclassified)]
        [DataMember]
        public double MinimumThreshold
        {
            get => this.minimumThreshold;
            set => this.OnPropertyChanged<double>(ref this.minimumThreshold, value, nameof(MinimumThreshold));
        }

        [API(APICategory.Unclassified)]
        [DataMember]
        public double MaximumThreshold
        {
            get => this.maximumThreshold;
            set
            {
                if (!this.OnPropertyChanged<double>(ref this.maximumThreshold, value, nameof(MaximumThreshold)))
                    return;
                this.lowerThreshold = this.CalculateThreshold(this.lowerPercent);
                this.middleThreshold = this.CalculateThreshold(this.middlePercent);
                this.upperThreshold = this.CalculateThreshold(this.upperPercent);
            }
        }

        [API(APICategory.Unclassified)]
        public double LowerThreshold
        {
            get => this.lowerThreshold;
            set
            {
                if (!this.OnPropertyChanged<double>(ref this.lowerThreshold, value, nameof(LowerThreshold)))
                    return;
                this.LowerPercent = this.CalculateRatio(this.lowerThreshold);
            }
        }

        [API(APICategory.Unclassified)]
        public double MiddleThreshold
        {
            get => this.middleThreshold;
            set
            {
                if (!this.OnPropertyChanged<double>(ref this.middleThreshold, value, nameof(MiddleThreshold)))
                    return;
                this.MiddlePercent = this.CalculateRatio(this.middleThreshold);
            }
        }

        [API(APICategory.Unclassified)]
        public double UpperThreshold
        {
            get => this.upperThreshold;
            set
            {
                if (!this.OnPropertyChanged<double>(ref this.upperThreshold, value, nameof(UpperThreshold)))
                    return;
                this.UpperPercent = this.CalculateRatio(this.upperThreshold);
            }
        }

        public double EnergyBaselineLevel
        {
            get
            {
                switch (this.EnergyBaselineOption)
                {
                    case EnergyBaselineOption.LowerCursor:
                        return this.EnergyBaselineLowerCursorLevel;
                    case EnergyBaselineOption.Custom:
                        return this.EnergyBaselineCustomLevel;
                    default:
                        return 0.0;
                }
            }
        }

        [API(APICategory.Unclassified)]
        public Zone Classify(double value)
        {
            if (value < this.LowerThreshold)
                return Zone.Bottom;
            if (value < this.MiddleThreshold)
                return Zone.LowerHalf;
            return value < this.UpperThreshold ? Zone.UpperHalf : Zone.Top;
        }

        public bool SetMinAndMax(double minimum, double maximum)
        {
            this.MinimumThreshold = minimum;
            this.MaximumThreshold = maximum;
            this.CalculateThresholds();
            return true;
        }

        public void SetThresholds(double lower, double middle, double upper)
        {
            this.lowerThreshold = lower;
            this.middleThreshold = middle;
            this.upperThreshold = upper;
            this.SetPercents(this.CalculateRatio(lower), this.CalculateRatio(middle), this.CalculateRatio(upper));
        }

        public void SetBounds(BufferBounds bounds) => this.EnergyBaselineLowerCursorLevel = bounds.Lower;

        public void SetPercents(double lower, double middle, double upper)
        {
            this.LowerPercent = lower;
            this.MiddlePercent = middle;
            this.UpperPercent = upper;
            this.CalculateThresholds();
        }

        public void SetPercents(double[] threshold)
        {
            if (threshold == null)
                this.SetDefaultPercents();
            else
                this.SetPercents(threshold[0], threshold[1], threshold[2]);
        }

        public void SetPercents(LevelCombination levels) => this.SetPercents(levels.Lower, levels.Middle, levels.Upper);

        public void SetDefaultPercents() => this.SetPercents(this.DefaultThresholds);

        public void CalculateThresholds()
        {
            this.LowerThreshold = this.CalculateThreshold(this.LowerPercent);
            this.MiddleThreshold = this.CalculateThreshold(this.MiddlePercent);
            this.UpperThreshold = this.CalculateThreshold(this.UpperPercent);
        }

        public double CalculateThreshold(double ratio) => ratio * this.MaximumThreshold;

        public double CalculateRatio(double threshold) => this.MaximumThreshold == 0.0 ? 0.0 : threshold / this.MaximumThreshold;

        public IEnumerable<PopularOption> PopularOptions => PulseAnalysisOptions.StandardOptions.Concat<PopularOption>((IEnumerable<PopularOption>)this.PopularAdditions);

        public PopularOption SelectedPopularOption() => this.Search(this.PopularOptions, this.LowerPercent, this.MiddlePercent, this.UpperPercent);

        public PopularOption Search(
          IEnumerable<PopularOption> options,
          double lower,
          double middle,
          double upper)
        {
            return this.Search(options, new LevelCombination(lower, middle, upper));
        }

        public PopularOption Search(
          IEnumerable<PopularOption> options,
          LevelCombination levels)
        {
            foreach (PopularOption option in options)
            {
                if (levels.Matches(option.Value))
                    return option;
            }
            return PopularOption.CustomOption;
        }

        public void AddPopularOption_Execute() => this.PopularAdditions.Add(new PopularOption(new LevelCombination(this.LowerPercent, this.MiddlePercent, this.UpperPercent)));

        public void RemovePopularOption_Execute() => this.PopularAdditions.Remove(this.SelectedPopularOption());

        public bool AddPopularOption_CanExecute() => this.SelectedPopularOption() == PopularOption.CustomOption;

        public bool RemovePopularOption_CanExecute() => PopularOption.CustomOption != this.PopularAdditions.Search(this.SelectedPopularOption().Value);

        [DataMember]
        public double SamplePeriod_Seconds { get; set; }

        [DataMember]
        public Level WidthStartThreshold
        {
            get => this.widthStartThreshold;
            set => this.OnPropertyChanged<Level>(ref this.widthStartThreshold, value, nameof(WidthStartThreshold));
        }

        [DataMember]
        public Level WidthStopThreshold
        {
            get => this.widthStopThreshold;
            set => this.OnPropertyChanged<Level>(ref this.widthStopThreshold, value, nameof(WidthStopThreshold));
        }

        public void SetWidthThresholds(Level startThreshold, Level stopThreshold)
        {
            this.widthStartThreshold = startThreshold;
            this.widthStopThreshold = stopThreshold;
        }

        public void InitializeWidthThresholds() => this.SetWidthThresholds(Level.Lower, Level.Lower);

        public ICommand ExportAnalysisReport => this._ExportAnalysisReport ?? (this._ExportAnalysisReport = (ICommand)new RelayCommand(new Action(this.ExportAnalysisReportDialog), (Func<bool>)(() => this.CanExportAnalysisReport)));

        public void ExportAnalysisReportDialog() => new PulseAnalysisWriteReportDialog(this.PhoenixMeter).ShowDialog();

        public ICommand ExportPulseAnalysisSettingsCommand => this._ExportPulseAnalysisSettingsCommand ?? (this._ExportPulseAnalysisSettingsCommand = (ICommand)new RelayCommand(new Action(this.ExportPulseAnalysisSettingsDialog), (Func<bool>)(() => this.CanExportAnalysisReport)));

        [DataMember]
        public string ImportExportPathname
        {
            get => this._ImportExportPathname ?? (this._ImportExportPathname = Path.Combine(StandardPathnames.DesktopFolder, "PulseAnalysisSettings.txt"));
            set => this._ImportExportPathname = value;
        }

        public void ExportPulseAnalysisSettingsDialog()
        {
            ExportDialog exportDialog1 = new ExportDialog();
            exportDialog1.Title = "Export Pulse Analysis Settings to File";
            exportDialog1.InitialDirectory = Path.GetDirectoryName(this.ImportExportPathname);
            exportDialog1.FileName = this.ImportExportPathname;
            exportDialog1.FilterIndex = 1;
            exportDialog1.Filter = "TXT Files (*.txt)|*.txt|All Files (*.*)|*.*";
            ExportDialog exportDialog2 = exportDialog1;
            bool? nullable = exportDialog2.ShowDialog();
            bool flag = true;
            if (!(nullable.GetValueOrDefault() == flag & nullable.HasValue))
                return;
            this.ExportPulseAnalysisSettings(exportDialog2.FileName);
            this.ImportExportPathname = exportDialog2.FileName;
        }

        public void ExportPulseAnalysisSettings(string filename)
        {
            using (TextWriter textUtF8 = (TextWriter)FileX.CreateTextUTF8(filename))
                textUtF8.WriteLine(this.Settings);
        }

        public ICommand ImportPulseAnalysisSettingsCommand => this._ImportPulseAnalysisSettingsCommand ?? (this._ImportPulseAnalysisSettingsCommand = (ICommand)new RelayCommand(new Action(this.ImportPulseAnalysisSettingsDialog)));

        public void ImportPulseAnalysisSettingsDialog()
        {
            ImportDialog importDialog1 = new ImportDialog();
            importDialog1.Title = "Import Pulse Analysis Settings from File";
            importDialog1.InitialDirectory = Path.GetDirectoryName(this.ImportExportPathname);
            importDialog1.FileName = this.ImportExportPathname;
            importDialog1.FilterIndex = 1;
            importDialog1.Filter = "TXT Files (*.txt)|*.txt|All Files (*.*)|*.*";
            ImportDialog importDialog2 = importDialog1;
            bool? nullable = importDialog2.ShowDialog();
            bool flag = true;
            if (!(nullable.GetValueOrDefault() == flag & nullable.HasValue))
                return;
            this.ImportPulseAnalysisSettings(importDialog2.FileName);
            this.ImportExportPathname = importDialog2.FileName;
        }

        public void ImportPulseAnalysisSettings(string filename)
        {
            using (TextReader textReader = (TextReader)File.OpenText(filename))
                this.Settings = textReader.ReadToEnd();
        }

        [API(APICategory.Unclassified)]
        public string Settings
        {
            get => SharedLibrary.Serialization.Serialize((object)this, "V1.5.0.7");
            set
            {
                try
                {
                    this.SetDefaults();
                    if (string.IsNullOrWhiteSpace(value))
                        return;
                    this.DeSerialize(value, "V1.5.0.7");
                }
                catch (Library.ImportExport.Exception ex)
                {
                }
            }
        }

        [System.Runtime.Serialization.OnDeserializing]
        private void OnDeserializing(StreamingContext c) => this.SetDefaults();

        [DataMember]
        public int SettingsTab_SelectedIndex { get; set; }

        public static void WriteLine(TextWriter writer, params string[] fields) => writer.WriteLine(string.Join(PulseAnalysisOptions.Separator, fields));

        private string PercentFormat(double ratio) => (100.0 * ratio).ToString() + "%";

        public void WriteValue(TextWriter writer, string name, double value) => PulseAnalysisOptions.WriteLine(writer, name, this.PercentFormat(this.CalculateRatio(value)), value.ToString());

        public void WritePercent(TextWriter writer, string name, double ratio) => PulseAnalysisOptions.WriteLine(writer, name, this.PercentFormat(ratio), this.CalculateThreshold(ratio).ToString());

        public void WriteReport(TextWriter writer)
        {
            PulseAnalysisOptions.WriteLine(writer, "Analysis Thresholds");
            PulseAnalysisOptions.WriteLine(writer, "Threshold", "Percent", "Watts");
            this.WriteValue(writer, "Maximum", this.MaximumThreshold);
            this.WritePercent(writer, "Upper", this.UpperPercent);
            this.WritePercent(writer, "Middle", this.MiddlePercent);
            this.WritePercent(writer, "Lower", this.LowerPercent);
            this.WriteValue(writer, "Minimum", this.MinimumThreshold);
            this.WriteValue(writer, "EnergyBaseline", this.EnergyBaselineLevel);
        }

        [Conditional("TRACE")]
        protected void Trace(string format, params object[] args)
        {
        }

        [Conditional("TRACE_THRESHOLDS")]
        protected void TraceThresholds(string format, params object[] args)
        {
        }

        [Conditional("TRACE_SERIALIZATION")]
        protected void TraceSerialization(string format, params object[] args)
        {
        }

        [Conditional("TRACE_PROPERTY_CHANGING")]
        protected void TracePropertyChanging(string message)
        {
        }
    }
}