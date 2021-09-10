using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedLibrary;

namespace DEV.PowerMeter.Library
{
    [API(APICategory.Measurement, "This class includes all the information collected during polling. The PhoenixMeter object has a MeasurementUpdate Event that can be registered for. When the meter is not running, the handler will be called with a recently updated instance of this class. ")]
    public class PollingData
    {
        [API("System Status")]
        public SystemStatusBits SystemStatus;
        [API("System Faults. System Faults are not updated unless SystemStatus shows a SystemFault flag set.")]
        public SystemFaultBits SystemFaults;
        [API("Sensor Temperature (if sensor supports Temperature readings)")]
        public string SensorTemperature;
        [API("At polling time, the meter reads a single measurement record. This is it. ")]
        public string DataRecord;
        [API("At polling time, a Meter was visible to this software. ")]
        public bool HasMeter;
        [API("At polling time, a Sensor was visible to this software. ")]
        public bool HasSensor;

        [API("True iff PollingThread is actively requesting polling data. ")]
        public static bool IsPolling { get; protected set; }

        public SystemStatusBits SystemStatus_SensorFlags => this.SystemStatus & SystemStatusBits.SensorFlags;

        [API("At polling time, the Meter was able to perform a measurement and send it to this software. ")]
        public bool HasData => !string.IsNullOrEmpty(this.DataRecord);

        [API("At polling time, Sensor Temperature was visible to this software. ")]
        public bool HasTemperature => !string.IsNullOrEmpty(this.SensorTemperature);

        [API("Constructor")]
        public PollingData()
        {
        }

        [API("Clear properties to defaults.")]
        public void Clear()
        {
            this.SystemStatus = (SystemStatusBits)0;
            this.SystemFaults = (SystemFaultBits)0;
            this.SensorTemperature = "";
            this.DataRecord = "";
            this.HasMeter = false;
            this.HasSensor = false;
        }

        [API("Method for fetching Polling Data from a meter. This method is invoked from Meter.UpdateSystemStatusAndFaults, via a call to Communicator.LockAndDelegate, which guarantees exclusive Meter access to this method for the duration of this polling action.")]
        public void FetchData(Meter meter)
        {
            try
            {
                PollingData.IsPolling = true;
                Device device = meter.Device;
                this.Clear();
                try
                {
                    this.HasMeter = true;
                    this.SystemStatus = device.SystemStatus_AsEnum;
                    this.SystemFaults = (this.SystemStatus & SystemStatusBits.SystemFault) != (SystemStatusBits)0 ? device.SystemFaults_AsEnum : (SystemFaultBits)0;
                }
                catch (Exception ex)
                {
                    this.HasMeter = false;
                    return;
                }
                this.HasSensor = (this.SystemStatus & SystemStatusBits.SensorFlags) == SystemStatusBits.SensorIsAttached;
                if (!this.HasSensor)
                    return;
                try
                {
                    if (meter.SensorHasTemperature)
                        this.SensorTemperature = device.SensorTemperature;
                }
                catch (Exception ex)
                {
                }
                try
                {
                    this.DataRecord = device.ReadNewDataRecord();
                }
                catch (Exception ex)
                {
                }
            }
            finally
            {
                PollingData.IsPolling = false;
            }
        }

        [Conditional("TRACE_POLLING")]
        public void Trace(string message)
        {
        }
    }
}
