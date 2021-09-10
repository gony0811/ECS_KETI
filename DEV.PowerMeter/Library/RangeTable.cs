using SharedLibrary;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DEV.PowerMeter.Library
{
    [API(APICategory.Measurement, "Range Table contains a table of all the ranges for a given Sensor, modified by certain operating mode settings.")]
    public class RangeTable : IEnumerable, IEnumerable<double>
    {
        [API("The \"Auto\" setting is represented by 0, which otherwise is an impossible range setting.")]
        public const double AutoRangeValue = 0.0;

        [API("The list of current numeric ranges.")]
        public List<double> Ranges { get; protected set; }

        [API("Access ranges by numerical index.")]
        public double this[int index] => this.Ranges[index];

        [API("Maximum range setting.")]
        public double Maximum => this.Ranges.Max<double>();

        [API("Minimum range setting.")]
        public double Minimum => this.Ranges.Min<double>();

        [API("Constuctor.")]
        public RangeTable()
        {
            this.Ranges = new List<double>();
            this.Ranges.Add(0.0);
        }

        [API("Construct and initialize from a list.")]
        public RangeTable(List<double> ranges)
        {
            this.Ranges = new List<double>();
            foreach (double range in ranges)
                this.Ranges.Add(range);
        }

        [API("Show list of comma separated range values.")]
        public override string ToString() => this.Ranges.CommaSeparated();

        [API("Equality Comparison of two RangeTables.")]
        public override bool Equals(object obj) => obj is RangeTable rangeTable && this.Equals(rangeTable.Ranges);

        [API("Equality Comparison of this RangeTable to an arbitrary list of range values.")]
        public bool Equals(List<double> ranges)
        {
            if (ranges == null || ranges.Count != this.Ranges.Count)
                return false;
            for (int index = 0; index < this.Ranges.Count; ++index)
            {
                if (this.Ranges[index] != ranges[index])
                    return false;
            }
            return true;
        }

        public override int GetHashCode() => base.GetHashCode();

        [API("Enumerate table contents")]
        IEnumerator IEnumerable.GetEnumerator() => (IEnumerator)this.GetEnumerator();

        [API("Enumerate table contents")]
        public IEnumerator<double> GetEnumerator() => (IEnumerator<double>)this.Ranges.GetEnumerator();
    }
}
