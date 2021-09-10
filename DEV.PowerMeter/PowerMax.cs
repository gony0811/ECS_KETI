using DEV.PowerMeter.Library;
using DEV.PowerMeter.Library.ViewModels;
using INNO6.IO.Interface;
using SharedLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DEV.PowerMeter
{
    public class PowerMax : IDeviceHandler
    {
        public bool SuppressCannotOpenMessage;
        public string DeviceName;

        [Conditional("TRACE_START_STOP_EXITS")]
        public void TraceStartStopExits(string fmt, params object[] args) => this.Trace("StartStopExits: " + fmt, args);

        protected bool BufferNonEmpty => this.CaptureBuffer != null && this.CaptureBuffer.Count > 0U;

        protected bool MeterIsOpen => this.PhoenixMeter.IsOpen;

        protected bool MeterCanOpen => !this.Meter.IsRunning;

        public PortManager PortManager => ServiceLocator.Resolve<PortManager>();

        public PhoenixMeter PhoenixMeter => ServiceLocator.Resolve<PhoenixMeter>();

        public Meter Meter => this.PhoenixMeter?.Meter;

        public Statistics Statistics => this.PhoenixMeter?.Statistics;

        public Computations Computations => this.PhoenixMeter?.Computations;

        public CaptureBuffer CaptureBuffer => this.PhoenixMeter == null ? (CaptureBuffer)null : this.PhoenixMeter.CaptureBuffer;

        public CaptureBuffer PreviewBuffer => this.PreviewBufferController == null ? (CaptureBuffer)null : (CaptureBuffer)this.PreviewBufferController.PreviewBuffer;

        public PreviewBufferController PreviewBufferController => this.PhoenixMeter.PreviewBufferController;

        public Sensor SelectedSensor => this.PhoenixMeter == null ? (Sensor)null : this.PhoenixMeter.SelectedSensor;

        public SensorType SelectedSensorType => this.PhoenixMeter == null ? SensorType.None : this.PhoenixMeter.SelectedSensorType;

        protected MainViewModel MainViewModel => ServiceLocator.Resolve<MainViewModel>();

        public OperatingMode OperatingMode => this.PhoenixMeter.OperatingMode;

        public void SetOperatingMode(OperatingMode newMode)
        {
            OperatingMode operatingMode = this.OperatingMode;
            this.PhoenixMeter.OperatingMode = newMode;
        }


        private WavelengthsTable WavelengthsTable => !this.MeterIsOpen ? (WavelengthsTable)null : this.Meter.WavelengthsTable;



        protected IErrorReporter ErrorLogger => ServiceLocator.Resolve<IErrorReporter>();

        public bool IgnoreNextMeterDisconnect { get; protected set; }

        protected ComPortInfo CurrentPortProperties;

        public WavelengthOption SelectedWavelengthOption;

        public bool DeviceAttach(string deviceName, string portName, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7, string arg8, string arg9)
        {
            DeviceName = deviceName;
            this.OpenMeterPort(portName);

            if(this.PhoenixMeter.IsOpen)
            {

            }

            return this.PhoenixMeter.IsOpen;
        }

        public bool DeviceDettach()
        {
            throw new NotImplementedException();
        }

        public bool DeviceInit()
        {
            throw new NotImplementedException();
        }

        public bool DeviceReset()
        {
            return true;
        }

        public object GET_DATA_IN(string id_1, string id_2, string id_3, string id_4, ref bool result)
        {
            throw new NotImplementedException();
        }

        public double GET_DOUBLE_IN(string id_1, string id_2, string id_3, string id_4, ref bool result)
        {
            throw new NotImplementedException();
        }

        public int GET_INT_IN(string id_1, string id_2, string id_3, string id_4, ref bool result)
        {
            throw new NotImplementedException();
        }

        public string GET_STRING_IN(string id_1, string id_2, string id_3, string id_4, ref bool result)
        {
            throw new NotImplementedException();
        }

        public eDevMode IsDevMode()
        {
            if (this.PhoenixMeter.IsOpen) return eDevMode.CONNECT;
            else return eDevMode.DISCONNECT;
        }

        public void SET_DATA_OUT(string id_1, string id_2, string id_3, string id_4, object value, ref bool result)
        {
            throw new NotImplementedException();
        }

        public void SET_DOUBLE_OUT(string id_1, string id_2, string id_3, string id_4, double value, ref bool result)
        {
            throw new NotImplementedException();
        }

        public void SET_INT_OUT(string id_1, string id_2, string id_3, string id_4, int value, ref bool result)
        {
            throw new NotImplementedException();
        }

        public void SET_STRING_OUT(string id_1, string id_2, string id_3, string id_4, string value, ref bool result)
        {
            throw new NotImplementedException();
        }




     
        protected void OpenMeterPort(string portName, bool silent = false)
        {
            this.TraceInit("\nMain.Init: OpenMeterPort( " + portName + " ) ============================");
            try
            {
                if (this.PhoenixMeter.IsOpen)
                    this.CloseMeterPortNoMessage();
                this.TraceInit("OpenMeterPort.PhoenixMeter.Open( " + portName + " )");
                this.PhoenixMeter.Open(portName);
                this.TraceInit("OpenMeterPort.PhoenixMeter.Opened");
                PortManager.SelectedPortName = portName;
                this.CurrentPortProperties = Win32Wrapper.GetUsbComPortProperties(portName);

               
            }
            catch (ObsoleteHardwareOrFirmwareException ex)
            {
                this.CloseMeterPort();
            }
            catch (UnauthorizedAccessException ex)
            {
                this.TraceInit("OpenMeterPort.UnauthorizedAccessException: " + ex.Message);
                this.CloseMeterPort();
                return;
            }
            catch (System.Exception ex)
            {
                this.Trace("OpenMeterPort.Exception" + (silent ? " [Silent]" : "") + ": " + ex.Message);
                if (!silent)
                    this.ShowOpenMeterException(portName, ex);
                this.CloseMeterPort();
                return;
            }

            this.TraceInit("...OpenMeterPort exits");
        }

        private void ShowOpenMeterException(string portName, System.Exception ex)
        {
            if (this.SuppressCannotOpenMessage)
                return;
            string messageBoxText = ex.Message;
            while (ex.InnerException != null)
            {
                ex = ex.InnerException;
                messageBoxText = ex.Message + "\n" + messageBoxText;
            }
            int num = (int)MessageBox.Show(messageBoxText, DeviceName + " " + portName, MessageBoxButton.OK, MessageBoxImage.Hand);
        }


        public void Trace(string format, params object[] args)
        {
            string remainder;
            if (!format.SplitLeadingWhitespace(out string _, out remainder))
                return;
            format = remainder;
        }

        [Conditional("TRACE_INIT")]
        public void TraceInit(string format, params object[] args)
        {
        }

        [Conditional("TRACE_METER_STATUS_CHANGED")]
        public void TraceMeterStatusChanged(string format, params object[] args)
        {
        }

        public void Start()
        {
            if (this.UnsecifiedWavelengthWarning())
                return;
            try
            {
                this.PhoenixMeter.PrepareToStart();
                this.CaptureBuffer.Clear();
                this.PhoenixMeter.Start();
            }
            catch (System.Exception ex)
            {
                this.ErrorLogger.ReportError("Error Starting Meter: {0}", (object)ex.Message);
            }
        }

        protected bool UnsecifiedWavelengthWarning()
        {
            if (!this.MainViewModel.SelectedWavelengthOption.IsDisabled)
                return false;
            return MessageBox.Show("Wavelength Compensation has not been specified for the attached sensor (" + this.Meter.SelectedSensor.SensorTypeAndQualifier + ", #" + this.Meter.SelectedSensor.SerialNumber + ").\n\nPress CANCEL to return to main window and specify the correct value.\nThen select the desired value in the Waveform Correction combo box, before restarting the meter.\n\nPress OK if you prefer to proceed and make these measurements using the Factory Default wavelength correction for the current Sensor (which is not recommended).", "Starting Meter with Unspecified Wavelength", MessageBoxButton.OKCancel, MessageBoxImage.Question, MessageBoxResult.Cancel) == MessageBoxResult.Cancel;
        }


        public void CloseMeterPortNoMessage()
        {
            this.TraceMeterStatusChanged("CloseMeterPortNoMessage...");
            this.IgnoreNextMeterDisconnect = true;
            this.CloseMeterPort();
            this.TraceMeterStatusChanged("...CloseMeterPortNoMessage");
        }

        public void CloseMeterPort()
        {
            this.TraceInit("CloseMeterPort...");
            this.PhoenixMeter.Close();
            CMC_CLA.Current.Clear();
            this.TraceInit("...CloseMeterPort");
        }
    }
}
