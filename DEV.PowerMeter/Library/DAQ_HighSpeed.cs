using System;

namespace DEV.PowerMeter.Library
{
    public class DAQ_HighSpeed : DAQ_Thread
    {
        public DAQ_HighSpeed(
          DAQ_StateChanged daq_StateChanged,
          IDaqMeter meter,
          IDaqDevice device,
          CaptureBuffer captureBuffer)
          : base(daq_StateChanged, meter, device, captureBuffer)
        {
        }

        protected override void ThreadBody()
        {
            try
            {
                this.OnDAQ_StateChanged(DAQ_State.Start);
                while (!this.IsStopping)
                {
                    if (this.ReadOneRecord(this.Data))
                    {
                        if (this.IsWaiting)
                        {
                            this.IsWaiting = false;
                            this.OnDAQ_StateChanged(DAQ_State.Triggered);
                        }
                        this.Record.Read(this.Data);
                        if (!this.TerminatedByMeter(this.Record))
                        {
                            this.CaptureBuffer.TimestampAndAdd(this.Record);
                            ++this.Count;
                            if (this.StopOnCount)
                            {
                                if ((long)this.Count >= (long)this.Capacity)
                                    break;
                            }
                        }
                        else
                            break;
                    }
                    else
                        break;
                }
            }
            catch (Exception ex)
            {
                this.ReportException(ex);
            }
            this.OnThreadExits();
        }

        protected virtual bool ReadOneRecord(byte[] data)
        {
            int length = data.Length;
            int offset = 0;
            this.SampleTime.Start();
            while (length > 0)
            {
                if (!this.IsStopping)
                {
                    try
                    {
                        while (length > 0 && !this.IsStopping)
                        {
                            int num = this.Channel.Read(data, offset, length);
                            if (num <= 0)
                            {
                                this.ReportUnexpectedEOF();
                                return false;
                            }
                            length -= num;
                            offset += num;
                        }
                        this.BytesRead += data.Length;
                        ++this.RecordsRead;
                    }
                    catch (TimeoutException ex)
                    {
                        if (this.IsStopping)
                            return false;
                        if (!this.OperatingMode_IsTrueEnergy && this.SampleTime.ElapsedMilliseconds > (long)this.PowerModeMaxElapsed_ms)
                        {
                            this.ReportUnexpectedTimeout();
                            return false;
                        }
                        if (!this.IsWaiting)
                        {
                            this.IsWaiting = true;
                            this.OnDAQ_StateChanged(DAQ_State.TriggerWait);
                        }
                    }
                    finally
                    {
                        this.SampleTime.Stop();
                    }
                }
                else
                    break;
            }
            return length == 0;
        }
    }
}
