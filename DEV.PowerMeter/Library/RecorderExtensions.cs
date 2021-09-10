using SharedLibrary;
using System;
using System.Collections.Generic;
using System.Text;

namespace DEV.PowerMeter.Library
{
    public static class RecorderExtensions
    {
        public static string GetRemarks(this Meter meter)
        {
            Sensor selectedSensor = meter.SelectedSensor;
            return StringExtensions.CommaSeparated((IEnumerable<string>)new string[5]
            {
        selectedSensor.SensorTypeAndQualifier,
        selectedSensor.ModelName,
        selectedSensor.SerialNumber,
        meter.GetOpMode(),
        DateTime.Now.ToString()
            });
        }

        public static string GetOpMode(this Meter meter)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(meter.OperatingMode.ToString());
            if (meter.ContinuousMode)
                stringBuilder.Append("+Continuous");
            if (meter.SnapshotModeEnabled)
                stringBuilder.Append("+Snapshot");
            if (meter.WaitForTriggerModeEnabled)
                stringBuilder.Append("+WaitTrigger");
            return stringBuilder.ToString();
        }

        public static string GetFilename(this Meter meter)
        {
            Sensor selectedSensor = meter.SelectedSensor;
            return StringExtensions.CommaSeparated((IEnumerable<string>)new string[5]
            {
        selectedSensor.SensorTypeAndQualifier,
        selectedSensor.ModelName,
        selectedSensor.SerialNumber,
        meter.Capacity.ToString(),
        meter.GetOpMode()
            }).SubstituteIllegalFilenameChars();
        }
    }
}
