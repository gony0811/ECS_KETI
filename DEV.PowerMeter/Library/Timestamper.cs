using SharedLibrary;
using System;

namespace DEV.PowerMeter.Library
{
    public static class Timestamper
    {
        public const int FinalEnergyIncrement = 270;
        public const int TicksPerMillisec = 10000;
        public const int TicksPerMicrosec = 10;
        public const int LowSpeedPeriod_Ticks = 1000000;
        public const int HighSpeedPeriod_Ticks = 500;
        public const int SnapshotPeriod_Ticks = 16;
        public const int DefaultPeriod_Ticks = 1000000;
        public const int SecondsPerMinute = 60;
        public const int PulseIdPeriod_Ticks = 1;
        public const double BaseClockRate = 40000000.0;
        public const double Decimation = 64.0;
        public const double SnapshotSampleRateMax = 625000.0;
        public const double SnapshotSamplePeriod = 1.6E-06;
        public const double SnapshotSamplePeriod_uS = 1.6;

        [API(APICategory.Unclassified)]
        public static void SetSamplePeriod(CaptureBuffer buffer) => Timestamper.SetSamplePeriod(buffer.Header.Units.IsEnergy, buffer.SamplePeriod);

        [API(APICategory.Unclassified)]
        public static void SetSamplePeriod(IDaqMeter meter)
        {
            if (meter.SnapshotMode_IsSelected)
                Timestamper.SetSamplePeriodSnapshot();
            else
                Timestamper.SetSamplePeriod(meter.Sensor_IsPyro, meter.SamplePeriod);
        }

        [API(APICategory.Unclassified)]
        public static void SetSamplePeriod(bool energyMode, long samplePeriod)
        {
            Timestamper.SamplePeriod_Ticks = energyMode ? 1L : samplePeriod;
            TimestampConverter.EnergyMode = energyMode;
            TimestampConverter.SetSamplePeriod(Timestamper.SamplePeriod_Ticks);
            Timestamper.SnapshotModeActive = false;
        }

        [API(APICategory.Unclassified)]
        public static void SetSamplePeriod(double Hertz)
        {
            Timestamper.SamplePeriod_Ticks = (long)(10000000.0 / Hertz);
            Timestamper.SnapshotModeActive = false;
        }

        public static void SetSamplePeriodSnapshot()
        {
            Timestamper.SetSamplePeriod(false, 16L);
            Timestamper.SnapshotModeActive = true;
        }

        public static bool SnapshotModeActive { get; set; }

        [API(APICategory.Unclassified)]
        public static long SamplePeriod_Ticks { get; set; }

        [API(APICategory.Unclassified)]
        public static DateTime Next { get; private set; }

        static Timestamper()
        {
            Timestamper.Next = new DateTime();
            Timestamper.SamplePeriod_Ticks = 1000000L;
        }

        [API(APICategory.Unclassified)]
        public static void Clear() => Timestamper.Next = new DateTime();

        public static void AssignNewTimestamp(DataRecordSingle item)
        {
            if ((DataRecordBase.SelectedDataFields & DataFieldFlags.Sequence) == (DataFieldFlags)0)
            {
                if ((item.Flags & MeasurementFlags.FinalEnergyRecord) != (MeasurementFlags)0)
                    item.Timestamp = Timestamper.FinalEnergyTimestamp();
                else
                    item.Timestamp = Timestamper.NextSequentialTimestamp();
            }
            else
                item.Timestamp = Timestamper.TimestampFromSequence(item.Sequence);
        }

        [API(APICategory.Unclassified)]
        public static DateTime NextSequentialTimestamp()
        {
            DateTime next = Timestamper.Next;
            Timestamper.Next = Timestamper.Next.AddTicks(Timestamper.SamplePeriod_Ticks);
            return next;
        }

        [API(APICategory.Unclassified)]
        public static DateTime TimestampFromSequence(ulong sequence) => !Timestamper.SnapshotModeActive ? new DateTime((long)sequence * 10L) : new DateTime((long)sequence * 16L);

        [API(APICategory.Unclassified)]
        public static DateTime FinalEnergyTimestamp() => Timestamper.Next.AddTicks((Timestamper.Next.Ticks > Timestamper.SamplePeriod_Ticks ? -Timestamper.SamplePeriod_Ticks : 0L) + 2700L);
    }
}
