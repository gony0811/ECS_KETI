
using System.Runtime.Serialization;

namespace DEV.PowerMeter.Library
{
    [DataContract]
    public class CustomPopularOption : PopularOption
    {
        public CustomPopularOption()
          : base((LevelCombination)new CustomLevelCombination())
        {
        }
    }
}
