using DEV.PowerMeter.Library.DeviceModels;
using SharedLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DEV.PowerMeter.Library
{
    [API(APICategory.Utility, "PortManager examines all the available COM ports on a PC,\r\nand selects suitable ones for use by LabMax-Pro software.\r\n\r\nUSB ports are included if they have a suitable USB device ID.\r\nAll RS-232 ports also are included (non-Meters will be rejected when Opened).\r\nAll USB ports also are include, to handle the case where they are \r\nUSB->RS232 converters.\r\n\r\nIn the Example Project, the FindAndOpenSuitablePort shows a suitable way to\r\nuse this class.\r\nAll this complexity may be dispensed with if you can simply utilize a fixed COM port.\r\n")]
    public class PortManager
    {
        [API("A list of all available ports.")]
        public List<ComPortInfo> AllPorts;
        [API("A dictionary mapping port names onto ComPortInfo objects.")]
        protected Dictionary<string, ComPortInfo> IndexByPortName;
        [API("A dictionary mapping port FriendlyNames onto ComPortInfo objects.")]
        protected Dictionary<string, ComPortInfo> IndexByFriendlyName;
        private const string NL = "\r\n";
        public const string SCPI_ClearErrors = "SYST:ERR:CLE";
        public const string SCPI_StopStop = "abort\r\ndsp";
        public const string SCPI_InitializeAny = "\r\nabort\r\ndsp\r\nSYST:ERR:CLE\r\n";
        public const string SCPI_IdnQuery = "*idn?";
        public static readonly Dictionary<string, Type> ValidIdentPrefixes = new Dictionary<string, Type>()
    {
      {
        "Coherent, Inc - LabMax-Pro SSIM",
        typeof (Meter_SSIM)
      },
      {
        "Coherent, Inc - PowerMax-Pro USB",
        typeof (Meter_SSIM)
      },
      {
        "Coherent, Inc - PowerMax-Pro RS",
        typeof (Meter_SSIM)
      },
      {
        "Coherent, Inc - PowerMax USB",
        typeof (Meter_MP)
      },
      {
        "Coherent, Inc - PowerMax RS",
        typeof (Meter_MP)
      },
      {
        "Coherent, Inc - EnergyMax USB",
        typeof (Meter_ME)
      },
      {
        "Coherent, Inc - EnergyMax RS",
        typeof (Meter_ME)
      }
    };
        public int[] BaudRates = new int[2] { 115200, 9600 };

        [API("Selected Port Name, a static/global setting for the system.")]
        public static string SelectedPortName { get; set; }

        public IEnumerable<ComPortInfo> FriendlyNames => (IEnumerable<ComPortInfo>)this.IndexByFriendlyName.Values.OrderBy<ComPortInfo, int>((Func<ComPortInfo, int>)(cpi => this.PortNameNumericSuffix(cpi.PortName)));

        private int PortNameNumericSuffix(string portname)
        {
            int result;
            return portname.StartsWith("COM") && int.TryParse(portname.Substring(3), out result) ? result : 0;
        }

        [API("The number of ports in this collection.")]
        public int Count => this.AllPorts.Count;

        [API("Access the collection by integer index.")]
        public ComPortInfo this[int index]
        {
            get
            {
                try
                {
                    return this.AllPorts[index];
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    return (ComPortInfo)null;
                }
            }
        }

        [API("Access the collection by portName.")]
        public ComPortInfo this[string portName]
        {
            get
            {
                try
                {
                    return this.IndexByPortName[portName];
                }
                catch (KeyNotFoundException ex)
                {
                    return (ComPortInfo)null;
                }
            }
        }

        [API("Membership test for portName.")]
        public bool HasPortName(string portName) => !string.IsNullOrEmpty(portName) && this.IndexByPortName.Keys.Contains<string>(portName);

        [API("Construct a PortManager, and initialize it's list of ports.\r\nThere is no PortManager object in PhoenixMeter, \r\nso if one is used it needs to be created by the application.")]
        public PortManager() => this.Refresh();

        [API("Clear the list.")]
        public void Clear()
        {
            this.AllPorts = new List<ComPortInfo>();
            this.IndexByPortName = new Dictionary<string, ComPortInfo>((IEqualityComparer<string>)StringComparer.InvariantCultureIgnoreCase);
            this.IndexByFriendlyName = new Dictionary<string, ComPortInfo>((IEqualityComparer<string>)StringComparer.InvariantCultureIgnoreCase);
        }

        [API("Update the list and rebuild indicies.")]
        public void Refresh()
        {
            this.Clear();
            this.AllPorts = Win32Wrapper.GetComPortInfo();
            foreach (ComPortInfo allPort in this.AllPorts)
                this.AddIndex(allPort);
        }

        protected void AddIndex(ComPortInfo info)
        {
            this.IndexByFriendlyName[info.FriendlyName] = info;
            this.IndexByPortName[info.PortName] = info;
        }

        public void Add(ComPortInfo info) => this.AllPorts.Add(info);

        public void Add(string portName)
        {
            ComPortInfo info = new ComPortInfo(portName);
            this.AllPorts.Add(info);
            this.AddIndex(info);
        }

        [API("finds the first qualified candidate, \r\nintentionally ignoring excludePort, if specified.")]
        public ComPortInfo FindQualifiedCandidate(string excludePort = null)
        {
            foreach (ComPortInfo allPort in this.AllPorts)
            {
                if (allPort.IsQualified && (excludePort == null || allPort.PortName != excludePort))
                    return allPort;
            }
            return (ComPortInfo)null;
        }

        [API("finds the Name of the first qualified candidate, \r\nintentionally ignoring excludePort, if specified.")]
        public string FindQualifiedCandidateName(string excludePort = null) => this.FindQualifiedCandidate(excludePort)?.PortName;

        public static PortManager.ValidationResults ValidateDevice(Channel channel)
        {
            for (int index = 0; index < 3; ++index)
            {
                channel.WriteLine("\r\nabort\r\ndsp\r\nSYST:ERR:CLE\r\n");
                if (channel.SkipUnusedData(600f))
                    break;
            }
            Stopwatch.StartNew();
            string oneLine = channel.SendAndReceiveOneLine("*idn?");
            channel.SkipUnusedData(600f);
            if (!string.IsNullOrWhiteSpace(oneLine))
            {
                foreach (KeyValuePair<string, Type> validIdentPrefix in PortManager.ValidIdentPrefixes)
                {
                    if (validIdentPrefix.Key.Length <= oneLine.Length && validIdentPrefix.Key.Equals(oneLine.Substring(0, validIdentPrefix.Key.Length)))
                        return new PortManager.ValidationResults()
                        {
                            Identity = oneLine,
                            MeterClassType = validIdentPrefix.Value,
                            BaudRate = 115200
                        };
                }
            }
            throw new DeviceNotValidException(oneLine);
        }

        public PortManager.ValidationResults ProbeUnqualifiedDevice(ComPortInfo port)
        {
            port.BaudRate = 0;
            foreach (int baudRate in this.BaudRates)
            {
                Channel_SerialPort channelSerialPort = (Channel_SerialPort)null;
                try
                {
                    PortManager.Trace_Probe("Probing {0}, {1} /////////////////////////////////", (object)port.PortName, (object)baudRate);
                    channelSerialPort = new Channel_SerialPort(port.PortName, baudRate);
                    PortManager.ValidationResults validationResults = PortManager.ValidateDevice((Channel)channelSerialPort);
                    validationResults.BaudRate = baudRate;
                    PortManager.Trace_Probe("Validated {0}: {1}, {2}", (object)port.PortName, (object)validationResults.MeterClassType.Name, (object)validationResults.Identity);
                    return validationResults;
                }
                catch (UnauthorizedAccessException ex)
                {
                    PortManager.Trace_Probe("UnauthorizedAccess {0}", (object)ex.Message);
                }
                catch (Exception ex)
                {
                    PortManager.Trace_Probe("Exception {0}", (object)ex.Message);
                }
                finally
                {
                    channelSerialPort?.Close();
                }
            }
            PortManager.Trace_Probe("Invalid {0}", (object)port.PortName);
            throw new DeviceNotValidException(port.PortName);
        }

        public Channel CreateOpenChannel(string portName)
        {
            try
            {
                return this.CreateOpenChannel(this.IndexByPortName[portName]);
            }
            catch (KeyNotFoundException ex)
            {
                throw new DeviceOpenException(new Exception("Unknown port name " + portName));
            }
        }

        public Channel CreateOpenChannel(ComPortInfo comPortInfo)
        {
            if (comPortInfo.IsQualified)
                return (Channel)new Channel_SerialPort(comPortInfo.PortName);
            PortManager.ValidationResults validationResults = this.ProbeUnqualifiedDevice(comPortInfo);
            return (Channel)new Channel_SerialPort(comPortInfo.PortName, validationResults.BaudRate);
        }

        public static void Trace(string format, params object[] args)
        {
        }

        [Conditional("TRACE_PORT_NAMES")]
        public static void Trace_PortNames(string format, params object[] args)
        {
        }

        [Conditional("TRACE_PROBING")]
        public static void Trace_Probe(string format, params object[] args)
        {
        }

        [Conditional("TRACE_PROBE_ID")]
        public static void Trace_ProbeId(string format, params object[] args)
        {
        }

        public class ValidationResults
        {
            public string Identity;
            public Type MeterClassType;
            public int BaudRate;
        }
    }
}
