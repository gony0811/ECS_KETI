
using DEV.PowerMeter.Library.ImportExport;
using SharedLibrary;
using System.Collections.Generic;
using System.Diagnostics;

namespace DEV.PowerMeter.Library
{
    [API(APICategory.Measurement, "CaptureBufferUnbounded is a concrete class, which can hold arbitrarily large amounts of data and it's capacity can dynamically grow, bounded only by the overall memory capacity of the app. However, its access time degrades as the capacity increases, making it unacceptable for high-speed data acquisition. It's used for importing data from an external file,and other measurement storage not related to high-speed acquisition.")]
    public class CaptureBufferUnbounded : CaptureBuffer
    {
        [API("The data, stored in list form.")]
        public List<DataRecordSingle> DataBuffer { get; protected set; }

        public override uint Count => (uint)this.DataBuffer.Count;

        public override bool IsPreview => true;

        public override IEnumerator<DataRecordSingle> GetEnumerator() => (IEnumerator<DataRecordSingle>)this.DataBuffer.GetEnumerator();

        [API("Create empty Unbounded CaptureBuffer with a default Header, generally for importing a file.")]
        public CaptureBufferUnbounded()
          : this(new Header())
        {
        }

        [API("Create empty Unbounded CaptureBuffer with a specific Header, generally for exporting a file (though data also may be exported directly from the main CaptureBuffer itself.")]
        public CaptureBufferUnbounded(Header header)
          : base(header)
        {
            this.SetData(new List<DataRecordSingle>());
        }

        private void SetData(List<DataRecordSingle> data) => this.DataBuffer = data;

        [API("Discard all captured data.")]
        public override void Clear()
        {
            base.Clear();
            this.DataBuffer.Clear();
        }

        public override DataRecordSingle this[int index]
        {
            get => this.DataBuffer[index];
            set => this.DataBuffer[index] = value;
        }

        protected override void Add(DataRecordSingle item) => this.DataBuffer.Add(item);

        [Conditional("TRACE_BUFFER")]
        public void TRACE1(string fmt, params object[] args)
        {
        }

        [Conditional("TRACE_BUFFER")]
        public void TRACE(string fmt, params object[] args)
        {
        }

        [Conditional("TRACE_ADD_REMOVE_AT")]
        public void TRACE_ADD_REMOVE(string fmt, params object[] args)
        {
        }
    }
}
