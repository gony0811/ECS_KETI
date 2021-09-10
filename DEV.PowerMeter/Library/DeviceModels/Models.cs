
using System;
using System.Collections.Generic;

namespace DEV.PowerMeter.Library.DeviceModels
{
    public class Models
    {
        public static readonly Dictionary<string, Type> NameTypeMap = new Dictionary<string, Type>()
    {
      {
        "Meter",
        typeof (Meter)
      },
      {
        "Meter_ME",
        typeof (Meter_ME)
      },
      {
        "Meter_MP",
        typeof (Meter_MP)
      },
      {
        "Meter_SSIM",
        typeof (Meter_SSIM)
      },
      {
        "Device",
        typeof (Device)
      },
      {
        "Device_ME",
        typeof (Device_ME)
      },
      {
        "Device_MP",
        typeof (Device_MP)
      },
      {
        "Device_SSIM",
        typeof (Device_SSIM)
      }
    };
    }
}
