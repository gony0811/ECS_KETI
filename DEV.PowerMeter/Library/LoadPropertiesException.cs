using System;


namespace DEV.PowerMeter.Library
{
    public class LoadPropertiesException : Exception
    {
        public LoadPropertiesException(Exception inner)
          : base("Failure loading meter properties", inner)
        {
        }

        public LoadPropertiesException(string message)
          : base(message)
        {
        }
    }
}
