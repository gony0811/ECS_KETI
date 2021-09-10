using SharedLibrary;
using System.IO;
using System.Text;

namespace DEV.PowerMeter.Library
{
    [API(APICategory.Analysis)]
    public class PulseAverager : IShortFormat
    {
        [API(APICategory.Unclassified)]
        public int Count;

        [API(APICategory.Unclassified)]
        public double Width => this.Mean(this.TotalWidth);

        [API(APICategory.Unclassified)]
        public double Height => this.Mean(this.TotalHeight);

        [API(APICategory.Unclassified)]
        public double Energy => this.Mean(this.TotalEnergy);

        [API(APICategory.Unclassified)]
        public double PeakPower => this.Mean(this.TotalPeakPower);

        [API(APICategory.Unclassified)]
        public double AveragePower => this.Mean(this.TotalAveragePower);

        [API(APICategory.Unclassified)]
        public double RiseTime => this.Mean(this.TotalRiseTime);

        [API(APICategory.Unclassified)]
        public double FallTime => this.Mean(this.TotalFallTime);

        private double Mean(double sum) => this.Count <= 0 ? 0.0 : sum / (double)this.Count;

        [API(APICategory.Unclassified)]
        public double TotalWidth { get; protected set; }

        [API(APICategory.Unclassified)]
        public double TotalHeight { get; protected set; }

        [API(APICategory.Unclassified)]
        public double TotalEnergy { get; protected set; }

        [API(APICategory.Unclassified)]
        public double TotalPeakPower { get; protected set; }

        [API(APICategory.Unclassified)]
        public double TotalAveragePower { get; protected set; }

        [API(APICategory.Unclassified)]
        public double TotalRiseTime { get; protected set; }

        [API(APICategory.Unclassified)]
        public double TotalFallTime { get; protected set; }

        public PulseAverager() => this.Clear();

        [API(APICategory.Unclassified)]
        public void Clear()
        {
            this.Count = 0;
            this.TotalWidth = this.TotalHeight = this.TotalEnergy = this.TotalPeakPower = this.TotalAveragePower = this.TotalRiseTime = this.TotalFallTime = 0.0;
        }

        [API(APICategory.Unclassified)]
        public void Add(PulseProperties pulse)
        {
            ++this.Count;
            this.TotalWidth += pulse.Width;
            this.TotalHeight += pulse.Energy.Height;
            this.TotalEnergy += pulse.Energy.Total;
            this.TotalPeakPower += pulse.Energy.PeakPower;
            this.TotalAveragePower += pulse.Energy.AveragePower;
            this.TotalRiseTime += pulse.RiseTime;
            this.TotalFallTime += pulse.FallTime;
        }

        public string[] ShortFormat() => new string[7]
        {
      string.Format("Mean"),
      string.Format("{0:e}", (object) this.Width),
      string.Format("{0:e}", (object) this.Height),
      string.Format("{0:e}", (object) this.Energy),
      string.Format("{0:e}", (object) this.PeakPower),
      string.Format("{0:e}", (object) this.RiseTime),
      string.Format("{0:e}", (object) this.FallTime)
        };

        public void WriteReport(TextWriter writer) => writer.WriteLine(string.Join(PulseAnalysisOptions.Separator, this.ShortFormat()));

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("PulseAverager:\n");
            this.AddFields(sb);
            return sb.ToString();
        }

        public void AddFields(StringBuilder sb)
        {
            sb.AppendFormat("\tMean Width {0:e}\n", (object)this.Width);
            sb.AppendFormat("\tMean Height {0:e}\n", (object)this.Height);
            sb.AppendFormat("\tMean Energy {0:e}\n", (object)this.Energy);
            sb.AppendFormat("\tMean PeakPower {0:e}\n", (object)this.PeakPower);
            sb.AppendFormat("\tMean RiseTime {0:e}\n", (object)this.RiseTime);
            sb.AppendFormat("\tMean FallTime {0:e}\n", (object)this.FallTime);
        }
    }
}
