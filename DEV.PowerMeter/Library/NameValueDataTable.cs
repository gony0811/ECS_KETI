
using System.Data;


namespace DEV.PowerMeter.Library
{
    public class NameValueDataTable : DataTable
    {
        public NameValueDataTable()
          : base("Analysis Summary")
        {
            this.Columns.Add("Property");
            this.Columns.Add("Value");
        }

        public void Add(string name, string value)
        {
            DataRow row = this.NewRow();
            row[0] = (object)name;
            row[1] = (object)value;
            this.Rows.Add(row);
        }

        public new void Clear()
        {
            foreach (DataRow row in (InternalDataCollectionBase)this.Rows)
                row[1] = (object)"";
        }
    }
}
