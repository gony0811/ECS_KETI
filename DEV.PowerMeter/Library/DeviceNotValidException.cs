using System;


namespace DEV.PowerMeter.Library
{
    public class DeviceNotValidException : Exception
    {
        public string Identity { get; protected set; }

        public DeviceNotValidException(string Identity)
          : this(Identity, (Exception)null)
        {
        }

        public DeviceNotValidException(string Identity, Exception inner)
          : base("Unsupported device: " + Identity, inner)
        {
            this.Identity = Identity;
        }
    }
}
