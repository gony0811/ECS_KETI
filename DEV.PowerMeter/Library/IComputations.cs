
using System;
using System.Collections.Generic;


namespace DEV.PowerMeter.Library
{
    public interface IComputations
    {
        int Above { get; }

        int Below { get; }

        BufferBounds BufferBounds { get; }

        CaptureBuffer CaptureBuffer { get; }

        int Count { get; }

        double EnergyBaselineLevel { get; }

        DataRecordSingle FirstSample { get; }

        DataRecordSingle FirstTrigger { get; }

        double Height { get; }

        DataRecordSingle LastSample { get; }

        DataRecordSingle LastTrigger { get; }

        double LeftValue { get; }

        double Maximum { get; }

        double MeasurementSum { get; }

        double Minimum { get; }

        PulseAnalysisOptions PulseAnalysisOptions { get; }

        PulseAnalysisResults PulseAnalysisResults { get; }

        PulseAnalyzer PulseAnalyzer { get; }

        int PulseCount { get; }

        double PulseEnergy { get; }

        double PulsePeriod { get; }

        double PulseRate { get; }

        double PulseWidth { get; }

        double RightValue { get; }

        double SampleWidth { get; }

        double TotalEnergy { get; }

        int TriggerCount { get; }

        Units Units { get; set; }

        TimeSpan Width { get; }

        double Width_Seconds { get; }

        void Add(DataRecordSingle record);

        void AttachCaptureBuffer(CaptureBuffer captureBuffer);

        void Clear();

        double OptionalMeasurement(DataRecordSingle sample);

        void Reload(IEnumerable<DataRecordSingle> data);
    }
}