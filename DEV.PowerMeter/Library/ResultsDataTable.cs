using System.Collections.Generic;
using System.Data;

namespace DEV.PowerMeter.Library
{
    public class ResultsDataTable : DataTable
    {
        public ResultsDataTable()
          : base("Analysis Results")
        {
            for (int index = 0; index < 7; ++index)
                this.Columns.Add(PulseProperties.ReportHeadings[index]);
        }

        public void AddRow(string[] items)
        {
            DataRow row = this.NewRow();
            for (int columnIndex = 0; columnIndex < 7; ++columnIndex)
                row[columnIndex] = (object)items[columnIndex];
            this.Rows.Add(row);
        }

        public void AddRow(string message)
        {
            DataRow row = this.NewRow();
            row[0] = (object)message;
            this.Rows.Add(row);
        }

        public void AddRow(IShortFormat source) => this.AddRow(source.ShortFormat());

        public void AddRowRange(IEnumerable<IShortFormat> source)
        {
            foreach (IShortFormat source1 in source)
                this.AddRow(source1);
        }
    }
}
