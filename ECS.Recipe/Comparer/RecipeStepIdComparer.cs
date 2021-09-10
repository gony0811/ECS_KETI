using ECS.Recipe.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS.Recipe.Comparer
{
    public class RecipeStepIdComparer : IComparer<RECIPE_STEP>
    {
        public int Compare(RECIPE_STEP x, RECIPE_STEP y)
        {
            return x.STEP_ID - y.STEP_ID;
        }
    }
}
