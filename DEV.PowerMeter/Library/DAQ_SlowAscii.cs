using System;


namespace DEV.PowerMeter.Library
{
    public class DAQ_SlowAscii : DAQ_Thread_Ascii
    {
        public DAQ_SlowAscii(
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
                    if (this.ReadOneRecord(this.Record))
                    {
                        if (this.IsWaiting)
                        {
                            this.IsWaiting = false;
                            this.OnDAQ_StateChanged(DAQ_State.Triggered);
                        }
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
    }
}
