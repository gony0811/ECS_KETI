using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INNO6.IO
{
    public enum eDataType
    {
        Int,
        Double,
        String,
        Object,
    }

    public enum eDirection { IN, OUT, BOTH }

    public enum eInterlock { NONE, SETPOINT, SETVALUE }

    [Serializable()]
    public class Data
    {
        public string Name { get; set; }
        public string Module { get; set; }
        public string Group { get; set; }
        public eDataType Type { get; set; }
        public string DriverName { get; set; }
        public eDirection Direction { get; set; }
        public string Config1 { get; set; }
        public string Config2 { get; set; }
        public string Config3 { get; set; }
        public string Config4 { get; set; }
        public int PollingTime { get; set; }

        public int DataResetTimeout { get; set; }

        public int? DataSetTime { get; set; }
        public string Unit { get; set; }
        public double Format { get; set; }
        public bool Use { get; set; }

        public bool Logging { get; set; }

        public DateTime CheckTime { get; set; }
        public int Index { get; set; }
        public string Description { get; set; }
        public string DefaultValue { get; set; }
        //public eInterlock InterlockMode { get; set; }
        //public string InterlockName { get; set; }

        public object Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public string StringValue
        {
            get { return _value.ToString(); }
            set { _value = (object)value; }
        }

        public Data()
        {
            Value = 0;
        }

        public Data(string name, eDataType type, string driveName, eDirection direction, bool use = true)
        {
            Name = name;
            Type = type;
            Use = use;
            PollingTime = 0;
            DataResetTimeout = 0;

            if (type == eDataType.Int) Value = 0;
            else if (type == eDataType.Double) Value = 0.0;
            else Value = string.Empty;
        }

        public Data(Data data)
        {
            Name = data.Name;
            Module = data.Module;
            Group = data.Group;
            Type = data.Type;
            DriverName = data.DriverName;
            Direction = data.Direction;
            Config1 = data.Config1;
            Config2 = data.Config2;
            Config3 = data.Config3;
            Config4 = data.Config4;

            PollingTime = data.PollingTime;
            DataResetTimeout = data.DataResetTimeout;
            DataSetTime = data.DataSetTime;
            Unit = data.Unit;
            Format = data.Format;
            Use = data.Use;
            CheckTime = data.CheckTime;
            Index = data.Index;
            Description = data.Description;
            DefaultValue = data.DefaultValue;
            StringValue = data.StringValue;
            _value = data.Value;
        }

        private object _value;
    }
}
