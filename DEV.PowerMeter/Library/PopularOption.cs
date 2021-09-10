
using SharedLibrary;
using System.Runtime.Serialization;

namespace DEV.PowerMeter.Library
{
    [DataContract]
    public class PopularOption : NamedValue<LevelCombination>
    {
        public static readonly PopularOption CustomOption = (PopularOption)new CustomPopularOption();

        public PopularOption(LevelCombination value)
          : base(value, value.ToString())
        {
        }

        public bool Matches(PopularOption other) => other == this || this.Value.Matches(other.Value);
    }
}
