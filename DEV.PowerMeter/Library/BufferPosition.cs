using SharedLibrary;
using System;

namespace DEV.PowerMeter.Library
{
    [API(APICategory.Formatting, "Represents a possible Measurement in a CaptureBuffer. ")]
    public class BufferPosition : IEquatable<BufferPosition>
    {
        [API("Horizontal position in buffer (Timestamp/DateTime)")]
        public DateTime X;
        [API("Vertical position in buffer (Measurement value)")]
        public double Y;

        [API("IEquatablefor BufferPositions")]
        public bool Equals(BufferPosition other) => other != null && this.X == other.X && this.Y == other.Y;
    }
}
