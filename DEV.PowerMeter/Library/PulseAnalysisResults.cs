using SharedLibrary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace DEV.PowerMeter.Library
{
    [API(APICategory.Analysis, "PulseAnalysis results are stored in this object.")]
    public class PulseAnalysisResults : IEnumerable<IShortFormat>, IEnumerable
    {
        [API("The arithmetic mean of selected pulse properties.")]
        public PulseAverager Mean;

        [API("the number of pulses detected")]
        public int Count => this.Pulses == null ? 0 : this.Pulses.Count;

        [API("A list of the properties for all the pulses detected.")]
        public List<PulseProperties> Pulses { get; protected set; }

        [API("Return the detected pulse that straddles a given timestamp, or null if none match.")]
        public PulseProperties FindPulse(DateTime dateTime) => this.Pulses.FirstOrDefault<PulseProperties>((Func<PulseProperties, bool>)(pp => pp.Contains(dateTime)));

        public PulseAnalysisResults()
        {
            this.Pulses = new List<PulseProperties>();
            this.Mean = new PulseAverager();
        }

        public void Clear()
        {
            this.Pulses.Clear();
            this.Mean.Clear();
        }

        public IEnumerator<IShortFormat> GetEnumerator()
        {
            foreach (IShortFormat pulse in this.Pulses)
                yield return pulse;
            yield return (IShortFormat)this.Mean;
        }

        IEnumerator IEnumerable.GetEnumerator() => (IEnumerator)this.GetEnumerator();

        public DataView ResultsDataView => this.ResultsTable.AsDataView();

        public DataView OverviewDataView => this.OverviewTable.AsDataView();

        public ResultsDataTable ResultsTable
        {
            get
            {
                ResultsDataTable resultsDataTable = new ResultsDataTable();
                if (this.Count > 0)
                    resultsDataTable.AddRowRange((IEnumerable<IShortFormat>)this);
                else
                    resultsDataTable.AddRow("[No pulses found]");
                return resultsDataTable;
            }
        }

        public NameValueDataTable OverviewTable
        {
            get
            {
                NameValueDataTable nameValueDataTable = new NameValueDataTable();
                nameValueDataTable.Add("Pulses Detected", string.Format("{0:d}", (object)this.Count));
                nameValueDataTable.Add("Overall Duration", string.Format("{0:e}", (object)this.Duration_Seconds));
                nameValueDataTable.Add("Overall Duration Start", string.Format("{0:e}", (object)this.Duration_Start_Seconds));
                nameValueDataTable.Add("Overall Duration Stop", string.Format("{0:e}", (object)this.Duration_Stop_Seconds));
                return nameValueDataTable;
            }
        }

        public double Duration_Seconds => this.Count <= 0 ? 0.0 : this.Duration.TotalSeconds;

        public double Duration_Start_Seconds => this.Count <= 0 ? 0.0 : this.Duration_Start.ToSeconds();

        public double Duration_Stop_Seconds => this.Count <= 0 ? 0.0 : this.Duration_Stop.ToSeconds();

        public DateTime Duration_Start => this.Pulses[0].StartTime;

        public DateTime Duration_Stop => this.Pulses[this.Count - 1].StopTime;

        public TimeSpan Duration => this.Duration_Stop - this.Duration_Start;

        public void Add(PulseProperties pending)
        {
            pending.Index = this.Pulses.Count + 1;
            this.Pulses.Add(pending);
            this.Mean.Add(pending);
        }

        public void WriteReport(string filename)
        {
            using (TextWriter textUtF8 = (TextWriter)FileX.CreateTextUTF8(filename))
                this.WriteReport(textUtF8);
        }

        public void WriteReport(TextWriter writer)
        {
            writer.WriteLine("Pulse Count:{0}{1}", (object)PulseAnalysisOptions.Separator, (object)this.Count);
            writer.WriteLine("Overall Duration: {0:e}", (object)this.Duration_Seconds);
            writer.WriteLine("Overall Duration Start: {0:e}", (object)this.Duration_Start_Seconds);
            writer.WriteLine("Overall Duration Stop: {0:e}", (object)this.Duration_Stop_Seconds);
            writer.WriteLine();
            if (this.Count <= 0)
            {
                writer.WriteLine("[No Pulses]");
            }
            else
            {
                writer.WriteLine("Measurement{0}Pulses...", (object)PulseAnalysisOptions.Separator);
                writer.WriteLine(string.Join(PulseAnalysisOptions.Separator, PulseProperties.ReportHeadings));
                foreach (PulseProperties pulse in this.Pulses)
                    pulse.WriteReport(writer);
                this.Mean.WriteReport(writer);
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("PulseAnalysisResults: ");
            sb.AppendFormat("\n\tPulse Count {0}\n", (object)this.Count);
            sb.AppendFormat("\tOverall Duration {0:e}\n", (object)this.Duration_Seconds);
            this.Mean.AddFields(sb);
            return sb.ToString();
        }
    }
}
