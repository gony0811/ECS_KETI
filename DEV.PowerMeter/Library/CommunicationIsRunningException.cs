
namespace DEV.PowerMeter.Library
{
    public class CommunicationIsRunningException : CommunicationException
    {
        public CommunicationIsRunningException(string message)
          : base(message)
        {
        }
    }
}
