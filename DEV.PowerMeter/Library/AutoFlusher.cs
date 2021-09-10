using System;

namespace DEV.PowerMeter.Library
{
    public class AutoFlusher
    {
        public const double DefaultElapsedPerFlush_mS = 10000.0;
        public double ElapsedPerFlush_mS = 10000.0;
        public DateTime PreviousFlushTimestamp;

        public void Restart() => this.PreviousFlushTimestamp = new DateTime();

        public bool ShouldDoFlush(DateTime timestamp)
        {
            int num = (timestamp - this.PreviousFlushTimestamp).TotalMilliseconds >= this.ElapsedPerFlush_mS ? 1 : 0;
            if (num == 0)
                return num != 0;
            this.PreviousFlushTimestamp = timestamp;
            return num != 0;
        }
    }
}
