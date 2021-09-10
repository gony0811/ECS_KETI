
using System;

namespace DEV.PowerMeter.Library
{
    public abstract class DAQ_Thread_Ascii : DAQ_Thread
    {
        public DAQ_Thread_Ascii(
          DAQ_StateChanged daq_StateChanged,
          IDaqMeter meter,
          IDaqDevice device,
          CaptureBuffer captureBuffer)
          : base(daq_StateChanged, meter, device, captureBuffer)
        {
        }

        protected bool ReadOneRecord(DataRecordSingle Record)
        {
            this.SampleTime.Start();
            while (!this.IsStopping)
            {
                try
                {
                    string line = this.Channel.ReadData();
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        this.ReportUnexpectedEOF();
                        return false;
                    }
                    if (line.StartsWith("*"))
                        line = line.Substring(1);
                    DataRecordSingle dataRecordSingle = this.Device.DecodeMeasurement(line);
                    Record.Set((IDataRecordSingle)dataRecordSingle);
                    this.ReportRecordRead(line.Length);
                    return true;
                }
                catch (TimeoutException ex)
                {
                    if (this.IsStopping)
                        return false;
                    if (!this.IsWaiting)
                    {
                        this.IsWaiting = true;
                        this.OnDAQ_StateChanged(DAQ_State.TriggerWait);
                    }
                    if (!this.OperatingMode_IsTrueEnergy)
                    {
                        if (this.SampleTime.ElapsedMilliseconds > (long)this.PowerModeMaxElapsed_ms)
                        {
                            this.ReportUnexpectedTimeout();
                            return false;
                        }
                    }
                }
                catch (UnexpectedEofException ex)
                {
                    this.ReportUnexpectedEOF();
                    return false;
                }
                finally
                {
                    this.SampleTime.Stop();
                }
            }
            return false;
        }
    }
}
