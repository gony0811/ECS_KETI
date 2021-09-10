using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Basler.Pylon;


namespace ECS.UI.Model
{
    public class VisionCamera
    { 
        public string CameraName { get; set; }
        public object CameraInfo { get; set; }
        public string Description { get; set; }

        public VisionCamera(string camera_name)
        {
            CameraName = camera_name;
        }
    }
}
