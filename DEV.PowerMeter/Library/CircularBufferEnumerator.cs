using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;


namespace DEV.PowerMeter.Library
{
    internal class CircularBufferEnumerator<T> : IEnumerator<T>, IDisposable, IEnumerator
    {
        protected bool AtBeginning;
        protected IList<T> DataBuffer;
        protected uint Count;
        protected uint DataOut;
        protected uint Index;

        protected uint MapIndex(uint index) => (this.DataOut + index) % (uint)this.DataBuffer.Count;

        public CircularBufferEnumerator(IList<T> buffer, uint count, uint dataOut)
        {
            this.DataBuffer = buffer;
            this.Count = count;
            this.DataOut = dataOut;
            this.Reset();
        }

        public void Reset() => this.AtBeginning = true;

        public bool MoveNext()
        {
            if (this.AtBeginning)
            {
                this.AtBeginning = false;
                this.Index = 0U;
            }
            else
                ++this.Index;
            return this.Index < this.Count;
        }

        public T Current => this.DataBuffer[(int)this.MapIndex(this.Index)];

        object IEnumerator.Current => (object)this.Current;

        void IDisposable.Dispose()
        {
        }

        [Conditional("TRACE_ENABLED")]
        public void TRACE(string fmt, params object[] args)
        {
        }
    }
}
