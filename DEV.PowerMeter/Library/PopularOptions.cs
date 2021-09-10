using System.Collections.Generic;


namespace DEV.PowerMeter.Library
{
    public class PopularOptions : List<PopularOption>
    {
        public PopularOptions()
        {
        }

        public PopularOptions(IEnumerable<PopularOption> options)
          : base(options)
        {
        }

        public PopularOption Search(double lower, double middle, double upper) => this.Search(new LevelCombination(lower, middle, upper));

        public PopularOption Search(LevelCombination levels)
        {
            foreach (PopularOption popularOption in (List<PopularOption>)this)
            {
                if (levels.Matches(popularOption.Value))
                    return popularOption;
            }
            return PopularOption.CustomOption;
        }
    }
}
