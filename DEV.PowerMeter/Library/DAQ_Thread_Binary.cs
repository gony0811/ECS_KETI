using System;

namespace DEV.PowerMeter.Library
{
    public abstract class DAQ_Thread_Binary : DAQ_Thread
    {
        public DAQ_Thread_Binary(
          DAQ_StateChanged daq_StateChanged,
          IDaqMeter meter,
          IDaqDevice device,
          CaptureBuffer captureBuffer)
          : base(daq_StateChanged, meter, device, captureBuffer)
        {
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
                                break;
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
                        this.ReportUnexpectedTimeout();
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
