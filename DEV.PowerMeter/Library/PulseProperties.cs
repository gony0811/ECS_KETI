using SharedLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;


namespace DEV.PowerMeter.Library
{
    [API(APICategory.Analysis)]
    public class PulseProperties : IShortFormat
    {
        public static readonly string[] ReportHeadings = new string[16]
        {
      nameof (Index),
      nameof (Width),
      "Height",
      nameof (Energy),
      "PeakPower",
      nameof (RiseTime),
      nameof (FallTime),
      nameof (RiseStartTime),
      nameof (MiddleStartTime),
      nameof (RiseStopTime),
      nameof (FallStartTime),
      nameof (MiddleStopTime),
      nameof (FallStopTime),
      "EnergyStartTime",
      "PeakTime",
      "EnergyStopTime"
        };
        public const int ShortFormatCount = 7;

        public int Index { get; set; }

        [API(APICategory.Unclassified)]
        public EnergyCalculator Energy { get; protected set; }

        public PulseProperties(PulseAnalysisOptions options)
        {
            this.StartThreshold = options.WidthStartThreshold;
            this.StopThreshold = options.WidthStopThreshold;
            this.Energy = new EnergyCalculator(options.EnergyBaselineLevel);
        }

        [API(APICategory.Unclassified)]
        public Level StartThreshold { get; protected set; }

        [API(APICategory.Unclassified)]
        public Level StopThreshold { get; protected set; }

        [API(APICategory.Unclassified)]
        public DateTime StartTime
        {
            get
            {
                switch (this.StartThreshold)
                {
                    case Level.Middle:
                        return this.MiddleStartTime;
                    case Level.Upper:
                        return this.RiseStopTime;
                    default:
                        return this.RiseStartTime;
                }
            }
        }

        [API(APICategory.Unclassified)]
        public DateTime StopTime
        {
            get
            {
                switch (this.StopThreshold)
                {
                    case Level.Middle:
                        return this.MiddleStopTime;
                    case Level.Upper:
                        return this.FallStartTime;
                    default:
                        return this.FallStopTime;
                }
            }
        }

        [API(APICategory.Unclassified)]
        public double Width => (this.StopTime - this.StartTime).TotalSeconds;

        [API(APICategory.Unclassified)]
        public DateTime RiseStartTime { get; set; }

        [API(APICategory.Unclassified)]
        public DateTime RiseStopTime { get; set; }

        [API(APICategory.Unclassified)]
        public double RiseTime => (this.RiseStopTime - this.RiseStartTime).TotalSeconds;

        [API(APICategory.Unclassified)]
        public DateTime FallStartTime { get; set; }

        [API(APICategory.Unclassified)]
        public DateTime FallStopTime { get; set; }

        [API(APICategory.Unclassified)]
        public double FallTime => (this.FallStopTime - this.FallStartTime).TotalSeconds;

        [API(APICategory.Unclassified)]
        public DateTime MiddleStartTime { get; set; }

        [API(APICategory.Unclassified)]
        public DateTime MiddleStopTime { get; set; }

        [API(APICategory.Unclassified)]
        public double MiddleWidth => (this.MiddleStopTime - this.MiddleStartTime).TotalSeconds;

        [API(APICategory.Unclassified)]
        public bool Contains(DateTime dateTime) => this.RiseStartTime <= dateTime && dateTime <= this.FallStopTime;

        public string[] Format()
        {
            List<string> source = new List<string>();
            source.AddRange((IEnumerable<string>)this.ShortFormat());
            source.Add(string.Format("{0}", (object)this.RiseStartTime.ToStringMicrosec()));
            source.Add(string.Format("{0}", (object)this.MiddleStartTime.ToStringMicrosec()));
            source.Add(string.Format("{0}", (object)this.RiseStopTime.ToStringMicrosec()));
            source.Add(string.Format("{0}", (object)this.FallStartTime.ToStringMicrosec()));
            source.Add(string.Format("{0}", (object)this.MiddleStopTime.ToStringMicrosec()));
            source.Add(string.Format("{0}", (object)this.FallStopTime.ToStringMicrosec()));
            source.Add(string.Format("{0}", (object)this.Energy.StartTime.ToStringMicrosec()));
            source.Add(string.Format("{0}", (object)this.Energy.PeakPowerTime.ToStringMicrosec()));
            source.Add(string.Format("{0}", (object)this.Energy.StopTime.ToStringMicrosec()));
            return source.ToArray<string>();
        }

        public string[] ShortFormat() => new string[7]
        {
      string.Format("{0}", (object) this.Index),
      string.Format("{0:e}", (object) this.Width),
      string.Format("{0:e}", (object) this.Energy.Height),
      string.Format("{0:e}", (object) this.Energy.Total),
      string.Format("{0:e}", (object) this.Energy.PeakPower),
      string.Format("{0:e}", (object) this.RiseTime),
      string.Format("{0:e}", (object) this.FallTime)
        };

        [API(APICategory.Unclassified)]
        public void WriteReport(TextWriter writer) => writer.WriteLine(string.Join(PulseAnalysisOptions.Separator, this.Format()));

        [API(APICategory.Unclassified)]
        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            string[] strArray = this.Format();
            stringBuilder.AppendFormat("Pulse {0}:\n", (object)this.Index);
            for (int index = 1; index < strArray.Length; ++index)
                stringBuilder.AppendFormat("\t{0}: {1}\n", (object)PulseProperties.ReportHeadings[index], (object)strArray[index]);
            return stringBuilder.ToString();
        }
    }
}
