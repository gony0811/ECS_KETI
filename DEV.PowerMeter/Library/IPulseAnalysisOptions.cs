using System;
using System.Collections.Generic;
using System.IO;

namespace DEV.PowerMeter.Library
{
    public interface IPulseAnalysisOptions
    {
        double EnergyBaselineCustomLevel { get; set; }

        double EnergyBaselineLevel { get; }

        double EnergyBaselineLowerCursorLevel { get; set; }

        EnergyBaselineOption EnergyBaselineOption { get; set; }

        bool EnergyBaseline_CustomLevel_IsSelected { get; set; }

        bool EnergyBaseline_LiteralZero_IsSelected { get; set; }

        bool EnergyBaseline_LowerCursor_IsSelected { get; set; }

        double LowerPercent { get; set; }

        double LowerThreshold { get; set; }

        double MaximumThreshold { get; set; }

        double MiddlePercent { get; set; }

        double MiddleThreshold { get; set; }

        double MinimumThreshold { get; set; }

        bool PulseAnalysisEnabled { get; set; }

        double SamplePeriod_Seconds { get; set; }

        string Settings { get; set; }

        int SettingsTab_SelectedIndex { get; set; }

        bool ShowAnalysisLevelHigh_IsChecked { get; set; }

        bool ShowAnalysisLevelLow_IsChecked { get; set; }

        bool ShowAnalysisLevelMiddle_IsChecked { get; set; }

        bool ShowAveragePowerLevel_IsChecked { get; set; }

        bool ShowEnergyBaselineLevel_IsChecked { get; set; }

        bool ShowEnergyTimeStart_IsChecked { get; set; }

        bool ShowEnergyTimeStop_IsChecked { get; set; }

        bool ShowFallTimeStart_IsChecked { get; set; }

        bool ShowFallTimeStop_IsChecked { get; set; }

        bool ShowMaximumLevel_IsChecked { get; set; }

        bool ShowMiddleTimeStart_IsChecked { get; set; }

        bool ShowMiddleTimeStop_IsChecked { get; set; }

        bool ShowMinimumLevel_IsChecked { get; set; }

        bool ShowPeakPowerLevel_IsChecked { get; set; }

        bool ShowPeakPowerTime_IsChecked { get; set; }

        bool ShowRiseTimeStart_IsChecked { get; set; }

        bool ShowRiseTimeStop_IsChecked { get; set; }

        bool ShowSelectionBounds_IsChecked { get; set; }

        bool ShowTrackingCursor_IsChecked { get; set; }

        bool ShowTriggerMarks_IsChecked { get; set; }

        bool SnapToTriggers_IsChecked { get; set; }

        double UpperPercent { get; set; }

        double UpperThreshold { get; set; }

        Level WidthStartThreshold { get; set; }

        Level WidthStopThreshold { get; set; }

        event Action UpdateComputations;

        double CalculateRatio(double threshold);

        double CalculateThreshold(double ratio);

        void CalculateThresholds();

        Zone Classify(double value);

        void OnBaselinePropertiesChanged();

        PopularOption SelectedPopularOption();

        PopularOption Search(
          IEnumerable<PopularOption> options,
          double lower,
          double middle,
          double upper);

        PopularOption Search(IEnumerable<PopularOption> options, LevelCombination levels);

        void SetDefaultPercents();

        void SetDefaults();

        bool SetMinAndMax(double minimum, double maximum);

        void SetPercents(LevelCombination levels);

        void SetPercents(double[] threshold);

        void SetPercents(double lower, double middle, double upper);

        void SetThresholds(double lower, double middle, double upper);

        void InitializeWidthThresholds();

        void SetWidthThresholds(Level startThreshold, Level stopThreshold);

        void WritePercent(TextWriter writer, string name, double ratio);

        void WriteReport(TextWriter writer);

        void WriteValue(TextWriter writer, string name, double value);
    }
}
