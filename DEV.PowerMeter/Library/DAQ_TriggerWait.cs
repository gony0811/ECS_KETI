using System;

namespace DEV.PowerMeter.Library
{
    public class DAQ_TriggerWait : DAQ_Thread
    {
        public DAQ_TriggerWait(
          DAQ_StateChanged daq_StateChanged,
          IDaqMeter meter,
          IDaqDevice device,
          CaptureBuffer captureBuffer)
          : base(daq_StateChanged, meter, device, captureBuffer)
        {
        }

        protected override void ThreadBody()
        {
            uint num = this.Capacity - this.PreTrigger;
            try
            {
                this.OnDAQ_StateChanged(DAQ_State.Start);
                while (!this.IsStopping)
                {
                    if (this.ReadOneRecord(this.Data))
                    {
                        if ((this.Record.Flags & MeasurementFlags.TriggerDetected) != (MeasurementFlags)0 && this.IsWaiting)
                        {
                            this.IsWaiting = false;
                            this.OnDAQ_StateChanged(DAQ_State.Triggered);
                        }
                        this.Record.Read(this.Data);
                        if (!this.TerminatedByMeter(this.Record))
                        {
                            this.CaptureBuffer.TimestampAndAdd(this.Record);
                            if (!this.IsWaiting && this.StopOnCount)
                            {
                                --num;
                                if (num == 0U)
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

        protected bool ReadOneRecord(byte[] data)
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
