

namespace DEV.PowerMeter.Library
{
    public interface IDaqDevice : IDecodeMeasurement
    {
        Channel Channel { get; }

        void Start(uint count);

        void Stop();

        void Close();

        bool IsOpen { get; }

        void ForceTrigger();
    }
}
