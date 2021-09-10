using System.Globalization;

namespace DEV.PowerMeter.Library
{
    public class DataRecordAccumulatorQuad :
       DataRecordAccumulator,
       IDataRecordQuad,
       IDataRecordSingle,
       IEncodable
    {
        public DataRecordAccumulatorQuad() => this.Data = (DataRecordSingle)new DataRecordQuad();

        private DataRecordQuad Quad => this.Data as DataRecordQuad;

        public override void Clear()
        {
            base.Clear();
            this.Quad.X = this.Quad.Y = 0.0;
        }

        public double X => this.Quad.X / (double)this.Count;

        public double Y => this.Quad.Y / (double)this.Count;

        public override void Add(IDataRecordSingle data)
        {
            base.Add(data);
            if (!(data is DataRecordQuad dataRecordQuad))
                return;
            this.Quad.X += dataRecordQuad.X;
            this.Quad.Y += dataRecordQuad.Y;
        }

        public override string[] Encode(CultureInfo culture) => new DataRecordQuad((IDataRecordQuad)this).Encode(culture);

        public override string ToString() => string.Format("{0} {1} {2} {3} {4} {5} {6} // DRA quad", (object)this.Timestamp.ToStringMicrosec(), (object)this.Measurement, (object)this.X, (object)this.Y, (object)this.Sequence, (object)this.Flags.ToString().Replace(",", "|"), (object)this.Period);
    }
}
