using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS.Recipe.Model
{
    [Serializable]
    public class RECIPE
    {

        public RECIPE()
        {
            STEP_LIST = new List<RECIPE_STEP>();
        }

        public string RECIPE_NAME { get; set; }
        public int? STEP_COUNT { get; set; }
        public string EDITOR { get; set; }

        public DateTime? CREATETIME { get; set; }
        public DateTime? CHANGETIME { get; set; }
        public bool USE { get; set; }
        public bool? SELECTED { get; set; }
        public string RECIPEFILEPATH { get; set; }
        public string DESCRIPTION { get; set; }
        public List<RECIPE_STEP> STEP_LIST { get; set; }
    }
}
