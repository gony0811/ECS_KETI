using System.Runtime.Serialization;

namespace DEV.PowerMeter.Library
{
    [DataContract]
    public class LevelCombination
    {
        [DataMember]
        public double Lower { get; protected set; }

        [DataMember]
        public double Middle { get; protected set; }

        [DataMember]
        public double Upper { get; protected set; }

        protected LevelCombination()
        {
        }

        public LevelCombination(double lower, double middle, double upper)
        {
            this.Lower = lower;
            this.Middle = middle;
            this.Upper = upper;
        }

        public bool Matches(LevelCombination other) => other != null && this.Lower == other.Lower && this.Middle == other.Middle && this.Upper == other.Upper;

        public static string Percent(double ratio) => ratio.ToString("P1");

        public override string ToString() => LevelCombination.Percent(this.Lower) + " / " + LevelCombination.Percent(this.Middle) + " / " + LevelCombination.Percent(this.Upper);
    }
}
