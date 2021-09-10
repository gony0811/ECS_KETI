
using System.Runtime.Serialization;

namespace DEV.PowerMeter.Library
{
    [DataContract]
    public class CustomLevelCombination : LevelCombination
    {
        public override string ToString() => "[ Select Popular Combination ]";
    }
}
