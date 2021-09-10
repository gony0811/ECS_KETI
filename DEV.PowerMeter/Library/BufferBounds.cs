using SharedLibrary;
using System;
using System.Runtime.Serialization;

namespace DEV.PowerMeter.Library
{
    [API(APICategory.Formatting, "Represents a subset of data in a CaptureBuffer. \r\nThe subset is bounded left and right by timestamps,\r\nand also by arbitrary upper and lower measurement values.")]
    [DataContract]
    public class BufferBounds : IEquatable<BufferBounds>
    {
        [API("Timestamp of first sample in this selection")]
        [DataMember]
        public DateTime First;
        [API("Timestamp of last sample in this selection")]
        [DataMember]
        public DateTime Last;
        [API("Measurement value of lower boundary in this selection")]
        [DataMember]
        public double Lower;
        [API("Measurement value of upper boundary in this selection")]
        [DataMember]
        public double Upper;

        [API("Width between Left and Right bounds")]
        public TimeSpan Width => this.Last - this.First;

        [API("Height between Top and Bottom bounds")]
        public double Height => this.Upper - this.Lower;

        [API("True iff bounds Width or Height are non-zero.")]
        public bool IsInitialized => this.Width != TimeSpan.Zero || this.Height != 0.0;

        [API("IEquatablefor BufferPositions")]
        public bool Equals(BufferBounds other) => other != null && this.First == other.First && this.Last == other.Last && this.Upper == other.Upper && this.Lower == other.Lower;

        [API(APICategory.Unclassified)]
        public BufferBounds()
        {
        }

        [API(APICategory.Unclassified)]
        public BufferBounds(DateTime first, DateTime last)
        {
            this.First = first;
            this.Last = last;
        }

        [API(APICategory.Unclassified)]
        public BufferBounds(CaptureBuffer buffer)
          : this(buffer, 0U, buffer.Count - 1U)
        {
        }

        [API(APICategory.Unclassified)]
        public BufferBounds(CaptureBuffer buffer, uint firstIndex, uint lastIndex)
          : this(buffer[firstIndex].Timestamp, buffer[lastIndex].Timestamp)
        {
        }

        [API("Reset all properties.")]
        public void Clear()
        {
            this.First = this.Last = new DateTime();
            this.Upper = this.Lower = 0.0;
        }

        [API("Overwrite values of all properties")]
        public void Set(DateTime left, DateTime right, double bottom, double top)
        {
            this.First = left;
            this.Last = right;
            this.Upper = top;
            this.Lower = bottom;
        }

        [API("Overwrite values of all properties")]
        public void Set(BufferBounds other)
        {
            if (other == null)
                return;
            this.First = other.First;
            this.Last = other.Last;
            this.Upper = other.Upper;
            this.Lower = other.Lower;
        }

        [API(APICategory.Unclassified)]
        public string Settings
        {
            get => !this.IsInitialized ? "" : SharedLibrary.Serialization.Serialize((object)this);
            set
            {
                try
                {
                    this.DeSerialize(value);
                }
                catch (Exception ex)
                {
                }
            }
        }

        public override string ToString() => string.Format("{0}, {1}, {2}, {3}", (object)this.First.ToStringMicrosec(), (object)this.Last.ToStringMicrosec(), (object)this.Upper, (object)this.Lower);
    }
}
