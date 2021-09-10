using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ECS.Recipe.Model
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class RECIPE_STEP
    {
        public int STEP_ID { get; set; }
        public double X_POSITION { get; set; }
        public double Y_POSITION { get; set; }
        public double ENERGY { get; set; }
        public double HV { get; set; }
        public string EGY_MODE { get; set; }
        public int PROCESS_TIME { get; set; }
    }
}
