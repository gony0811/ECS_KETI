using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEV.PowerMeter.Library
{
    public interface ISequenceIds
    {
        bool UploadSequenceIDs { get; set; }

        bool CanChangeUploadSequenceIDs { get; }
    }
}
